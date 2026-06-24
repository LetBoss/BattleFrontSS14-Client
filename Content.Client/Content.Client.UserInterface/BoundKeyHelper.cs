using System.Diagnostics.CodeAnalysis;
using Robust.Client.Input;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface;

public static class BoundKeyHelper
{
	public static string ShortKeyName(BoundKeyFunction keyFunction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetShortKeyName(keyFunction, out string name))
		{
			return " ";
		}
		string result = default(string);
		if (!IoCManager.Resolve<ILocalizationManager>().TryGetString(name, ref result))
		{
			return name;
		}
		return result;
	}

	public static bool IsBound(BoundKeyFunction keyFunction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		string name;
		return TryGetShortKeyName(keyFunction, out name);
	}

	private static string? DefaultShortKeyName(BoundKeyFunction keyFunction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(keyFunction.FunctionName))
		{
			return null;
		}
		IDependencyCollection instance = IoCManager.Instance;
		IInputManager val = default(IInputManager);
		if (instance == null || !instance.TryResolveType<IInputManager>(ref val))
		{
			return null;
		}
		string text = FormattedMessage.EscapeText(val.GetKeyFunctionButtonString(keyFunction));
		if (text.Length <= 3)
		{
			return text;
		}
		return null;
	}

	public static bool TryGetShortKeyName(BoundKeyFunction keyFunction, [NotNullWhen(true)] out string? name)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Expected I4, but got Unknown
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(keyFunction.FunctionName))
		{
			name = null;
			return false;
		}
		IDependencyCollection instance = IoCManager.Instance;
		IInputManager val = default(IInputManager);
		if (instance == null || !instance.TryResolveType<IInputManager>(ref val))
		{
			name = null;
			return false;
		}
		IKeyBinding val2 = default(IKeyBinding);
		if (val.TryGetKeyBinding(keyFunction, ref val2))
		{
			Key baseKey = val2.BaseKey;
			if ((int)val2.Mod1 != 0 || (int)val2.Mod2 != 0 || (int)val2.Mod3 != 0)
			{
				name = null;
				return false;
			}
			name = null;
			name = (baseKey - 1) switch
			{
				67 => "'", 
				65 => ",", 
				82 => "Del", 
				92 => "Dwn", 
				55 => "Esc", 
				71 => "=", 
				80 => "Hom", 
				81 => "Ins", 
				89 => "Lft", 
				61 => "Men", 
				83 => "-", 
				35 => "0", 
				36 => "1", 
				37 => "2", 
				38 => "3", 
				39 => "4", 
				40 => "5", 
				41 => "6", 
				42 => "7", 
				43 => "8", 
				44 => "9", 
				117 => "||", 
				66 => ".", 
				73 => "Ret", 
				90 => "Rgt", 
				68 => "/", 
				72 => "Spc", 
				76 => "Tab", 
				70 => "~", 
				69 => "\\", 
				75 => "Bks", 
				62 => "[", 
				3 => "M4", 
				4 => "M5", 
				5 => "M6", 
				6 => "M7", 
				7 => "M8", 
				8 => "M9", 
				0 => "ML", 
				2 => "MM", 
				1 => "MR", 
				88 => "N.", 
				86 => "N/", 
				74 => "Ent", 
				87 => "*", 
				45 => "0", 
				46 => "1", 
				47 => "2", 
				48 => "3", 
				49 => "4", 
				50 => "5", 
				51 => "6", 
				52 => "7", 
				53 => "8", 
				54 => "9", 
				85 => "N-", 
				78 => "PgD", 
				77 => "PgU", 
				63 => "]", 
				64 => ";", 
				_ => DefaultShortKeyName(keyFunction), 
			};
			return name != null;
		}
		name = null;
		return false;
	}
}
