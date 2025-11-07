using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using API.Data;
using API.Models;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SharedController : ControllerBase
    {
        private readonly DataContext _context;
        public SharedController(DataContext context)
        {
            _context = context;
        }
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            var status = new
            {
                Status = "API is running",
                Timestamp = DateTime.UtcNow
            };
            return Ok(status);
        }
        public IActionResult GetAll() 
        {
            return Ok();
        }

        public IActionResult Get(int id) 
        {
            return Ok();
        }
        [HttpPost("{ElementId}/{Id}")]
        public IActionResult Post(int ElementId, int Id) 
        {

            return Ok();
        }
    }
}
