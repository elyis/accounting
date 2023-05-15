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
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialRepository _materialRepository;

        public MaterialController(AppDbContext context)
        {
            _materialRepository = new MaterialRepository(context);
        }

        [HttpPost("material")]
        [Authorize]
        [SwaggerOperation(Summary = "Создать новый материал")]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "id продукта", Type = typeof(UUIDBody))]

        public async Task<IActionResult> CreateMaterial(CreateMaterialBody body)
        {
            var material = await _materialRepository.AddAsync(body);
            var uuidBody = new UUIDBody { Id = material.Id.ToString() };
            return Ok(uuidBody);
        }

        [HttpPut("material/{id}"), Authorize]
        [SwaggerOperation(Summary = "Обновить информацию по материалу")]
        [SwaggerResponse((int) HttpStatusCode.OK, Description = "Успешно изменено", Type = typeof(MaterialBody))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, Description = "Ошибочная сериализация или id материала")]
        [SwaggerResponse((int) HttpStatusCode.NotFound, Description = "Материал не найден")]

        public async Task<IActionResult> UpdateMaterial(CreateMaterialBody body, string id)
        {
            if(Guid.TryParse(id, out var materialId))
            {
                var result = await _materialRepository.UpdateAsync(body, materialId);
                return result == null ? NotFound() : Ok(result.ToMaterialBody());
            }
            return BadRequest();
        }

        [HttpPut("material")]
        [Authorize]

        public async Task<IActionResult> UpdateMaterial(MaterialUpdateBody body)
        {
            var result = await _materialRepository.Update(body);
            return result == null ? NotFound() : Ok();
        }



        [HttpGet("material/{id}")]
        [SwaggerOperation(Summary = "Получить информацию о материале по id")]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(MaterialBody))]

        public async Task<IActionResult> GetMaterial(Guid id)
        {
            var material = await _materialRepository.GetAsync(id);
            return material == null ? NotFound() : Ok(material.ToMaterialBody());
        }

        [HttpGet("materials")]
        [SwaggerOperation(Summary = "Получить все материалы")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<MaterialBody>))]

        public IActionResult GetAllMaterials()
        {
            var materials = _materialRepository.GetAll(null);
            return Ok(materials.Select(m => m.ToMaterialBody()));
        }

        [HttpGet("materials/{pattern}")]
        [SwaggerOperation(Summary = "Получить все материалы по патерну названия материала")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<MaterialBody>))]

        public IActionResult GetAllMaterialsByPattern(string pattern)
        {
            var materials = _materialRepository.GetAll(pattern);
            return Ok(materials.Select(m => m.ToMaterialBody()));
        }


        [HttpGet("materialIcon/{filename}")]
        [SwaggerOperation(Summary = "Получить иконку материала")]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(File))]

        public async Task<IActionResult> GetMaterialImage(string filename)
        {
            var bytes = await FileUploader.GetStreamImage(Constants.localPathToMaterialIcons, filename);
            if (bytes == null)
                return NotFound();

            return File(bytes, "image/jpeg", filename);
        }

        [HttpPut("materialIcon/{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Загрузить иконку материала")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Файл пустой; Файл отсутствует")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Указан неверный id материала")]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "Получить название файла", Type = typeof(string))]

        public async Task<IActionResult> UploadImageMaterial(string id)
        {
            if(Guid.TryParse(id, out Guid materialId))
            {
                if (Request.Headers.ContentLength == 0)
                    return BadRequest("File is empty");

                var filename = await FileUploader.UploadImage(Constants.localPathToMaterialIcons, Request.Body);
                if (filename == null)
                    return BadRequest("File cann't create");

                var updateResult = await _materialRepository.UpdateImage(materialId, filename);
                return updateResult == null ? NotFound() : Ok(filename);
            }
            return BadRequest();
            
        }

    }
}
