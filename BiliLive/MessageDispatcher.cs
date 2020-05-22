using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BiliLive.Message;
using Newtonsoft.Json.Linq;

namespace BiliLive
{
    public class MessageDispatcher : IMessageDispatcher
    {
        public async Task DispatchAsync(JObject message, IMessageHandler messageHandler)
        {
            try
            {
                switch (message["cmd"].ToString())
                {
                    case "DANMU_MSG":
                        await messageHandler.DanmuMessageHandlerAsync(DanmuMessage.JsonToDanmuMessage(message));
                        break;
                    case "SEND_GIFT":
                        await messageHandler.GiftMessageHandlerAsync(GiftMessage.JsonToGiftMessage(message));
                        break;
                    case "GUARD_MSG": // TODO 上舰信息未处理
                        break;
                    case "NOTICE_MSG": // TODO 通知信息未处理
                        break;
                    case "WELCOME":
                        await messageHandler.WelcomeMessageHandlerAsync(WelcomeMessage.JsonToWelcomeMessage(message));
                        break;
                    case "SYS_MSG": // TODO 系统消息未处理
                        break;
                    case "COMBO_END":
                        await messageHandler.ComboEndMessageHandlerAsync(ComboEndMessage.JsonToComboEndMessage(message));
                        break;
                    case "SUPER_CHAT_MESSAGE": // TODO 醒目留言信息未处理
                        break;
                    case "ROOM_REAL_TIME_MESSAGE_UPDATE":
                        await messageHandler.RoomUpdateMessageHandlerAsync(RoomUpdateMessage.JsonToRoomUpdateMessage(message));
                        break;
                    case "SUPER_CHAT_MESSAGE_JPN": // TODO 另一种醒目留言信息未处理
                        break;
                    case "WELCOME_GUARD":
                        await messageHandler.WelcomeGuardMessageHandlerAsync(WelcomeGuardMessage.JsonToWelcomeGuardMessage(message));
                        break;
                    case "ROOM_RANK": // TODO 房间排行信息未处理
                        break;
                    case "ENTRY_EFFECT": // TODO 貌似是舰长的进入信息
                        break;
                    case "COMBO_SEND": // TODO COMBO_SEND
                        await messageHandler.ComboEndMessageHandlerAsync(ComboEndMessage.JsonToComboEndMessage(message));
                        break;
                    case "ANCHOR_LOT_START": // TODO 天选时刻
                        break;
                    case "ACTIVITY_BANNER_UPDATE_V2":
                        break;
                    case "ROOM_CHANGE": //
                        break;
                    case "WEEK_STAR_CLOCK":
                        break;
                    case "LIVE":
                        await messageHandler.LiveStartMessageHandlerAsync(int.Parse(message["roomid"].ToString()));
                        break;
                    case "PREPARING":
                        await messageHandler.LiveStopMessageHandlerAsync(int.Parse(message["roomid"].ToString()));
                        break;
                    default:
                        Debug.WriteLine("// TODO 未记录的信息");
                        break;
                }
            }
            catch (ArgumentException e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }
    }
}