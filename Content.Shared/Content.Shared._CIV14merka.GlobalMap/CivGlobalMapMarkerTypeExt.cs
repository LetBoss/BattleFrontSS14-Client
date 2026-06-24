namespace Content.Shared._CIV14merka.GlobalMap;

public static class CivGlobalMapMarkerTypeExt
{
	public static bool IsGlobal(this CivGlobalMapMarkerType type)
	{
		if (type <= CivGlobalMapMarkerType.Defense)
		{
			return true;
		}
		return false;
	}
}
