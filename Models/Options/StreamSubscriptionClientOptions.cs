namespace Models.Options
{
    public class StreamSubscriptionClientOptions
    {
        public string Url { get; }
        
        public string Path { get; }
        
        public StreamSubscriptionClientOptions(string url, string path)
        {
            Url = url;
            Path = path;
        }
    }
}