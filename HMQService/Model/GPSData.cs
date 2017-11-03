using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMQService.Model
{
    /// <summary>
    /// 车载GPS数据
    /// </summary>
    public struct GPSData
    {
        double longitude;   //经度
        double latitude;  //纬度
        float directionAngle;   //方向角
        float speed;    //速度
        float mileage;  //里程

        public GPSData(double _longitude, double _latitude, float _directionAngle, float _speed, float _mileage)
        {
            this.longitude = _longitude;
            this.latitude = _latitude;
            this.directionAngle = _directionAngle;
            this.speed = _speed;
            this.mileage = _mileage;
        }

        public double Longitude
        {
            get { return longitude; }
        }

        public double Latitude
        {
            get { return latitude; }
        }

        public float DirectionAngle
        {
            get { return directionAngle; }
        }

        public float Speed
        {
            get { return speed; }
        }

        public float Mileage
        {
            get { return mileage; }
        }
    }
}
