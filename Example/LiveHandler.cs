using System;
using System.Threading.Tasks;
using BiliLive;
using BiliLive.Message;
namespace Example
{
    public class LiveHandler:IMessageHandler
    {
        //可以放置自己的参数用来使用,比如WPF的window对象
        public string Param;
        public async Task DanmuMessageHandlerAsync(DanmuMessage danmuMessage)
        {
            Console.WriteLine($"发送者:{danmuMessage.Username},内容:{danmuMessage.Content}");
        }

        public async Task AudiencesHandlerAsync(int audiences)
        {
            Console.WriteLine($"当前人气值:{audiences}");
        }

        public async Task NoticeMessageHandlerAsync(NoticeMessage noticeMessage)
        {
            Console.WriteLine("通知信息未处理");
        }

        public async Task GiftMessageHandlerAsync(GiftMessage giftMessage)
        {
            Console.WriteLine($"{giftMessage.Username}送出了{giftMessage.GiftNum}个{giftMessage.GiftName}");
        }

        public async Task WelcomeMessageHandlerAsync(WelcomeMessage welcomeMessage)
        {
            Console.WriteLine($"欢迎{welcomeMessage.Username}");
        }
    }
}