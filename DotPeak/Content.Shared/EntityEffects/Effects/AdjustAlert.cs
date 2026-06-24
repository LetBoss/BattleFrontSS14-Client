// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.AdjustAlert
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects;

public sealed class AdjustAlert : 
  EntityEffect,
  ISerializationGenerated<AdjustAlert>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<AlertPrototype> AlertType;
  [DataField(null, false, 1, false, false, null)]
  public bool Clear;
  [DataField(null, false, 1, false, false, null)]
  public bool ShowCooldown;
  [DataField(null, false, 1, false, false, null)]
  public float Time;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return (string) null;
  }

  public override void Effect(EntityEffectBaseArgs args)
  {
    AlertsSystem entitySystem = args.EntityManager.EntitySysManager.GetEntitySystem<AlertsSystem>();
    if (!args.EntityManager.HasComponent<AlertsComponent>(args.TargetEntity))
      return;
    if (this.Clear && (double) this.Time <= 0.0)
    {
      entitySystem.ClearAlert(args.TargetEntity, this.AlertType);
    }
    else
    {
      IGameTiming gameTiming = IoCManager.Resolve<IGameTiming>();
      (TimeSpan, TimeSpan)? nullable1 = new (TimeSpan, TimeSpan)?();
      if ((this.ShowCooldown || this.Clear) && (double) this.Time > 0.0)
        nullable1 = new (TimeSpan, TimeSpan)?((gameTiming.CurTime, gameTiming.CurTime + TimeSpan.FromSeconds((double) this.Time)));
      AlertsSystem alertsSystem = entitySystem;
      EntityUid targetEntity = args.TargetEntity;
      ProtoId<AlertPrototype> alertType = this.AlertType;
      (TimeSpan, TimeSpan)? nullable2 = nullable1;
      bool clear = this.Clear;
      bool showCooldown = this.ShowCooldown;
      short? severity = new short?();
      (TimeSpan, TimeSpan)? cooldown = nullable2;
      int num1 = clear ? 1 : 0;
      int num2 = showCooldown ? 1 : 0;
      alertsSystem.ShowAlert(targetEntity, alertType, severity, cooldown, num1 != 0, num2 != 0);
    }
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AdjustAlert target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AdjustAlert) target1;
    if (serialization.TryCustomCopy<AdjustAlert>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<AlertPrototype> target2 = new ProtoId<AlertPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(this.AlertType, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<AlertPrototype>>(this.AlertType, hookCtx, context);
    target.AlertType = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Clear, ref target3, hookCtx, false, context))
      target3 = this.Clear;
    target.Clear = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowCooldown, ref target4, hookCtx, false, context))
      target4 = this.ShowCooldown;
    target.ShowCooldown = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Time, ref target5, hookCtx, false, context))
      target5 = this.Time;
    target.Time = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AdjustAlert target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EntityEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AdjustAlert target1 = (AdjustAlert) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EntityEffect) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AdjustAlert target1 = (AdjustAlert) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual AdjustAlert EntityEffect.Instantiate() => new AdjustAlert();
}
