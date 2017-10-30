using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMQService.Model
{
    /// <summary>
    /// 考生信息
    /// </summary>
    public struct StudentInfo
    {
        private string kch;    //考车号
        private string bz;    //备注（车牌号）
        private string kscx; //考试车型
        private string xingming; //姓名
        private string xb; //性别
        private string date; //日期
        private string lsh;  //流水号
        private string sfzmbh;   //身份证明编号
        private string jxmc;   //驾校名称
        private string ksy1;   //考试员1
        private string ksyyDes;   //考试原因
        private Byte[] arrayZp;   //照片
        private Byte[] arrayMjzp;   //门禁照片

        public StudentInfo(string _kch, string _bz, string _kscx, string _xingming, string _xb, string _date, string _lsh, string _sfzmbh,
            string _jxmc, string _ksy1, string _ksyyDes, Byte[] _arrayZp, Byte[] _arrayMjzp)
        {
            this.kch = _kch;
            this.bz = _bz;
            this.kscx = _kscx;
            this.xingming = _xingming;
            this.xb = _xb;
            this.date = _date;
            this.lsh = _lsh;
            this.sfzmbh = _sfzmbh;
            this.jxmc = _jxmc;
            this.ksy1 = _ksy1;
            this.ksyyDes = _ksyyDes;
            this.arrayZp = _arrayZp;
            this.arrayMjzp = _arrayMjzp;
        }

        public string Kch
        {
            get { return kch; }
            set { kch = value; }
        }

        public string Bz
        {
            get { return bz; }
            set { bz = value; }
        }
        public string Kscx
        {
            get { return kscx; }
            set { kscx = value; }
        }

        public string Xingming
        {
            get { return xingming; }
            set { xingming = value; }
        }

        public string Xb
        {
            get { return xb; }
            set { xb = value; }
        }

        public string Date
        {
            get { return date; }
            set { date = value; }
        }

        public string Lsh
        {
            get { return lsh; }
            set { lsh = value; }
        }

        public string Sfzmbh
        {
            get { return sfzmbh; }
            set { sfzmbh = value; }
        }

        public string Jxmc
        {
            get { return jxmc; }
            set { jxmc = value; }
        }

        public string Ksy1
        {
            get { return ksy1; }
            set { ksy1 = value; }
        }

        public string KsyyDes
        {
            get { return ksyyDes; }
            set { ksyyDes = value; }
        }

        public Byte[] ArrayZp
        {
            get { return arrayZp; }
            set { arrayZp = value; }
        }

        public Byte[] ArrayMjzp
        {
            get { return arrayMjzp; }
            set { arrayMjzp = value; }
        }



    }
}
