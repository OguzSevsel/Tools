using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Tools.SaveSystem
{
    public class SaveManager : MonoBehaviour
    {
        private static string SavePath(string slot) =>
            Path.Combine(Application.persistentDataPath, $"save_{slot}.json");

        public static void Save(string slot, int saveSlot)
        {
            // TODO: Prompt player to confirm overwrite if save already exists
            if (!SaveExists(slot))
            {
                var data = new GameData();
                data.metadata = new SaveMetadata
                {
                    slotName = slot,
                    lastSaved = DateTime.Now,
                    sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
                    saveSlot = saveSlot
                };

                foreach (var s in SaveableRegistry.GetAllSaveables())
                {
                    string id = s.GetUniqueId();
                    data.savedObjects[id] = s.CaptureState();
                }

                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Formatting = Formatting.Indented
                };

                string json = JsonConvert.SerializeObject(data, settings);

                File.WriteAllText(SavePath(slot), json);

                Debug.Log($"Game saved to slot '{slot}' at {SavePath(slot)}");
            }
        }

        public static void Load(string slot)
        {
            string path = SavePath(slot);
            if (!File.Exists(path))
            {
                Debug.LogWarning("No save file found for slot: " + slot);
                return;
            }

            string json = File.ReadAllText(path);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented
            };

            GameData data = JsonConvert.DeserializeObject<GameData>(json, settings);

            foreach (var saveable in SaveableRegistry.GetAllSaveables())
            {
                string id = saveable.GetUniqueId();
                if (data.savedObjects.TryGetValue(id, out object state))
                {
                    saveable.RestoreState(state);
                }
            }

            Debug.Log($"Game loaded from slot '{slot}' ({data.metadata.sceneName}, saved {data.metadata.lastSaved})");
        }

        public static void DeleteSave(string slot)
        {
            string path = SavePath(slot);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else
            {
                Debug.LogWarning("No save file found to delete for slot: " + slot);
            }
        }

        public static bool SaveExists(string slot) => File.Exists(SavePath(slot));
    } 
}
