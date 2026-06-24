using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.Console;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Robust.Shared.Configuration;

public static class CVarCommandUtil
{
	public static object ParseObject(Type type, string input)
	{
		if (type == typeof(bool))
		{
			if (bool.TryParse(input, out var result))
			{
				return result;
			}
			if (Parse.TryInt32(input.AsSpan(), out var result2))
			{
				switch (result2)
				{
				case 0:
					return false;
				case 1:
					return true;
				}
			}
			throw new FormatException("Could not parse bool value: " + input);
		}
		if (type == typeof(string))
		{
			return input;
		}
		if (type == typeof(int))
		{
			return Parse.Int32(input.AsSpan());
		}
		if (type == typeof(float))
		{
			return Parse.Float(input.AsSpan());
		}
		if (type == typeof(long))
		{
			return long.Parse(input);
		}
		if (type == typeof(ushort))
		{
			return ushort.Parse(input);
		}
		throw new NotSupportedException();
	}

	internal static IEnumerable<CompletionOption> GetCVarCompletionOptions(IConfigurationManager cfg)
	{
		return from c in cfg.GetRegisteredCVars()
			select new CompletionOption(c, GetCVarValueHint(cfg, c));
	}

	private static string GetCVarValueHint(IConfigurationManager cfg, string cVar)
	{
		if ((cfg.GetCVarFlags(cVar) & CVar.CONFIDENTIAL) != CVar.NONE)
		{
			return Loc.GetString("cmd-cvar-value-hidden");
		}
		string text = cfg.GetCVar<object>(cVar).ToString() ?? "";
		if (text.Length > 50)
		{
			text = text.Substring(0, 51) + "…";
		}
		return text;
	}
}
