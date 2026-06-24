using System;
using System.Collections.Generic;
using Robust.Shared.Player;

namespace Content.Shared.Players.PlayTimeTracking;

public interface ISharedPlaytimeManager
{
	IReadOnlyDictionary<string, TimeSpan> GetPlayTimes(ICommonSession session);
}
