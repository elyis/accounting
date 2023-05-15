using accounting.src.Core.IRepository;
using accounting.src.Core.IService;
using accounting.src.Data;
using accounting.src.Entity;
using accounting.src.Entity.Request;
using accounting.src.Repository;
using accounting.src.Service;
using accounting.src.Utility;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace accounting.src.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public AuthController(AppDbContext context, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AuthController>();
            _userRepository = new UserRepository(context);
            _emailService = new EmailService();
        }


        //Регистрация, в случае успеха получает access и refresh token
        [HttpPost("signup")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(TokenPair))]
        [SwaggerResponse((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> SignUp(SignUpBody body)
        {
            //Если успешно добавлен, то вернется пользователь, иначе null = то есть пользователь есть
            var signUpResult = await _userRepository.AddAsync(body);
            if (signUpResult == null)
                return Conflict();

            //Генерирует access и refresh token
            var token = JwtManager.GenerateTokenPair(signUpResult.Id, signUpResult.Role);

            //Сохраняет refresh token в базе, чтобы потом обновлять access token
            await _userRepository.UpdateToken(signUpResult.Id, token.RefreshToken);
            return Ok(token);
        }


        //Авторизация, в случае успеха получает access и refresh token
        [HttpPost("signin")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(TokenPair))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]

        public async Task<IActionResult> SignIn(SignInBody body)
        {
            //Вернет пользователь, если такой зарегестрирован, иначе null - пользователя не существует
            var user = await _userRepository.GetAsync(body.Email);
            if (user == null)
                return NotFound();

            //Сравнивает хэши пароли
            var hashPassword = Hmac512Provider.Compute(body.Password);
            if (user.Password != hashPassword)
                return BadRequest();

            //Генерирует токены и обновляет refresh token в базе
            var token = JwtManager.GenerateTokenPair(user.Id, user.Role);
            await _userRepository.UpdateToken(user.Id, token.RefreshToken);
            return Ok(token);
        }


        //Для обновления access token
        //Получает refresh token и пытается найти его в базе

        [HttpPost("token")]
        [SwaggerOperation(Summary = "Update token")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(TokenPair))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "User not found")]
        public async Task<IActionResult> UpdateToken(TokenBody token)
        {
            //Если токен есть в базе, то вернется пользователь с ним, иначе null
            var user = await _userRepository.GetByToken(token.Token);
            if (user == null)
                return NotFound();

            //Генерация пары токенов
            var tokenPair = JwtManager.GenerateTokenPair(user.Id, user.Role);
            await _userRepository.UpdateToken(user.Id, tokenPair.RefreshToken);
            return Ok(tokenPair);
        }


        //Отправить код восстановления на почту
        [HttpPost("recovery")]
        [SwaggerOperation(Summary = "Send an email with a recovery code")]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "Recovery code has been sent")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "User doesn't exist")]

        public async Task<IActionResult> SendRecoveryCode(EmailBody body)
        {
            //Если аккаунт зарегистрирован и есть в бд, то вернется пользователь, иначе null
            var recoveryCode = await _userRepository.GenerateRecoveryCode(body.Email);
            if (recoveryCode == null)
                return NotFound();

            //Отправка сообщения с кодом восстановления на почту
            await _emailService.SendEmail(
                email: body.Email,
                subject: "Восстановление доступа к аккаунту",
                message: $"Код восстановления: {recoveryCode}. " +
                $"Если это были не вы, проигнорируйте данное сообщение");
            return Ok();
        }

        [HttpPost("confirmation")]
        [SwaggerOperation(Summary = "recovery code confirmation")]
        [SwaggerResponse((int)(HttpStatusCode.OK), Description = "Success")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "invalid code or code timed out")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "User doesn't exist")]

        public async Task<IActionResult> RecoveryCodeConfirmation(ConfirmationBody body)
        {
            var user = await _userRepository.GetAsync(body.Email);
            if (user == null)
                return NotFound();

            if (!user.WasPasswordResetRequest || body.RecoveryCode != user.RecoveryCode || user.RecoveryCodeValidBefore < DateTime.UtcNow)
                return BadRequest();

            return Ok();
        }


        //Подтверждение введенного пароля восстановления
        [HttpPost("reset")]
        [SwaggerOperation(Summary = "reset password")]
        [SwaggerResponse((int)(HttpStatusCode.OK), Description = "Reset password")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "invalid code or code timed out")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "User doesn't exist")]

        public async Task<IActionResult> ResetPassword(PasswordResetBody body)
        {
            var user = await _userRepository.GetAsync(body.Email);
            if (user == null)
                return NotFound();

            //Если коды восстановления не совпадают, то ошибочный запрос
            if (!user.WasPasswordResetRequest || body.RecoveryCode != user.RecoveryCode || user.RecoveryCodeValidBefore < DateTime.UtcNow)
                return BadRequest();
            await _userRepository.ResetPassword(user,body.Password);
            await _emailService.SendEmail(body.Email, "Пароль был изменен", "");
            return Ok();
        }


    }
}