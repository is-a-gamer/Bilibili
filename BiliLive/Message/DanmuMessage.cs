using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BiliLive.Message
{
    public class DanmuMessage : BaseMessage
    {
        /// <summary>
        /// 用户UID
        /// </summary>
        public long UserId;

        /// <summary>
        /// 用户名称
        /// </summary>
        public string Username;

        /// <summary>
        /// 弹幕内容
        /// </summary>
        public string Content;

        /// <summary>
        /// 勋章名称
        /// </summary>
        public string Medal;

        /// <summary>
        /// 勋章等级
        /// </summary>
        public int MedalLevel;

        /// <summary>
        /// 勋章所有者
        /// </summary>
        public string MedalOwnerName;

        public DanmuMessage()
        {
        }

        public static DanmuMessage JsonToDanmuMessage(JObject json)
        {
            if (!"DANMU_MSG".Equals(json["cmd"].ToString()))
            {
                throw new ArgumentException("'cmd' 的值不是 'DANMU_MSG'");
            }

            var info = json["info"];
            // if (int.Parse(info[2][3].ToString()) != 0 && int.Parse(info[2][4].ToString()) != 0 )
            // {
            //     Console.WriteLine("老爷");
            // }
            // if (int.Parse(info[7].ToString())!=0 && int.Parse(info[7].ToString())!=3)
            // {
            //     Console.WriteLine("老爷");
            // }
            try
            {
                //判断有没有佩戴粉丝勋章
                if (info[3].ToArray().Length == 0)
                {
                    return new DanmuMessage
                    {
                        //不用ToString 防止json为null
                        UserId = long.Parse(info[2][0].ToString()),
                        Username = info[2][1].ToString(),
                        Content = info[1].ToString(),
                        Medal = "",
                        MedalLevel = 0,
                        MedalOwnerName = "",
                        Metadata = JsonConvert.SerializeObject(json)
                    };
                }

                return new DanmuMessage
                {
                    UserId = long.Parse(info[2][0].ToString()),
                    Username = info[2][1].ToString(),
                    Content = info[1].ToString(),
                    Medal = info[3][1].ToString(),
                    MedalLevel = (int) info[3][0],
                    MedalOwnerName = info[3][2].ToString(),
                    Metadata = JsonConvert.SerializeObject(json)
                };
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static DanmuMessage JsonToDanmuMessage(string jsonStr)
        {
            try
            {
                var json = JObject.Parse(jsonStr);
                return JsonToDanmuMessage(json);
            }
            catch (JsonReaderException)
            {
                throw new AggregateException("JSON字符串没有成功转换成Json对象");
            }
        }
    }
}