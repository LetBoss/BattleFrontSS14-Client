// Decompiled with JetBrains decompiler
// Type: Content.Shared.Radio.Components.EncryptionKeyComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Radio.Components;

[RegisterComponent]
public sealed class EncryptionKeyComponent : 
  Component,
  ISerializationGenerated<EncryptionKeyComponent>,
  ISerializationGenerated
{
  [DataField("channels", false, 1, false, false, typeof (PrototypeIdHashSetSerializer<RadioChannelPrototype>))]
  public HashSet<string> Channels = new HashSet<string>();
  [DataField("defaultChannel", false, 1, false, false, typeof (PrototypeIdSerializer<RadioChannelPrototype>))]
  public string? DefaultChannel;
  [DataField(null, false, 1, false, false, null)]
  public HashSet<ProtoId<RadioChannelPrototype>> ReadOnlyChannels = new HashSet<ProtoId<RadioChannelPrototype>>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EncryptionKeyComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EncryptionKeyComponent) target1;
    if (serialization.TryCustomCopy<EncryptionKeyComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<string> target2 = (HashSet<string>) null;
    if (this.Channels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<string>>(this.Channels, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<HashSet<string>>(this.Channels, hookCtx, context);
    target.Channels = target2;
    string target3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.DefaultChannel, ref target3, hookCtx, false, context))
      target3 = this.DefaultChannel;
    target.DefaultChannel = target3;
    HashSet<ProtoId<RadioChannelPrototype>> target4 = (HashSet<ProtoId<RadioChannelPrototype>>) null;
    if (this.ReadOnlyChannels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<RadioChannelPrototype>>>(this.ReadOnlyChannels, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<HashSet<ProtoId<RadioChannelPrototype>>>(this.ReadOnlyChannels, hookCtx, context);
    target.ReadOnlyChannels = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EncryptionKeyComponent target,
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
    EncryptionKeyComponent target1 = (EncryptionKeyComponent) target;
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
    EncryptionKeyComponent target1 = (EncryptionKeyComponent) target;
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
    EncryptionKeyComponent target1 = (EncryptionKeyComponent) target;
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
  virtual EncryptionKeyComponent Component.Instantiate() => new EncryptionKeyComponent();
}
