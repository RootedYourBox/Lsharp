using LeagueSharp;
using LeagueSharp.Common;
using System;

namespace DProject
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
                    //PrintMessage("Syndra loaded!");
                    break;
                default:
                   PrintMessage(ChampionName);
                    break;
            }
        }
        public static void PrintMessage(string msg)
        {
            Game.PrintChat("<font color=\"#6699ff\"><b>DProject: </b></font> <font color=\"#FFFFFF\">" + msg + "</font>");
        }
    }
}
