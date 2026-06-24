using System;
using Content.Shared.Access.Systems;
using Content.Shared.Doors.Components;
using Content.Shared.Examine;
using Content.Shared.Popups;
using Content.Shared.Prying.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Doors.Systems;

public abstract class SharedFirelockSystem : EntitySystem
{
	[Dependency]
	private AccessReaderSystem _accessReaderSystem;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedDoorSystem _doorSystem;

	[Dependency]
	private IGameTiming _gameTiming;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FirelockComponent, BeforeDoorOpenedEvent>((ComponentEventHandler<FirelockComponent, BeforeDoorOpenedEvent>)OnBeforeDoorOpened, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FirelockComponent, BeforePryEvent>((ComponentEventRefHandler<FirelockComponent, BeforePryEvent>)OnBeforePry, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FirelockComponent, GetPryTimeModifierEvent>((ComponentEventRefHandler<FirelockComponent, GetPryTimeModifierEvent>)OnDoorGetPryTimeModifier, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FirelockComponent, PriedEvent>((ComponentEventRefHandler<FirelockComponent, PriedEvent>)OnAfterPried, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FirelockComponent, MapInitEvent>((ComponentEventHandler<FirelockComponent, MapInitEvent>)UpdateVisuals, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FirelockComponent, ComponentStartup>((EntityEventRefHandler<FirelockComponent, ComponentStartup>)OnComponentStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FirelockComponent, ExaminedEvent>((EntityEventRefHandler<FirelockComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	public bool EmergencyPressureStop(EntityUid uid, FirelockComponent? firelock = null, DoorComponent? door = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<FirelockComponent, DoorComponent>(uid, ref firelock, ref door, true))
		{
			return false;
		}
		if (door.State == DoorState.Open)
		{
			if (firelock.EmergencyCloseCooldown.HasValue)
			{
				TimeSpan curTime = _gameTiming.CurTime;
				TimeSpan? emergencyCloseCooldown = firelock.EmergencyCloseCooldown;
				if (curTime < emergencyCloseCooldown)
				{
					goto IL_0054;
				}
			}
			if (!_doorSystem.TryClose(uid, door))
			{
				return false;
			}
			return _doorSystem.OnPartialClose(uid, door);
		}
		goto IL_0054;
		IL_0054:
		return false;
	}

	private void OnBeforeDoorOpened(EntityUid uid, FirelockComponent component, BeforeDoorOpenedEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		bool overrideAccess = args.User.HasValue && _accessReaderSystem.IsAllowed(args.User.Value, uid);
		if (!component.Powered || (!overrideAccess && component.IsLocked))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
		else if (args.User.HasValue)
		{
			WarnPlayer(Entity<FirelockComponent>.op_Implicit((uid, component)), args.User.Value);
		}
	}

	private void OnBeforePry(EntityUid uid, FirelockComponent component, ref BeforePryEvent args)
	{
		if (!args.Cancelled && component.Powered && !args.StrongPry && !args.PryPowered)
		{
			args.Cancelled = true;
		}
	}

	private void OnDoorGetPryTimeModifier(EntityUid uid, FirelockComponent component, ref GetPryTimeModifierEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		WarnPlayer(Entity<FirelockComponent>.op_Implicit((uid, component)), args.User);
		if (component.IsLocked)
		{
			args.PryTimeModifier *= component.LockedPryTimeModifier;
		}
	}

	private void WarnPlayer(Entity<FirelockComponent> ent, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Temperature)
		{
			_popupSystem.PopupClient(base.Loc.GetString("firelock-component-is-holding-fire-message"), ent.Owner, user, PopupType.MediumCaution);
		}
		else if (ent.Comp.Pressure)
		{
			_popupSystem.PopupClient(base.Loc.GetString("firelock-component-is-holding-pressure-message"), ent.Owner, user, PopupType.MediumCaution);
		}
	}

	private void OnAfterPried(EntityUid uid, FirelockComponent component, ref PriedEvent args)
	{
		component.EmergencyCloseCooldown = _gameTiming.CurTime + component.EmergencyCloseCooldownDuration;
	}

	protected virtual void OnComponentStartup(Entity<FirelockComponent> ent, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		UpdateVisuals(ent.Owner, ent.Comp, (EntityEventArgs)(object)args);
	}

	private void UpdateVisuals(EntityUid uid, FirelockComponent component, EntityEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateVisuals(uid, component);
	}

	private void UpdateVisuals(EntityUid uid, FirelockComponent? firelock = null, DoorComponent? door = null, AppearanceComponent? appearance = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<DoorComponent, AppearanceComponent>(uid, ref door, ref appearance, false))
		{
			if (door.State != DoorState.Closed && door.State != DoorState.Welded && door.State != DoorState.Denying)
			{
				_appearance.SetData(uid, (Enum)DoorVisuals.ClosedLights, (object)false, appearance);
			}
			else if (((EntitySystem)this).Resolve<FirelockComponent, AppearanceComponent>(uid, ref firelock, ref appearance, false))
			{
				_appearance.SetData(uid, (Enum)DoorVisuals.ClosedLights, (object)firelock.IsLocked, appearance);
			}
		}
	}

	private void OnExamined(Entity<FirelockComponent> ent, ref ExaminedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("FirelockComponent"))
		{
			if (ent.Comp.Pressure)
			{
				args.PushMarkup(base.Loc.GetString("firelock-component-examine-pressure-warning"));
			}
			if (ent.Comp.Temperature)
			{
				args.PushMarkup(base.Loc.GetString("firelock-component-examine-temperature-warning"));
			}
		}
	}
}
