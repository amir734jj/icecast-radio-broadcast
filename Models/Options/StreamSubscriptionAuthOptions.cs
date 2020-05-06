namespace Models.Options
{
    public class StreamSubscriptionAuthOptions
    {
        public string Url { get; }
        
        public string Username { get; }
        
        public string Password { get; }

        public StreamSubscriptionAuthOptions(string url, string username, string password)
        {
            Url = url;
            Username = username;
            Password = password;
        }
    }
}