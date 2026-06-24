using System;
using Content.Shared.DeviceNetwork.Components;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Shared.DeviceNetwork;

public static class DeviceNetworkConstants
{
	public const string LogicState = "logic_state";

	public const string Command = "command";

	public const string CmdSetState = "set_state";

	public const string CmdUpdatedState = "updated_state";

	public const string StateEnabled = "state_enabled";

	public static string FrequencyToString(this uint frequency)
	{
		string result = frequency.ToString();
		if (result.Length <= 2)
		{
			return result + ".0";
		}
		return result.Insert(result.Length - 1, ".");
	}

	public static string DeviceNetIdToLocalizedName(this int id)
	{
		if (!Enum.IsDefined(typeof(DeviceNetworkComponent.DeviceNetIdDefaults), id))
		{
			return id.ToString();
		}
		DeviceNetworkComponent.DeviceNetIdDefaults deviceNetIdDefaults = (DeviceNetworkComponent.DeviceNetIdDefaults)id;
		string result = deviceNetIdDefaults.ToString();
		string resultKebab = "device-net-id-" + CaseConversion.PascalToKebab(result);
		string name = default(string);
		if (IoCManager.Resolve<ILocalizationManager>().TryGetString(resultKebab, ref name))
		{
			return name;
		}
		return result;
	}
}
