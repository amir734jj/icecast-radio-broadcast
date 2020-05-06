using System.Threading.Tasks;

namespace Logic.Interfaces
{
    public interface IStreamSubscriptionClient
    {
        Task Listen();
    }
}