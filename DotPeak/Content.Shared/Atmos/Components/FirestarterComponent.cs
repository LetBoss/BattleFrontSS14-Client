// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Components.FirestarterComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Atmos.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedFirestarterSystem)})]
public sealed class FirestarterComponent : 
  Component,
  ISerializationGenerated<FirestarterComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float IgnitionRadius = 4f;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? FireStarterAction = EntProtoId.op_Implicit("ActionFireStarter");
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? FireStarterActionEntity;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier IgniteSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Magic/rumble.ogg", new AudioParams?());

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FirestarterComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (FirestarterComponent) component;
    if (serialization.TryCustomCopy<FirestarterComponent>(this, ref target, hookCtx, false, context))
      return;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.IgnitionRadius, ref num, hookCtx, false, context))
      num = this.IgnitionRadius;
    target.IgnitionRadius = num;
    EntProtoId? nullable1 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.FireStarterAction, ref nullable1, hookCtx, false, context))
      nullable1 = serialization.CreateCopy<EntProtoId?>(this.FireStarterAction, hookCtx, context, false);
    target.FireStarterAction = nullable1;
    EntityUid? nullable2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.FireStarterActionEntity, ref nullable2, hookCtx, false, context))
      nullable2 = serialization.CreateCopy<EntityUid?>(this.FireStarterActionEntity, hookCtx, context, false);
    target.FireStarterActionEntity = nullable2;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (this.IgniteSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.IgniteSound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.IgniteSound, hookCtx, context, false);
    target.IgniteSound = soundSpecifier;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FirestarterComponent target,
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
    FirestarterComponent target1 = (FirestarterComponent) target;
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
    FirestarterComponent target1 = (FirestarterComponent) target;
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
    FirestarterComponent target1 = (FirestarterComponent) target;
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
  virtual FirestarterComponent Component.Instantiate() => new FirestarterComponent();
}
