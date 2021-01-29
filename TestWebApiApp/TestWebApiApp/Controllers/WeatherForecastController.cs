using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MimeMapping;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace TestWebApiApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private IConfiguration _iConfiguration;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration iConfiguration)
        {
            _logger = logger;
            _iConfiguration = iConfiguration;
        }


        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var myOptions = _iConfiguration.GetSection("JWTSettings").Get<JWTSettings>();

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }


        [AllowAnonymous]
        [HttpGet("GetPhysicalFile")]
        public IActionResult GetPhysicalFile()
        {
            string path = @"C:\Users\skeswani\source\repos\aspnet-ej1-demos\App_Data\PdfViewer\Linearized.pdf";
            return new PhysicalFileResult(path, "application/pdf");
        }


        [AllowAnonymous]
        [HttpGet("GetFileResponse")]
        public HttpResponseMessage GetFileResponse(string id)
        {

            string localFilePath = @"C:\Users\skeswani\source\repos\aspnet-ej1-demos\App_Data\PdfViewer\Linearized.pdf";


            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(localFilePath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = "Test File";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            return response;
        }

        //[EnableCors()]
        [AllowAnonymous]
        [HttpGet("GetPdfFile")]
        //public async Task<HttpResponseMessage> GetPdfFileAsync()
        public ActionResult GetPdfFileAsync()
        {
            //string url = string.Empty;
            //var client = new HttpClient();
            //client.DefaultRequestHeaders.Range = new RangeHeaderValue(0, 600000);
            //using (var stream = await client.GetStreamAsync(url))
            //using (var output = File.Create(@"C:\Videofile.pm4"))
            //{
            //    await stream.CopyToAsync(output);
            //}
            var fileName = "PDFToolsLinearized.pdf";
            string pdfPath = @"C:\Users\skeswani\Downloads\pdf\RQAPDF\Linearized pdf\PDFToolsLinearized.pdf";
            return PhysicalFile(pdfPath, MimeUtility.GetMimeMapping(fileName), fileName, true);


        }


        [AllowAnonymous]
        [HttpGet("GetFileInByte")]
        public FileResult GetFileInByte()
        {
            string PDFpath = @"C:\Users\skeswani\source\repos\aspnet-ej1-demos\App_Data\PdfViewer\Linearized.pdf";
            byte[] abc = System.IO.File.ReadAllBytes(PDFpath);
            System.IO.File.WriteAllBytes(PDFpath, abc);
            MemoryStream ms = new MemoryStream(abc);
            return new FileStreamResult(ms, "application/pdf");
        }

        [AllowAnonymous]
        [HttpGet("login")]
        public string GetToken(string name, string password)
        {
            //just hard code here.  
            if (name == "cp" && password == "123")
            {
                var now = DateTime.UtcNow;

                var claims = new Claim[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(), ClaimValueTypes.Integer64)
                };

                var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("this is the secret key to add some default jwt token, lets see how it works"));
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = true,
                    ValidIssuer = "icra",
                    ValidateAudience = true,
                    ValidAudience = "enduser",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true,
                };

                var jwt = new JwtSecurityToken(
                    issuer: "icra",
                    audience: "enduser",
                    claims: claims,
                    notBefore: now,
                    expires: now.Add(TimeSpan.FromMinutes(2)),
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
                var responseJson = new
                {
                    access_token = encodedJwt,
                    expires_in = (int)TimeSpan.FromMinutes(30).TotalSeconds
                };

                return responseJson.access_token;


            }
            return string.Empty;
        }
    }
}