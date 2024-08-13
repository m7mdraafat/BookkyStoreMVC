
using Microsoft.AspNetCore.Http;

namespace Store.Utility
{
    public interface IMailingService
    {
        Task SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile>? attachments=null);
    }
}
