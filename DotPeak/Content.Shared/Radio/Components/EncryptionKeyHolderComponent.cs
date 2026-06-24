// Decompiled with JetBrains decompiler
// Type: Content.Shared.Radio.Components.EncryptionKeyHolderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Tools;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Radio.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class EncryptionKeyHolderComponent : 
  Component,
  ISerializationGenerated<EncryptionKeyHolderComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("keysUnlocked", false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool KeysUnlocked = true;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("keysExtractionMethod", false, 1, false, false, typeof (PrototypeIdSerializer<ToolQualityPrototype>))]
  [AutoNetworkedField]
  public string KeysExtractionMethod = "Screwing";
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("keySlots", false, 1, false, false, null)]
  [AutoNetworkedField]
  public int KeySlots = 2;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("keyExtractionSound", false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier KeyExtractionSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/pistol_magout.ogg");
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("keyInsertionSound", false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier KeyInsertionSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/pistol_magin.ogg");
  [Robust.Shared.ViewVariables.ViewVariables]
  public Container KeyContainer;
  public const string KeyContainerName = "key_slots";
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public HashSet<string> Channels = new HashSet<string>();
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public string? DefaultChannel;
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public HashSet<ProtoId<RadioChannelPrototype>> ReadOnlyChannels = new HashSet<ProtoId<RadioChannelPrototype>>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EncryptionKeyHolderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EncryptionKeyHolderComponent) target1;
    if (serialization.TryCustomCopy<EncryptionKeyHolderComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.KeysUnlocked, ref target2, hookCtx, false, context))
      target2 = this.KeysUnlocked;
    target.KeysUnlocked = target2;
    string target3 = (string) null;
    if (this.KeysExtractionMethod == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.KeysExtractionMethod, ref target3, hookCtx, false, context))
      target3 = this.KeysExtractionMethod;
    target.KeysExtractionMethod = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.KeySlots, ref target4, hookCtx, false, context))
      target4 = this.KeySlots;
    target.KeySlots = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.KeyExtractionSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.KeyExtractionSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.KeyExtractionSound, hookCtx, context);
    target.KeyExtractionSound = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (this.KeyInsertionSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.KeyInsertionSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.KeyInsertionSound, hookCtx, context);
    target.KeyInsertionSound = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EncryptionKeyHolderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EncryptionKeyHolderComponent target1 = (EncryptionKeyHolderComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EncryptionKeyHolderComponent target1 = (EncryptionKeyHolderComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EncryptionKeyHolderComponent target1 = (EncryptionKeyHolderComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual EncryptionKeyHolderComponent Component.Instantiate()
  {
    return new EncryptionKeyHolderComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class EncryptionKeyHolderComponent_AutoState : IComponentState
  {
    public bool KeysUnlocked;
    public string KeysExtractionMethod;
    public int KeySlots;
    public SoundSpecifier KeyExtractionSound;
    public SoundSpecifier KeyInsertionSound;
    public HashSet<string> Channels;
    public string? DefaultChannel;
    public HashSet<ProtoId<RadioChannelPrototype>> ReadOnlyChannels;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class EncryptionKeyHolderComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<EncryptionKeyHolderComponent, ComponentGetState>(new ComponentEventRefHandler<EncryptionKeyHolderComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<EncryptionKeyHolderComponent, ComponentHandleState>(new ComponentEventRefHandler<EncryptionKeyHolderComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      EncryptionKeyHolderComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new EncryptionKeyHolderComponent.EncryptionKeyHolderComponent_AutoState()
      {
        KeysUnlocked = component.KeysUnlocked,
        KeysExtractionMethod = component.KeysExtractionMethod,
        KeySlots = component.KeySlots,
        KeyExtractionSound = component.KeyExtractionSound,
        KeyInsertionSound = component.KeyInsertionSound,
        Channels = component.Channels,
        DefaultChannel = component.DefaultChannel,
        ReadOnlyChannels = component.ReadOnlyChannels
      };
    }

    private void OnHandleState(
      EntityUid uid,
      EncryptionKeyHolderComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is EncryptionKeyHolderComponent.EncryptionKeyHolderComponent_AutoState current))
        return;
      component.KeysUnlocked = current.KeysUnlocked;
      component.KeysExtractionMethod = current.KeysExtractionMethod;
      component.KeySlots = current.KeySlots;
      component.KeyExtractionSound = current.KeyExtractionSound;
      component.KeyInsertionSound = current.KeyInsertionSound;
      component.Channels = current.Channels == null ? (HashSet<string>) null : new HashSet<string>((IEnumerable<string>) current.Channels);
      component.DefaultChannel = current.DefaultChannel;
      component.ReadOnlyChannels = current.ReadOnlyChannels == null ? (HashSet<ProtoId<RadioChannelPrototype>>) null : new HashSet<ProtoId<RadioChannelPrototype>>((IEnumerable<ProtoId<RadioChannelPrototype>>) current.ReadOnlyChannels);
    }
  }
}
