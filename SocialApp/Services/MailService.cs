using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using SocialApp.Models;

namespace SocialApp.Services;
    
public class MailService
{
    private readonly IConfiguration _configuration;

    public MailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public void SendEmail(MailServiceRequest mailRequest)
    {
        var email = new MimeMessage();
        var configurationSection = _configuration.GetSection("From").Value;
        email.From.Add(MailboxAddress.Parse(configurationSection));
        email.To.Add(MailboxAddress.Parse(mailRequest.To));
        email.Subject = mailRequest.Subject;
        var body = new BodyBuilder
        {
            HtmlBody = mailRequest.Body
        };
        email.Body = body.ToMessageBody();

        // send email
        using var smtp = new SmtpClient();

        //if true = production
        
        smtp.Connect(_configuration.GetSection("Host").Value, 587, SecureSocketOptions.StartTls);
        
        //smtp.Connect("localhost");

        var username = _configuration.GetSection("GoogleUsername").Value;

        var password = _configuration.GetSection("Password").Value;
        
        smtp.Authenticate(username, password);
        
        smtp.Send(email);
        
        smtp.Disconnect(true);
    }
}
