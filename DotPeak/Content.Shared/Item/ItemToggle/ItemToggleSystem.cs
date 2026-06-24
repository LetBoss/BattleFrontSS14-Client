// Decompiled with JetBrains decompiler
// Type: Content.Shared.Item.ItemToggle.ItemToggleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Popups;
using Content.Shared.Temperature;
using Content.Shared.Toggleable;
using Content.Shared.Verbs;
using Content.Shared.Wieldable;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Item.ItemToggle;

public sealed class ItemToggleSystem : EntitySystem
{
  [Dependency]
  private INetManager _netManager;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedPopupSystem _popup;
  private Robust.Shared.GameObjects.EntityQuery<ItemToggleComponent> _query;

  public override void Initialize()
  {
    base.Initialize();
    this._query = this.GetEntityQuery<ItemToggleComponent>();
    this.SubscribeLocalEvent<ItemToggleComponent, ComponentStartup>(new EntityEventRefHandler<ItemToggleComponent, ComponentStartup>(this.OnStartup));
    this.SubscribeLocalEvent<ItemToggleComponent, MapInitEvent>(new EntityEventRefHandler<ItemToggleComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<ItemToggleComponent, ItemUnwieldedEvent>(new EntityEventRefHandler<ItemToggleComponent, ItemUnwieldedEvent>(this.TurnOffOnUnwielded));
    this.SubscribeLocalEvent<ItemToggleComponent, ItemWieldedEvent>(new EntityEventRefHandler<ItemToggleComponent, ItemWieldedEvent>(this.TurnOnOnWielded));
    this.SubscribeLocalEvent<ItemToggleComponent, UseInHandEvent>(new EntityEventRefHandler<ItemToggleComponent, UseInHandEvent>(this.OnUseInHand));
    this.SubscribeLocalEvent<ItemToggleComponent, GetVerbsEvent<ActivationVerb>>(new EntityEventRefHandler<ItemToggleComponent, GetVerbsEvent<ActivationVerb>>(this.OnActivateVerb));
    this.SubscribeLocalEvent<ItemToggleComponent, ActivateInWorldEvent>(new EntityEventRefHandler<ItemToggleComponent, ActivateInWorldEvent>(this.OnActivate));
    this.SubscribeLocalEvent<ItemToggleHotComponent, IsHotEvent>(new EntityEventRefHandler<ItemToggleHotComponent, IsHotEvent>(this.OnIsHotEvent));
    this.SubscribeLocalEvent<ItemToggleActiveSoundComponent, ItemToggledEvent>(new EntityEventRefHandler<ItemToggleActiveSoundComponent, ItemToggledEvent>(this.UpdateActiveSound));
  }

  private void OnStartup(Entity<ItemToggleComponent> ent, ref ComponentStartup args)
  {
    this.UpdateVisuals(ent);
  }

  private void OnMapInit(Entity<ItemToggleComponent> ent, ref MapInitEvent args)
  {
    if (!ent.Comp.Activated)
      return;
    ItemToggledEvent args1 = new ItemToggledEvent(ent.Comp.Predictable, ent.Comp.Activated, new EntityUid?());
    this.RaiseLocalEvent<ItemToggledEvent>((EntityUid) ent, ref args1);
  }

  private void OnUseInHand(Entity<ItemToggleComponent> ent, ref UseInHandEvent args)
  {
    if (args.Handled || !ent.Comp.OnUse)
      return;
    args.Handled = true;
    this.Toggle((Entity<ItemToggleComponent>) ((EntityUid) ent, ent.Comp), new EntityUid?(args.User), ent.Comp.Predictable);
  }

  private void OnActivateVerb(
    Entity<ItemToggleComponent> ent,
    ref GetVerbsEvent<ActivationVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || !ent.Comp.OnActivate)
      return;
    EntityUid user = args.User;
    if (ent.Comp.Activated)
    {
      ItemToggleActivateAttemptEvent args1 = new ItemToggleActivateAttemptEvent(new EntityUid?(args.User));
      this.RaiseLocalEvent<ItemToggleActivateAttemptEvent>(ent.Owner, ref args1);
      if (args1.Cancelled)
        return;
    }
    else
    {
      ItemToggleDeactivateAttemptEvent args2 = new ItemToggleDeactivateAttemptEvent(new EntityUid?(args.User));
      this.RaiseLocalEvent<ItemToggleDeactivateAttemptEvent>(ent.Owner, ref args2);
      if (args2.Cancelled)
        return;
    }
    SortedSet<ActivationVerb> verbs = args.Verbs;
    ActivationVerb activationVerb = new ActivationVerb();
    activationVerb.Text = !ent.Comp.Activated ? this.Loc.GetString(ent.Comp.VerbToggleOn) : this.Loc.GetString(ent.Comp.VerbToggleOff);
    activationVerb.Act = (Action) (() => this.Toggle((Entity<ItemToggleComponent>) (ent.Owner, ent.Comp), new EntityUid?(user), ent.Comp.Predictable));
    verbs.Add(activationVerb);
  }

