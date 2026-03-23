using UnityEngine;
using System.IO;


namespace GeminiAPI
{
    
    [System.Serializable]
    public class UnityAndGeminiKey
    {
        public string key;
    }

    public class GeminiCore : MonoBehaviour
    {
        public static GeminiCore Instance;

        [Header("Settings")]
        public string apiKey;
        public string apiEndpoint = "https://generativelanguage.googleapis.com/v1/models/gemini-3.1-flash:generateContent";

        void Awake()
        {
            // Singleton Pattern: resta vivo tra le scene
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadSettings();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadSettings()
        {
            string path = Path.Combine(Application.persistentDataPath, "gemini_config.json");

            if(apiEndpoint != null && apiEndpoint != "" && apiKey != null && apiKey != "")
            {
                Debug.Log("Configurazione già presente in memoria, non carico da file.");
                return;
            }

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                var data = JsonUtility.FromJson<GeminiAPI.UnityAndGeminiKey>(json);
                apiKey = data.key;
                // Se nel JSON c'è anche l'endpoint, carichiamo quello
                Debug.Log("Configurazione caricata correttamente.");
            }
            
        }

        public void SaveSettings(string newKey)
        {
            apiKey = newKey;
            string path = Path.Combine(Application.persistentDataPath, "gemini_config.json");
            File.WriteAllText(path, JsonUtility.ToJson(new GeminiAPI.UnityAndGeminiKey { key = newKey }));
        }
    }

}