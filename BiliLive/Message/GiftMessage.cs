using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BiliLive.Message
{
    public class GiftMessage:BaseMessage
    {
        /// <summary>
        /// 礼物名称
        /// </summary>
        public string GiftName;

        /// <summary>
        /// 礼物ID
        /// </summary>
        public int GiftId;

        /// <summary>
        /// 礼物数量
        /// </summary>
        public int GiftNum;

        /// <summary>
        /// 送礼物的用户名
        /// </summary>
        public string Username;

        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId;

        /// <summary>
        /// 头像的URL
        /// </summary>
        public string FaceUrl;

        /// <summary>
        /// 礼物类型,
        /// TODO 没收集类型
        /// </summary>
        public string GiftType;

        public static GiftMessage JsonToGiftMessage(JObject json)
        {
            if (!"SEND_GIFT".Equals(json["cmd"].ToString()))
            {
                throw new ArgumentException("字段 'cmd' 的值不是 'SEND_GIFT'");
            }

            var data = json["data"];
            return new GiftMessage
            {
                //防止有字段不存在,所以使用 + "" 没用ToString方法
                GiftName = data["giftName"] + "",
                GiftId = int.Parse(data["giftId"] + ""),
                GiftNum = int.Parse(data["num"] + ""),
                Username = data["uname"] + "",
                UserId = long.Parse(data["uid"] + ""),
                FaceUrl = data["face"] + "",
                GiftType = data["giftType"] + "",
                Metadata = JsonConvert.SerializeObject(json)
            };
        }

        public static GiftMessage JsonToGiftMessage(string jsonStr)
        {
            try
            {
                var json = JObject.Parse(jsonStr);
                return JsonToGiftMessage(json);
            }
            catch (JsonReaderException)
            {
                throw new AggregateException("JSON字符串没有成功转换成Json对象");
            }
        }
    }
}