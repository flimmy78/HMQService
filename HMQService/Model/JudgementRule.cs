using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMQService.Model
{
    /// <summary>
    /// 扣分规则
    /// </summary>
    public struct JudgementRule
    {
        string judgementType;   //扣分类型
        int points; //扣除分数

        public JudgementRule(string type, int ps)
        {
            this.judgementType = type;
            this.points = ps;
        }

        public string JudgementType
        {
            get { return judgementType; }
            set { judgementType = value; }
        }

        public int Points
        {
            get { return points; }
            set { points = value; }
        }
    }
}
