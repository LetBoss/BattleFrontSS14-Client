using System;
using System.Linq;
using Content.Shared._RMC14.Movement;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Movement.Systems;
using Content.Shared.Rejuvenate;
using Content.Shared.Standing;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Slow;

public sealed class RMCSlowSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private MovementSpeedModifierSystem _speed;

	[Dependency]
	private StandingStateSystem _standing;

	[Dependency]
	private TemporarySpeedModifiersSystem _temporarySpeed;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCSlowdownComponent, ComponentStartup>((EntityEventRefHandler<RMCSlowdownComponent, ComponentStartup>)OnAdded<RMCSlowdownComponent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSuperSlowdownComponent, ComponentStartup>((EntityEventRefHandler<RMCSuperSlowdownComponent, ComponentStartup>)OnAdded<RMCSuperSlowdownComponent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCRootedComponent, ComponentStartup>((EntityEventRefHandler<RMCRootedComponent, ComponentStartup>)OnAdded<RMCRootedComponent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSlowdownComponent, ComponentShutdown>((EntityEventRefHandler<RMCSlowdownComponent, ComponentShutdown>)OnExpire<RMCSlowdownComponent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSuperSlowdownComponent, ComponentShutdown>((EntityEventRefHandler<RMCSuperSlowdownComponent, ComponentShutdown>)OnExpire<RMCSuperSlowdownComponent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCRootedComponent, ComponentShutdown>((EntityEventRefHandler<RMCRootedComponent, ComponentShutdown>)OnExpire<RMCRootedComponent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSlowdownComponent, ComponentRemove>((EntityEventRefHandler<RMCSlowdownComponent, ComponentRemove>)OnRemove<RMCSlowdownComponent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSuperSlowdownComponent, ComponentRemove>((EntityEventRefHandler<RMCSuperSlowdownComponent, ComponentRemove>)OnRemove<RMCSuperSlowdownComponent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCRootedComponent, ComponentRemove>((EntityEventRefHandler<RMCRootedComponent, ComponentRemove>)OnRemove<RMCRootedComponent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSlowdownComponent, RejuvenateEvent>((EntityEventRefHandler<RMCSlowdownComponent, RejuvenateEvent>)OnRejuvenate<RMCSlowdownComponent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSuperSlowdownComponent, RejuvenateEvent>((EntityEventRefHandler<RMCSuperSlowdownComponent, RejuvenateEvent>)OnRejuvenate<RMCSuperSlowdownComponent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCRootedComponent, RejuvenateEvent>((EntityEventRefHandler<RMCRootedComponent, RejuvenateEvent>)OnRejuvenate<RMCRootedComponent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSlowdownComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<RMCSlowdownComponent, RefreshMovementSpeedModifiersEvent>)OnSlowdownRefresh, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSuperSlowdownComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<RMCSuperSlowdownComponent, RefreshMovementSpeedModifiersEvent>)OnSuperSlowdownRefresh, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCRootedComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<RMCRootedComponent, RefreshMovementSpeedModifiersEvent>)OnRootRefresh, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSpeciesSlowdownModifierComponent, StunnedEvent>((EntityEventRefHandler<RMCSpeciesSlowdownModifierComponent, StunnedEvent>)OnModifierStun, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSpeciesSlowdownModifierComponent, KnockedDownEvent>((EntityEventRefHandler<RMCSpeciesSlowdownModifierComponent, KnockedDownEvent>)OnModifierKnockdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSpeciesSlowdownModifierComponent, StatusEffectEndedEvent>((EntityEventRefHandler<RMCSpeciesSlowdownModifierComponent, StatusEffectEndedEvent>)OnModifierEffectEnd, (Type[])null, (Type[])null);
	}

	public bool TrySlowdown(EntityUid ent, TimeSpan duration, bool refresh = true, bool ignoreDurationModifier = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		RMCSpeciesSlowdownModifierComponent slow = default(RMCSpeciesSlowdownModifierComponent);
		if (!((EntitySystem)this).TryComp<RMCSpeciesSlowdownModifierComponent>(ent, ref slow))
		{
			return false;
		}
		TimeSpan expire = _timing.CurTime + duration * (ignoreDurationModifier ? 1f : slow.DurationMultiplier);
		RMCSlowdownComponent slowdown = ((EntitySystem)this).EnsureComp<RMCSlowdownComponent>(ent);
		if (refresh && expire > slowdown.ExpiresAt)
		{
			slowdown.ExpiresAt = expire;
		}
		else if (!refresh)
		{
			slowdown.ExpiresAt += duration;
		}
		return true;
	}

	public bool TrySuperSlowdown(EntityUid ent, TimeSpan duration, bool refresh = true, bool ignoreDurationModifier = false)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.ApplyingState)
		{
			return false;
		}
		RMCSpeciesSlowdownModifierComponent slow = default(RMCSpeciesSlowdownModifierComponent);
		if (!((EntitySystem)this).TryComp<RMCSpeciesSlowdownModifierComponent>(ent, ref slow))
		{
			return false;
		}
		TimeSpan expire = _timing.CurTime + duration * (ignoreDurationModifier ? 1f : slow.DurationMultiplier);
		RMCSuperSlowdownComponent slowdown = ((EntitySystem)this).EnsureComp<RMCSuperSlowdownComponent>(ent);
		if (refresh && expire > slowdown.ExpiresAt)
		{
			slowdown.ExpiresAt = expire;
		}
		else if (!refresh)
		{
			slowdown.ExpiresAt += duration;
		}
		return true;
	}

	public bool TryRoot(EntityUid ent, TimeSpan duration, bool refresh = true)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan expire = _timing.CurTime + duration;
		RMCRootedComponent slowdown = ((EntitySystem)this).EnsureComp<RMCRootedComponent>(ent);
		if (refresh && expire > slowdown.ExpiresAt)
		{
			slowdown.ExpiresAt = expire;
		}
		else if (!refresh)
		{
			slowdown.ExpiresAt += duration;
		}
		return true;
	}

	private void OnAdded<T>(Entity<T> ent, ref ComponentStartup args) where T : IComponent
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		_speed.RefreshMovementSpeedModifiers(Entity<T>.op_Implicit(ent));
		if (!((EntitySystem)this).HasComp<XenoComponent>(Entity<T>.op_Implicit(ent)))
		{
			if (typeof(T) != typeof(RMCRootedComponent))
			{
				((EntitySystem)this).EnsureComp<XenoSlowVisualsComponent>(Entity<T>.op_Implicit(ent));
			}
			else
			{
				((EntitySystem)this).EnsureComp<XenoImmobileVisualsComponent>(Entity<T>.op_Implicit(ent));
			}
		}
	}

	private void OnExpire<T>(Entity<T> ent, ref ComponentShutdown args) where T : IComponent
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (typeof(T) != typeof(RMCRootedComponent))
		{
			if (typeof(T) == typeof(RMCSlowdownComponent))
			{
				MaybeRemoveSlowVisuals(Entity<T>.op_Implicit(ent));
			}
			else
			{
				MaybeRemoveSuperSlowVisuals(Entity<T>.op_Implicit(ent));
			}
		}
		else
		{
			MaybeRemoveStunVisuals(Entity<T>.op_Implicit(ent));
		}
	}

	private void OnRemove<T>(Entity<T> ent, ref ComponentRemove args) where T : Component
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).TerminatingOrDeleted(Entity<T>.op_Implicit(ent), (MetaDataComponent)null))
		{
			_speed.RefreshMovementSpeedModifiers(Entity<T>.op_Implicit(ent));
		}
	}

	private void OnRejuvenate<T>(Entity<T> ent, ref RejuvenateEvent args) where T : IComponent
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<T>(Entity<T>.op_Implicit(ent));
	}

	private void OnSlowdownRefresh(Entity<RMCSlowdownComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		RMCSpeciesSlowdownModifierComponent slow = default(RMCSpeciesSlowdownModifierComponent);
		if (((EntitySystem)this).TryComp<RMCSpeciesSlowdownModifierComponent>(Entity<RMCSlowdownComponent>.op_Implicit(ent), ref slow) && ((Component)ent.Comp).Running)
		{
			float? multiplier = _temporarySpeed.CalculateSpeedModifier(Entity<RMCSlowdownComponent>.op_Implicit(ent), slow.SlowModifier);
			RMCSuperSlowdownComponent comp = default(RMCSuperSlowdownComponent);
			if (multiplier.HasValue && (!((EntitySystem)this).TryComp<RMCSuperSlowdownComponent>(Entity<RMCSlowdownComponent>.op_Implicit(ent), ref comp) || !((Component)comp).Running))
			{
				args.ModifySpeed(multiplier.Value, multiplier.Value);
			}
		}
	}

	private void OnSuperSlowdownRefresh(Entity<RMCSuperSlowdownComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		RMCSpeciesSlowdownModifierComponent slow = default(RMCSpeciesSlowdownModifierComponent);
		if (((EntitySystem)this).TryComp<RMCSpeciesSlowdownModifierComponent>(Entity<RMCSuperSlowdownComponent>.op_Implicit(ent), ref slow) && ((Component)ent.Comp).Running)
		{
			float? multiplier = _temporarySpeed.CalculateSpeedModifier(Entity<RMCSuperSlowdownComponent>.op_Implicit(ent), slow.SuperSlowModifier);
			if (multiplier.HasValue)
			{
				args.ModifySpeed(multiplier.Value, multiplier.Value);
			}
		}
	}

	private void OnRootRefresh(Entity<RMCRootedComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (((Component)ent.Comp).Running)
		{
			args.ModifySpeed(0f, 0f);
		}
	}

	private void MaybeRemoveSlowVisuals(EntityUid ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoSlowVisualsComponent>(ent) && !((EntitySystem)this).HasComp<RMCSuperSlowdownComponent>(ent))
		{
			((EntitySystem)this).RemCompDeferred<XenoSlowVisualsComponent>(ent);
		}
	}

	private void MaybeRemoveSuperSlowVisuals(EntityUid ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoSlowVisualsComponent>(ent) && !((EntitySystem)this).HasComp<RMCSlowdownComponent>(ent))
		{
			((EntitySystem)this).RemCompDeferred<XenoSlowVisualsComponent>(ent);
		}
	}

	private void MaybeRemoveStunVisuals(EntityUid ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoImmobileVisualsComponent>(ent) && (!((EntitySystem)this).HasComp<StunnedComponent>(ent) || _standing.IsDown(ent)))
		{
			((EntitySystem)this).RemCompDeferred<XenoImmobileVisualsComponent>(ent);
		}
	}

	private void OnModifierStun(Entity<RMCSpeciesSlowdownModifierComponent> ent, ref StunnedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!_standing.IsDown(Entity<RMCSpeciesSlowdownModifierComponent>.op_Implicit(ent)))
		{
			((EntitySystem)this).EnsureComp<XenoImmobileVisualsComponent>(Entity<RMCSpeciesSlowdownModifierComponent>.op_Implicit(ent));
		}
	}

	private void OnModifierKnockdown(Entity<RMCSpeciesSlowdownModifierComponent> ent, ref KnockedDownEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoImmobileVisualsComponent>(Entity<RMCSpeciesSlowdownModifierComponent>.op_Implicit(ent)))
		{
			((EntitySystem)this).RemCompDeferred<XenoImmobileVisualsComponent>(Entity<RMCSpeciesSlowdownModifierComponent>.op_Implicit(ent));
		}
	}

	private void OnModifierEffectEnd(Entity<RMCSpeciesSlowdownModifierComponent> ent, ref StatusEffectEndedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (Enumerable.Contains(ent.Comp.StatusesToUpdateOn, args.Key))
		{
			if (args.Key != "KnockedDown" && !((EntitySystem)this).HasComp<RMCRootedComponent>(Entity<RMCSpeciesSlowdownModifierComponent>.op_Implicit(ent)))
			{
				((EntitySystem)this).RemCompDeferred<XenoImmobileVisualsComponent>(Entity<RMCSpeciesSlowdownModifierComponent>.op_Implicit(ent));
			}
			else if ((args.Key == "KnockedDown" || !_standing.IsDown(Entity<RMCSpeciesSlowdownModifierComponent>.op_Implicit(ent))) && ((EntitySystem)this).HasComp<StunnedComponent>(Entity<RMCSpeciesSlowdownModifierComponent>.op_Implicit(ent)))
			{
				((EntitySystem)this).EnsureComp<XenoImmobileVisualsComponent>(Entity<RMCSpeciesSlowdownModifierComponent>.op_Implicit(ent));
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<RMCSlowdownComponent> slowQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCSlowdownComponent>();
		EntityUid uid = default(EntityUid);
		RMCSlowdownComponent slow = default(RMCSlowdownComponent);
		while (slowQuery.MoveNext(ref uid, ref slow))
		{
			if (!(time < slow.ExpiresAt))
			{
				((EntitySystem)this).RemCompDeferred<RMCSlowdownComponent>(uid);
				_speed.RefreshMovementSpeedModifiers(uid);
			}
		}
		EntityQueryEnumerator<RMCSuperSlowdownComponent> superSlowQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCSuperSlowdownComponent>();
		EntityUid uid2 = default(EntityUid);
		RMCSuperSlowdownComponent slow2 = default(RMCSuperSlowdownComponent);
		while (superSlowQuery.MoveNext(ref uid2, ref slow2))
		{
			if (!(time < slow2.ExpiresAt))
			{
				((EntitySystem)this).RemCompDeferred<RMCSuperSlowdownComponent>(uid2);
				_speed.RefreshMovementSpeedModifiers(uid2);
			}
		}
		EntityQueryEnumerator<RMCRootedComponent> rootQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCRootedComponent>();
		EntityUid uid3 = default(EntityUid);
		RMCRootedComponent root = default(RMCRootedComponent);
		while (rootQuery.MoveNext(ref uid3, ref root))
		{
			if (!(time < root.ExpiresAt))
			{
				((EntitySystem)this).RemCompDeferred<RMCRootedComponent>(uid3);
				_speed.RefreshMovementSpeedModifiers(uid3);
			}
		}
	}
}
