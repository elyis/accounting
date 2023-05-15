using accounting.src.Core.IRepository;
using accounting.src.Data;
using accounting.src.Entity;
using accounting.src.Entity.Request;
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
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(AppDbContext context)
        {
            _productRepository = new ProductRepository(context);
        }

        [HttpPost("product")]
        [Authorize]
        [SwaggerOperation(Summary = "Создать новый продукт")]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, Description = "Неверно указан id материала в списке")]
        [SwaggerResponse((int) HttpStatusCode.OK, Description = "id продукта", Type = typeof(UUIDBody))]

        public async Task<IActionResult> CreateProduct(CreateProductBody body)
        {
            var result = await _productRepository.AddAsync(body);

            return result == null ? BadRequest() : Ok(new UUIDBody { Id = result.Id.ToString()});
        }


        [HttpGet("product/{id}")]
        [SwaggerOperation(Summary = "Получить информацию о продукте по id")]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(ProductBody))]

        public async Task<IActionResult> GetMaterial(Guid id)
        {
            var material = await _productRepository.GetAsync(id);
            return material == null ? NotFound() : Ok(material.ToProductBody());
        }

        [HttpGet("productIcon/{filename}")]
        [SwaggerOperation(Summary = "Получить иконку продукта")]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(File))]

        public async Task<IActionResult> GetMaterialImage(string filename)
        {
            var bytes = await FileUploader.GetStreamImage(Constants.localPathToProductIcons, filename);
            if (bytes == null)
                return NotFound();

            return File(bytes, "image/jpeg", filename);
        }

        [HttpGet("products")]
        [SwaggerOperation(Summary = "Получить все продукты")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ProductBody>))]

        public IActionResult GetProducts()
        {
            var products = _productRepository.GetAll(null);
            return Ok(products.Select(p => p.ToProductBody()).ToList());
        }

        [HttpGet("products/{pattern}")]
        [SwaggerOperation(Summary = "Получить все продукты по патерну названия продукта")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ProductBody>))]

        public IActionResult GetProducts(string pattern)
        {
            var products = _productRepository.GetAll(pattern);
            return Ok(products.Select(p => p.ToProductBody()).ToList());
        }


        [HttpPut("product/{id}"), Authorize]
        [SwaggerOperation(Summary = "Обновить информацию по продукту")]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "Успешно изменено", Type = typeof(ProductBody))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Ошибочная сериализация или id материала")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Продукт не найден")]

        public async Task<IActionResult> UpdateProduct(CreateProductBody body, string id)
        {
            if(Guid.TryParse(id, out var productId))
            {
                var result = await _productRepository.UpdateAsync(body, productId);
                return result == null ? NotFound() : Ok(result.ToProductBody());
            };
            return BadRequest();
        }


        [HttpPut("productIcon/{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Загрузить иконку продукта")]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, Description = "Файл пустой; Файл отсутствует")]
        [SwaggerResponse((int) HttpStatusCode.NotFound, Description = "Указан неверный id продукта")]
        [SwaggerResponse((int) HttpStatusCode.OK, Description = "Получить название файла", Type = typeof(string))]

        public async Task<IActionResult> UploadImageMaterial(string id)
        {
            if(Guid.TryParse(id, out Guid materialId))
            {
                if (Request.Headers.ContentLength == 0)
                    return BadRequest("File is empty");

                var filename = await FileUploader.UploadImage(Constants.localPathToProductIcons, Request.Body);
                if (filename == null)
                    return BadRequest("File cann't create");

                var updateResult = await _productRepository.UpdateImage(materialId, filename);
                return updateResult == null ? NotFound() : Ok(filename);
            }
            return BadRequest();
        }
    }
}
