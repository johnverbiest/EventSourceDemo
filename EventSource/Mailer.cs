using System;
using System.Threading.Tasks;
using Eventsource.BusinessLogic.Dependencies;

namespace EventSource;

public class Mailer: IMailer
{
    public Task SendEmail(string body)
    {
        Console.WriteLine($"Email Sent: {body}");
        return Task.CompletedTask;
    }
}