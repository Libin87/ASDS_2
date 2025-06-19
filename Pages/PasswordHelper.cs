using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;


public static class PasswordHelper
{
    public static string HashPassword(string password)
    {
        byte[] salt;
        byte[] buffer2;
        using (var bytes = new Rfc2898DeriveBytes(password, 16, 10000))
        {
            salt = bytes.Salt;
            buffer2 = bytes.GetBytes(32);
        }
        byte[] dst = new byte[49];
        Buffer.BlockCopy(salt, 0, dst, 1, 16);
        Buffer.BlockCopy(buffer2, 0, dst, 17, 32);
        return Convert.ToBase64String(dst);
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        byte[] src = Convert.FromBase64String(hashedPassword);
        byte[] salt = new byte[16];
        Buffer.BlockCopy(src, 1, salt, 0, 16);
        byte[] storedSubkey = new byte[32];
        Buffer.BlockCopy(src, 17, storedSubkey, 0, 32);

        using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, 10000))
        {
            byte[] generatedSubkey = deriveBytes.GetBytes(32);
            return storedSubkey.SequenceEqual(generatedSubkey);
        }
    }
}
