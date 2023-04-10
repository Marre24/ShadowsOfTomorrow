﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowsOfTomorrow
{
    public class Speech
    {
        public Dialogue Dialogue => dialogue;
        protected Dialogue dialogue;

        public Speech(string name) 
        {
            dialogue = new(name);
        }

        public virtual void Answer(int choice) { }
        
        
    }
}
