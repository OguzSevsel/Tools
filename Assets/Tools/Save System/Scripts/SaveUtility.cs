using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Elements;

namespace Tools.SaveSystem
{
    public class SaveUtility : MonoBehaviour
    {
        public InputFieldElement saveNameInputField;
        public ButtonElement saveButton1;
        public ButtonElement saveButton2;
        public ButtonElement saveButton3;
        public ButtonElement loadButton1;
        public ButtonElement loadButton2;
        public ButtonElement loadButton3;
        public ButtonElement deleteButton1;
        public ButtonElement deleteButton2;
        public ButtonElement deleteButton3;
        public TextElement saveSlotText1;
        public TextElement saveSlotText2;
        public TextElement saveSlotText3;
        public GameObject savePanel;

        public static SaveUtility Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            IEnumerable<GameData> saveFiles = GetAllSaves();

            foreach (var save in saveFiles)
            {
                if (save.metadata.saveSlot == 1)
                {
                    saveSlotText1.SetText(save.metadata.slotName);
                }

                if (save.metadata.saveSlot == 2)
                {
                    saveSlotText2.SetText(save.metadata.slotName);
                }

                if (save.metadata.saveSlot == 3)
                {
                    saveSlotText3.SetText(save.metadata.slotName);
                }
            }
            saveButton1.OnMouseClick += (data) =>
            {
                SaveManager.Save(saveNameInputField.Text, 1);
                saveSlotText1.SetText(saveNameInputField.Text);
            };

            saveButton2.OnMouseClick += (data) =>
            {
                SaveManager.Save(saveNameInputField.Text, 2);
                saveSlotText2.SetText(saveNameInputField.Text);
            };

            saveButton3.OnMouseClick += (data) =>
            {
                SaveManager.Save(saveNameInputField.Text, 3);
                saveSlotText3.SetText(saveNameInputField.Text);
            };

            deleteButton1.OnMouseClick += (data) =>
            {
                SaveManager.DeleteSave(saveSlotText1.Text.text);
                saveSlotText1.SetText("Save Slot 1");
            };

            deleteButton2.OnMouseClick += (data) =>
            {
                SaveManager.DeleteSave(saveSlotText2.Text.text);
                saveSlotText2.SetText("Save Slot 2");
            };

            deleteButton3.OnMouseClick += (data) =>
            {
                SaveManager.DeleteSave(saveSlotText3.Text.text);
                saveSlotText3.SetText("Save Slot 3");
            };

            loadButton1.OnMouseClick += (data) => SaveManager.Load(saveSlotText1.Text.text);
            loadButton2.OnMouseClick += (data) => SaveManager.Load(saveSlotText2.Text.text);
            loadButton3.OnMouseClick += (data) => SaveManager.Load(saveSlotText3.Text.text);
        }

        public static IEnumerable<GameData> GetAllSaves()
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath, "save_*.json");
            foreach (string file in files)
            {
                string json = File.ReadAllText(file);

                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Formatting = Formatting.Indented
                };

                GameData data = JsonConvert.DeserializeObject<GameData>(json, settings);

                if (data != null) yield return data;
            }
        }
    } 
}
