namespace MvcWebPage.Models
{
    public class Usuario
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string[] Roles { get; set; }
        public string MsgErr { get; set; }
    }
}
