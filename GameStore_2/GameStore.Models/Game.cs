using System;

namespace GameStore.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Studio { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }

        public string GameMode { get; set; } = string.Empty;
        public int CopiesSold { get; set; }
    }
}