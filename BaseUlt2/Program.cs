using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using LeagueSharp;

namespace BaseUlt2
{
    class Program
    {
        static void Main(string[] args)
        {
            Game.OnGameStart += Game_OnGameStart;

            if (Game.Mode == GameMode.Running)
                Game_OnGameStart(new EventArgs());
        }

        private static void Game_OnGameStart(EventArgs args)
        {
            Game.PrintChat("<font color=\"#FF0000\">BaseUlt2 NOT LOADED. BaseUlt2 IS NOW INTEGRATED INTO UltimateCarry.-</font>");
            Game.PrintChat("<font color=\"#FF0000\">BaseUlt2 NOT LOADED. BaseUlt2 IS NOW INTEGRATED INTO UltimateCarry.-</font>");
            Game.PrintChat("<font color=\"#FF0000\">BaseUlt2 NOT LOADED. BaseUlt2 IS NOW INTEGRATED INTO UltimateCarry.-</font>");
        }
    }
}
