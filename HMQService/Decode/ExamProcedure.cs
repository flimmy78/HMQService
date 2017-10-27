using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using HMQService.Common;

namespace HMQService.Decode
{
    /// <summary>
    /// 考试过程信息处理
    /// </summary>
    public class ExamProcedure
    {
        private int m_userId;
        private int m_kch;
        private int m_thirdPassiveHandle;
        private int m_fourthPassiveHandle;
        private Graphics gThirdPic;
        private Graphics gFourthPic;
        private Bitmap bmThirdPic;
        private Bitmap bmFourthPic;
        private Font font;
        private Brush brush;
        private Image imgTbk;
        private Image imgMark;
        private Image imgHgorbhg;
        private Image imgTime;
        private Image imgXmp;

        public ExamProcedure()
        {
            m_userId = -1;
            m_kch = -1;
            m_thirdPassiveHandle = -1;
            m_fourthPassiveHandle = -1;
            font = new Font("宋体", 16, FontStyle.Regular);
            brush = new SolidBrush(Color.FromArgb(255, 255, 255));

            imgTbk = Image.FromFile(BaseDefine.IMG_PATH_TBK);
            imgMark = Image.FromFile(BaseDefine.IMG_PATH_MARK);
            imgHgorbhg = Image.FromFile(BaseDefine.IMG_PATH_HGORBHG);
            imgTime = Image.FromFile(BaseDefine.IMG_PATH_TIME);
            imgXmp = Image.FromFile(BaseDefine.IMG_PATH_XMP);
        }

        ~ExamProcedure()
        { }

        public bool Init(int userId, int kch, int thirdPH, int fourthPH)
        {
            m_userId = userId;
            m_kch = kch;
            m_thirdPassiveHandle = thirdPH;
            m_fourthPassiveHandle = fourthPH;

            bmThirdPic = new Bitmap(imgTbk);
            bmFourthPic = new Bitmap(imgMark);
            gThirdPic = Graphics.FromImage(bmThirdPic);
            gFourthPic = Graphics.FromImage(bmFourthPic);

            string strInit = string.Format(BaseDefine.STRING_INIT_CAR, m_kch);
            gThirdPic.DrawString(strInit, font, brush, new Rectangle(0, 6, 350, 34));
            


            //临时
            SavePngFile();

            return true;
        }

        private bool SavePngFile()
        {
            bmThirdPic.Save(@"D:\image\thirdPic.png");
            bmFourthPic.Save(@"D:\image\fourthPic.png");

            BaseMethod.MakeAviFile(@"D:\image\third.avi", bmThirdPic);
            BaseMethod.MakeAviFile(@"D:\image\fourth.avi", bmFourthPic);

            return true;
        }

    }
}
