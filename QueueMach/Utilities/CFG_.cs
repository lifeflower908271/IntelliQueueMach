using System;
using System.Collections.Generic;
using System.Configuration;
using NLECloudSDK;

namespace Utilities
{
    public static class CFG_
    {
        public const String API_HOST = nameof(API_HOST); // 云平台API域名
        public const String LOGIN_USER_ACCOUNT = nameof(LOGIN_USER_ACCOUNT); // 登录模块：用户名
        public const String LOGIN_USER_PASSWORD = nameof(LOGIN_USER_PASSWORD); // 登录模块：密码
        public const String LOGIN_IS_REMEMBER = nameof(LOGIN_IS_REMEMBER); // 登录模块：记住密码
        public const String LOGIN_IS_AUTOLOGIN = nameof(LOGIN_IS_AUTOLOGIN); // 登录模块：自动登录
    }
}