using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace TongHop
{
	class LS
	{
		public static Obj_AI_Hero Player = ObjectManager.Player;
		public static IEnumerable<Obj_AI_Hero> AllHeros = ObjectManager.Get<Obj_AI_Hero>();
		public static IEnumerable<Obj_AI_Hero> AllHerosFriend = ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsAlly);
		public static IEnumerable<Obj_AI_Hero> AllHerosEnemy = ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy);
		
		//public Champion Champion;
		//public Orbwalker Orbwalker;

		public static Menu Menu;

		public LS()
		{
			Game.PrintChat("****  Tong Hop Loaded! ****");
			Game.PrintChat("Beta version v 0.1");
			//Game.PrintChat("This is a Beta version, not all is active,");
			Game.PrintChat("***************************");

			Player = ObjectManager.Player;
            Menu = new Menu("Tong Hop", Player.ChampionName + "Assembly", true);

			var trackerMenu = new Menu("Tracker", "Tracker");
			Tracker.AddtoMenu(trackerMenu);

			//var tsMenu = new Menu("Primes TargetSelector", "Primes_TS");
			//TargetSelector.AddtoMenu(tsMenu);

			var orbwalkMenu = new Menu("Orbwalker", "Orbwalker");
			Orbwalker.AddtoMenu(orbwalkMenu);

			var activatorMenu = new Menu("Activator", "Activator");
			Activator.AddtoMenu(activatorMenu);
            var loadbaseult = true;
            //switch (Player.ChampionName)
            //{
            //    case "Ashe":
            //        loadbaseult = true;
            //        break;
            //    case "Draven":
            //        loadbaseult = true;
            //        break;
            //   case "Ezreal":
            //        loadbaseult = true;
            //        break;
            //    case "jinx":
            //        loadbaseult = true;
            //        break;
            //}
            if (loadbaseult)
            {
                var baseUltMenu = new Menu("BaseUlt", "BaseUlt");
                BaseUlt.AddtoMenu(baseUltMenu);
            }

			//if(Utility.Map.GetMap()._MapType == Utility.Map.MapType.SummonersRift ||
			//	Utility.Map.GetMap()._MapType == Utility.Map.MapType.TwistedTreeline)
			//{
			//	var tarzanMenu = new Menu("Primes Tarzan", "Primes_Tarzan");
			//	Jungle.AddtoMenu(tarzanMenu);
			//}



			Menu.AddToMainMenu();

			
		}

		

		
	}
}
