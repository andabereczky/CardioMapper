namespace CardioMapper
{
    using System;

    public class GeoUtil
    {
        /// <summary>
        /// A constant used to convert from lat/long units to kilometers.
        /// </summary>
        private static double R = 6371000.0;

        /// <summary>
        /// Calculates the distance between two points on the surface of
        /// the Earth (in meters).
        /// </summary>
        /// <param name="latitude1"></param>
        /// <param name="longitude1"></param>
        /// <param name="latitude2"></param>
        /// <param name="longitude2"></param>
        /// <returns></returns>
        public static double CalculateDistance(
            double latitude1,
            double longitude1,
            double latitude2,
            double longitude2)
        {
            /*
             * The Haversine Formula:
             * 
             *     dlon = lon2 - lon1
             *     dlat = lat2 - lat1
             *     a = (sin(dlat/2))^2 + cos(lat1) * cos(lat2) * (sin(dlon/2))^2
             *     c = 2 * atan2(sqrt(a), sqrt(1-a))
             *     d = R * c
             *     
             * (from R. W. Sinnott, "Virtues of the Haversine," Sky and Telescope, vol. 68, no. 2, 1984, p. 159)
            */
            latitude1 = DegreesToRadians(latitude1);
            longitude1 = DegreesToRadians(longitude1);
            latitude2 = DegreesToRadians(latitude2);
            longitude2 = DegreesToRadians(longitude2);
            double dLon = longitude2 - longitude1;
            double dLat = latitude2 - latitude1;
            double a = Math.Pow(Math.Sin(dLat / 2.0), 2) + Math.Cos(latitude1) * Math.Cos(latitude2) * Math.Pow(Math.Sin(dLon / 2.0), 2);
            double c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return GeoUtil.R * c;
        }

        public static double DegreesToRadians(double degrees)
        {
            return (degrees * Math.PI) / 180.0;
        }

        public static double RadiansToDegrees(double radians)
        {
            return (radians * 180.0) / Math.PI;
        }
    }
}
