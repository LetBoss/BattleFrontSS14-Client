// Decompiled with JetBrains decompiler
// Type: Content.Shared.Radio.EntitySystems.EncryptionKeySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chat;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Radio.Components;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Content.Shared.Wires;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Radio.EntitySystems;

public sealed class EncryptionKeySystem : EntitySystem
{
  [Robust.Shared.IoC.Dependency]
  private IPrototypeManager _protoManager;
  [Robust.Shared.IoC.Dependency]
  private SharedToolSystem _tool;
  [Robust.Shared.IoC.Dependency]
  private SharedPopupSystem _popup;
  [Robust.Shared.IoC.Dependency]
  private SharedContainerSystem _container;
  [Robust.Shared.IoC.Dependency]
  private SharedAudioSystem _audio;
  [Robust.Shared.IoC.Dependency]
  private SharedHandsSystem _hands;
  [Robust.Shared.IoC.Dependency]
  private SharedWiresSystem _wires;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<EncryptionKeyComponent, ExaminedEvent>(new ComponentEventHandler<EncryptionKeyComponent, ExaminedEvent>(this.OnKeyExamined));
    this.SubscribeLocalEvent<EncryptionKeyHolderComponent, ExaminedEvent>(new ComponentEventHandler<EncryptionKeyHolderComponent, ExaminedEvent>(this.OnHolderExamined));
    this.SubscribeLocalEvent<EncryptionKeyHolderComponent, ComponentStartup>(new ComponentEventHandler<EncryptionKeyHolderComponent, ComponentStartup>(this.OnStartup));
    this.SubscribeLocalEvent<EncryptionKeyHolderComponent, InteractUsingEvent>(new ComponentEventHandler<EncryptionKeyHolderComponent, InteractUsingEvent>(this.OnInteractUsing));
    this.SubscribeLocalEvent<EncryptionKeyHolderComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<EncryptionKeyHolderComponent, EntInsertedIntoContainerMessage>(this.OnContainerModified));
    this.SubscribeLocalEvent<EncryptionKeyHolderComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<EncryptionKeyHolderComponent, EntRemovedFromContainerMessage>(this.OnContainerModified));
    this.SubscribeLocalEvent<EncryptionKeyHolderComponent, EncryptionKeySystem.EncryptionRemovalFinishedEvent>(new ComponentEventHandler<EncryptionKeyHolderComponent, EncryptionKeySystem.EncryptionRemovalFinishedEvent>(this.OnKeyRemoval));
  }

  private void OnKeyRemoval(
    EntityUid uid,
    EncryptionKeyHolderComponent component,
    EncryptionKeySystem.EncryptionRemovalFinishedEvent args)
  {
    if (args.Cancelled)
      return;
    EntityUid[] array = component.KeyContainer.ContainedEntities.ToArray<EntityUid>();
    this._container.EmptyContainer((BaseContainer) component.KeyContainer, reparent: false);
    foreach (EntityUid entity in array)
      this._hands.PickupOrDrop(new EntityUid?(args.User), entity, dropNear: true);
    this._popup.PopupPredicted(this.Loc.GetString("encryption-keys-all-extracted"), uid, new EntityUid?(args.User));
    this._audio.PlayPredicted(component.KeyExtractionSound, uid, new EntityUid?(args.User));
  }

  public void UpdateChannels(EntityUid uid, EncryptionKeyHolderComponent component)
  {
    if (!component.Initialized)
      return;
    component.Channels.Clear();
    component.DefaultChannel = (string) null;
    component.ReadOnlyChannels.Clear();
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) component.KeyContainer.ContainedEntities)
    {
      EncryptionKeyComponent comp;
      if (this.TryComp<EncryptionKeyComponent>(containedEntity, out comp))
      {
        component.Channels.UnionWith((IEnumerable<string>) comp.Channels);
        component.ReadOnlyChannels.UnionWith((IEnumerable<ProtoId<RadioChannelPrototype>>) comp.ReadOnlyChannels);
        EncryptionKeyHolderComponent keyHolderComponent = component;
        if (keyHolderComponent.DefaultChannel == null)
          keyHolderComponent.DefaultChannel = comp.DefaultChannel;
      }
    }
    this.RaiseLocalEvent<EncryptionChannelsChangedEvent>(uid, new EncryptionChannelsChangedEvent(component));
    this.Dirty(uid, (IComponent) component);
  }

  private void OnContainerModified(
    EntityUid uid,
    EncryptionKeyHolderComponent component,
    ContainerModifiedMessage args)
  {
    if (!(args.Container.ID == "key_slots"))
      return;
    this.UpdateChannels(uid, component);
  }

  private void OnInteractUsing(
    EntityUid uid,
    EncryptionKeyHolderComponent component,
    InteractUsingEvent args)
  {
    if (args.Handled)
      return;
    if (this.HasComp<EncryptionKeyComponent>(args.Used))
    {
      args.Handled = true;
      this.TryInsertKey(uid, component, args);
    }
    else
    {
      ToolComponent comp;
      if (!this.TryComp<ToolComponent>(args.Used, out comp) || !this._tool.HasQuality(args.Used, component.KeysExtractionMethod, comp) || component.KeyContainer.ContainedEntities.Count <= 0)
        return;
      args.Handled = true;
      this.TryRemoveKey(uid, component, args, comp);
    }
  }

  private void TryInsertKey(
    EntityUid uid,
    EncryptionKeyHolderComponent component,
    InteractUsingEvent args)
  {
    if (!component.KeysUnlocked)
    {
      this._popup.PopupClient(this.Loc.GetString("encryption-keys-are-locked"), uid, new EntityUid?(args.User));
    }
    else
    {
      WiresPanelComponent comp;
      if (this.TryComp<WiresPanelComponent>(uid, out comp) && !comp.Open)
        this._popup.PopupClient(this.Loc.GetString("encryption-keys-panel-locked"), uid, new EntityUid?(args.User));
      else if (component.KeySlots <= component.KeyContainer.ContainedEntities.Count)
      {
        this._popup.PopupClient(this.Loc.GetString("encryption-key-slots-already-full"), uid, new EntityUid?(args.User));
      }
      else
      {
        if (!this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) args.Used, (BaseContainer) component.KeyContainer))
          return;
        this._popup.PopupClient(this.Loc.GetString("encryption-key-successfully-installed"), uid, new EntityUid?(args.User));
        this._audio.PlayPredicted(component.KeyInsertionSound, args.Target, new EntityUid?(args.User));
        args.Handled = true;
      }
    }
  }

  private void TryRemoveKey(
    EntityUid uid,
    EncryptionKeyHolderComponent component,
    InteractUsingEvent args,
    ToolComponent? tool)
  {
    if (!component.KeysUnlocked)
      this._popup.PopupClient(this.Loc.GetString("encryption-keys-are-locked"), uid, new EntityUid?(args.User));
    else if (!this._wires.IsPanelOpen((Entity<WiresPanelComponent>) uid))
      this._popup.PopupClient(this.Loc.GetString("encryption-keys-panel-locked"), uid, new EntityUid?(args.User));
    else if (component.KeyContainer.ContainedEntities.Count == 0)
      this._popup.PopupClient(this.Loc.GetString("encryption-keys-no-keys"), uid, new EntityUid?(args.User));
    else
      this._tool.UseTool(args.Used, args.User, new EntityUid?(uid), 1f, component.KeysExtractionMethod, (DoAfterEvent) new EncryptionKeySystem.EncryptionRemovalFinishedEvent(), toolComponent: tool);
  }

  private void OnStartup(
    EntityUid uid,
    EncryptionKeyHolderComponent component,
    ComponentStartup args)
  {
    component.KeyContainer = this._container.EnsureContainer<Container>(uid, "key_slots");
    this.UpdateChannels(uid, component);
  }

  private void OnHolderExamined(
    EntityUid uid,
    EncryptionKeyHolderComponent component,
    ExaminedEvent args)
  {
    if (!args.IsInDetailsRange)
      return;
    if (component.KeyContainer.ContainedEntities.Count == 0 && component.Channels.Count == 0)
    {
      args.PushMarkup(this.Loc.GetString("encryption-keys-no-keys"));
    }
    else
    {
      if (component.Channels.Count <= 0)
        return;
      using (args.PushGroup("EncryptionKeyComponent"))
      {
        args.PushMarkup(this.Loc.GetString("examine-encryption-channels-prefix"));
        this.AddChannelsExamine(component.Channels, component.DefaultChannel, args, this._protoManager, "examine-encryption-channel", component.ReadOnlyChannels);
      }
    }
  }

  private void OnKeyExamined(EntityUid uid, EncryptionKeyComponent component, ExaminedEvent args)
  {
    if (!args.IsInDetailsRange || component.Channels.Count <= 0)
      return;
    args.PushMarkup(this.Loc.GetString("examine-encryption-channels-prefix"));
    this.AddChannelsExamine(component.Channels, component.DefaultChannel, args, this._protoManager, "examine-encryption-channel", component.ReadOnlyChannels);
  }

  public void AddChannelsExamine(
    HashSet<string> channels,
    string? defaultChannel,
    ExaminedEvent examineEvent,
    IPrototypeManager protoManager,
    string channelFTLPattern,
    HashSet<ProtoId<RadioChannelPrototype>>? ReadonlyChannels)
  {
    RadioChannelPrototype prototype;
    foreach (string channel in channels)
    {
      prototype = this._protoManager.Index<RadioChannelPrototype>(channel);
      string str1;
      if (!((ProtoId<RadioChannelPrototype>) channel == SharedChatSystem.CommonChannel))
        str1 = $"{':'}{prototype.KeyCode}";
      else
        str1 = ';'.ToString();
      string str2 = str1;
      string str3 = "";
      if (ReadonlyChannels != null && ReadonlyChannels.Contains((ProtoId<RadioChannelPrototype>) channel))
        str3 = " Read Only";
      examineEvent.PushMarkup(this.Loc.GetString(channelFTLPattern, ("color", (object) prototype.Color), ("key", (object) str2), ("id", (object) prototype.LocalizedName), ("freq", (object) (float) ((double) prototype.Frequency / 10.0))) + str3);
    }
    if (defaultChannel == null || !this._protoManager.TryIndex<RadioChannelPrototype>(defaultChannel, out prototype))
      return;
    if (this.HasComp<HeadsetComponent>(examineEvent.Examined))
    {
      string markup = this.Loc.GetString("examine-headset-default-channel", ("prefix", (object) SharedChatSystem.DefaultChannelPrefix), ("channel", (object) prototype.LocalizedName), ("color", (object) prototype.Color));
      examineEvent.PushMarkup(markup);
    }
    if (!this.HasComp<EncryptionKeyComponent>(examineEvent.Examined))
      return;
    string markup1 = this.Loc.GetString("examine-encryption-default-channel", ("channel", (object) prototype.LocalizedName), ("color", (object) prototype.Color));
    examineEvent.PushMarkup(markup1);
  }

  [NetSerializable]
  [Serializable]
  public sealed class EncryptionRemovalFinishedEvent : 
    SimpleDoAfterEvent,
    ISerializationGenerated<EncryptionKeySystem.EncryptionRemovalFinishedEvent>,
    ISerializationGenerated
  {
    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref EncryptionKeySystem.EncryptionRemovalFinishedEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
      this.InternalCopy(ref target1, serialization, hookCtx, context);
      target = (EncryptionKeySystem.EncryptionRemovalFinishedEvent) target1;
      serialization.TryCustomCopy<EncryptionKeySystem.EncryptionRemovalFinishedEvent>(this, ref target, hookCtx, false, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref EncryptionKeySystem.EncryptionRemovalFinishedEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref SimpleDoAfterEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      EncryptionKeySystem.EncryptionRemovalFinishedEvent target1 = (EncryptionKeySystem.EncryptionRemovalFinishedEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (SimpleDoAfterEvent) target1;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      EncryptionKeySystem.EncryptionRemovalFinishedEvent target1 = (EncryptionKeySystem.EncryptionRemovalFinishedEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [PreserveBaseOverrides]
    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    virtual EncryptionKeySystem.EncryptionRemovalFinishedEvent SimpleDoAfterEvent.Instantiate()
    {
      return new EncryptionKeySystem.EncryptionRemovalFinishedEvent();
    }
  }
}
