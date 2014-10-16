using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using Color = System.Drawing.Color;

namespace MidSeries
{
    internal class Katarina
    {




        //Orbwalker instance
        public static Orbwalking.Orbwalker Orbwalker;

        //Spells
        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Items.Item Dfg;
        public static bool IsChanneling;
        public static int Count;
        //Menu
        public static Menu Config;
        private static Obj_AI_Hero _player;


        // ReSharper disable once UnusedParameter.Local
        public Katarina()
        {
            Game_OnGameLoad();
        }
      

        static void Game_OnGameLoad()
        {
            _player = ObjectManager.Player;
            Q = new Spell(SpellSlot.Q, 675);
            W = new Spell(SpellSlot.W, 375);
            E = new Spell(SpellSlot.E, 700);
            R = new Spell(SpellSlot.R, 550);

            Dfg = new Items.Item(3128, 750f);
            PrintFancy("Loaded!");
            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);
            //Create the menu
            Config = new Menu("MidSeries - " + ObjectManager.Player.SkinName, "MidSeries", true);

            //Orbwalker submenu            
            var orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Config.AddSubMenu(orbwalkerMenu);
            //Add the targer selector to the menu.
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);



            //Combo menu
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseRCombo", "Use R").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("ProcQ", "Proc Q").SetValue(false));

            Config.AddSubMenu(new Menu("Farm", "Farm")); // creds tc-crew
            Config.SubMenu("Farm")
                .AddItem(
                    new MenuItem("UseQFarm", "Use Q").SetValue(
                        true));
            Config.SubMenu("Farm")
                .AddItem(
                    new MenuItem("UseWFarm", "Use W").SetValue(
                        true));
            var waveclear = new Menu("Waveclear", "WaveclearMenu");
            waveclear.AddItem(new MenuItem("useQW", "Use Q?").SetValue(true));
            waveclear.AddItem(new MenuItem("useWW", "Use W?").SetValue(true));
            Config.AddSubMenu(waveclear); // Thanks to ChewyMoon for the idea of doing the menu this way

            // Misc
            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("KillstealQ", "Killsteal with Q").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("KillstealE", "Killsteal with E").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("Escape", "Escape").SetValue(new KeyBind("G".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("Misc").AddItem(new MenuItem("UltCancel", "Ult Cancel(EXPERIMENTAL)").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("AlwaysW", "Auto W").SetValue(true));

            // Drawings
            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("QRange", "Q Range").SetValue(new Circle(true, Color.FromArgb(150, Color.DodgerBlue))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("ERange", "E Range").SetValue(new Circle(true, Color.FromArgb(150, Color.OrangeRed))));
            var dmgAfterComboItem = new MenuItem("DamageAfterCombo", "Draw damage after Combo").SetValue(true);
            Utility.HpBarDamageIndicator.DamageToUnit += hero => (float)CalculateDamageDrawing(hero);

            Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
            dmgAfterComboItem.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
            };
            Config.SubMenu("Drawings").AddItem(dmgAfterComboItem);
            Config.AddSubMenu(new Menu("Exploits", "Exploits"));
            Config.SubMenu("Exploits").AddItem(new MenuItem("QNFE", "Q No-Face").SetValue(true));
            // Config.SubMenu("Drawings").AddItem(new MenuItem("ERange", "E Range").SetValue(new Circle(true, Color.FromArgb(150, Color.DodgerBlue))));
            
            //SkinManager.AddToMenu(ref Config);
           
            Config.AddToMainMenu();
            //Add the events we are going to use
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;

            Orbwalking.BeforeAttack += LXOrbwalker_BeforeAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Game.OnGameSendPacket += Game_OnGameSendPacket;
            Game.PrintChat("Config Katarina loaded!");

        }

        static void Game_OnGameSendPacket(GamePacketEventArgs args)
        {
            if (args.PacketData[0] != 0x72 || !_player.HasBuff("katarinarsound",true) || Count >= 2) return;
            Count += 1;
            args.Process = false;
        }

