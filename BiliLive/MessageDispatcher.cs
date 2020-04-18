using System;
using System.Threading.Tasks;
using BiliLive.Message;
using Newtonsoft.Json.Linq;

namespace BiliLive
{
    public class MessageDispatcher : IMessageDispatcher
    {
        public async Task DispatchAsync(JObject message, IMessageHandler messageHandler)
        {
            switch (message["cmd"].ToString())
            {
                case "DANMU_MSG":
                    messageHandler.DanmuMessageHandlerAsync(DanmuMessage.JsonToDanmuMessage(message));
                    break;
                case "SEND_GIFT":
                    messageHandler.GiftMessageHandlerAsync(GiftMessage.JsonToGiftMessage(message));
                    break;
                case "GUARD_MSG":
                    Console.WriteLine("// TODO 上舰信息未处理");
                    break;
                case "NOTICE_MSG":
                    Console.WriteLine("// TODO 通知信息未处理");
                    break;
                case "WELCOME":
                    messageHandler.WelcomeMessageHandlerAsync(WelcomeMessage.JsonToWelcomeMessage(message));
                    break;
                case "SYS_MSG":
                    Console.WriteLine("// TODO 系统消息未处理");
                    break;
                case "COMBO_END":
                    messageHandler.ComboEndMessageHandlerAsync(ComboEndMessage.JsonToComboEndMessage(message));
                    break;
                case "SUPER_CHAT_MESSAGE":
                    Console.WriteLine("// TODO 醒目留言信息未处理");
                    break;
                case "ROOM_REAL_TIME_MESSAGE_UPDATE":
                    messageHandler.RoomUpdateMessageHandlerAsync(RoomUpdateMessage.JsonToRoomUpdateMessage(message));
                    break;
                case "SUPER_CHAT_MESSAGE_JPN":
                    Console.WriteLine("// TODO 另一种醒目留言信息未处理");
                    break;
                case "WELCOME_GUARD":
                    messageHandler.WelcomeGuardMessageHandlerAsync(WelcomeGuardMessage.JsonToWelcomeGuardMessage(message));
                    break;
                case "ROOM_RANK":
                    Console.WriteLine("// TODO 房间排行信息未处理");
                    break;
                case "ENTRY_EFFECT":
                    Console.WriteLine("// TODO 貌似是舰长的进入信息");
                    break;
                case "COMBO_SEND":
                    Console.WriteLine("//  COMBO_SEND");
                    break;
                case "ANCHOR_LOT_START":
                    Console.WriteLine("// 天选时刻");
                    break;
                case "ACTIVITY_BANNER_UPDATE_V2":
                    break;
                default:
                    Console.WriteLine("// TODO 未记录的信息");
                    break;
            }
        }
    }
}