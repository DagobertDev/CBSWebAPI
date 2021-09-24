namespace CBSWebAPI.Models
{
	public readonly struct GeoPosition
	{
		public GeoPosition(float longitude, float latitude)
		{
			Longitude = longitude;
			Latitude = latitude;
		}
		
		public float Longitude { get; init; }
		public float Latitude { get; init; }
	}
}
