using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Slow;
using Content.Shared.Clothing;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Movement;

public sealed class TemporarySpeedModifiersSystem : EntitySystem
{
	private const float MaxSpeedModifier = 1f;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeedSystem;

	[Dependency]
	private INetManager _netManager;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedContainerSystem _container;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<TemporarySpeedModifiersComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<TemporarySpeedModifiersComponent, RefreshMovementSpeedModifiersEvent>)OnRefreshMovement, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClothingSpeedModifierComponent, RMCMovementSpeedRefreshedEvent>((EntityEventRefHandler<ClothingSpeedModifierComponent, RMCMovementSpeedRefreshedEvent>)OnRMCRefreshMovement, (Type[])null, (Type[])null);
	}

	private void OnRefreshMovement(Entity<TemporarySpeedModifiersComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		foreach (var modifier in ent.Comp.Modifiers)
		{
			if (!(modifier.ExpiresAt <= _timing.CurTime))
			{
				args.ModifySpeed(modifier.Walk, modifier.Sprint);
			}
		}
	}

	private void OnRMCRefreshMovement(Entity<ClothingSpeedModifierComponent> ent, ref RMCMovementSpeedRefreshedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer wearer = default(BaseContainer);
		MovementSpeedModifierComponent movement = default(MovementSpeedModifierComponent);
		if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(Entity<ClothingSpeedModifierComponent>.op_Implicit(ent), null, null)), ref wearer) && ((EntitySystem)this).TryComp<MovementSpeedModifierComponent>(wearer.Owner, ref movement) && (((EntitySystem)this).HasComp<RMCSlowdownComponent>(wearer.Owner) || ((EntitySystem)this).HasComp<RMCSuperSlowdownComponent>(wearer.Owner)))
		{
			float newModifier = (args.WalkModifier = 1f - (1f - args.SprintModifier) * (movement.CurrentSprintSpeed / movement.BaseSprintSpeed));
			args.SprintModifier = newModifier;
		}
	}

	public override void Update(float frameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<TemporarySpeedModifiersComponent> speedModsQuery = ((EntitySystem)this).EntityQueryEnumerator<TemporarySpeedModifiersComponent>();
		EntityUid uid = default(EntityUid);
		TemporarySpeedModifiersComponent speedModsComponent = default(TemporarySpeedModifiersComponent);
		while (speedModsQuery.MoveNext(ref uid, ref speedModsComponent))
		{
			List<(TimeSpan, float, float)> toRemove = new List<(TimeSpan, float, float)>();
			foreach (var modifier in speedModsComponent.Modifiers)
			{
				if (!(modifier.ExpiresAt > time))
				{
					toRemove.Add(modifier);
				}
			}
			foreach (var modifier2 in toRemove)
			{
				speedModsComponent.Modifiers.Remove(modifier2);
			}
			if (toRemove.Count > 0)
			{
				((EntitySystem)this).Dirty(uid, (IComponent)(object)speedModsComponent, (MetaDataComponent)null);
			}
			if (speedModsComponent.Modifiers.Count <= 0)
			{
				((EntitySystem)this).RemCompDeferred<TemporarySpeedModifiersComponent>(uid);
			}
			_movementSpeedSystem.RefreshMovementSpeedModifiers(uid);
		}
	}

	public void ModifySpeed(EntityUid entUid, List<TemporarySpeedModifierSet> modifiers)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (_netManager.IsClient)
		{
			return;
		}
		TemporarySpeedModifiersComponent comp = ((EntitySystem)this).EnsureComp<TemporarySpeedModifiersComponent>(entUid);
		foreach (TemporarySpeedModifierSet modifier in modifiers)
		{
			comp.Modifiers.Add((_timing.CurTime + modifier.Duration, modifier.Walk, modifier.Sprint));
		}
	}

	public float? CalculateSpeedModifier(EntityUid uid, float modifier, MovementSpeedModifierComponent? movement = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MovementSpeedModifierComponent>(uid, ref movement, true) || movement.CurrentSprintSpeed == 0f)
		{
			return null;
		}
		float currentSpeed = movement.CurrentSprintSpeed;
		float baseSpeed = movement.BaseSprintSpeed;
		float val = 1f / (Math.Max(1f / currentSpeed * 10f + modifier, 1f) / 10f) / movement.CurrentSprintSpeed;
		float baseSpeedModifier = 1f / (Math.Max(1f / baseSpeed * 10f + modifier, 1f) / 10f) / movement.BaseSprintSpeed;
		return Math.Min(val, baseSpeedModifier);
	}
}
