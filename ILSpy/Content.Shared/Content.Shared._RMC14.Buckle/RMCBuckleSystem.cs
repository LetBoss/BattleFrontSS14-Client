using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.CrashLand;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Buckle;
using Content.Shared.Buckle.Components;
using Content.Shared.Interaction;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Shuttles.Components;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Events;

namespace Content.Shared._RMC14.Buckle;

public sealed class RMCBuckleSystem : EntitySystem
{
	[Dependency]
	private SharedBuckleSystem _buckle;

	[Dependency]
	private SharedCrashLandSystem _crashLand;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private EntityWhitelistSystem _entityWhitelist;

	[Dependency]
	private SharedPopupSystem _popup;

	private readonly HashSet<EntityUid> _intersecting = new HashSet<EntityUid>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<BuckleClimbableComponent, StrappedEvent>((EntityEventRefHandler<BuckleClimbableComponent, StrappedEvent>)OnBuckleClimbableStrapped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveBuckleClimbingComponent, PreventCollideEvent>((EntityEventRefHandler<ActiveBuckleClimbingComponent, PreventCollideEvent>)OnBuckleClimbablePreventCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BuckleWhitelistComponent, BuckleAttemptEvent>((EntityEventRefHandler<BuckleWhitelistComponent, BuckleAttemptEvent>)OnBuckleWhitelistAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, AttemptMobTargetCollideEvent>((EntityEventRefHandler<BuckleComponent, AttemptMobTargetCollideEvent>)OnBuckleAttemptMobTargetCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrapComponent, EntParentChangedMessage>((EntityEventRefHandler<StrapComponent, EntParentChangedMessage>)OnBuckleParentChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrapComponent, CombatModeShouldHandInteractEvent>((EntityEventRefHandler<StrapComponent, CombatModeShouldHandInteractEvent>)OnStrapCombatModeShouldHandInteract, (Type[])null, (Type[])null);
	}

	private void OnBuckleClimbableStrapped(Entity<BuckleClimbableComponent> ent, ref StrappedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		ActiveBuckleClimbingComponent active = ((EntitySystem)this).EnsureComp<ActiveBuckleClimbingComponent>(Entity<BuckleComponent>.op_Implicit(args.Buckle));
		active.Strap = Entity<BuckleClimbableComponent>.op_Implicit(ent);
		((EntitySystem)this).Dirty(Entity<BuckleComponent>.op_Implicit(args.Buckle), (IComponent)(object)active, (MetaDataComponent)null);
	}

	private void OnBuckleClimbablePreventCollide(Entity<ActiveBuckleClimbingComponent> ent, ref PreventCollideEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			EntityUid? strap = ent.Comp.Strap;
			EntityUid otherEntity = args.OtherEntity;
			if (strap.HasValue && strap.GetValueOrDefault() == otherEntity)
			{
				args.Cancelled = true;
			}
		}
	}

	private void OnBuckleWhitelistAttempt(Entity<BuckleWhitelistComponent> ent, ref BuckleAttemptEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!_entityWhitelist.IsWhitelistPassOrNull(ent.Comp.Whitelist, Entity<StrapComponent>.op_Implicit(args.Strap)))
		{
			args.Cancelled = true;
		}
	}

	private void OnBuckleAttemptMobTargetCollide(Entity<BuckleComponent> ent, ref AttemptMobTargetCollideEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && ent.Comp.Buckled)
		{
			args.Cancelled = true;
		}
	}

	private void OnBuckleParentChanged(Entity<StrapComponent> ent, ref EntParentChangedMessage args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<FTLMapComponent>(((EntParentChangedMessage)(ref args)).Transform.ParentUid) || !((EntParentChangedMessage)(ref args)).OldParent.HasValue)
		{
			return;
		}
		foreach (EntityUid entity in ent.Comp.BuckledEntities)
		{
			_buckle.TryUnbuckle(Entity<BuckleComponent>.op_Implicit(entity), entity, popup: false);
			AttemptCrashLandEvent ev = new AttemptCrashLandEvent(entity);
			((EntitySystem)this).RaiseLocalEvent<AttemptCrashLandEvent>(((EntParentChangedMessage)(ref args)).OldParent.Value, ref ev, false);
			if (!ev.Cancelled)
			{
				_crashLand.TryCrashLand(Entity<CrashLandableComponent>.op_Implicit(entity), doDamage: true);
			}
		}
	}

	private void OnStrapCombatModeShouldHandInteract(Entity<StrapComponent> ent, ref CombatModeShouldHandInteractEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(args.User))
		{
			args.Cancelled = true;
		}
	}

	public Vector2 GetOffset(Entity<RMCBuckleOffsetComponent?> offset)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<RMCBuckleOffsetComponent>(Entity<RMCBuckleOffsetComponent>.op_Implicit(offset), ref offset.Comp, false))
		{
			return Vector2.Zero;
		}
		return offset.Comp.Offset;
	}

	public bool CanBuckle(EntityUid? user, EntityUid buckle, bool popup = true)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<XenoComponent>(user))
		{
			return true;
		}
		if (popup)
		{
			_popup.PopupPredicted("You don't have the dexterity to do that, try a nest.", buckle, user.Value, PopupType.SmallCaution);
		}
		return false;
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<ActiveBuckleClimbingComponent> query = ((EntitySystem)this).EntityQueryEnumerator<ActiveBuckleClimbingComponent>();
		EntityUid uid = default(EntityUid);
		ActiveBuckleClimbingComponent comp = default(ActiveBuckleClimbingComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			EntityUid? strap = comp.Strap;
			if (strap.HasValue)
			{
				EntityUid strap2 = strap.GetValueOrDefault();
				_intersecting.Clear();
				_entityLookup.GetEntitiesIntersecting(uid, _intersecting, (LookupFlags)110);
				if (!_intersecting.Contains(strap2))
				{
					((EntitySystem)this).RemCompDeferred<ActiveBuckleClimbingComponent>(uid);
				}
			}
			else
			{
				((EntitySystem)this).RemCompDeferred<ActiveBuckleClimbingComponent>(uid);
			}
		}
	}
}
