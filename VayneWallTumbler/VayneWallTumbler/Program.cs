using LeagueSharp.Common;
using LeagueSharp;
using SharpDX;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VayneWallTumbler
{
    class Program
    {
        public const string ChampionName = "Vayne";
        public const string Assembly = "Vayne Wall Tumbler";

        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q;

        public static Menu Config;

        private static Obj_AI_Hero Player;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;
            if (Player.BaseSkinName != ChampionName) return;
            Game.PrintChat("Vayne Wall Tumbler by Chogart loaded!");

            Q = new Spell(SpellSlot.Q, 0f);
            SpellList.Add(Q);

            Config = new Menu(Assembly, Assembly, true);
            Config.AddSubMenu(new Menu("DrakeWallTumbler", "DrakeWall"));
            Config.SubMenu("DrakeWall").AddItem(new MenuItem("DrakeWallT", "Tumble Drake Wall!").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("DrakeWall").AddItem(new MenuItem("DrawCD", "Draw Drake Wall Circle").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));

            Config.AddSubMenu(new Menu("MidWallTumbler", "MidWall"));
            Config.SubMenu("MidWall").AddItem(new MenuItem("MidWallT", "Tumble Mid Wall").SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("MidWall").AddItem(new MenuItem("DrawCM", "Draw Mid Wall Circle").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));


            Config.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Item("DrawCD").GetValue<Circle>().Active)
            {
                Drawing.DrawCircle(new Vector3(11590.95f, 4656.26f, 0f), 80f, System.Drawing.Color.FromArgb(255, 255, 255, 255));
            }
            if (Config.Item("DrawCM").GetValue<Circle>().Active)
            {
                Drawing.DrawCircle(new Vector3(6623, 8649, 0f), 80f, System.Drawing.Color.FromArgb(255, 255, 255, 255));
            }
            
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Config.Item("DrakeWallT").GetValue<KeyBind>().Active)
            {
                DrakeWall();
            }
            if (Config.Item("MidWallT").GetValue<KeyBind>().Active)
            {
                MidWall();
            }
        }

        private static void DrakeWall()
        {
            Vector2 DrakeWallQPos = new Vector2(11334.74f, 4517.47f);
            if (Player.Position.X < 11540 || Player.Position.X > 11600 || Player.Position.Y < 4638 || Player.Position.Y > 4712)
            {
                Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(11590.95f, 4656.26f)).Send();
            }
            else
            {
                Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(11590.95f, 4656.26f)).Send();
                Q.Cast(DrakeWallQPos, true);
            }
        }

        private static void MidWall()
        {
            Vector2 MidWallQPos = new Vector2(6010.5869140625f, 8508.8740234375f);
            if (Player.Position.X < 6600 || Player.Position.X > 6660 || Player.Position.Y < 8630 || Player.Position.Y > 8680)
            {
                Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(6623, 8649)).Send();
            }
            else
            {
                Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(6623, 8649)).Send();
                Q.Cast(MidWallQPos, true);
            }
        }

        
    }
}
