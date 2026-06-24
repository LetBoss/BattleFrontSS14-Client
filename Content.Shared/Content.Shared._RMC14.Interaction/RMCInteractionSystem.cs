using System;
using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Light.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Shared._RMC14.Interaction;

public sealed class RMCInteractionSystem : EntitySystem
{
	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<InteractedBlacklistComponent, GettingInteractedWithAttemptEvent>((EntityEventRefHandler<InteractedBlacklistComponent, GettingInteractedWithAttemptEvent>)OnBlacklistInteractionAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NoHandsInteractionBlockedComponent, GettingInteractedWithAttemptEvent>((EntityEventRefHandler<NoHandsInteractionBlockedComponent, GettingInteractedWithAttemptEvent>)OnNoHandsInteractionAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InsertBlacklistComponent, ContainerIsInsertingAttemptEvent>((EntityEventRefHandler<InsertBlacklistComponent, ContainerIsInsertingAttemptEvent>)OnInsertBlacklistContainerIsInsertingAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IgnoreInteractionRangeComponent, InRangeOverrideEvent>((EntityEventRefHandler<IgnoreInteractionRangeComponent, InRangeOverrideEvent>)OnInRangeOverride, (Type[])null, (Type[])null);
	}

	private void OnNoHandsInteractionAttempt(Entity<NoHandsInteractionBlockedComponent> ent, ref GettingInteractedWithAttemptEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !((EntitySystem)this).HasComp<HandsComponent>(args.Uid))
		{
			args.Cancelled = true;
		}
	}

	private void OnBlacklistInteractionAttempt(Entity<InteractedBlacklistComponent> ent, ref GettingInteractedWithAttemptEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = default(TransformComponent);
		HandheldLightComponent handheldLight = default(HandheldLightComponent);
		if (!args.Cancelled && ent.Comp.Blacklist != null && ((EntitySystem)this).TryComp(Entity<InteractedBlacklistComponent>.op_Implicit(ent), ref xform) && (!ent.Comp.AnchoredOnly || xform.Anchored) && (!((EntitySystem)this).TryComp<HandheldLightComponent>(Entity<InteractedBlacklistComponent>.op_Implicit(ent), ref handheldLight) || !handheldLight.Activated) && _whitelist.IsValid(ent.Comp.Blacklist, args.Uid))
		{
			args.Cancelled = true;
		}
	}

	private void OnInsertBlacklistContainerIsInsertingAttempt(Entity<InsertBlacklistComponent> ent, ref ContainerIsInsertingAttemptEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled)
		{
			InsertBlacklistComponent comp = ent.Comp;
			MobStateComponent blacklistedMobState = default(MobStateComponent);
			MobStateComponent whitelistedMobState = default(MobStateComponent);
			if ((comp.Blacklist != null && _whitelist.IsValid(comp.Blacklist, ((ContainerAttemptEventBase)args).EntityUid)) || (comp.BlacklistedMobStates != null && ((EntitySystem)this).TryComp<MobStateComponent>(((ContainerAttemptEventBase)args).EntityUid, ref blacklistedMobState) && comp.BlacklistedMobStates.Contains(blacklistedMobState.CurrentState)))
			{
				((CancellableEntityEventArgs)args).Cancel();
			}
			else if ((comp.Whitelist != null && !_whitelist.IsValid(comp.Whitelist, ((ContainerAttemptEventBase)args).EntityUid)) || (comp.WhitelistedMobStates != null && ((EntitySystem)this).TryComp<MobStateComponent>(((ContainerAttemptEventBase)args).EntityUid, ref whitelistedMobState) && !comp.WhitelistedMobStates.Contains(whitelistedMobState.CurrentState)))
			{
				((CancellableEntityEventArgs)args).Cancel();
			}
		}
	}

	private void OnInRangeOverride(Entity<IgnoreInteractionRangeComponent> ent, ref InRangeOverrideEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (_whitelist.IsWhitelistPassOrNull(ent.Comp.Whitelist, args.Target) && _transform.InRange(Entity<TransformComponent>.op_Implicit(args.User), Entity<TransformComponent>.op_Implicit(args.Target), ent.Comp.Range))
		{
			args.InRange = true;
			args.Handled = true;
		}
	}

	public void TryCapWorldRotation(Entity<MaxRotationComponent?, TransformComponent?> max, ref Angle angle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<MaxRotationComponent, TransformComponent>(Entity<MaxRotationComponent, TransformComponent>.op_Implicit(max), ref max.Comp1, ref max.Comp2, false))
		{
			Angle set = max.Comp1.Set;
			Angle deviation = max.Comp1.Deviation;
			if (Angle.op_Implicit(Angle.ShortestDistance(ref angle, ref set)) > Angle.op_Implicit(deviation))
			{
				angle = set + deviation;
			}
			if (Angle.op_Implicit(Angle.ShortestDistance(ref angle, ref set)) < Angle.op_Implicit(-deviation))
			{
				angle = set - deviation;
			}
		}
	}

	public bool CanFaceMaxRotation(Entity<MaxRotationComponent?, TransformComponent?> max, Angle angle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MaxRotationComponent, TransformComponent>(Entity<MaxRotationComponent, TransformComponent>.op_Implicit(max), ref max.Comp1, ref max.Comp2, false))
		{
			return true;
		}
		Angle set = max.Comp1.Set;
		Angle deviation = max.Comp1.Deviation;
		if (Angle.op_Implicit(Angle.ShortestDistance(ref angle, ref set)) > Angle.op_Implicit(deviation) || Angle.op_Implicit(Angle.ShortestDistance(ref angle, ref set)) < Angle.op_Implicit(-deviation))
		{
			return false;
		}
		return true;
	}

	public void SetMaxRotation(Entity<MaxRotationComponent?> ent, Angle set, Angle deviation)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		ref MaxRotationComponent comp = ref ent.Comp;
		if (comp == null)
		{
			comp = ((EntitySystem)this).EnsureComp<MaxRotationComponent>(Entity<MaxRotationComponent>.op_Implicit(ent));
		}
		ent.Comp.Set = set;
		ent.Comp.Deviation = deviation;
		((EntitySystem)this).Dirty<MaxRotationComponent>(ent, (MetaDataComponent)null);
	}
}
