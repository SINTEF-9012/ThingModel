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
	
	/**
	 * Return true if the location contain the same values.
	 * 
	 * A location.Point and a Location.LatLng are not the same
	 * even if the values are identical.
	 * 
	 * The comparison doesn't manage issues with numbers precision.
	 * If a value is rounded, the location is not the same.
	 */
	public boolean Compare(Location other) {
		return other != null &&
			X == other.X &&
			Y == other.Y &&
			(Z == other.Z || (Z != null && Z.equals(other.Z))) &&
			(System == other.System || (System != null && System.equals(other.System)));
			
	}
	
	/**
	 *	Simple location value.
	 *	Often used in videogames.
	 */
	public class Point extends Location {

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
	public class LatLng extends Location {
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
			return Y;
		}
		
		public void setLatitude(double latitude) {
			Y = latitude;
		}
		
		public double getLongitude() {
			return X;
		}
		
		public void setLongitude(double longitude) {
			X = longitude;
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
	public class Equatorial extends Location {
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
