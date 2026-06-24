using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Robust.Shared.Toolshed.Commands.Entities;

[ToolshedCommand]
internal sealed class NearbyCommand : ToolshedCommand
{
	[Dependency]
	private readonly IConfigurationManager _cfg;

	private EntityLookupSystem? _lookup;

	[CommandImplementation(null)]
	public IEnumerable<EntityUid> Nearby([PipedArgument] IEnumerable<EntityUid> input, float range)
	{
		int cVar = _cfg.GetCVar(CVars.ToolshedNearbyLimit);
		if (range > (float)cVar)
		{
			throw new ArgumentException($"Tried to query too big of a range with nearby ({range})! Limit: {cVar}. Change the {CVars.ToolshedNearbyLimit.Name} cvar to increase this at your own risk.");
		}
		if (_lookup == null)
		{
			_lookup = GetSys<EntityLookupSystem>();
		}
		int i = 0;
		int entitiesLimit = _cfg.GetCVar(CVars.ToolshedNearbyEntitiesLimit);
		return input.SelectMany(delegate(EntityUid x)
		{
			if (i++ > entitiesLimit)
			{
				throw new ArgumentException($"Too many entities were passed to nearby ({i})! Limit: {entitiesLimit}. Change the {CVars.ToolshedNearbyEntitiesLimit.Name} cvar to increase this at your own risk.");
			}
			return _lookup.GetEntitiesInRange(x, range);
		}).Distinct();
	}
}
