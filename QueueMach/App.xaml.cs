using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace QueueMach
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public class Cfg
        {
            public static String LoginUserAccount = nameof(LoginUserAccount);
            public static String LoginUserPassword = nameof(LoginUserPassword);
            public static String LoginIsRemember = nameof(LoginIsRemember);
            public static String LoginIsAutoLogin = nameof(LoginIsAutoLogin);
        }
    }
}
