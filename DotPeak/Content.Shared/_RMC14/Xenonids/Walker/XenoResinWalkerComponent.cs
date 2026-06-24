// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Walker.XenoResinWalkerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Walker;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (XenoResinWalkerSystem)})]
public sealed class XenoResinWalkerComponent : 
  Component,
  ISerializationGenerated<XenoResinWalkerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Active;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PlasmaCost = (FixedPoint2) 50;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PlasmaUpkeep = (FixedPoint2) 15;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextPlasmaUse;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan PlasmaUseDelay = TimeSpan.FromSeconds(0.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SpeedMultiplier = 1.66f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoResinWalkerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoResinWalkerComponent) target1;
    if (serialization.TryCustomCopy<XenoResinWalkerComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Active, ref target2, hookCtx, false, context))
      target2 = this.Active;
    target.Active = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaCost, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.PlasmaCost, hookCtx, context);
    target.PlasmaCost = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaUpkeep, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.PlasmaUpkeep, hookCtx, context);
    target.PlasmaUpkeep = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextPlasmaUse, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.NextPlasmaUse, hookCtx, context);
    target.NextPlasmaUse = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.PlasmaUseDelay, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.PlasmaUseDelay, hookCtx, context);
    target.PlasmaUseDelay = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpeedMultiplier, ref target7, hookCtx, false, context))
      target7 = this.SpeedMultiplier;
    target.SpeedMultiplier = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoResinWalkerComponent target,
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
    XenoResinWalkerComponent target1 = (XenoResinWalkerComponent) target;
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
    XenoResinWalkerComponent target1 = (XenoResinWalkerComponent) target;
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
    XenoResinWalkerComponent target1 = (XenoResinWalkerComponent) target;
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
  virtual XenoResinWalkerComponent Component.Instantiate() => new XenoResinWalkerComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoResinWalkerComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoResinWalkerComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoResinWalkerComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoResinWalkerComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextPlasmaUse += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoResinWalkerComponent_AutoState : IComponentState
  {
    public bool Active;
    public FixedPoint2 PlasmaCost;
    public FixedPoint2 PlasmaUpkeep;
    public TimeSpan NextPlasmaUse;
    public TimeSpan PlasmaUseDelay;
    public float SpeedMultiplier;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoResinWalkerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoResinWalkerComponent, ComponentGetState>(new ComponentEventRefHandler<XenoResinWalkerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoResinWalkerComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoResinWalkerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      XenoResinWalkerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoResinWalkerComponent.XenoResinWalkerComponent_AutoState()
      {
        Active = component.Active,
        PlasmaCost = component.PlasmaCost,
        PlasmaUpkeep = component.PlasmaUpkeep,
        NextPlasmaUse = component.NextPlasmaUse,
        PlasmaUseDelay = component.PlasmaUseDelay,
        SpeedMultiplier = component.SpeedMultiplier
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoResinWalkerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoResinWalkerComponent.XenoResinWalkerComponent_AutoState current))
        return;
      component.Active = current.Active;
      component.PlasmaCost = current.PlasmaCost;
      component.PlasmaUpkeep = current.PlasmaUpkeep;
      component.NextPlasmaUse = current.NextPlasmaUse;
      component.PlasmaUseDelay = current.PlasmaUseDelay;
      component.SpeedMultiplier = current.SpeedMultiplier;
    }
  }
}
