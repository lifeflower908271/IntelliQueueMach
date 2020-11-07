using System;
using System.Windows;
using Stylet;
using StyletIoC;
using NLECloudSDK;
using Microsoft.Research.DynamicDataDisplay;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using Microsoft.Research.DynamicDataDisplay.Charts;
using QueueMach.Model;
using System.Windows.Controls;
using Utilities;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;

namespace QueueMach.Pages
{
    public class ShellViewModel : Screen
    {
        private readonly IWindowManager _windowManager;
        private readonly IContainer _container;
        public ShellView TransView => (ShellView)View; // 与当前vm关联的视图

        private NLECloudAPI nle_api; // api接口
        private String m_token; // 接口访问令牌
        private object _mutex = new object(); // 互斥锁的同步块索引对象

        private QueueBeanPointCollection m_queueBeanPointCollection; // 排队数据集合
        private DispatcherTimer m_dispatcherTimer;

        #region 数据源
        public EnumerableDataSource<QueueBeanPoint> DatSrcQueueBeanPoint { get; set; } // 排队数据集合
        public String ReportMessage { get; set; } // 语音播报消息文本
        public int QueueNumber { get; set; } // 排队人数
        #endregion

        public ShellViewModel(IWindowManager windowManager, IContainer container)
        {
            _windowManager = windowManager;
            _container = container;

            nle_api = Store.Get<NLECloudAPI>(UDG_.NLE_API);
        }

        protected override void OnViewLoaded()
        {
            // 登录排队机系统
            this.View.Visibility = Visibility.Hidden;
            var vmLogin = _container.Get<LoginViewModel>();
            var rst = _windowManager.ShowDialog(vmLogin);
            this.View.Visibility = Visibility.Visible;
            if (rst == false) RequestClose();
            m_token = Store.Get<String>(UDG_.ACCESS_TOKEN);

          
            m_queueBeanPointCollection = new QueueBeanPointCollection(); // 实例绘制点集合
            DatSrcQueueBeanPoint = new EnumerableDataSource<QueueBeanPoint>(m_queueBeanPointCollection); // 关联折线的数据集合
            DatSrcQueueBeanPoint.SetXMapping(data => TransView.__HAxis_Queue.ConvertToDouble(data.Date)); // 数据到点x坐标的映射
            DatSrcQueueBeanPoint.SetYMapping(data => data.Number); // 数据到点y坐标的映射
            TransView.__LinePlotter_Queue.AddLineGraph(
                DatSrcQueueBeanPoint,
                new Pen(Brushes.Green, 2),
                new PenDescription("历史在线人数")); // 向绘制器Plotter添加折线

            // 新建调度程序计时器用以更新折线图
            UpdateQueue(null, null);
            m_dispatcherTimer = new DispatcherTimer();
            m_dispatcherTimer.Interval = TimeSpan.FromSeconds(5);
            m_dispatcherTimer.Tick += new EventHandler(UpdateQueue);
            m_dispatcherTimer.Start();



        }

