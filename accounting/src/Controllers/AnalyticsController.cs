using accounting.src.Core.IRepository;
using accounting.src.Data;
using accounting.src.Entity.Response;
using accounting.src.Models;
using accounting.src.Repository;
using accounting.src.Utility;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

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
            string token = Request.Headers.Authorization;
            var userId = JwtManager.GetClaimId(token);
            var result = _productAccountingRepository.GetProfitOfTheMonths(userId);

            
            return result != null ? Ok(result) : Ok(new IncomeForAllMonths
            {
                Currency = Currency.Rub,
                SoldToday = 0,
                EarningForToday = 0,
                EarningThisMonth = new(),
                MonthlyEarnings = new(),
            });
        }

        [HttpGet("sellers")]
        [SwaggerOperation(Summary = "Получить статистику продаж продавцов")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ManagerAnalyticsBody))]

        public IActionResult GetManagerAnalytics()
        {
            var sellers = _userRepository.GetUsers(UserRole.Seller);
            var result = _productAccountingRepository.GetManagerAnalytics(sellers);
            return Ok(result);
        }

    }
}
