using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoffeeAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoffeeController : ControllerBase
    {
        private static readonly string[] Coffees = new[]
        {
            "Flat White", "Long Black", "Latte", "Americano", "Cappuccino"
        };

        
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IEnumerable<string> Get()
        {
            var rng = new Random();

            //return Ok(Coffees[rng.Next(Coffees.Length)]);
            return Coffees;

        }
    }
}