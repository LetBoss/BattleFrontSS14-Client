using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Content.Shared.Administration;

public static class AdminFlagsHelper
{
	private static readonly Dictionary<string, AdminFlags> NameFlagsMap;

	private static readonly string[] FlagsNameMap;

	public static readonly AdminFlags Everything;

	public static readonly IReadOnlyList<AdminFlags> AllFlags;

	static AdminFlagsHelper()
	{
		NameFlagsMap = new Dictionary<string, AdminFlags>();
		FlagsNameMap = new string[32];
		AdminFlags[] obj = (AdminFlags[])Enum.GetValues(typeof(AdminFlags));
		List<AdminFlags> allFlags = new List<AdminFlags>();
		AdminFlags[] array = obj;
		for (int i = 0; i < array.Length; i++)
		{
			AdminFlags value = array[i];
			string name = value.ToString().ToUpper();
			if (BitOperations.PopCount((uint)value) == 1)
			{
				allFlags.Add(value);
				Everything |= value;
				NameFlagsMap.Add(name, value);
				FlagsNameMap[BitOperations.Log2((uint)value)] = name;
			}
		}
		AllFlags = allFlags.ToArray();
	}

	public static AdminFlags NamesToFlags(IEnumerable<string> names)
	{
		AdminFlags flags = AdminFlags.None;
		foreach (string name in names)
		{
			if (!NameFlagsMap.TryGetValue(name, out var value))
			{
				throw new ArgumentException("Invalid admin flag name: " + name);
			}
			flags |= value;
		}
		return flags;
	}

	public static AdminFlags NameToFlag(string name)
	{
		return NameFlagsMap[name];
	}

	public static string[] FlagsToNames(AdminFlags flags)
	{
		string[] array = new string[BitOperations.PopCount((uint)flags)];
		int highest = BitOperations.LeadingZeroCount((uint)flags);
		int ai = 0;
		for (int i = 0; i < 32 - highest; i++)
		{
			AdminFlags flagValue = (AdminFlags)(1 << i);
			if ((flags & flagValue) != AdminFlags.None)
			{
				array[ai++] = FlagsNameMap[i];
			}
		}
		return array;
	}

	public static string PosNegFlagsText(AdminFlags posFlags, AdminFlags negFlags)
	{
		IEnumerable<(string, string)> posFlagNames = from f in FlagsToNames(posFlags)
			select (flag: f, fText: "+" + f);
		IEnumerable<(string, string)> negFlagNames = from f in FlagsToNames(negFlags)
			select (flag: f, fText: "-" + f);
		return string.Join(' ', from p in posFlagNames.Concat(negFlagNames)
			orderby p.flag
			select p.fText);
	}
}
