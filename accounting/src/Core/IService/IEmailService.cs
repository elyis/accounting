namespace accounting.src.Core.IService
{
    public interface IEmailService
    {
        Task SendEmail(string email, string subject, string message);
    }
}
