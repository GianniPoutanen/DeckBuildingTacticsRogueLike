using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class SecureJsonManager : MonoBehaviour
{
    private static string secretKey = "yourSecretKey"; // Change this to your secret key

    // Example: Save encrypted JSON data
    public static void SaveEncryptedJsonData<T>(JsonDataType dataType, T data)
    {
        string fileName = GetFileName(dataType);
        string jsonData = JsonUtility.ToJson(data);
        string encryptedData = Encrypt(jsonData);
        PlayerPrefs.SetString(fileName, encryptedData);
        PlayerPrefs.Save();
    }

    // Example: Load encrypted JSON data
    public static T LoadEncryptedJsonData<T>(JsonDataType dataType)
    {
        string fileName = GetFileName(dataType);
        string encryptedData = PlayerPrefs.GetString(fileName);
        string jsonData = Decrypt(encryptedData);
        return JsonUtility.FromJson<T>(jsonData);
    }

    private static string GetFileName(JsonDataType dataType)
    {
        // Convert the enum to a string or use a switch statement for more complex scenarios
        return dataType.ToString();
    }

    private static string Encrypt(string data)
    {
        using (AesManaged aesAlg = new AesManaged())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(secretKey);
            aesAlg.IV = new byte[16]; // Use a random IV for additional security

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(data);
                    }
                }

                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    private static string Decrypt(string encryptedData)
    {
        using (AesManaged aesAlg = new AesManaged())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(secretKey);
            aesAlg.IV = new byte[16]; // Use the same IV used during encryption

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedData)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}
