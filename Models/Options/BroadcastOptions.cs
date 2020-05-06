using System.Net;

namespace Models.Options
{
    public class BroadcastOptions
    {
        public IPAddress Address { get; set; }
        
        public int Port { get; set; }
    }
}