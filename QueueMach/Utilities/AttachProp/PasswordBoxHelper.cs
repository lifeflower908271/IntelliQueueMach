using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Utilities.AttachProp
{
    public static class PasswordBoxHelper
    {
        #region 密码值
        public static readonly DependencyProperty PasswordValueProperty =
            DependencyProperty.RegisterAttached("PasswordValue", typeof(string), typeof(PasswordBoxHelper), new FrameworkPropertyMetadata(null, OnIsPasswordValueChanged));
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static string GetPasswordValue(DependencyObject obj)
        {
            return (string)obj.GetValue(PasswordValueProperty);
        }

        public static void SetPasswordValue(DependencyObject obj, string value)
        {
            obj.SetValue(PasswordValueProperty, value);
        }

        private static void OnIsPasswordValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue == e.NewValue)
                return;

            var pb = d as PasswordBox;
            if (pb == null)
                return;
            
            if ((string)e.NewValue == pb.Password)
                return;

            pb.Password = (string)e.NewValue;
        }
        #endregion

        #region 密码长度
        public static readonly DependencyProperty PasswordLengthProperty =
            DependencyProperty.RegisterAttached("PasswordLength", typeof(int), typeof(PasswordBoxHelper), new FrameworkPropertyMetadata(0));
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static int GetPasswordLength(DependencyObject obj)
        {
            return (int)obj.GetValue(PasswordLengthProperty);
        }

        public static void SetPasswordLength(DependencyObject obj, int value)
        {
            obj.SetValue(PasswordLengthProperty, value);
        }

        #endregion

        #region 密码监视器
        public static readonly DependencyProperty IsMonitoringProperty =
            DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(PasswordBoxHelper), new FrameworkPropertyMetadata(false, OnIsMonitoringChanged));
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static bool GetIsMonitoring(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMonitoringProperty);
        }
        
        public static void SetIsMonitoring(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMonitoringProperty, value);
        }

        private static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pb = d as PasswordBox;
            if (pb == null)
            {
                return;
            }
            if ((bool)e.NewValue)
            {
                pb.PasswordChanged += PasswordChanged;
            }
            else
            {
                pb.PasswordChanged -= PasswordChanged;
            }
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            var pb = sender as PasswordBox;
            if (pb == null)
                return;

            if (GetPasswordValue(pb) != pb.Password)
                SetPasswordValue(pb, pb.Password);
                
            SetPasswordLength(pb, pb.Password.Length);
        }


        #endregion
    }
}
