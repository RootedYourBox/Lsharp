using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace BaseUlt2
{
    class Program
    {
        public static Helper Helper;
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            Helper = new Helper();
            var baseUlt = new BaseUlt();
        }
    }
}
