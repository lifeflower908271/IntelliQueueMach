using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace QueueMach.Model
{
    /// <summary>
    /// 存储可索引的图表数据（y：排队人数/x：日期时间）
    /// </summary>
    public class QueueBeanPointCollection : RingArray <QueueBeanPoint>
    {
        private const int TOTAL_POINTS = 10;

        public QueueBeanPointCollection()
            : base(TOTAL_POINTS) // here i set how much values to show 
        {    
        }
    }

    public class QueueBeanPoint
    {        
        public DateTime Date { get; set; }
        
        public int Number { get; set; }

        public QueueBeanPoint(DateTime date, int number)
        {
            Date = date;
            Number = number;
        }
    }
}
