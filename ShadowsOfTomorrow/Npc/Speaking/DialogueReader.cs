using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ShadowsOfTomorrow
{
    public static class DialogueReader
    {
        public static Dictionary<string, Dictionary<string, string>> GetDialogueFor(string npcName)
        {
            Dictionary<string, Dictionary<string, string>> dialogue = new();

            TextReader reader = new StreamReader("Content/TextFiles/Dialogue.txt");
            List<string> textFromFile = reader.ReadToEnd().Replace("\n", string.Empty).Split("\r").ToList();
            for (int i = textFromFile.IndexOf(npcName) + 1; i < textFromFile.Count; i++)
            {
                if (!textFromFile[i].Contains('|') && textFromFile[i] != npcName)
                    break;

                List<string> keyValue = textFromFile[i].Split('|').ToList();
                string key = keyValue[0];
                List<string> value = keyValue[1].Replace("[", string.Empty).Split(']').ToList();
                value.Remove("");
                Dictionary<string, string> questionsAndAnswers = new();
                foreach (string str in value)
                    questionsAndAnswers.Add(str.Split(';')[0], str.Split(';')[1]);

                dialogue.Add(key, questionsAndAnswers);

            }

            return dialogue;
        }

        public static List<string> GetDialogueFor(string npcName, bool isBoss)
        {
            List<string> value = new();
            TextReader reader = new StreamReader("Content/TextFiles/Dialogue.txt");
            List<string> textFromFile = reader.ReadToEnd().Replace("\n", string.Empty).Split("\r").ToList();
            value = textFromFile[textFromFile.IndexOf(npcName) + 1].Split(';').ToList();
            value.Remove("");
            return value;
        }
    }
}
