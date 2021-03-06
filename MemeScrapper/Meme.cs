﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeScrapper
{
    class Meme
    {
        public string URL { get; set; }
        public string Name { get; set; }
        public int Views { get; set; }
        public string[] Tags { get; set; }
        public byte[] ImageByteArray { get; set; }

        public override string ToString()
        {
            return "URL: " + URL + "\nName: " + Name + "\nViews: " + Views + "\nTags " + Tags + "\nImageByeArray " + ImageByteArray;
        }
    }
}
