using System;
using Content.Shared._RMC14.Stealth;
using Content.Shared.Coordinates;
using Content.Shared.Humanoid;
using Content.Shared.Interaction.Events;
using Content.Shared.Stealth.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Whistle;

public sealed class WhistleSystem : EntitySystem
{
	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<WhistleComponent, UseInHandEvent>((ComponentEventHandler<WhistleComponent, UseInHandEvent>)OnUseInHand, (Type[])null, (Type[])null);
	}

	private void ExclamateTarget(EntityUid target, WhistleComponent component)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(component.Effect), target.ToCoordinates(), (ComponentRegistry)null, default(Angle));
	}

	public void OnUseInHand(EntityUid uid, WhistleComponent component, UseInHandEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && _timing.IsFirstTimePredicted)
		{
			((HandledEntityEventArgs)args).Handled = TryMakeLoudWhistle(uid, args.User, component);
		}
	}

	public bool TryMakeLoudWhistle(EntityUid uid, EntityUid owner, WhistleComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<WhistleComponent>(uid, ref component, false) || component.Distance <= 0f)
		{
			return false;
		}
		MakeLoudWhistle(uid, owner, component);
		return true;
	}

	private void MakeLoudWhistle(EntityUid uid, EntityUid owner, WhistleComponent component)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		StealthComponent stealth = null;
		foreach (Entity<HumanoidAppearanceComponent> iterator in _entityLookup.GetEntitiesInRange<HumanoidAppearanceComponent>(_transform.GetMapCoordinates(uid, (TransformComponent)null), component.Distance, (LookupFlags)110))
		{
			if ((!((EntitySystem)this).TryComp<StealthComponent>(Entity<HumanoidAppearanceComponent>.op_Implicit(iterator), ref stealth) || !stealth.Enabled) && !(iterator.Owner == owner) && !((EntitySystem)this).HasComp<EntityActiveInvisibleComponent>(Entity<HumanoidAppearanceComponent>.op_Implicit(iterator)))
			{
				ExclamateTarget(Entity<HumanoidAppearanceComponent>.op_Implicit(iterator), component);
			}
		}
	}
}
