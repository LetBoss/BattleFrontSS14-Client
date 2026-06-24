using System;

namespace Content.Shared._RMC14.Vehicle;

public static class VehicleTurretSlotIds
{
	public const string Separator = "::";

	public static string Compose(string parentSlotId, string childSlotId)
	{
		return parentSlotId + "::" + childSlotId;
	}

	public static bool TryParse(string slotId, out string parentSlotId, out string childSlotId)
	{
		parentSlotId = string.Empty;
		childSlotId = string.Empty;
		if (string.IsNullOrWhiteSpace(slotId))
		{
			return false;
		}
		int index = slotId.IndexOf("::", StringComparison.Ordinal);
		if (index <= 0 || index >= slotId.Length - "::".Length)
		{
			return false;
		}
		parentSlotId = slotId.Substring(0, index);
		int num = index + "::".Length;
		childSlotId = slotId.Substring(num, slotId.Length - num);
		if (!string.IsNullOrWhiteSpace(parentSlotId))
		{
			return !string.IsNullOrWhiteSpace(childSlotId);
		}
		return false;
	}
}
