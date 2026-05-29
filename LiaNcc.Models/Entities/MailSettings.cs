namespace LiaNcc.Models.Entities
{
    public class MailSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public bool EnableSSL { get; set; } = true;
        public string FromEmail { get; set; } = string.Empty;
        public string FromEmailPwd { get; set; } = string.Empty;
        public string SenderName { get; set; } = "Lia NCC";
        public string AdminEmail { get; set; } = string.Empty;
        public bool SendCustomerConfirmation { get; set; } = true;
        public bool SendAdminNotification { get; set; } = true;
    }
}
