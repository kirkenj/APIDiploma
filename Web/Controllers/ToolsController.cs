using Microsoft.AspNetCore.Mvc;
using Logic.Services;
using System.Globalization;
using WebFront.Constants;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToolsController : Controller
    {
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
