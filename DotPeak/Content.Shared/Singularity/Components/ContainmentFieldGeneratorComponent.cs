// Decompiled with JetBrains decompiler
// Type: Content.Shared.Singularity.Components.ContainmentFieldGeneratorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Singularity.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class ContainmentFieldGeneratorComponent : 
  Component,
  ISerializationGenerated<ContainmentFieldGeneratorComponent>,
  ISerializationGenerated
{
  private int _powerBuffer;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("powerMinimum", false, 1, false, false, null)]
  public int PowerMinimum = 6;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("power", false, 1, false, false, null)]
  public int PowerReceived = 3;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("powerLoss", false, 1, false, false, null)]
  public int PowerLoss = 2;
  [DataField("accumulator", false, 1, false, false, null)]
  public float Accumulator;
  [DataField("threshold", false, 1, false, false, null)]
  public float Threshold = 20f;
  [DataField("maxLength", false, 1, false, false, null)]
  public float MaxLength = 8f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("idTag", false, 1, false, false, typeof (PrototypeIdSerializer<TagPrototype>))]
  public string IDTag = "EmitterBolt";
  [DataField(null, false, 1, false, false, null)]
  public string SourceFixtureId = "projectile";
  [DataField(null, false, 1, false, false, null)]
  public bool Enabled;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool IsConnected;
  [DataField("collisionMask", false, 1, false, false, null)]
  public int CollisionMask = 31 /*0x1F*/;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Dictionary<Direction, (Entity<ContainmentFieldGeneratorComponent>, List<EntityUid>)> Connections = new Dictionary<Direction, (Entity<ContainmentFieldGeneratorComponent>, List<EntityUid>)>();
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("createdField", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string CreatedField = "ContainmentField";

  [DataField("powerBuffer", false, 1, false, false, null)]
  public int PowerBuffer
  {
    get => this._powerBuffer;
    set => this._powerBuffer = Math.Clamp(value, 0, 25);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ContainmentFieldGeneratorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ContainmentFieldGeneratorComponent) target1;
    if (serialization.TryCustomCopy<ContainmentFieldGeneratorComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.PowerBuffer, ref target2, hookCtx, false, context))
      target2 = this.PowerBuffer;
    target.PowerBuffer = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.PowerMinimum, ref target3, hookCtx, false, context))
      target3 = this.PowerMinimum;
    target.PowerMinimum = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.PowerReceived, ref target4, hookCtx, false, context))
      target4 = this.PowerReceived;
    target.PowerReceived = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.PowerLoss, ref target5, hookCtx, false, context))
      target5 = this.PowerLoss;
    target.PowerLoss = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Accumulator, ref target6, hookCtx, false, context))
      target6 = this.Accumulator;
    target.Accumulator = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Threshold, ref target7, hookCtx, false, context))
      target7 = this.Threshold;
    target.Threshold = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxLength, ref target8, hookCtx, false, context))
      target8 = this.MaxLength;
    target.MaxLength = target8;
    string target9 = (string) null;
    if (this.IDTag == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.IDTag, ref target9, hookCtx, false, context))
      target9 = this.IDTag;
    target.IDTag = target9;
    string target10 = (string) null;
    if (this.SourceFixtureId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SourceFixtureId, ref target10, hookCtx, false, context))
      target10 = this.SourceFixtureId;
    target.SourceFixtureId = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target11, hookCtx, false, context))
      target11 = this.Enabled;
    target.Enabled = target11;
    int target12 = 0;
    if (!serialization.TryCustomCopy<int>(this.CollisionMask, ref target12, hookCtx, false, context))
      target12 = this.CollisionMask;
    target.CollisionMask = target12;
    string target13 = (string) null;
    if (this.CreatedField == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.CreatedField, ref target13, hookCtx, false, context))
      target13 = this.CreatedField;
    target.CreatedField = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ContainmentFieldGeneratorComponent target,
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
    ContainmentFieldGeneratorComponent target1 = (ContainmentFieldGeneratorComponent) target;
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
    ContainmentFieldGeneratorComponent target1 = (ContainmentFieldGeneratorComponent) target;
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
    ContainmentFieldGeneratorComponent target1 = (ContainmentFieldGeneratorComponent) target;
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
  virtual ContainmentFieldGeneratorComponent Component.Instantiate()
  {
    return new ContainmentFieldGeneratorComponent();
  }
}
