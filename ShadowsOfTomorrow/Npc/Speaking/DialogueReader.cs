using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowsOfTomorrow
{
    public static class DialogueReader
    {
        public static Dialogue GetDialogueFor(string npcName)
        {
            Dialogue dialogue = new Dialogue();

            using (TextReader reader = new StreamReader("Content/TextFiles/Dialogue.txt"))
            {
                dialogue.dialogue.Add(reader.ReadLine());
            }


            return dialogue;
        }

    }
}
