using LeagueSharp;
using LeagueSharp.Common;
using System;

namespace MidSeries
{
    class Program
    {
        public static string ChampionName;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            ChampionName = ObjectManager.Player.BaseSkinName;

            switch (ChampionName)
            {
                case "Syndra":
                    new Syndra();
                    break;
                case "Katarina":
                    new Katarina();
                    break;
                case "Ahri":
                    new Ahri();
                    break;
                default:
                   PrintMessage("Chưa hỗ trợ champion " + ChampionName);
                    break;
            }
        }
        public static void PrintMessage(string msg)
        {
            Game.PrintChat("<font color=\"#6699ff\"><b>DProject: </b></font> <font color=\"#FFFFFF\">" + msg + "</font>");
        }
    }
}
