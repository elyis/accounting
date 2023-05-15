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


//Доступ только токенам, где есть полезная нагрузка роли(claim) == Administrator

namespace accounting.src.Controllers
{
    [Route("api")]
    [ApiController]
    public class AdminPanelController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public AdminPanelController(AppDbContext context)
        {
            _userRepository = new UserRepository(context);
        }



        [HttpGet("users")]
        [SwaggerOperation(Summary = "Получить всех пользователей")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<UserBody>))]
        [Authorize(Roles = "Administrator")]
        public IActionResult GetUsers()
        {
            string token = Request.Headers.Authorization!;
            Guid id = JwtManager.GetClaimId(token);

            var users = _userRepository.GetUsers(id);
            return Ok(users);
        }



        [HttpPut("user")]
        [Authorize(Roles = "Administrator")]
        [SwaggerOperation(Summary = "Обновить роль пользователя")]
        [SwaggerResponse((int) HttpStatusCode.OK, Description = "Роль успешно сменена")]
        [SwaggerResponse((int) HttpStatusCode.NotFound, Description = "Пользователь не существует")]

        public async Task<IActionResult> UpdateUserRole(RoleChangeBody body)
        {
            var updateResult = await _userRepository.UpdateRole(body.Email, body.NewRoleLevel);
            return updateResult == null ? NotFound() : Ok();
        }

    }
}
