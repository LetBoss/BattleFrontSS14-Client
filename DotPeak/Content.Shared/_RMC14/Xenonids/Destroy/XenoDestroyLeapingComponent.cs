// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Destroy.XenoDestroyLeapingComponent
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
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Destroy;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoDestroyLeapingComponent : 
  Component,
  ISerializationGenerated<XenoDestroyLeapingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityCoordinates? Target;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? LeapMoveAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? LeapEndAt;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoDestroyLeapingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoDestroyLeapingComponent) target1;
    if (serialization.TryCustomCopy<XenoDestroyLeapingComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityCoordinates? target2 = new EntityCoordinates?();
    if (!serialization.TryCustomCopy<EntityCoordinates?>(this.Target, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityCoordinates?>(this.Target, hookCtx, context);
    target.Target = target2;
    TimeSpan? target3 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.LeapMoveAt, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan?>(this.LeapMoveAt, hookCtx, context);
    target.LeapMoveAt = target3;
    TimeSpan? target4 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.LeapEndAt, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan?>(this.LeapEndAt, hookCtx, context);
    target.LeapEndAt = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoDestroyLeapingComponent target,
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
    XenoDestroyLeapingComponent target1 = (XenoDestroyLeapingComponent) target;
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
    XenoDestroyLeapingComponent target1 = (XenoDestroyLeapingComponent) target;
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
    XenoDestroyLeapingComponent target1 = (XenoDestroyLeapingComponent) target;
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
  virtual XenoDestroyLeapingComponent Component.Instantiate() => new XenoDestroyLeapingComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoDestroyLeapingComponent_AutoState : IComponentState
  {
    public NetCoordinates? Target;
    public TimeSpan? LeapMoveAt;
    public TimeSpan? LeapEndAt;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoDestroyLeapingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoDestroyLeapingComponent, ComponentGetState>(new ComponentEventRefHandler<XenoDestroyLeapingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoDestroyLeapingComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoDestroyLeapingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoDestroyLeapingComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoDestroyLeapingComponent.XenoDestroyLeapingComponent_AutoState()
      {
        Target = this.GetNetCoordinates(component.Target),
        LeapMoveAt = component.LeapMoveAt,
        LeapEndAt = component.LeapEndAt
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoDestroyLeapingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoDestroyLeapingComponent.XenoDestroyLeapingComponent_AutoState current))
        return;
      component.Target = this.EnsureCoordinates<XenoDestroyLeapingComponent>(current.Target, uid);
      component.LeapMoveAt = current.LeapMoveAt;
      component.LeapEndAt = current.LeapEndAt;
    }
  }
}
