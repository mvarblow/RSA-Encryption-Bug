using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ConsoleApp
{
    class Program
    {
        private static readonly string embeddedKeyName = "ConsoleApp.rsa-key.pfx";
        private static readonly string password = "P@sSwOrd";
        private static readonly string encryptedData =
            "3CmuEDG0Bok6ltMjX7V7mkxE9cHv5KSTFkc73Krd4H79yqDgDabMw6G5rYWApPGNtF155bC8IN1/s+r3M1AOFjmZmA0Q1pJU028/F2+KsMid/5yPf2Ra6C7285eVkrFpYCHeT6Os4qHw8TICBqVQxqhVPXPdWn+sTwu4P9Dr7wRCejR+tntZ81rvZcWNeBpP21DWFvpy0v6jvA04Gi46e6Zkt6Z00C/rO55T003C9axhpWlSOAtuZCDcPy3YtsLZl24LE/rY3p/Rejc5jc+mbPxZAPxtTJcDGE79MZ+cI9Sqy60RzkTQvPLlYXaTzIeNLVRSFhz7eXJa2P/Ntq+IiQ==";

        static void Main(string[] args)
        {
            if (args.Any(arg => arg.Equals("rsa", StringComparison.InvariantCultureIgnoreCase)))
            {
                DecryptUsingRsaCryptoServiceProvider();
                Console.WriteLine("Decrypted the data using RSACryptServiceProvider and cleared the resulting data buffer.");
            }

            if (args.Any(arg => arg.Equals("cng", StringComparison.InvariantCultureIgnoreCase)))
            {
                DecryptUsingRsaCng();
                Console.WriteLine("Decrypted the data using RSACng and cleared the resulting data buffer. Press any key to exit.");
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey(true);
        }

        private static void DecryptUsingRsaCryptoServiceProvider()
        {
            // Obtain the private key from the embedded RSA certificate to use for decryption
            var rsa = (RSACryptoServiceProvider)GetEmbeddedCertificate().PrivateKey;

            // Decrypt the cipher text and clear the resulting memory buffer
            DecryptAndClear(rsa);
        }

        private static void DecryptUsingRsaCng()
        {
            // Obtain the private key from the embedded RSA certificate to use for decryption
            var rsa = (RSACng)GetEmbeddedCertificate().GetRSAPrivateKey();

            // Decrypt the cipher text and clear the resulting memory buffer
            DecryptAndClear(rsa);
        }
        
        private static void DecryptAndClear(RSA rsa)
        {
            // Convert the base-64 version of the encrypted data to an byte array containing the cipher text
            byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

            // Decrypt the cipher text
            byte[] clearBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.Pkcs1);

            // Clear the buffer containing the clear text string. This clear text string should no longer be resident in memory.
            Array.Clear(clearBytes, 0, clearBytes.Length);
        }

        private static X509Certificate2 GetEmbeddedCertificate()
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedKeyName);
            Debug.Assert(stream != null, nameof(stream) + " != null");

            var bytes = new byte[stream.Length];
            X509Certificate2 cert;
            
            try
            {
                stream.Read(bytes, 0, bytes.Length);
                cert = new X509Certificate2(bytes, password);
            }
            finally
            {
                Array.Clear(bytes, 0, bytes.Length);
            }
            return cert;
        }

        #if false

        // This method was used to generate the encrypted data. Example usage:
        //   string encryptedData = Encrypt("2223000010309703");
        private static string Encrypt(string clearText)
        {
            byte[] clearBytes = Encoding.UTF8.GetBytes(clearText);
            X509Certificate2 cert = GetCertificate(rsaKeyName, certPassword);
            var rsa = (RSACryptoServiceProvider)cert.PublicKey.Key;
            byte[] encryptedBytes = rsa.Encrypt(clearBytes, RSAEncryptionPadding.Pkcs1);
            return Convert.ToBase64String(encryptedBytes);
        }

        #endif
    }
}
