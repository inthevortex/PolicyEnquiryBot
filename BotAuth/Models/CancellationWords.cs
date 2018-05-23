using System.Collections.Generic;
using System.Linq;

namespace BotAuth.Models
{
    public static class CancellationWords
    {
        public static List<string> GetCancellationWords() =>
            AuthText.CancellationWords.Split(',').ToList();
    }
}
