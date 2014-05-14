namespace ThingModel
{
    public abstract class Location
    {
        public double X;
        public double Y;
        
        /** This parameter is optionnal **/
        public double? Z;

        /**
         * Name of the location system.
         * 
         * Examples, WGS84, ETRS89...
         */
        public string System;
        
        /**
         * You cannot use this constructor directly.
         *  Location.{Point|LatLng|Equatorial} should be used instead.
         */
        protected Location(double x, double y, double? z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /**
         * Return true if the location contain the same values.
         * 
         * A Location.Point and a Location.LatLng are the same
		 * if the values are the same.
         * 
         * The comparaison doesn't manage issues with precision numbers. If
         * a value is rounded, the location is not the same.
         */
        public bool Compare(Location other)
        {
            // Double comparaison without Epsilon mouahahaha
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return other != null &&
				X == other.X &&
				Y == other.Y &&
				Z == other.Z &&
				System == other.System;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /**
         * Used for videogames and for default coordinates systems.
         */
        public class Point : Location
        {
            public Point(double x = 0.0, double y = 0.0, double? z = null)
                : base(x, y, z)
            {
            }
        }

        /**
         * For location on planets.
         * 
         * Will be used often with Earth WGS-84 (GPS).
         */
        public class LatLng : Location
        {
            public LatLng(double latitude = 0.0, double longitude = 0.0, double? altitude = null)
                : base(latitude, longitude, altitude)
            {
            }

            public double Latitude
            {
                get { return X; }
                set { X = value; }
            }

            public double Longitude
            {
                get { return Y; }
                set { Y = value; }
            }

            public double? Altitude
            {
                get { return Z; }
                set { Z = value; }
            }
        }

        /**
         * For location in space.
         */
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


	    protected bool Equals(Location other)
	    {
		    return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && string.Equals(System, other.System);
	    }

	    public override bool Equals(object obj)
	    {
		    if (ReferenceEquals(null, obj)) return false;
		    if (ReferenceEquals(this, obj)) return true;
		    if (obj.GetType() != GetType()) return false;
		    return Equals((Location) obj);
	    }

	    public override int GetHashCode()
	    {
		    unchecked
		    {
// ReSharper disable NonReadonlyFieldInGetHashCode
			    int hashCode = X.GetHashCode();
			    hashCode = (hashCode*397) ^ Y.GetHashCode();
			    hashCode = (hashCode*397) ^ Z.GetHashCode();
			    hashCode = (hashCode*397) ^ (System != null ? System.GetHashCode() : 0);
// ReSharper restore NonReadonlyFieldInGetHashCode
			    return hashCode;
		    }
	    }
    }
}