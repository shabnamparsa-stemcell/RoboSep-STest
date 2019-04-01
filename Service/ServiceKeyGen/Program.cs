using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Collections;

namespace ServiceKeyGen
{
    class Program
    {
        #region password encrypt
        public static string EncryptString(string inputString, int dwKeySize,
                             string xmlString)
        {
            // TODO: Add Proper Exception Handlers
            RSACryptoServiceProvider rsaCryptoServiceProvider =
                                          new RSACryptoServiceProvider(dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(xmlString);
            int keySize = dwKeySize / 8;
            byte[] bytes = Encoding.UTF32.GetBytes(inputString);
            // The hash function in use by the .NET RSACryptoServiceProvider here 
            // is SHA1
            // int maxLength = ( keySize ) - 2 - 
            //              ( 2 * SHA1.Create().ComputeHash( rawBytes ).Length );
            int maxLength = keySize - 42;
            int dataLength = bytes.Length;
            int iterations = dataLength / maxLength;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i <= iterations; i++)
            {
                byte[] tempBytes = new byte[
                        (dataLength - maxLength * i > maxLength) ? maxLength :
                                                      dataLength - maxLength * i];
                Buffer.BlockCopy(bytes, maxLength * i, tempBytes, 0,
                                  tempBytes.Length);
                byte[] encryptedBytes = rsaCryptoServiceProvider.Encrypt(tempBytes,
                                                                          true);
                // Be aware the RSACryptoServiceProvider reverses the order of 
                // encrypted bytes. It does this after encryption and before 
                // decryption. If you do not require compatibility with Microsoft 
                // Cryptographic API (CAPI) and/or other vendors. Comment out the 
                // next line and the corresponding one in the DecryptString function.
                Array.Reverse(encryptedBytes);
                // Why convert to base 64?
                // Because it is the largest power-of-two base printable using only 
                // ASCII characters
                stringBuilder.Append(Convert.ToBase64String(encryptedBytes));
            }
            return stringBuilder.ToString();
        }

        public static string DecryptString(string inputString, int dwKeySize,
                                     string xmlString)
        {
            // TODO: Add Proper Exception Handlers
            RSACryptoServiceProvider rsaCryptoServiceProvider
                                     = new RSACryptoServiceProvider(dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(xmlString);
            int base64BlockSize = ((dwKeySize / 8) % 3 != 0) ?
              (((dwKeySize / 8) / 3) * 4) + 4 : ((dwKeySize / 8) / 3) * 4;
            int iterations = inputString.Length / base64BlockSize;
            ArrayList arrayList = new ArrayList();
            for (int i = 0; i < iterations; i++)
            {
                byte[] encryptedBytes = Convert.FromBase64String(
                     inputString.Substring(base64BlockSize * i, base64BlockSize));
                // Be aware the RSACryptoServiceProvider reverses the order of 
                // encrypted bytes after encryption and before decryption.
                // If you do not require compatibility with Microsoft Cryptographic 
                // API (CAPI) and/or other vendors.
                // Comment out the next line and the corresponding one in the 
                // EncryptString function.
                Array.Reverse(encryptedBytes);
                arrayList.AddRange(rsaCryptoServiceProvider.Decrypt(
                                    encryptedBytes, true));
            }
            return Encoding.UTF32.GetString(arrayList.ToArray(
                                      Type.GetType("System.Byte")) as byte[]);
        }
        #endregion
        static void Main(string[] args)
        {
            string key = "<RSAKeyValue><Modulus>yXCnyqwaxlf9hwy62VF3leHhjS7I1QKp5dlQTtSDM4rSSyyDfseXTHgB1c3Nwb3h</Modulus><Exponent>AQAB</Exponent><P>/+n6AIKzciaOLbxO3W9fWSa1GKiQxJf3</P><Q>yYH9rx44N1CExe02hRx+sZQCoaSw7NLn</Q><DP>TSEPoCfEPZsxLseaXVK7wfrQieYD+7xx</DP><DQ>h8X8pofYHP012R7yQ1Jl00UFWODdDVU1</DQ><InverseQ>djPYkO3Tb4rLvfmENfX8n0OzVW/QIop2</InverseQ><D>Rw3q0c2lYCM3dYSi//cBlKfplJBVHPXj8KsNYJKlOHpAuxCkq7bRYQPw+QR942A1</D></RSAKeyValue>";
            //int keysize = 384;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(384);
            rsa.FromXmlString(key);
            int keysize = rsa.KeySize;

            if (args.Length != 1)
            {
                Console.WriteLine("Please provide exactly 1 argument in the format username:password:level(1-3) ");
                Console.WriteLine("e.g. stemcell:stemcell:3 ");
                return;
            }

            string encString = EncryptString(args[0], keysize, key);
            string decString = DecryptString(encString, keysize, key);

            Console.WriteLine(encString);

            return;
        }
    }
}
