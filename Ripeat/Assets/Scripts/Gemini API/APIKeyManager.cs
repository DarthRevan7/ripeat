using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.SceneManagement;
using GeminiAPI;

public class APIKeyManager : MonoBehaviour
{
    // Questa è la "Password" per criptare il file. 
    // Cambiala con una stringa casuale lunga!
    private const string SecretSalt = "furyd2473f29g429gpcmo4'ìfròèàè@qwc9-.,efvnup9834u904bda'shf"; 
    private string filePath;

    //Stringa API Key in chiaro, da inserire nel menu
    [SerializeField] private string apiKeyInput;

    void Awake()
    {
        // Il file verrà salvato in: C:/Users/Nome/AppData/LocalLow/NomeCompagnia/NomeGioco
        filePath = Path.Combine(Application.persistentDataPath, "gemini_enc.dat");
        string loadedKey = LoadKey();

        if(SceneManager.GetActiveScene().name.Equals("DialogueWithAI"))
        {
            // Se siamo nel menu, prova a caricare la chiave
            
            
            UnityAndGeminiV3 geminiManager = FindFirstObjectByType<UnityAndGeminiV3>();

            geminiManager.apiKey = loadedKey;
            Debug.Log("<color=blue>Chiave API caricata e assegnata a UnityAndGeminiV3!</color>");
            Debug.Log("Chiave decrittata: " + geminiManager.apiKey);
        }
        if(SceneManager.GetActiveScene().name.Equals("NewIntro"))
        {
            GeminiCore geminiCore = FindFirstObjectByType<GeminiCore>();

            if (geminiCore != null)
            {
                geminiCore.apiKey = loadedKey;
                Debug.Log("<color=blue>Chiave API caricata e assegnata a GeminiCore!</color>");
                Debug.Log("Chiave decrittata: " + geminiCore.apiKey);
            }
        }


    }

    public void SetApiKeyInput(string input)
    {
        apiKeyInput = input;
    }

    // Salva e cripta la chiave
    public void SaveKey()
    {
        try
        {
            string encryptedKey = Encrypt(apiKeyInput, SecretSalt);
            File.WriteAllText(filePath, encryptedKey);
            Debug.Log("<color=green>Chiave API salvata e criptata!</color>");
            Debug.Log("Chiave crittata: " + encryptedKey);
        }
        catch (Exception e)
        {
            Debug.LogError("Errore nel salvataggio: " + e.Message);
        }
    }

    // Recupera e decripta la chiave
    public string LoadKey()
    {
        if (!File.Exists(filePath)) return null;

        try
        {
            string encryptedKey = File.ReadAllText(filePath);
            return Decrypt(encryptedKey, SecretSalt);
        }
        catch (Exception e)
        {
            Debug.LogError("Errore nel caricamento (chiave corrotta?): " + e.Message);
            return null;
        }
    }

    #region Motore di Crittografia (AES)

    private string Encrypt(string clearText, string passphrase)
    {
        byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(passphrase, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                clearText = Convert.ToBase64String(ms.ToArray());
            }
        }
        return clearText;
    }

    private string Decrypt(string cipherText, string passphrase)
    {
        cipherText = cipherText.Replace(" ", "+");
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(passphrase, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }
        }
        return cipherText;
    }
    #endregion
}