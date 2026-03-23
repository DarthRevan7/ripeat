using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace GeminiAPI
{
    public static class GeminiRequester
    {
        public static IEnumerator SendRequest(string prompt, System.Action<string> onSuccess, System.Action<long> onError)
        {
            var core = GeminiCore.Instance;
            string url = $"{core.apiEndpoint}?key={core.apiKey}";

            // Costruzione JSON pulita
            string jsonData = "{\"contents\": [{\"parts\": [{\"text\": \"" + EscapeJson(prompt) + "\"}]}]}";
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

            using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
            {
                www.uploadHandler = new UploadHandlerRaw(jsonToSend);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[GeminiRequester] Errore {www.responseCode}: {www.error}");
                    onError?.Invoke(www.responseCode);
                }
                else
                {
                    Risposta response = JsonUtility.FromJson<Risposta>(www.downloadHandler.text);
                    if (response?.candidates?.Length > 0)
                    {
                        onSuccess?.Invoke(response.candidates[0].content.parts[0].text);
                    }
                }
            }
        }

        private static string EscapeJson(string text) => text.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}