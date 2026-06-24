using System;

namespace Content.Shared.Atmos;

public sealed class AtmosCommandUtils
{
	public static bool TryParseGasID(string str, out int x)
	{
		x = -1;
		if (Enum.TryParse<Gas>(str, ignoreCase: true, out var gas))
		{
			x = (int)gas;
		}
		else if (!int.TryParse(str, out x))
		{
			return false;
		}
		if (x >= 0)
		{
			return x < 9;
		}
		return false;
	}
}
