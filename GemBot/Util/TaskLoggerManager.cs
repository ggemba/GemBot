using System;

namespace GemBot.Util
{
    public class TaskLoggerManager
    {
        public ulong UserId { get; set; }
        public string Method { get; set; }
        public DateTime DateStart { get; set; }

        public static TaskLoggerManager CreateTask(ulong userId, string method)
        {
            return new TaskLoggerManager
            {
                UserId = userId,
                Method = method,
                DateStart = DateTime.Now
            };
        }
    }
}
