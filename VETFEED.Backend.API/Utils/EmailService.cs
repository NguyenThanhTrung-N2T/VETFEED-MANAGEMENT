using System.Net;
using System.Net.Mail;

namespace VETFEED.Backend.API.Utils
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendResetPasswordEmailAsync(string toEmail, string newPassword)
        {
            var smtpSettings = _config.GetSection("Smtp");
            var client = new SmtpClient(smtpSettings["Host"], int.Parse(smtpSettings["Port"]!))
            {
                Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]),
                EnableSsl = true
            };

            // HTML body cho đẹp
            string htmlBody = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; background-color: #f9f9f9; }}
                        .container {{ padding: 20px; background-color: #ffffff; border-radius: 8px; }}
                        h2 {{ color: #2c3e50; }}
                        p {{ font-size: 14px; color: #333333; }}
                        .password {{ font-weight: bold; color: #e74c3c; font-size: 16px; }}
                        .footer {{ margin-top: 20px; font-size: 12px; color: #888888; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h2>VetFeed Management</h2>
                        <p>Xin chào,</p>
                        <p>Mật khẩu mới của bạn là:</p>
                        <p class='password'>{newPassword}</p>
                        <p>Vui lòng đăng nhập lại bằng mật khẩu này và đổi sang mật khẩu khác để đảm bảo an toàn.</p>
                        <div class='footer'>
                            <p>Đây là email tự động, vui lòng không trả lời.</p>
                        </div>
                    </div>
                </body>
                </html>";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["From"]!, "VetFeed Management"),
                Subject = "🔐 Mật khẩu mới của bạn",
                Body = htmlBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            // Thêm một số header cho chuyên nghiệp
            mailMessage.Headers.Add("X-Priority", "1"); // ưu tiên cao
            mailMessage.Headers.Add("X-Mailer", "VetFeed Management System");

            await client.SendMailAsync(mailMessage);
        }
    }
}
