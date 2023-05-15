using accounting.src.Core.IRepository;
using accounting.src.Data;
using accounting.src.Entity.Request;
using accounting.src.Entity.Response;
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
    public class ProfileController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;

        public ProfileController(ILoggerFactory loggerFactory, AppDbContext context)
        {
            _logger = loggerFactory.CreateLogger<ProfileController>();
            _userRepository = new UserRepository(context);
        }


        [HttpGet("profile")]
        [Authorize]
        [SwaggerOperation(Summary = "Получить профиль")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(UserBody))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Пользователь не найден")]
        public async Task<IActionResult> GetProfile()
        {
            string token = Request.Headers.Authorization!;
            var userId = JwtManager.GetClaimId(token);

            var user = await _userRepository.GetAsync(userId);
            if (user == null)
                return NotFound();

            return Ok(user.ToUserBody());
        }


        [HttpPut("profileIcon")]
        [Authorize]
        [SwaggerOperation(Summary = "Загрузить иконку профиля")]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "Возвращает имя загруженного файла", Type = typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized)]

        public async Task<IActionResult> UploadProfileImage()
        {
            string token = Request.Headers.Authorization!;
            Guid id = JwtManager.GetClaimId(token);

            var filename = await FileUploader.UploadImage(Constants.localPathToProfileIcons, Request.Body);
            if (filename == null)
                return BadRequest("file could not be created");

            var imageUpdated = await _userRepository.UpdateImage(id, filename);
            return Ok(filename);
        }



        [HttpGet("profileIcon/{filename}")]
        [SwaggerOperation(Summary = "Получить иконку профиля")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(File))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Фото не найдено")]

        public async Task<IActionResult> GetProfileIcon(string filename)
        {
            var bytes = await FileUploader.GetStreamImage(Constants.localPathToProfileIcons, filename);
            if (bytes == null)
                return NotFound();

            return File(bytes, $"image/jpeg", filename);
        }

        [HttpPut("profile")]
        [Authorize]
        [SwaggerOperation(Summary = "Обновить данные профиля")]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "Успешно обновлен")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Пользователь не существует")]

        public async Task<IActionResult> UpdateUser(UpdateUserBody body)
        {
            string token = Request.Headers.Authorization!;
            Guid id = JwtManager.GetClaimId(token);
            var user = await _userRepository.UpdateAsync(id, body);
            return user == null ? NotFound() : Ok(user.ToUserBody());
        }

    }
}
