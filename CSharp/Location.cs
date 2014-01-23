namespace ThingModel
{
    public class Location
    {
        public double X;
        public double Y;
        public double? Z;
        public string System;
        
        protected Location(double x, double y, double? z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public bool Compare(Location other)
        {
            // Double comparaison without Epsilon mouahahaha
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return other != null && X == other.X && Y == other.Y && Z == other.Z;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public class Point : Location
        {
            public Point(double x = 0.0, double y = 0.0, double? z = null)
                : base(x, y, z)
            {
            }
        }

        public class LatLng : Location
        {
            public LatLng(double latitude = 0.0, double longitude = 0.0, double? altitude = null)
                : base(latitude, longitude, altitude)
            {
            }

            public double Latitude
            {
                get { return Y; }
                set { Y = value; }
            }

            public double Longitude
            {
                get { return X; }
                set { X = value; }
            }

            public double? Altitude
            {
                get { return Z; }
                set { Z = value; }
            }
        }

        public class Equatorial : Location
        {
            public Equatorial(double rightAscension = 0.0, double declination = 0.0, double hourAngle = 0.0)
                : base(rightAscension, declination, hourAngle)
            {
            }

            public double RightAscension
            {
                get { return Y; }
                set { Y = value; }
            }

            public double Declination
            {
                get { return X; }
                set { X = value; }
            }

            public double HourAngle
            {
                get { return Z != null ? (double) Z : 0.0; }
                set { Z = value; }
            }
        }
    }
}