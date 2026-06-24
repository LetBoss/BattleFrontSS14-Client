// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.ManageHive.ManageHiveComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Content.Shared.Players.PlayTimeTracking;
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
namespace Content.Shared._RMC14.Xenonids.ManageHive;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (ManageHiveSystem)})]
public sealed class ManageHiveComponent : 
  Component,
  ISerializationGenerated<ManageHiveComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 DevolvePlasmaCost = (FixedPoint2) 500;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 JellyPlasmaCost = (FixedPoint2) 500;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 SacrificeBurrowedLarvaForEvolutionCost = (FixedPoint2) 100;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<PlayTimeTrackerPrototype> PlayTime = (ProtoId<PlayTimeTrackerPrototype>) "CMJobXenoQueen";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan JellyRequiredTime = TimeSpan.FromHours(25);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ManageHiveComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ManageHiveComponent) target1;
    if (serialization.TryCustomCopy<ManageHiveComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.DevolvePlasmaCost, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.DevolvePlasmaCost, hookCtx, context);
    target.DevolvePlasmaCost = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.JellyPlasmaCost, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.JellyPlasmaCost, hookCtx, context);
    target.JellyPlasmaCost = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.SacrificeBurrowedLarvaForEvolutionCost, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.SacrificeBurrowedLarvaForEvolutionCost, hookCtx, context);
    target.SacrificeBurrowedLarvaForEvolutionCost = target4;
    ProtoId<PlayTimeTrackerPrototype> target5 = new ProtoId<PlayTimeTrackerPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<PlayTimeTrackerPrototype>>(this.PlayTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<ProtoId<PlayTimeTrackerPrototype>>(this.PlayTime, hookCtx, context);
    target.PlayTime = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.JellyRequiredTime, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.JellyRequiredTime, hookCtx, context);
    target.JellyRequiredTime = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ManageHiveComponent target,
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
    ManageHiveComponent target1 = (ManageHiveComponent) target;
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
    ManageHiveComponent target1 = (ManageHiveComponent) target;
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
    ManageHiveComponent target1 = (ManageHiveComponent) target;
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
  virtual ManageHiveComponent Component.Instantiate() => new ManageHiveComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ManageHiveComponent_AutoState : IComponentState
  {
    public FixedPoint2 DevolvePlasmaCost;
    public FixedPoint2 JellyPlasmaCost;
    public FixedPoint2 SacrificeBurrowedLarvaForEvolutionCost;
    public ProtoId<PlayTimeTrackerPrototype> PlayTime;
    public TimeSpan JellyRequiredTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ManageHiveComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ManageHiveComponent, ComponentGetState>(new ComponentEventRefHandler<ManageHiveComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ManageHiveComponent, ComponentHandleState>(new ComponentEventRefHandler<ManageHiveComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ManageHiveComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ManageHiveComponent.ManageHiveComponent_AutoState()
      {
        DevolvePlasmaCost = component.DevolvePlasmaCost,
        JellyPlasmaCost = component.JellyPlasmaCost,
        SacrificeBurrowedLarvaForEvolutionCost = component.SacrificeBurrowedLarvaForEvolutionCost,
        PlayTime = component.PlayTime,
        JellyRequiredTime = component.JellyRequiredTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ManageHiveComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ManageHiveComponent.ManageHiveComponent_AutoState current))
        return;
      component.DevolvePlasmaCost = current.DevolvePlasmaCost;
      component.JellyPlasmaCost = current.JellyPlasmaCost;
      component.SacrificeBurrowedLarvaForEvolutionCost = current.SacrificeBurrowedLarvaForEvolutionCost;
      component.PlayTime = current.PlayTime;
      component.JellyRequiredTime = current.JellyRequiredTime;
    }
  }
}
