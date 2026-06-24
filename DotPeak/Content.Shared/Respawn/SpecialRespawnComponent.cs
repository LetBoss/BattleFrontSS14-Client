// Decompiled with JetBrains decompiler
// Type: Content.Shared.Respawn.SpecialRespawnComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Respawn;

[RegisterComponent]
[NetworkedComponent]
public sealed class SpecialRespawnComponent : 
  Component,
  ISerializationGenerated<SpecialRespawnComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("stationMap", false, 1, false, false, null)]
  public (EntityUid?, EntityUid?) StationMap;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("respawn", false, 1, false, false, null)]
  public bool Respawn = true;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("prototype", false, 1, true, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string Prototype = "";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SpecialRespawnComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SpecialRespawnComponent) target1;
    if (serialization.TryCustomCopy<SpecialRespawnComponent>(this, ref target, hookCtx, false, context))
      return;
    (EntityUid?, EntityUid?) target2 = ();
    if (!serialization.TryCustomCopy<(EntityUid?, EntityUid?)>(this.StationMap, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<(EntityUid?, EntityUid?)>(this.StationMap, hookCtx, context);
    target.StationMap = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Respawn, ref target3, hookCtx, false, context))
      target3 = this.Respawn;
    target.Respawn = target3;
    string target4 = (string) null;
    if (this.Prototype == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Prototype, ref target4, hookCtx, false, context))
      target4 = this.Prototype;
    target.Prototype = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SpecialRespawnComponent target,
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
    SpecialRespawnComponent target1 = (SpecialRespawnComponent) target;
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
    SpecialRespawnComponent target1 = (SpecialRespawnComponent) target;
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
    SpecialRespawnComponent target1 = (SpecialRespawnComponent) target;
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
  virtual SpecialRespawnComponent Component.Instantiate() => new SpecialRespawnComponent();
}
