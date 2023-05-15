using accounting.src.Core.IRepository;
using accounting.src.Data;
using accounting.src.Entity;
using accounting.src.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace accounting.src.Controllers
{
    [Route("api/warehouse")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly IMaterialRepository _materialRepository;

        public WarehouseController(AppDbContext context)
        {
            _materialRepository = new MaterialRepository(context);
        }


        [HttpGet("materials"), Authorize]
        [SwaggerOperation(Summary = "Получить все материалы со склада")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<AmountOfMaterial>))]
        public IActionResult GetMaterials()
        {
            var materials = _materialRepository.GetAll(null);
            return Ok(materials.Select(e => e.ToAmountOfMaterial()));
        }
    }
}
