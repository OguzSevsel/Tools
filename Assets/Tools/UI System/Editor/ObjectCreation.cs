namespace Tools
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [ExecuteInEditMode()]
    public static class ObjectCreation
    {
        #region UI Elements

        [MenuItem("GameObject/Tools/UI/Button", false, priority = 0, secondaryPriority = 0)]
        public static void CreateButtonElement(MenuCommand menuCommand)
        {
            CreateUIElement(menuCommand, "Button Element");
        }

        [MenuItem("GameObject/Tools/UI/Panel", false, priority = 0, secondaryPriority = 1)]
        public static void CreatePanelElement(MenuCommand menuCommand)
        {
            CreateUIElement(menuCommand, "Panel Element");
        }

        [MenuItem("GameObject/Tools/UI/Text", false, priority = 0, secondaryPriority = 2)]
        public static void CreateTextElement(MenuCommand menuCommand)
        {
            CreateUIElement(menuCommand, "Text Element");
        }

        [MenuItem("GameObject/Tools/UI/Dropdown", false, priority = 0, secondaryPriority = 6)]
        public static void CreateDropdownElement(MenuCommand menuCommand)
        {
            CreateUIElement(menuCommand, "Dropdown Element");
        }

        [MenuItem("GameObject/Tools/UI/Input Field", false, priority = 0, secondaryPriority = 5)]
        public static void CreateInputFieldElement(MenuCommand menuCommand)
        {
            CreateUIElement(menuCommand, "Input Field Element");
        }

        [MenuItem("GameObject/Tools/UI/Slider", false, priority = 0, secondaryPriority = 4)]
        public static void CreateSliderElement(MenuCommand menuCommand)
        {
            CreateUIElement(menuCommand, "Slider Element");
        }

        [MenuItem("GameObject/Tools/UI/Toggle", false, priority = 0, secondaryPriority = 3)]    
        public static void CreateToggleElement(MenuCommand menuCommand)
        {
            CreateUIElement(menuCommand, "Toggle Element");
        }

        [MenuItem("GameObject/Tools/UI/Progress Bars/Rounded Progress Bar", false, priority = 1, secondaryPriority = 0)]
        public static void CreateRoundedProgressBarElement(MenuCommand menuCommand)
        {
            CreateUIElement(menuCommand, "Rounded Progress Bar Element");
        }

        [MenuItem("GameObject/Tools/UI/Progress Bars/Rectangle Progress Bar", false, priority =  1, secondaryPriority = 0)]
        public static void CreateRectangleProgressBarElement(MenuCommand menuCommand)
        {
            CreateUIElement(menuCommand, "Rectangle Progress Bar Element");
        }

        [MenuItem("GameObject/Tools/UI/Progress Bars/Radial Progress Bar", false, priority = 1, secondaryPriority = 0)]
        public static void CreateRadialProgressBarElement(MenuCommand menuCommand)
        {
            CreateUIElement(menuCommand, "Radial Progress Bar Element");
        }

        [MenuItem("GameObject/Tools/UI/Progress Bars/Radial Progress Bar Iconed", false, priority = 1, secondaryPriority = 0)]
        public static void CreateRadialIconedProgressBarElement(MenuCommand menuCommand)
        {
            CreateUIElement(menuCommand, "Radial Progress Bar Iconed Element");
        }

        #endregion

        #region Managers and Systems

        [MenuItem("GameObject/Tools/Menu Navigation Manager", false, priority = 0)]
        public static void CreateMenuNavigation(MenuCommand menuCommand)
        {
            CreateUIElement(menuCommand, "Menu Navigation Element");
        }

        [MenuItem("GameObject/Tools/Audio Manager", false, priority = 0)]
        public static void CreateAudioManager(MenuCommand menuCommand)
        {
            CreateWorldElement(menuCommand, "Audio Manager Element");
        }

        [MenuItem("GameObject/Tools/Tooltip System", false, priority = 0)]
        public static void CreateToolTipElement(MenuCommand menuCommand)
        {
            CreateWorldElement(menuCommand, "Tooltip Element");
        }

        [MenuItem("GameObject/Tools/Auto Tag System", false, priority = 0)]
        public static void CreateAutoTag(MenuCommand menuCommand)
        {
            CreateWorldElement(menuCommand, "Auto Tag System Element");
        }

        [MenuItem("GameObject/Tools/Dialogue System", false, priority = 0)]
        public static void CreateDialogueElement(MenuCommand menuCommand)
        {
            CreateUIElement(menuCommand, "Dialogue Element");
            CreateWorldElement(menuCommand, "Dialogue Manager Element");
        }

        [MenuItem("GameObject/Tools/Save System", false, priority = 0)]
        public static void CreateSaveElement(MenuCommand menuCommand)
        {
            CreateWorldElement(menuCommand, "Save Manager Element");
        }

        #endregion

        #region Gameplay Elements

        [MenuItem("GameObject/Tools/Gameplay/Camera", false, priority = 0)]
        public static void CreateCameraElement(MenuCommand menuCommand)
        {
            CreateWorldElement(menuCommand, "Camera Element");
        }

        [MenuItem("GameObject/Tools/Gameplay/World Element", false, priority = 0)]
        public static void CreateWorldElement(MenuCommand menuCommand)
        {
            CreateWorldElement(menuCommand, "World Element");
        }

        #endregion

        #region Utils

        private static void CreateUIElement(MenuCommand menuCommand, string elementName)
        {
            GameObject parent = menuCommand.context as GameObject;

            GameObject canvasGO = GetOrCreateCanvas();

            var prefab = LoadPrefab(elementName);

            if (prefab == null)
            {
                Debug.LogError($"{elementName} prefab not found.");
                return;
            }

            GameObject instance =
                (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            PrefabUtility.UnpackPrefabInstance(
                instance,
                PrefabUnpackMode.Completely,
                InteractionMode.UserAction
            );

            if (parent != null && parent.GetComponentInParent<Canvas>() != null)
            {
                GameObjectUtility.SetParentAndAlign(instance, parent);
            }
            else
            {
                GameObjectUtility.SetParentAndAlign(instance, canvasGO);
            }

            Undo.RegisterCreatedObjectUndo(instance, $"Create {elementName} Element");
            Selection.activeGameObject = instance;

            EditorSceneManager.MarkSceneDirty(instance.scene);
        }

        private static GameObject LoadPrefab(string elementName)
        {
            string[] guids = AssetDatabase.FindAssets($"{elementName} t:Prefab");
            if (guids.Length == 0)
            {
                Debug.LogError($"{elementName} prefab not found.");
                return null;
            }
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }

        private static void CreateWorldElement(MenuCommand menuCommand, string elementName)
        {
            GameObject parent = menuCommand.context as GameObject;

            var prefab = LoadPrefab(elementName);

            if (prefab == null)
            {
                Debug.LogError($"{elementName} prefab not found.");
                return;
            }

            GameObject instance =
                (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            PrefabUtility.UnpackPrefabInstance(
                instance,
                PrefabUnpackMode.Completely,
                InteractionMode.UserAction
            );

            GameObjectUtility.SetParentAndAlign(instance, parent);

            Undo.RegisterCreatedObjectUndo(instance, $"Create {elementName} Element");
            Selection.activeGameObject = instance;

            EditorSceneManager.MarkSceneDirty(instance.scene);
        }

        private static GameObject GetOrCreateCanvas()
        {
            Canvas canvas = Object.FindAnyObjectByType<Canvas>();

            if (canvas != null)
                return canvas.gameObject;

            GameObject canvasGO = new GameObject("Canvas");
            Undo.RegisterCreatedObjectUndo(canvasGO, "Create Canvas");

            canvasGO.layer = LayerMask.NameToLayer("UI");

            Canvas c = canvasGO.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;

            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            if (Object.FindAnyObjectByType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                Undo.RegisterCreatedObjectUndo(eventSystem, "Create EventSystem");

                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }

            return canvasGO;
        }

        #endregion
    }
#endif
}