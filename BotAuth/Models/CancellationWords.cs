using System.Collections.Generic;
using System.Linq;

namespace BotAuth.Models
{
    public class CancellationWords
    {
        public static List<string> GetCancellationWords() =>
            AuthText.CancellationWords.Split(',').ToList();
    }
}
