// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Commander.CivCommanderBotControlComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client._CIV14merka.Commander;

[RegisterComponent]
public sealed class CivCommanderBotControlComponent : 
  Component,
  ISerializationGenerated<CivCommanderBotControlComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float AggroRange = 18f;
  [DataField(null, false, 1, false, false, null)]
  public float FireRange = 14f;
  [DataField(null, false, 1, false, false, null)]
  public float LoseRange = 22f;
  [DataField(null, false, 1, false, false, null)]
  public float PickupRange = 3f;
  [DataField(null, false, 1, false, false, null)]
  public float IdleRange = 4.5f;
  [DataField(null, false, 1, false, false, null)]
  public float IdleMinRange = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  public float IdleDelay = 3f;
  [DataField(null, false, 1, false, false, null)]
  public float CoverRange = 7.5f;
  [DataField(null, false, 1, false, false, null)]
  public float ArriveRange = 0.65f;
  [DataField(null, false, 1, false, false, null)]
  public float RepathRange = 1.25f;
  [DataField(null, false, 1, false, false, null)]
  public float RepathDelay = 0.35f;
  [DataField(null, false, 1, false, false, null)]
  public float RetargetDelay = 0.2f;
  [DataField(null, false, 1, false, false, null)]
  public float FightHold = 1.2f;
  [DataField(null, false, 1, false, false, null)]
  public float ShootDelay = 0.25f;
  [DataField(null, false, 1, false, false, null)]
  public bool UseOpaqueLos;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CivCommanderBotControlComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (CivCommanderBotControlComponent) component;
    if (serialization.TryCustomCopy<CivCommanderBotControlComponent>(this, ref target, hookCtx, false, context))
      return;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AggroRange, ref num1, hookCtx, false, context))
      num1 = this.AggroRange;
    target.AggroRange = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FireRange, ref num2, hookCtx, false, context))
      num2 = this.FireRange;
    target.FireRange = num2;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LoseRange, ref num3, hookCtx, false, context))
      num3 = this.LoseRange;
    target.LoseRange = num3;
    float num4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PickupRange, ref num4, hookCtx, false, context))
      num4 = this.PickupRange;
    target.PickupRange = num4;
    float num5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.IdleRange, ref num5, hookCtx, false, context))
      num5 = this.IdleRange;
    target.IdleRange = num5;
    float num6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.IdleMinRange, ref num6, hookCtx, false, context))
      num6 = this.IdleMinRange;
    target.IdleMinRange = num6;
    float num7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.IdleDelay, ref num7, hookCtx, false, context))
      num7 = this.IdleDelay;
    target.IdleDelay = num7;
    float num8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CoverRange, ref num8, hookCtx, false, context))
      num8 = this.CoverRange;
    target.CoverRange = num8;
    float num9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ArriveRange, ref num9, hookCtx, false, context))
      num9 = this.ArriveRange;
    target.ArriveRange = num9;
    float num10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RepathRange, ref num10, hookCtx, false, context))
      num10 = this.RepathRange;
    target.RepathRange = num10;
    float num11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RepathDelay, ref num11, hookCtx, false, context))
      num11 = this.RepathDelay;
    target.RepathDelay = num11;
    float num12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RetargetDelay, ref num12, hookCtx, false, context))
      num12 = this.RetargetDelay;
    target.RetargetDelay = num12;
    float num13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FightHold, ref num13, hookCtx, false, context))
      num13 = this.FightHold;
    target.FightHold = num13;
    float num14 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ShootDelay, ref num14, hookCtx, false, context))
      num14 = this.ShootDelay;
    target.ShootDelay = num14;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.UseOpaqueLos, ref flag, hookCtx, false, context))
      flag = this.UseOpaqueLos;
    target.UseOpaqueLos = flag;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CivCommanderBotControlComponent target,
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
    CivCommanderBotControlComponent target1 = (CivCommanderBotControlComponent) target;
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
    CivCommanderBotControlComponent target1 = (CivCommanderBotControlComponent) target;
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
    CivCommanderBotControlComponent target1 = (CivCommanderBotControlComponent) target;
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
  virtual CivCommanderBotControlComponent Component.Instantiate()
  {
    return new CivCommanderBotControlComponent();
  }
}
