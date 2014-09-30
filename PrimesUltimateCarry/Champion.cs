using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Primes_Ultimate_Carry
{
	class Champion
	{
		public Spell Q;
		public Spell W;
		public Spell E;
		public Spell R;
		public Menu ChampionMenu;

		public Champion()
		{
			Chat.Print(PUC.Player.ChampionName  + " Plugin Loading ...");
			MenuBasics();
		}

		public void PluginLoaded()
		{
		Chat.Print(PUC.Player.ChampionName  + " Plugin Loadet!");	
		}
		private void MenuBasics()
		{
			ChampionMenu = new Menu("Primes " + PUC.Player.ChampionName, "Primes_Champion_" + PUC.Player.ChampionName);
			
			ChampionMenu.AddSubMenu(new Menu("Packet Setting", "Primes_Champion_Packets"));
			ChampionMenu.SubMenu("Primes_Champion_Packets").AddItem(new MenuItem("Primes_Champion_Packets_sep0", "===== Settings"));
			ChampionMenu.SubMenu("Primes_Champion_Packets").AddItem(new MenuItem("Primes_Champion_Packets_active", "= Use Packets").SetValue(true));
			ChampionMenu.SubMenu("Primes_Champion_Packets").AddItem(new MenuItem("Primes_Champion_Packets_sep1", "========="));

		}

		public MenuItem GetMenuItem(string name, string displayName)
		{
			return new MenuItem("Primes_Champion_Control_" + name, "= " + displayName);
		}

		public static bool IsInsideEnemyTower(Vector3 position)
		{
			return ObjectManager.Get<Obj_AI_Turret>()
									.Any(tower => tower.IsEnemy && tower.Health > 0 && tower.Position.Distance(position) < 775);
		}

		public Obj_AI_Hero Cast_BasicSkillshot_Enemy(Spell spell, TargetSelector.PriorityMode prio = TargetSelector.PriorityMode.AutoPriority, float extrarange = 0)
		{
			if(!spell.IsReady())
				return null;
			var target = TargetSelector.GetTarget(spell.Range, prio);
			if(target == null)
				return null;
			if (!target.IsValidTarget(spell.Range + extrarange) || spell.GetPrediction(target).Hitchance < HitChance.High)
				return null;
			spell.Cast(target, UsePackets());
			return target;
		}

		public void Cast_BasicSkillshot_AOE_Farm(Spell spell, int extrawidth = 0)
		{
			if(!spell.IsReady() )
				return;
			var minions = MinionManager.GetMinions(ObjectManager.Player.Position, spell.Type == SkillshotType.SkillshotLine ? spell.Range : spell.Range + ((spell.Width + extrawidth) / 2),MinionTypes.All , MinionTeam.NotAlly);
			if(minions.Count == 0)
				return;
			var castPostion = new MinionManager.FarmLocation();
			
			if(spell.Type == SkillshotType.SkillshotCircle)
				castPostion = MinionManager.GetBestCircularFarmLocation(minions.Select(minion => minion.ServerPosition.To2D()).ToList(), spell.Width + extrawidth, spell.Range);
			if(spell.Type == SkillshotType.SkillshotLine)
				castPostion = MinionManager.GetBestLineFarmLocation(minions.Select(minion => minion.ServerPosition.To2D()).ToList(), spell.Width, spell.Range);
			spell.Cast(castPostion.Position, UsePackets());
		}

		public bool UsePackets()
		{
			return ChampionMenu.Item("Primes_Champion_Packets_active").GetValue<bool>();
		}

		public string GetSpellName(SpellSlot slot, Obj_AI_Hero unit = null)
		{
			return unit != null ? unit.Spellbook.GetSpell(slot).Name : PUC.Player.Spellbook.GetSpell(slot).Name;
		}

		public bool EnemysinRange(float range ,int min = 1, Obj_AI_Hero unit = null)
		{
			if (unit == null)
				unit = PUC.Player;
			return min <= PUC.AllHerosEnemy.Count(hero => hero.Distance(unit) < range && hero.IsValidTarget());
		}
		public bool EnemysinRange(float range, int min , Vector3  pos)
		{
			return min <= PUC.AllHerosEnemy.Count(hero => hero.Position.Distance(pos) < range && hero.IsValidTarget());
		}

		public void AddManaManager(Menu menu,string name,int basic)
		{
			menu.AddItem(new MenuItem(name, "= Mana-Manager").SetValue(new Slider(basic, 100, 0)));
		}

		public bool ManamanagerAllowCast(string name)
		{
			return PUC.Menu.Item(name).GetValue<Slider>().Value < PUC.Player.Mana/PUC.Player.MaxMana*100;
		}

		public bool EnoughManaFor(SpellSlot spell, SpellSlot spell2 = SpellSlot.Unknown, SpellSlot spell3 = SpellSlot.Unknown, SpellSlot spell4 = SpellSlot.Unknown)
		{
			var cost1 = PUC.Player.Spellbook.GetSpell(spell).ManaCost;
			var cost2 = 0f;
			var cost3 = 0f;
			var cost4 = 0f;
			if(spell2 != SpellSlot.Unknown)
				cost2 = PUC.Player.Spellbook.GetSpell(spell2).ManaCost;
			if(spell3 != SpellSlot.Unknown)
				cost3 = PUC.Player.Spellbook.GetSpell(spell3).ManaCost;
			if(spell4 != SpellSlot.Unknown)
				cost4 = PUC.Player.Spellbook.GetSpell(spell4).ManaCost;

			return cost1 + cost2 + cost3 + cost4 <= PUC.Player.Mana;
		}

		

	}
}
