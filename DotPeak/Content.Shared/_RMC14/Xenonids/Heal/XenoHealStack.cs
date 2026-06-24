// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Heal.XenoHealStack
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Heal;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class XenoHealStack : ISerializationGenerated<XenoHealStack>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 HealAmount;
  [DataField(null, false, 1, false, false, null)]
  public int Charges;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan TimeBetweenHeals;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  public TimeSpan NextHealAt;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoHealStack target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<XenoHealStack>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target1 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.HealAmount, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<FixedPoint2>(this.HealAmount, hookCtx, context);
    target.HealAmount = target1;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Charges, ref target2, hookCtx, false, context))
      target2 = this.Charges;
    target.Charges = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TimeBetweenHeals, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.TimeBetweenHeals, hookCtx, context);
    target.TimeBetweenHeals = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextHealAt, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.NextHealAt, hookCtx, context);
    target.NextHealAt = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoHealStack target,
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
    XenoHealStack target1 = (XenoHealStack) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public XenoHealStack Instantiate() => new XenoHealStack();
}
