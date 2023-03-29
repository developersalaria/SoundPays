using Newtonsoft.Json.Linq;
using SoundpaysAdd.Core.Models;


namespace SoundpaysAdd.Core.Interfaces
{
    public interface IEmailSenderService
    {
        Task<bool> SendEmailAsync(string email, string subject, JObject param, long templateId, JObject headers = null, List<string> emailCarbonCopyList = null, byte[] attachment = null, string attachmentName = null);
    }
}
