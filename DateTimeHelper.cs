using System;

class DateTimeHelper {

    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp) {
        var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        try {
            return dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        } catch (Exception) {
            return DateTime.MinValue;
        }
    }

    public static int ToUnixTimestamp(DateTime dt) {
        return (int)(dt.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }

    public static int CurrentUnixTimeStamp {
        get {
            return (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }

}