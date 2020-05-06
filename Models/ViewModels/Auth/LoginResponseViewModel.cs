using System;

namespace Models.ViewModels
{
    public class LoginResponseViewModel
    {
        public string Token { get; set; }
        
        public DateTimeOffset Timestamp { get; set; }
        
        public DateTimeOffset Expires { get; set; }
    }
}