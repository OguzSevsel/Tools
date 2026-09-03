using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using static System.Net.WebRequestMethods;
using Debug = UnityEngine.Debug;

/// <summary>
/// Editor tool that:
/// 1) Installs a predefined/editable list of Unity packages (your own git-hosted
///    packages, or third-party/OpenUPM-style packages via a scoped registry).
/// 2) Initializes the current project folder as a git repo, creates a GitHub
///    repo via the GitHub CLI (gh), and pushes the initial commit.
///
/// Requirements:
/// - git must be installed and on PATH.
/// - gh (GitHub CLI) must be installed and authenticated ("gh auth login")
///   for the repo-creation step. This avoids storing a personal access token
///   inside the Unity project or EditorPrefs.
/// </summary>
public class ProjectInitiatorWindow : EditorWindow
{
    // ---- Package installation state ----
    private List<string> packageUrls = new List<string>
    {
        "https://github.com/yourname/your-package.git",
        "https://github.com/LoveFloodGames/LoveFloodPackages.git?path=/DialogueSystem"

    };

    private bool addOpenUpmRegistry = false;
    private string openUpmScope = "com.yourcompany";
    private string thirdPartyPackageName = "com.openupm.example-package";

    // ---- Git / GitHub state ----
    private string githubOwner = "";
    private string repoName = "";
    private bool isPrivate = true;
    private string commitMessage = "Initial commit";

    private AddRequest currentAddRequest;
    private readonly Queue<string> pendingPackages = new Queue<string>();

    [MenuItem("Tools/Project Initiator")]
    public static void ShowWindow()
    {
        GetWindow<ProjectInitiatorWindow>("Project Initiator");
    }

    private void OnGUI()
    {
        GUILayout.Label("1. Install Packages", EditorStyles.boldLabel);

        for (int i = 0; i < packageUrls.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            packageUrls[i] = EditorGUILayout.TextField(packageUrls[i]);
            if (GUILayout.Button("-", GUILayout.Width(24)))
            {
                packageUrls.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("+ Add Package Git URL"))
            packageUrls.Add("");

        if (GUILayout.Button("Install All Packages"))
            InstallPackages();

        GUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "For third-party packages not distributed via git (e.g. OpenUPM), " +
            "add a scoped registry to manifest.json instead of using Client.Add().",
            MessageType.Info);

        addOpenUpmRegistry = EditorGUILayout.Toggle("Add OpenUPM Scoped Registry", addOpenUpmRegistry);
        if (addOpenUpmRegistry)
        {
            openUpmScope = EditorGUILayout.TextField("Scope (e.g. com.yourcompany)", openUpmScope);
            thirdPartyPackageName = EditorGUILayout.TextField("Package Name To Add After", thirdPartyPackageName);
            if (GUILayout.Button("Add Scoped Registry + Install Package"))
                AddScopedRegistryAndInstall();
        }

        GUILayout.Space(20);
        GUILayout.Label("2. Git / GitHub Setup", EditorStyles.boldLabel);

        githubOwner = EditorGUILayout.TextField("GitHub Owner/Org", githubOwner);
        repoName = EditorGUILayout.TextField("Repo Name", repoName);
        isPrivate = EditorGUILayout.Toggle("Private Repo", isPrivate);
        commitMessage = EditorGUILayout.TextField("Initial Commit Message", commitMessage);

        EditorGUILayout.HelpBox(
            "Requires git + GitHub CLI (gh) installed, and 'gh auth login' already run once.",
            MessageType.Info);

        if (GUILayout.Button("Init Git + Create GitHub Repo + Push"))
            InitGitAndPush();
    }

    // ---------------- Package installation ----------------

    private void InstallPackages()
    {
        pendingPackages.Clear();
        foreach (var url in packageUrls)
            if (!string.IsNullOrWhiteSpace(url))
                pendingPackages.Enqueue(url);

        EditorApplication.update += ProcessQueue;
    }

