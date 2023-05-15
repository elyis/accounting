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


        //�����������, � ������ ������ �������� access � refresh token
        [HttpPost("signup")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(TokenPair))]
        [SwaggerResponse((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> SignUp(SignUpBody body)
        {
            //���� ������� ��������, �� �������� ������������, ����� null = �� ���� ������������ ����
            var signUpResult = await _userRepository.AddAsync(body);
            if (signUpResult == null)
                return Conflict();

            //���������� access � refresh token
            var token = JwtManager.GenerateTokenPair(signUpResult.Id, signUpResult.Role);

            //��������� refresh token � ����, ����� ����� ��������� access token
            await _userRepository.UpdateToken(signUpResult.Id, token.RefreshToken);
            return Ok(token);
        }


        //�����������, � ������ ������ �������� access � refresh token
        [HttpPost("signin")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(TokenPair))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]

        public async Task<IActionResult> SignIn(SignInBody body)
        {
            //������ ������������, ���� ����� ���������������, ����� null - ������������ �� ����������
            var user = await _userRepository.GetAsync(body.Email);
            if (user == null)
                return NotFound();

            //���������� ���� ������
            var hashPassword = Hmac512Provider.Compute(body.Password);
            if (user.Password != hashPassword)
                return BadRequest();

            //���������� ������ � ��������� refresh token � ����
            var token = JwtManager.GenerateTokenPair(user.Id, user.Role);
            await _userRepository.UpdateToken(user.Id, token.RefreshToken);
            return Ok(token);
        }


        //��� ���������� access token
        //�������� refresh token � �������� ����� ��� � ����

        [HttpPost("token")]
        [SwaggerOperation(Summary = "Update token")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(TokenPair))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "User not found")]
        public async Task<IActionResult> UpdateToken(TokenBody token)
        {
            //���� ����� ���� � ����, �� �������� ������������ � ���, ����� null
            var user = await _userRepository.GetByToken(token.Token);
            if (user == null)
                return NotFound();

            //��������� ���� �������
            var tokenPair = JwtManager.GenerateTokenPair(user.Id, user.Role);
            await _userRepository.UpdateToken(user.Id, tokenPair.RefreshToken);
            return Ok(tokenPair);
        }


        //��������� ��� �������������� �� �����
        [HttpPost("recovery")]
        [SwaggerOperation(Summary = "Send an email with a recovery code")]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "Recovery code has been sent")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "User doesn't exist")]

        public async Task<IActionResult> SendRecoveryCode(EmailBody body)
        {
            //���� ������� ��������������� � ���� � ��, �� �������� ������������, ����� null
            var recoveryCode = await _userRepository.GenerateRecoveryCode(body.Email);
            if (recoveryCode == null)
                return NotFound();

            //�������� ��������� � ����� �������������� �� �����
            await _emailService.SendEmail(
                email: body.Email,
                subject: "�������������� ������� � ��������",
                message: $"��� ��������������: {recoveryCode}. " +
                $"���� ��� ���� �� ��, �������������� ������ ���������");
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


        //������������� ���������� ������ ��������������
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

            //���� ���� �������������� �� ���������, �� ��������� ������
            if (!user.WasPasswordResetRequest || body.RecoveryCode != user.RecoveryCode || user.RecoveryCodeValidBefore < DateTime.UtcNow)
                return BadRequest();
            await _userRepository.ResetPassword(user,body.Password);
            await _emailService.SendEmail(body.Email, "������ ��� �������", "");
            return Ok();
        }


    }
}