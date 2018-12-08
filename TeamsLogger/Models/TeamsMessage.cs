using System;

namespace TeamsLogger.Models
{
    [Serializable]
    public class TeamsMessage

    {
        public string Text { get; set; }
        public string ThemeColor { get; set; }
    }
}
