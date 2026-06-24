// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Projectile.Spit.Ball.XenoAcidBallComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared._RMC14.Xenonids.Projectile.Spit.Ball;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoSpitSystem)})]
public sealed class XenoAcidBallComponent : 
  Component,
  ISerializationGenerated<XenoAcidBallComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PlasmaCost = (FixedPoint2) 80 /*0x50*/;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Speed = 15f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Delay = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId ProjectileId = (EntProtoId) "XenoAcidBallProjectile";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MaxRange = 6f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Cooldown = TimeSpan.FromSeconds(18L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoAcidBallComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoAcidBallComponent) target1;
    if (serialization.TryCustomCopy<XenoAcidBallComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaCost, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.PlasmaCost, hookCtx, context);
    target.PlasmaCost = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Speed, ref target3, hookCtx, false, context))
      target3 = this.Speed;
    target.Speed = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target4;
    EntProtoId target5 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ProjectileId, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId>(this.ProjectileId, hookCtx, context);
    target.ProjectileId = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxRange, ref target6, hookCtx, false, context))
      target6 = this.MaxRange;
    target.MaxRange = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context);
    target.Cooldown = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoAcidBallComponent target,
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
    XenoAcidBallComponent target1 = (XenoAcidBallComponent) target;
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
    XenoAcidBallComponent target1 = (XenoAcidBallComponent) target;
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
    XenoAcidBallComponent target1 = (XenoAcidBallComponent) target;
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
  virtual XenoAcidBallComponent Component.Instantiate() => new XenoAcidBallComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoAcidBallComponent_AutoState : IComponentState
  {
    public FixedPoint2 PlasmaCost;
    public float Speed;
    public TimeSpan Delay;
    public EntProtoId ProjectileId;
    public float MaxRange;
    public TimeSpan Cooldown;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoAcidBallComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoAcidBallComponent, ComponentGetState>(new ComponentEventRefHandler<XenoAcidBallComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoAcidBallComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoAcidBallComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoAcidBallComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoAcidBallComponent.XenoAcidBallComponent_AutoState()
      {
        PlasmaCost = component.PlasmaCost,
        Speed = component.Speed,
        Delay = component.Delay,
        ProjectileId = component.ProjectileId,
        MaxRange = component.MaxRange,
        Cooldown = component.Cooldown
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoAcidBallComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoAcidBallComponent.XenoAcidBallComponent_AutoState current))
        return;
      component.PlasmaCost = current.PlasmaCost;
      component.Speed = current.Speed;
      component.Delay = current.Delay;
      component.ProjectileId = current.ProjectileId;
      component.MaxRange = current.MaxRange;
      component.Cooldown = current.Cooldown;
    }
  }
}
