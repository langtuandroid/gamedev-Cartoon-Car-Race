using UnityEngine;

public static class Utils
{
    public static string TimeToString(float time)
    {
        var seconds = Mathf.Floor(time % 60.0f);
        var minutes = Mathf.Floor(time / 60.0f);
        var hours = Mathf.Floor(minutes / 60.0f);
        if (hours > 0)
        {
            minutes = minutes - hours * 60.0f;
            return hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
        }

        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }

}
