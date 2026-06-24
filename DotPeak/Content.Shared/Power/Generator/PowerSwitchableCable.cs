// Decompiled with JetBrains decompiler
// Type: Content.Shared.Power.Generator.PowerSwitchableCable
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Shared.Power.Generator;

[DataDefinition]
public sealed class PowerSwitchableCable : 
  ISerializationGenerated<PowerSwitchableCable>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public SwitchableVoltage Voltage;
  [DataField(null, false, 1, true, false, null)]
  public string Node = string.Empty;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PowerSwitchableCable target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<PowerSwitchableCable>(this, ref target, hookCtx, false, context))
      return;
    SwitchableVoltage target1 = SwitchableVoltage.HV;
    if (!serialization.TryCustomCopy<SwitchableVoltage>(this.Voltage, ref target1, hookCtx, false, context))
      target1 = this.Voltage;
    target.Voltage = target1;
    string target2 = (string) null;
    if (this.Node == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Node, ref target2, hookCtx, false, context))
      target2 = this.Node;
    target.Node = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PowerSwitchableCable target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PowerSwitchableCable target1 = (PowerSwitchableCable) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public PowerSwitchableCable Instantiate() => new PowerSwitchableCable();
}
