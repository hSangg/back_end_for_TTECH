using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using tech_project_back_end.Helpter;

namespace tech_project_back_end.Services.IService
{
    public interface IEmailService
    {
        Task SendEmailAsync(MailRequest mailrequest);

        public string ProcessEmail(string template, Dictionary<string, string> placeholders);
    }
}