        private void UpdateQueue(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                var resp = nle_api.GetSensorInfo(UDG_.DEVICE_ID, UDG_.SENSOR_QUEUE_NUMBER, m_token); // 获取排队人数信息
                if (!resp.IsSuccess())
                    return;
                var val = resp.ResultObj.Value.ToString(); // 读取排队人数的字符串表达式
                if (string.IsNullOrEmpty(val))
                    val = "0";
                var nbr = QueueNumber = int.Parse(val); // 转换为与字符串表达式等效的排队人数
                var date = DateTime.Now;
                lock (_mutex)
                {
                    Execute.OnUIThreadSync(() =>
                    {
                        m_queueBeanPointCollection.Add(new QueueBeanPoint(date, nbr));
                    }); // 使用调度程序向UI线程递交任务，挂起EnumerableDataSource的DataChanged事件，以更新折线图
                }
            });
        }

        #region 基本功能与消息功能
        /// <summary>
        /// 语音播报
        /// </summary>
        public async void CmdReport()
        {
            if (String.IsNullOrEmpty(ReportMessage))
            {
                _windowManager.ShowMessageBox("播报内容不可为空!", "错误");
                return;
            }
            var isSuccess = await Task<bool>.Factory.StartNew(() =>
            {
                var resp = nle_api.Cmds(UDG_.DEVICE_ID, UDG_.ACTUATOR_REPORT, ReportMessage, m_token);

                return resp.IsSuccess();
            });
            if (isSuccess)
            {
                _windowManager.ShowMessageBox("发送成功!", "提示");
            }
            else
            {
                _windowManager.ShowMessageBox("发送失败!", "提示");
            }
        }

        /// <summary>
        /// 开始取号
        /// </summary>
        public async void CmdTakeStart()
        {
            var isSuccess = await Task<bool>.Factory.StartNew(() =>
            {
                var resp = nle_api.Cmds(UDG_.DEVICE_ID, UDG_.ACTUATOR_TAKE, 1, m_token);
                return resp.IsSuccess();
            });
            if (isSuccess)
            {
                _windowManager.ShowMessageBox("已开始取号!", "提示");
            }
            else
            {
                _windowManager.ShowMessageBox("响应失败!", "提示");
            }
        }

        /// <summary>
        /// 暂停取号
        /// </summary>
        public async void CmdTakeEnd()
        {
            var isSuccess = await Task<bool>.Factory.StartNew(() =>
            {
                var resp = nle_api.Cmds(UDG_.DEVICE_ID, UDG_.ACTUATOR_TAKE, 0, m_token);
                return resp.IsSuccess();
            });
            if (isSuccess)
            {
                _windowManager.ShowMessageBox("已暂停取号!", "提示");
            }
            else
            {
                _windowManager.ShowMessageBox("响应失败!", "提示");
            }
        }

        /// <summary>
        /// 叫号功能
        /// </summary>
        public async void CmdTakeCall()
        {
            var isSuccess = await Task<bool>.Factory.StartNew(() =>
            {
                var resp = nle_api.Cmds(UDG_.DEVICE_ID, UDG_.ACTUATOR_CALL, 1, m_token);
                return resp.IsSuccess();
            });
            if (isSuccess)
            {
                _windowManager.ShowMessageBox("已叫号!", "提示");
            }
            else
            {
                _windowManager.ShowMessageBox("响应失败!", "提示");
            }
        }
        #endregion

        #region 图表功能
        /// <summary>
        /// 导出图表数据
        /// </summary>
        public async void CmdExportChart()
        {
            var result = await Task<bool>.Factory.StartNew(() =>
            {
                var dialog = new SaveFileDialog();
                dialog.Filter = "图表数据 (*.txt)|*.txt";
                if (dialog.ShowDialog() ?? false)
                {
                    lock (_mutex)
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine("记录时间" + "\t" + "排队人数");
                        foreach (var row in m_queueBeanPointCollection)
                        {
                            sb.AppendLine($"{row.Date.ToString()}\t{row.Number}");
                        }
                        using (FileStream fs = new FileStream(dialog.FileName, FileMode.Create, FileAccess.Write))
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            sw.Write(sb.ToString().ToCharArray());
                            Execute.OnUIThreadAsync(() => { _windowManager.ShowMessageBox("导出成功！", "提示"); });
                        }

                    }
                }
                return true;

            });
        }

        /// <summary>
        /// 导入图表数据
        /// </summary>
        public async void CmdImportChart()
        {
            var result = await Task<bool>.Factory.StartNew(() =>
            {

                var dialog = new OpenFileDialog();
                dialog.Filter = "图表数据 (*.txt)|*.txt";
                if (dialog.ShowDialog() ?? false)
                {
                    lock (_mutex)
                    {
                        using (var fs = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read))
                        using (var sw = new StreamReader(fs))
                        {
                            m_queueBeanPointCollection.Clear();
                            sw.ReadLine();
                            string nextLine;
                            while (!String.IsNullOrEmpty(nextLine = sw.ReadLine()))
                            {
                                var split = Regex.Split(nextLine, "\t");
                                var date = split[0];
                                var number = split[1];
                                var point = new QueueBeanPoint(DateTime.Parse(date), int.Parse(number));
                                Execute.OnUIThreadSync(() => { m_queueBeanPointCollection.Add(point); }); // 当前线程与ui线程代码同步
                            }
                            Execute.OnUIThreadAsync(() => { _windowManager.ShowMessageBox("导入成功！", "提示"); });
                        }

                    }
                }
                return true;

            });
        }
        #endregion
    }
}
