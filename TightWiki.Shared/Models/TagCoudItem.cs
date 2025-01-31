﻿using System;

namespace TightWiki.Shared.Models
{
    public class TagCoudItem
    {
        public String Name = "";
        public String HTML = "";
        public int Rank = 0;

        public TagCoudItem(string name, int rank, string html)
        {
            Name = name;
            HTML = html;
            Rank = rank;
        }

        public static int CompareItem(TagCoudItem x, TagCoudItem y)
        {
            return string.Compare(x.Name, y.Name);
        }
    }
}