  private void OnActivate(Entity<ItemToggleComponent> ent, ref ActivateInWorldEvent args)
  {
    if (args.Handled || !ent.Comp.OnActivate)
      return;
    args.Handled = true;
    this.Toggle((Entity<ItemToggleComponent>) (ent.Owner, ent.Comp), new EntityUid?(args.User), ent.Comp.Predictable);
  }

  public bool Toggle(Entity<ItemToggleComponent?> ent, EntityUid? user = null, bool predicted = true)
  {
    return this._query.Resolve((EntityUid) ent, ref ent.Comp, false) && this.TrySetActive(ent, !ent.Comp.Activated, user, predicted);
  }

  public bool TrySetActive(
    Entity<ItemToggleComponent?> ent,
    bool active,
    EntityUid? user = null,
    bool predicted = true)
  {
    return active ? this.TryActivate(ent, user, predicted) : this.TryDeactivate(ent, user, predicted);
  }

  public bool TryActivate(Entity<ItemToggleComponent?> ent, EntityUid? user = null, bool predicted = true)
  {
    if (!this._query.Resolve((EntityUid) ent, ref ent.Comp, false))
      return false;
    EntityUid owner = ent.Owner;
    ItemToggleComponent comp = ent.Comp;
    if (comp.Activated)
      return true;
    ItemToggleActivateAttemptEvent args = new ItemToggleActivateAttemptEvent(user);
    this.RaiseLocalEvent<ItemToggleActivateAttemptEvent>(owner, ref args);
    if (!comp.Predictable)
      predicted = false;
    if (!predicted && this._netManager.IsClient)
      return false;
    if (args.Cancelled)
    {
      if (args.Silent)
        return false;
      if (predicted)
        this._audio.PlayPredicted(comp.SoundFailToActivate, owner, user);
      else
        this._audio.PlayPvs(comp.SoundFailToActivate, owner);
      if (args.Popup != null && user.HasValue)
      {
        if (predicted)
          this._popup.PopupClient(args.Popup, owner, new EntityUid?(user.Value));
        else
          this._popup.PopupEntity(args.Popup, owner, user.Value);
      }
      return false;
    }
    this.Activate((Entity<ItemToggleComponent>) (owner, comp), predicted, user);
    return true;
  }

  public bool TryDeactivate(Entity<ItemToggleComponent?> ent, EntityUid? user = null, bool predicted = true)
  {
    if (!this._query.Resolve((EntityUid) ent, ref ent.Comp, false))
      return false;
    EntityUid owner = ent.Owner;
    ItemToggleComponent comp = ent.Comp;
    if (!comp.Activated)
      return true;
    if (!comp.Predictable)
      predicted = false;
    ItemToggleDeactivateAttemptEvent args = new ItemToggleDeactivateAttemptEvent(user);
    this.RaiseLocalEvent<ItemToggleDeactivateAttemptEvent>(owner, ref args);
    if (!predicted && this._netManager.IsClient)
      return false;
    if (args.Cancelled)
    {
      if (args.Silent || args.Popup == null || !user.HasValue)
        return false;
      if (predicted)
        this._popup.PopupClient(args.Popup, owner, new EntityUid?(user.Value));
      else
        this._popup.PopupEntity(args.Popup, owner, user.Value);
      return false;
    }
    this.Deactivate((Entity<ItemToggleComponent>) (owner, comp), predicted, user);
    return true;
  }

