using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using FrontEndApp1.Models;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Net;
using Newtonsoft.Json;

namespace FrontEndApp1.Controllers
{
    public class HomeController : Controller
    {

        static readonly HttpClient client = new HttpClient();
        public static byte[] IV = new byte[] {85, 169, 6, 99, 64, 46, 160, 36, 246, 225, 127, 84, 49, 94, 100, 7 };
        public static byte[] KEY = new byte[] {30, 36, 100, 95, 23, 218, 211, 105, 225, 158, 4, 231, 123, 220, 30, 66, 76, 248, 204, 226, 169, 93, 82, 208, 101, 12, 248, 246, 53, 95, 53, 20 };

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public async Task<IActionResult> SendInfoToBackEnd()
        {
            /*
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:51039/api/values");
            request.Content = new ByteArrayContent(EncryptStringToBytes_Aes(CreatePasswordModel().Password, KEY, IV));
            */

            string jsonPayload = JsonConvert.SerializeObject(new { EncryptedPassword = EncryptStringToBytes_Aes(CreatePasswordModel().Password, KEY, IV) });

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://localhost:51039/api/values"),
                Headers = {
                    { HttpRequestHeader.Authorization.ToString(), "Basic YXBpOmFwaQ==" },
                },
                Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
            };


            HttpResponseMessage response = await client.SendAsync(request);

            /*
{StatusCode: 415, ReasonPhrase: 'Unsupported Media Type', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  Date: Thu, 16 Apr 2020 00:14:35 GMT
  Server: Kestrel
  X-SourceFiles: =?UTF-8?B?YzpcdXNlcnNcdHNhbmp1bFxkb2N1bWVudHNcdmlzdWFsIHN0dWRpbyAyMDE3XFByb2plY3RzXEFQSTFcQVBJMVxhcGlcdmFsdWVz?=
  X-Powered-By: ASP.NET
  Content-Length: 0
}}
             
             */

            string responseBody = await response.Content.ReadAsStringAsync();

            Model1 returnedPassword = new Model1();
            returnedPassword.Password = responseBody;
            return View(returnedPassword);
        }

        public Model1 CreatePasswordModel()
        {
            string password = "P@ssword";

            return new Model1 { Password = password };
        }

        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
    }
}
