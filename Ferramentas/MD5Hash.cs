using System;
using System.Security.Cryptography;
using System.Text;

namespace Ferramentas
{
    public static class MD5Hash
    {
        public static string CalculaHash(string senha)
        {
            try
            {
                MD5 md5 = MD5.Create();
                byte[] inputBytes = Encoding.ASCII.GetBytes(senha);
                byte[] hash = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
                return sb.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
