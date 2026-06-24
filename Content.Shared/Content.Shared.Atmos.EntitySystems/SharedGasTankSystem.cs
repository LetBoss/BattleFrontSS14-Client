using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Atmos.Components;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Examine;
using Content.Shared.Timing;
using Content.Shared.Toggleable;
using Content.Shared.UserInterface;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Atmos.EntitySystems;

public abstract class SharedGasTankSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedContainerSystem _containers;

	[Dependency]
	private SharedInternalsSystem _internals;

	[Dependency]
	protected SharedUserInterfaceSystem UI;

	[Dependency]
	private UseDelaySystem _delay;

	public const string GasTankDelay = "gasTank";

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GasTankComponent, ComponentShutdown>((EntityEventRefHandler<GasTankComponent, ComponentShutdown>)OnGasShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasTankComponent, BeforeActivatableUIOpenEvent>((EntityEventRefHandler<GasTankComponent, BeforeActivatableUIOpenEvent>)BeforeUiOpen, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasTankComponent, GetItemActionsEvent>((ComponentEventHandler<GasTankComponent, GetItemActionsEvent>)OnGetActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasTankComponent, ExaminedEvent>((ComponentEventHandler<GasTankComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasTankComponent, ToggleActionEvent>((EntityEventRefHandler<GasTankComponent, ToggleActionEvent>)OnActionToggle, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasTankComponent, GasTankSetPressureMessage>((EntityEventRefHandler<GasTankComponent, GasTankSetPressureMessage>)OnGasTankSetPressure, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasTankComponent, GasTankToggleInternalsMessage>((EntityEventRefHandler<GasTankComponent, GasTankToggleInternalsMessage>)OnGasTankToggleInternals, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasTankComponent, GetVerbsEvent<AlternativeVerb>>((ComponentEventHandler<GasTankComponent, GetVerbsEvent<AlternativeVerb>>)OnGetAlternativeVerb, (Type[])null, (Type[])null);
	}

	private void OnGasShutdown(Entity<GasTankComponent> gasTank, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		DisconnectFromInternals(gasTank);
	}

	private void OnGasTankToggleInternals(Entity<GasTankComponent> ent, ref GasTankToggleInternalsMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		ToggleInternals(ent, ((BaseBoundUserInterfaceEvent)args).Actor);
	}

	private void OnGasTankSetPressure(Entity<GasTankComponent> ent, ref GasTankSetPressureMessage args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		float pressure = Math.Clamp(args.Pressure, 0f, ent.Comp.MaxOutputPressure);
		ent.Comp.OutputPressure = pressure;
		((EntitySystem)this).Dirty<GasTankComponent>(ent, (MetaDataComponent)null);
		UpdateUserInterface(ent);
	}

	public virtual void UpdateUserInterface(Entity<GasTankComponent> ent)
	{
	}

	private void BeforeUiOpen(Entity<GasTankComponent> ent, ref BeforeActivatableUIOpenEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateUserInterface(ent);
	}

	private void OnGetActions(EntityUid uid, GasTankComponent component, GetItemActionsEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		args.AddAction(ref component.ToggleActionEntity, EntProtoId.op_Implicit(component.ToggleAction));
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	private void OnExamined(EntityUid uid, GasTankComponent component, ExaminedEvent args)
	{
		using (args.PushGroup("GasTankComponent"))
		{
			if (args.IsInDetailsRange)
			{
				args.PushMarkup(base.Loc.GetString("comp-gas-tank-examine", (ValueTuple<string, object>)("pressure", Math.Round(component.Air?.Pressure ?? 0f))));
			}
			if (component.IsConnected)
			{
				args.PushMarkup(base.Loc.GetString("comp-gas-tank-connected"));
			}
			args.PushMarkup(base.Loc.GetString(component.IsValveOpen ? "comp-gas-tank-examine-open-valve" : "comp-gas-tank-examine-closed-valve"));
		}
	}

	private void OnActionToggle(Entity<GasTankComponent> gasTank, ref ToggleActionEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			ToggleInternals(gasTank, args.Performer);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnGetAlternativeVerb(EntityUid uid, GasTankComponent component, GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract && args.Hands != null)
		{
			args.Verbs.Add(new AlternativeVerb
			{
				Text = (component.IsValveOpen ? base.Loc.GetString("comp-gas-tank-close-valve") : base.Loc.GetString("comp-gas-tank-open-valve")),
				Act = delegate
				{
					//IL_0030: Unknown result type (might be due to invalid IL or missing references)
					//IL_003b: Unknown result type (might be due to invalid IL or missing references)
					//IL_005b: Unknown result type (might be due to invalid IL or missing references)
					component.IsValveOpen = !component.IsValveOpen;
					_audio.PlayPredicted(component.ValveSound, uid, (EntityUid?)args.User, (AudioParams?)null);
					((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
				},
				Disabled = component.IsConnected
			});
		}
	}

	public bool CanConnectToInternals(Entity<GasTankComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		TryGetInternalsComp(ent, out EntityUid? _, out InternalsComponent internalsComp, ent.Comp.User);
		if (internalsComp != null && internalsComp.BreathTools.Count != 0)
		{
			return !ent.Comp.IsValveOpen;
		}
		return false;
	}

	public bool ConnectToInternals(Entity<GasTankComponent> ent, EntityUid? user = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		Entity<GasTankComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		GasTankComponent gasTankComponent = default(GasTankComponent);
		val.Deconstruct(ref val2, ref gasTankComponent);
		EntityUid owner = val2;
		GasTankComponent component = gasTankComponent;
		if (component.IsConnected || !CanConnectToInternals(ent))
		{
			return false;
		}
		TryGetInternalsComp(ent, out EntityUid? internalsUid, out InternalsComponent internalsComp, ent.Comp.User);
		if (!internalsUid.HasValue || internalsComp == null)
		{
			return false;
		}
		if (!_delay.TryResetDelay(ent.Owner, checkDelayed: true, null, "gasTank"))
		{
			return false;
		}
		if (_internals.TryConnectTank(Entity<InternalsComponent>.op_Implicit((internalsUid.Value, internalsComp)), owner))
		{
			component.User = internalsUid.Value;
		}
		((EntitySystem)this).Dirty<GasTankComponent>(ent, (MetaDataComponent)null);
		SharedActionsSystem actions = _actions;
		EntityUid? toggleActionEntity = component.ToggleActionEntity;
		actions.SetToggled(toggleActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(toggleActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), component.IsConnected);
		SharedActionsSystem actions2 = _actions;
		toggleActionEntity = component.ToggleActionEntity;
		actions2.SetCooldown(toggleActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(toggleActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), TimeSpan.FromSeconds(1L));
		if (!component.IsConnected)
		{
			return false;
		}
		component.ConnectStream = _audio.Stop(component.ConnectStream, (AudioComponent)null);
		component.ConnectStream = _audio.PlayPredicted(component.ConnectSound, owner, user, (AudioParams?)null)?.Item1;
		UpdateUserInterface(ent);
		return true;
	}

	private bool TryGetInternalsComp(Entity<GasTankComponent> ent, out EntityUid? internalsUid, out InternalsComponent? internalsComp, EntityUid? user = null)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		internalsUid = null;
		internalsComp = null;
		if (((EntitySystem)this).TerminatingOrDeleted(ent.Owner, (MetaDataComponent)null))
		{
			return false;
		}
		EntityUid? val = user;
		if (!val.HasValue)
		{
			user = ent.Comp.User;
		}
		InternalsComponent userInternalsComp = default(InternalsComponent);
		if (((EntitySystem)this).TryComp<InternalsComponent>(user, ref userInternalsComp))
		{
			internalsUid = user;
			internalsComp = userInternalsComp;
			return true;
		}
		BaseContainer container = default(BaseContainer);
		InternalsComponent containerInternalsComp = default(InternalsComponent);
		if (_containers.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ent.Owner, ((EntitySystem)this).Transform(ent.Owner))), ref container) && ((EntitySystem)this).TryComp<InternalsComponent>(container.Owner, ref containerInternalsComp))
		{
			internalsUid = container.Owner;
			internalsComp = containerInternalsComp;
			return true;
		}
		return false;
	}

	public bool DisconnectFromInternals(Entity<GasTankComponent> ent, EntityUid? user = null, bool forced = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		Entity<GasTankComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		GasTankComponent gasTankComponent = default(GasTankComponent);
		val.Deconstruct(ref val2, ref gasTankComponent);
		EntityUid owner = val2;
		GasTankComponent component = gasTankComponent;
		if (!component.User.HasValue)
		{
			return false;
		}
		if (!forced && !_delay.TryResetDelay(ent.Owner, checkDelayed: true, null, "gasTank"))
		{
			return false;
		}
		TryGetInternalsComp(ent, out EntityUid? internalsUid, out InternalsComponent internalsComp, component.User);
		component.User = null;
		((EntitySystem)this).Dirty<GasTankComponent>(ent, (MetaDataComponent)null);
		SharedActionsSystem actions = _actions;
		EntityUid? toggleActionEntity = component.ToggleActionEntity;
		actions.SetToggled(toggleActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(toggleActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), toggled: false);
		if (!forced && _delay.TryGetDelayInfo(Entity<UseDelayComponent>.op_Implicit(ent.Owner), out UseDelayInfo delayInfo, "gasTank"))
		{
			SharedActionsSystem actions2 = _actions;
			toggleActionEntity = component.ToggleActionEntity;
			actions2.SetCooldown(toggleActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(toggleActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), delayInfo.Length);
		}
		if (internalsUid.HasValue && internalsComp != null)
		{
			_internals.DisconnectTank(Entity<InternalsComponent>.op_Implicit((internalsUid.Value, internalsComp)), forced);
		}
		component.DisconnectStream = _audio.Stop(component.DisconnectStream, (AudioComponent)null);
		component.DisconnectStream = _audio.PlayPredicted(component.DisconnectSound, owner, user, (AudioParams?)null)?.Item1;
		UpdateUserInterface(ent);
		return true;
	}

	private bool ToggleInternals(Entity<GasTankComponent> ent, EntityUid? user = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.IsConnected)
		{
			return DisconnectFromInternals(ent, user);
		}
		return ConnectToInternals(ent, user);
	}
}
