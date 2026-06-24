using Content.Shared._PUBG.BattlePass;
using Robust.Shared.Localization;

namespace Content.Client._PUBG.BattlePass;

public static class PubgBattlePassTaskFormatter
{
	public static string GetTaskDisplayText(BattlePassTaskInfo task)
	{
		return task.TaskType switch
		{
			"damage" => Loc.GetString("pubg-bp-task-damage", new(string, object)[1] { ("value", task.TargetValue) }), 
			"damage_weapon" => Loc.GetString("pubg-bp-task-damage-weapon", new(string, object)[2]
			{
				("value", task.TargetValue),
				("weapon", task.WeaponId ?? "?")
			}), 
			"kills" => Loc.GetString("pubg-bp-task-kills", new(string, object)[1] { ("value", task.TargetValue) }), 
			"healing_bandage" => Loc.GetString("pubg-bp-task-healing-bandage", new(string, object)[1] { ("value", task.TargetValue) }), 
			"healing_medkit" => Loc.GetString("pubg-bp-task-healing-medkit", new(string, object)[1] { ("value", task.TargetValue) }), 
			"healing_full" => Loc.GetString("pubg-bp-task-healing-full", new(string, object)[1] { ("value", task.TargetValue) }), 
			"energy_drink" => Loc.GetString("pubg-bp-task-energy-drink", new(string, object)[1] { ("value", task.TargetValue) }), 
			"revive_ally" => Loc.GetString("pubg-bp-task-revive-ally", new(string, object)[1] { ("value", task.TargetValue) }), 
			"placement" => Loc.GetString("pubg-bp-task-placement", new(string, object)[2]
			{
				("value", ExtractPlacementTarget(task.NameKey)),
				("count", task.TargetValue)
			}), 
			"survival" => Loc.GetString("pubg-bp-task-survival", new(string, object)[1] { ("value", task.TargetValue) }), 
			_ => task.NameKey, 
		};
	}

	private static int ExtractPlacementTarget(string nameKey)
	{
		int num = 0;
		bool flag = false;
		foreach (char c in nameKey)
		{
			if (c >= '0' && c <= '9')
			{
				flag = true;
				num = num * 10 + (c - 48);
			}
			else if (flag)
			{
				break;
			}
		}
		if (!flag)
		{
			return 10;
		}
		return num;
	}
}
