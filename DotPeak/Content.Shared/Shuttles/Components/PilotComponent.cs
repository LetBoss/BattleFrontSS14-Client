// Decompiled with JetBrains decompiler
// Type: Content.Shared.Shuttles.Components.PilotComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Shuttles.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class PilotComponent : 
  Component,
  ISerializationGenerated<PilotComponent>,
  ISerializationGenerated
{
  public Vector2 CurTickStrafeMovement = Vector2.Zero;
  public float CurTickRotationMovement;
  public float CurTickBraking;
  public GameTick LastInputTick = GameTick.Zero;
  public ushort LastInputSubTick;
  [Robust.Shared.ViewVariables.ViewVariables]
  public ShuttleButtons HeldButtons;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertPrototype> PilotingAlert = (ProtoId<AlertPrototype>) "PilotingShuttle";

  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityUid? Console { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityCoordinates? Position { get; set; }

  public override bool SendOnlyToOwner => true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PilotComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PilotComponent) target1;
    if (serialization.TryCustomCopy<PilotComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<AlertPrototype> target2 = new ProtoId<AlertPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(this.PilotingAlert, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<AlertPrototype>>(this.PilotingAlert, hookCtx, context);
    target.PilotingAlert = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PilotComponent target,
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
    PilotComponent target1 = (PilotComponent) target;
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
    PilotComponent target1 = (PilotComponent) target;
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
    PilotComponent target1 = (PilotComponent) target;
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
  virtual PilotComponent Component.Instantiate() => new PilotComponent();
}
