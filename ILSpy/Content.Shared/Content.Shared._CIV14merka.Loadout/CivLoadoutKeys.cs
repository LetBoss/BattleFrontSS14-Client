using Content.Shared._CIV14merka.Teams;

namespace Content.Shared._CIV14merka.Loadout;

public static class CivLoadoutKeys
{
	public const string BonusSlot = "bonus";

	public static string Item(string slot, string proto)
	{
		return slot + ":" + proto;
	}

	public static string Combo(string faction, CivTdmClass cls)
	{
		return faction + ":" + cls;
	}

	public static bool TryParseItem(string key, out string slot, out string proto)
	{
		int idx = key.IndexOf(':');
		if (idx <= 0 || idx >= key.Length - 1)
		{
			slot = string.Empty;
			proto = string.Empty;
			return false;
		}
		slot = key.Substring(0, idx);
		proto = key.Substring(idx + 1);
		return true;
	}

	public static string SlotChoice(string slot, string proto)
	{
		return slot + "=" + proto;
	}

	public static bool TryParseSlotChoice(string entry, out string slot, out string proto)
	{
		int idx = entry.IndexOf('=');
		if (idx <= 0 || idx >= entry.Length - 1)
		{
			slot = string.Empty;
			proto = string.Empty;
			return false;
		}
		slot = entry.Substring(0, idx);
		proto = entry.Substring(idx + 1);
		return true;
	}

	public static string Owned(string faction, int count, string proto)
	{
		return $"{faction}|{count}|{proto}";
	}

	public static bool TryParseOwned(string entry, out string faction, out int count, out string proto)
	{
		faction = string.Empty;
		count = 0;
		proto = string.Empty;
		int first = entry.IndexOf('|');
		if (first < 0)
		{
			proto = entry;
			return !string.IsNullOrEmpty(proto);
		}
		int second = entry.IndexOf('|', first + 1);
		if (second < 0)
		{
			faction = entry.Substring(0, first);
			proto = entry.Substring(first + 1);
			return !string.IsNullOrEmpty(proto);
		}
		faction = entry.Substring(0, first);
		int.TryParse(entry.Substring(first + 1, second - first - 1), out count);
		proto = entry.Substring(second + 1);
		return !string.IsNullOrEmpty(proto);
	}
}
