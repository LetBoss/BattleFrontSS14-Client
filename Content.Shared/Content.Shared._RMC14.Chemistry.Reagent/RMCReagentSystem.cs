using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Chemistry.Reagent;

public sealed class RMCReagentSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private ISerializationManager _serialization;

	private FrozenDictionary<string, Reagent> _reagents = FrozenDictionary<string, Reagent>.Empty;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<PrototypesReloadedEventArgs>((EntityEventHandler<PrototypesReloadedEventArgs>)OnPrototypesReloaded, (Type[])null, (Type[])null);
		ReloadPrototypes();
	}

	private void OnPrototypesReloaded(PrototypesReloadedEventArgs ev)
	{
		if (ev.WasModified<ReagentPrototype>())
		{
			ReloadPrototypes();
		}
	}

	private void ReloadPrototypes()
	{
		Dictionary<string, Reagent> dict = new Dictionary<string, Reagent>();
		foreach (ReagentPrototype reagentProto in _prototypes.EnumeratePrototypes<ReagentPrototype>())
		{
			object reagentObj = new Reagent();
			_serialization.CopyTo((object)reagentProto, ref reagentObj, (ISerializationContext)null, false, false);
			if (reagentObj is Reagent reagent)
			{
				dict[reagentProto.ID] = reagent;
			}
		}
		_reagents = dict.ToFrozenDictionary();
	}

	public Reagent Index(ProtoId<ReagentPrototype> id)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _reagents[ProtoId<ReagentPrototype>.op_Implicit(id)];
	}

	public bool TryIndex(ProtoId<ReagentPrototype> id, [NotNullWhen(true)] out Reagent? reagent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _reagents.TryGetValue(ProtoId<ReagentPrototype>.op_Implicit(id), out reagent);
	}

	public bool TryIndex(ReagentId id, [NotNullWhen(true)] out Reagent? reagent)
	{
		return _reagents.TryGetValue(id.Prototype, out reagent);
	}
}
