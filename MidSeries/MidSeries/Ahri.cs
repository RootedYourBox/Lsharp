
using System;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;

namespace MidSeries
        {
    internal class Ahri
    
        {
                

          
                public static string Name = "Ahri";
                public static Orbwalking.Orbwalker Orbwalker ;
                public static Obj_AI_Hero Player = ObjectManager.Player;
                public static Spell Q, W, E;
                public static Items.Item Dfg;

                public static Menu Mid;

                    public Ahri()
                    {
                        Game_OnGameLoad();
                    }
                    


        static void Game_OnGameLoad()
        {
            if (Player.BaseSkinName != Name) return;
            //im there
            Q = new Spell(SpellSlot.Q, 880);
            W = new Spell(SpellSlot.W, 800);
            E = new Spell(SpellSlot.E, 975);
            Dfg = Utility.Map.GetMap()._MapType == Utility.Map.MapType.TwistedTreeline ? new Items.Item(3188, 750) : new Items.Item(3128, 750);
            Q.SetSkillshot(0.50f, 100f, 1100f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.50f, 60f, 1200f, true, SkillshotType.SkillshotLine);
            //Base menu
            Mid = new Menu("MidSeries - " + Name, "MidSeries", true);
            //Orbwalker and menu
            
            

            var orbwalkerMenu = new Menu("Orbwalker", "LX_Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Mid.AddSubMenu(orbwalkerMenu);
            //Target selector and menu  y thats all 
            var ts = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(ts);
            Mid.AddSubMenu(ts);
            //Combo menu
            Mid.AddSubMenu(new Menu("Combo", "Combo"));
            Mid.SubMenu("Combo").AddItem(new MenuItem("useQ", "Use Q?").SetValue(true));
            Mid.SubMenu("Combo").AddItem(new MenuItem("useW", "Use W?").SetValue(true));
            Mid.SubMenu("Combo").AddItem(new MenuItem("useE", "Use E?").SetValue(true));
            var harras = new Menu("Harras", "Harras");
            harras.AddItem(new MenuItem("useQH", "Use Q?").SetValue(true));
            Mid.AddSubMenu(harras);
            //Exploits
            Mid.AddItem(new MenuItem("NFE", "No-Face").SetValue(false));
            //Make the menu visible
            Mid.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw; // Add onDraw
            Game.OnGameUpdate += Game_OnGameUpdate; // adds OnGameUpdate (Same as onTick in bol)
            Game.PrintChat("Ahri loaded!");


        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            Utility.DrawCircle(Player.Position, Q.Range, Color.Crimson);
            Utility.DrawCircle(Player.Position,E.Range,Color.Chartreuse);
        }


        static void Game_OnGameUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed :
                    Harras();
                    break;
            }
        }
        


        public static void Combo()
        {
            // Game.PrintChat("Got to COMBO function");
            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
            if (target == null) return;


            if (target.IsValidTarget(Q.Range) && Q.IsReady())
            {
                Q.Cast(target, Mid.Item("NFE").GetValue<bool>());
            }
            if (target.IsValidTarget(W.Range) && W.IsReady())
            {
                W.Cast();
            }
            if (target.IsValidTarget(E.Range) & E.IsReady())
            {
                E.Cast(target, Mid.Item("NFE").GetValue<bool>());
            }

        }
        


        public static void Harras()
        {
            var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Magical);
            if (target == null) return;


            if (target.IsValidTarget(Q.Range) && Q.IsReady())
            {
                Q.Cast(target, Mid.Item("NFE").GetValue<bool>());
            }
        }
        

           }

      }