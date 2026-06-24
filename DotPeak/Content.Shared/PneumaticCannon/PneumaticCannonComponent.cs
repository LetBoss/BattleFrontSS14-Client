// Decompiled with JetBrains decompiler
// Type: Content.Shared.PneumaticCannon.PneumaticCannonComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Tools;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.PneumaticCannon;

[RegisterComponent]
[NetworkedComponent]
public sealed class PneumaticCannonComponent : 
  Component,
  ISerializationGenerated<PneumaticCannonComponent>,
  ISerializationGenerated
{
  public const string TankSlotId = "gas_tank";
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public PneumaticCannonPower Power = PneumaticCannonPower.Medium;
  [DataField("toolModifyPower", false, 1, false, false, typeof (PrototypeIdSerializer<ToolQualityPrototype>))]
  public string ToolModifyPower = "Anchoring";
  [DataField("highPowerStunTime", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float HighPowerStunTime = 3f;
  [DataField("gasUsage", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float GasUsage = 0.142f;
  [DataField("baseProjectileSpeed", false, 1, false, false, null)]
  public float BaseProjectileSpeed = 20f;
  [DataField(null, false, 1, false, false, null)]
  public float? ProjectileSpeed;
  [DataField("throwItems", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool ThrowItems = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PneumaticCannonComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PneumaticCannonComponent) target1;
    if (serialization.TryCustomCopy<PneumaticCannonComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.ToolModifyPower == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ToolModifyPower, ref target2, hookCtx, false, context))
      target2 = this.ToolModifyPower;
    target.ToolModifyPower = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HighPowerStunTime, ref target3, hookCtx, false, context))
      target3 = this.HighPowerStunTime;
    target.HighPowerStunTime = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.GasUsage, ref target4, hookCtx, false, context))
      target4 = this.GasUsage;
    target.GasUsage = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BaseProjectileSpeed, ref target5, hookCtx, false, context))
      target5 = this.BaseProjectileSpeed;
    target.BaseProjectileSpeed = target5;
    float? target6 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.ProjectileSpeed, ref target6, hookCtx, false, context))
      target6 = this.ProjectileSpeed;
    target.ProjectileSpeed = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.ThrowItems, ref target7, hookCtx, false, context))
      target7 = this.ThrowItems;
    target.ThrowItems = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PneumaticCannonComponent target,
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
    PneumaticCannonComponent target1 = (PneumaticCannonComponent) target;
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
    PneumaticCannonComponent target1 = (PneumaticCannonComponent) target;
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
    PneumaticCannonComponent target1 = (PneumaticCannonComponent) target;
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
  virtual PneumaticCannonComponent Component.Instantiate() => new PneumaticCannonComponent();
}
