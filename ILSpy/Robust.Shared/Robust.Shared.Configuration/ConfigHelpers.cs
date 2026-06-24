namespace Robust.Shared.Configuration;

internal static class ConfigHelpers
{
	public static int GetEffectiveMaxConnections(this IConfigurationManager cfg)
	{
		int cVar = cfg.GetCVar(CVars.GameMaxPlayers);
		if (cVar != 0)
		{
			return cVar;
		}
		return cfg.GetCVar(CVars.NetMaxConnections);
	}
}
