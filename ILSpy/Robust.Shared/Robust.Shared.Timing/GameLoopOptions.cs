using Robust.Shared.Configuration;

namespace Robust.Shared.Timing;

internal sealed record GameLoopOptions(bool Precise)
{
	public static GameLoopOptions FromCVars(IConfigurationManager cfg)
	{
		return new GameLoopOptions(cfg.GetCVar(CVars.SysPreciseSleep));
	}
}
