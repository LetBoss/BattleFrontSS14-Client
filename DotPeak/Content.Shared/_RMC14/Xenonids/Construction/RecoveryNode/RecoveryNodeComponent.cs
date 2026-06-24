// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.RecoveryNode.RecoveryNodeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction.RecoveryNode;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RecoveryNodeComponent : 
  Component,
  ISerializationGenerated<RecoveryNodeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 HealAmount = (FixedPoint2) 25;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float HealRange = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan HealCooldown = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan NextHealAt;
  [DataField(null, false, 1, false, false, null)]
  public DoAfterId? HealDoAfter;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RecoveryNodeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RecoveryNodeComponent) target1;
    if (serialization.TryCustomCopy<RecoveryNodeComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.HealAmount, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.HealAmount, hookCtx, context);
    target.HealAmount = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HealRange, ref target3, hookCtx, false, context))
      target3 = this.HealRange;
    target.HealRange = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.HealCooldown, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.HealCooldown, hookCtx, context);
    target.HealCooldown = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextHealAt, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.NextHealAt, hookCtx, context);
    target.NextHealAt = target5;
    DoAfterId? target6 = new DoAfterId?();
    if (!serialization.TryCustomCopy<DoAfterId?>(this.HealDoAfter, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<DoAfterId?>(this.HealDoAfter, hookCtx, context);
    target.HealDoAfter = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RecoveryNodeComponent target,
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
    RecoveryNodeComponent target1 = (RecoveryNodeComponent) target;
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
    RecoveryNodeComponent target1 = (RecoveryNodeComponent) target;
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
    RecoveryNodeComponent target1 = (RecoveryNodeComponent) target;
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
  virtual RecoveryNodeComponent Component.Instantiate() => new RecoveryNodeComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RecoveryNodeComponent_AutoState : IComponentState
  {
    public FixedPoint2 HealAmount;
    public float HealRange;
    public TimeSpan HealCooldown;
    public TimeSpan NextHealAt;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RecoveryNodeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RecoveryNodeComponent, ComponentGetState>(new ComponentEventRefHandler<RecoveryNodeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RecoveryNodeComponent, ComponentHandleState>(new ComponentEventRefHandler<RecoveryNodeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RecoveryNodeComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RecoveryNodeComponent.RecoveryNodeComponent_AutoState()
      {
        HealAmount = component.HealAmount,
        HealRange = component.HealRange,
        HealCooldown = component.HealCooldown,
        NextHealAt = component.NextHealAt
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RecoveryNodeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RecoveryNodeComponent.RecoveryNodeComponent_AutoState current))
        return;
      component.HealAmount = current.HealAmount;
      component.HealRange = current.HealRange;
      component.HealCooldown = current.HealCooldown;
      component.NextHealAt = current.NextHealAt;
    }
  }
}
