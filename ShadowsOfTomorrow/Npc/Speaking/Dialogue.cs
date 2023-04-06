using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowsOfTomorrow
{
    public class Dialogue
    {
        public List<string> dialogue = new List<string>();         //fix better logic

        public Dialogue() 
        {
        }

        public void AddDialogue(List<string> dialogue)
        {
            this.dialogue = dialogue;
        }

        public string GetMessage()
        {
            return dialogue[0];
        }


    }
}
