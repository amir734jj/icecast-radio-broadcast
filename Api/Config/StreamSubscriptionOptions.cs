namespace Api.Config
{
    public class StreamSubscriptionOptions
    {
        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public string Host { get; set; }

        public string LoginRoute { get; set; }
        
        public string StreamRoute { get; set; }
    }
}