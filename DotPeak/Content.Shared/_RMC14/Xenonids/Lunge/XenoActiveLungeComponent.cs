// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Lunge.XenoActiveLungeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Lunge;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoLungeSystem)})]
public sealed class XenoActiveLungeComponent : 
  Component,
  ISerializationGenerated<XenoActiveLungeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public MapCoordinates Origin;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 Charge;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid Target;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public MapCoordinates TargetCoordinates;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan StunTime;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoActiveLungeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoActiveLungeComponent) target1;
    if (serialization.TryCustomCopy<XenoActiveLungeComponent>(this, ref target, hookCtx, false, context))
      return;
    MapCoordinates target2 = new MapCoordinates();
    if (!serialization.TryCustomCopy<MapCoordinates>(this.Origin, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<MapCoordinates>(this.Origin, hookCtx, context);
    target.Origin = target2;
    Vector2 target3 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Charge, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Vector2>(this.Charge, hookCtx, context);
    target.Charge = target3;
    EntityUid target4 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.Target, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid>(this.Target, hookCtx, context);
    target.Target = target4;
    MapCoordinates target5 = new MapCoordinates();
    if (!serialization.TryCustomCopy<MapCoordinates>(this.TargetCoordinates, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<MapCoordinates>(this.TargetCoordinates, hookCtx, context);
    target.TargetCoordinates = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target6, hookCtx, false, context))
      target6 = this.Range;
    target.Range = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StunTime, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.StunTime, hookCtx, context);
    target.StunTime = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoActiveLungeComponent target,
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
    XenoActiveLungeComponent target1 = (XenoActiveLungeComponent) target;
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
    XenoActiveLungeComponent target1 = (XenoActiveLungeComponent) target;
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
    XenoActiveLungeComponent target1 = (XenoActiveLungeComponent) target;
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
  virtual XenoActiveLungeComponent Component.Instantiate() => new XenoActiveLungeComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoActiveLungeComponent_AutoState : IComponentState
  {
    public MapCoordinates Origin;
    public Vector2 Charge;
    public NetEntity Target;
    public MapCoordinates TargetCoordinates;
    public float Range;
    public TimeSpan StunTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoActiveLungeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoActiveLungeComponent, ComponentGetState>(new ComponentEventRefHandler<XenoActiveLungeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoActiveLungeComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoActiveLungeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoActiveLungeComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoActiveLungeComponent.XenoActiveLungeComponent_AutoState()
      {
        Origin = component.Origin,
        Charge = component.Charge,
        Target = this.GetNetEntity(component.Target),
        TargetCoordinates = component.TargetCoordinates,
        Range = component.Range,
        StunTime = component.StunTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoActiveLungeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoActiveLungeComponent.XenoActiveLungeComponent_AutoState current))
        return;
      component.Origin = current.Origin;
      component.Charge = current.Charge;
      component.Target = this.EnsureEntity<XenoActiveLungeComponent>(current.Target, uid);
      component.TargetCoordinates = current.TargetCoordinates;
      component.Range = current.Range;
      component.StunTime = current.StunTime;
    }
  }
}
