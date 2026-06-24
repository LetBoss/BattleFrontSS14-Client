// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Zoom.XenoZoomComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Zoom;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoZoomSystem)})]
public sealed class XenoZoomComponent : 
  Component,
  ISerializationGenerated<XenoZoomComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 Zoom = new Vector2(1.25f, 1.25f);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int OffsetLength;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 Offset;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Speed = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DoAfter = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BlockLeaps;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoZoomComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoZoomComponent) target1;
    if (serialization.TryCustomCopy<XenoZoomComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target2, hookCtx, false, context))
      target2 = this.Enabled;
    target.Enabled = target2;
    Vector2 target3 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Zoom, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Vector2>(this.Zoom, hookCtx, context);
    target.Zoom = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.OffsetLength, ref target4, hookCtx, false, context))
      target4 = this.OffsetLength;
    target.OffsetLength = target4;
    Vector2 target5 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Offset, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<Vector2>(this.Offset, hookCtx, context);
    target.Offset = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Speed, ref target6, hookCtx, false, context))
      target6 = this.Speed;
    target.Speed = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DoAfter, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.DoAfter, hookCtx, context);
    target.DoAfter = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.BlockLeaps, ref target8, hookCtx, false, context))
      target8 = this.BlockLeaps;
    target.BlockLeaps = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoZoomComponent target,
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
    XenoZoomComponent target1 = (XenoZoomComponent) target;
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
    XenoZoomComponent target1 = (XenoZoomComponent) target;
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
    XenoZoomComponent target1 = (XenoZoomComponent) target;
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
  virtual XenoZoomComponent Component.Instantiate() => new XenoZoomComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoZoomComponent_AutoState : IComponentState
  {
    public bool Enabled;
    public Vector2 Zoom;
    public int OffsetLength;
    public Vector2 Offset;
    public float Speed;
    public TimeSpan DoAfter;
    public bool BlockLeaps;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoZoomComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoZoomComponent, ComponentGetState>(new ComponentEventRefHandler<XenoZoomComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoZoomComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoZoomComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, XenoZoomComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoZoomComponent.XenoZoomComponent_AutoState()
      {
        Enabled = component.Enabled,
        Zoom = component.Zoom,
        OffsetLength = component.OffsetLength,
        Offset = component.Offset,
        Speed = component.Speed,
        DoAfter = component.DoAfter,
        BlockLeaps = component.BlockLeaps
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoZoomComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoZoomComponent.XenoZoomComponent_AutoState current))
        return;
      component.Enabled = current.Enabled;
      component.Zoom = current.Zoom;
      component.OffsetLength = current.OffsetLength;
      component.Offset = current.Offset;
      component.Speed = current.Speed;
      component.DoAfter = current.DoAfter;
      component.BlockLeaps = current.BlockLeaps;
    }
  }
}
