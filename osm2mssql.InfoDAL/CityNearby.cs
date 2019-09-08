namespace osm2mssql.InfoDAL
{
	public class CityNearby
	{
		public CityInformation Nearest { get; set; }
		public CityInformation NearestTown { get; set; }
		public CityInformation NearestCity { get; set; }
	}
}