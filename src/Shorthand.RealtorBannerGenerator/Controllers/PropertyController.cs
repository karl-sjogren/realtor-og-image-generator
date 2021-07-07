using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shorthand.RealtorBannerGenerator.Core.Contracts;
using Shorthand.RealtorBannerGenerator.Core.Models;

namespace Shorthand.RealtorBannerGenerator.Controllers {
    [ApiController]
    [Route("api/property")]
    public class PropertyController : Controller {
        private readonly IPropertyProvider _propertyProvider;
        private readonly ILogger<PropertyController> _logger;

        public PropertyController(IPropertyProvider propertyProvider, ILogger<PropertyController> logger) {
            _propertyProvider = propertyProvider;
            _logger = logger;
        }

        [HttpGet("{propertyId}")]
        public async Task<ActionResult<RealtorProperty>> GetProfileAsync([FromRoute] string propertyId, CancellationToken cancellationToken) {
            var property = await _propertyProvider.GetRealtorPropertyAsync(propertyId, cancellationToken);

            if(property == null)
                return NotFound();

            return property;
        }
    }
}
