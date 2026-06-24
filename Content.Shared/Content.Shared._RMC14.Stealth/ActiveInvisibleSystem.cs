using System;
using Content.Shared._RMC14.Evasion;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Stealth;

public sealed class ActiveInvisibleSystem : EntitySystem
{
	[Dependency]
	private EvasionSystem _evasionSystem;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<EntityActiveInvisibleComponent, ComponentAdd>((EntityEventRefHandler<EntityActiveInvisibleComponent, ComponentAdd>)OnInvisibleComponentAdd, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityActiveInvisibleComponent, ComponentRemove>((EntityEventRefHandler<EntityActiveInvisibleComponent, ComponentRemove>)OnInvisibleComponentRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityActiveInvisibleComponent, EvasionRefreshModifiersEvent>((EntityEventRefHandler<EntityActiveInvisibleComponent, EvasionRefreshModifiersEvent>)OnInvisibleRefreshModifiers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityActiveInvisibleComponent, AttemptMobCollideEvent>((EntityEventRefHandler<EntityActiveInvisibleComponent, AttemptMobCollideEvent>)OnAttemptMobCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityActiveInvisibleComponent, AttemptMobTargetCollideEvent>((EntityEventRefHandler<EntityActiveInvisibleComponent, AttemptMobTargetCollideEvent>)OnAttemptMobTargetCollide, (Type[])null, (Type[])null);
	}

	private void OnInvisibleComponentAdd(Entity<EntityActiveInvisibleComponent> entity, ref ComponentAdd args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_evasionSystem.RefreshEvasionModifiers(entity.Owner);
	}

	private void OnInvisibleComponentRemove(Entity<EntityActiveInvisibleComponent> entity, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_evasionSystem.RefreshEvasionModifiers(entity.Owner);
	}

	private void OnInvisibleRefreshModifiers(Entity<EntityActiveInvisibleComponent> entity, ref EvasionRefreshModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (!(entity.Owner != args.Entity.Owner))
		{
			args.Evasion += entity.Comp.EvasionModifier;
			args.EvasionFriendly += entity.Comp.EvasionFriendlyModifier;
		}
	}

	private void OnAttemptMobCollide(Entity<EntityActiveInvisibleComponent> ent, ref AttemptMobCollideEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.DisableMobCollision)
		{
			args.Cancelled = true;
		}
	}

	private void OnAttemptMobTargetCollide(Entity<EntityActiveInvisibleComponent> ent, ref AttemptMobTargetCollideEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.DisableMobCollision)
		{
			args.Cancelled = true;
		}
	}
}
