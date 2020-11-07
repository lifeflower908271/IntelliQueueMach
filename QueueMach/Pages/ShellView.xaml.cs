using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;

namespace QueueMach.Pages
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();

            ChartPlotterInitialize();
        }

        /// <summary>
        /// 初始化折线图控件
        /// </summary>
        private void ChartPlotterInitialize()
        {
            // 取消绘制底部的扩展标签
            __HAxis_Queue.ShowMayorLabels = false;
            // 设置日期刻度的时间格式
            __HAxis_Queue.LabelProvider.SetCustomFormatter(info => info.Tick.ToString("hh:mm:ss"));
            // 新建折线图的指示器
            CursorCoordinateGraph cursor = new CursorCoordinateGraph();
            cursor.XTextMapping = value =>
            {
                if (Double.IsNaN(value))
                    return "";

                DateTime time = __HAxis_Queue.ConvertFromDouble(value);
                return time.ToString();
            }; // 对应在折线图x轴上的指针坐标数值以日期的字符格式呈现
            cursor.YTextMapping = value =>
            {
                if (Double.IsNaN(value))
                    return "";
                return $@"{((int)(value + 0.5)).ToString()}人";
            }; // 对应在折线图y轴上的指针坐标数值以排队人数的字符格式呈现
            __LinePlotter_Queue.Children.Add(cursor);
        }
    }
}
