using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BiliLive
{
    public interface IMessageDispatcher
    {
        Task DispatchAsync(JObject message,IMessageHandler messageHandler);
    }
}