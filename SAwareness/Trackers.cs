﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using LeagueSharp;
using LeagueSharp.Common;
using SAwareness.Properties;
using SharpDX;
using SharpDX.Direct3D9;
using Color = SharpDX.Color;
using Font = SharpDX.Direct3D9.Font;

namespace SAwareness
{
    class CloneTracker
    {
        public CloneTracker()
        {
            Drawing.OnDraw += Drawing_OnDraw;
        }

        ~CloneTracker()
        {
            Drawing.OnDraw -= Drawing_OnDraw;
        }

        public bool IsActive()
        {
            return Menu.Tracker.GetActive() && Menu.CloneTracker.GetActive();
        }

        void Drawing_OnDraw(EventArgs args)
        {
            if (!IsActive())
                return;
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero.IsEnemy)
                {
                    if (hero.ChampionName.Contains("Shaco") ||
                        hero.ChampionName.Contains("LeBlanc") ||
                        hero.ChampionName.Contains("MonkeyKing") ||
                        hero.ChampionName.Contains("Yorick"))
                    {
                        Utility.DrawCircle(hero.ServerPosition, 100, System.Drawing.Color.Red);
                        Utility.DrawCircle(hero.ServerPosition, 110, System.Drawing.Color.Red);
                    }
                }
            }
        }
    }

    class HiddenObject
    {
        public class Object
        {
            public ObjectType Type;
            public String Name;
            public String ObjectName;
            public String SpellName;
            public float Duration;
            public int Id;
            public int Id2;
            public System.Drawing.Color Color;

            public Object(ObjectType type, String name, String objectName, String spellName, float duration, int id, int id2, System.Drawing.Color color)
            {
                Type = type;
                Name = name;
                ObjectName = objectName;
                SpellName = spellName;
                Duration = duration;
                Id = id;
                Id2 = id2;
                Color = color;
            }
        }

        public class ObjectData
        {
            public Object ObjectBase;
            public Vector3 Position;
            public float EndTime;
            public String Creator;
            public List<Vector2> Points;
            public int NetworkId;

            public ObjectData(Object objectBase, Vector3 position, float endTime, String creator, List<Vector2> points, int networkId)
            {
                ObjectBase = objectBase;
                Position = position;
                EndTime = endTime;
                Creator = creator;
                Points = points;
                NetworkId = networkId;
            }
        }

        public enum ObjectType
        {
            Vision,
            Sight,
            Trap
        }

        const int WardRange = 1200;
        const int TrapRange = 300;
        public List<Object> Objects = new List<Object>();
        public List<ObjectData> HidObjects = new List<ObjectData>();

        public HiddenObject()
        {
            Objects.Add(new Object(ObjectType.Vision, "Vision Ward", "VisionWard", "VisionWard", float.MaxValue, 8, 6424612, System.Drawing.Color.BlueViolet));
            Objects.Add(new Object(ObjectType.Sight, "Stealth Ward", "SightWard", "SightWard", 180.0f, 161, 234594676, System.Drawing.Color.Green));
            Objects.Add(new Object(ObjectType.Sight, "Warding Totem (Trinket)", "SightWard", "TrinketTotemLvl1", 60.0f, 56, 263796881, System.Drawing.Color.Green));
            Objects.Add(new Object(ObjectType.Sight, "Warding Totem (Trinket)", "SightWard", "trinkettotemlvl2", 120.0f, 56, 263796882, System.Drawing.Color.Green));
            Objects.Add(new Object(ObjectType.Sight, "Greater Stealth Totem (Trinket)", "SightWard", "TrinketTotemLvl3", 180.0f, 56, 263796882, System.Drawing.Color.Green));
            Objects.Add(new Object(ObjectType.Sight, "Greater Vision Totem (Trinket)", "SightWard", "TrinketTotemLvl3B", 9999.9f, 137, 194218338, System.Drawing.Color.BlueViolet));
            Objects.Add(new Object(ObjectType.Sight, "Wriggle's Lantern", "SightWard", "wrigglelantern", 180.0f, 73, 177752558, System.Drawing.Color.Green));
            Objects.Add(new Object(ObjectType.Sight, "Quill Coat", "SightWard", "", 180.0f, 73, 135609454, System.Drawing.Color.Green));
            Objects.Add(new Object(ObjectType.Sight, "Ghost Ward", "SightWard", "ItemGhostWard", 180.0f, 229, 101180708, System.Drawing.Color.Green));

            Objects.Add(new Object(ObjectType.Trap, "Yordle Snap Trap", "Cupcake Trap", "CaitlynYordleTrap", 240.0f, 62, 176176816, System.Drawing.Color.Red));
            Objects.Add(new Object(ObjectType.Trap, "Jack In The Box", "Jack In The Box", "JackInTheBox", 60.0f, 2, 44637032, System.Drawing.Color.Red));
            Objects.Add(new Object(ObjectType.Trap, "Bushwhack", "Noxious Trap", "Bushwhack", 240.0f, 9, 167611995, System.Drawing.Color.Red));
            Objects.Add(new Object(ObjectType.Trap, "Noxious Trap", "Noxious Trap", "BantamTrap", 600.0f, 48, 176304336, System.Drawing.Color.Red));

            Game.OnGameProcessPacket += Game_OnGameProcessPacket;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            GameObject.OnDelete += Obj_AI_Base_OnDelete;
            Drawing.OnDraw += Drawing_OnDraw;
            GameObject.OnCreate += GameObject_OnCreate;
        }

        ~HiddenObject()
        {
            Game.OnGameProcessPacket -= Game_OnGameProcessPacket;
            Obj_AI_Base.OnProcessSpellCast -= Obj_AI_Base_OnProcessSpellCast;
            GameObject.OnCreate -= GameObject_OnCreate;
            GameObject.OnDelete -= Obj_AI_Base_OnDelete;
            Drawing.OnDraw -= Drawing_OnDraw;
        }

        public bool IsActive()
        {
            return Menu.Tracker.GetActive() && Menu.VisionDetector.GetActive();
        }

        void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (!IsActive())
                return;
            try
            {
                if (sender.Type == GameObjectType.obj_AI_Marker && ObjectManager.Player.Team != sender.Team)
                {
                    foreach (Object obj in Objects)
                    {
                        if (sender.Name == obj.ObjectName && !ObjectExist(sender.Position))
                        {
                            HidObjects.Add(new ObjectData(obj, sender.Position, Game.Time + obj.Duration, sender.Name, null, sender.NetworkId));
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HiddenObjectCreate: " + ex.ToString());
                return;
            }
        }

        void Drawing_OnDraw(EventArgs args)
        {
            if (!IsActive())
                return;
            try
            {
                for (int i = 0; i < HidObjects.Count; i++)
                {
                    ObjectData obj = HidObjects[i];
                    if (Game.Time > obj.EndTime)
                    {
                        HidObjects.RemoveAt(i);
                        break;
                    }
                    Vector2 objMPos = Drawing.WorldToMinimap(obj.Position);
                    Vector2 objPos = Drawing.WorldToScreen(obj.Position);
                    List<Vector3> posList = new List<Vector3>();
                    switch (obj.ObjectBase.Type)
                    {
                        case ObjectType.Sight:
                            Utility.DrawCircle(obj.Position, WardRange, obj.ObjectBase.Color);
                            posList = GetVision(obj.Position, WardRange);
                            for (int j = 0; j < posList.Count; j++)
                            {
                                Vector2 visionPos1 = Drawing.WorldToScreen(posList[i]);
                                Vector2 visionPos2 = Drawing.WorldToScreen(posList[i]);
                                Drawing.DrawLine(visionPos1[0], visionPos1[1], visionPos2[0], visionPos2[1], 1.0f, obj.ObjectBase.Color);
                            }
                            Drawing.DrawText(objMPos[0], objMPos[1], obj.ObjectBase.Color, "S");
                            break;

                        case ObjectType.Trap:
                            Utility.DrawCircle(obj.Position, TrapRange, obj.ObjectBase.Color);
                            posList = GetVision(obj.Position, TrapRange);
                            for (int j = 0; j < posList.Count; j++)
                            {
                                Vector2 visionPos1 = Drawing.WorldToScreen(posList[i]);
                                Vector2 visionPos2 = Drawing.WorldToScreen(posList[i]);
                                Drawing.DrawLine(visionPos1[0], visionPos1[1], visionPos2[0], visionPos2[1], 1.0f, obj.ObjectBase.Color);
                            }
                            Drawing.DrawText(objMPos[0], objMPos[1], obj.ObjectBase.Color, "T");
                            break;

                        case ObjectType.Vision:
                            Utility.DrawCircle(obj.Position, WardRange, obj.ObjectBase.Color);
                            posList = GetVision(obj.Position, WardRange);
                            for (int j = 0; j < posList.Count; j++)
                            {
                                Vector2 visionPos1 = Drawing.WorldToScreen(posList[i]);
                                Vector2 visionPos2 = Drawing.WorldToScreen(posList[i]);
                                Drawing.DrawLine(visionPos1[0], visionPos1[1], visionPos2[0], visionPos2[1], 1.0f, obj.ObjectBase.Color);
                            }
                            Drawing.DrawText(objMPos[0], objMPos[1], obj.ObjectBase.Color, "V");
                            break;
                    }
                    Utility.DrawCircle(obj.Position, 50, obj.ObjectBase.Color);
                    float endTime = obj.EndTime - Game.Time;
                    if (!float.IsInfinity(endTime) && !float.IsNaN(endTime) && endTime.CompareTo(float.MaxValue) != 0)
                    {
                        var m = (float)Math.Floor(endTime / 60);
                        var s = (float)Math.Ceiling(endTime % 60);
                        String ms = (s < 10 ? m + ":0" + s : m + ":" + s);
                        Drawing.DrawText(objPos[0], objPos[1], obj.ObjectBase.Color, ms);
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HiddenObjectDraw: " + ex.ToString());
                return;
            }

        }

        List<Vector3> GetVision(Vector3 viewPos, float range) //TODO: ADD IT
        {
            List<Vector3> list = new List<Vector3>();
            //double qual = 2*Math.PI/25;
            //for (double i = 0; i < 2*Math.PI + qual;)
            //{
            //    Vector3 pos = new Vector3(viewPos.X + range * (float)Math.Cos(i), viewPos.Y - range * (float)Math.Sin(i), viewPos.Z);
            //    for (int j = 1; j < range; j = j + 25)
            //    {
            //        Vector3 nPos = new Vector3(viewPos.X + j * (float)Math.Cos(i), viewPos.Y - j * (float)Math.Sin(i), viewPos.Z);
            //        if (NavMesh.GetCollisionFlags(nPos).HasFlag(CollisionFlags.Wall))
            //        {
            //            pos = nPos;
            //            break;
            //        }
            //    }
            //    list.Add(pos);
            //    i = i + 0.1;
            //}
            return list;
        }

        Object HiddenObjectById(int id)
        {
            return Objects.FirstOrDefault(vision => id == vision.Id2);
        }

        bool ObjectExist(Vector3 pos)
        {
            return HidObjects.Any(obj => pos.Distance(obj.Position) < 30);
        }

        void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            if (!IsActive())
                return;
            try
            {
                var reader = new BinaryReader(new MemoryStream(args.PacketData));
                byte PacketId = reader.ReadByte(); //PacketId
                if (PacketId == 181) //OLD 180
                {
                    int networkId = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                    var creator = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>(networkId);
                    if (creator != null && creator.Team != ObjectManager.Player.Team)
                    {
                        reader.ReadBytes(7);
                        var id = reader.ReadInt32();
                        reader.ReadBytes(21);
                        networkId = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                        reader.ReadBytes(12);
                        float x = BitConverter.ToSingle(reader.ReadBytes(4), 0);
                        float y = BitConverter.ToSingle(reader.ReadBytes(4), 0);
                        float z = BitConverter.ToSingle(reader.ReadBytes(4), 0);
                        var pos = new Vector3(x, y, z);
                        Object obj = HiddenObjectById(id);
                        if (obj != null && !ObjectExist(pos))
                        {
                            if (obj.Type == ObjectType.Trap)
                                pos = new Vector3(x, z, y);
                            networkId = networkId + 2;
                            Utility.DelayAction.Add(1, () =>
                            {
                                for (int i = 0; i < HidObjects.Count; i++)
                                {
                                    var objectData = HidObjects[i];
                                    if (objectData != null && objectData.NetworkId == networkId)
                                    {
                                        var objNew = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>(networkId);
                                        if (objNew != null && objNew.IsValid)
                                            objectData.Position = objNew.Position;
                                    }
                                }
                            });
                            HidObjects.Add(new ObjectData(obj, pos, Game.Time + obj.Duration, creator.Name, null, networkId));
                        }
                    }
                }
                else if (PacketId == 178)
                {
                    int networkId = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                    var gObject = ObjectManager.GetUnitByNetworkId<GameObject>(networkId);
                    if (gObject != null)
                    {
                        for (int i = 0; i < HidObjects.Count; i++)
                        {
                            var objectData = HidObjects[i];
                            if (objectData != null && objectData.NetworkId == networkId)
                            {
                                objectData.Position = gObject.Position;
                            }
                        }
                    }
                }
                else if (PacketId == 50) //OLD 49
                {
                    int networkId = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                    for (int i = 0; i < HidObjects.Count; i++)
                    {
                        var objectData = HidObjects[i];
                        var gObject = ObjectManager.GetUnitByNetworkId<GameObject>(networkId);
                        if (objectData != null && objectData.NetworkId == networkId)
                        {
                            HidObjects.RemoveAt(i);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HiddenObjectProcess: " + ex.ToString());
                return;
            }

        }

        void Obj_AI_Base_OnDelete(GameObject sender, EventArgs args)
        {
            if (!IsActive())
                return;
            try
            {
                for (int i = 0; i < HidObjects.Count; i++)
                {
                    ObjectData obj = HidObjects[i];
                    if (sender.Name == obj.ObjectBase.ObjectName || sender.Name.Contains("Ward") && sender.Name.Contains("Death"))
                        if (sender.Position.Distance(obj.Position) < 30)
                        {
                            HidObjects.RemoveAt(i);
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HiddenObjectDelete: " + ex.ToString());
                return;
            }

        }

        void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!IsActive())
                return;
            try
            {
                if (sender.Type == GameObjectType.obj_AI_Minion && ObjectManager.Player.Team != sender.Team)
                {
                    foreach (Object obj in Objects)
                    {
                        if (args.SData.Name == obj.SpellName && !ObjectExist(args.End))
                        {
                            HidObjects.Add(new ObjectData(obj, args.End, Game.Time + obj.Duration, sender.Name, null, sender.NetworkId));
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HiddenObjectSpell: " + ex.ToString());
                return;
            }

        }
    }

    class DestinationTracker
    {
        static readonly Dictionary<Obj_AI_Hero, List<Ability>> Enemies = new Dictionary<Obj_AI_Hero, List<Ability>>();

        public class Ability
        {
            public String SpellName;
            public int Range;
            public float Delay;
            public bool Casted;
            public int TimeCasted;
            public Vector3 StartPos;
            public Vector3 EndPos;
            public bool OutOfBush;
            public Obj_AI_Hero Owner;
            public Obj_AI_Hero Target;
            public bool TargetDead;
            public int ExtraTicks;

            public Ability(string spellName, int range, float delay, Obj_AI_Hero owner)
            {
                SpellName = spellName;
                Range = range;
                Delay = delay;
                Owner = owner;
            }
        }

        private bool AddObject(Obj_AI_Hero hero, List<Ability> abilities)
        {
            if (Enemies.ContainsKey(hero))
                return false;
            else
            {
                Enemies.Add(hero, abilities);
            }
            return true;
            //TODO:Add
        }

        public DestinationTracker()
        {
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero.IsEnemy)
                {
                    var abilities = new List<Ability>();
                    foreach (var spell in hero.SummonerSpellbook.Spells)
                    {
                        if (spell.Name.Contains("Flash"))
                        {
                            abilities.Add(new Ability("SummonerFlash", 400, 0, hero));
                            //AddObject(hero, abilities);
                        }
                    }

                    //abilities.Clear(); //TODO: Check if it delets the flash abilities

                    switch (hero.ChampionName)
                    {
                        case "Ezreal":
                            abilities.Add(new Ability("EzrealArcaneShift", 475, 0, hero));
                            //AddObject(hero, abilities);
                            break;

                        case "Fiora":
                            abilities.Add(new Ability("FioraDance", 700, 1, hero));
                            //AddObject(hero, abilities);
                            break;

                        case "Kassadin":
                            abilities.Add(new Ability("RiftWalk", 700, 0, hero));
                            //AddObject(hero, abilities);
                            break;

                        case "Katarina":
                            abilities.Add(new Ability("KatarinaE", 700, 0, hero));
                            //AddObject(hero, abilities);
                            break;

                        case "Leblanc":
                            abilities.Add(new Ability("LeblancSlide", 600, 0.5f, hero));
                            abilities.Add(new Ability("leblancslidereturn", 0, 0, hero));
                            abilities.Add(new Ability("LeblancSlideM", 600, 0.5f, hero));
                            abilities.Add(new Ability("leblancslidereturnm", 0, 0, hero));
                            //AddObject(hero, abilities);
                            break;

                        case "Lissandra":
                            abilities.Add(new Ability("LissandraE", 700, 0, hero));
                            //AddObject(hero, abilities);
                            break;

                        case "MasterYi":
                            abilities.Add(new Ability("AlphaStrike", 600, 0.9f, hero));
                            //AddObject(hero, abilities);
                            break;

                        case "Shaco":
                            abilities.Add(new Ability("Deceive", 400, 0, hero));
                            //AddObject(hero, abilities);
                            break;

                        case "Talon":
                            abilities.Add(new Ability("TalonCutthroat", 700, 0, hero));
                            //AddObject(hero, abilities);
                            break;

                        case "Vayne":
                            abilities.Add(new Ability("VayneTumble", 250, 0, hero));
                            //AddObject(hero, abilities);
                            break;

                        case "Zed":
                            abilities.Add(new Ability("ZedShadowDash", 999, 0, hero));
                            //AddObject(hero, abilities);
                            break;
                    }
                    if (abilities.Count > 0)
                        AddObject(hero, abilities);
                }
            }
            Game.OnGameUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        ~DestinationTracker()
        {
            Game.OnGameUpdate -= Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast -= Obj_AI_Hero_OnProcessSpellCast;
            GameObject.OnCreate -= Obj_AI_Base_OnCreate;
            Drawing.OnDraw -= Drawing_OnDraw;
        }

        public bool IsActive()
        {
            return Menu.Tracker.GetActive() && Menu.DestinationTracker.GetActive();
        }

        void Drawing_OnDraw(EventArgs args)
        {
            if (!IsActive())
                return;
            foreach (var enemy in Enemies)
            {
                foreach (var ability in enemy.Value)
                {
                    if (ability.Casted)
                    {
                        Vector2 startPos = Drawing.WorldToScreen(ability.StartPos);
                        Vector2 endPos = Drawing.WorldToScreen(ability.EndPos);

                        if (ability.OutOfBush)
                        {
                            Utility.DrawCircle(ability.EndPos, ability.Range, System.Drawing.Color.Red);
                        }
                        else
                        {
                            Utility.DrawCircle(ability.EndPos, ability.Range, System.Drawing.Color.Red);
                            Drawing.DrawLine(startPos[0], startPos[1], endPos[0], endPos[1], 1.0f, System.Drawing.Color.Red);
                        }
                        Drawing.DrawText(endPos[0], endPos[1], System.Drawing.Color.Bisque, enemy.Key.ChampionName + " " + ability.SpellName);
                    }
                }
            }
        }

        void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            if (!IsActive())
                return;
            foreach (KeyValuePair<Obj_AI_Hero, List<Ability>> enemy in Enemies)
            {
                if (enemy.Key.ChampionName == "Shaco")
                {
                    if (sender.Name == "JackintheboxPoof2.troy" && !enemy.Value[0].Casted)
                    {
                        enemy.Value[0].StartPos = sender.Position;
                        enemy.Value[0].EndPos = sender.Position;
                        enemy.Value[0].Casted = true;
                        enemy.Value[0].TimeCasted = (int)Game.Time;
                        enemy.Value[0].OutOfBush = true;
                    }
                }
            }

        }

        Vector3 CalculateEndPos(Ability ability, GameObjectProcessSpellCastEventArgs args)
        {
            float dist = Vector3.Distance(args.Start, args.End);
            if (dist <= ability.Range)
            {
                ability.EndPos = args.End;
            }
            else
            {
                Vector3 norm = args.Start - args.End;
                norm.Normalize();
                Vector3 endPos = args.Start - norm * ability.Range;

                //endPos = FindNearestNonWall(); TODO: Add FindNearestNonWall

                ability.EndPos = endPos;
            }
            return ability.EndPos;
        }

        void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!IsActive())
                return;
            if (sender.GetType() == typeof(Obj_AI_Hero))
            {
                var hero = (Obj_AI_Hero)sender;
                if (hero.IsEnemy)
                {
                    Obj_AI_Hero enemy = hero;
                    foreach (KeyValuePair<Obj_AI_Hero, List<Ability>> abilities in Enemies)
                    {
                        if (abilities.Key.NetworkId != enemy.NetworkId)
                            continue;
                        int index = 0;
                        foreach (var ability in abilities.Value)
                        {
                            if (args.SData.Name == "vayneinquisition")
                            {
                                if (ability.ExtraTicks > 0)
                                {
                                    ability.ExtraTicks = (int)Game.Time + 6 + 2 * args.Level;
                                    return;
                                }
                            }
                            if (args.SData.Name == ability.SpellName)
                            {
                                switch (ability.SpellName)
                                {
                                    case "VayneTumble":
                                        if (Game.Time >= ability.ExtraTicks)
                                            return;
                                        ability.StartPos = args.Start;
                                        ability.EndPos = CalculateEndPos(ability, args);
                                        break;

                                    case "Deceive":
                                        ability.OutOfBush = false;
                                        ability.StartPos = args.Start;
                                        ability.EndPos = CalculateEndPos(ability, args);
                                        break;

                                    case "LeblancSlideM":
                                        abilities.Value[index - 2].Casted = false;
                                        ability.StartPos = abilities.Value[index - 2].StartPos;
                                        ability.EndPos = CalculateEndPos(ability, args);
                                        break;

                                    case "leblancslidereturn":
                                    case "leblancslidereturnm":
                                        if (ability.SpellName == "leblancslidereturn")
                                        {
                                            abilities.Value[index - 1].Casted = false;
                                            abilities.Value[index + 1].Casted = false;
                                            abilities.Value[index + 2].Casted = false;
                                        }
                                        else
                                        {
                                            abilities.Value[index - 3].Casted = false;
                                            abilities.Value[index - 2].Casted = false;
                                            abilities.Value[index - 1].Casted = false;
                                        }
                                        ability.StartPos = args.Start;
                                        ability.EndPos = abilities.Value[index - 1].StartPos;
                                        break;

                                    case "FioraDance":
                                    case "AlphaStrike":
                                        //TODO: Get Target
                                        //ability.Target = args.Target;
                                        ability.TargetDead = false;
                                        ability.StartPos = args.Start;
                                        //ability.EndPos = args.Target.Position;
                                        break;

                                    default:
                                        ability.StartPos = args.Start;
                                        ability.EndPos = CalculateEndPos(ability, args);
                                        break;
                                }
                                ability.Casted = true;
                                ability.TimeCasted = (int)Game.Time;
                                return;
                            }
                            index++;
                        }
                    }
                }
            }
        }

        void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive())
                return;
            foreach (KeyValuePair<Obj_AI_Hero, List<Ability>> abilities in Enemies)
            {
                foreach (var ability in abilities.Value)
                {
                    if (ability.Casted)
                    {
                        if (ability.SpellName == "FioraDance" || ability.SpellName == "AlphaStrike" &&
                            !ability.TargetDead)
                        {
                            if (Game.Time > (ability.TimeCasted + ability.Delay + 0.2))
                                ability.Casted = false;
                            /*else if (ability.Target.IsDead()) TODO: Waiting for adding Target
                            {
                                Vector3 temp = ability.EndPos;
                                ability.EndPos = ability.StartPos;
                                ability.StartPos = temp;
                                ability.TargetDead = true;
                            }*/
                            else
                            {
                                //ability.EndPos = ability.Target.ServerPosition; TODO: Waiting for adding Target
                            }
                        }
                        else if (ability.Owner.IsDead ||
                                 (!ability.Owner.IsValid && Game.Time > (ability.TimeCasted + /*variable*/ 2)) ||
                                 (ability.Owner.IsVisible &&
                                  Game.Time > (ability.TimeCasted + /*variable*/ 5 + ability.Delay)))
                        {
                            ability.Casted = false;
                        }
                        else if (!ability.OutOfBush && ability.Owner.IsVisible &&
                                 Game.Time > (ability.TimeCasted + ability.Delay))
                        {
                            ability.EndPos = ability.Owner.ServerPosition;
                        }
                    }
                }
            }
        }
    }

    class SsCaller
    {
        static readonly Dictionary<Obj_AI_Hero, Time> Enemies = new Dictionary<Obj_AI_Hero, Time>();

        public class Time
        {
            public int VisibleTime;
            public int InvisibleTime;
            public int LastTimeCalled;
            public Vector3 LastPosition;
            public bool Called;
        }

        public SsCaller()
        {
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero.IsEnemy)
                {
                    Enemies.Add(hero, new Time());
                }
            }
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        ~SsCaller()
        {
            Game.OnGameUpdate -= Game_OnGameUpdate;
        }

        public bool IsActive()
        {
            return Menu.Tracker.GetActive() && Menu.SsCaller.GetActive() && Game.Time < (Menu.SsCaller.GetMenuItem("SAwarenessSSCallerDisableTime").GetValue<Slider>().Value * 60);
        }

        void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive())
                return;
            foreach (KeyValuePair<Obj_AI_Hero, Time> enemy in Enemies)
            {
                UpdateTime(enemy);
                HandleSs(enemy);
            }
        }

        void HandleSs(KeyValuePair<Obj_AI_Hero, Time> enemy)
        {
            Obj_AI_Hero hero = enemy.Key;
            if (enemy.Value.InvisibleTime > 5 && !enemy.Value.Called && Game.Time - enemy.Value.LastTimeCalled > 30)
            {
                Vector2 pos = new Vector2(hero.Position.X, hero.Position.Y);
                Packet.PingType pingType = Packet.PingType.Normal;
                StringList t = Menu.SsCaller.GetMenuItem("SAwarenessSSCallerPingType").GetValue<StringList>();
                pingType = (Packet.PingType)t.SelectedIndex + 1;
                GamePacket gPacketT;
                for (int i = 0; i < Menu.SsCaller.GetMenuItem("SAwarenessSSCallerPingTimes").GetValue<Slider>().Value; i++)
                {
                    if (Menu.SsCaller.GetMenuItem("SAwarenessSSCallerLocalPing").GetValue<bool>())
                    {
                        gPacketT = Packet.S2C.Ping.Encoded(new Packet.S2C.Ping.Struct(pos[0], pos[1], 0, 0, pingType + 176));
                        gPacketT.Process();
                    }
                    else
                    {
                        gPacketT = Packet.C2S.Ping.Encoded(new Packet.C2S.Ping.Struct(enemy.Value.LastPosition.X, enemy.Value.LastPosition.Y, 0, pingType));
                        //gPacketT.Send();
                    }

                }
                if (Menu.SsCaller.GetMenuItem("SAwarenessSSCallerChatChoice").GetValue<StringList>().SelectedIndex == 1)
                {
                    Game.PrintChat("ss {0}", hero.ChampionName);
                }
                else if (Menu.SsCaller.GetMenuItem("SAwarenessSSCallerChatChoice").GetValue<StringList>().SelectedIndex == 2)
                {
                    //Game.Say("ss {0}", hero.ChampionName);
                }
                enemy.Value.LastTimeCalled = (int)Game.Time;
                enemy.Value.Called = true;
            }
        }

        void UpdateTime(KeyValuePair<Obj_AI_Hero, Time> enemy)
        {
            Obj_AI_Hero hero = enemy.Key;
            if (hero.IsVisible)
            {
                Enemies[hero].InvisibleTime = 0;
                Enemies[hero].VisibleTime = (int)Game.Time;
                enemy.Value.Called = false;
                Enemies[hero].LastPosition = hero.ServerPosition;
            }
            else
            {
                if (Enemies[hero].VisibleTime != 0)
                {
                    Enemies[hero].InvisibleTime = (int)(Game.Time - Enemies[hero].VisibleTime);
                }
                else
                {
                    Enemies[hero].InvisibleTime = 0;
                }
            }
        }
    }

    public class UITracker
    {
        class ChampInfos
        {

            public class Gui
            {
                public class SpriteInfos
                {
                    public Texture Texture;
                    public int Cd;
                    public Size SizeSideBar;
                    public Size SizeHpBar;
                    public Size CoordsSideBar;
                    public Size CoordsHpBar;
                }

                public SpriteInfos Champ = new SpriteInfos();
                public SpriteInfos SpellPassive = new SpriteInfos();
                public SpriteInfos SpellQ = new SpriteInfos();
                public SpriteInfos SpellW = new SpriteInfos();
                public SpriteInfos SpellE = new SpriteInfos();
                public SpriteInfos SpellR = new SpriteInfos();
                public SpriteInfos SpellSum1 = new SpriteInfos();
                public SpriteInfos SpellSum2 = new SpriteInfos();
                public SpriteInfos BackBar = new SpriteInfos();
                public SpriteInfos HealthBar = new SpriteInfos();
                public SpriteInfos ManaBar = new SpriteInfos();
                public SpriteInfos RecallBar = new SpriteInfos();
                public SpriteInfos[] Item = new SpriteInfos[7];
                public ItemId[] ItemId = new ItemId[7];
                public int DeathTime;
                public int VisibleTime;
                public int InvisibleTime;
                public String SHealth;
                public String SMana;
                public Vector2 Pos = new Vector2();
            }

            public Gui SGui = new Gui();
        }

        public UITracker()
        {
            if (!IsActive())
                return;
            var loaded = false;
            var tries = 0;
            while (!loaded)
            {
                loaded = Init(tries >= 5);

                tries++;
                if (tries > 9)
                {
                    Console.WriteLine("Couldn't load Interface. It got disabled.");
                    Menu.UiTracker.ForceDisable = true;
                    Menu.UiTracker.Item = null;
                    return;
                }
                Thread.Sleep(10);
            }
            Game.OnGameUpdate += Game_OnGameUpdate;

            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Game.OnWndProc += Game_OnWndProc;
            AppDomain.CurrentDomain.DomainUnload += delegate { Drawing_OnPreReset(new EventArgs()); };
            AppDomain.CurrentDomain.ProcessExit += delegate { Drawing_OnPreReset(new EventArgs()); };
        }

        void Game_OnWndProc(WndEventArgs args)
        {
            if (!IsActive())
                return;
            HandleInput((WindowsMessages)args.Msg, Utils.GetCursorPos(), args.WParam);
        }

        void HandleInput(WindowsMessages message, Vector2 cursorPos, uint key)
        {
            HandleUIMove(message, cursorPos, key);
            HandleChampClick(message, cursorPos, key);
        }

        void HandleUIMove(WindowsMessages message, Vector2 cursorPos, uint key)
        {
            if (message != WindowsMessages.WM_LBUTTONDOWN && message != WindowsMessages.WM_MOUSEMOVE && message != WindowsMessages.WM_LBUTTONUP || (!moveActive && message == WindowsMessages.WM_MOUSEMOVE)
                )
            {
                return;
            }
            if (message == WindowsMessages.WM_LBUTTONDOWN)
            {
                lastCursorPos = cursorPos;
            }
            if (message == WindowsMessages.WM_LBUTTONUP)
            {
                lastCursorPos = new Vector2();
                moveActive = false;
                return;
            }
            KeyValuePair<Obj_AI_Hero, ChampInfos> firstEnemyHero = new KeyValuePair<Obj_AI_Hero, ChampInfos>();
            foreach (KeyValuePair<Obj_AI_Hero, ChampInfos> enemy in Enemies.Reverse())
            {
                firstEnemyHero = enemy;
                break;
            }
            if (firstEnemyHero.Key != null && Common.IsInside(cursorPos, firstEnemyHero.Value.SGui.SpellPassive.SizeSideBar, 
                hudSize.Width, hudSize.Height))
            {
                moveActive = true;
                if (message == WindowsMessages.WM_MOUSEMOVE)
                {
                    Slider curSliderX = Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker").GetMenuItem("SAwarenessUITrackerEnemyTrackerXPos").GetValue<Slider>();
                    Slider curSliderY = Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker").GetMenuItem("SAwarenessUITrackerEnemyTrackerYPos").GetValue<Slider>();
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker").GetMenuItem("SAwarenessUITrackerEnemyTrackerXPos")
                        .SetValue(new Slider((int)(curSliderX.Value + cursorPos.X - lastCursorPos.X), curSliderX.MinValue, curSliderX.MaxValue));
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker").GetMenuItem("SAwarenessUITrackerEnemyTrackerYPos")
                        .SetValue(new Slider((int)(curSliderY.Value + cursorPos.Y - lastCursorPos.Y), curSliderY.MinValue, curSliderY.MaxValue));
                    lastCursorPos = cursorPos;
                }
            }
            KeyValuePair<Obj_AI_Hero, ChampInfos> firstAllyHero = new KeyValuePair<Obj_AI_Hero, ChampInfos>();
            foreach (KeyValuePair<Obj_AI_Hero, ChampInfos> ally in Allies.Reverse())
            {
                firstAllyHero = ally;
                break;
            }
            if (firstAllyHero.Key != null && Common.IsInside(cursorPos, firstAllyHero.Value.SGui.SpellPassive.SizeSideBar,
                hudSize.Width, hudSize.Height))
            {
                moveActive = true;
                if (message == WindowsMessages.WM_MOUSEMOVE)
                {
                    Slider curSliderX = Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker").GetMenuItem("SAwarenessUITrackerAllyTrackerXPos").GetValue<Slider>();
                    Slider curSliderY = Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker").GetMenuItem("SAwarenessUITrackerAllyTrackerYPos").GetValue<Slider>();
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker").GetMenuItem("SAwarenessUITrackerAllyTrackerXPos")
                        .SetValue(new Slider((int)(curSliderX.Value + cursorPos.X - lastCursorPos.X), curSliderX.MinValue, curSliderX.MaxValue));
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker").GetMenuItem("SAwarenessUITrackerAllyTrackerYPos")
                        .SetValue(new Slider((int)(curSliderY.Value + cursorPos.Y - lastCursorPos.Y), curSliderY.MinValue, curSliderY.MaxValue));
                    lastCursorPos = cursorPos;
                }
            }
            
        }

        void HandleChampClick(WindowsMessages message, Vector2 cursorPos, uint key)
        {
            if ((message != WindowsMessages.WM_KEYDOWN && key == 16) && message != WindowsMessages.WM_LBUTTONDOWN && (message != WindowsMessages.WM_KEYUP && key == 16) || (!shiftActive && message == WindowsMessages.WM_LBUTTONDOWN))
            {
                return;
            }
            if (message == WindowsMessages.WM_KEYDOWN && key == 16)
            {
                shiftActive = true;
            }
            if (message == WindowsMessages.WM_KEYUP && key == 16)
            {
                shiftActive = false;
            }
            if (message == WindowsMessages.WM_LBUTTONDOWN)
            {
                foreach (KeyValuePair<Obj_AI_Hero, ChampInfos> enemy in Enemies.Reverse())
                {
                    if (Common.IsInside(cursorPos, enemy.Value.SGui.Champ.SizeSideBar, _champSize.Width, _champSize.Height))
                    {
                        //TODO: Add Camera move
                        if (Menu.UiTracker.GetMenuItem("SAwarenessUITrackerPingActive").GetValue<bool>())
                        {
                            Packet.S2C.Ping.Encoded(new Packet.S2C.Ping.Struct(enemy.Key.ServerPosition.X,
                                enemy.Key.ServerPosition.Y, 0, 0, Packet.PingType.NormalSound)).Process();
                        }
                    }
                }
            }
            
        }

        void Drawing_OnPostReset(EventArgs args)
        {
            if (Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
                return;
            S.OnResetDevice();
            ChampF.OnResetDevice();
            SpellF.OnResetDevice();
            SumF.OnResetDevice();
            RecF.OnResetDevice();
            RecS.OnPostReset();
            RecB.OnPreReset();
            drawActive = true;
        }

        void Drawing_OnPreReset(EventArgs args)
        {
            S.OnLostDevice();
            ChampF.OnLostDevice();
            SpellF.OnLostDevice();
            SumF.OnLostDevice();
            RecF.OnLostDevice();
            RecS.OnPreReset();
            RecB.OnPreReset();
            drawActive = false;
        }

        ~UITracker()
        {
            Game.OnGameUpdate -= Game_OnGameUpdate;
            Drawing.OnPreReset -= Drawing_OnPreReset;
            Drawing.OnPostReset -= Drawing_OnPostReset;
            Drawing.OnEndScene -= Drawing_OnEndScene;
        }

        private bool drawActive = true;
        Sprite S;
        Font SpellF;
        Font ChampF;
        Font SumF;
        Font RecF;
        private Render.Rectangle RecB;
        private Render.Rectangle RecS;
        Vector2 _screen = new Vector2(Drawing.Width, Drawing.Height / 2);
        readonly Dictionary<Obj_AI_Hero, ChampInfos> Enemies = new Dictionary<Obj_AI_Hero, ChampInfos>();
        readonly Dictionary<Obj_AI_Hero, ChampInfos> Allies = new Dictionary<Obj_AI_Hero, ChampInfos>();
        private Texture _overlaySummoner;
        private Texture _overlaySummonerSpell;
        private Texture _overlaySpellItem;
        private Texture _overlayEmptyItem;
        private Texture _overlayRecall;
        private Texture _healthBar;
        private Texture _manaBar;
        private Texture _backBar;
        Size _champSize = new Size(64, 64);
        Size _sumSize = new Size(32, 32);
        Size _spellSize = new Size(16, 16);
        Size _healthManaBarSize = new Size(96, 5);
        Size _backBarSize = new Size(96, 10);
        Size _recSize = new Size(64, 12);

        

        private float scalePc = 1.0f;
        private int oldEX = 0;
        private int oldEY = 0;
        private int oldAX = 0;
        private int oldAY = 0;
        private Size hudSize = new Size();
        private bool moveActive = false;
        private bool shiftActive = false;
        private Vector2 lastCursorPos = new Vector2();

        public bool IsActive()
        {
            return Menu.Tracker.GetActive() && Menu.UiTracker.GetActive();
        }

        private bool Init(bool force)
        {
            try
            {
                S = new Sprite(Drawing.Direct3DDevice);
                RecF = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Times New Roman", 12));
                SpellF = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Times New Roman", 8));
                ChampF = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Times New Roman", 30));
                SumF = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Times New Roman", 16));
                RecS = new Render.Rectangle(0, 0, 16, 16, Color.Green);
                RecB = new Render.Rectangle(0, 0, (int)(16 * 1.7), (int)(16 * 1.7), Color.Green);
            }
            catch (Exception)
            {

                return false;
                //throw;
            }
            if (Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker").GetMenuItem("SAwarenessUITrackerEnemyTrackerXPos").GetValue<Slider>().Value == -1)
            {
                Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker").GetMenuItem("SAwarenessUITrackerEnemyTrackerXPos").SetValue(new Slider((int)_screen.X, Drawing.Width, 0));
            }
            if (Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker").GetMenuItem("SAwarenessUITrackerEnemyTrackerYPos").GetValue<Slider>().Value == -1)
            {
                Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker").GetMenuItem("SAwarenessUITrackerEnemyTrackerYPos").SetValue(new Slider((int)_screen.Y, Drawing.Height, 0));
            }
            if (Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker").GetMenuItem("SAwarenessUITrackerAllyTrackerXPos").GetValue<Slider>().Value == -1)
            {
                Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker").GetMenuItem("SAwarenessUITrackerAllyTrackerXPos").SetValue(new Slider((int)_screen.X, Drawing.Width, 0));
            }
            if (Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker").GetMenuItem("SAwarenessUITrackerAllyTrackerYPos").GetValue<Slider>().Value == -1)
            {
                Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker").GetMenuItem("SAwarenessUITrackerAllyTrackerYPos").SetValue(new Slider((int)_screen.Y, Drawing.Height, 0));
            }

            //var loc = Assembly.GetExecutingAssembly().Location;
            //loc = loc.Remove(loc.LastIndexOf("\\", StringComparison.Ordinal));
            //loc = loc + "\\Sprites\\SAwareness\\";

            //SpriteHelper.LoadTexture("SummonerTint.dds", "SUMMONERS/", loc + "SUMMONERS\\SummonerTint.dds", ref _overlaySummoner);
            //SpriteHelper.LoadTexture("SummonerSpellTint.dds", "SUMMONERS/", loc + "SUMMONERS\\SummonerSpellTint.dds", ref _overlaySummonerSpell);
            //SpriteHelper.LoadTexture("SpellTint.dds", "SUMMONERS/", loc + "SUMMONERS\\SpellTint.dds", ref _overlaySpellItem);

            //SpriteHelper.LoadTexture("BarBackground.dds", "EXT/", loc + "EXT\\BarBackground.dds", ref _backBar);
            //SpriteHelper.LoadTexture("HealthBar.dds", "EXT/", loc + "EXT\\HealthBar.dds", ref _healthBar);
            //SpriteHelper.LoadTexture("ManaBar.dds", "EXT/", loc + "EXT\\ManaBar.dds", ref _manaBar);
            //SpriteHelper.LoadTexture("ItemSlotEmpty.dds", "EXT/", loc + "EXT\\ItemSlotEmpty.dds", ref _overlayEmptyItem);
            //SpriteHelper.LoadTexture("RecallBar.dds", "EXT/", loc + "EXT\\RecallBar.dds", ref _overlayRecall);

            SpriteHelper.LoadTexture("SummonerTint", ref _overlaySummoner, SpriteHelper.TextureType.Default);
            SpriteHelper.LoadTexture("SummonerSpellTint", ref _overlaySummonerSpell, SpriteHelper.TextureType.Default);
            SpriteHelper.LoadTexture("SpellTint", ref _overlaySpellItem, SpriteHelper.TextureType.Default);

            SpriteHelper.LoadTexture("BarBackground", ref _backBar, SpriteHelper.TextureType.Default);
            SpriteHelper.LoadTexture("HealthBar", ref _healthBar, SpriteHelper.TextureType.Default);
            SpriteHelper.LoadTexture("ManaBar", ref _manaBar, SpriteHelper.TextureType.Default);
            SpriteHelper.LoadTexture("ItemSlotEmpty", ref _overlayEmptyItem, SpriteHelper.TextureType.Default);
            SpriteHelper.LoadTexture("RecallBar", ref _overlayRecall, SpriteHelper.TextureType.Default);


            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero.IsEnemy)
                {
                    var champ = new ChampInfos();
                    //SpriteHelper.LoadTexture(hero.ChampionName + ".dds", "CHAMP/", loc + "CHAMP\\" + hero.ChampionName + ".dds", ref champ.SGui.Champ.Texture);
                    SpriteHelper.LoadTexture(hero.ChampionName, ref champ.SGui.Champ.Texture, SpriteHelper.TextureType.Default);
                    var s1 = hero.Spellbook.Spells;
                    //SpriteHelper.LoadTexture(s1[0].Name + ".dds", "PASSIVE/", loc + "PASSIVE\\" + s1[0].Name + ".dds", ref champ.SGui.Passive.Texture);
                    //SpriteHelper.LoadTexture(s1[0].Name + ".dds", "SPELLS/", loc + "SPELLS\\" + s1[0].Name + ".dds", ref champ.SGui.SpellQ.Texture);
                    //SpriteHelper.LoadTexture(s1[1].Name + ".dds", "SPELLS/", loc + "SPELLS\\" + s1[1].Name + ".dds", ref champ.SGui.SpellW.Texture);
                    //SpriteHelper.LoadTexture(s1[2].Name + ".dds", "SPELLS/", loc + "SPELLS\\" + s1[2].Name + ".dds", ref champ.SGui.SpellE.Texture);
                    //SpriteHelper.LoadTexture(s1[3].Name + ".dds", "SPELLS/", loc + "SPELLS\\" + s1[3].Name + ".dds", ref champ.SGui.SpellR.Texture);
                    SpriteHelper.LoadTexture(s1[0].Name, ref champ.SGui.SpellQ.Texture, SpriteHelper.TextureType.Default);
                    SpriteHelper.LoadTexture(s1[1].Name, ref champ.SGui.SpellW.Texture, SpriteHelper.TextureType.Default);
                    SpriteHelper.LoadTexture(s1[2].Name, ref champ.SGui.SpellE.Texture, SpriteHelper.TextureType.Default);
                    SpriteHelper.LoadTexture(s1[3].Name, ref champ.SGui.SpellR.Texture, SpriteHelper.TextureType.Default);

                    //var s2 = hero.SummonerSpellbook.Spells;
                    //SpriteHelper.LoadTexture(s2[0].Name + ".dds", "SUMMONERS/", loc + "SUMMONERS\\" + s2[0].Name + ".dds", ref champ.SGui.SpellSum1.Texture);
                    //SpriteHelper.LoadTexture(s2[1].Name + ".dds", "SUMMONERS/", loc + "SUMMONERS\\" + s2[1].Name + ".dds", ref champ.SGui.SpellSum2.Texture);
                    var s2 = hero.SummonerSpellbook.Spells;
                    SpriteHelper.LoadTexture(s2[0].Name + "1", ref champ.SGui.SpellSum1.Texture, SpriteHelper.TextureType.Summoner);
                    SpriteHelper.LoadTexture(s2[1].Name + "1", ref champ.SGui.SpellSum2.Texture, SpriteHelper.TextureType.Summoner);

                    Enemies.Add(hero, champ);
                }
            }

            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (!hero.IsEnemy && !hero.IsMe)
                {
                    var champ = new ChampInfos();
                    SpriteHelper.LoadTexture(hero.ChampionName, ref champ.SGui.Champ.Texture, SpriteHelper.TextureType.Default);
                    var s1 = hero.Spellbook.Spells;
                    //SpriteHelper.LoadTexture(s1[0].Name + ".dds", "PASSIVE/", loc + "PASSIVE\\" + s1[0].Name + ".dds", ref champ.SGui.Passive.Texture);
                    SpriteHelper.LoadTexture(s1[0].Name, ref champ.SGui.SpellQ.Texture, SpriteHelper.TextureType.Default);
                    SpriteHelper.LoadTexture(s1[1].Name, ref champ.SGui.SpellW.Texture, SpriteHelper.TextureType.Default);
                    SpriteHelper.LoadTexture(s1[2].Name, ref champ.SGui.SpellE.Texture, SpriteHelper.TextureType.Default);
                    SpriteHelper.LoadTexture(s1[3].Name, ref champ.SGui.SpellR.Texture, SpriteHelper.TextureType.Default);

                    var s2 = hero.SummonerSpellbook.Spells;
                    SpriteHelper.LoadTexture(s2[0].Name, ref champ.SGui.SpellSum1.Texture, SpriteHelper.TextureType.Summoner);
                    SpriteHelper.LoadTexture(s2[1].Name + "1", ref champ.SGui.SpellSum2.Texture, SpriteHelper.TextureType.Summoner);

                    Allies.Add(hero, champ);
                }
            }
            UpdateItems(true);
            UpdateItems(false);
            CalculateSizes(true);
            CalculateSizes(false);

            return true;
        }

        private void CalculateSizes(bool calcEenemy)
        {
            Dictionary<Obj_AI_Hero, ChampInfos> heroes;
            float percentScale;
            StringList mode;
            StringList modeHead;
            int count;
            int xOffset;
            int yOffset;
            int yOffsetAdd;
            if (calcEenemy)
            {
                heroes = Enemies;
                percentScale = (float)Menu.UiTracker.GetMenuItem("SAwarenessUITrackerScale").GetValue<Slider>().Value / 100;
                mode = Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker").GetMenuItem("SAwarenessUITrackerEnemyTrackerMode").GetValue<StringList>();
                modeHead = Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker").GetMenuItem("SAwarenessUITrackerEnemyTrackerHeadMode").GetValue<StringList>();
                count = 0;
                xOffset = Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker").GetMenuItem("SAwarenessUITrackerEnemyTrackerXPos").GetValue<Slider>().Value;
                oldEX = xOffset;
                yOffset = Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker").GetMenuItem("SAwarenessUITrackerEnemyTrackerYPos").GetValue<Slider>().Value;
                oldEY = yOffset;
                yOffsetAdd = (int)(20 * percentScale);
            }
            else
            {
                heroes = Allies;
                percentScale = (float)Menu.UiTracker.GetMenuItem("SAwarenessUITrackerScale").GetValue<Slider>().Value / 100;
                mode = Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker").GetMenuItem("SAwarenessUITrackerAllyTrackerMode").GetValue<StringList>();
                modeHead = Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker").GetMenuItem("SAwarenessUITrackerAllyTrackerHeadMode").GetValue<StringList>();
                count = 0;
                xOffset = Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker").GetMenuItem("SAwarenessUITrackerAllyTrackerXPos").GetValue<Slider>().Value;
                oldEX = xOffset;
                yOffset = Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker").GetMenuItem("SAwarenessUITrackerAllyTrackerYPos").GetValue<Slider>().Value;
                oldEY = yOffset;
                yOffsetAdd = (int)(20 * percentScale);
            }
            
            hudSize = new Size();
            foreach (var hero in heroes)
            {
                if (mode.SelectedIndex == 0 || mode.SelectedIndex == 2)
                {
                    hero.Value.SGui.SpellPassive.SizeSideBar = new Size((int)xOffset - (int)(_champSize.Width * percentScale) - (int)(_sumSize.Width * percentScale) - (int)(_spellSize.Width * percentScale), (int)yOffset - (int)(_spellSize.Height * percentScale) * (count * 4 - 0) - count * (int)(_backBarSize.Height * percentScale) - count * (int)(_spellSize.Height * percentScale) - yOffsetAdd);
                    hero.Value.SGui.SpellQ.SizeSideBar = new Size(hero.Value.SGui.SpellPassive.SizeSideBar.Width, hero.Value.SGui.SpellPassive.SizeSideBar.Height + (int)(_spellSize.Height * percentScale) * 1);
                    hero.Value.SGui.SpellW.SizeSideBar = new Size(hero.Value.SGui.SpellPassive.SizeSideBar.Width, hero.Value.SGui.SpellPassive.SizeSideBar.Height + (int)(_spellSize.Height * percentScale) * 2);
                    hero.Value.SGui.SpellE.SizeSideBar = new Size(hero.Value.SGui.SpellPassive.SizeSideBar.Width, hero.Value.SGui.SpellPassive.SizeSideBar.Height + (int)(_spellSize.Height * percentScale) * 3);
                    hero.Value.SGui.SpellR.SizeSideBar = new Size(hero.Value.SGui.SpellPassive.SizeSideBar.Width, hero.Value.SGui.SpellPassive.SizeSideBar.Height + (int)(_spellSize.Height * percentScale) * 4);

                    hero.Value.SGui.Champ.SizeSideBar = new Size(hero.Value.SGui.SpellPassive.SizeSideBar.Width + (int)(_spellSize.Width * percentScale), hero.Value.SGui.SpellPassive.SizeSideBar.Height);
                    hero.Value.SGui.SpellSum1.SizeSideBar = new Size(hero.Value.SGui.Champ.SizeSideBar.Width + (int)(_champSize.Width * percentScale), hero.Value.SGui.SpellPassive.SizeSideBar.Height);
                    hero.Value.SGui.SpellSum2.SizeSideBar = new Size(hero.Value.SGui.SpellSum1.SizeSideBar.Width, hero.Value.SGui.SpellPassive.SizeSideBar.Height + (int)(_sumSize.Height * percentScale));

                    if (hero.Value.SGui.Item[0] == null)
                        hero.Value.SGui.Item[0] = new ChampInfos.Gui.SpriteInfos();
                    hero.Value.SGui.Item[0].SizeSideBar = new Size(hero.Value.SGui.SpellR.SizeSideBar.Width, hero.Value.SGui.SpellR.SizeSideBar.Height + (int)(_spellSize.Height * percentScale));
                    for (int i = 1; i < hero.Value.SGui.Item.Length; i++)
                    {
                        if (hero.Value.SGui.Item[i] == null)
                            hero.Value.SGui.Item[i] = new ChampInfos.Gui.SpriteInfos();
                        hero.Value.SGui.Item[i].SizeSideBar = new Size(hero.Value.SGui.Item[0].SizeSideBar.Width + (int)(_spellSize.Width * percentScale) * i, hero.Value.SGui.Item[0].SizeSideBar.Height);
                    }

                    hero.Value.SGui.SpellSum1.CoordsSideBar = new Size(hero.Value.SGui.SpellSum1.SizeSideBar.Width + (int)(_sumSize.Width * percentScale) / 2, hero.Value.SGui.SpellSum1.SizeSideBar.Height + (int)(_sumSize.Height * percentScale) / 8);
                    hero.Value.SGui.SpellSum2.CoordsSideBar = new Size(hero.Value.SGui.SpellSum2.SizeSideBar.Width + (int)(_sumSize.Width * percentScale) / 2, hero.Value.SGui.SpellSum2.SizeSideBar.Height + (int)(_sumSize.Height * percentScale) / 8);
                    hero.Value.SGui.Champ.CoordsSideBar = new Size(hero.Value.SGui.Champ.SizeSideBar.Width + (int)(_champSize.Width * percentScale) / 2, hero.Value.SGui.Champ.SizeSideBar.Height + (int)(_champSize.Height * percentScale) / 8);
                    hero.Value.SGui.SpellPassive.CoordsSideBar = new Size(hero.Value.SGui.SpellPassive.SizeSideBar.Width + (int)(_spellSize.Width * percentScale) / 2, hero.Value.SGui.SpellPassive.SizeSideBar.Height + (int)(_spellSize.Height * percentScale) / 8);
                    hero.Value.SGui.SpellQ.CoordsSideBar = new Size(hero.Value.SGui.SpellQ.SizeSideBar.Width + (int)(_spellSize.Width * percentScale) / 2, hero.Value.SGui.SpellQ.SizeSideBar.Height + (int)(_spellSize.Height * percentScale) / 8);
                    hero.Value.SGui.SpellW.CoordsSideBar = new Size(hero.Value.SGui.SpellW.SizeSideBar.Width + (int)(_spellSize.Width * percentScale) / 2, hero.Value.SGui.SpellW.SizeSideBar.Height + (int)(_spellSize.Height * percentScale) / 8);
                    hero.Value.SGui.SpellE.CoordsSideBar = new Size(hero.Value.SGui.SpellE.SizeSideBar.Width + (int)(_spellSize.Width * percentScale) / 2, hero.Value.SGui.SpellE.SizeSideBar.Height + (int)(_spellSize.Height * percentScale) / 8);
                    hero.Value.SGui.SpellR.CoordsSideBar = new Size(hero.Value.SGui.SpellR.SizeSideBar.Width + (int)(_spellSize.Width * percentScale) / 2, hero.Value.SGui.SpellR.SizeSideBar.Height + (int)(_spellSize.Height * percentScale) / 8);

                    hero.Value.SGui.BackBar.SizeSideBar = new Size(hero.Value.SGui.Champ.SizeSideBar.Width, hero.Value.SGui.SpellSum2.SizeSideBar.Height + (int)(_sumSize.Height * percentScale));
                    hero.Value.SGui.HealthBar.SizeSideBar = new Size(hero.Value.SGui.BackBar.SizeSideBar.Width, hero.Value.SGui.BackBar.SizeSideBar.Height);
                    hero.Value.SGui.ManaBar.SizeSideBar = new Size(hero.Value.SGui.BackBar.SizeSideBar.Width, hero.Value.SGui.BackBar.SizeSideBar.Height + (int)(_healthManaBarSize.Height * percentScale) + 3);
                    hero.Value.SGui.SHealth = ((int)hero.Key.Health) + "/" + ((int)hero.Key.MaxHealth);
                    hero.Value.SGui.SMana = ((int)hero.Key.Mana) + "/" + ((int)hero.Key.MaxMana);
                    hero.Value.SGui.HealthBar.CoordsSideBar = new Size(hero.Value.SGui.HealthBar.SizeSideBar.Width + (int)(_healthManaBarSize.Width * percentScale) / 2, hero.Value.SGui.HealthBar.SizeSideBar.Height - (int)(_healthManaBarSize.Height * percentScale) / 2);
                    hero.Value.SGui.ManaBar.CoordsSideBar = new Size(hero.Value.SGui.ManaBar.SizeSideBar.Width + (int)(_healthManaBarSize.Width * percentScale) / 2, hero.Value.SGui.ManaBar.SizeSideBar.Height - (int)(_healthManaBarSize.Height * percentScale) / 2);

                    hero.Value.SGui.RecallBar.SizeSideBar = new Size(hero.Value.SGui.Champ.SizeSideBar.Width, hero.Value.SGui.BackBar.SizeSideBar.Height - (int)(_champSize.Height * percentScale) / 4);
                    hero.Value.SGui.RecallBar.CoordsSideBar = new Size(hero.Value.SGui.RecallBar.SizeSideBar.Width + (int)(_recSize.Width * percentScale) / 2, hero.Value.SGui.RecallBar.SizeSideBar.Height - (int)(_recSize.Height * percentScale) / 2);

                    yOffsetAdd += (int)(20 * percentScale);
                    Size nSize = (hero.Value.SGui.Item[hero.Value.SGui.Item.Length - 1].SizeSideBar) -
                        (hero.Value.SGui.SpellPassive.SizeSideBar);
                    nSize.Height += (int)(8 * percentScale);
                    hudSize += nSize;
                    hudSize.Width = nSize.Width;
                    hudSize.Width += _spellSize.Width;
                    hudSize.Height += (int)(20 * percentScale);
                    count++;
                }
                if (mode.SelectedIndex == 1 || mode.SelectedIndex == 2)
                {
                    if (modeHead.SelectedIndex == 0)
                    {
                        float HpPosScale = 0.8f;
                        Vector2 hpPos = hero.Key.HPBarPosition;
                        hero.Value.SGui.SpellSum1.SizeHpBar = new Size((int)hpPos.X - 20, (int)hpPos.Y);
                        hero.Value.SGui.SpellSum2.SizeHpBar = new Size(hero.Value.SGui.SpellSum1.SizeHpBar.Width, hero.Value.SGui.SpellSum1.SizeHpBar.Height + (int)(_sumSize.Height * HpPosScale));
                        hero.Value.SGui.SpellPassive.SizeHpBar = new Size(hero.Value.SGui.SpellSum1.SizeHpBar.Width + _sumSize.Width, hero.Value.SGui.SpellSum2.SizeHpBar.Height + (int)((_spellSize.Height * HpPosScale) / 1.5));
                        hero.Value.SGui.SpellQ.SizeHpBar = new Size(hero.Value.SGui.SpellPassive.SizeHpBar.Width + _spellSize.Width, hero.Value.SGui.SpellPassive.SizeHpBar.Height);
                        hero.Value.SGui.SpellW.SizeHpBar = new Size(hero.Value.SGui.SpellQ.SizeHpBar.Width + _spellSize.Width, hero.Value.SGui.SpellQ.SizeHpBar.Height);
                        hero.Value.SGui.SpellE.SizeHpBar = new Size(hero.Value.SGui.SpellW.SizeHpBar.Width + _spellSize.Width, hero.Value.SGui.SpellW.SizeHpBar.Height);
                        hero.Value.SGui.SpellR.SizeHpBar = new Size(hero.Value.SGui.SpellE.SizeHpBar.Width + _spellSize.Width, hero.Value.SGui.SpellE.SizeHpBar.Height);

                        hero.Value.SGui.SpellSum1.CoordsHpBar = new Size(hero.Value.SGui.SpellSum1.SizeHpBar.Width + _sumSize.Width / 2, hero.Value.SGui.SpellSum1.SizeHpBar.Height + _sumSize.Height / 8);
                        hero.Value.SGui.SpellSum2.CoordsHpBar = new Size(hero.Value.SGui.SpellSum2.SizeHpBar.Width + _sumSize.Width / 2, hero.Value.SGui.SpellSum2.SizeHpBar.Height + _sumSize.Height / 8);
                        hero.Value.SGui.SpellPassive.CoordsHpBar = new Size(hero.Value.SGui.SpellPassive.SizeHpBar.Width + _spellSize.Width / 2, hero.Value.SGui.SpellPassive.SizeHpBar.Height + _spellSize.Height / 8);
                        hero.Value.SGui.SpellQ.CoordsHpBar = new Size(hero.Value.SGui.SpellQ.SizeHpBar.Width + _spellSize.Width / 2, hero.Value.SGui.SpellQ.SizeHpBar.Height + _spellSize.Height / 8);
                        hero.Value.SGui.SpellW.CoordsHpBar = new Size(hero.Value.SGui.SpellW.SizeHpBar.Width + _spellSize.Width / 2, hero.Value.SGui.SpellW.SizeHpBar.Height + _spellSize.Height / 8);
                        hero.Value.SGui.SpellE.CoordsHpBar = new Size(hero.Value.SGui.SpellE.SizeHpBar.Width + _spellSize.Width / 2, hero.Value.SGui.SpellE.SizeHpBar.Height + _spellSize.Height / 8);
                        hero.Value.SGui.SpellR.CoordsHpBar = new Size(hero.Value.SGui.SpellR.SizeHpBar.Width + _spellSize.Width / 2, hero.Value.SGui.SpellR.SizeHpBar.Height + _spellSize.Height / 8);
                    }
                    else
                    {
                        float HpPosScale = 1.7f;
                        Vector2 hpPos = hero.Key.HPBarPosition;
                        hero.Value.SGui.SpellSum1.SizeHpBar = new Size((int)hpPos.X - 25, (int)hpPos.Y + 2);
                        hero.Value.SGui.SpellSum2.SizeHpBar = new Size(hero.Value.SGui.SpellSum1.SizeHpBar.Width, hero.Value.SGui.SpellSum1.SizeHpBar.Height + (int)(_sumSize.Height * 1.0f));
                        hero.Value.SGui.SpellPassive.SizeHpBar = new Size(hero.Value.SGui.SpellSum1.SizeHpBar.Width + (int)(_spellSize.Width * HpPosScale), hero.Value.SGui.SpellSum2.SizeHpBar.Height);
                        hero.Value.SGui.SpellQ.SizeHpBar = new Size(hero.Value.SGui.SpellPassive.SizeHpBar.Width + (int)(_spellSize.Width * HpPosScale), hero.Value.SGui.SpellPassive.SizeHpBar.Height);
                        hero.Value.SGui.SpellW.SizeHpBar = new Size(hero.Value.SGui.SpellQ.SizeHpBar.Width + (int)(_spellSize.Width * HpPosScale), hero.Value.SGui.SpellQ.SizeHpBar.Height);
                        hero.Value.SGui.SpellE.SizeHpBar = new Size(hero.Value.SGui.SpellW.SizeHpBar.Width + (int)(_spellSize.Width * HpPosScale), hero.Value.SGui.SpellW.SizeHpBar.Height);
                        hero.Value.SGui.SpellR.SizeHpBar = new Size(hero.Value.SGui.SpellE.SizeHpBar.Width + (int)(_spellSize.Width * HpPosScale), hero.Value.SGui.SpellE.SizeHpBar.Height);

                        hero.Value.SGui.SpellSum1.CoordsHpBar = new Size(hero.Value.SGui.SpellSum1.SizeHpBar.Width + _sumSize.Width / 2, hero.Value.SGui.SpellSum1.SizeHpBar.Height + _sumSize.Height / 8);
                        hero.Value.SGui.SpellSum2.CoordsHpBar = new Size(hero.Value.SGui.SpellSum2.SizeHpBar.Width + _sumSize.Width / 2, hero.Value.SGui.SpellSum2.SizeHpBar.Height + _sumSize.Height / 8);
                        hero.Value.SGui.SpellPassive.CoordsHpBar = new Size(hero.Value.SGui.SpellPassive.SizeHpBar.Width + (int)(_spellSize.Width / 1.7), hero.Value.SGui.SpellPassive.SizeHpBar.Height + _spellSize.Height / 8);
                        hero.Value.SGui.SpellQ.CoordsHpBar = new Size(hero.Value.SGui.SpellQ.SizeHpBar.Width + (int)(_spellSize.Width / 1.3), hero.Value.SGui.SpellQ.SizeHpBar.Height + _spellSize.Height / 8);
                        hero.Value.SGui.SpellW.CoordsHpBar = new Size(hero.Value.SGui.SpellW.SizeHpBar.Width + (int)(_spellSize.Width / 1.3), hero.Value.SGui.SpellW.SizeHpBar.Height + _spellSize.Height / 8);
                        hero.Value.SGui.SpellE.CoordsHpBar = new Size(hero.Value.SGui.SpellE.SizeHpBar.Width + (int)(_spellSize.Width / 1.3), hero.Value.SGui.SpellE.SizeHpBar.Height + _spellSize.Height / 8);
                        hero.Value.SGui.SpellR.CoordsHpBar = new Size(hero.Value.SGui.SpellR.SizeHpBar.Width + (int)(_spellSize.Width / 1.3), hero.Value.SGui.SpellR.SizeHpBar.Height + _spellSize.Height / 8);
                    }                    
                }                
            }           
        }

        private void UpdateItems(bool enemy)
        {
            if (!Menu.UiTracker.GetMenuItem("SAwarenessItemPanelActive").GetValue<bool>())
                return;
            //var loc = Assembly.GetExecutingAssembly().Location;
            //loc = loc.Remove(loc.LastIndexOf("\\", StringComparison.Ordinal));
            //loc = loc + "\\Sprites\\SAwareness\\";

            Dictionary<Obj_AI_Hero, ChampInfos> heroes;

            if (enemy)
            {
                heroes = Enemies;
            }
            else
            {
                heroes = Allies;
            }

            foreach (var hero in heroes)
            {
                InventorySlot[] i1 = hero.Key.InventoryItems;
                var champ = hero.Value;
                var slot = new List<int>();
                var unusedId = new List<int> { 0, 1, 2, 3, 4, 5, 6 };
                foreach (var inventorySlot in i1)
                {
                        slot.Add(inventorySlot.Slot);
                        if (inventorySlot.Slot >= 0 && inventorySlot.Slot <= 6)
                        {
                            unusedId.Remove(inventorySlot.Slot);
                            if (champ.SGui.Item[inventorySlot.Slot] == null)
                                champ.SGui.Item[inventorySlot.Slot] = new ChampInfos.Gui.SpriteInfos();
                            if (champ.SGui.Item[inventorySlot.Slot].Texture == null || champ.SGui.ItemId[inventorySlot.Slot] != inventorySlot.Id)
                            {
                                //SpriteHelper.LoadTexture(inventorySlot.Id + ".dds", "ITEMS/",
                                //    loc + "ITEMS\\" + inventorySlot.Id + ".dds",
                                //    ref champ.SGui.Item[inventorySlot.Slot].Texture, true);
                                SpriteHelper.LoadTexture(inventorySlot.Id.ToString(), ref champ.SGui.Item[inventorySlot.Slot].Texture, SpriteHelper.TextureType.Item);
                                if( champ.SGui.Item[inventorySlot.Slot].Texture != null )
                                    champ.SGui.ItemId[inventorySlot.Slot] = inventorySlot.Id;
                            }
                        }
                }

                for (int i = 0; i < unusedId.Count; i++)
                {
                    int id = unusedId[i];
                    champ.SGui.ItemId[id] = 0;
                    if (champ.SGui.Item[id] == null)
                        champ.SGui.Item[id] = new ChampInfos.Gui.SpriteInfos();
                    champ.SGui.Item[id].Texture = null;
                    if (/*id == i*/champ.SGui.Item[id].Texture == null && champ.SGui.Item[id].Texture != _overlayEmptyItem)
                    {
                        champ.SGui.Item[id].Texture = _overlayEmptyItem;
                    }
                }
            }
        }

        void UpdateCds(Dictionary<Obj_AI_Hero, ChampInfos> heroes)
        {
            try
            {
                UpdateItems(true);
                UpdateItems(false);

                if (ObjectManager.Player.DeathDuration > 0.0f)
                {

                }
                foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
                {
                    foreach (var enemy in heroes)
                    {
                        enemy.Value.SGui.SHealth = ((int)enemy.Key.Health) + "/" + ((int)enemy.Key.MaxHealth);
                        enemy.Value.SGui.SMana = ((int)enemy.Key.Mana) + "/" + ((int)enemy.Key.MaxMana);
                        if (enemy.Key.NetworkId == hero.NetworkId)
                        {
                            //InventorySlot[] i1 = hero.InventoryItems; TODO: Add Item Cooldowns

                            var s1 = hero.Spellbook.Spells;
                            if (s1[0].CooldownExpires - Game.Time > 0.0f)
                            {
                                enemy.Value.SGui.SpellQ.Cd = (int)(s1[0].CooldownExpires - Game.Time);
                            }
                            else if (s1[0].CooldownExpires - Game.Time <= 0.0f && enemy.Value.SGui.SpellQ.Cd != 0)
                            {
                                enemy.Value.SGui.SpellQ.Cd = 0;
                            }
                            if (s1[1].CooldownExpires - Game.Time > 0.0f)
                            {
                                enemy.Value.SGui.SpellW.Cd = (int)(s1[1].CooldownExpires - Game.Time);
                            }
                            else if (s1[1].CooldownExpires - Game.Time <= 0.0f && enemy.Value.SGui.SpellW.Cd != 0)
                            {
                                enemy.Value.SGui.SpellW.Cd = 0;
                            }
                            if (s1[2].CooldownExpires - Game.Time > 0.0f)
                            {
                                enemy.Value.SGui.SpellE.Cd = (int)(s1[2].CooldownExpires - Game.Time);
                            }
                            else if (s1[2].CooldownExpires - Game.Time <= 0.0f && enemy.Value.SGui.SpellE.Cd != 0)
                            {
                                enemy.Value.SGui.SpellE.Cd = 0;
                            }
                            if (s1[3].CooldownExpires - Game.Time > 0.0f)
                            {
                                enemy.Value.SGui.SpellR.Cd = (int)(s1[3].CooldownExpires - Game.Time);
                            }
                            else if (s1[3].CooldownExpires - Game.Time <= 0.0f && enemy.Value.SGui.SpellR.Cd != 0)
                            {
                                enemy.Value.SGui.SpellR.Cd = 0;
                            }
                            var s2 = hero.SummonerSpellbook.Spells;
                            if (s2[0].CooldownExpires - Game.Time > 0.0f)
                            {
                                enemy.Value.SGui.SpellSum1.Cd = (int)(s2[0].CooldownExpires - Game.Time);
                            }
                            else if (s2[0].CooldownExpires - Game.Time <= 0.0f && enemy.Value.SGui.SpellSum1.Cd != 0)
                            {
                                enemy.Value.SGui.SpellSum1.Cd = 0;
                            }
                            if (s2[1].CooldownExpires - Game.Time > 0.0f)
                            {
                                enemy.Value.SGui.SpellSum2.Cd = (int)(s2[1].CooldownExpires - Game.Time);
                            }
                            else if (s2[1].CooldownExpires - Game.Time <= 0.0f && enemy.Value.SGui.SpellSum2.Cd != 0)
                            {
                                enemy.Value.SGui.SpellSum2.Cd = 0;
                            }
                            if (hero.DeathDuration - Game.Time > 0.0f)
                            {
                                enemy.Value.SGui.DeathTime = (int)(hero.DeathDuration - Game.Time);
                            }
                            if (hero.IsVisible)
                            {
                                enemy.Value.SGui.InvisibleTime = 0;
                                enemy.Value.SGui.VisibleTime = (int)Game.Time;
                            }
                            else
                            {
                                if (enemy.Value.SGui.VisibleTime != 0)
                                {
                                    enemy.Value.SGui.InvisibleTime = (int)(Game.Time - enemy.Value.SGui.VisibleTime);
                                }
                                else
                                {
                                    enemy.Value.SGui.InvisibleTime = 0;
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("UITrackerUpdate: " + ex.ToString());
                throw;
            }
        }

        void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive())
                return;
            
            UpdateCds(Enemies);
            UpdateCds(Allies);

        }

        float CalcHpBar(Obj_AI_Hero hero)
        {
            float percent = (100 / hero.MaxHealth * hero.Health);
            return percent / 100;
        }

        float CalcManaBar(Obj_AI_Hero hero)
        {
            float percent = (100 / hero.MaxMana * hero.Mana);
            return (percent <= 0 || Single.IsNaN(percent) ? 0 : percent / 100);
        }

        float CalcRecallBar(RecallDetector.RecallInfo recall)
        {
            float maxTime = (recall.Recall.Duration / 1000);
            float percent = (100 / maxTime * (Game.Time - recall.StartTime));
            return (percent <= 100 ? percent / 100 : 1);
        }

        System.Drawing.Font CalcFont(int size, float scale)
        {
            double calcSize = (int)(size * scale);
            int newSize = (int)Math.Ceiling(calcSize);
            if (newSize%2 == 0 && newSize != 0)
                return new System.Drawing.Font("Times New Roman", (int)(size * scale));
            else 
                return null;
        }

        void CheckValidSprite(ref Sprite sprite)
        {
            if (sprite.Device != Drawing.Direct3DDevice)
            {
                sprite = new Sprite(Drawing.Direct3DDevice);
            }
        }

        void CheckValidFont(ref Font font)
        {
            if (font.Device != Drawing.Direct3DDevice)
            {
                AssingFonts(scalePc, true);
            }
        }

        void AssingFonts(float percentScale, bool force = false)
        {
            System.Drawing.Font font = CalcFont(12, percentScale);
            if (font != null || force)
                RecF = new Font(Drawing.Direct3DDevice, font);
            font = CalcFont(8, percentScale);
            if (font != null || force)
                SpellF = new Font(Drawing.Direct3DDevice, font);
            font = CalcFont(30, percentScale);
            if (font != null || force)
                ChampF = new Font(Drawing.Direct3DDevice, font);
            font = CalcFont(16, percentScale);
            if (font != null || force)
                SumF = new Font(Drawing.Direct3DDevice, font);
        }

        RecallDetector.RecallInfo GetRecall(int networkId)
        {
            StringList t = Menu.RecallDetector.GetMenuItem("SAwarenessRecallDetectorMode").GetValue<StringList>();
            if (t.SelectedIndex == 1 || t.SelectedIndex == 2)
            {
                RecallDetector recallDetector = (RecallDetector)Menu.RecallDetector.Item;
                if (recallDetector == null)
                    return null;
                foreach (var info in recallDetector._recalls)
                {
                    if (info.NetworkId == networkId)
                    {
                        return info;
                    }
                }
            }
            return null;
        }

        void DrawInterface(bool enemy)
        {
            try
            {
                StringList modeChoice;
                StringList modeHeadChoice;
                StringList modeHeadDisplayChoice;
                Dictionary<Obj_AI_Hero, ChampInfos> heroes;
                if (enemy)
                {
                    heroes = Enemies;
                    modeChoice = Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker").GetMenuItem("SAwarenessUITrackerEnemyTrackerMode").GetValue<StringList>();
                    modeHeadChoice = Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker").GetMenuItem("SAwarenessUITrackerEnemyTrackerHeadMode").GetValue<StringList>();
                    modeHeadDisplayChoice = Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker").GetMenuItem("SAwarenessUITrackerEnemyTrackerHeadDisplayMode").GetValue<StringList>();
                }
                else
                {
                    heroes = Allies;
                    modeChoice = Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker").GetMenuItem("SAwarenessUITrackerAllyTrackerMode").GetValue<StringList>();
                    modeHeadChoice = Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker").GetMenuItem("SAwarenessUITrackerAllyTrackerHeadMode").GetValue<StringList>();
                    modeHeadDisplayChoice = Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker").GetMenuItem("SAwarenessUITrackerAllyTrackerHeadDisplayMode").GetValue<StringList>();
                }

                float percentScale =
                    (float)Menu.UiTracker.GetMenuItem("SAwarenessUITrackerScale").GetValue<Slider>().Value / 100;
                if (Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker").GetMenuItem("SAwarenessUITrackerEnemyTrackerXPos").GetValue<Slider>().Value != oldEX ||
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker").GetMenuItem("SAwarenessUITrackerEnemyTrackerYPos").GetValue<Slider>().Value != oldEY
                    ||
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker").GetMenuItem("SAwarenessUITrackerAllyTrackerXPos").GetValue<Slider>().Value != oldAX ||
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker").GetMenuItem("SAwarenessUITrackerAllyTrackerYPos").GetValue<Slider>().Value != oldAY
                    || percentScale != scalePc)
                {
                    CalculateSizes(true);
                    CalculateSizes(false);
                }
                   
                if (percentScale != scalePc)
                {
                    scalePc = percentScale;
                    AssingFonts(percentScale);
                }

                StringList mode = modeChoice;
                StringList modeHead = modeHeadChoice;
                if (S == null || S.IsDisposed)
                {
                    return;
                }
                if (mode.SelectedIndex == 0 || mode.SelectedIndex == 2)
                {
                    S.Begin();
                    foreach (var hero in heroes)
                    {
                        var percentHealth = CalcHpBar(hero.Key);
                        var percentMana = CalcManaBar(hero.Key);

                        //DrawSprite(S, enemy.Value.PassiveTexture, nPassiveSize, Color.White);
                        DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellQ.Texture,
                            hero.Value.SGui.SpellQ.SizeSideBar,
                            new[] { 1.0f * percentScale, 1.0f * percentScale });
                        DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellW.Texture,
                            hero.Value.SGui.SpellW.SizeSideBar,
                            new[] { 1.0f * percentScale, 1.0f * percentScale });
                        DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellE.Texture,
                            hero.Value.SGui.SpellE.SizeSideBar,
                            new[] { 1.0f * percentScale, 1.0f * percentScale });
                        DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellR.Texture,
                            hero.Value.SGui.SpellR.SizeSideBar,
                            new[] { 1.0f * percentScale, 1.0f * percentScale });

                        DirectXDrawer.DrawSprite(S, hero.Value.SGui.Champ.Texture,
                            hero.Value.SGui.Champ.SizeSideBar,
                            new[] { 1.0f * percentScale, 1.0f * percentScale });
                        DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellSum1.Texture,
                            hero.Value.SGui.SpellSum1.SizeSideBar,
                            new[] { 1.0f * percentScale, 1.0f * percentScale });
                        DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellSum2.Texture,
                            hero.Value.SGui.SpellSum2.SizeSideBar,
                            new[] { 1.0f * percentScale, 1.0f * percentScale });

                        DirectXDrawer.DrawSprite(S, _backBar,
                            hero.Value.SGui.BackBar.SizeSideBar,
                            new[] { 1.0f * percentScale * 0.75f, 1.0f * percentScale });
                        DirectXDrawer.DrawSprite(S, _healthBar,
                            hero.Value.SGui.HealthBar.SizeSideBar,
                            new[] { 1.0f * percentHealth * percentScale * 0.75f, 1.0f * percentScale });
                        DirectXDrawer.DrawSprite(S, _manaBar,
                            hero.Value.SGui.ManaBar.SizeSideBar,
                            new[] { 1.0f * percentMana * percentScale * 0.75f, 1.0f * percentScale });

                        if (Menu.UiTracker.GetMenuItem("SAwarenessItemPanelActive").GetValue<bool>())
                        {
                            foreach (var spriteInfo in hero.Value.SGui.Item)
                            {
                                DirectXDrawer.DrawSprite(S, spriteInfo.Texture,
                                    spriteInfo.SizeSideBar,
                                    new[] { 1.0f * percentScale, 1.0f * percentScale });
                            }
                        }

                        if (hero.Value.SGui.SpellQ.Cd > 0.0f || hero.Key.Spellbook.GetSpell(SpellSlot.Q).Level < 1)
                        {
                            DirectXDrawer.DrawSprite(S, _overlaySpellItem,
                                hero.Value.SGui.SpellQ.SizeSideBar,
                                new ColorBGRA(Color3.White, 0.55f), new[] { 1.0f * percentScale, 1.0f * percentScale });
                        }
                        if (hero.Value.SGui.SpellW.Cd > 0.0f || hero.Key.Spellbook.GetSpell(SpellSlot.W).Level < 1)
                        {
                            DirectXDrawer.DrawSprite(S, _overlaySpellItem,
                                hero.Value.SGui.SpellW.SizeSideBar,
                                new ColorBGRA(Color3.White, 0.55f), new[] { 1.0f * percentScale, 1.0f * percentScale });
                        }
                        if (hero.Value.SGui.SpellE.Cd > 0.0f || hero.Key.Spellbook.GetSpell(SpellSlot.E).Level < 1)
                        {
                            DirectXDrawer.DrawSprite(S, _overlaySpellItem,
                                hero.Value.SGui.SpellE.SizeSideBar,
                                new ColorBGRA(Color3.White, 0.55f), new[] { 1.0f * percentScale, 1.0f * percentScale });
                        }
                        if (hero.Value.SGui.SpellR.Cd > 0.0f || hero.Key.Spellbook.GetSpell(SpellSlot.R).Level < 1)
                        {
                            DirectXDrawer.DrawSprite(S, _overlaySpellItem,
                                hero.Value.SGui.SpellR.SizeSideBar,
                                new ColorBGRA(Color3.White, 0.55f), new[] { 1.0f * percentScale, 1.0f * percentScale });
                        }
                        if (hero.Value.SGui.DeathTime > 0.0f)
                        {
                            DirectXDrawer.DrawSprite(S, _overlaySummoner,
                                hero.Value.SGui.Champ.SizeSideBar,
                                new ColorBGRA(Color3.White, 0.55f), new[] { 1.0f * percentScale, 1.0f * percentScale });
                        }
                        if (hero.Value.SGui.SpellSum1.Cd > 0.0f)
                        {
                            DirectXDrawer.DrawSprite(S, _overlaySummonerSpell,
                                hero.Value.SGui.SpellSum1.SizeSideBar,
                                new ColorBGRA(Color3.White, 0.55f), new[] { 1.0f * percentScale, 1.0f * percentScale });
                        }
                        if (hero.Value.SGui.SpellSum2.Cd > 0.0f)
                        {
                            DirectXDrawer.DrawSprite(S, _overlaySummonerSpell,
                                hero.Value.SGui.SpellSum2.SizeSideBar,
                                new ColorBGRA(Color3.White, 0.55f), new[] { 1.0f * percentScale, 1.0f * percentScale });
                        }
                        if (Menu.RecallDetector.GetActive())
                        {
                            RecallDetector.RecallInfo info = GetRecall(hero.Key.NetworkId);
                            if (info != null)
                            {
                                var percentRecall = CalcRecallBar(info);
                                if (info != null && info.StartTime != 0)
                                {
                                    float time = Game.Time + info.Recall.Duration / 1000 - info.StartTime;
                                    if (time > 0.0f &&
                                        (info.Recall.Status == Packet.S2C.Recall.RecallStatus.TeleportStart ||
                                         info.Recall.Status == Packet.S2C.Recall.RecallStatus.RecallStarted))
                                    {
                                        DirectXDrawer.DrawSprite(S, _overlayRecall,
                                            hero.Value.SGui.RecallBar.SizeSideBar,
                                            new ColorBGRA(Color3.White, 0.80f),
                                            new[] { 1.0f * percentRecall * percentScale, 1.0f * percentScale });
                                    }
                                    else if (time < 30.0f &&
                                             (info.Recall.Status == Packet.S2C.Recall.RecallStatus.TeleportEnd ||
                                              info.Recall.Status == Packet.S2C.Recall.RecallStatus.RecallFinished))
                                    {
                                        DirectXDrawer.DrawSprite(S, _overlayRecall,
                                            hero.Value.SGui.RecallBar.SizeSideBar,
                                            new ColorBGRA(Color3.White, 0.80f),
                                            new[] { 1.0f * percentScale, 1.0f * percentScale });
                                    }
                                    else if (time < 30.0f &&
                                             (info.Recall.Status == Packet.S2C.Recall.RecallStatus.TeleportAbort ||
                                              info.Recall.Status == Packet.S2C.Recall.RecallStatus.RecallAborted))
                                    {
                                        DirectXDrawer.DrawSprite(S, _overlayRecall,
                                            hero.Value.SGui.RecallBar.SizeSideBar,
                                            new ColorBGRA(Color3.White, 0.80f),
                                            new[] { 1.0f * percentScale, 1.0f * percentScale });
                                    }
                                }
                            }
                        }
                    }
                    S.End();

                    foreach (var hero in heroes)
                    {
                        DirectXDrawer.DrawText(SpellF, hero.Value.SGui.SHealth,
                            hero.Value.SGui.HealthBar.CoordsSideBar, Color.Orange);
                        DirectXDrawer.DrawText(SpellF, hero.Value.SGui.SMana,
                            hero.Value.SGui.ManaBar.CoordsSideBar, Color.Orange);
                        if (hero.Value.SGui.SpellQ.Cd > 0.0f)
                        {
                            DirectXDrawer.DrawText(SpellF, hero.Value.SGui.SpellQ.Cd.ToString(),
                                hero.Value.SGui.SpellQ.CoordsSideBar, Color.Orange);
                        }
                        if (hero.Value.SGui.SpellW.Cd > 0.0f)
                        {
                            DirectXDrawer.DrawText(SpellF, hero.Value.SGui.SpellW.Cd.ToString(),
                                hero.Value.SGui.SpellW.CoordsSideBar, Color.Orange);
                        }
                        if (hero.Value.SGui.SpellE.Cd > 0.0f)
                        {
                            DirectXDrawer.DrawText(SpellF, hero.Value.SGui.SpellE.Cd.ToString(),
                                hero.Value.SGui.SpellE.CoordsSideBar, Color.Orange);
                        }
                        if (hero.Value.SGui.SpellR.Cd > 0.0f)
                        {
                            DirectXDrawer.DrawText(SpellF, hero.Value.SGui.SpellR.Cd.ToString(),
                                hero.Value.SGui.SpellR.CoordsSideBar, Color.Orange);
                        }
                        if (hero.Value.SGui.DeathTime > 0.0f && hero.Key.IsDead)
                        {
                            DirectXDrawer.DrawText(ChampF, hero.Value.SGui.DeathTime.ToString(),
                                hero.Value.SGui.Champ.CoordsSideBar, Color.Orange);
                        }
                        else if (hero.Value.SGui.InvisibleTime > 0.0f && !hero.Key.IsVisible)
                        {
                            DirectXDrawer.DrawText(ChampF, hero.Value.SGui.InvisibleTime.ToString(),
                                hero.Value.SGui.Champ.CoordsSideBar, Color.Red);
                        }
                        if (hero.Value.SGui.SpellSum1.Cd > 0.0f)
                        {
                            DirectXDrawer.DrawText(SumF, hero.Value.SGui.SpellSum1.Cd.ToString(),
                                hero.Value.SGui.SpellSum1.CoordsSideBar, Color.Orange);
                        }
                        if (hero.Value.SGui.SpellSum2.Cd > 0.0f)
                        {
                            DirectXDrawer.DrawText(SumF, hero.Value.SGui.SpellSum2.Cd.ToString(),
                                hero.Value.SGui.SpellSum2.CoordsSideBar, Color.Orange);
                        }
                        if (Menu.RecallDetector.GetActive())
                        {
                            RecallDetector.RecallInfo info = GetRecall(hero.Key.NetworkId);
                            if (info != null && info.StartTime != 0)
                            {
                                float time = Game.Time + info.Recall.Duration / 1000 - info.StartTime;
                                if (time > 0.0f &&
                                    (info.Recall.Status == Packet.S2C.Recall.RecallStatus.TeleportStart ||
                                     info.Recall.Status == Packet.S2C.Recall.RecallStatus.RecallStarted))
                                {
                                    DirectXDrawer.DrawText(RecF, "Porting",
                                        hero.Value.SGui.RecallBar.CoordsSideBar,
                                        Color.Chartreuse);
                                }
                                else if (time < 30.0f &&
                                         (info.Recall.Status == Packet.S2C.Recall.RecallStatus.TeleportEnd ||
                                          info.Recall.Status == Packet.S2C.Recall.RecallStatus.RecallFinished))
                                {
                                    DirectXDrawer.DrawText(RecF, "Ported",
                                        hero.Value.SGui.RecallBar.CoordsSideBar,
                                        Color.Chartreuse);
                                }
                                else if (time < 30.0f &&
                                         (info.Recall.Status == Packet.S2C.Recall.RecallStatus.TeleportAbort ||
                                          info.Recall.Status == Packet.S2C.Recall.RecallStatus.RecallAborted))
                                {
                                    DirectXDrawer.DrawText(RecF, "Canceled",
                                        hero.Value.SGui.RecallBar.CoordsSideBar,
                                        Color.Chartreuse);
                                }
                            }
                        }
                    }
                }
                if (mode.SelectedIndex == 1 || mode.SelectedIndex == 2)
                {
                    CalculateSizes(true);
                    CalculateSizes(false);
                    if (modeHeadDisplayChoice.SelectedIndex == 0)
                    {
                        if (modeHead.SelectedIndex == 0)
                        {
                            S.Begin();
                            foreach (var hero in heroes)
                            {
                                if (!hero.Key.IsDead && hero.Key.IsVisible)
                                {
                                    DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellPassive.Texture,
                                        hero.Value.SGui.SpellPassive.SizeHpBar,
                                        new[] {1.0f*percentScale, 1.0f*percentScale});
                                    DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellQ.Texture,
                                        hero.Value.SGui.SpellQ.SizeHpBar,
                                        new[] {1.0f*percentScale, 1.0f*percentScale});
                                    DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellW.Texture,
                                        hero.Value.SGui.SpellW.SizeHpBar,
                                        new[] {1.0f*percentScale, 1.0f*percentScale});
                                    DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellE.Texture,
                                        hero.Value.SGui.SpellE.SizeHpBar,
                                        new[] {1.0f*percentScale, 1.0f*percentScale});
                                    DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellR.Texture,
                                        hero.Value.SGui.SpellR.SizeHpBar,
                                        new[] {1.0f*percentScale, 1.0f*percentScale});
                                    DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellSum1.Texture,
                                        hero.Value.SGui.SpellSum1.SizeHpBar,
                                        new[] {0.8f*percentScale, 0.8f*percentScale});
                                    DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellSum2.Texture,
                                        hero.Value.SGui.SpellSum2.SizeHpBar,
                                        new[] {0.8f*percentScale, 0.8f*percentScale});

                                    if (hero.Value.SGui.SpellQ.Cd > 0.0f ||
                                        hero.Key.Spellbook.GetSpell(SpellSlot.Q).Level < 1)
                                    {
                                        DirectXDrawer.DrawSprite(S, _overlaySpellItem,
                                            hero.Value.SGui.SpellQ.SizeHpBar,
                                            new ColorBGRA(Color3.White, 0.55f),
                                            new[] {1.0f*percentScale, 1.0f*percentScale});
                                    }
                                    if (hero.Value.SGui.SpellW.Cd > 0.0f ||
                                        hero.Key.Spellbook.GetSpell(SpellSlot.W).Level < 1)
                                    {
                                        DirectXDrawer.DrawSprite(S, _overlaySpellItem,
                                            hero.Value.SGui.SpellW.SizeHpBar,
                                            new ColorBGRA(Color3.White, 0.55f),
                                            new[] {1.0f*percentScale, 1.0f*percentScale});
                                    }
                                    if (hero.Value.SGui.SpellE.Cd > 0.0f ||
                                        hero.Key.Spellbook.GetSpell(SpellSlot.E).Level < 1)
                                    {
                                        DirectXDrawer.DrawSprite(S, _overlaySpellItem,
                                            hero.Value.SGui.SpellE.SizeHpBar,
                                            new ColorBGRA(Color3.White, 0.55f),
                                            new[] {1.0f*percentScale, 1.0f*percentScale});
                                    }
                                    if (hero.Value.SGui.SpellR.Cd > 0.0f ||
                                        hero.Key.Spellbook.GetSpell(SpellSlot.R).Level < 1)
                                    {
                                        DirectXDrawer.DrawSprite(S, _overlaySpellItem,
                                            hero.Value.SGui.SpellR.SizeHpBar,
                                            new ColorBGRA(Color3.White, 0.55f),
                                            new[] {1.0f*percentScale, 1.0f*percentScale});
                                    }
                                    if (hero.Value.SGui.SpellSum1.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawSprite(S, _overlaySummonerSpell,
                                            hero.Value.SGui.SpellSum1.SizeHpBar,
                                            new ColorBGRA(Color3.White, 0.55f),
                                            new[] {0.8f*percentScale, 0.8f*percentScale});
                                    }
                                    if (hero.Value.SGui.SpellSum2.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawSprite(S, _overlaySummonerSpell,
                                            hero.Value.SGui.SpellSum2.SizeHpBar,
                                            new ColorBGRA(Color3.White, 0.55f),
                                            new[] {0.8f*percentScale, 0.8f*percentScale});
                                    }
                                }
                            }
                            S.End();
                            foreach (var hero in heroes)
                            {
                                if (!hero.Key.IsDead && hero.Key.IsVisible)
                                {
                                    if (hero.Value.SGui.SpellQ.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SpellF, hero.Value.SGui.SpellQ.Cd.ToString(),
                                            hero.Value.SGui.SpellQ.CoordsHpBar, Color.Orange);
                                    }
                                    if (hero.Value.SGui.SpellW.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SpellF, hero.Value.SGui.SpellW.Cd.ToString(),
                                            hero.Value.SGui.SpellW.CoordsHpBar, Color.Orange);
                                    }
                                    if (hero.Value.SGui.SpellE.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SpellF, hero.Value.SGui.SpellE.Cd.ToString(),
                                            hero.Value.SGui.SpellE.CoordsHpBar, Color.Orange);
                                    }
                                    if (hero.Value.SGui.SpellR.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SpellF, hero.Value.SGui.SpellR.Cd.ToString(),
                                            hero.Value.SGui.SpellR.CoordsHpBar, Color.Orange);
                                    }
                                    if (hero.Value.SGui.SpellSum1.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SumF, hero.Value.SGui.SpellSum1.Cd.ToString(),
                                            hero.Value.SGui.SpellSum1.CoordsHpBar, Color.Orange);
                                    }
                                    if (hero.Value.SGui.SpellSum2.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SumF, hero.Value.SGui.SpellSum2.Cd.ToString(),
                                            hero.Value.SGui.SpellSum2.CoordsHpBar, Color.Orange);
                                    }
                                }
                            }
                        }
                        else
                        {
                            S.Begin();
                            foreach (var hero in heroes)
                            {
                                if (!hero.Key.IsDead && hero.Key.IsVisible)
                                {
                                    DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellPassive.Texture,
                                        hero.Value.SGui.SpellPassive.SizeHpBar,
                                        new[] {1.7f*percentScale, 1.7f*percentScale});
                                    DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellQ.Texture,
                                        hero.Value.SGui.SpellQ.SizeHpBar,
                                        new[] {1.7f*percentScale, 1.7f*percentScale});
                                    DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellW.Texture,
                                        hero.Value.SGui.SpellW.SizeHpBar,
                                        new[] {1.7f*percentScale, 1.7f*percentScale});
                                    DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellE.Texture,
                                        hero.Value.SGui.SpellE.SizeHpBar,
                                        new[] {1.7f*percentScale, 1.7f*percentScale});
                                    DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellR.Texture,
                                        hero.Value.SGui.SpellR.SizeHpBar,
                                        new[] {1.7f*percentScale, 1.7f*percentScale});
                                    DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellSum1.Texture,
                                        hero.Value.SGui.SpellSum1.SizeHpBar,
                                        new[] {1.0f*percentScale, 1.0f*percentScale});
                                    DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellSum2.Texture,
                                        hero.Value.SGui.SpellSum2.SizeHpBar,
                                        new[] {1.0f*percentScale, 1.0f*percentScale});

                                    if (hero.Value.SGui.SpellQ.Cd > 0.0f ||
                                        hero.Key.Spellbook.GetSpell(SpellSlot.Q).Level < 1)
                                    {
                                        DirectXDrawer.DrawSprite(S, _overlaySpellItem,
                                            hero.Value.SGui.SpellQ.SizeHpBar,
                                            new ColorBGRA(Color3.White, 0.55f),
                                            new[] {1.7f*percentScale, 1.7f*percentScale});
                                    }
                                    if (hero.Value.SGui.SpellW.Cd > 0.0f ||
                                        hero.Key.Spellbook.GetSpell(SpellSlot.W).Level < 1)
                                    {
                                        DirectXDrawer.DrawSprite(S, _overlaySpellItem,
                                            hero.Value.SGui.SpellW.SizeHpBar,
                                            new ColorBGRA(Color3.White, 0.55f),
                                            new[] {1.7f*percentScale, 1.7f*percentScale});
                                    }
                                    if (hero.Value.SGui.SpellE.Cd > 0.0f ||
                                        hero.Key.Spellbook.GetSpell(SpellSlot.E).Level < 1)
                                    {
                                        DirectXDrawer.DrawSprite(S, _overlaySpellItem,
                                            hero.Value.SGui.SpellE.SizeHpBar,
                                            new ColorBGRA(Color3.White, 0.55f),
                                            new[] {1.7f*percentScale, 1.7f*percentScale});
                                    }
                                    if (hero.Value.SGui.SpellR.Cd > 0.0f ||
                                        hero.Key.Spellbook.GetSpell(SpellSlot.R).Level < 1)
                                    {
                                        DirectXDrawer.DrawSprite(S, _overlaySpellItem,
                                            hero.Value.SGui.SpellR.SizeHpBar,
                                            new ColorBGRA(Color3.White, 0.55f),
                                            new[] {1.7f*percentScale, 1.7f*percentScale});
                                    }
                                    if (hero.Value.SGui.SpellSum1.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawSprite(S, _overlaySummonerSpell,
                                            hero.Value.SGui.SpellSum1.SizeHpBar,
                                            new ColorBGRA(Color3.White, 0.55f),
                                            new[] {1.0f*percentScale, 1.0f*percentScale});
                                    }
                                    if (hero.Value.SGui.SpellSum2.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawSprite(S, _overlaySummonerSpell,
                                            hero.Value.SGui.SpellSum2.SizeHpBar,
                                            new ColorBGRA(Color3.White, 0.55f),
                                            new[] {1.0f*percentScale, 1.0f*percentScale});
                                    }
                                }
                            }
                            S.End();
                            foreach (var hero in heroes)
                            {
                                if (!hero.Key.IsDead && hero.Key.IsVisible)
                                {
                                    if (hero.Value.SGui.SpellQ.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SumF, hero.Value.SGui.SpellQ.Cd.ToString(),
                                            hero.Value.SGui.SpellQ.CoordsHpBar, Color.Orange);
                                    }
                                    if (hero.Value.SGui.SpellW.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SumF, hero.Value.SGui.SpellW.Cd.ToString(),
                                            hero.Value.SGui.SpellW.CoordsHpBar, Color.Orange);
                                    }
                                    if (hero.Value.SGui.SpellE.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SumF, hero.Value.SGui.SpellE.Cd.ToString(),
                                            hero.Value.SGui.SpellE.CoordsHpBar, Color.Orange);
                                    }
                                    if (hero.Value.SGui.SpellR.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SumF, hero.Value.SGui.SpellR.Cd.ToString(),
                                            hero.Value.SGui.SpellR.CoordsHpBar, Color.Orange);
                                    }
                                    if (hero.Value.SGui.SpellSum1.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SumF, hero.Value.SGui.SpellSum1.Cd.ToString(),
                                            hero.Value.SGui.SpellSum1.CoordsHpBar, Color.Orange);
                                    }
                                    if (hero.Value.SGui.SpellSum2.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SumF, hero.Value.SGui.SpellSum2.Cd.ToString(),
                                            hero.Value.SGui.SpellSum2.CoordsHpBar, Color.Orange);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (modeHead.SelectedIndex == 0)
                        {
                            S.Begin();
                            foreach (var hero in heroes)
                            {
                                if (!hero.Key.IsDead && hero.Key.IsVisible)
                                {                                                                       
                                    DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellSum1.Texture,
                                        hero.Value.SGui.SpellSum1.SizeHpBar,
                                        new[] { 0.8f * percentScale, 0.8f * percentScale });
                                    DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellSum2.Texture,
                                        hero.Value.SGui.SpellSum2.SizeHpBar,
                                        new[] { 0.8f * percentScale, 0.8f * percentScale });

                                    if (hero.Value.SGui.SpellQ.Cd > 0.0f ||
                                        hero.Key.Spellbook.GetSpell(SpellSlot.Q).Level < 1)
                                    {
                                        RecS.Color = Color.Red;                                        
                                    }
                                    else
                                    {
                                        RecS.Color = Color.Green; 
                                    }
                                    RecS.X = hero.Value.SGui.SpellQ.SizeHpBar.Width;
                                    RecS.Y = hero.Value.SGui.SpellQ.SizeHpBar.Height;
                                    RecS.OnEndScene();
                                    if (hero.Value.SGui.SpellW.Cd > 0.0f ||
                                        hero.Key.Spellbook.GetSpell(SpellSlot.W).Level < 1)
                                    {
                                        RecS.Color = Color.Red;
                                    }
                                    else
                                    {
                                        RecS.Color = Color.Green;
                                    }
                                    RecS.X = hero.Value.SGui.SpellW.SizeHpBar.Width;
                                    RecS.Y = hero.Value.SGui.SpellW.SizeHpBar.Height;
                                    RecS.OnEndScene();
                                    if (hero.Value.SGui.SpellE.Cd > 0.0f ||
                                        hero.Key.Spellbook.GetSpell(SpellSlot.E).Level < 1)
                                    {
                                        RecS.Color = Color.Red;
                                    }
                                    else
                                    {
                                        RecS.Color = Color.Green;
                                    }
                                    RecS.X = hero.Value.SGui.SpellE.SizeHpBar.Width;
                                    RecS.Y = hero.Value.SGui.SpellE.SizeHpBar.Height;
                                    RecS.OnEndScene();
                                    if (hero.Value.SGui.SpellR.Cd > 0.0f ||
                                        hero.Key.Spellbook.GetSpell(SpellSlot.R).Level < 1)
                                    {
                                        RecS.Color = Color.Red;
                                    }
                                    else
                                    {
                                        RecS.Color = Color.Green;
                                    }
                                    RecS.X = hero.Value.SGui.SpellR.SizeHpBar.Width;
                                    RecS.Y = hero.Value.SGui.SpellR.SizeHpBar.Height;
                                    RecS.OnEndScene();
                                    if (hero.Value.SGui.SpellSum1.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawSprite(S, _overlaySummonerSpell,
                                            hero.Value.SGui.SpellSum1.SizeHpBar,
                                            new ColorBGRA(Color3.White, 0.55f),
                                            new[] { 0.8f * percentScale, 0.8f * percentScale });
                                    }
                                    if (hero.Value.SGui.SpellSum2.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawSprite(S, _overlaySummonerSpell,
                                            hero.Value.SGui.SpellSum2.SizeHpBar,
                                            new ColorBGRA(Color3.White, 0.55f),
                                            new[] { 0.8f * percentScale, 0.8f * percentScale });
                                    }
                                }
                            }
                            S.End();
                            foreach (var hero in heroes)
                            {
                                if (!hero.Key.IsDead && hero.Key.IsVisible)
                                {
                                    if (hero.Value.SGui.SpellQ.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SpellF, hero.Value.SGui.SpellQ.Cd.ToString(),
                                            hero.Value.SGui.SpellQ.CoordsHpBar, Color.Orange);
                                    }
                                    else
                                    {
                                        DirectXDrawer.DrawText(SpellF, "Q",
                                            hero.Value.SGui.SpellQ.CoordsHpBar, Color.Orange);
                                    }
                                    if (hero.Value.SGui.SpellW.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SpellF, hero.Value.SGui.SpellW.Cd.ToString(),
                                            hero.Value.SGui.SpellW.CoordsHpBar, Color.Orange);
                                    }
                                    else
                                    {
                                        DirectXDrawer.DrawText(SpellF, "W",
                                            hero.Value.SGui.SpellW.CoordsHpBar, Color.Orange);
                                    }
                                    if (hero.Value.SGui.SpellE.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SpellF, hero.Value.SGui.SpellE.Cd.ToString(),
                                            hero.Value.SGui.SpellE.CoordsHpBar, Color.Orange);
                                    }
                                    else
                                    {
                                        DirectXDrawer.DrawText(SpellF, "E",
                                            hero.Value.SGui.SpellE.CoordsHpBar, Color.Orange);
                                    }
                                    if (hero.Value.SGui.SpellR.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SpellF, hero.Value.SGui.SpellR.Cd.ToString(),
                                            hero.Value.SGui.SpellR.CoordsHpBar, Color.Orange);
                                    }
                                    else
                                    {
                                        DirectXDrawer.DrawText(SpellF, "R",
                                            hero.Value.SGui.SpellR.CoordsHpBar, Color.Orange);
                                    }
                                    if (hero.Value.SGui.SpellSum1.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SumF, hero.Value.SGui.SpellSum1.Cd.ToString(),
                                            hero.Value.SGui.SpellSum1.CoordsHpBar, Color.Orange);
                                    }
                                    if (hero.Value.SGui.SpellSum2.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SumF, hero.Value.SGui.SpellSum2.Cd.ToString(),
                                            hero.Value.SGui.SpellSum2.CoordsHpBar, Color.Orange);
                                    }
                                }
                            }
                        }
                        else
                        {
                            S.Begin();
                            foreach (var hero in heroes)
                            {
                                if (!hero.Key.IsDead && hero.Key.IsVisible)
                                {
                                    DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellSum1.Texture,
                                        hero.Value.SGui.SpellSum1.SizeHpBar,
                                        new[] { 1.0f * percentScale, 1.0f * percentScale });
                                    DirectXDrawer.DrawSprite(S, hero.Value.SGui.SpellSum2.Texture,
                                        hero.Value.SGui.SpellSum2.SizeHpBar,
                                        new[] { 1.0f * percentScale, 1.0f * percentScale });
                                    
                                    if (hero.Value.SGui.SpellQ.Cd > 0.0f ||
                                        hero.Key.Spellbook.GetSpell(SpellSlot.Q).Level < 1)
                                    {
                                        RecB.Color = Color.Red;
                                    }
                                    else
                                    {
                                        RecB.Color = Color.Green;
                                    }
                                    RecB.X = (int)(hero.Value.SGui.SpellQ.SizeHpBar.Width);
                                    RecB.Y = (int)(hero.Value.SGui.SpellQ.SizeHpBar.Height);
                                    RecB.OnEndScene();
                                    if (hero.Value.SGui.SpellW.Cd > 0.0f ||
                                        hero.Key.Spellbook.GetSpell(SpellSlot.W).Level < 1)
                                    {
                                        RecB.Color = Color.Red;
                                    }
                                    else
                                    {
                                        RecB.Color = Color.Green;
                                    }
                                    RecB.X = (int)(hero.Value.SGui.SpellW.SizeHpBar.Width);
                                    RecB.Y = (int)(hero.Value.SGui.SpellW.SizeHpBar.Height);
                                    RecB.OnEndScene();
                                    if (hero.Value.SGui.SpellE.Cd > 0.0f ||
                                        hero.Key.Spellbook.GetSpell(SpellSlot.E).Level < 1)
                                    {
                                        RecB.Color = Color.Red;
                                    }
                                    else
                                    {
                                        RecB.Color = Color.Green;
                                    }
                                    RecB.X = (int)(hero.Value.SGui.SpellE.SizeHpBar.Width);
                                    RecB.Y = (int)(hero.Value.SGui.SpellE.SizeHpBar.Height);
                                    RecB.OnEndScene();
                                    if (hero.Value.SGui.SpellR.Cd > 0.0f ||
                                        hero.Key.Spellbook.GetSpell(SpellSlot.R).Level < 1)
                                    {
                                        RecB.Color = Color.Red;
                                    }
                                    else
                                    {
                                        RecB.Color = Color.Green;
                                    }
                                    RecB.X = (int)(hero.Value.SGui.SpellR.SizeHpBar.Width);
                                    RecB.Y = (int)(hero.Value.SGui.SpellR.SizeHpBar.Height);
                                    RecB.OnEndScene();
                                    
                                }
                            }
                            S.End();
                            foreach (var hero in heroes)
                            {
                                if (!hero.Key.IsDead && hero.Key.IsVisible)
                                {
                                    if (hero.Value.SGui.SpellQ.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SumF, hero.Value.SGui.SpellQ.Cd.ToString(),
                                            hero.Value.SGui.SpellQ.CoordsHpBar, Color.Orange);
                                    }
                                    else
                                    {
                                        DirectXDrawer.DrawText(SumF, "Q",
                                            hero.Value.SGui.SpellQ.CoordsHpBar, Color.Orange);
                                    }
                                    if (hero.Value.SGui.SpellW.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SumF, hero.Value.SGui.SpellW.Cd.ToString(),
                                            hero.Value.SGui.SpellW.CoordsHpBar, Color.Orange);
                                    }
                                    else
                                    {
                                        DirectXDrawer.DrawText(SumF, "W",
                                            hero.Value.SGui.SpellW.CoordsHpBar, Color.Orange);
                                    }
                                    if (hero.Value.SGui.SpellE.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SumF, hero.Value.SGui.SpellE.Cd.ToString(),
                                            hero.Value.SGui.SpellE.CoordsHpBar, Color.Orange);
                                    }
                                    else
                                    {
                                        DirectXDrawer.DrawText(SumF, "E",
                                            hero.Value.SGui.SpellE.CoordsHpBar, Color.Orange);
                                    }
                                    if (hero.Value.SGui.SpellR.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SumF, hero.Value.SGui.SpellR.Cd.ToString(),
                                            hero.Value.SGui.SpellR.CoordsHpBar, Color.Orange);
                                    }
                                    else
                                    {
                                        DirectXDrawer.DrawText(SumF, "R",
                                            hero.Value.SGui.SpellR.CoordsHpBar, Color.Orange);
                                    }
                                    if (hero.Value.SGui.SpellSum1.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SumF, hero.Value.SGui.SpellSum1.Cd.ToString(),
                                            hero.Value.SGui.SpellSum1.CoordsHpBar, Color.Orange);
                                    }
                                    if (hero.Value.SGui.SpellSum2.Cd > 0.0f)
                                    {
                                        DirectXDrawer.DrawText(SumF, hero.Value.SGui.SpellSum2.Cd.ToString(),
                                            hero.Value.SGui.SpellSum2.CoordsHpBar, Color.Orange);
                                    }
                                }
                            }
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (ex.GetType() == typeof(SharpDXException))
                {
                    Menu.UiTracker.SetActive(false);
                    Game.PrintChat("UITracker: An error occured. Please activate CDPanel in your menu again.");
                }
            }
        }

        void Drawing_OnEndScene(EventArgs args)
        {
            if (!IsActive() || !drawActive)
                return;

            if(Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker").GetActive())
                DrawInterface(true);
            if (Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker").GetActive())
                DrawInterface(false);
        }
    }

    public class UIMTracker
    {
        private bool drawActive = true;
        Sprite S;
        Font RecF;
        readonly Dictionary<Obj_AI_Hero, Texture> Enemies = new Dictionary<Obj_AI_Hero, Texture>();

        public UIMTracker()
        {
            if (!IsActive())
                return;
            var loaded = false;
            var tries = 0;
            while (!loaded)
            {
                loaded = Init(tries >= 5);

                tries++;
                if (tries > 9)
                {
                    Console.WriteLine("Couldn't load Interface. It got disabled.");
                    Menu.UimTracker.ForceDisable = true;
                    Menu.UimTracker.Item = null;
                    return;
                }
                Thread.Sleep(10);
            }

            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Drawing.OnEndScene += Drawing_OnEndScene;
            AppDomain.CurrentDomain.DomainUnload += delegate { Drawing_OnPreReset(new EventArgs()); };
            AppDomain.CurrentDomain.ProcessExit += delegate { Drawing_OnPreReset(new EventArgs()); };
        }

        void Drawing_OnPostReset(EventArgs args)
        {
            S.OnResetDevice();
            RecF.OnResetDevice();
            drawActive = true;
        }

        void Drawing_OnPreReset(EventArgs args)
        {
            S.OnLostDevice();
            RecF.OnLostDevice();
            drawActive = false;
        }

        ~UIMTracker()
        {
            Drawing.OnPreReset -= Drawing_OnPreReset;
            Drawing.OnPostReset -= Drawing_OnPostReset;
            Drawing.OnEndScene -= Drawing_OnEndScene;
        }

        public bool IsActive()
        {
            return Menu.Tracker.GetActive() && Menu.UimTracker.GetActive();
        }

        private bool Init(bool force)
        {
            try
            {
                S = new Sprite(Drawing.Direct3DDevice);
                RecF = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Times New Roman", 8));
            }
            catch (Exception)
            {

                return false;
                //throw;
            }

            //var loc = Assembly.GetExecutingAssembly().Location;
            //loc = loc.Remove(loc.LastIndexOf("\\", StringComparison.Ordinal));
            //loc = loc + "\\Sprites\\SAwareness\\";

            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero.IsEnemy)
                {
                    Texture champ = null;
                    //SpriteHelper.LoadTexture(hero.ChampionName + ".dds", "CHAMP/", loc + "CHAMP\\" + hero.ChampionName + ".dds", ref champ);
                    SpriteHelper.LoadTexture(hero.ChampionName, ref champ, SpriteHelper.TextureType.Default);
                    Enemies.Add(hero, champ);
                }
            }

            return true;
        }

        RecallDetector.RecallInfo GetRecall(int networkId)
        {
            StringList t = Menu.RecallDetector.GetMenuItem("SAwarenessRecallDetectorMode").GetValue<StringList>();
            if (t.SelectedIndex == 1 || t.SelectedIndex == 2)
            {
                RecallDetector recallDetector = (RecallDetector)Menu.RecallDetector.Item;
                if (recallDetector == null)
                    return null;
                foreach (var info in recallDetector._recalls)
                {
                    if (info.NetworkId == networkId)
                    {
                        return info;
                    }
                }
            }
            return null;
        }

        void Drawing_OnEndScene(EventArgs args)
        {
            if (!IsActive() || !drawActive)
                return;
            try
            {
                float percentScale = (float)Menu.UimTracker.GetMenuItem("SAwarenessUIMTrackerScale").GetValue<Slider>().Value / 100;

                if (S.IsDisposed)
                {
                    return;
                }
                S.Begin();
                foreach (var enemy in Enemies)
                {
                    if(enemy.Key.IsVisible)
                        continue;
                    Vector2 serverPos = Drawing.WorldToMinimap(enemy.Key.ServerPosition);
                    Size mPos = new Size((int) (serverPos[0] - 32 * 0.3f), (int) (serverPos[1] - 32 * 0.3f));
                    DirectXDrawer.DrawSprite(S, enemy.Value,
                        mPos.ScaleSize(percentScale, new Vector2(mPos.Width, mPos.Height)),
                        new[] { 0.3f * percentScale, 0.3f * percentScale });
                }
                S.End();
                foreach (var enemy in Enemies)
                {
                    if (Menu.RecallDetector.GetActive())
                    {
                        RecallDetector.RecallInfo info = GetRecall(enemy.Key.NetworkId);
                        if (info != null && info.StartTime != 0)
                        {
                            float time = Game.Time + info.Recall.Duration/1000 - info.StartTime;
                            Vector2 vec = Drawing.WorldToMinimap(enemy.Key.ServerPosition);
                            Size pos = new Size((int)vec.X, (int)vec.Y);
                            if (time > 0.0f &&
                                (info.Recall.Status == Packet.S2C.Recall.RecallStatus.TeleportStart ||
                                 info.Recall.Status == Packet.S2C.Recall.RecallStatus.RecallStarted))
                            {
                                DirectXDrawer.DrawText(RecF, enemy.Key.ChampionName, pos, Color.Chartreuse);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (ex.GetType() == typeof(SharpDXException))
                {
                    Menu.UimTracker.SetActive(false);
                    Game.PrintChat("UIM: An error occured. Please activate UI Minimap in your menu again.");
                }
            }
        }
    }

    class WaypointTracker
    {
        public WaypointTracker()
        {
            Drawing.OnDraw += Drawing_OnDraw;
        }

        ~WaypointTracker()
        {
            Drawing.OnDraw -= Drawing_OnDraw;
        }

        public bool IsActive()
        {
            return Menu.Tracker.GetActive() && Menu.WaypointTracker.GetActive();
        }

        void Drawing_OnDraw(EventArgs args)
        {
            if (!IsActive())
                return;
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>())
            {
                float arrivalTime = 0.0f;
                if (enemy.IsValid && enemy.IsVisible && !enemy.IsDead && enemy.IsEnemy)
                {
                    var waypoints = enemy.GetWaypoints();
                    for (int i = 0; i < waypoints.Count - 1; i++)
                    {
                        Vector2 oWp;
                        Vector2 nWp;
                        float time = 0;
                        oWp = Drawing.WorldToScreen(waypoints[i].To3D());
                        nWp = Drawing.WorldToScreen(waypoints[i + 1].To3D());
                        Drawing.DrawLine(oWp[0], oWp[1], nWp[0], nWp[1], 1, System.Drawing.Color.White);
                        time =
                                ((Vector3.Distance(waypoints[i].To3D(), waypoints[i + 1].To3D()) /
                                  (ObjectManager.Player.MoveSpeed / 1000)) / 1000);
                        time = (float)Math.Round(time, 2);
                        arrivalTime += time;
                        if (i == enemy.Path.Length - 1)
                        {
                            DrawCross(nWp[0], nWp[1], 1.0f, 3.0f, System.Drawing.Color.Red);
                            Drawing.DrawText(nWp[0] - 15, nWp[1] + 10, System.Drawing.Color.Red, arrivalTime.ToString());
                        }
                    }
                }
            }
        }

        void DrawCross(float x, float y, float size, float thickness, System.Drawing.Color color)
        {
            var topLeft = new Vector2(x - 10 * size, y - 10 * size);
            var topRight = new Vector2(x + 10 * size, y - 10 * size);
            var botLeft = new Vector2(x - 10 * size, y + 10 * size);
            var botRight = new Vector2(x + 10 * size, y + 10 * size);

            Drawing.DrawLine(topLeft.X, topLeft.Y, botRight.X, botRight.Y, thickness, color);
            Drawing.DrawLine(topRight.X, topRight.Y, botLeft.X, botLeft.Y, thickness, color);
        }
    }
}
