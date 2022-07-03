namespace XORCipher;

public class XorCipher
{
    public static string GetRandomKey(int key, int length)
    {
        var gamma = string.Empty;
        var rnd = new Random(key);
        for (int i = 0; i < length; i++) gamma += (char)rnd.Next(35, 126);
        return gamma;
    }
    private string GetRepeatKey(string s, int n)
    {
        var r = s;
        while (r.Length < n) r += r;
        return r.Substring(0, n);
    }

    private string Cipher(string text, string secretKey)
    {
        var currentKey = GetRepeatKey(secretKey, text.Length);
        var res = string.Empty;
        for (var i = 0; i < text.Length; i++) res += (char)(text[i] ^ currentKey[i]);
        return res;
    }

    public string Encrypt(string plainText, string password) => Cipher(plainText, password);
    
    public string Decrypt(string encryptedText, string password) => Cipher(encryptedText, password);
}