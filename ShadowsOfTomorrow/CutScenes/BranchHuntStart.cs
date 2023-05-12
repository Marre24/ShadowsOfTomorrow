using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShadowsOfTomorrow
{
    public class BranchHuntStart
    {
        public bool HaveBeenTriggered { get; internal set; }
        public bool HaveEnded { get; internal set; }
        private Rectangle rec;
        private double timeSinceStop = 0;
        private const double interval = 2;
        private bool haveShownCutscene = false;

        public BranchHuntStart()
        { 
            HaveBeenTriggered = false;
            HaveEnded = false;
        }

        public void Play(Map map, Camera camera, Player player, GameTime gameTime)
        {
            if (player.playerMovement.HorizontalSpeed != 0)
                return;

            camera.Follow(rec, map, true);

            if (camera.Window.Left - 96 - 2 >= map.Left)
            {
                if (haveShownCutscene)
                    rec.Location -= new Point(4, 0);
                rec.Location -= new Point(2, 0);
                player.Facing = Facing.Left;
            }
            else if (map.branchWall.HitBox.Right < 15 * 48)
            {
                map.branchWall.Move(haveShownCutscene);
                timeSinceStop = gameTime.TotalGameTime.TotalSeconds;
            }
            else if (gameTime.TotalGameTime.TotalSeconds >= timeSinceStop + interval || haveShownCutscene)
                End(player);
        }

        private void End(Player player)
        {
            haveShownCutscene = true;
            HaveEnded = true;
            player.CurrentAction = Action.Standing;
        }

        public void Start(Player player)
        {
            HaveBeenTriggered = true;
            player.CurrentAction = Action.WachingCutScene;
            rec = new(new(player.HitBox.Left, player.HitBox.Bottom - 32), new(32, 32));
        }

        public void Reset()
        {
            HaveBeenTriggered = false;
            HaveEnded = false;
        }
    }
}