        static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || args.SData.Name != "KatarinaR") return;
            IsChanneling = true;
            Orbwalker.SetMovement(false);
            Orbwalker.SetAttack(false);
            Utility.DelayAction.Add(1, () => IsChanneling = false);
        }

        static void LXOrbwalker_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Magical);
            if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo) return;
            if (!Config.Item("ProcQ").GetValue<bool>()) return;
            Q.CastOnUnit(target, Config.Item("QNFE").GetValue<bool>());
        }



        private static void Game_OnGameUpdate(EventArgs args)
        {
            //SkinManager.Update();
            
            if (_player.IsDead) return;
            if (Config.Item("UltCancel").GetValue<bool>())
            {
                if (_player.HasBuff("katarinarsound", true) && Utility.CountEnemysInRange((int) R.Range) == 0)
                    IssueMoveComand();
            }
            if (_player.HasBuff("katarinarsound",true))
            {
                Orbwalker.SetMovement(false);
                Orbwalker.SetAttack(false);
            }
            if (!_player.HasBuff("katarinarsound",true) && !IsChanneling)
            {
                Count = 0;
                Orbwalker.SetMovement(true);
                Orbwalker.SetAttack(true);
            }
            if(IsChanneling) return;
            var useQks = Config.Item("KillstealQ").GetValue<bool>() && Q.IsReady();
            var useEks = Config.Item("KillstealE").GetValue<bool>() && E.IsReady();
            var useWa = Config.Item("AlwaysW").GetValue<bool>() && W.IsReady();
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    Farm();
                    break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                    Harras();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    WaveClear();
                    break;
            }
            Escape();
            if(useWa)
            AlwaysW();
            if(useQks)
                Killsteal();
            if (useEks)
               KillstealE();

        }

        private static void IssueMoveComand()
        {
            _player.IssueOrder(GameObjectOrder.MoveTo, _player.Position);
        }

        private static void KillstealE()
        {
            foreach (var player in (from player in ObjectManager.Get<Obj_AI_Hero>() where !player.IsMe where player.IsValidTarget(E.Range) where _player.GetSpellDamage(player, SpellSlot.E) > player.Health select player).Where(player => E.IsReady()))
            {
                E.CastOnUnit(player, Config.Item("QNFE").GetValue<bool>());
            }
        }

        private static void AlwaysW()
        {
// ReSharper disable once UnusedVariable
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(W.Range)).Where(hero => W.IsReady()))
            {
                W.Cast();
            }
        }


        private static void Farm() // Credits TC-CREW
        {
            if (!Orbwalking.CanMove(40)) return;
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);
            var useQ = Config.Item("UseQFarm").GetValue<bool>();
            var useW = Config.Item("UseWFarm").GetValue<bool>();
            if (useQ && Q.IsReady())
            {
                foreach (var minion in from minion in allMinions where minion.Distance(_player) < Q.Range let predictedHealth = HealthPrediction.GetHealthPrediction(minion, (int)(Q.Delay + (minion.Distance(_player) / Q.Speed)) * 1000) where predictedHealth > 0f && predictedHealth < 0.75 * Q.GetDamage(minion) select minion)
                {

                    Q.CastOnUnit(minion);
                }
            }
            else if (useW && W.IsReady())
            {
                if (!allMinions.Any(minion => minion.IsValidTarget(W.Range) && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.W))) return;

                W.Cast();
            }
        }

        public static void WaveClear()
        {
            if (!Orbwalking.CanMove(40)) return;

            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);
            var useQ = Config.Item("useQW").GetValue<bool>();
            var useW = Config.Item("useWW").GetValue<bool>();
            if (useQ && Q.IsReady())
            {
                foreach (var minion in allMinions.Where(minion => minion.IsValidTarget(Q.Range)))
                {
                    Q.CastOnUnit(minion, Config.Item("QNFE").GetValue<bool>());
                    return;
                }
            }
            else if (useW && W.IsReady())
            {
                if (!allMinions.Any(minion => minion.IsValidTarget(W.Range))) return;
                W.Cast();
            }
        }

        private static void Combo()
        {
            var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Magical);
            if (target == null) return;


            if (!_player.IsChannelingImportantSpell())
            {
                if (Dfg != null)
                {
                
                if (Dfg.IsReady())
                    Dfg.Cast(target);
                }
            if (Q.IsReady() && _player.Distance(target) < Q.Range + target.BoundingRadius && !Config.Item("ProcQ").GetValue<bool>())
                        Q.CastOnUnit(target, Config.Item("QNFE").GetValue<bool>());
                    if (E.IsReady() && _player.Distance(target) < E.Range + target.BoundingRadius)
                        E.CastOnUnit(target, Config.Item("QNFE").GetValue<bool>());
                    if (W.IsReady() && _player.Distance(target) < W.Range)
                        W.Cast();
                }
                if (!Q.IsReady() && R.IsReady() && _player.Distance(target) < (R.Range - 200))
                R.Cast();
                
            

        }


        private static void Harras()
        {
            var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Magical);
            if (target == null) return;
            if (ObjectManager.Player.Distance(target) < Q.Range && Q.IsReady())
                Q.CastOnUnit(target, true);

            /*if (ObjectManager.Player.Distance(target) < E.Range && E.IsReady())
                E.CastOnUnit(target);*/

            if (ObjectManager.Player.Distance(target) < W.Range && W.IsReady())
                W.Cast();
            
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in SpellList)
            {
                var menuItem = Config.Item(spell.Slot + "Range").GetValue<Circle>();
                if (menuItem.Active)
                    Utility.DrawCircle(_player.Position, spell.Range, menuItem.Color);
                // Drawing.DrawText(playerPos[0] - 65, playerPos[1] + 20, drawUlt.Color, "Hit R To kill " + UltTarget + "!");

            }
            //Drawing tempoarily disabled
        }

        private static void Killsteal() // Creds to TC-Crew
        {
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(Q.Range)).Where(hero => Q.IsReady() && hero.Distance(ObjectManager.Player) <= Q.Range && _player.GetSpellDamage(hero, SpellSlot.Q) >= hero.Health))
            {
                Q.CastOnUnit(hero, Config.Item("QNFE").GetValue<bool>());
            }
        }


        private static void Escape() // HUGE CREDITS TO FLUXY FOR FIXING THIS
        {
            var basePos = _player.Position.To2D();
            var newPos = (Game.CursorPos.To2D() - _player.Position.To2D());
            var finalVector = basePos + (newPos.Normalized() * (560));
            var movePos = basePos + (newPos.Normalized() * (100));
            if (!Config.Item("Escape").GetValue<KeyBind>().Active) return;
            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, movePos.To3D());
            if (!E.IsReady()) return;
            var castWard = true;
            foreach (var esc in ObjectManager.Get<Obj_AI_Base>().Where(esc => esc.Distance(ObjectManager.Player) <= E.Range))
            {
                if (Vector2.Distance(Game.CursorPos.To2D(), esc.ServerPosition.To2D()) <= 175)
                {
                    E.Cast(esc, true);
                    castWard = false;
                }
                if (!esc.Name.Contains("Ward") || !(Vector2.Distance(finalVector, esc.ServerPosition.To2D()) <= 175))
                    continue;
                E.Cast(esc, true);
                castWard = false;
            }
            var ward = FindBestWardItem();
            if (ward != null && castWard)
            {
                ward.UseItem(finalVector.To3D());
            }
        }


        private static SpellDataInst GetItemSpell(InventorySlot invSlot)
        {
            return ObjectManager.Player.Spellbook.Spells.FirstOrDefault(spell => (int)spell.Slot == invSlot.Slot + 4);
        }
        private static InventorySlot FindBestWardItem()
        {
            var slot = Items.GetWardSlot();
            if (slot == default(InventorySlot)) return null;

            var sdi = GetItemSpell(slot);

            if (sdi != default(SpellDataInst) && sdi.State == SpellState.Ready)
            {
                return slot;
            }
            return null;
        }
  

        public static double CalculateDamageDrawing(Obj_AI_Base target)
        {
            double totaldamage = 0;
            if (Q.IsReady())
            {
                totaldamage += _player.GetSpellDamage(target, SpellSlot.Q, 2);
            }
            if (E.IsReady() && W.IsReady())
            {
                totaldamage += _player.GetSpellDamage(target, SpellSlot.W);
            }
            if (E.IsReady())
            {
                totaldamage += _player.GetSpellDamage(target, SpellSlot.E);
            }
            if (R.IsReady())
            {
                totaldamage += _player.GetSpellDamage(target, SpellSlot.R) * 8;
            }
            if (!Q.IsReady())
            {
                totaldamage += _player.CalcDamage(target, Damage.DamageType.Magical, (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level * 15) + (0.15 * ObjectManager.Player.FlatMagicDamageMod));
            }
            if (Dfg != null)
            return (Dfg.IsReady() ? totaldamage * 1.2f : totaldamage * 1f);
            return totaldamage;
        }
        protected static int CountEnemiesNearPosition(Vector3 pos, float range)
        {
            return
                ObjectManager.Get<Obj_AI_Hero>().Count(
                    hero => hero.IsEnemy && !hero.IsDead && hero.IsValid && hero.Distance(pos) <= range);
        }
        public static void PrintFancy(string msg) // Credits to ChewyMoon, and his Brain.exe
        {
            Game.PrintChat("<font color=\"#6699ff\"><b>DProject: </b></font> <font color=\"#FFFFFF\">" + msg + "</font>");
        }
    }
}