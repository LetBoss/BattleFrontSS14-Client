using System;
using Content.Shared.Doors.Components;
using Content.Shared.Popups;
using Content.Shared.Prying.Components;
using Content.Shared.Wires;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Doors.Systems;

public abstract class SharedAirlockSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	protected SharedAppearanceSystem Appearance;

	[Dependency]
	protected SharedAudioSystem Audio;

	[Dependency]
	protected SharedDoorSystem DoorSystem;

	[Dependency]
	protected SharedPopupSystem Popup;

	[Dependency]
	private SharedWiresSystem _wiresSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AirlockComponent, BeforeDoorClosedEvent>((ComponentEventHandler<AirlockComponent, BeforeDoorClosedEvent>)OnBeforeDoorClosed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AirlockComponent, DoorStateChangedEvent>((ComponentEventHandler<AirlockComponent, DoorStateChangedEvent>)OnStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AirlockComponent, DoorBoltsChangedEvent>((ComponentEventHandler<AirlockComponent, DoorBoltsChangedEvent>)OnBoltsChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AirlockComponent, BeforeDoorOpenedEvent>((ComponentEventHandler<AirlockComponent, BeforeDoorOpenedEvent>)OnBeforeDoorOpened, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AirlockComponent, BeforeDoorDeniedEvent>((ComponentEventHandler<AirlockComponent, BeforeDoorDeniedEvent>)OnBeforeDoorDenied, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AirlockComponent, GetPryTimeModifierEvent>((ComponentEventRefHandler<AirlockComponent, GetPryTimeModifierEvent>)OnGetPryMod, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AirlockComponent, BeforePryEvent>((ComponentEventRefHandler<AirlockComponent, BeforePryEvent>)OnBeforePry, (Type[])null, (Type[])null);
	}

	private void OnBeforeDoorClosed(EntityUid uid, AirlockComponent airlock, BeforeDoorClosedEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled)
		{
			if (!airlock.Safety)
			{
				args.PerformCollisionCheck = false;
			}
			DoorComponent door = default(DoorComponent);
			if (((EntitySystem)this).TryComp<DoorComponent>(uid, ref door) && !args.Partial && !CanChangeState(uid, airlock))
			{
				((CancellableEntityEventArgs)args).Cancel();
			}
		}
	}

	private void OnStateChanged(EntityUid uid, AirlockComponent component, DoorStateChangedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			WiresPanelComponent wiresPanel = default(WiresPanelComponent);
			if (((EntitySystem)this).TryComp<WiresPanelComponent>(uid, ref wiresPanel))
			{
				_wiresSystem.ChangePanelVisibility(uid, wiresPanel, component.OpenPanelVisible || args.State != DoorState.Open);
			}
			UpdateAutoClose(uid, component);
			if (args.State == DoorState.Closed)
			{
				component.AutoClose = true;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			}
		}
	}

	private void OnBoltsChanged(EntityUid uid, AirlockComponent component, DoorBoltsChangedEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (!args.BoltsDown)
		{
			UpdateAutoClose(uid, component);
		}
	}

	private void OnBeforeDoorOpened(EntityUid uid, AirlockComponent component, BeforeDoorOpenedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!CanChangeState(uid, component))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnBeforeDoorDenied(EntityUid uid, AirlockComponent component, BeforeDoorDeniedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!CanChangeState(uid, component))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnGetPryMod(EntityUid uid, AirlockComponent component, ref GetPryTimeModifierEvent args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (component.Powered)
		{
			args.PryTimeModifier *= component.PoweredPryModifier;
		}
		if (DoorSystem.IsBolted(uid))
		{
			args.PryTimeModifier *= component.BoltedPryModifier;
		}
	}

	public void UpdateAutoClose(EntityUid uid, AirlockComponent? airlock = null, DoorComponent? door = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<AirlockComponent, DoorComponent>(uid, ref airlock, ref door, true) && door.State == DoorState.Open && airlock.AutoClose && CanChangeState(uid, airlock))
		{
			BeforeDoorAutoCloseEvent autoev = new BeforeDoorAutoCloseEvent();
			((EntitySystem)this).RaiseLocalEvent<BeforeDoorAutoCloseEvent>(uid, autoev, false);
			if (!((CancellableEntityEventArgs)autoev).Cancelled)
			{
				DoorSystem.SetNextStateChange(uid, airlock.AutoCloseDelay * airlock.AutoCloseDelayModifier);
			}
		}
	}

	private void OnBeforePry(EntityUid uid, AirlockComponent component, ref BeforePryEvent args)
	{
		if (!args.Cancelled && component.Powered && !args.PryPowered)
		{
			args.Message = "airlock-component-cannot-pry-is-powered-message";
			args.Cancelled = true;
		}
	}

	public void UpdateEmergencyLightStatus(EntityUid uid, AirlockComponent component)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Appearance.SetData(uid, (Enum)DoorVisuals.EmergencyLights, (object)component.EmergencyAccess, (AppearanceComponent)null);
	}

	public void SetEmergencyAccess(Entity<AirlockComponent> ent, bool value, EntityUid? user = null, bool predicted = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Powered && ent.Comp.EmergencyAccess != value)
		{
			ent.Comp.EmergencyAccess = value;
			((EntitySystem)this).Dirty(Entity<AirlockComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp, (MetaDataComponent)null);
			UpdateEmergencyLightStatus(Entity<AirlockComponent>.op_Implicit(ent), ent.Comp);
			SoundSpecifier sound = (ent.Comp.EmergencyAccess ? ent.Comp.EmergencyOnSound : ent.Comp.EmergencyOffSound);
			if (predicted)
			{
				Audio.PlayPredicted(sound, Entity<AirlockComponent>.op_Implicit(ent), user, (AudioParams?)null);
			}
			else
			{
				Audio.PlayPvs(sound, Entity<AirlockComponent>.op_Implicit(ent), (AudioParams?)null);
			}
		}
	}

	public void SetAutoCloseDelayModifier(AirlockComponent component, float value)
	{
		if (!component.AutoCloseDelayModifier.Equals(value))
		{
			component.AutoCloseDelayModifier = value;
		}
	}

	public void SetSafety(AirlockComponent component, bool value)
	{
		component.Safety = value;
	}

	public bool CanChangeState(EntityUid uid, AirlockComponent component)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (component.Powered)
		{
			return !DoorSystem.IsBolted(uid);
		}
		return false;
	}
}
