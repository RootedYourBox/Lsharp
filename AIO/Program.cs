using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace AIO
{
	class Program
	{
		public const int LocalVersion = 1; //for update
        public const String Version = "1.0.1.*";

		
		public static Menu Menu;
		public static Orbwalking.Orbwalker Orbwalker;
		
        public static Helper Helper;

		// ReSharper disable once UnusedParameter.Local
		private static void Main(string[] args)
		{
			CustomEvents.Game.OnGameLoad  += Game_OnGameLoad;
		}

		private static void Game_OnGameLoad(EventArgs args)
		{
			//AutoUpdater.InitializeUpdater();
			//Chat.Print("AIO Version " + LocalVersion + " load ...");
			Helper = new Helper();
			Menu = new Menu("AIO", "AIO", true);
            {
                //var AIO = Menu.AddSubMenu(new Menu("AIO by PawPaw", "PawPaw"));
                var orbwalking = Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
                Orbwalker = new Orbwalking.Orbwalker(orbwalking);
                Menu.Item("FarmDelay").SetValue(new Slider(0, 0, 200));
            }

			//var activator = new Activator();
			var potionManager = new PotionManager();
			var baseult = new BaseUlt();
			var bushRevealer = new AutoBushRevealer();
            
		//var overlay = new Overlay();
		
			
					
			Menu.AddToMainMenu();
			Chat.Print("AIO loaded!");
		}
	}
}
