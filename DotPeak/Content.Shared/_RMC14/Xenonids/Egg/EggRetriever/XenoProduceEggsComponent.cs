// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Egg.EggRetriever.XenoGenerateEggsComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Egg.EggRetriever;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoGenerateEggsComponent : 
  Component,
  ISerializationGenerated<XenoGenerateEggsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Active;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PlasmaDrain = FixedPoint2.New(15);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DrainEvery = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan EggEvery = TimeSpan.FromSeconds(30L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? NextDrain;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? NextEgg;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoGenerateEggsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoGenerateEggsComponent) target1;
    if (serialization.TryCustomCopy<XenoGenerateEggsComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Active, ref target2, hookCtx, false, context))
      target2 = this.Active;
    target.Active = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaDrain, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.PlasmaDrain, hookCtx, context);
    target.PlasmaDrain = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DrainEvery, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.DrainEvery, hookCtx, context);
    target.DrainEvery = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.EggEvery, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.EggEvery, hookCtx, context);
    target.EggEvery = target5;
    TimeSpan? target6 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.NextDrain, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan?>(this.NextDrain, hookCtx, context);
    target.NextDrain = target6;
    TimeSpan? target7 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.NextEgg, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan?>(this.NextEgg, hookCtx, context);
    target.NextEgg = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoGenerateEggsComponent target,
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
    XenoGenerateEggsComponent target1 = (XenoGenerateEggsComponent) target;
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
    XenoGenerateEggsComponent target1 = (XenoGenerateEggsComponent) target;
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
    XenoGenerateEggsComponent target1 = (XenoGenerateEggsComponent) target;
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
  virtual XenoGenerateEggsComponent Component.Instantiate() => new XenoGenerateEggsComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoGenerateEggsComponent_AutoState : IComponentState
  {
    public bool Active;
    public FixedPoint2 PlasmaDrain;
    public TimeSpan DrainEvery;
    public TimeSpan EggEvery;
    public TimeSpan? NextDrain;
    public TimeSpan? NextEgg;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoGenerateEggsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoGenerateEggsComponent, ComponentGetState>(new ComponentEventRefHandler<XenoGenerateEggsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoGenerateEggsComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoGenerateEggsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoGenerateEggsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoGenerateEggsComponent.XenoGenerateEggsComponent_AutoState()
      {
        Active = component.Active,
        PlasmaDrain = component.PlasmaDrain,
        DrainEvery = component.DrainEvery,
        EggEvery = component.EggEvery,
        NextDrain = component.NextDrain,
        NextEgg = component.NextEgg
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoGenerateEggsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoGenerateEggsComponent.XenoGenerateEggsComponent_AutoState current))
        return;
      component.Active = current.Active;
      component.PlasmaDrain = current.PlasmaDrain;
      component.DrainEvery = current.DrainEvery;
      component.EggEvery = current.EggEvery;
      component.NextDrain = current.NextDrain;
      component.NextEgg = current.NextEgg;
    }
  }
}
