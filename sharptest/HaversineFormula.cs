namespace sharptest
{
    public class HaversineFormula()
    {
        public const double EarthRadiusKm = 6371.0;

        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // compare latitude and longtitudes of two distances
            double dLat = ConvertToRadians(lat2 - lat1);
            double dLon = ConvertToRadians(lon2 - lon1);

            // radians of two latitudes
            double rLat1 = ConvertToRadians(lat1);
            double rLat2 = ConvertToRadians(lat2);

            // this is the actual haversine formula
            double haversineMathFormula = Math.Pow(Math.Sin(dLat / 2), 2) + Math.Pow(Math.Sin(dLon / 2), 2) * Math.Cos(rLat1) * Math.Cos(rLat2);

            double haversineAngle = 2 * Math.Atan2(Math.Sqrt(haversineMathFormula), Math.Sqrt(1 - haversineMathFormula));

            return EarthRadiusKm * haversineAngle;
        }

        public static double ConvertToRadians(double angleInDegrees)
        {
            return Math.PI / 180.0 * angleInDegrees; 
        }
    }
}