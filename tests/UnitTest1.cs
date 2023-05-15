using accounting.src.Controllers;
using accounting.src.Data;
using accounting.src.Entity.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace tests
{

    //prod@gamail.com
    //sha-256
    public class UnitTest1
    {
        private AuthController _authController;
        private string _accessToken;

        [Fact]
        public async void Test1()
        {
            var config = new ConfigurationBuilder()
                    .AddJsonFile("launchSettings.json")
                    .AddEnvironmentVariables()
                    .Build();
            _authController = new AuthController(
                new AppDbContext(new DbContextOptions<AppDbContext>(), config), 
                LoggerFactory.Create(builder => builder.AddConsole())
                );

            var signInBody = new SignInBody
            {
                Email = "prod@gamail.com",
                Password = "7e7ef545661d929fc7995dfd8fb4a95efcaa8733bded93b770823385cac0b207"
            };


            var result = await _authController.SignIn(signInBody) as OkObjectResult;
            Assert.NotNull(result);

            _accessToken = result.Value as string;
            Assert.NotNull(_accessToken);
        }
    }
}