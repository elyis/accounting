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
    public class MaterialAccountingController :ControllerBase
    {
        private readonly IMaterialAccountingRepository _materialAccountingRepository;
        private readonly IUserRepository _userRepository;
        public MaterialAccountingController(AppDbContext context)
        {
            _materialAccountingRepository = new MaterialAccountingRepository(context);
            _userRepository = new UserRepository(context);
        }

        [HttpPost("materialAccounting/receipt"), Authorize]
        [SwaggerOperation(Summary = "Добавить запись поступления")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(UUIDBody))]
        public async Task<IActionResult> AddAdmissionInvertoryRecord(CreateMaterialAccounting body)
        {
            string token = Request.Headers.Authorization;
            var userId = JwtManager.GetClaimId(token);
            var user = await _userRepository.GetAsync(userId);

            var result = await _materialAccountingRepository.AddAsync(body, user!, TypeOfGoodsAccounting.Receipt);
            return result == null ? 
                BadRequest() : 
                Ok(new UUIDBody { Id = result.Id.ToString() });
        }

        [HttpPost("materialAccounting/writeOff"), Authorize]
        [SwaggerOperation(Summary = "Добавить запись списания")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(UUIDBody))]
        public async Task<IActionResult> AddWriteOffInvertoryRecord(CreateMaterialAccounting body)
        {
            string token = Request.Headers.Authorization;
            var userId = JwtManager.GetClaimId(token);
            var user = await _userRepository.GetAsync(userId);

            var result = await _materialAccountingRepository.AddAsync(body, user!, TypeOfGoodsAccounting.Write_off);
            return result == null ?
                BadRequest() :
                Ok(new UUIDBody { Id = result.Id.ToString() });
        }

        [HttpPut("materialAccounting/{id}")]
        [SwaggerOperation(Summary = "Загрузить список изображений учета материалов")]
        public async Task<IActionResult> UploadListAccountingMaterialIcons(string id)
        {
            if (!Guid.TryParse(id, out var materialAccountingId))
                return BadRequest();

            var files = Request.Form.Files;

            await _materialAccountingRepository.UploadImages(files, materialAccountingId);
            return Ok();
        }

        [HttpGet("materialAccounting/write-off")]
        [SwaggerOperation(Summary = "Получить все списания")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<ReceiptOrWriteOffMaterialModel>))]
        public IActionResult GetWriteOffRecords()
        {
            var result = _materialAccountingRepository.GetAll(TypeOfGoodsAccounting.Write_off);
            return Ok(result.Select(e => e.ToReceiptOrWriteOffMaterialModel()));
        }

        [HttpGet("materialAccounting/receipt")]
        [SwaggerOperation(Summary = "Получить все поступления")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ReceiptOrWriteOffMaterialModel>))]
        public IActionResult GetReceiptRecords()
        {
            var result = _materialAccountingRepository.GetAll(TypeOfGoodsAccounting.Receipt);
            return Ok(result.Select(e => e.ToReceiptOrWriteOffMaterialModel()));
        }

        [HttpGet("materialAccountingIcon/{filename}")]
        [SwaggerOperation(Summary = "Получить изображение списка учета товаров")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(File))]
        public async Task<IActionResult> GetMaterialAccountingIcon(string filename)
        {
            var bytes = await FileUploader.GetStreamImage(Constants.localPathToMaterialAccounting, filename);
            if (bytes == null)
                return NotFound();

            return File(bytes, $"image/jpeg", filename);
        }



        [HttpGet("materialAccounting/{id}"), Authorize]
        [SwaggerOperation(Summary = "Получить учет материалов по id")]
        public async Task<IActionResult> GetMaterialAccounting(string id)
        {
            if(!Guid.TryParse(id, out var materialAccountingId))
                return BadRequest();

            var materialAccounting = await _materialAccountingRepository.GetAsync(materialAccountingId);
            return materialAccounting == null ? 
                NotFound() : 
                Ok(materialAccounting.ToReceiptOrWriteOffMaterialModel());
        }


    }
}
