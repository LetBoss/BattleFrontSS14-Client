using System;
using System.Collections.Generic;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Chemistry.EntitySystems;

public sealed class RehydratableSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedSolutionContainerSystem _solutions;

	[Dependency]
	private SharedTransformSystem _xform;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RehydratableComponent, SolutionContainerChangedEvent>((EntityEventRefHandler<RehydratableComponent, SolutionContainerChangedEvent>)OnSolutionChange, (Type[])null, (Type[])null);
	}

	private void OnSolutionChange(Entity<RehydratableComponent> ent, ref SolutionContainerChangedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		FixedPoint2 quantity = _solutions.GetTotalPrototypeQuantity(Entity<RehydratableComponent>.op_Implicit(ent), ProtoId<ReagentPrototype>.op_Implicit(ent.Comp.CatalystPrototype));
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(44, 2);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(ent.Owner)), "ToPrettyString(ent.Owner)");
		handler.AppendLiteral(" was hydrated, now contains a solution of: ");
		handler.AppendFormatted(SharedSolutionContainerSystem.ToPrettyString(args.Solution));
		handler.AppendLiteral(".");
		adminLogger.Add(LogType.Action, LogImpact.Medium, ref handler);
		if (quantity != FixedPoint2.Zero && quantity >= ent.Comp.CatalystMinimum)
		{
			Expand(ent);
		}
	}

	private void Expand(Entity<RehydratableComponent> ent)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			Entity<RehydratableComponent> val = ent;
			EntityUid val2 = default(EntityUid);
			RehydratableComponent rehydratableComponent = default(RehydratableComponent);
			val.Deconstruct(ref val2, ref rehydratableComponent);
			EntityUid uid = val2;
			RehydratableComponent comp = rehydratableComponent;
			EntProtoId randomMob = RandomExtensions.Pick<EntProtoId>(_random, (IReadOnlyList<EntProtoId>)comp.PossibleSpawns);
			EntityUid target = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(randomMob), ((EntitySystem)this).Transform(uid).Coordinates);
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(43, 2);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(ent.Owner)), "ToPrettyString(ent.Owner)");
			handler.AppendLiteral(" has been hydrated correctly and spawned: ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "ToPrettyString(target)");
			handler.AppendLiteral(".");
			adminLogger.Add(LogType.Action, LogImpact.Medium, ref handler);
			_popup.PopupEntity(base.Loc.GetString("rehydratable-component-expands-message", (ValueTuple<string, object>)("owner", uid)), target);
			_xform.AttachToGridOrMap(target, (TransformComponent)null);
			GotRehydratedEvent ev = new GotRehydratedEvent(target);
			((EntitySystem)this).RaiseLocalEvent<GotRehydratedEvent>(uid, ref ev, false);
			((EntitySystem)this).RemComp<RehydratableComponent>(uid);
			((EntitySystem)this).QueueDel((EntityUid?)uid);
		}
	}
}
