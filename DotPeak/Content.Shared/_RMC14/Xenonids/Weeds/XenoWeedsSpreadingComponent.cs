// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Weeds.XenoWeedsSpreadingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.Construction;
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
namespace Content.Shared._RMC14.Xenonids.Weeds;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedXenoWeedsSystem), typeof (SharedXenoConstructionSystem)})]
public sealed class XenoWeedsSpreadingComponent : 
  Component,
  ISerializationGenerated<XenoWeedsSpreadingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SpreadDelay = TimeSpan.FromSeconds(3.33);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan RepairedSpreadDelay = TimeSpan.FromSeconds(15L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan SpreadAt;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoWeedsSpreadingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoWeedsSpreadingComponent) target1;
    if (serialization.TryCustomCopy<XenoWeedsSpreadingComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SpreadDelay, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.SpreadDelay, hookCtx, context);
    target.SpreadDelay = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RepairedSpreadDelay, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.RepairedSpreadDelay, hookCtx, context);
    target.RepairedSpreadDelay = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SpreadAt, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.SpreadAt, hookCtx, context);
    target.SpreadAt = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoWeedsSpreadingComponent target,
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
    XenoWeedsSpreadingComponent target1 = (XenoWeedsSpreadingComponent) target;
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
    XenoWeedsSpreadingComponent target1 = (XenoWeedsSpreadingComponent) target;
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
    XenoWeedsSpreadingComponent target1 = (XenoWeedsSpreadingComponent) target;
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
  virtual XenoWeedsSpreadingComponent Component.Instantiate() => new XenoWeedsSpreadingComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoWeedsSpreadingComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoWeedsSpreadingComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoWeedsSpreadingComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoWeedsSpreadingComponent component,
      ref EntityUnpausedEvent args)
    {
      component.SpreadAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoWeedsSpreadingComponent_AutoState : IComponentState
  {
    public TimeSpan SpreadDelay;
    public TimeSpan RepairedSpreadDelay;
    public TimeSpan SpreadAt;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoWeedsSpreadingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoWeedsSpreadingComponent, ComponentGetState>(new ComponentEventRefHandler<XenoWeedsSpreadingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoWeedsSpreadingComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoWeedsSpreadingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      XenoWeedsSpreadingComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoWeedsSpreadingComponent.XenoWeedsSpreadingComponent_AutoState()
      {
        SpreadDelay = component.SpreadDelay,
        RepairedSpreadDelay = component.RepairedSpreadDelay,
        SpreadAt = component.SpreadAt
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoWeedsSpreadingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoWeedsSpreadingComponent.XenoWeedsSpreadingComponent_AutoState current))
        return;
      component.SpreadDelay = current.SpreadDelay;
      component.RepairedSpreadDelay = current.RepairedSpreadDelay;
      component.SpreadAt = current.SpreadAt;
    }
  }
}
