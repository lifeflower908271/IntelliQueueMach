using System;
using NLECloudSDK;

namespace Utilities
{
    /// <summary>
    /// 用户定义的全局变量（Udg：User define global）
    /// </summary>
    public static class UDG_
    {
        private const String API_HOST = nameof(API_HOST); // 云平台API域名
        public const String NLE_API = nameof(NLE_API); // 云平台API接口
        public const String ACCESS_TOKEN = nameof(ACCESS_TOKEN); // 访问令牌

        public const int DEVICE_ID = 142604; // 设备ID
        public const string APITAG_NUMBER = "m_num";  // 传感器：排队人数
        public const string APITAG_REPORT = "m_report"; // // 执行器：语音播报
        public const string APITAG_TAKE = "m_take"; // 执行器：取号
        public const string APITAG_CALL = "m_call"; // 执行器：叫号

        public static void Initialize()
        {
            Store.Set(API_HOST, ApplicationSettings.Get(CFG_.API_HOST));
            Store.Set(NLE_API, new NLECloudAPI(Store.Get<String>(API_HOST)));
            Store.Set<object>(ACCESS_TOKEN, null);
        }
    }
}