package org.thingmodel;

public abstract class Location {
	public double X;
	public double Y;

	/** This parameter is an object, because
	 *	it is optional and so nullable
	 */
	public Double Z;

	/**
	 * Name of the location system.
	 * 
	 * Examples : WGS84, ETRS89...
	 */
	public String System;

	/**
	 * You cannot use this constructor directly.
	 * 
	 * Location.{Point|LatLng|Equatorial} should be used instead.
	 */
	protected Location(double x, double y, Double z) {
		X = x;
		Y = y;
		Z = z;
	}
	
	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		result = prime * result + ((System == null) ? 0 : System.hashCode());
		long temp;
		temp = Double.doubleToLongBits(X);
		result = prime * result + (int) (temp ^ (temp >>> 32));
		temp = Double.doubleToLongBits(Y);
		result = prime * result + (int) (temp ^ (temp >>> 32));
		result = prime * result + ((Z == null) ? 0 : Z.hashCode());
		return result;
	}

	/**
	 * Return true if the location contain the same values.
	 * 
	 * A location.Point and a Location.LatLng are not the same
	 * even if the values are identical.
	 * 
	 */
	@Override
	public boolean equals(Object obj) {
		if (this == obj)
			return true;
		if (obj == null)
			return false;
		if (!(obj instanceof Location))
			return false;
		Location other = (Location) obj;
		if (System == null) {
			if (other.System != null)
				return false;
		} else if (!System.equals(other.System))
			return false;
		if (Double.doubleToLongBits(X) != Double.doubleToLongBits(other.X))
			return false;
		if (Double.doubleToLongBits(Y) != Double.doubleToLongBits(other.Y))
			return false;
		if (Z == null) {
			if (other.Z != null)
				return false;
		} else if (!Z.equals(other.Z))
			return false;
		return true;
	}

	@Override
	public String toString() {
		return "Location [X=" + X + ", Y=" + Y + ", "
				+ (Z != null ? "Z=" + Z + ", " : "")
				+ (System != null ? "System=" + System : "") + "]";
	}

	/**
	 *	Simple location value.
	 *	Often used in videogames.
	 */
	public static class Point extends Location {

		public Point() {
			this(0.0,0.0,null);
		}
		
		public Point(double x, double y) {
			this(x,y,null);
		}
		
		public Point(double x, double y, Double z) {
			super(x, y, z);
		}
	}
	
	/**
	 *	Latitude longitude representation, often used for WGS 84 GPS localizations.
	 *	And if the System property is null, it's considered by default as a WGS 84 LatLng system.
	 */
	public static class LatLng extends Location {
		public LatLng() {
			super(0.0,0.0,null);
		}
		
		public LatLng(double latitude, double longitude) {
			super(latitude, longitude, null);
		}
		
		public LatLng(double latitude, double longitude, Double altitude) {
			super(latitude, longitude, altitude);
		}
		
		public double getLatitude() {
			return X;
		}
		
		public void setLatitude(double latitude) {
			X = latitude;
		}
		
		public double getLongitude() {
			return Y;
		}
		
		public void setLongitude(double longitude) {
			Y = longitude;
		}
		
		public Double getAltitude() {
			return Z;
		}
		
		public void setAltitude(Double altitude) {
			Z = altitude;
		}
		
	}

    /**
     * For location in space.
     */
	public static class Equatorial extends Location {
		public Equatorial() {
			super(0.0,0.0,0.0);
		}
		
		public Equatorial(double rightAscension, double declination, double hourAngle) {
			super(rightAscension, declination, hourAngle);
		}
		
		public double getRightAscension() {
			return X;
		}
		
		public void setRightAscension(double rightAscension) {
			X = rightAscension;
		}
		
		public double getDeclination() {
			return Y;
		}
		
		public void setDeclination(double declination) {
			Y = declination;
		}
		
		public double getHourAngle() {
			return Z;
		}
		
		public void setHourAngle(Double hourAngle) {
			Z = hourAngle;
		}
		
	}
}
