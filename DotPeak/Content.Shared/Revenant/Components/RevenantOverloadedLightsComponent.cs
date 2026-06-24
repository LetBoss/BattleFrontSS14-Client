// Decompiled with JetBrains decompiler
// Type: Content.Shared.Revenant.Components.RevenantOverloadedLightsComponent
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
namespace Content.Shared.Revenant.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class RevenantOverloadedLightsComponent : 
  Component,
  ISerializationGenerated<RevenantOverloadedLightsComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityUid? Target;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float Accumulator;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float ZapDelay = 3f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float ZapRange = 4f;
  [DataField("zapBeamEntityId", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string ZapBeamEntityId = "LightningRevenant";
  public float? OriginalEnergy;
  public bool OriginalEnabled;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RevenantOverloadedLightsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RevenantOverloadedLightsComponent) target1;
    if (serialization.TryCustomCopy<RevenantOverloadedLightsComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.ZapBeamEntityId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ZapBeamEntityId, ref target2, hookCtx, false, context))
      target2 = this.ZapBeamEntityId;
    target.ZapBeamEntityId = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RevenantOverloadedLightsComponent target,
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
    RevenantOverloadedLightsComponent target1 = (RevenantOverloadedLightsComponent) target;
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
    RevenantOverloadedLightsComponent target1 = (RevenantOverloadedLightsComponent) target;
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
    RevenantOverloadedLightsComponent target1 = (RevenantOverloadedLightsComponent) target;
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
  virtual RevenantOverloadedLightsComponent Component.Instantiate()
  {
    return new RevenantOverloadedLightsComponent();
  }
}
