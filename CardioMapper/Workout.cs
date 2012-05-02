using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace CardioMapper
{
    public class Workout
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double LastLatitude { get; set; }
        public double LastLongitude { get; set; }

        public double Altitude { get; set; }

        public TimeSpan Time { get; set; }
        public DateTimeOffset LastTime { get; set; }
        
        public double Distance { get; set; }
        
        public double MaxSpeed { get; set; }
        public double Speed { get; set; }
        
        public double AverageSpeed
        {
            get
            {
                if (this.Time.TotalSeconds < 0.001)
                {
                    return 0.0;
                }
                else
                {
                    return this.Distance / this.Time.TotalSeconds;
                }
            }
        }

        public Workout()
        {
            this.Latitude = Double.NaN;
            this.Longitude = Double.NaN;
            this.LastLatitude = Double.NaN;
            this.LastLongitude = Double.NaN;

            this.Altitude = Double.NaN;

            this.Time = TimeSpan.Zero;
            this.LastTime = DateTimeOffset.MinValue;

            this.Distance = 0.0;

            this.MaxSpeed = 0.0;
            this.Speed = 0.0;
        }

        public double Update(double latitude, double longitude, double altitude, double speed, DateTimeOffset timestamp)
        {
            // Save position values.
            if (!latitude.Equals(Double.NaN) && !longitude.Equals(Double.NaN))
            {
                this.LastLatitude = this.Latitude;
                this.LastLongitude = this.Longitude;
                this.Latitude = latitude;
                this.Longitude = longitude;
            }

            // Save altitude value.
            this.Altitude = altitude;

            // Calculate the time since the last read.
            TimeSpan t = ComputeCurrentTime(timestamp);

            // Save timing values.
            this.Time = this.Time.Add(t);
            this.LastTime = timestamp;

            // Calculate the distance traveled since the last read.
            double d = 0.0;
            if (!this.LastLatitude.Equals(Double.NaN) && !this.LastLongitude.Equals(Double.NaN))
            {
                d = GeoUtil.CalculateDistance(this.LastLatitude, this.LastLongitude, this.Latitude, this.Longitude);
            }
            
            // Save distance.
            this.Distance += d;

            // Save current speed.
            if (!Double.IsNaN(speed))
            {
                this.Speed = speed;
            }
            else
            {
                if (t.TotalSeconds < 0.001)
                {
                    this.Speed = 0.0;
                }
                else
                {
                    this.Speed = d / t.TotalSeconds;
                }
            }

            // Save maximum speed.
            if (this.Speed > this.MaxSpeed)
            {
                this.MaxSpeed = this.Speed;
            }

            return d;
        }

        public TimeSpan ComputeCurrentTime(DateTimeOffset timestamp)
        {
            TimeSpan result;

            if (this.LastTime != DateTimeOffset.MinValue)
            {
                result = timestamp - this.LastTime;
            }
            else
            {
                result = new TimeSpan(0);
            }

            return result;
        }
    }
}
