using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared.Actions;
using Content.Shared.Administration.Logs;
using Content.Shared.Audio;
using Content.Shared.Database;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Content.Shared.UserInterface;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Telephone;

public abstract class SharedRMCTelephoneSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private SharedAmbientSoundSystem _ambientSound;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SquadSystem _squad;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	private static readonly SoundSpecifier RemotePickupSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/_RMC14/Phone/remote_pickup.ogg", (AudioParams?)null);

	private static readonly SoundSpecifier RemoteHangupSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/_RMC14/Phone/remote_hangup.ogg", (AudioParams?)null);

	private static readonly SoundSpecifier BusySound = (SoundSpecifier)new SoundPathSpecifier("/Audio/_RMC14/Phone/phone_busy.ogg", (AudioParams?)null);

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RotaryPhoneComponent, MapInitEvent>((EntityEventRefHandler<RotaryPhoneComponent, MapInitEvent>)OnRotaryPhoneMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RotaryPhoneComponent, BeforeActivatableUIOpenEvent>((EntityEventRefHandler<RotaryPhoneComponent, BeforeActivatableUIOpenEvent>)OnRotaryPhoneBeforeOpen, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RotaryPhoneComponent, ComponentShutdown>((EntityEventRefHandler<RotaryPhoneComponent, ComponentShutdown>)OnRotaryPhoneTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RotaryPhoneComponent, EntityTerminatingEvent>((EntityEventRefHandler<RotaryPhoneComponent, EntityTerminatingEvent>)OnRotaryPhoneTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RotaryPhoneDialingComponent, InteractUsingEvent>((EntityEventRefHandler<RotaryPhoneDialingComponent, InteractUsingEvent>)OnRotaryPhoneDialingInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RotaryPhoneReceivingComponent, InteractHandEvent>((EntityEventRefHandler<RotaryPhoneReceivingComponent, InteractHandEvent>)OnRotaryPhoneReceivingInteractHand, new Type[1] { typeof(ActivatableUISystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RotaryPhoneReceivingComponent, InteractUsingEvent>((EntityEventRefHandler<RotaryPhoneReceivingComponent, InteractUsingEvent>)OnRotaryPhoneReceivingInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCTelephoneComponent, ComponentShutdown>((EntityEventRefHandler<RMCTelephoneComponent, ComponentShutdown>)OnTelephoneTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCTelephoneComponent, EntityTerminatingEvent>((EntityEventRefHandler<RMCTelephoneComponent, EntityTerminatingEvent>)OnTelephoneTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RotaryPhoneBackpackComponent, GetItemActionsEvent>((EntityEventRefHandler<RotaryPhoneBackpackComponent, GetItemActionsEvent>)OnBackpackGetItemActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RotaryPhoneBackpackComponent, RMCTelephoneActionEvent>((EntityEventRefHandler<RotaryPhoneBackpackComponent, RMCTelephoneActionEvent>)OnBackpackTelephoneAction, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<RotaryPhoneComponent>(((EntitySystem)this).Subs, (object)RMCTelephoneUiKey.Key, (BuiEventSubscriber<RotaryPhoneComponent>)delegate(Subscriber<RotaryPhoneComponent> subs)
		{
			subs.Event<RMCTelephoneCallBuiMsg>((EntityEventRefHandler<RotaryPhoneComponent, RMCTelephoneCallBuiMsg>)OnTelephoneCallMsg);
			subs.Event<RMCTelephoneDndBuiMsg>((EntityEventRefHandler<RotaryPhoneComponent, RMCTelephoneDndBuiMsg>)OnTelephoneDndMsg);
		});
	}

	private void OnRotaryPhoneMapInit(Entity<RotaryPhoneComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		_container.EnsureContainer<ContainerSlot>(Entity<RotaryPhoneComponent>.op_Implicit(ent), ent.Comp.ContainerId, (ContainerManagerComponent)null);
		EntityUid? phone = default(EntityUid?);
		if (((EntitySystem)this).TrySpawnInContainer(EntProtoId<RMCTelephoneComponent>.op_Implicit(ent.Comp.PhoneId), Entity<RotaryPhoneComponent>.op_Implicit(ent), ent.Comp.ContainerId, ref phone, (ContainerManagerComponent)null, (ComponentRegistry)null))
		{
			ent.Comp.Phone = phone.Value;
			((EntitySystem)this).Dirty<RotaryPhoneComponent>(ent, (MetaDataComponent)null);
			RMCTelephoneComponent phoneComp = default(RMCTelephoneComponent);
			if (((EntitySystem)this).TryComp<RMCTelephoneComponent>(phone, ref phoneComp))
			{
				phoneComp.RotaryPhone = Entity<RotaryPhoneComponent>.op_Implicit(ent);
				((EntitySystem)this).Dirty(phone.Value, (IComponent)(object)phoneComp, (MetaDataComponent)null);
			}
		}
	}

	private void OnRotaryPhoneBeforeOpen(Entity<RotaryPhoneComponent> ent, ref BeforeActivatableUIOpenEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		SendUIState(Entity<RotaryPhoneComponent>.op_Implicit(ent));
	}

	private void OnRotaryPhoneTerminating<T>(Entity<RotaryPhoneComponent> ent, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		RMCTelephoneComponent phone = default(RMCTelephoneComponent);
		if (((EntitySystem)this).TryComp<RMCTelephoneComponent>(ent.Comp.Phone, ref phone))
		{
			phone.RotaryPhone = null;
			((EntitySystem)this).Dirty(ent.Comp.Phone.Value, (IComponent)(object)phone, (MetaDataComponent)null);
		}
	}

	private void OnRotaryPhoneDialingInteractUsing(Entity<RotaryPhoneDialingComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (HangUpDialing(ent, args.Used, args.User))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnRotaryPhoneReceivingInteractHand(Entity<RotaryPhoneReceivingComponent> ent, ref InteractHandEvent args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			((HandledEntityEventArgs)args).Handled = true;
			PickupReceiving(ent, args.User);
		}
	}

	private void OnRotaryPhoneReceivingInteractUsing(Entity<RotaryPhoneReceivingComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (HangUpReceiving(ent, args.Used, args.User))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnTelephoneTerminating<T>(Entity<RMCTelephoneComponent> ent, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		RotaryPhoneComponent phone = default(RotaryPhoneComponent);
		if (((EntitySystem)this).TryComp<RotaryPhoneComponent>(ent.Comp.RotaryPhone, ref phone))
		{
			phone.Phone = null;
			((EntitySystem)this).Dirty(ent.Comp.RotaryPhone.Value, (IComponent)(object)phone, (MetaDataComponent)null);
		}
	}

	private void OnBackpackGetItemActions(Entity<RotaryPhoneBackpackComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if ((args.SlotFlags & ent.Comp.Slot) != 0 || args.InHands)
		{
			args.AddAction(ref ent.Comp.Action, EntProtoId.op_Implicit(ent.Comp.ActionId), Entity<RotaryPhoneBackpackComponent>.op_Implicit(ent));
		}
	}

	private void OnBackpackTelephoneAction(Entity<RotaryPhoneBackpackComponent> ent, ref RMCTelephoneActionEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = true;
		if (!((EntitySystem)this).HasComp<RotaryPhoneDialingComponent>(Entity<RotaryPhoneBackpackComponent>.op_Implicit(ent)))
		{
			RotaryPhoneReceivingComponent receiving = default(RotaryPhoneReceivingComponent);
			if (((EntitySystem)this).TryComp<RotaryPhoneReceivingComponent>(Entity<RotaryPhoneBackpackComponent>.op_Implicit(ent), ref receiving))
			{
				PickupReceiving(Entity<RotaryPhoneReceivingComponent>.op_Implicit((Entity<RotaryPhoneBackpackComponent>.op_Implicit(ent), receiving)), args.Performer);
				return;
			}
			SendUIState(Entity<RotaryPhoneBackpackComponent>.op_Implicit(ent));
			_ui.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)RMCTelephoneUiKey.Key, args.Performer, false);
		}
	}

	private void OnTelephoneCallMsg(Entity<RotaryPhoneComponent> ent, ref RMCTelephoneCallBuiMsg args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		if (time < ent.Comp.LastCall + ent.Comp.CallCooldown)
		{
			return;
		}
		_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)RMCTelephoneUiKey.Key);
		if (_net.IsClient)
		{
			return;
		}
		EntityUid target = ((EntitySystem)this).GetEntity(args.Id);
		RotaryPhoneComponent targetRotaryPhone = default(RotaryPhoneComponent);
		if (!((EntityUid)(ref target)).Valid || ent.Owner == target || !((EntitySystem)this).TryComp<RotaryPhoneComponent>(target, ref targetRotaryPhone) || ((EntitySystem)this).HasComp<RotaryPhoneDialingComponent>(Entity<RotaryPhoneComponent>.op_Implicit(ent)))
		{
			return;
		}
		EntityUid user = ((BaseBoundUserInterfaceEvent)args).Actor;
		if (IsPhoneBusy(target))
		{
			_popup.PopupEntity("That phone is busy!", user, user, PopupType.MediumCaution);
			return;
		}
		if (((EntitySystem)this).HasComp<RotaryPhoneBackpackComponent>(target) && !TryGetPhoneBackpackHolder(target, out var _))
		{
			_popup.PopupEntity("No transmitters could be located to call!", user, user, PopupType.MediumCaution);
			return;
		}
		MetaDataComponent marineMeta = default(MetaDataComponent);
		MetaDataComponent phoneMeta = default(MetaDataComponent);
		if (((EntitySystem)this).HasComp<MarineComponent>(user) && ((EntitySystem)this).TryComp(user, ref marineMeta) && ((EntitySystem)this).TryComp(Entity<RotaryPhoneComponent>.op_Implicit(ent), ref phoneMeta))
		{
			_popup.PopupEntity(marineMeta.EntityName + " dials a number on the " + phoneMeta.EntityName + ".", Entity<RotaryPhoneComponent>.op_Implicit(ent));
		}
		ent.Comp.Idle = false;
		ent.Comp.LastCall = time;
		((EntitySystem)this).Dirty<RotaryPhoneComponent>(ent, (MetaDataComponent)null);
		RotaryPhoneDialingComponent dialing = ((EntitySystem)this).EnsureComp<RotaryPhoneDialingComponent>(Entity<RotaryPhoneComponent>.op_Implicit(ent));
		dialing.Other = target;
		((EntitySystem)this).Dirty(Entity<RotaryPhoneComponent>.op_Implicit(ent), (IComponent)(object)dialing, (MetaDataComponent)null);
		RotaryPhoneReceivingComponent receiving = ((EntitySystem)this).EnsureComp<RotaryPhoneReceivingComponent>(target);
		receiving.Other = Entity<RotaryPhoneComponent>.op_Implicit(ent);
		((EntitySystem)this).Dirty(target, (IComponent)(object)receiving, (MetaDataComponent)null);
		if (_net.IsServer)
		{
			SoundSpecifier dialingSound = ent.Comp.DialingSound;
			AudioParams val;
			if (dialingSound != null)
			{
				AmbientSoundComponent selfSound = ((EntitySystem)this).EnsureComp<AmbientSoundComponent>(Entity<RotaryPhoneComponent>.op_Implicit(ent));
				_ambientSound.SetSound(Entity<RotaryPhoneComponent>.op_Implicit(ent), dialingSound, selfSound);
				_ambientSound.SetRange(Entity<RotaryPhoneComponent>.op_Implicit(ent), 16f, selfSound);
				SharedAmbientSoundSystem ambientSound = _ambientSound;
				EntityUid uid = Entity<RotaryPhoneComponent>.op_Implicit(ent);
				val = dialingSound.Params;
				ambientSound.SetVolume(uid, ((AudioParams)(ref val)).Volume, selfSound);
				_ambientSound.SetAmbience(Entity<RotaryPhoneComponent>.op_Implicit(ent), value: true, selfSound);
			}
			SoundSpecifier receivingSound = ent.Comp.ReceivingSound;
			if (receivingSound != null)
			{
				AmbientSoundComponent otherSound = ((EntitySystem)this).EnsureComp<AmbientSoundComponent>(target);
				_ambientSound.SetSound(target, receivingSound, otherSound);
				_ambientSound.SetRange(target, 16f, otherSound);
				SharedAmbientSoundSystem ambientSound2 = _ambientSound;
				EntityUid uid2 = target;
				val = receivingSound.Params;
				ambientSound2.SetVolume(uid2, ((AudioParams)(ref val)).Volume, otherSound);
				_ambientSound.SetAmbience(target, value: true, otherSound);
				RMCTelephoneRingEvent ev = new RMCTelephoneRingEvent(target, Entity<RotaryPhoneComponent>.op_Implicit(ent), ((BaseBoundUserInterfaceEvent)args).Actor);
				((EntitySystem)this).RaiseLocalEvent<RMCTelephoneRingEvent>(ref ev);
			}
		}
		EntityUid? phone = ent.Comp.Phone;
		if (phone.HasValue)
		{
			EntityUid phone2 = phone.GetValueOrDefault();
			PickupPhone(ent, phone2, user);
		}
		UpdateAppearance(Entity<RotaryPhoneComponent>.op_Implicit((Entity<RotaryPhoneComponent>.op_Implicit(ent), Entity<RotaryPhoneComponent>.op_Implicit(ent))));
		UpdateAppearance(Entity<RotaryPhoneComponent>.op_Implicit((target, targetRotaryPhone)));
		ISharedAdminLogManager adminLog = _adminLog;
		LogStringHandler handler = new LogStringHandler(24, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "ToPrettyString(args.Actor)");
		handler.AppendLiteral(" started calling ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "ToPrettyString(target)");
		handler.AppendLiteral(" using ");
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<RotaryPhoneComponent>.op_Implicit(ent), (MetaDataComponent)null), "ToPrettyString(ent)");
		adminLog.Add(LogType.RMCTelephone, ref handler);
	}

	private void OnTelephoneDndMsg(Entity<RotaryPhoneComponent> ent, ref RMCTelephoneDndBuiMsg args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (args.Dnd && ent.Comp.CanDnd)
		{
			((EntitySystem)this).EnsureComp<RotaryPhoneDndComponent>(Entity<RotaryPhoneComponent>.op_Implicit(ent));
		}
		else
		{
			((EntitySystem)this).RemComp<RotaryPhoneDndComponent>(Entity<RotaryPhoneComponent>.op_Implicit(ent));
		}
		SendUIState(Entity<RotaryPhoneComponent>.op_Implicit(ent));
	}

	private bool IsPhoneBusy(EntityUid ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<RotaryPhoneDialingComponent>(ent) && !((EntitySystem)this).HasComp<RotaryPhoneReceivingComponent>(ent))
		{
			return ((EntitySystem)this).HasComp<RotaryPhoneDndComponent>(ent);
		}
		return true;
	}

	private void UpdateAppearance(Entity<RotaryPhoneComponent?> phone, bool forceNotRinging = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<RotaryPhoneComponent>(Entity<RotaryPhoneComponent>.op_Implicit(phone), ref phone.Comp, false))
		{
			RotaryPhoneVisuals visual = RotaryPhoneVisuals.Base;
			BaseContainer container = default(BaseContainer);
			if (!_container.TryGetContainer(Entity<RotaryPhoneComponent>.op_Implicit(phone), phone.Comp.ContainerId, ref container, (ContainerManagerComponent)null) || container.ContainedEntities.Count == 0)
			{
				visual = RotaryPhoneVisuals.Ear;
			}
			else if (((EntitySystem)this).HasComp<RotaryPhoneReceivingComponent>(Entity<RotaryPhoneComponent>.op_Implicit(phone)) && !forceNotRinging)
			{
				visual = RotaryPhoneVisuals.Ring;
			}
			_appearance.SetData(Entity<RotaryPhoneComponent>.op_Implicit(phone), (Enum)RotaryPhoneLayers.Layer, (object)visual, (AppearanceComponent)null);
		}
	}

	protected virtual void PickupPhone(Entity<RotaryPhoneComponent> rotary, EntityUid telephone, EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (_container.TryGetContainer(Entity<RotaryPhoneComponent>.op_Implicit(rotary), rotary.Comp.ContainerId, ref container, (ContainerManagerComponent)null))
		{
			_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(telephone), container, true, false, (EntityCoordinates?)null, (Angle?)null);
		}
		_hands.TryPickupAnyHand(user, telephone);
		((EntitySystem)this).EnsureComp<RMCPickedUpPhoneComponent>(telephone);
		PlayGrabSound(Entity<RotaryPhoneComponent>.op_Implicit(rotary));
	}

	private void ReturnPhone(EntityUid rotary, EntityUid telephone, EntityUid? user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		RotaryPhoneComponent rotaryPhone = default(RotaryPhoneComponent);
		if (!((EntitySystem)this).TryComp<RotaryPhoneComponent>(rotary, ref rotaryPhone))
		{
			return;
		}
		EntityUid? phone = rotaryPhone.Phone;
		BaseContainer container = default(BaseContainer);
		if (!phone.HasValue || phone.GetValueOrDefault() != telephone || !_container.TryGetContainer(rotary, rotaryPhone.ContainerId, ref container, (ContainerManagerComponent)null))
		{
			return;
		}
		if (user.HasValue)
		{
			if (_hands.TryDropIntoContainer(Entity<HandsComponent>.op_Implicit(user.Value), telephone, container))
			{
				PlayGrabSound(rotary);
			}
		}
		else if (_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(telephone), container, (TransformComponent)null, false))
		{
			PlayGrabSound(rotary);
		}
	}

	private void HangUp(EntityUid self, EntityUid other, EntityUid? user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		StopSound(self);
		if (!((EntitySystem)this).HasComp<RotaryPhoneDialingComponent>(other) && !((EntitySystem)this).HasComp<RotaryPhoneReceivingComponent>(other))
		{
			StopSound(other);
			return;
		}
		if (_net.IsServer)
		{
			_ambientSound.SetSound(other, BusySound);
			SharedAmbientSoundSystem ambientSound = _ambientSound;
			AudioParams val = BusySound.Params;
			ambientSound.SetVolume(other, ((AudioParams)(ref val)).Volume);
			_ambientSound.SetAmbience(other, value: true);
		}
		if (HasPickedUp(Entity<RotaryPhoneComponent, RotaryPhoneReceivingComponent>.op_Implicit(other)))
		{
			if (_net.IsServer)
			{
				_audio.PlayPvs(RemoteHangupSound, other, (AudioParams?)null);
			}
			ISharedAdminLogManager adminLog = _adminLog;
			LogStringHandler handler = new LogStringHandler(24, 3);
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString(user, (MetaDataComponent)null), "ToPrettyString(user)");
			handler.AppendLiteral(" hung up ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(self)), "ToPrettyString(self)");
			handler.AppendLiteral(" while calling ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(other)), "ToPrettyString(other)");
			adminLog.Add(LogType.RMCTelephone, ref handler);
		}
	}

	private void StopSound(EntityUid ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_ambientSound.SetAmbience(ent, value: false);
	}

	private void PlayGrabSound(EntityUid rotary)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		RotaryPhoneComponent comp = default(RotaryPhoneComponent);
		if (!_net.IsClient && ((EntitySystem)this).TryComp<RotaryPhoneComponent>(rotary, ref comp))
		{
			_audio.PlayPvs(comp.GrabSound, rotary, (AudioParams?)null);
			_audio.Stop(comp.VoicemailSoundEntity, (AudioComponent)null);
		}
	}

	protected bool TryGetOtherPhone(EntityUid rotary, out EntityUid other)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		RotaryPhoneDialingComponent dialing = default(RotaryPhoneDialingComponent);
		RotaryPhoneComponent otherRotary = default(RotaryPhoneComponent);
		if (((EntitySystem)this).TryComp<RotaryPhoneDialingComponent>(rotary, ref dialing) && ((EntitySystem)this).TryComp<RotaryPhoneComponent>(dialing.Other, ref otherRotary) && otherRotary.Phone.HasValue)
		{
			other = otherRotary.Phone.Value;
			return true;
		}
		RotaryPhoneReceivingComponent receiving = default(RotaryPhoneReceivingComponent);
		if (((EntitySystem)this).TryComp<RotaryPhoneReceivingComponent>(rotary, ref receiving) && ((EntitySystem)this).TryComp<RotaryPhoneComponent>(receiving.Other, ref otherRotary) && otherRotary.Phone.HasValue)
		{
			other = otherRotary.Phone.Value;
			return true;
		}
		other = default(EntityUid);
		return false;
	}

	private bool IsCorrectPhone(Entity<RotaryPhoneComponent?> rotary, EntityUid phone)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<RotaryPhoneComponent>(Entity<RotaryPhoneComponent>.op_Implicit(rotary), ref rotary.Comp, false))
		{
			EntityUid? phone2 = rotary.Comp.Phone;
			if (!phone2.HasValue)
			{
				return false;
			}
			return phone2.GetValueOrDefault() == phone;
		}
		return false;
	}

	private bool HasPickedUp(Entity<RotaryPhoneComponent?, RotaryPhoneReceivingComponent?> receiving)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (((EntitySystem)this).Resolve<RotaryPhoneComponent, RotaryPhoneReceivingComponent>(Entity<RotaryPhoneComponent, RotaryPhoneReceivingComponent>.op_Implicit(receiving), ref receiving.Comp1, ref receiving.Comp2, false) && _container.TryGetContainer(Entity<RotaryPhoneComponent, RotaryPhoneReceivingComponent>.op_Implicit(receiving), receiving.Comp1.ContainerId, ref container, (ContainerManagerComponent)null))
		{
			return container.ContainedEntities.Count == 0;
		}
		return false;
	}

	private bool TryGetPhoneBackpackHolder(EntityUid backpack, out EntityUid holder)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		holder = default(EntityUid);
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(backpack, null, null)), ref container))
		{
			return false;
		}
		if (!((EntitySystem)this).HasComp<InventoryComponent>(container.Owner))
		{
			return false;
		}
		holder = container.Owner;
		return true;
	}

	private void SendUIState(EntityUid phone)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		List<RMCPhone> phones = new List<RMCPhone>();
		EntityQueryEnumerator<RotaryPhoneComponent> phonesQuery = ((EntitySystem)this).EntityQueryEnumerator<RotaryPhoneComponent>();
		EntityUid otherId = default(EntityUid);
		RotaryPhoneComponent otherComp = default(RotaryPhoneComponent);
		while (phonesQuery.MoveNext(ref otherId, ref otherComp))
		{
			if (!(otherId == phone))
			{
				string name = GetPhoneName(Entity<RotaryPhoneComponent>.op_Implicit((otherId, otherComp)));
				phones.Add(new RMCPhone(((EntitySystem)this).GetNetEntity(otherId, (MetaDataComponent)null), otherComp.Category, name));
			}
		}
		bool canDnd = ((EntitySystem)this).Comp<RotaryPhoneComponent>(phone).CanDnd;
		bool dnd = ((EntitySystem)this).HasComp<RotaryPhoneDndComponent>(phone);
		_ui.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(phone), (Enum)RMCTelephoneUiKey.Key, (BoundUserInterfaceState)(object)new RMCTelephoneBuiState(phones, canDnd, dnd));
	}

	private void PickupReceiving(Entity<RotaryPhoneReceivingComponent> receiving, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		RotaryPhoneComponent rotaryPhone = default(RotaryPhoneComponent);
		EntityUid? phone;
		if (((EntitySystem)this).TryComp<RotaryPhoneComponent>(Entity<RotaryPhoneReceivingComponent>.op_Implicit(receiving), ref rotaryPhone))
		{
			phone = rotaryPhone.Phone;
			if (phone.HasValue)
			{
				EntityUid phone2 = phone.GetValueOrDefault();
				PickupPhone(Entity<RotaryPhoneComponent>.op_Implicit((Entity<RotaryPhoneReceivingComponent>.op_Implicit(receiving), rotaryPhone)), phone2, user);
			}
		}
		StopSound(Entity<RotaryPhoneReceivingComponent>.op_Implicit(receiving));
		phone = receiving.Comp.Other;
		if (phone.HasValue)
		{
			EntityUid other = phone.GetValueOrDefault();
			StopSound(other);
			if (_net.IsServer)
			{
				_audio.PlayPvs(RemotePickupSound, other, (AudioParams?)null);
			}
		}
		UpdateAppearance(Entity<RotaryPhoneComponent>.op_Implicit((Entity<RotaryPhoneReceivingComponent>.op_Implicit(receiving), rotaryPhone)));
		ISharedAdminLogManager adminLog = _adminLog;
		LogStringHandler handler = new LogStringHandler(11, 2);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
		handler.AppendLiteral(" picked up ");
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<RotaryPhoneReceivingComponent>.op_Implicit(receiving), (MetaDataComponent)null), "ToPrettyString(receiving)");
		adminLog.Add(LogType.RMCTelephone, ref handler);
	}

	protected string GetPhoneName(Entity<RotaryPhoneComponent?> phone)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		string name = ((EntitySystem)this).Name(Entity<RotaryPhoneComponent>.op_Implicit(phone), (MetaDataComponent)null);
		if (!((EntitySystem)this).Resolve<RotaryPhoneComponent>(Entity<RotaryPhoneComponent>.op_Implicit(phone), ref phone.Comp, false))
		{
			return name;
		}
		if (!phone.Comp.TryGetHolderName)
		{
			return name;
		}
		if (!TryGetPhoneBackpackHolder(Entity<RotaryPhoneComponent>.op_Implicit(phone), out var holder))
		{
			return name;
		}
		name = ((EntitySystem)this).Name(holder, (MetaDataComponent)null);
		JobPrefixComponent jobPrefix = default(JobPrefixComponent);
		if (((EntitySystem)this).TryComp<JobPrefixComponent>(holder, ref jobPrefix))
		{
			name = base.Loc.GetString(LocId.op_Implicit(jobPrefix.Prefix)) + " " + name;
		}
		if (_squad.TryGetMemberSquad(Entity<SquadMemberComponent>.op_Implicit(holder), out Entity<SquadTeamComponent> squad))
		{
			name = name + " (" + ((EntitySystem)this).Name(Entity<SquadTeamComponent>.op_Implicit(squad), (MetaDataComponent)null) + ")";
		}
		return name;
	}

	private bool HangUpDialing(Entity<RotaryPhoneDialingComponent> ent, EntityUid phone, EntityUid? user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (!IsCorrectPhone(Entity<RotaryPhoneComponent>.op_Implicit(ent.Owner), phone))
		{
			return false;
		}
		((EntitySystem)this).RemCompDeferred<RotaryPhoneDialingComponent>(Entity<RotaryPhoneDialingComponent>.op_Implicit(ent));
		ReturnPhone(ent.Owner, phone, user);
		StopSound(ent.Owner);
		EntityUid? other = ent.Comp.Other;
		if (other.HasValue)
		{
			EntityUid other2 = other.GetValueOrDefault();
			StopSound(other2);
			HangUp(Entity<RotaryPhoneDialingComponent>.op_Implicit(ent), other2, user);
			if (!HasPickedUp(Entity<RotaryPhoneComponent, RotaryPhoneReceivingComponent>.op_Implicit(other2)))
			{
				((EntitySystem)this).RemCompDeferred<RotaryPhoneReceivingComponent>(other2);
				StopSound(other2);
			}
			UpdateAppearance(Entity<RotaryPhoneComponent>.op_Implicit(other2), forceNotRinging: true);
		}
		UpdateAppearance(Entity<RotaryPhoneComponent>.op_Implicit(ent.Owner), forceNotRinging: true);
		return true;
	}

	private bool HangUpReceiving(Entity<RotaryPhoneReceivingComponent> ent, EntityUid used, EntityUid? user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		if (!IsCorrectPhone(Entity<RotaryPhoneComponent>.op_Implicit(ent.Owner), used))
		{
			return false;
		}
		((EntitySystem)this).RemCompDeferred<RotaryPhoneReceivingComponent>(Entity<RotaryPhoneReceivingComponent>.op_Implicit(ent));
		ReturnPhone(ent.Owner, used, user);
		EntityUid? other = ent.Comp.Other;
		if (other.HasValue)
		{
			EntityUid other2 = other.GetValueOrDefault();
			RotaryPhoneDialingComponent dialing = default(RotaryPhoneDialingComponent);
			if (((EntitySystem)this).TryComp<RotaryPhoneDialingComponent>(other2, ref dialing))
			{
				dialing.Other = null;
				((EntitySystem)this).Dirty(other2, (IComponent)(object)dialing, (MetaDataComponent)null);
			}
			HangUp(Entity<RotaryPhoneReceivingComponent>.op_Implicit(ent), other2, user);
			if (!HasPickedUp(Entity<RotaryPhoneComponent, RotaryPhoneReceivingComponent>.op_Implicit(other2)))
			{
				((EntitySystem)this).RemCompDeferred<RotaryPhoneReceivingComponent>(other2);
			}
		}
		UpdateAppearance(Entity<RotaryPhoneComponent>.op_Implicit(ent.Owner), forceNotRinging: true);
		return true;
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<RotaryPhoneDialingComponent, RotaryPhoneComponent> dialingQuery = ((EntitySystem)this).EntityQueryEnumerator<RotaryPhoneDialingComponent, RotaryPhoneComponent>();
		EntityUid uid = default(EntityUid);
		RotaryPhoneDialingComponent dialing = default(RotaryPhoneDialingComponent);
		RotaryPhoneComponent phone = default(RotaryPhoneComponent);
		RotaryPhoneReceivingComponent receiving = default(RotaryPhoneReceivingComponent);
		RotaryPhoneComponent receivingPhone = default(RotaryPhoneComponent);
		while (dialingQuery.MoveNext(ref uid, ref dialing, ref phone))
		{
			if (!phone.Phone.HasValue)
			{
				continue;
			}
			AudioParams val;
			if (time > dialing.LastVoicemail + phone.VoicemailTimeoutDelay && dialing.DidVoicemail && !dialing.DidVoicemailTimeout)
			{
				dialing.DidVoicemailTimeout = true;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)dialing, (MetaDataComponent)null);
				_ambientSound.SetSound(uid, BusySound);
				SharedAmbientSoundSystem ambientSound = _ambientSound;
				EntityUid uid2 = uid;
				val = BusySound.Params;
				ambientSound.SetVolume(uid2, ((AudioParams)(ref val)).Volume);
				_ambientSound.SetAmbience(uid, value: true);
			}
			EntityUid? other = dialing.Other;
			if (!other.HasValue)
			{
				continue;
			}
			EntityUid other2 = other.GetValueOrDefault();
			if (!((EntitySystem)this).TryComp<RotaryPhoneReceivingComponent>(other2, ref receiving) || !((EntitySystem)this).TryComp<RotaryPhoneComponent>(other2, ref receivingPhone) || !receivingPhone.Phone.HasValue || HasPickedUp(Entity<RotaryPhoneComponent, RotaryPhoneReceivingComponent>.op_Implicit(other2)))
			{
				continue;
			}
			if (phone.Idle)
			{
				if (time > phone.LastCall + phone.VoicemailDelay && !dialing.DidVoicemail)
				{
					if (HangUpReceiving(Entity<RotaryPhoneReceivingComponent>.op_Implicit((other2, receiving)), receivingPhone.Phone.Value, null))
					{
						StopSound(other2);
						StopSound(uid);
					}
					phone.VoicemailSoundEntity = _audio.PlayPvs(phone.VoicemailSound, phone.Phone.Value, (AudioParams?)null)?.Item1;
					dialing.DidVoicemail = true;
					dialing.LastVoicemail = time;
					((EntitySystem)this).Dirty(uid, (IComponent)(object)dialing, (MetaDataComponent)null);
					((EntitySystem)this).Dirty(uid, (IComponent)(object)phone, (MetaDataComponent)null);
				}
			}
			else if (time > phone.LastCall + phone.DialingIdleDelay)
			{
				SoundSpecifier sound = phone.DialingIdleSound;
				if (sound != null)
				{
					phone.Idle = true;
					((EntitySystem)this).Dirty(uid, (IComponent)(object)phone, (MetaDataComponent)null);
					_ambientSound.SetSound(uid, sound);
					SharedAmbientSoundSystem ambientSound2 = _ambientSound;
					EntityUid uid3 = uid;
					val = sound.Params;
					ambientSound2.SetVolume(uid3, ((AudioParams)(ref val)).Volume);
					_ambientSound.SetAmbience(uid, value: true);
				}
			}
		}
		EntityQueryEnumerator<RMCPickedUpPhoneComponent, RMCTelephoneComponent> pickedUpPhonesQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCPickedUpPhoneComponent, RMCTelephoneComponent>();
		EntityUid uid4 = default(EntityUid);
		RMCPickedUpPhoneComponent pickedUp = default(RMCPickedUpPhoneComponent);
		RMCTelephoneComponent telephone = default(RMCTelephoneComponent);
		float distance = default(float);
		RotaryPhoneDialingComponent dialing2 = default(RotaryPhoneDialingComponent);
		RotaryPhoneReceivingComponent receiving2 = default(RotaryPhoneReceivingComponent);
		while (true)
		{
			if (!pickedUpPhonesQuery.MoveNext(ref uid4, ref pickedUp, ref telephone))
			{
				break;
			}
			EntityUid? other = telephone.RotaryPhone;
			if (!other.HasValue)
			{
				continue;
			}
			EntityUid rotary = other.GetValueOrDefault();
			EntityCoordinates phonePosition = _transform.GetMoverCoordinates(uid4);
			EntityCoordinates rotaryPosition = _transform.GetMoverCoordinates(rotary);
			if (((EntityCoordinates)(ref phonePosition)).TryDistance((IEntityManager)(object)base.EntityManager, _transform, rotaryPosition, ref distance) && !(distance > (float)pickedUp.Range))
			{
				continue;
			}
			if (((EntitySystem)this).TryComp<RotaryPhoneDialingComponent>(rotary, ref dialing2))
			{
				if (HangUpDialing(Entity<RotaryPhoneDialingComponent>.op_Implicit((rotary, dialing2)), uid4, null))
				{
					PhoneSnapBackPopup();
				}
			}
			else if (((EntitySystem)this).TryComp<RotaryPhoneReceivingComponent>(rotary, ref receiving2) && HangUpReceiving(Entity<RotaryPhoneReceivingComponent>.op_Implicit((rotary, receiving2)), uid4, null))
			{
				PhoneSnapBackPopup();
			}
			continue;
			void PhoneSnapBackPopup()
			{
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				_popup.PopupEntity($"The {((EntitySystem)this).Name(uid4, (MetaDataComponent)null)} snaps back to the {((EntitySystem)this).Name(rotary, (MetaDataComponent)null)}!", uid4, PopupType.MediumCaution);
			}
			void PhoneSnapBackPopup()
			{
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				_popup.PopupEntity($"The {((EntitySystem)this).Name(uid4, (MetaDataComponent)null)} snaps back to the {((EntitySystem)this).Name(rotary, (MetaDataComponent)null)}!", uid4, PopupType.MediumCaution);
			}
		}
	}
}
