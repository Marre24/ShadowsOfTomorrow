using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowsOfTomorrow
{
    public class Keybinds
    {
        public Dictionary<string, Keys> AllKeys => new() { { "RightKey", RightKey }, { "LeftKey", LeftKey }, { "CrouchKey", CrouchKey }, { "JumpKey", JumpKey }, { "TalkKey", TalkKey },
            { "AttackKey", AttackKey }, { "AccelerateKey", AccelerateKey },  { "DialogueDown", DialogueDown }, { "DialogueUp", DialogueUp }, { "SelectText", SelectText }};
        public Dictionary<string, Keys> MovementKeys => new() { { "RightKey", RightKey }, { "LeftKey", LeftKey }, { "CrouchKey", CrouchKey }, { "JumpKey", JumpKey }, { "TalkKey", TalkKey }, 
            { "AttackKey", AttackKey }, { "AccelerateKey", AccelerateKey }};
        public Dictionary<string, Keys> DialogueKeys => new() { { "DialogueDown", DialogueDown }, { "DialogueUp", DialogueUp }, { "SelectText", SelectText } };

        public Keys RightKey { get; set; }
        public Keys LeftKey { get; set; }
        public Keys DialogueDown { get; set; }
        public Keys DialogueUp { get; set; }
        public Keys SelectText { get; set; }
        public Keys CrouchKey { get; set; }
        public Keys JumpKey { get; set; }
        public Keys TalkKey { get; set; }
        public Keys AttackKey { get; set; }
        public Keys AccelerateKey { get; set; }

        public Keybinds() 
        {
            RightKey = Keys.D;
            LeftKey = Keys.A;
            DialogueDown = Keys.S;
            DialogueUp = Keys.W;
            SelectText = Keys.Space;
            CrouchKey = Keys.S;
            JumpKey = Keys.Space;
            TalkKey = Keys.K;
            AttackKey = Keys.J;
            AccelerateKey = Keys.LeftShift;
        }

    }
}
