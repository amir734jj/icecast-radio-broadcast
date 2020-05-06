namespace Logic.Interfaces
{
    public interface IStreamSubscriptionAuth
    {
        string Token { get; }
        
        void ResolveToken();
    }
}