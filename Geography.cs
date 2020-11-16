using static System.Math;

class Geography {

    public const double EarthRadius = 6371;

    public static double Deg2rad(double deg) {
        return deg * (PI / 180);
    }

    public static double Deg2rad (decimal deg) {
        return Deg2rad((double)deg);
    }

    /// <summary>
    /// Author : Levi
    /// 
    /// Old implementation
    /// </summary>
    //public static double Distance(double latA, double longA, double latB, double longB) {
    //    double deg2radMultiplier = PI / 180;
    //    latA *= deg2radMultiplier;
    //    longA *= deg2radMultiplier;
    //    latB *= deg2radMultiplier;
    //    longB *= deg2radMultiplier;
    //    double radius = 6378.137; //earth mean radius defined by WGS84
    //    double dlon = longB - longA;
    //    return Acos(Sin(latA) * Sin(latB) + Cos(latA) * Cos(latB) * Cos(dlon)) * radius;
    //}

    /// <summary>
    /// Calculates the distance between two coordenates.
    /// </summary>
    /// <returns>The distance in Km.</returns>
    public static double Distance(double latA, double lonA, double latB, double lonB) {
        var dLat = Deg2rad(latB - latA); 
        var dLon = Deg2rad(lonB - lonA);
        var a = Sin(dLat / 2) * Sin(dLat / 2) + Cos(Deg2rad(latA)) * Cos(Deg2rad(latB)) * Sin(dLon / 2) * Sin(dLon / 2);
        var c = 2 * Atan2(Sqrt(a), Sqrt(1 - a));
        return EarthRadius * c; 
    }



}