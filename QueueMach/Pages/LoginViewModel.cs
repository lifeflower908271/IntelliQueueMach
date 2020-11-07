using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NLECloudSDK;
using Stylet;
using StyletIoC;
using Utilities;

namespace QueueMach.Pages
{
    public class LoginViewModel : Screen
    {
        private readonly IWindowManager _windowManager;
        private readonly IContainer _container;

        private NLECloudAPI nle_api;
        private object _mutex = new object(); // 互斥锁的同步块索引对象

        public LoginViewModel(IWindowManager windowManager, IContainer container)
        {
            _windowManager = windowManager;
            _container = container;
            nle_api = Store.Get<NLECloudAPI>(UDG_.NLE_API);
        }

        public String UserAccount { get; set; } = "";
        public String UserPassword { get; set; } = "";
        public Boolean IsRemember { get; set; } = false;
        public Boolean IsAutoLogin { get; set; } = false;

        protected override void OnViewLoaded()
        {
            bool isSuccess, isRemember, isAutoLogin;

            // 尝试对appSettings配置节中记住密码选项进行读取转换
            isSuccess = Boolean.TryParse(ApplicationSettings.Get(CFG_.LOGIN_IS_REMEMBER), out isRemember);
            if (!isSuccess || !isRemember)
                return;
            IsRemember = true;
            UserAccount = ApplicationSettings.Get(CFG_.LOGIN_USER_ACCOUNT);
            UserPassword = ApplicationSettings.Get(CFG_.LOGIN_USER_PASSWORD);

            // 尝试对appSettings配置节中自动登录选项进行读取转换
            isSuccess = Boolean.TryParse(ApplicationSettings.Get(CFG_.LOGIN_IS_AUTOLOGIN), out isAutoLogin);
            if (!isSuccess || !isAutoLogin)
                return;
            IsAutoLogin = true;
            Login();
        }

        private bool m_pending = false; // 挂起登录请求
        public async void Login()
        {
            if(String.IsNullOrEmpty(UserAccount) || String.IsNullOrEmpty(UserPassword))
            {
                _windowManager.ShowMessageBox("账号名或密码为空！", "提示", MessageBoxButton.OK);
                return;
            }
            var resp = await Task.Factory.StartNew(() =>
            {
                return nle_api.UserLogin(new AccountLoginDTO() { Account = UserAccount, Password = UserPassword });
            });
            lock (_mutex)
            {
                if (!resp.IsSuccess())
                {
                    ApplicationSettings.Set(CFG_.LOGIN_USER_ACCOUNT, "");
                    ApplicationSettings.Set(CFG_.LOGIN_USER_PASSWORD, "");
                    ApplicationSettings.Set(CFG_.LOGIN_IS_REMEMBER, false.ToString());
                    ApplicationSettings.Set(CFG_.LOGIN_IS_AUTOLOGIN, false.ToString());
                    _windowManager.ShowMessageBox(resp.Msg, "提示", MessageBoxButton.OK);
                    return;
                }

                if (m_pending)
                    return;
                m_pending = true;

                Store.Set(UDG_.ACCESS_TOKEN, resp.ResultObj.AccessToken);
                ApplicationSettings.Set(CFG_.LOGIN_IS_REMEMBER, IsRemember.ToString());
                ApplicationSettings.Set(CFG_.LOGIN_IS_AUTOLOGIN, IsAutoLogin.ToString());
                if (IsRemember)
                {
                    ApplicationSettings.Set(CFG_.LOGIN_USER_ACCOUNT, UserAccount);
                    ApplicationSettings.Set(CFG_.LOGIN_USER_PASSWORD, UserPassword);
                }
                _windowManager.ShowMessageBox("登录成功！", "提示", MessageBoxButton.OK);
                RequestClose(true);
            }
        }


        public void Cancel()
        {
            RequestClose(false);
        }
    }
}
