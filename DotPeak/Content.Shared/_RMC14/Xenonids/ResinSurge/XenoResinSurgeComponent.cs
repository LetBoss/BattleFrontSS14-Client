// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.ResinSurge.XenoResinSurgeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.ResinSurge;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedXenoResinSurgeSystem)})]
public sealed class XenoResinSurgeComponent : 
  Component,
  ISerializationGenerated<XenoResinSurgeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan FruitGrowth = TimeSpan.FromSeconds(60L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 ReinforceAmount = (FixedPoint2) 6000;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ReinforceDuration = TimeSpan.FromSeconds(15L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan StickyResinDoAfterPeriod = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int StickyResinRadius = 1;
  [DataField(null, false, 1, false, false, null)]
  public DoAfterId? ResinDoafter;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId UnstableWallId = (EntProtoId) "WallXenoResinWeak";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId StickyResinId = (EntProtoId) "XenoStickyResinWeak";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Range = 7;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan FruitCooldownDivisor = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId SurgeWallEffect = (EntProtoId) "XenoSurgeResinWall";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId SurgeDoorEffect = (EntProtoId) "XenoSurgeResinDoor";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoResinSurgeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoResinSurgeComponent) target1;
    if (serialization.TryCustomCopy<XenoResinSurgeComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FruitGrowth, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.FruitGrowth, hookCtx, context);
    target.FruitGrowth = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.ReinforceAmount, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.ReinforceAmount, hookCtx, context);
    target.ReinforceAmount = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ReinforceDuration, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.ReinforceDuration, hookCtx, context);
    target.ReinforceDuration = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StickyResinDoAfterPeriod, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.StickyResinDoAfterPeriod, hookCtx, context);
    target.StickyResinDoAfterPeriod = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.StickyResinRadius, ref target6, hookCtx, false, context))
      target6 = this.StickyResinRadius;
    target.StickyResinRadius = target6;
    DoAfterId? target7 = new DoAfterId?();
    if (!serialization.TryCustomCopy<DoAfterId?>(this.ResinDoafter, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<DoAfterId?>(this.ResinDoafter, hookCtx, context);
    target.ResinDoafter = target7;
    EntProtoId target8 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.UnstableWallId, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<EntProtoId>(this.UnstableWallId, hookCtx, context);
    target.UnstableWallId = target8;
    EntProtoId target9 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.StickyResinId, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<EntProtoId>(this.StickyResinId, hookCtx, context);
    target.StickyResinId = target9;
    int target10 = 0;
    if (!serialization.TryCustomCopy<int>(this.Range, ref target10, hookCtx, false, context))
      target10 = this.Range;
    target.Range = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FruitCooldownDivisor, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.FruitCooldownDivisor, hookCtx, context);
    target.FruitCooldownDivisor = target11;
    EntProtoId target12 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.SurgeWallEffect, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<EntProtoId>(this.SurgeWallEffect, hookCtx, context);
    target.SurgeWallEffect = target12;
    EntProtoId target13 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.SurgeDoorEffect, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<EntProtoId>(this.SurgeDoorEffect, hookCtx, context);
    target.SurgeDoorEffect = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoResinSurgeComponent target,
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
    XenoResinSurgeComponent target1 = (XenoResinSurgeComponent) target;
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
    XenoResinSurgeComponent target1 = (XenoResinSurgeComponent) target;
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
    XenoResinSurgeComponent target1 = (XenoResinSurgeComponent) target;
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
  virtual XenoResinSurgeComponent Component.Instantiate() => new XenoResinSurgeComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoResinSurgeComponent_AutoState : IComponentState
  {
    public TimeSpan FruitGrowth;
    public FixedPoint2 ReinforceAmount;
    public TimeSpan ReinforceDuration;
    public TimeSpan StickyResinDoAfterPeriod;
    public int StickyResinRadius;
    public EntProtoId UnstableWallId;
    public EntProtoId StickyResinId;
    public int Range;
    public TimeSpan FruitCooldownDivisor;
    public EntProtoId SurgeWallEffect;
    public EntProtoId SurgeDoorEffect;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoResinSurgeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoResinSurgeComponent, ComponentGetState>(new ComponentEventRefHandler<XenoResinSurgeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoResinSurgeComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoResinSurgeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoResinSurgeComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoResinSurgeComponent.XenoResinSurgeComponent_AutoState()
      {
        FruitGrowth = component.FruitGrowth,
        ReinforceAmount = component.ReinforceAmount,
        ReinforceDuration = component.ReinforceDuration,
        StickyResinDoAfterPeriod = component.StickyResinDoAfterPeriod,
        StickyResinRadius = component.StickyResinRadius,
        UnstableWallId = component.UnstableWallId,
        StickyResinId = component.StickyResinId,
        Range = component.Range,
        FruitCooldownDivisor = component.FruitCooldownDivisor,
        SurgeWallEffect = component.SurgeWallEffect,
        SurgeDoorEffect = component.SurgeDoorEffect
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoResinSurgeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoResinSurgeComponent.XenoResinSurgeComponent_AutoState current))
        return;
      component.FruitGrowth = current.FruitGrowth;
      component.ReinforceAmount = current.ReinforceAmount;
      component.ReinforceDuration = current.ReinforceDuration;
      component.StickyResinDoAfterPeriod = current.StickyResinDoAfterPeriod;
      component.StickyResinRadius = current.StickyResinRadius;
      component.UnstableWallId = current.UnstableWallId;
      component.StickyResinId = current.StickyResinId;
      component.Range = current.Range;
      component.FruitCooldownDivisor = current.FruitCooldownDivisor;
      component.SurgeWallEffect = current.SurgeWallEffect;
      component.SurgeDoorEffect = current.SurgeDoorEffect;
    }
  }
}
