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
        public List<string> bossDialogue = new();
        public Dictionary<string, Dictionary<string, string>> dialogue = new();

        public bool IsBoss { get; set; }

        public Dialogue(string name, bool isBoss) 
        {
            IsBoss = isBoss;
            if (isBoss)
            {
                bossDialogue = DialogueReader.GetDialogueFor(name, isBoss);
                return;
            }
            dialogue = DialogueReader.GetDialogueFor(name);
        }

        public List<string> GetQuestions(string key)
        {
            if (dialogue.ContainsKey(key))
                return dialogue[key].Keys.ToList();
            return dialogue["First"].Keys.ToList();
        }

        public string GetAnswer(string key, int i)
        {
            if (dialogue.ContainsKey(key))
                return dialogue[key].Values.ElementAt(i);
            return "Error";
        }

        internal string GoTo(string displayAnswer)
        {
            List<string> gotos = new();
            foreach (var gotoAndValue in dialogue)
                gotos.Add(gotoAndValue.Key);
            
            if (gotos.Contains(displayAnswer))
                return displayAnswer;
            return "First";
        }
    }
}
