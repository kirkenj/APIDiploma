using Microsoft.AspNetCore.Mvc;
using Logic.Services;
using System.Globalization;
using WebFront.Constants;

namespace WebFront.Controllers
{
    [ApiController]
    public class ToolsController : Controller
    {
        [HttpGet("Tools/GetRequest")]
        public IActionResult Get()
        {
            var url = HttpContext.Request.Host;
            return Ok(url);
        }

        [HttpGet(IncludeModels.ControllersActionsRoot.SPELL_NUMBER_ROUTE)]
        public IActionResult SpellNumber(string numberToSpell)
        {
            var sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            numberToSpell = numberToSpell.Replace(",", sep);
            numberToSpell = numberToSpell.Replace(".", sep);
            return double.TryParse(numberToSpell, out double val) ? Ok(RusCurrency.Str(val)) : BadRequest();
        }
    }
}
