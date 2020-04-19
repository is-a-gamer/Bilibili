using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BitConverter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BiliLive
{
    public class LiveRoom
    {
        //直播页面的房间ID
        private int _shotRoomId;

        //10秒无法连接判定连接失败
        private HttpClient _httpClient = new HttpClient {Timeout = TimeSpan.FromSeconds(10)};
        private int _roomId;
        private TcpClient _tcpClient = new TcpClient();
        private Stream _roomStream;
        private const short ProtocolVersion = 2;
        private const int ProtocolHeadLength = 16;
        private IMessageHandler _messageHandler;
        private IMessageDispatcher messageDispatcher;
        private bool _connected = false;

        public LiveRoom(int roomId, IMessageHandler messageHandler)
        {
            _shotRoomId = roomId;
            messageDispatcher = new MessageDispatcher();
            _messageHandler = messageHandler;
        }

        public LiveRoom(int roomId, IMessageHandler messageHandler, IMessageDispatcher messageDispatcher)
        {
            _shotRoomId = roomId;
            this.messageDispatcher = messageDispatcher;
            _messageHandler = messageHandler;
        }

        public async Task<bool> ConnectAsync()
        {
            var tmpData =
                JObject.Parse(
                    await _httpClient.GetStringAsync(
                        $"https://api.live.bilibili.com/room/v1/Room/room_init?id={_shotRoomId}"));
            if (int.Parse(tmpData["code"].ToString()) != 0)
            {
                return false;
            }

            _roomId = int.Parse(tmpData["data"]["room_id"].ToString());
            tmpData = JObject.Parse(await _httpClient.GetStringAsync(
                $"https://api.live.bilibili.com/room/v1/Danmu/getConf?room_id={_roomId}&platform=pc&player=web"));
            //连接的令牌
            var token = tmpData["data"]["token"].ToString();
            //解析域名,拿取IP地址,用于连接
            var chatHost = tmpData["data"]["host"].ToString();
            var ips = await Dns.GetHostAddressesAsync(chatHost);
            //连接的端口
            var chatPort = int.Parse(tmpData["data"]["port"].ToString());
            Random random = new Random();
            //随机一个选择域名解析出来的IP,负载均衡
            await _tcpClient.ConnectAsync(ips[random.Next(ips.Length)], chatPort);
            if (!_tcpClient.Connected)
            {
                //这是错误处理的代码
                return false;
            }

            _roomStream = _tcpClient.GetStream();
            //判断能不能写入数据
            if (!_roomStream.CanWrite)
            {
                //这是错误处理的代码
                return false;
            }

            if (!await SendJoinMsgAsync(token))
            {
                //这是错误处理的代码
                return false;
            }

            var headBuffer = new byte[ProtocolHeadLength];
            await _roomStream.ReadAsync(headBuffer, 0, headBuffer.Length);
            DanmuHead danmuHead = DanmuHead.BufferToDanmuHead(headBuffer);
            if (danmuHead.HeaderLength != ProtocolHeadLength || danmuHead.Action != 8)
            {
                //如果头信息的长度不是16,或者Action的的值不是8 (服务器接受认证包后回应的第一个数据)
                //这是错误处理的代码
                return false;
            }

            var dataBuffer = new byte[danmuHead.PacketLength - danmuHead.HeaderLength];
            await _roomStream.ReadAsync(dataBuffer, 0, danmuHead.MessageLength());
            var s = Encoding.Default.GetString(dataBuffer);
            var data = JObject.Parse(s);
            if (int.Parse(data["code"].ToString()) != 0)
            {
                return false;
            }

            //循环发送心跳信息
#pragma warning disable 4014
            SendHeartbeatLoop();
#pragma warning restore 4014
            return true;
        }

        public async Task ReadMessageLoop()
        {
            while (_roomStream.CanRead)
            {
                var headBuffer = new byte[ProtocolHeadLength];
                //先读取一次头信息
                //BUG 高频率发送弹幕只能读取到一条
                await _roomStream.ReadAsync(headBuffer, 0, ProtocolHeadLength);
                //解析头信息
                DanmuHead danmuHead = DanmuHead.BufferToDanmuHead(headBuffer);
                //判断协议
                if (danmuHead.HeaderLength != ProtocolHeadLength)
                {
                    continue;
                }

                //初始化一个放数据的byte数组
                byte[] dataBuffer;
                if (danmuHead.Action == 3)
                {
                    //给服务器发送心跳信息后的回应信息,所带的数据是直播间的观看人数(人气值)
                    dataBuffer = new byte[danmuHead.MessageLength()];
                    await _roomStream.ReadAsync(dataBuffer, 0, danmuHead.MessageLength());
                    var audiences = EndianBitConverter.BigEndian.ToInt32(dataBuffer, 0);
                    _messageHandler.AudiencesHandlerAsync(audiences);
                    continue;
                }

                string tmpData;
                JObject json;
                if (danmuHead.Action == 5 && danmuHead.Version == 2)
                {
                    //有效负载为礼物、弹幕、公告等内容数据
                    //读取数据放入缓冲区
                    dataBuffer = new byte[danmuHead.MessageLength()];
                    await _roomStream.ReadAsync(dataBuffer, 0, danmuHead.MessageLength());
                    //之后把数据放入到内存流
                    using (var ms = new MemoryStream(dataBuffer, 2, danmuHead.MessageLength() - 2))
                    {
                        //使用内存流生成解压流(压缩流) 
                        var deflate = new DeflateStream(ms, CompressionMode.Decompress);
                        var headerbuffer = new byte[ProtocolHeadLength];
                        try
                        {
                            while (true)
                            {
                                await deflate.ReadAsync(headerbuffer, 0, ProtocolHeadLength);
                                danmuHead = DanmuHead.BufferToDanmuHead(headerbuffer);
                                var messageBuffer = new byte[danmuHead.MessageLength()];
                                await deflate.ReadAsync(messageBuffer, 0, danmuHead.MessageLength());
                                var jsonStr = Encoding.UTF8.GetString(messageBuffer, 0, danmuHead.MessageLength());
                                json = JObject.Parse(jsonStr);
                                messageDispatcher.DispatchAsync(json, _messageHandler);
                            }
                        }
                        catch (Exception)
                        {
                            //读数据超出长度
                        }
                    }

                    continue;
                }

                dataBuffer = new byte[danmuHead.MessageLength()];
                await _roomStream.ReadAsync(dataBuffer, 0, danmuHead.MessageLength());
                tmpData = Encoding.UTF8.GetString(dataBuffer);
                json = JObject.Parse(tmpData);
                messageDispatcher.DispatchAsync(json, _messageHandler);
            }
        }

        public async Task<bool> SendJoinMsgAsync(string token)
        {
            var packageModel = new Dictionary<string, object>
            {
                {"roomid", _roomId},
                {"uid", 0},
                {"protover", ProtocolVersion},
                {"token", token},
                {"platform", "web"},
                {"type", 2}
            };
            var body = JsonConvert.SerializeObject(packageModel);
            await SendSocketDataAsync(7, body);
            return true;
        }

        public Task SendSocketDataAsync(int action, string body)
        {
            return SendSocketDataAsync(ProtocolHeadLength, ProtocolVersion, action, 1, body);
        }

        public async Task SendSocketDataAsync(short headLength, short version, int action, int param,
            string body)
        {
            var data = Encoding.UTF8.GetBytes(body);
            var packageLength = data.Length + headLength;

            var buffer = new byte[packageLength];
            var ms = new MemoryStream(buffer);

            await ms.WriteAsync(EndianBitConverter.BigEndian.GetBytes(buffer.Length), 0, 4);
            await ms.WriteAsync(EndianBitConverter.BigEndian.GetBytes(headLength), 0, 2);
            await ms.WriteAsync(EndianBitConverter.BigEndian.GetBytes(version), 0, 2);
            await ms.WriteAsync(EndianBitConverter.BigEndian.GetBytes(action), 0, 4);
            await ms.WriteAsync(EndianBitConverter.BigEndian.GetBytes(param), 0, 4);
            if (data.Length > 0)
            {
                await ms.WriteAsync(data, 0, data.Length);
            }

            await _roomStream.WriteAsync(buffer, 0, buffer.Length);
        }

        private async Task SendHeartbeatLoop()
        {
            try
            {
                _connected = _tcpClient.Connected;

                while (_connected)
                {
                    try
                    {
                        await SendSocketDataAsync(ProtocolHeadLength, ProtocolVersion, 2, 1, "");
                        //休眠30秒
                        await Task.Delay(30000);
                    }
                    catch (Exception e)
                    {
                        _connected = false;
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
            catch (Exception)
            {
                _connected = false;
                throw;
            }
        }
    }
}