using System.Threading.Tasks;
using BiliLive;
using BiliLive.Message;

namespace BiliLive
{
    public interface IMessageHandler
    {
        /// <summary>
        /// 接收以及处理弹幕消息
        /// </summary>
        /// <param name="danmuMessage">弹幕消息</param>
        /// <returns></returns>
        Task DanmuMessageHandlerAsync(DanmuMessage danmuMessage);
        /// <summary>
        /// 处理人气值
        /// </summary>
        /// <param name="audiences">类型是int,为人气值</param>
        /// <returns></returns>
        Task AudiencesHandlerAsync(int audiences);
        /// <summary>
        /// 处理大范围通知消息,比如有谁的房间被送了小电视
        /// </summary>
        /// <param name="noticeMessage"></param>
        /// <returns></returns>
        Task NoticeMessageHandlerAsync(NoticeMessage noticeMessage);
        /// <summary>
        /// 礼物消息
        /// </summary>
        /// <param name="giftMessage"></param>
        /// <returns></returns>
        Task GiftMessageHandlerAsync(GiftMessage giftMessage);
        /// <summary>
        /// 普通的欢迎信息,比如月费老爷
        /// </summary>
        /// <param name="welcomeMessage"></param>
        /// <returns></returns>
        Task WelcomeMessageHandlerAsync(WelcomeMessage welcomeMessage);
        /// <summary>
        /// 送礼物的连接结束后返回的信息
        /// </summary>
        /// <param name="comboEndMessage"></param>
        /// <returns></returns>
        Task ComboEndMessageHandlerAsync(ComboEndMessage comboEndMessage);
        /// <summary>
        /// 房间的更新信息
        /// </summary>
        /// <param name="roomUpdateMessage"></param>
        /// <returns></returns>
        Task RoomUpdateMessageHandlerAsync(RoomUpdateMessage roomUpdateMessage);
        /// <summary>
        /// 欢迎房管进入的消息
        /// </summary>
        /// <param name="welcomeGuardMessage"></param>
        /// <returns></returns>
        Task WelcomeGuardMessageHandlerAsync(WelcomeGuardMessage welcomeGuardMessage);

    }
}