using System;

namespace GemBot.Util
{
    public class Date
    {
        public static int GetMinutesBetweenDates(DateTime date, DateTime dateCompare)
        {
            TimeSpan difference = date - dateCompare;
            return Convert.ToInt32(difference.TotalMinutes);
        }

        public static string GetResponseTimeStr(DateTimeOffset dateIni)
        {
            return $"(Tempo de resposta: {Math.Round((dateIni.UtcDateTime - DateTimeOffset.Now.UtcDateTime).TotalSeconds, 3)}ms).";
        }
    }
}
