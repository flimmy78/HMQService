using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMQService.Model
{
    //项目信息
    public class XmInfo
    {
        int kch;   //考车号
        int xmCode; //项目编号

        public XmInfo(int _kch, int _xmCode)
        {
            this.kch = _kch;
            this.xmCode = _xmCode;
        }

        public int Kch
        {
            get { return kch; }
            set { kch = value; }
        }

        public int XmCode
        {
            get { return xmCode; }
            set { xmCode = value; }
        }
    }
}
