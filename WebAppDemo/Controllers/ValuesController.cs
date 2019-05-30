using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAppDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public DateTimeOffset GetDate()
        {
            return DateTimeOffset.Now;
        }

        [HttpPut]
        public string Put(int i)
        {
            return string.Format("Input:{0}", i);
        }
    }
}