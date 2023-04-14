using System.Security.Cryptography;
using System.Text;

public class Aes256Provider
{
    private static readonly string _key = "gUkXp2s5u8x/A?D(G+KbPeShVmYq3t6w";
    private static readonly string _IV = "HR$2pIjHR$2pIj12";

    public static string Decrypt(string input)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = Encoding.UTF8.GetBytes(_key);
        aesAlg.IV = Encoding.UTF8.GetBytes(_IV);

        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
        var cipherText = Convert.FromBase64String(input);

        using MemoryStream msDecrypt = new MemoryStream(cipherText);
        using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

        using StreamReader srDecrypt = new StreamReader(csDecrypt);
        return srDecrypt.ReadToEnd();
    }

    public static string Encrypt(string input)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = Encoding.UTF8.GetBytes(_key);
        aesAlg.IV = Encoding.UTF8.GetBytes(_IV);

        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using MemoryStream msEncrypt = new MemoryStream();
        using CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(input);
        }
        return Convert.ToBase64String(msEncrypt.ToArray());
    }
}