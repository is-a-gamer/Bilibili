﻿using System;
using System.Threading.Tasks;
using BiliLive;
using BiliLive.Message;
namespace Example
{
    public class LiveHandler:IMessageHandler
    {
        //可以放置自己的参数用来使用,比如WPF的window对象
        public bool Param;

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
            Console.WriteLine($"欢迎{welcomeMessage.Username}进入直播间");
        }

        public async Task ComboEndMessageHandlerAsync(ComboEndMessage comboEndMessage)
        {
            Console.WriteLine($"{comboEndMessage.Username}的${comboEndMessage.GiftName}连击结束了");
        }

        public async Task RoomUpdateMessageHandlerAsync(RoomUpdateMessage roomUpdateMessage)
        {
            Console.WriteLine($"UP当前分数数量{roomUpdateMessage.Fans}");
        }

        public async Task WelcomeGuardMessageHandlerAsync(WelcomeGuardMessage welcomeGuardMessage)
        {
            Console.WriteLine($"房管{welcomeGuardMessage.Username}进入直播间");
        }

        public async Task LiveStartMessageHandlerAsync(int roomId)
        {
            Console.WriteLine("直播开始");
        }

        public async Task LiveStopMessageHandlerAsync(int roomId)
        {
            Console.WriteLine("直播关闭");
        }
    }
}