    private void ProcessQueue()
    {
        if (currentAddRequest != null)
        {
            if (!currentAddRequest.IsCompleted) return;

            if (currentAddRequest.Status == StatusCode.Success)
                Debug.Log($"[ProjectInitiator] Installed: {currentAddRequest.Result.packageId}");
            else if (currentAddRequest.Status >= StatusCode.Failure)
                Debug.LogError($"[ProjectInitiator] Install failed: {currentAddRequest.Error.message}");

            currentAddRequest = null;
        }

        if (currentAddRequest == null)
        {
            if (pendingPackages.Count == 0)
            {
                EditorApplication.update -= ProcessQueue;
                return;
            }
            currentAddRequest = Client.Add(pendingPackages.Dequeue());
        }
    }

    private void AddScopedRegistryAndInstall()
    {
        string manifestPath = Path.Combine(Application.dataPath, "..", "Packages", "manifest.json");
        manifestPath = Path.GetFullPath(manifestPath);

        if (!System.IO.File.Exists(manifestPath))
        {
            Debug.LogError("[ProjectInitiator] manifest.json not found.");
            return;
        }

        string json = System.IO.File.ReadAllText(manifestPath);

        // Minimal, dependency-free scoped registry insertion.
        // For anything more complex, consider a proper JSON library (e.g. Newtonsoft.Json).
        if (!json.Contains("\"scopedRegistries\""))
        {
            string registryBlock =
                "\"scopedRegistries\": [\n" +
                "    {\n" +
                "      \"name\": \"OpenUPM\",\n" +
                "      \"url\": \"https://package.openupm.com\",\n" +
                "      \"scopes\": [\"" + openUpmScope + "\"]\n" +
                "    }\n" +
                "  ],\n  ";

            int insertAt = json.IndexOf('{') + 1;
            json = json.Insert(insertAt, "\n  " + registryBlock);
            System.IO.File.WriteAllText(manifestPath, json);
            AssetDatabase.Refresh();
            Debug.Log("[ProjectInitiator] Added OpenUPM scoped registry.");
        }

        if (!string.IsNullOrWhiteSpace(thirdPartyPackageName))
        {
            pendingPackages.Enqueue(thirdPartyPackageName);
            EditorApplication.update += ProcessQueue;
        }
    }

    // ---------------- Git / GitHub ----------------

    private void InitGitAndPush()
    {
        string projectPath = Directory.GetParent(Application.dataPath).FullName;

        if (!Directory.Exists(Path.Combine(projectPath, ".git")))
            RunCommand("git", "init", projectPath);

        WriteGitignoreIfMissing(projectPath);

        RunCommand("git", "add .", projectPath);
        RunCommand("git", $"commit -m \"{commitMessage}\"", projectPath);
        RunCommand("git", "branch -M main", projectPath);

        string visibility = isPrivate ? "--private" : "--public";
        RunCommand("gh",
            $"repo create {githubOwner}/{repoName} {visibility} --source=. --remote=origin",
            projectPath);

        RunCommand("git", "push -u origin main", projectPath);
    }

    private void WriteGitignoreIfMissing(string projectPath)
    {
        string gitignorePath = Path.Combine(projectPath, ".gitignore");
        if (System.IO.File.Exists(gitignorePath)) return;

        string content =
            "Library/\n" +
            "Temp/\n" +
            "Obj/\n" +
            "Build/\n" +
            "Builds/\n" +
            "Logs/\n" +
            "UserSettings/\n" +
            "MemoryCaptures/\n" +
            ".vs/\n" +
            ".vscode/\n" +
            ".idea/\n" +
            "*.csproj\n" +
            "*.sln\n" +
            "*.suo\n" +
            "" +
            "*.user\n" +
            "*.pidb\n" +
            "*.booproj\n" +
            "*.svd\n" +
            "*.pdb\n" +
            "*.mdb\n" +
            "*.opendb\n" +
            "*.VC.db\n" +
            "sysinfo.txt\n" +
            "crashlytics-build.properties\n";

        System.IO.File.WriteAllText(gitignorePath, content);
    }

    private void RunCommand(string command, string args, string workingDir)
    {
        var psi = new ProcessStartInfo
        {
            FileName = command,
            Arguments = args,
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using var process = Process.Start(psi);
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(output))
                Debug.Log($"[{command}] {output}");
            if (!string.IsNullOrEmpty(error))
                Debug.LogWarning($"[{command}] {error}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ProjectInitiator] Failed to run '{command} {args}': {e.Message}");
        }
    }
}