using accounting.src.Core.IRepository;
using accounting.src.Data;
using accounting.src.Entity;
using accounting.src.Entity.Request;
using accounting.src.Entity.Response;
using accounting.src.Models;
using accounting.src.Repository;
using accounting.src.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace accounting.src.Controllers
{
    [Route("api")]
    [ApiController]
    public class SalesAccountingController : ControllerBase
    {
        private readonly IProductAccountingRepository _productAccountingRepository;
        private readonly IUserRepository _userRepository;

        public SalesAccountingController(AppDbContext context)
        {
            _userRepository = new UserRepository(context);
            _productAccountingRepository = new ProductAccountingRepository(context);
        }
        

        [HttpPut("salesAccounting/{id}")]
        [SwaggerOperation(Summary = "Загрузить список изображений учета продуктов")]

        public async Task<IActionResult> UploadListAccountingMaterialIcons(string id)
        {
            if (!Guid.TryParse(id, out var materialAccountingId))
                return BadRequest();

            var files = Request.Form.Files;

            await _productAccountingRepository.UploadImages(files, materialAccountingId);
            return Ok();
        }

        [HttpPut("saleAccounting/{id}"), Authorize]
        [SwaggerOperation(Summary = "Обновить информацию о продаже")]
        [SwaggerResponse((int) HttpStatusCode.OK)]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateSale(CreateSaleBody body, string id)
        {
            if (!Guid.TryParse(id, out var saleId))
                return BadRequest();

            var result = await _productAccountingRepository.UpdateAsync(body, saleId);
            return result == null ? NotFound() : Ok();
        }

        [HttpGet("saleAccounting/{id}"), Authorize]
        [SwaggerOperation(Summary = "Получить информацию по продаже")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetSaleAsync(string id)
        {
            if (!Guid.TryParse(id, out var saleId))
                return BadRequest();

            var result = await _productAccountingRepository.GetAsync(saleId);
            return result == null ? NotFound() : Ok(result.ToSaleBody());
        }

        [HttpGet("sales"), Authorize]
        [SwaggerOperation(Summary = "Получить продажи продавца")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<SaleBody>))]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        public IActionResult GetAllSalesBySeller()
        {
            string token = Request.Headers.Authorization;
            var userId = JwtManager.GetClaimId(token);
            var result = _productAccountingRepository.GetAll(userId);
            return Ok(result.Select(e => e.ToSaleBody()));
        }




        [HttpGet("salesAccounting")]
        [SwaggerOperation(Summary = "Получить все продажи")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<SaleBody>))]
        public IActionResult GetWriteOffRecords()
        {
            var result = _productAccountingRepository.GetAll();
            return Ok(result.Select(e => e.ToSaleBody()));
        }

        [HttpGet("salesAccountingIcon/{filename}")]
        [SwaggerOperation(Summary = "Получить изображение списка учета товаров")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(File))]
        public async Task<IActionResult> GetProductAccountingIcon(string filename)
        {
            var bytes = await FileUploader.GetStreamImage(Constants.localPathToProductAccounting, filename);
            if (bytes == null)
                return NotFound();

            return File(bytes, $"image/jpeg", filename);
        }

        [HttpPost("salesAccounting"), Authorize]
        [SwaggerOperation(Summary = "Добавить запись продажи")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(UUIDBody))]
        public async Task<IActionResult> AddWriteOffInvertoryRecord(CreateSaleBody body)
        {
            string token = Request.Headers.Authorization;
            var userId = JwtManager.GetClaimId(token);
            var user = await _userRepository.GetAsync(userId);

            var result = await _productAccountingRepository.AddAsync(body, user!, TypeOfGoodsAccounting.Write_off);
            return result == null ?
                BadRequest() :
                Ok(new UUIDBody { Id = result.Id.ToString() });
        }
    }
}
