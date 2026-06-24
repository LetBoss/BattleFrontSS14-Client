// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Invisibility.XenoActiveInvisibleComponent
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
namespace Content.Shared._RMC14.Xenonids.Invisibility;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (XenoInvisibilitySystem)})]
public sealed class XenoActiveInvisibleComponent : 
  Component,
  ISerializationGenerated<XenoActiveInvisibleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan ExpiresAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan FullCooldown = TimeSpan.FromSeconds(20L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 SpeedMultiplier = FixedPoint2.New(1.15);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DidPopup;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoActiveInvisibleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoActiveInvisibleComponent) target1;
    if (serialization.TryCustomCopy<XenoActiveInvisibleComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ExpiresAt, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.ExpiresAt, hookCtx, context);
    target.ExpiresAt = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FullCooldown, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.FullCooldown, hookCtx, context);
    target.FullCooldown = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.SpeedMultiplier, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.SpeedMultiplier, hookCtx, context);
    target.SpeedMultiplier = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.DidPopup, ref target5, hookCtx, false, context))
      target5 = this.DidPopup;
    target.DidPopup = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoActiveInvisibleComponent target,
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
    XenoActiveInvisibleComponent target1 = (XenoActiveInvisibleComponent) target;
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
    XenoActiveInvisibleComponent target1 = (XenoActiveInvisibleComponent) target;
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
    XenoActiveInvisibleComponent target1 = (XenoActiveInvisibleComponent) target;
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
  virtual XenoActiveInvisibleComponent Component.Instantiate()
  {
    return new XenoActiveInvisibleComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoActiveInvisibleComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoActiveInvisibleComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoActiveInvisibleComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoActiveInvisibleComponent component,
      ref EntityUnpausedEvent args)
    {
      component.ExpiresAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoActiveInvisibleComponent_AutoState : IComponentState
  {
    public TimeSpan ExpiresAt;
    public TimeSpan FullCooldown;
    public FixedPoint2 SpeedMultiplier;
    public bool DidPopup;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoActiveInvisibleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoActiveInvisibleComponent, ComponentGetState>(new ComponentEventRefHandler<XenoActiveInvisibleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoActiveInvisibleComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoActiveInvisibleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      XenoActiveInvisibleComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoActiveInvisibleComponent.XenoActiveInvisibleComponent_AutoState()
      {
        ExpiresAt = component.ExpiresAt,
        FullCooldown = component.FullCooldown,
        SpeedMultiplier = component.SpeedMultiplier,
        DidPopup = component.DidPopup
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoActiveInvisibleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoActiveInvisibleComponent.XenoActiveInvisibleComponent_AutoState current))
        return;
      component.ExpiresAt = current.ExpiresAt;
      component.FullCooldown = current.FullCooldown;
      component.SpeedMultiplier = current.SpeedMultiplier;
      component.DidPopup = current.DidPopup;
    }
  }
}
