// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Components.BatteryWeaponFireMode
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Components;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class BatteryWeaponFireMode : 
  ISerializationGenerated<BatteryWeaponFireMode>,
  ISerializationGenerated
{
  [DataField("proto", false, 1, true, false, null)]
  public EntProtoId Prototype;
  [DataField(null, false, 1, false, false, null)]
  public float FireCost = 100f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BatteryWeaponFireMode target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<BatteryWeaponFireMode>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target1 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Prototype, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<EntProtoId>(this.Prototype, hookCtx, context);
    target.Prototype = target1;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FireCost, ref target2, hookCtx, false, context))
      target2 = this.FireCost;
    target.FireCost = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BatteryWeaponFireMode target,
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
    BatteryWeaponFireMode target1 = (BatteryWeaponFireMode) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public BatteryWeaponFireMode Instantiate() => new BatteryWeaponFireMode();
}
