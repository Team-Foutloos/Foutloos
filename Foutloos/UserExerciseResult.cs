﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foutloos
{
    public class UserExerciseResult
    {
        public string Name { get; set; }
        public string Difficulty { get; set; }
        public string Type { get; set; }
        public int Mistakes { get; set; }
        public int WPM { get; set; }
        public int CPM { get; set; }

        public UserExerciseResult()
        {

        }
        public override string ToString()
        {
            return Name + " " + WPM + " " + Mistakes;
        }
    }

   
}