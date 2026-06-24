// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Telephone.SharedRMCTelephoneSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
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
  private static readonly SoundSpecifier RemotePickupSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Phone/remote_pickup.ogg");
  private static readonly SoundSpecifier RemoteHangupSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Phone/remote_hangup.ogg");
  private static readonly SoundSpecifier BusySound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Phone/phone_busy.ogg");

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RotaryPhoneComponent, MapInitEvent>(new EntityEventRefHandler<RotaryPhoneComponent, MapInitEvent>(this.OnRotaryPhoneMapInit));
    this.SubscribeLocalEvent<RotaryPhoneComponent, BeforeActivatableUIOpenEvent>(new EntityEventRefHandler<RotaryPhoneComponent, BeforeActivatableUIOpenEvent>(this.OnRotaryPhoneBeforeOpen));
    this.SubscribeLocalEvent<RotaryPhoneComponent, ComponentShutdown>(new EntityEventRefHandler<RotaryPhoneComponent, ComponentShutdown>(this.OnRotaryPhoneTerminating<ComponentShutdown>));
    this.SubscribeLocalEvent<RotaryPhoneComponent, EntityTerminatingEvent>(new EntityEventRefHandler<RotaryPhoneComponent, EntityTerminatingEvent>(this.OnRotaryPhoneTerminating<EntityTerminatingEvent>));
    this.SubscribeLocalEvent<RotaryPhoneDialingComponent, InteractUsingEvent>(new EntityEventRefHandler<RotaryPhoneDialingComponent, InteractUsingEvent>(this.OnRotaryPhoneDialingInteractUsing));
    this.SubscribeLocalEvent<RotaryPhoneReceivingComponent, InteractHandEvent>(new EntityEventRefHandler<RotaryPhoneReceivingComponent, InteractHandEvent>(this.OnRotaryPhoneReceivingInteractHand), new Type[1]
    {
      typeof (ActivatableUISystem)
    });
    this.SubscribeLocalEvent<RotaryPhoneReceivingComponent, InteractUsingEvent>(new EntityEventRefHandler<RotaryPhoneReceivingComponent, InteractUsingEvent>(this.OnRotaryPhoneReceivingInteractUsing));
    this.SubscribeLocalEvent<RMCTelephoneComponent, ComponentShutdown>(new EntityEventRefHandler<RMCTelephoneComponent, ComponentShutdown>(this.OnTelephoneTerminating<ComponentShutdown>));
    this.SubscribeLocalEvent<RMCTelephoneComponent, EntityTerminatingEvent>(new EntityEventRefHandler<RMCTelephoneComponent, EntityTerminatingEvent>(this.OnTelephoneTerminating<EntityTerminatingEvent>));
    this.SubscribeLocalEvent<RotaryPhoneBackpackComponent, GetItemActionsEvent>(new EntityEventRefHandler<RotaryPhoneBackpackComponent, GetItemActionsEvent>(this.OnBackpackGetItemActions));
    this.SubscribeLocalEvent<RotaryPhoneBackpackComponent, RMCTelephoneActionEvent>(new EntityEventRefHandler<RotaryPhoneBackpackComponent, RMCTelephoneActionEvent>(this.OnBackpackTelephoneAction));
    this.Subs.BuiEvents<RotaryPhoneComponent>((object) RMCTelephoneUiKey.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<RotaryPhoneComponent>) (subs =>
    {
      subs.Event<RMCTelephoneCallBuiMsg>(new EntityEventRefHandler<RotaryPhoneComponent, RMCTelephoneCallBuiMsg>(this.OnTelephoneCallMsg));
      subs.Event<RMCTelephoneDndBuiMsg>(new EntityEventRefHandler<RotaryPhoneComponent, RMCTelephoneDndBuiMsg>(this.OnTelephoneDndMsg));
    }));
  }

  private void OnRotaryPhoneMapInit(Entity<RotaryPhoneComponent> ent, ref MapInitEvent args)
  {
    this._container.EnsureContainer<ContainerSlot>((EntityUid) ent, ent.Comp.ContainerId);
    EntityUid? uid;
    if (!this.TrySpawnInContainer((string) ent.Comp.PhoneId, (EntityUid) ent, ent.Comp.ContainerId, out uid))
      return;
    ent.Comp.Phone = new EntityUid?(uid.Value);
    this.Dirty<RotaryPhoneComponent>(ent);
    RMCTelephoneComponent comp;
    if (!this.TryComp<RMCTelephoneComponent>(uid, out comp))
      return;
    comp.RotaryPhone = new EntityUid?((EntityUid) ent);
    this.Dirty(uid.Value, (IComponent) comp);
  }

  private void OnRotaryPhoneBeforeOpen(
    Entity<RotaryPhoneComponent> ent,
    ref BeforeActivatableUIOpenEvent args)
  {
    this.SendUIState((EntityUid) ent);
  }

  private void OnRotaryPhoneTerminating<T>(Entity<RotaryPhoneComponent> ent, ref T args)
  {
    RMCTelephoneComponent comp;
    if (!this.TryComp<RMCTelephoneComponent>(ent.Comp.Phone, out comp))
      return;
    comp.RotaryPhone = new EntityUid?();
    this.Dirty(ent.Comp.Phone.Value, (IComponent) comp);
  }

  private void OnRotaryPhoneDialingInteractUsing(
    Entity<RotaryPhoneDialingComponent> ent,
    ref InteractUsingEvent args)
  {
    if (!this.HangUpDialing(ent, args.Used, new EntityUid?(args.User)))
      return;
    args.Handled = true;
  }

  private void OnRotaryPhoneReceivingInteractHand(
    Entity<RotaryPhoneReceivingComponent> ent,
    ref InteractHandEvent args)
  {
    if (this._timing.ApplyingState)
      return;
    args.Handled = true;
    this.PickupReceiving(ent, args.User);
  }

  private void OnRotaryPhoneReceivingInteractUsing(
    Entity<RotaryPhoneReceivingComponent> ent,
    ref InteractUsingEvent args)
  {
    if (!this.HangUpReceiving(ent, args.Used, new EntityUid?(args.User)))
      return;
    args.Handled = true;
  }

  private void OnTelephoneTerminating<T>(Entity<RMCTelephoneComponent> ent, ref T args)
  {
    RotaryPhoneComponent comp;
    if (!this.TryComp<RotaryPhoneComponent>(ent.Comp.RotaryPhone, out comp))
      return;
    comp.Phone = new EntityUid?();
    this.Dirty(ent.Comp.RotaryPhone.Value, (IComponent) comp);
  }

  private void OnBackpackGetItemActions(
    Entity<RotaryPhoneBackpackComponent> ent,
    ref GetItemActionsEvent args)
  {
    SlotFlags? slotFlags1 = args.SlotFlags;
    SlotFlags slot = ent.Comp.Slot;
    SlotFlags? nullable = slotFlags1.HasValue ? new SlotFlags?(slotFlags1.GetValueOrDefault() & slot) : new SlotFlags?();
    SlotFlags slotFlags2 = SlotFlags.NONE;
    if (nullable.GetValueOrDefault() == slotFlags2 & nullable.HasValue && !args.InHands)
      return;
    args.AddAction(ref ent.Comp.Action, (string) ent.Comp.ActionId, (EntityUid) ent);
  }

  private void OnBackpackTelephoneAction(
    Entity<RotaryPhoneBackpackComponent> ent,
    ref RMCTelephoneActionEvent args)
  {
    args.Handled = true;
    if (this.HasComp<RotaryPhoneDialingComponent>((EntityUid) ent))
      return;
    RotaryPhoneReceivingComponent comp;
    if (this.TryComp<RotaryPhoneReceivingComponent>((EntityUid) ent, out comp))
    {
      this.PickupReceiving((Entity<RotaryPhoneReceivingComponent>) ((EntityUid) ent, comp), args.Performer);
    }
    else
    {
      this.SendUIState((EntityUid) ent);
      this._ui.TryOpenUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) RMCTelephoneUiKey.Key, args.Performer);
    }
  }

  private void OnTelephoneCallMsg(Entity<RotaryPhoneComponent> ent, ref RMCTelephoneCallBuiMsg args)
  {
    TimeSpan curTime = this._timing.CurTime;
    if (curTime < ent.Comp.LastCall + ent.Comp.CallCooldown)
      return;
    this._ui.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) RMCTelephoneUiKey.Key);
    if (this._net.IsClient)
      return;
    EntityUid entity = this.GetEntity(args.Id);
    RotaryPhoneComponent comp1;
    if (!entity.Valid || ent.Owner == entity || !this.TryComp<RotaryPhoneComponent>(entity, out comp1) || this.HasComp<RotaryPhoneDialingComponent>((EntityUid) ent))
      return;
    EntityUid actor = args.Actor;
    if (this.IsPhoneBusy(entity))
      this._popup.PopupEntity("That phone is busy!", actor, actor, PopupType.MediumCaution);
    else if (this.HasComp<RotaryPhoneBackpackComponent>(entity) && !this.TryGetPhoneBackpackHolder(entity, out EntityUid _))
    {
      this._popup.PopupEntity("No transmitters could be located to call!", actor, actor, PopupType.MediumCaution);
    }
    else
    {
      MetaDataComponent comp2;
      MetaDataComponent comp3;
      if (this.HasComp<MarineComponent>(actor) && this.TryComp(actor, out comp2) && this.TryComp((EntityUid) ent, out comp3))
        this._popup.PopupEntity($"{comp2.EntityName} dials a number on the {comp3.EntityName}.", (EntityUid) ent);
      ent.Comp.Idle = false;
      ent.Comp.LastCall = curTime;
      this.Dirty<RotaryPhoneComponent>(ent);
      RotaryPhoneDialingComponent dialingComponent = this.EnsureComp<RotaryPhoneDialingComponent>((EntityUid) ent);
      dialingComponent.Other = new EntityUid?(entity);
      this.Dirty((EntityUid) ent, (IComponent) dialingComponent);
      RotaryPhoneReceivingComponent receivingComponent = this.EnsureComp<RotaryPhoneReceivingComponent>(entity);
      receivingComponent.Other = new EntityUid?((EntityUid) ent);
      this.Dirty(entity, (IComponent) receivingComponent);
      if (this._net.IsServer)
      {
        SoundSpecifier dialingSound = ent.Comp.DialingSound;
        AudioParams audioParams;
        if (dialingSound != null)
        {
          AmbientSoundComponent ambience1 = this.EnsureComp<AmbientSoundComponent>((EntityUid) ent);
          this._ambientSound.SetSound((EntityUid) ent, dialingSound, ambience1);
          this._ambientSound.SetRange((EntityUid) ent, 16f, ambience1);
          SharedAmbientSoundSystem ambientSound = this._ambientSound;
          EntityUid uid = (EntityUid) ent;
          audioParams = dialingSound.Params;
          double volume = (double) audioParams.Volume;
          AmbientSoundComponent ambience2 = ambience1;
          ambientSound.SetVolume(uid, (float) volume, ambience2);
          this._ambientSound.SetAmbience((EntityUid) ent, true, ambience1);
        }
        SoundSpecifier receivingSound = ent.Comp.ReceivingSound;
        if (receivingSound != null)
        {
          AmbientSoundComponent ambience3 = this.EnsureComp<AmbientSoundComponent>(entity);
          this._ambientSound.SetSound(entity, receivingSound, ambience3);
          this._ambientSound.SetRange(entity, 16f, ambience3);
          SharedAmbientSoundSystem ambientSound = this._ambientSound;
          EntityUid uid = entity;
          audioParams = receivingSound.Params;
          double volume = (double) audioParams.Volume;
          AmbientSoundComponent ambience4 = ambience3;
          ambientSound.SetVolume(uid, (float) volume, ambience4);
          this._ambientSound.SetAmbience(entity, true, ambience3);
          RMCTelephoneRingEvent message = new RMCTelephoneRingEvent(entity, (EntityUid) ent, args.Actor);
          this.RaiseLocalEvent<RMCTelephoneRingEvent>(ref message);
        }
      }
      EntityUid? phone = ent.Comp.Phone;
      if (phone.HasValue)
      {
        EntityUid valueOrDefault = phone.GetValueOrDefault();
        this.PickupPhone(ent, valueOrDefault, actor);
      }
      this.UpdateAppearance((Entity<RotaryPhoneComponent>) ((EntityUid) ent, (RotaryPhoneComponent) ent));
      this.UpdateAppearance((Entity<RotaryPhoneComponent>) (entity, comp1));
      ISharedAdminLogManager adminLog = this._adminLog;
      LogStringHandler logStringHandler = new LogStringHandler(24, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "ToPrettyString(args.Actor)");
      logStringHandler.AppendLiteral(" started calling ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entity), "ToPrettyString(target)");
      logStringHandler.AppendLiteral(" using ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) ent)), "ToPrettyString(ent)");
      ref LogStringHandler local = ref logStringHandler;
      adminLog.Add(LogType.RMCTelephone, ref local);
    }
  }

  private void OnTelephoneDndMsg(Entity<RotaryPhoneComponent> ent, ref RMCTelephoneDndBuiMsg args)
  {
    if (args.Dnd && ent.Comp.CanDnd)
      this.EnsureComp<RotaryPhoneDndComponent>((EntityUid) ent);
    else
      this.RemComp<RotaryPhoneDndComponent>((EntityUid) ent);
    this.SendUIState((EntityUid) ent);
  }

  private bool IsPhoneBusy(EntityUid ent)
  {
    return this.HasComp<RotaryPhoneDialingComponent>(ent) || this.HasComp<RotaryPhoneReceivingComponent>(ent) || this.HasComp<RotaryPhoneDndComponent>(ent);
  }

  private void UpdateAppearance(Entity<RotaryPhoneComponent?> phone, bool forceNotRinging = false)
  {
    if (!this.Resolve<RotaryPhoneComponent>((EntityUid) phone, ref phone.Comp, false))
      return;
    RotaryPhoneVisuals rotaryPhoneVisuals = RotaryPhoneVisuals.Base;
    BaseContainer container;
    if (!this._container.TryGetContainer((EntityUid) phone, phone.Comp.ContainerId, out container) || container.ContainedEntities.Count == 0)
      rotaryPhoneVisuals = RotaryPhoneVisuals.Ear;
    else if (this.HasComp<RotaryPhoneReceivingComponent>((EntityUid) phone) && !forceNotRinging)
      rotaryPhoneVisuals = RotaryPhoneVisuals.Ring;
    this._appearance.SetData((EntityUid) phone, (Enum) RotaryPhoneLayers.Layer, (object) rotaryPhoneVisuals);
  }

  protected virtual void PickupPhone(
    Entity<RotaryPhoneComponent> rotary,
    EntityUid telephone,
    EntityUid user)
  {
    BaseContainer container;
    if (this._container.TryGetContainer((EntityUid) rotary, rotary.Comp.ContainerId, out container))
      this._container.Remove((Entity<TransformComponent, MetaDataComponent>) telephone, container);
    this._hands.TryPickupAnyHand(user, telephone);
    this.EnsureComp<RMCPickedUpPhoneComponent>(telephone);
    this.PlayGrabSound((EntityUid) rotary);
  }

  private void ReturnPhone(EntityUid rotary, EntityUid telephone, EntityUid? user)
  {
    RotaryPhoneComponent comp;
    if (!this.TryComp<RotaryPhoneComponent>(rotary, out comp))
      return;
    EntityUid? phone = comp.Phone;
    EntityUid entityUid = telephone;
    BaseContainer container;
    if ((phone.HasValue ? (phone.GetValueOrDefault() != entityUid ? 1 : 0) : 1) != 0 || !this._container.TryGetContainer(rotary, comp.ContainerId, out container))
      return;
    if (user.HasValue)
    {
      if (!this._hands.TryDropIntoContainer((Entity<HandsComponent>) user.Value, telephone, container))
        return;
      this.PlayGrabSound(rotary);
    }
    else
    {
      if (!this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) telephone, container))
        return;
      this.PlayGrabSound(rotary);
    }
  }

  private void HangUp(EntityUid self, EntityUid other, EntityUid? user)
  {
    this.StopSound(self);
    if (!this.HasComp<RotaryPhoneDialingComponent>(other) && !this.HasComp<RotaryPhoneReceivingComponent>(other))
    {
      this.StopSound(other);
    }
    else
    {
      if (this._net.IsServer)
      {
        this._ambientSound.SetSound(other, SharedRMCTelephoneSystem.BusySound);
        this._ambientSound.SetVolume(other, SharedRMCTelephoneSystem.BusySound.Params.Volume);
        this._ambientSound.SetAmbience(other, true);
      }
      if (!this.HasPickedUp((Entity<RotaryPhoneComponent, RotaryPhoneReceivingComponent>) other))
        return;
      if (this._net.IsServer)
        this._audio.PlayPvs(SharedRMCTelephoneSystem.RemoteHangupSound, other);
      ISharedAdminLogManager adminLog = this._adminLog;
      LogStringHandler logStringHandler = new LogStringHandler(24, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(user), "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" hung up ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) self), "ToPrettyString(self)");
      logStringHandler.AppendLiteral(" while calling ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) other), "ToPrettyString(other)");
      ref LogStringHandler local = ref logStringHandler;
      adminLog.Add(LogType.RMCTelephone, ref local);
    }
  }

  private void StopSound(EntityUid ent) => this._ambientSound.SetAmbience(ent, false);

  private void PlayGrabSound(EntityUid rotary)
  {
    RotaryPhoneComponent comp;
    if (this._net.IsClient || !this.TryComp<RotaryPhoneComponent>(rotary, out comp))
      return;
    this._audio.PlayPvs(comp.GrabSound, rotary);
    this._audio.Stop(comp.VoicemailSoundEntity);
  }

  protected bool TryGetOtherPhone(EntityUid rotary, out EntityUid other)
  {
    RotaryPhoneDialingComponent comp1;
    RotaryPhoneComponent comp2;
    if (this.TryComp<RotaryPhoneDialingComponent>(rotary, out comp1) && this.TryComp<RotaryPhoneComponent>(comp1.Other, out comp2) && comp2.Phone.HasValue)
    {
      other = comp2.Phone.Value;
      return true;
    }
    RotaryPhoneReceivingComponent comp3;
    if (this.TryComp<RotaryPhoneReceivingComponent>(rotary, out comp3) && this.TryComp<RotaryPhoneComponent>(comp3.Other, out comp2) && comp2.Phone.HasValue)
    {
      other = comp2.Phone.Value;
      return true;
    }
    other = new EntityUid();
    return false;
  }

  private bool IsCorrectPhone(Entity<RotaryPhoneComponent?> rotary, EntityUid phone)
  {
    if (!this.Resolve<RotaryPhoneComponent>((EntityUid) rotary, ref rotary.Comp, false))
      return false;
    EntityUid? phone1 = rotary.Comp.Phone;
    EntityUid entityUid = phone;
    return phone1.HasValue && phone1.GetValueOrDefault() == entityUid;
  }

  private bool HasPickedUp(
    Entity<RotaryPhoneComponent?, RotaryPhoneReceivingComponent?> receiving)
  {
    BaseContainer container;
    return this.Resolve<RotaryPhoneComponent, RotaryPhoneReceivingComponent>((EntityUid) receiving, ref receiving.Comp1, ref receiving.Comp2, false) && this._container.TryGetContainer((EntityUid) receiving, receiving.Comp1.ContainerId, out container) && container.ContainedEntities.Count == 0;
  }

  private bool TryGetPhoneBackpackHolder(EntityUid backpack, out EntityUid holder)
  {
    holder = new EntityUid();
    BaseContainer container;
    if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (backpack, (TransformComponent) null, (MetaDataComponent) null), out container) || !this.HasComp<InventoryComponent>(container.Owner))
      return false;
    holder = container.Owner;
    return true;
  }

  private void SendUIState(EntityUid phone)
  {
    if (this._net.IsClient)
      return;
    List<RMCPhone> phones = new List<RMCPhone>();
    Robust.Shared.GameObjects.EntityQueryEnumerator<RotaryPhoneComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RotaryPhoneComponent>();
    EntityUid uid;
    RotaryPhoneComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(uid == phone))
      {
        string phoneName = this.GetPhoneName((Entity<RotaryPhoneComponent>) (uid, comp1));
        phones.Add(new RMCPhone(this.GetNetEntity(uid), comp1.Category, phoneName));
      }
    }
    bool canDnd = this.Comp<RotaryPhoneComponent>(phone).CanDnd;
    bool dnd = this.HasComp<RotaryPhoneDndComponent>(phone);
    this._ui.SetUiState((Entity<UserInterfaceComponent>) phone, (Enum) RMCTelephoneUiKey.Key, (BoundUserInterfaceState) new RMCTelephoneBuiState(phones, canDnd, dnd));
  }

  private void PickupReceiving(Entity<RotaryPhoneReceivingComponent> receiving, EntityUid user)
  {
    RotaryPhoneComponent comp;
    if (this.TryComp<RotaryPhoneComponent>((EntityUid) receiving, out comp))
    {
      EntityUid? phone = comp.Phone;
      if (phone.HasValue)
      {
        EntityUid valueOrDefault = phone.GetValueOrDefault();
        this.PickupPhone((Entity<RotaryPhoneComponent>) ((EntityUid) receiving, comp), valueOrDefault, user);
      }
    }
    this.StopSound((EntityUid) receiving);
    EntityUid? other = receiving.Comp.Other;
    if (other.HasValue)
    {
      EntityUid valueOrDefault = other.GetValueOrDefault();
      this.StopSound(valueOrDefault);
      if (this._net.IsServer)
        this._audio.PlayPvs(SharedRMCTelephoneSystem.RemotePickupSound, valueOrDefault);
    }
    this.UpdateAppearance((Entity<RotaryPhoneComponent>) ((EntityUid) receiving, comp));
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(11, 2);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
    logStringHandler.AppendLiteral(" picked up ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) receiving)), "ToPrettyString(receiving)");
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.RMCTelephone, ref local);
  }

  protected string GetPhoneName(Entity<RotaryPhoneComponent?> phone)
  {
    string phoneName1 = this.Name((EntityUid) phone);
    EntityUid holder;
    if (!this.Resolve<RotaryPhoneComponent>((EntityUid) phone, ref phone.Comp, false) || !phone.Comp.TryGetHolderName || !this.TryGetPhoneBackpackHolder((EntityUid) phone, out holder))
      return phoneName1;
    string phoneName2 = this.Name(holder);
    JobPrefixComponent comp;
    if (this.TryComp<JobPrefixComponent>(holder, out comp))
      phoneName2 = $"{this.Loc.GetString((string) comp.Prefix)} {phoneName2}";
    Entity<SquadTeamComponent> squad;
    if (this._squad.TryGetMemberSquad((Entity<SquadMemberComponent>) holder, out squad))
      phoneName2 = $"{phoneName2} ({this.Name((EntityUid) squad)})";
    return phoneName2;
  }

  private bool HangUpDialing(
    Entity<RotaryPhoneDialingComponent> ent,
    EntityUid phone,
    EntityUid? user)
  {
    if (!this.IsCorrectPhone((Entity<RotaryPhoneComponent>) ent.Owner, phone))
      return false;
    this.RemCompDeferred<RotaryPhoneDialingComponent>((EntityUid) ent);
    this.ReturnPhone(ent.Owner, phone, user);
    this.StopSound(ent.Owner);
    EntityUid? other = ent.Comp.Other;
    if (other.HasValue)
    {
      EntityUid valueOrDefault = other.GetValueOrDefault();
      this.StopSound(valueOrDefault);
      this.HangUp((EntityUid) ent, valueOrDefault, user);
      if (!this.HasPickedUp((Entity<RotaryPhoneComponent, RotaryPhoneReceivingComponent>) valueOrDefault))
      {
        this.RemCompDeferred<RotaryPhoneReceivingComponent>(valueOrDefault);
        this.StopSound(valueOrDefault);
      }
      this.UpdateAppearance((Entity<RotaryPhoneComponent>) valueOrDefault, true);
    }
    this.UpdateAppearance((Entity<RotaryPhoneComponent>) ent.Owner, true);
    return true;
  }

  private bool HangUpReceiving(
    Entity<RotaryPhoneReceivingComponent> ent,
    EntityUid used,
    EntityUid? user)
  {
    if (!this.IsCorrectPhone((Entity<RotaryPhoneComponent>) ent.Owner, used))
      return false;
    this.RemCompDeferred<RotaryPhoneReceivingComponent>((EntityUid) ent);
    this.ReturnPhone(ent.Owner, used, user);
    EntityUid? other = ent.Comp.Other;
    if (other.HasValue)
    {
      EntityUid valueOrDefault = other.GetValueOrDefault();
      RotaryPhoneDialingComponent comp;
      if (this.TryComp<RotaryPhoneDialingComponent>(valueOrDefault, out comp))
      {
        comp.Other = new EntityUid?();
        this.Dirty(valueOrDefault, (IComponent) comp);
      }
      this.HangUp((EntityUid) ent, valueOrDefault, user);
      if (!this.HasPickedUp((Entity<RotaryPhoneComponent, RotaryPhoneReceivingComponent>) valueOrDefault))
        this.RemCompDeferred<RotaryPhoneReceivingComponent>(valueOrDefault);
    }
    this.UpdateAppearance((Entity<RotaryPhoneComponent>) ent.Owner, true);
    return true;
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<RotaryPhoneDialingComponent, RotaryPhoneComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<RotaryPhoneDialingComponent, RotaryPhoneComponent>();
    EntityUid uid1;
    RotaryPhoneDialingComponent comp1_1;
    RotaryPhoneComponent comp2_1;
    EntityUid? nullable1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1, out comp2_1))
    {
      if (comp2_1.Phone.HasValue)
      {
        if (curTime > comp1_1.LastVoicemail + comp2_1.VoicemailTimeoutDelay && comp1_1.DidVoicemail && !comp1_1.DidVoicemailTimeout)
        {
          comp1_1.DidVoicemailTimeout = true;
          this.Dirty(uid1, (IComponent) comp1_1);
          this._ambientSound.SetSound(uid1, SharedRMCTelephoneSystem.BusySound);
          this._ambientSound.SetVolume(uid1, SharedRMCTelephoneSystem.BusySound.Params.Volume);
          this._ambientSound.SetAmbience(uid1, true);
        }
        nullable1 = comp1_1.Other;
        if (nullable1.HasValue)
        {
          EntityUid valueOrDefault = nullable1.GetValueOrDefault();
          RotaryPhoneReceivingComponent comp1;
          RotaryPhoneComponent comp2;
          if (this.TryComp<RotaryPhoneReceivingComponent>(valueOrDefault, out comp1) && this.TryComp<RotaryPhoneComponent>(valueOrDefault, out comp2) && comp2.Phone.HasValue && !this.HasPickedUp((Entity<RotaryPhoneComponent, RotaryPhoneReceivingComponent>) valueOrDefault))
          {
            if (comp2_1.Idle)
            {
              if (curTime > comp2_1.LastCall + comp2_1.VoicemailDelay && !comp1_1.DidVoicemail)
              {
                Entity<RotaryPhoneReceivingComponent> ent = (Entity<RotaryPhoneReceivingComponent>) (valueOrDefault, comp1);
                EntityUid used = comp2.Phone.Value;
                nullable1 = new EntityUid?();
                EntityUid? user = nullable1;
                if (this.HangUpReceiving(ent, used, user))
                {
                  this.StopSound(valueOrDefault);
                  this.StopSound(uid1);
                }
                RotaryPhoneComponent rotaryPhoneComponent = comp2_1;
                (EntityUid, AudioComponent)? nullable2 = this._audio.PlayPvs(comp2_1.VoicemailSound, comp2_1.Phone.Value);
                ref (EntityUid, AudioComponent)? local = ref nullable2;
                EntityUid? nullable3;
                if (!local.HasValue)
                {
                  nullable1 = new EntityUid?();
                  nullable3 = nullable1;
                }
                else
                  nullable3 = new EntityUid?(local.GetValueOrDefault().Item1);
                rotaryPhoneComponent.VoicemailSoundEntity = nullable3;
                comp1_1.DidVoicemail = true;
                comp1_1.LastVoicemail = curTime;
                this.Dirty(uid1, (IComponent) comp1_1);
                this.Dirty(uid1, (IComponent) comp2_1);
              }
            }
            else if (curTime > comp2_1.LastCall + comp2_1.DialingIdleDelay)
            {
              SoundSpecifier dialingIdleSound = comp2_1.DialingIdleSound;
              if (dialingIdleSound != null)
              {
                comp2_1.Idle = true;
                this.Dirty(uid1, (IComponent) comp2_1);
                this._ambientSound.SetSound(uid1, dialingIdleSound);
                this._ambientSound.SetVolume(uid1, dialingIdleSound.Params.Volume);
                this._ambientSound.SetAmbience(uid1, true);
              }
            }
          }
        }
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCPickedUpPhoneComponent, RMCTelephoneComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<RMCPickedUpPhoneComponent, RMCTelephoneComponent>();
    RMCPickedUpPhoneComponent comp1_2;
    RMCTelephoneComponent comp2_2;
    EntityUid uid;
    while (entityQueryEnumerator2.MoveNext(out uid, out comp1_2, out comp2_2))
    {
      nullable1 = comp2_2.RotaryPhone;
      if (nullable1.HasValue)
      {
        EntityUid rotary = nullable1.GetValueOrDefault();
        float distance;
        if (!this._transform.GetMoverCoordinates(uid).TryDistance((IEntityManager) this.EntityManager, this._transform, this._transform.GetMoverCoordinates(rotary), out distance) || (double) distance > (double) comp1_2.Range)
        {
          RotaryPhoneDialingComponent comp3;
          if (this.TryComp<RotaryPhoneDialingComponent>(rotary, out comp3))
          {
            Entity<RotaryPhoneDialingComponent> ent = (Entity<RotaryPhoneDialingComponent>) (rotary, comp3);
            EntityUid phone = uid;
            nullable1 = new EntityUid?();
            EntityUid? user = nullable1;
            if (this.HangUpDialing(ent, phone, user))
              PhoneSnapBackPopup();
          }
          else
          {
            RotaryPhoneReceivingComponent comp4;
            if (this.TryComp<RotaryPhoneReceivingComponent>(rotary, out comp4))
            {
              Entity<RotaryPhoneReceivingComponent> ent = (Entity<RotaryPhoneReceivingComponent>) (rotary, comp4);
              EntityUid used = uid;
              nullable1 = new EntityUid?();
              EntityUid? user = nullable1;
              if (this.HangUpReceiving(ent, used, user))
                PhoneSnapBackPopup();
            }
          }
        }

        void PhoneSnapBackPopup()
        {
          this._popup.PopupEntity($"The {this.Name(uid)} snaps back to the {this.Name(rotary)}!", uid, PopupType.MediumCaution);
        }
      }
    }
  }
}
