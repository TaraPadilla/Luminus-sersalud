// TokenController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace farmamest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly byte[] Key = Encoding.UTF8.GetBytes("f4d8e7a3c2b9a0d1e4f5c6b7a8d9e0f1"); // 16 bytes for AES-128

        public TokenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var token = _configuration["MultiAPI_Token"];
            if (string.IsNullOrEmpty(token))
            {
                return NotFound("Token no configurado");
            }

            // Encrypt the token
            var encryptedToken = Encrypt(token, Key);
            return Ok(new { token = encryptedToken });
        }

        private string Encrypt(string plainText, byte[] key)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();
            var iv = aes.IV;
            
            using var encryptor = aes.CreateEncryptor(aes.Key, iv);
            using var ms = new MemoryStream();
            ms.Write(iv, 0, iv.Length); // Prepend IV
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }
            return Convert.ToBase64String(ms.ToArray());
        }
    }
}
