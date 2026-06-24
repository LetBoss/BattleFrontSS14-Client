using System;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Chemistry.Reagent;

public static class PrototypeManagerExtensions
{
	[Obsolete("Use ReagentSystem")]
	public static Reagent IndexReagent(this IPrototypeManager prototypes, ProtoId<ReagentPrototype> id)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return IoCManager.Resolve<IEntityManager>().System<RMCReagentSystem>().Index(id);
	}

	[Obsolete("Use ReagentSystem")]
	public static Reagent IndexReagent<T>(this IPrototypeManager prototypes, ProtoId<ReagentPrototype> id)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return IoCManager.Resolve<IEntityManager>().System<RMCReagentSystem>().Index(id);
	}

	[Obsolete("Use ReagentSystem")]
	public static bool TryIndexReagent(this IPrototypeManager prototypes, ProtoId<ReagentPrototype> id, [NotNullWhen(true)] out ReagentPrototype? reagentProto)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (!IoCManager.Resolve<IEntityManager>().System<RMCReagentSystem>().TryIndex(id, out Reagent reagent))
		{
			reagentProto = null;
			return false;
		}
		reagentProto = reagent;
		return true;
	}

	[Obsolete("Use ReagentSystem")]
	public static bool TryIndexReagent<T>(this IPrototypeManager prototypes, ProtoId<ReagentPrototype> id, [NotNullWhen(true)] out ReagentPrototype? reagentProto)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (!IoCManager.Resolve<IEntityManager>().System<RMCReagentSystem>().TryIndex(id, out Reagent reagent))
		{
			reagentProto = null;
			return false;
		}
		reagentProto = reagent;
		return true;
	}
}
