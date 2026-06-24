// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Projectile.Spit.Scattered.XenoScatteredSpitComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Projectile.Spit.Scattered;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoSpitSystem)})]
public sealed class XenoScatteredSpitComponent : 
  Component,
  ISerializationGenerated<XenoScatteredSpitComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PlasmaCost = (FixedPoint2) 30;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Speed = 30f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId ProjectileId = (EntProtoId) "XenoScatteredSpitProjectile";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundCollectionSpecifier("XenoSpitAcid", new AudioParams?(AudioParams.Default.WithVolume(-10f)));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxProjectiles = 5;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle MaxDeviation = Angle.FromDegrees(90.0);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoScatteredSpitComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoScatteredSpitComponent) target1;
    if (serialization.TryCustomCopy<XenoScatteredSpitComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaCost, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.PlasmaCost, hookCtx, context);
    target.PlasmaCost = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Speed, ref target3, hookCtx, false, context))
      target3 = this.Speed;
    target.Speed = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ProjectileId, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.ProjectileId, hookCtx, context);
    target.ProjectileId = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxProjectiles, ref target6, hookCtx, false, context))
      target6 = this.MaxProjectiles;
    target.MaxProjectiles = target6;
    Angle target7 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.MaxDeviation, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<Angle>(this.MaxDeviation, hookCtx, context);
    target.MaxDeviation = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoScatteredSpitComponent target,
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
    XenoScatteredSpitComponent target1 = (XenoScatteredSpitComponent) target;
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
    XenoScatteredSpitComponent target1 = (XenoScatteredSpitComponent) target;
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
    XenoScatteredSpitComponent target1 = (XenoScatteredSpitComponent) target;
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
  virtual XenoScatteredSpitComponent Component.Instantiate() => new XenoScatteredSpitComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoScatteredSpitComponent_AutoState : IComponentState
  {
    public FixedPoint2 PlasmaCost;
    public float Speed;
    public EntProtoId ProjectileId;
    public SoundSpecifier Sound;
    public int MaxProjectiles;
    public Angle MaxDeviation;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoScatteredSpitComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoScatteredSpitComponent, ComponentGetState>(new ComponentEventRefHandler<XenoScatteredSpitComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoScatteredSpitComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoScatteredSpitComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoScatteredSpitComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoScatteredSpitComponent.XenoScatteredSpitComponent_AutoState()
      {
        PlasmaCost = component.PlasmaCost,
        Speed = component.Speed,
        ProjectileId = component.ProjectileId,
        Sound = component.Sound,
        MaxProjectiles = component.MaxProjectiles,
        MaxDeviation = component.MaxDeviation
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoScatteredSpitComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoScatteredSpitComponent.XenoScatteredSpitComponent_AutoState current))
        return;
      component.PlasmaCost = current.PlasmaCost;
      component.Speed = current.Speed;
      component.ProjectileId = current.ProjectileId;
      component.Sound = current.Sound;
      component.MaxProjectiles = current.MaxProjectiles;
      component.MaxDeviation = current.MaxDeviation;
    }
  }
}
