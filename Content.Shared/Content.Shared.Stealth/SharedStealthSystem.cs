using System;
using Content.Shared.Examine;
using Content.Shared.Mobs;
using Content.Shared.Stealth.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Stealth;

public abstract class SharedStealthSystem : EntitySystem
{
	private sealed class GetVisibilityModifiersEvent : EntityEventArgs
	{
		public readonly StealthComponent Stealth;

		public readonly float SecondsSinceUpdate;

		public float FlatModifier;

		public GetVisibilityModifiersEvent(EntityUid uid, StealthComponent stealth, float secondsSinceUpdate, float flatModifier)
		{
			Stealth = stealth;
			SecondsSinceUpdate = secondsSinceUpdate;
			FlatModifier = flatModifier;
		}
	}

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<StealthComponent, ComponentGetState>((ComponentEventRefHandler<StealthComponent, ComponentGetState>)OnStealthGetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StealthComponent, ComponentHandleState>((ComponentEventRefHandler<StealthComponent, ComponentHandleState>)OnStealthHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StealthOnMoveComponent, MoveEvent>((ComponentEventRefHandler<StealthOnMoveComponent, MoveEvent>)OnMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StealthOnMoveComponent, GetVisibilityModifiersEvent>((ComponentEventHandler<StealthOnMoveComponent, GetVisibilityModifiersEvent>)OnGetVisibilityModifiers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StealthComponent, EntityPausedEvent>((ComponentEventRefHandler<StealthComponent, EntityPausedEvent>)OnPaused, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StealthComponent, EntityUnpausedEvent>((ComponentEventRefHandler<StealthComponent, EntityUnpausedEvent>)OnUnpaused, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StealthComponent, ComponentInit>((ComponentEventHandler<StealthComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StealthComponent, ExamineAttemptEvent>((ComponentEventHandler<StealthComponent, ExamineAttemptEvent>)OnExamineAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StealthComponent, ExaminedEvent>((ComponentEventHandler<StealthComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StealthComponent, MobStateChangedEvent>((ComponentEventHandler<StealthComponent, MobStateChangedEvent>)OnMobStateChanged, (Type[])null, (Type[])null);
	}

	private void OnExamineAttempt(EntityUid uid, StealthComponent component, ExamineAttemptEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!component.Enabled || GetVisibility(uid, component) > component.ExamineThreshold)
		{
			return;
		}
		EntityUid source = args.Examiner;
		do
		{
			if (source == uid)
			{
				return;
			}
			source = ((EntitySystem)this).Transform(source).ParentUid;
		}
		while (((EntityUid)(ref source)).IsValid());
		((CancellableEntityEventArgs)args).Cancel();
	}

	private void OnExamined(EntityUid uid, StealthComponent component, ExaminedEvent args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (component.Enabled)
		{
			args.PushMarkup(base.Loc.GetString(component.ExaminedDesc, (ValueTuple<string, object>)("target", uid)));
		}
	}

	public virtual void SetEnabled(EntityUid uid, bool value, StealthComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<StealthComponent>(uid, ref component, false) && component.Enabled != value)
		{
			component.Enabled = value;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	private void OnMobStateChanged(EntityUid uid, StealthComponent component, MobStateChangedEvent args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (args.NewMobState == MobState.Dead)
		{
			component.Enabled = component.EnabledOnDeath;
		}
		else
		{
			component.Enabled = true;
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	private void OnPaused(EntityUid uid, StealthComponent component, ref EntityPausedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		component.LastVisibility = GetVisibility(uid, component);
		component.LastUpdated = null;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	private void OnUnpaused(EntityUid uid, StealthComponent component, ref EntityUnpausedEvent args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		component.LastUpdated = _timing.CurTime;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	protected virtual void OnInit(EntityUid uid, StealthComponent component, ComponentInit args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!component.LastUpdated.HasValue && !((EntitySystem)this).Paused(uid, (MetaDataComponent)null))
		{
			component.LastUpdated = _timing.CurTime;
		}
	}

	private void OnStealthGetState(EntityUid uid, StealthComponent component, ref ComponentGetState args)
	{
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new StealthComponentState(component.LastVisibility, component.LastUpdated, component.Enabled);
	}

	private void OnStealthHandleState(EntityUid uid, StealthComponent component, ref ComponentHandleState args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentHandleState)(ref args)).Current is StealthComponentState cast)
		{
			SetEnabled(uid, cast.Enabled, component);
			component.LastVisibility = cast.Visibility;
			component.LastUpdated = cast.LastUpdated;
		}
	}

	private void OnMove(EntityUid uid, StealthOnMoveComponent component, ref MoveEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && !(args.NewPosition.EntityId != args.OldPosition.EntityId))
		{
			float delta = component.MovementVisibilityRate * (args.NewPosition.Position - args.OldPosition.Position).Length();
			ModifyVisibility(uid, delta);
		}
	}

	private void OnGetVisibilityModifiers(EntityUid uid, StealthOnMoveComponent component, GetVisibilityModifiersEvent args)
	{
		float mod = args.SecondsSinceUpdate * component.PassiveVisibilityRate;
		args.FlatModifier += mod;
	}

	public void ModifyVisibility(EntityUid uid, float delta, StealthComponent? component = null)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (delta != 0f && ((EntitySystem)this).Resolve<StealthComponent>(uid, ref component, true))
		{
			if (component.LastUpdated.HasValue)
			{
				component.LastVisibility = GetVisibility(uid, component);
				component.LastUpdated = _timing.CurTime;
			}
			component.LastVisibility = Math.Clamp(component.LastVisibility + delta, component.MinVisibility, component.MaxVisibility);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	public void SetVisibility(EntityUid uid, float value, StealthComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<StealthComponent>(uid, ref component, true))
		{
			component.LastVisibility = Math.Clamp(value, component.MinVisibility, component.MaxVisibility);
			if (component.LastUpdated.HasValue)
			{
				component.LastUpdated = _timing.CurTime;
			}
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	public float GetVisibility(EntityUid uid, StealthComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StealthComponent>(uid, ref component, true) || !component.Enabled)
		{
			return 1f;
		}
		if (!component.LastUpdated.HasValue)
		{
			return component.LastVisibility;
		}
		TimeSpan deltaTime = _timing.CurTime - component.LastUpdated.Value;
		GetVisibilityModifiersEvent ev = new GetVisibilityModifiersEvent(uid, component, (float)deltaTime.TotalSeconds, 0f);
		((EntitySystem)this).RaiseLocalEvent<GetVisibilityModifiersEvent>(uid, ev, false);
		return Math.Clamp(component.LastVisibility + ev.FlatModifier, component.MinVisibility, component.MaxVisibility);
	}
}
