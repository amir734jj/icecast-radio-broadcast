namespace Logic.Interfaces
{
    public interface IBroadcastServer
    {
        public void IceCastSend(string message);

        bool Start();
    }
}