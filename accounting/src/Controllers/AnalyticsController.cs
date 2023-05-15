using accounting.src.Core.IRepository;
using accounting.src.Data;
using accounting.src.Entity.Response;
using accounting.src.Models;
using accounting.src.Repository;
using accounting.src.Utility;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;


//Api аналитики
namespace accounting.src.Controllers
{
    [Route("api/analytics")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IProductAccountingRepository _productAccountingRepository;
        private readonly IUserRepository _userRepository;

        public AnalyticsController(AppDbContext context)
        {
            _productAccountingRepository = new ProductAccountingRepository(context);
            _userRepository = new UserRepository(context);
        }



        [HttpGet("sales")]
        [SwaggerOperation("Получить статистику продаж за текущий месяц и предыдущие")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IncomeForAllMonths))]
        public IActionResult GetIncomeByMonths()
        {
            //Извлекает id пользователя из токена
            string token = Request.Headers.Authorization;
            var userId = JwtManager.GetClaimId(token);
            var result = _productAccountingRepository.GetProfitOfTheMonths(userId);

            return Ok(result);
        }



        [HttpGet("sellers")]
        [SwaggerOperation(Summary = "Получить статистику продаж продавцов")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ManagerAnalyticsBody))]

        public IActionResult GetManagerAnalytics()
        {
            //Получает всех пользователей у которых роль - продавец
            var sellers = _userRepository.GetUsers(UserRole.Seller);

            //Получает отчет о продажах всех работников и об доходе за сегодня
            var result = _productAccountingRepository.GetManagerAnalytics(sellers);
            return Ok(result);
        }

    }
}
