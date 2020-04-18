using System;
using BiliLive;

namespace Example
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Start();
            Console.WriteLine("按回车结束");
            Console.ReadLine();
        }

        public async static void Start()
        {
            LiveHandler liveHandler = new LiveHandler();
            //第一个参数是直播间的房间号
            //第二个参数是自己实现的处理器
            //第三个参数是可选的,可以是默认的消息分发器,也可以是自己实现的消息分发器
            LiveRoom room = new LiveRoom(22128505,liveHandler);
            //等待连接,该方法会反回是否连接成功
            if (!await room.ConnectAsync())
            {
                Console.WriteLine("连接失败");
            }
            //开始读取消息,会启动另外一个线程
            //消息由Dispatcher分发到对应的MessageHandler中对应方法
            room.ReadMessageLoop();
        }
    }
}