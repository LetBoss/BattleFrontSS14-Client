// Decompiled with JetBrains decompiler
// Type: Content.Shared.Holopad.HolopadComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared.Holopad;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedHolopadSystem)})]
public sealed class HolopadComponent : 
  Component,
  ISerializationGenerated<HolopadComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public Entity<HolopadHologramComponent>? Hologram;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Entity<HolopadUserComponent>? User;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? HologramProtoId;
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public EntityUid? ControlLockoutOwner;
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public TimeSpan ControlLockoutStartTime;

  [DataField(null, false, 1, false, false, null)]
  public float ControlLockoutDuration { get; private set; } = 90f;

  [DataField(null, false, 1, false, false, null)]
  public float ControlLockoutCoolDown { get; private set; } = 180f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HolopadComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HolopadComponent) target1;
    if (serialization.TryCustomCopy<HolopadComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId? target2 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.HologramProtoId, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId?>(this.HologramProtoId, hookCtx, context);
    target.HologramProtoId = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ControlLockoutDuration, ref target3, hookCtx, false, context))
      target3 = this.ControlLockoutDuration;
    target.ControlLockoutDuration = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ControlLockoutCoolDown, ref target4, hookCtx, false, context))
      target4 = this.ControlLockoutCoolDown;
    target.ControlLockoutCoolDown = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HolopadComponent target,
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
    HolopadComponent target1 = (HolopadComponent) target;
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
    HolopadComponent target1 = (HolopadComponent) target;
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
    HolopadComponent target1 = (HolopadComponent) target;
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
  virtual HolopadComponent Component.Instantiate() => new HolopadComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class HolopadComponent_AutoState : IComponentState
  {
    public NetEntity? ControlLockoutOwner;
    public TimeSpan ControlLockoutStartTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HolopadComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<HolopadComponent, ComponentGetState>(new ComponentEventRefHandler<HolopadComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<HolopadComponent, ComponentHandleState>(new ComponentEventRefHandler<HolopadComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, HolopadComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new HolopadComponent.HolopadComponent_AutoState()
      {
        ControlLockoutOwner = this.GetNetEntity(component.ControlLockoutOwner),
        ControlLockoutStartTime = component.ControlLockoutStartTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      HolopadComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is HolopadComponent.HolopadComponent_AutoState current))
        return;
      component.ControlLockoutOwner = this.EnsureEntity<HolopadComponent>(current.ControlLockoutOwner, uid);
      component.ControlLockoutStartTime = current.ControlLockoutStartTime;
    }
  }
}
