using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API1.Models;
using System.Security.Cryptography;
using System.IO;

namespace API1.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {

        public static byte[] IV = new byte[] { 85, 169, 6, 99, 64, 46, 160, 36, 246, 225, 127, 84, 49, 94, 100, 7 };
        public static byte[] KEY = new byte[] { 30, 36, 100, 95, 23, 218, 211, 105, 225, 158, 4, 231, 123, 220, 30, 66, 76, 248, 204, 226, 169, 93, 82, 208, 101, 12, 248, 246, 53, 95, 53, 20 };

        public byte[] testPassword = new byte[] {224, 232, 143, 200, 187, 79, 248, 81, 40, 60, 195, 129, 10, 232, 187, 243};


        
        // GET api/values
        //testing to see if encryption works- testPassword was manually copied over from the front end
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { DecryptAES(testPassword, KEY, IV) };
        }
        


        [HttpGet("{input}")]
        public string ReturnUnencryptedPassword(string input)
        {
            return input;
        }
        
        /*
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get([FromBody] byte[] encryptedPassword)
        {
            if (encryptedPassword == null)
            {
                return new string[] { "value1", "value2" };
            }
            else
            {
                return new string[] { DecryptAES(encryptedPassword, KEY, IV) };
            }
        }
        */

        /*
        [HttpGet("{input}")]
        public string ReturnUnencryptedPassword(string input)
        {
            return input;
        }
        */


        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        public class Dto
        {
            public byte[] EncryptedPassword { get; set; }
        }

        // POST api/values
        //returns a 415 error
        [HttpPost]
        public string Post([FromBody]Dto dto)
        {            
            return DecryptAES(dto.EncryptedPassword, KEY, IV);
        }

        /*
        [HttpPost]
        public string Post([FromBody]System.Net.Http.StreamContent encryptedPassword)
        {
            byte[] password = encryptedPassword;
            return DecryptAES(encryptedPassword, KEY, IV);
        }
        */

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }


        public string DecryptAES(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