  private void Activate(Entity<ItemToggleComponent> ent, bool predicted, EntityUid? user = null)
  {
    (EntityUid entityUid, ItemToggleComponent comp) = ent;
    SoundSpecifier soundActivate = comp.SoundActivate;
    if (predicted)
      this._audio.PlayPredicted(soundActivate, entityUid, user);
    else
      this._audio.PlayPvs(soundActivate, entityUid);
    comp.Activated = true;
    this.UpdateVisuals((Entity<ItemToggleComponent>) (entityUid, comp));
    this.Dirty(entityUid, (IComponent) comp);
    ItemToggledEvent args = new ItemToggledEvent(predicted, true, user);
    this.RaiseLocalEvent<ItemToggledEvent>(entityUid, ref args);
  }

  private void Deactivate(Entity<ItemToggleComponent> ent, bool predicted, EntityUid? user = null)
  {
    (EntityUid entityUid, ItemToggleComponent comp) = ent;
    SoundSpecifier soundDeactivate = comp.SoundDeactivate;
    if (predicted)
      this._audio.PlayPredicted(soundDeactivate, entityUid, user);
    else
      this._audio.PlayPvs(soundDeactivate, entityUid);
    comp.Activated = false;
    this.UpdateVisuals((Entity<ItemToggleComponent>) (entityUid, comp));
    this.Dirty(entityUid, (IComponent) comp);
    ItemToggledEvent args = new ItemToggledEvent(predicted, false, user);
    this.RaiseLocalEvent<ItemToggledEvent>(entityUid, ref args);
  }

  public void SetOnActivate(Entity<ItemToggleComponent?> ent, bool val)
  {
    if (!this.Resolve<ItemToggleComponent>((EntityUid) ent, ref ent.Comp) || ent.Comp.OnActivate == val)
      return;
    ent.Comp.OnActivate = val;
    this.Dirty<ItemToggleComponent>(ent);
  }

  private void UpdateVisuals(Entity<ItemToggleComponent> ent)
  {
    AppearanceComponent comp;
    if (!this.TryComp<AppearanceComponent>((EntityUid) ent, out comp))
      return;
    this._appearance.SetData((EntityUid) ent, (Enum) ToggleableVisuals.Enabled, (object) ent.Comp.Activated, comp);
  }

  private void TurnOffOnUnwielded(Entity<ItemToggleComponent> ent, ref ItemUnwieldedEvent args)
  {
    this.TryDeactivate((Entity<ItemToggleComponent>) ((EntityUid) ent, ent.Comp), new EntityUid?(args.User));
  }

  private void TurnOnOnWielded(Entity<ItemToggleComponent> ent, ref ItemWieldedEvent args)
  {
    this.TryActivate((Entity<ItemToggleComponent>) ((EntityUid) ent, ent.Comp));
  }

  public bool IsActivated(Entity<ItemToggleComponent?> ent)
  {
    return !this._query.Resolve((EntityUid) ent, ref ent.Comp, false) || ent.Comp.Activated;
  }

  private void OnIsHotEvent(Entity<ItemToggleHotComponent> ent, ref IsHotEvent args)
  {
    args.IsHot |= this.IsActivated((Entity<ItemToggleComponent>) ent.Owner);
  }

  private void UpdateActiveSound(
    Entity<ItemToggleActiveSoundComponent> ent,
    ref ItemToggledEvent args)
  {
    (EntityUid entityUid, ItemToggleActiveSoundComponent comp) = ent;
    if (!args.Activated)
    {
      comp.PlayingStream = this._audio.Stop(comp.PlayingStream);
    }
    else
    {
      if (comp.ActiveSound == null || comp.PlayingStream.HasValue)
        return;
      AudioParams audioParams = comp.ActiveSound.Params.WithLoop(true);
      EntityUid? nullable = (args.Predicted ? this._audio.PlayPredicted(comp.ActiveSound, entityUid, args.User, new AudioParams?(audioParams)) : this._audio.PlayPvs(comp.ActiveSound, entityUid, new AudioParams?(audioParams))).Item1;
      if (!nullable.HasValue)
        return;
      EntityUid valueOrDefault = nullable.GetValueOrDefault();
      comp.PlayingStream = new EntityUid?(valueOrDefault);
    }
  }
}
