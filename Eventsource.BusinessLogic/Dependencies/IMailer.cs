using System;
using System.Threading.Tasks;

namespace Eventsource.BusinessLogic.Dependencies
{
    public interface IMailer
    {
        Task SendEmail(string body);
    }
}