using System;
using Content.Shared.Database;
using Robust.Shared.Configuration;
using Robust.Shared.Player;

namespace Content.Shared.Players.RateLimiting;

public sealed class RateLimitRegistration(CVarDef<float> cVarLimitPeriodLength, CVarDef<int> cVarLimitCount, Action<ICommonSession>? playerLimitedAction, CVarDef<int>? cVarAdminAnnounceDelay = null, Action<ICommonSession>? adminAnnounceAction = null, LogType adminLogType = LogType.RateLimited)
{
	public readonly CVarDef<float> CVarLimitPeriodLength = cVarLimitPeriodLength;

	public readonly CVarDef<int> CVarLimitCount = cVarLimitCount;

	public readonly Action<ICommonSession>? PlayerLimitedAction = playerLimitedAction;

	public readonly CVarDef<int>? CVarAdminAnnounceDelay = cVarAdminAnnounceDelay;

	public readonly Action<ICommonSession>? AdminAnnounceAction = adminAnnounceAction;

	public readonly LogType AdminLogType = adminLogType;
}
