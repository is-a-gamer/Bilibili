using System.Threading.Tasks;
using BiliLive;
using BiliLive.Message;

namespace BiliLive
{
    public interface IMessageHandler
    {
        //这是处理弹幕信息的
        Task DanmuMessageHandlerAsync(DanmuMessage danmuMessage);

        //这是处理人气值的
        Task AudiencesHandlerAsync(int audiences);

        //这是处理系统通知消息的
        Task NoticeMessageHandlerAsync(NoticeMessage noticeMessage);
        Task GiftMessageHandlerAsync(GiftMessage giftMessage);

        Task WelcomeMessageHandlerAsync(WelcomeMessage welcomeMessage);

        Task ComboEndMessageHandlerAsync(ComboEndMessage comboEndMessage);
        Task RoomUpdateMessageHandlerAsync(RoomUpdateMessage roomUpdateMessage);
        Task WelcomeGuardMessageHandlerAsync(WelcomeGuardMessage welcomeGuardMessage);

    }
}