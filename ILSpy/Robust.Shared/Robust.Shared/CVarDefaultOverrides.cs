using Robust.Shared.Configuration;

namespace Robust.Shared;

internal static class CVarDefaultOverrides
{
	public static void OverrideClient(IConfigurationManager cfg)
	{
		OverrideShared(cfg);
	}

	public static void OverrideServer(IConfigurationManager cfg)
	{
		OverrideShared(cfg);
	}

	private static void OverrideShared(IConfigurationManager cfg)
	{
	}
}
