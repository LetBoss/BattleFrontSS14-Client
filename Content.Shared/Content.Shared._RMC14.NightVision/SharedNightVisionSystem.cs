using System;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Scoping;
using Content.Shared._RMC14.Visor;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Alert;
using Content.Shared.IgnitionSource;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Popups;
using Content.Shared.Toggleable;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.NightVision;

public abstract class SharedNightVisionSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private AlertsSystem _alerts;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private VisorSystem _visor;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<NightVisionComponent, ComponentStartup>((EntityEventRefHandler<NightVisionComponent, ComponentStartup>)OnNightVisionStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NightVisionComponent, MapInitEvent>((EntityEventRefHandler<NightVisionComponent, MapInitEvent>)OnNightVisionMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NightVisionComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<NightVisionComponent, AfterAutoHandleStateEvent>)OnNightVisionAfterHandle, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NightVisionComponent, ComponentRemove>((EntityEventRefHandler<NightVisionComponent, ComponentRemove>)OnNightVisionRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NightVisionComponent, ToggleNightVisionAlertEvent>((EntityEventRefHandler<NightVisionComponent, ToggleNightVisionAlertEvent>)OnNightVisionToggle, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NightVisionItemComponent, GetItemActionsEvent>((EntityEventRefHandler<NightVisionItemComponent, GetItemActionsEvent>)OnNightVisionItemGetActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NightVisionItemComponent, ToggleActionEvent>((EntityEventRefHandler<NightVisionItemComponent, ToggleActionEvent>)OnNightVisionItemToggle, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NightVisionItemComponent, GotEquippedEvent>((EntityEventRefHandler<NightVisionItemComponent, GotEquippedEvent>)OnNightVisionItemGotEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NightVisionItemComponent, GotUnequippedEvent>((EntityEventRefHandler<NightVisionItemComponent, GotUnequippedEvent>)OnNightVisionItemGotUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NightVisionItemComponent, ActionRemovedEvent>((EntityEventRefHandler<NightVisionItemComponent, ActionRemovedEvent>)OnNightVisionItemActionRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NightVisionItemComponent, ComponentRemove>((EntityEventRefHandler<NightVisionItemComponent, ComponentRemove>)OnNightVisionItemRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NightVisionItemComponent, EntityTerminatingEvent>((EntityEventRefHandler<NightVisionItemComponent, EntityTerminatingEvent>)OnNightVisionItemTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NightVisionVisorComponent, ActivateVisorEvent>((EntityEventRefHandler<NightVisionVisorComponent, ActivateVisorEvent>)OnNightVisionActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NightVisionVisorComponent, DeactivateVisorEvent>((EntityEventRefHandler<NightVisionVisorComponent, DeactivateVisorEvent>)OnNightVisionDeactivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NightVisionVisorComponent, VisorRelayedEvent<ScopedEvent>>((EntityEventRefHandler<NightVisionVisorComponent, VisorRelayedEvent<ScopedEvent>>)OnNightVisionScoped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCNightVisionVisibleOnIgniteComponent, IgnitionEvent>((EntityEventRefHandler<RMCNightVisionVisibleOnIgniteComponent, IgnitionEvent>)OnNightVisionVisibleIgnition, (Type[])null, (Type[])null);
	}

	private void OnNightVisionStartup(Entity<NightVisionComponent> ent, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		NightVisionChanged(ent);
	}

	private void OnNightVisionAfterHandle(Entity<NightVisionComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		NightVisionChanged(ent);
	}

	private void OnNightVisionMapInit(Entity<NightVisionComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateAlert(ent);
	}

	private void OnNightVisionRemove(Entity<NightVisionComponent> ent, ref ComponentRemove args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		ProtoId<AlertPrototype>? alert = ent.Comp.Alert;
		if (alert.HasValue)
		{
			ProtoId<AlertPrototype> alert2 = alert.GetValueOrDefault();
			_alerts.ClearAlert(Entity<NightVisionComponent>.op_Implicit(ent), alert2);
		}
		NightVisionRemoved(ent);
	}

	private void OnNightVisionToggle(Entity<NightVisionComponent> ent, ref ToggleNightVisionAlertEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Toggle(Entity<NightVisionComponent>.op_Implicit((Entity<NightVisionComponent>.op_Implicit(ent), Entity<NightVisionComponent>.op_Implicit(ent))));
	}

	private void OnNightVisionItemGetActions(Entity<NightVisionItemComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.ActionId.HasValue && !args.InHands && ent.Comp.Toggleable && ent.Comp.SlotFlags == args.SlotFlags)
		{
			GetItemActionsEvent obj = args;
			ref EntityUid? action = ref ent.Comp.Action;
			EntProtoId? actionId = ent.Comp.ActionId;
			obj.AddAction(ref action, actionId.HasValue ? EntProtoId.op_Implicit(actionId.GetValueOrDefault()) : null);
		}
	}

	private void OnNightVisionItemToggle(Entity<NightVisionItemComponent> ent, ref ToggleActionEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			ToggleNightVisionItem(ent, args.Performer);
		}
	}

	private void OnNightVisionItemGotEquipped(Entity<NightVisionItemComponent> ent, ref GotEquippedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.SlotFlags == args.SlotFlags)
		{
			EnableNightVisionItem(ent, args.Equipee);
		}
	}

	private void OnNightVisionItemGotUnequipped(Entity<NightVisionItemComponent> ent, ref GotUnequippedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.SlotFlags == args.SlotFlags)
		{
			DisableNightVisionItem(ent, args.Equipee);
		}
	}

	private void OnNightVisionItemActionRemoved(Entity<NightVisionItemComponent> ent, ref ActionRemovedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		DisableNightVisionItem(ent, ent.Comp.User);
	}

	private void OnNightVisionItemRemove(Entity<NightVisionItemComponent> ent, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		DisableNightVisionItem(ent, ent.Comp.User);
	}

	private void OnNightVisionItemTerminating(Entity<NightVisionItemComponent> ent, ref EntityTerminatingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		DisableNightVisionItem(ent, ent.Comp.User);
	}

	private void OnNightVisionActivate(Entity<NightVisionVisorComponent> ent, ref ActivateVisorEvent args)
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		if (args.User.HasValue && ((EntitySystem)this).HasComp<ScopingComponent>(args.User))
		{
			_popup.PopupClient("You cannot use the night vision optic while using optics.", args.User.Value, args.User, PopupType.SmallCaution);
			return;
		}
		args.Handled = true;
		if (!_timing.ApplyingState)
		{
			NightVisionItemComponent comp = new NightVisionItemComponent
			{
				ActionId = null,
				SlotFlags = SlotFlags.HEAD,
				Green = true,
				BlockScopes = true
			};
			((EntitySystem)this).AddComp<NightVisionItemComponent>(Entity<CycleableVisorComponent>.op_Implicit(args.CycleableVisor), comp, true);
			((EntitySystem)this).Dirty(Entity<CycleableVisorComponent>.op_Implicit(args.CycleableVisor), (IComponent)(object)comp, (MetaDataComponent)null);
			if (_inventory.InSlotWithFlags(Entity<TransformComponent, MetaDataComponent>.op_Implicit(args.CycleableVisor.Owner), comp.SlotFlags) && args.User.HasValue)
			{
				EnableNightVisionItem(Entity<NightVisionItemComponent>.op_Implicit((Entity<CycleableVisorComponent>.op_Implicit(args.CycleableVisor), comp)), args.User.Value);
				_audio.PlayLocal(ent.Comp.SoundOn, Entity<NightVisionVisorComponent>.op_Implicit(ent), args.User, (AudioParams?)null);
			}
		}
	}

	private void OnNightVisionDeactivate(Entity<NightVisionVisorComponent> ent, ref DeactivateVisorEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && !((EntitySystem)this).TerminatingOrDeleted(Entity<NightVisionVisorComponent>.op_Implicit(ent), (MetaDataComponent)null))
		{
			((EntitySystem)this).RemComp<NightVisionItemComponent>(Entity<CycleableVisorComponent>.op_Implicit(args.CycleableVisor));
			_audio.PlayLocal(ent.Comp.SoundOff, Entity<NightVisionVisorComponent>.op_Implicit(ent), args.User, (AudioParams?)null);
		}
	}

	private void OnNightVisionScoped(Entity<NightVisionVisorComponent> ent, ref VisorRelayedEvent<ScopedEvent> args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		_visor.DeactivateVisor(args.CycleableVisor, Entity<VisorComponent>.op_Implicit(ent.Owner), args.Event.User);
	}

	private void OnNightVisionVisibleIgnition(Entity<RMCNightVisionVisibleOnIgniteComponent> ent, ref IgnitionEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			if (args.Ignite)
			{
				((EntitySystem)this).EnsureComp<RMCNightVisionVisibleComponent>(Entity<RMCNightVisionVisibleOnIgniteComponent>.op_Implicit(ent));
			}
			else
			{
				((EntitySystem)this).RemCompDeferred<RMCNightVisionVisibleComponent>(Entity<RMCNightVisionVisibleOnIgniteComponent>.op_Implicit(ent));
			}
		}
	}

	public NightVisionState Toggle(Entity<NightVisionComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<NightVisionComponent>(Entity<NightVisionComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return NightVisionState.Off;
		}
		NightVisionComponent comp = ent.Comp;
		comp.State = ent.Comp.State switch
		{
			NightVisionState.Off => NightVisionState.Half, 
			NightVisionState.Half => (!ent.Comp.OnlyHalf) ? NightVisionState.Full : NightVisionState.Off, 
			NightVisionState.Full => NightVisionState.Off, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
		((EntitySystem)this).Dirty<NightVisionComponent>(ent, (MetaDataComponent)null);
		UpdateAlert(Entity<NightVisionComponent>.op_Implicit((Entity<NightVisionComponent>.op_Implicit(ent), ent.Comp)));
		return ent.Comp.State;
	}

	private void UpdateAlert(Entity<NightVisionComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		ProtoId<AlertPrototype>? alert = ent.Comp.Alert;
		if (alert.HasValue)
		{
			ProtoId<AlertPrototype> alert2 = alert.GetValueOrDefault();
			short state = (short)ent.Comp.State;
			short max = _alerts.GetMaxSeverity(alert2);
			short min = _alerts.GetMinSeverity(alert2);
			short severity = ((state > max) ? max : ((state < min) ? min : state));
			_alerts.ShowAlert(Entity<NightVisionComponent>.op_Implicit(ent), alert2, severity);
		}
		NightVisionChanged(ent);
	}

	private void ToggleNightVisionItem(Entity<NightVisionItemComponent> item, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? user2 = item.Comp.User;
		if (user2.HasValue && user2.GetValueOrDefault() == user && item.Comp.Toggleable)
		{
			DisableNightVisionItem(item, item.Comp.User);
			_audio.PlayLocal(item.Comp.SoundOff, item.Owner, (EntityUid?)user, (AudioParams?)null);
		}
		else
		{
			EnableNightVisionItem(item, user);
			_audio.PlayLocal(item.Comp.SoundOn, item.Owner, (EntityUid?)user, (AudioParams?)null);
		}
	}

	private void EnableNightVisionItem(Entity<NightVisionItemComponent> item, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		DisableNightVisionItem(item, item.Comp.User);
		if (item.Comp.Skills != null && !_skills.HasAllSkills(Entity<SkillsComponent>.op_Implicit(user), item.Comp.Skills))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-skills-hud-toggle"), user, user, PopupType.MediumCaution);
			return;
		}
		item.Comp.User = user;
		((EntitySystem)this).Dirty<NightVisionItemComponent>(item, (MetaDataComponent)null);
		_appearance.SetData(Entity<NightVisionItemComponent>.op_Implicit(item), (Enum)NightVisionItemVisuals.Active, (object)true, (AppearanceComponent)null);
		if (!_timing.ApplyingState)
		{
			NightVisionComponent nightVision = default(NightVisionComponent);
			if (((EntitySystem)this).TryComp<NightVisionComponent>(user, ref nightVision))
			{
				nightVision = ((EntitySystem)this).EnsureComp<NightVisionComponent>(user);
				nightVision.State = NightVisionState.Full;
				nightVision.Green = item.Comp.Green;
				nightVision.Mesons = item.Comp.Mesons;
				nightVision.BlockScopes = item.Comp.BlockScopes;
				((EntitySystem)this).Dirty(user, (IComponent)(object)nightVision, (MetaDataComponent)null);
			}
			else
			{
				nightVision = new NightVisionComponent
				{
					State = NightVisionState.Full,
					Green = item.Comp.Green,
					Mesons = item.Comp.Mesons,
					BlockScopes = item.Comp.BlockScopes
				};
				((EntitySystem)this).AddComp<NightVisionComponent>(user, nightVision, true);
				((EntitySystem)this).Dirty(user, (IComponent)(object)nightVision, (MetaDataComponent)null);
			}
		}
		SharedActionsSystem actions = _actions;
		EntityUid? action = item.Comp.Action;
		actions.SetToggled(action.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), toggled: true);
	}

	protected virtual void NightVisionChanged(Entity<NightVisionComponent> ent)
	{
	}

	protected virtual void NightVisionRemoved(Entity<NightVisionComponent> ent)
	{
	}

	public void DisableNightVisionItem(Entity<NightVisionItemComponent> item, EntityUid? user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		SharedActionsSystem actions = _actions;
		EntityUid? action = item.Comp.Action;
		actions.SetToggled(action.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), toggled: false);
		item.Comp.User = null;
		((EntitySystem)this).Dirty<NightVisionItemComponent>(item, (MetaDataComponent)null);
		_appearance.SetData(Entity<NightVisionItemComponent>.op_Implicit(item), (Enum)NightVisionItemVisuals.Active, (object)false, (AppearanceComponent)null);
		NightVisionComponent nightVision = default(NightVisionComponent);
		if (((EntitySystem)this).TryComp<NightVisionComponent>(user, ref nightVision) && !nightVision.Innate)
		{
			((EntitySystem)this).RemCompDeferred<NightVisionComponent>(user.Value);
		}
	}

	public void SetSeeThroughContainers(Entity<NightVisionComponent?> ent, bool see)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<NightVisionComponent>(Entity<NightVisionComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			ent.Comp.SeeThroughContainers = see;
			((EntitySystem)this).Dirty<NightVisionComponent>(ent, (MetaDataComponent)null);
		}
	}
}
