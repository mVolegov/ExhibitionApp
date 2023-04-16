using System.Security.Cryptography;
using System.Text;

namespace ExhibitionApp.Utils
{
    public class PasswordHash
    {
        public static IConfiguration? Configuration;

        public static string GetHash(char[] password)
        {
            using SHA256 crypt = SHA256.Create();
            var hash = new StringBuilder();
            int iter = 2;
            var saltArr = "12qwASzx".ToCharArray();
            if (Configuration != null)
            {
                iter = int.Parse(Configuration["hashIterations"]!);
                saltArr = Configuration["passwordSalt"]!.ToCharArray();
            }
            var newCharArr = new char[password.Length + saltArr.Length];
            Array.Copy(password, newCharArr, password.Length);
            Array.Copy(saltArr, 0, newCharArr, password.Length, saltArr.Length);
            byte[] crypto = Encoding.UTF8.GetBytes(newCharArr);
            for (int i = 0; i < iter; i++)
            {
                crypto = crypt.ComputeHash(crypto);
            }

            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }

            return hash.ToString();
        }
    }
}
