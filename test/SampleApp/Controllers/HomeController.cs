using System;
using Microsoft.AspNetCore.Mvc;
using SampleApp;

namespace SampleApp.Controllers
{
    [ApiController]
    [Route("/")]
    public class HomeController : ControllerBase
    {
        private readonly EncryptionService _encryptionService;

        public HomeController(EncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        [HttpGet]
        public string Get()
        {
            return "Welcome.";
        }

        [HttpGet]
        [Route("encrypt")]
        public string Encrypt(string input)
        {
            return _encryptionService.Encrypt(input);
        }

        [HttpGet]
        [Route("decrypt")]
        public string Decrypt(string input)
        {
            return _encryptionService.Decrypt(input);
        }
    }
}
