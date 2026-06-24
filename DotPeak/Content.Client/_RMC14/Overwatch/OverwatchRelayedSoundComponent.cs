// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Overwatch.OverwatchRelayedSoundComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client._RMC14.Overwatch;

[RegisterComponent]
[Access(new Type[] {typeof (OverwatchConsoleSystem)})]
public sealed class OverwatchRelayedSoundComponent : 
  Component,
  ISerializationGenerated<OverwatchRelayedSoundComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? Relay;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref OverwatchRelayedSoundComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (OverwatchRelayedSoundComponent) component;
    if (serialization.TryCustomCopy<OverwatchRelayedSoundComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? nullable = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Relay, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<EntityUid?>(this.Relay, hookCtx, context, false);
    target.Relay = nullable;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref OverwatchRelayedSoundComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    OverwatchRelayedSoundComponent target1 = (OverwatchRelayedSoundComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    OverwatchRelayedSoundComponent target1 = (OverwatchRelayedSoundComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    OverwatchRelayedSoundComponent target1 = (OverwatchRelayedSoundComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual OverwatchRelayedSoundComponent Component.Instantiate()
  {
    return new OverwatchRelayedSoundComponent();
  }
}
