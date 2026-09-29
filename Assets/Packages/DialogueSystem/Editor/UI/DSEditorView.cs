using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Utilities;
using UnityEditor;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSEditorView : EditorWindow
    {
        DSGraphTab graphTab;

        [MenuItem("Tools/Dialogue Graph")]
        public static void Open()
        {
            var window = GetWindow<DSEditorView>();
        }

        private void OnEnable()
        {
            rootVisualElement.style.flexGrow = 1;
            rootVisualElement.style.justifyContent = Justify.Center;

            graphTab = new DSGraphTab();

            rootVisualElement.Add(graphTab);

            DSIOUtility.CreateFolderIfNotExists("Assets", "DialogueSystem");
            DSIOUtility.CreateFolderIfNotExists("Assets/DialogueSystem", "Conversations");
            DSIOUtility.CreateFolderIfNotExists("Assets/DialogueSystem", "Databases");

            AddStyles();
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("DialogueSystem/DSVariables.uss");
        }
    }
}
