﻿namespace DataAccess
{
    public class Exchange
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Symbol> Symbols { get; set; }

    }
}
