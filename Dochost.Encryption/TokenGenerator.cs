using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Dochost.Encryption;

public record DecryptedUserFile
{
    public required string UserId { get; init; }
    public required string FileId { get; init; }
    public required string DateString { get; init; }
}

public static class TokenGenerator
{
    
    private static byte[] IV =
    {
        0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
        0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
    };
	
    private static byte[] DeriveKeyFromPassword(string password)
    {
        var emptySalt = Array.Empty<byte>();
        const int iterations = 1000;
        const int desiredKeyLength = 16; // 16 bytes equal 128 bits.
        var hashMethod = HashAlgorithmName.SHA384;
        return Rfc2898DeriveBytes.Pbkdf2(Encoding.Unicode.GetBytes(password),
            emptySalt,
            iterations,
            hashMethod,
            desiredKeyLength);
    }

    private static async Task<string> EncryptAsync(string clearText, string passphrase)
    {
        using var aes = Aes.Create();
        aes.Key = DeriveKeyFromPassword(passphrase);
        aes.IV = IV;

        using MemoryStream output = new();
        await using CryptoStream cryptoStream = new(output, aes.CreateEncryptor(), CryptoStreamMode.Write);

        await cryptoStream.WriteAsync(Encoding.Unicode.GetBytes(clearText));
        await cryptoStream.FlushFinalBlockAsync();

        return Convert.ToBase64String(output.ToArray());
    }

    private static async Task<string> DecryptAsync(string encrypted, string passphrase)
    {
        var encryptedBytes = Convert.FromBase64String(encrypted);
        using var aes = Aes.Create();
        aes.Key = DeriveKeyFromPassword(passphrase);
        aes.IV = IV;

        using MemoryStream input = new(encryptedBytes);
        await using CryptoStream cryptoStream = new(input, aes.CreateDecryptor(), CryptoStreamMode.Read);

        using MemoryStream output = new();
        await cryptoStream.CopyToAsync(output);

        return Encoding.Unicode.GetString(output.ToArray());
    }
    
    public static async Task<string> EncryptUserFile(string userId, Guid fileId, string secret)
    {
        var dateString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff", CultureInfo.InvariantCulture);
        var source = $"{userId};{fileId};{dateString}";
        return await EncryptAsync(source, secret);
    }

    public static async Task<DecryptedUserFile> DecryptUserFile(string encrypted, string secret)
    {
        var decrypted = await DecryptAsync(encrypted, secret);
        var decryptedSplit = decrypted.Split(";");
        return new DecryptedUserFile
        {
            UserId = decryptedSplit[0],
            FileId = decryptedSplit[1],
            DateString = decryptedSplit[2]
        };
    }
}