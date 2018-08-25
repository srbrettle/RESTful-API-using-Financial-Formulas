using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RESTful_Financial_Formulas_API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class DefaultController : Controller
    {
        [HttpGet]
        public string Get()
        {
            return "For FinancialFormulas API -> /api/FinancialFormulas";
        }
    }
}
