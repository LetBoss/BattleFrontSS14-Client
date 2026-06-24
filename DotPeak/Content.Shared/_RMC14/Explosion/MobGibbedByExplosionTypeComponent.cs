// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Explosion.MobGibbedByExplosionTypeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Explosion;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Explosion;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCExplosionSystem)})]
public sealed class MobGibbedByExplosionTypeComponent : 
  Component,
  ISerializationGenerated<MobGibbedByExplosionTypeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ExplosionPrototype>[] Explosions = new ProtoId<ExplosionPrototype>[2]
  {
    (ProtoId<ExplosionPrototype>) "RMCOB",
    (ProtoId<ExplosionPrototype>) "RMCOBXenoTunnel"
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Threshold = (FixedPoint2) 600;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MobGibbedByExplosionTypeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MobGibbedByExplosionTypeComponent) target1;
    if (serialization.TryCustomCopy<MobGibbedByExplosionTypeComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<ExplosionPrototype>[] target2 = (ProtoId<ExplosionPrototype>[]) null;
    if (this.Explosions == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ProtoId<ExplosionPrototype>[]>(this.Explosions, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<ProtoId<ExplosionPrototype>[]>(this.Explosions, hookCtx, context);
    target.Explosions = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Threshold, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.Threshold, hookCtx, context);
    target.Threshold = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MobGibbedByExplosionTypeComponent target,
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
    MobGibbedByExplosionTypeComponent target1 = (MobGibbedByExplosionTypeComponent) target;
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
    MobGibbedByExplosionTypeComponent target1 = (MobGibbedByExplosionTypeComponent) target;
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
    MobGibbedByExplosionTypeComponent target1 = (MobGibbedByExplosionTypeComponent) target;
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
  virtual MobGibbedByExplosionTypeComponent Component.Instantiate()
  {
    return new MobGibbedByExplosionTypeComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MobGibbedByExplosionTypeComponent_AutoState : IComponentState
  {
    public ProtoId<ExplosionPrototype>[] Explosions;
    public FixedPoint2 Threshold;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MobGibbedByExplosionTypeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MobGibbedByExplosionTypeComponent, ComponentGetState>(new ComponentEventRefHandler<MobGibbedByExplosionTypeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MobGibbedByExplosionTypeComponent, ComponentHandleState>(new ComponentEventRefHandler<MobGibbedByExplosionTypeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      MobGibbedByExplosionTypeComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new MobGibbedByExplosionTypeComponent.MobGibbedByExplosionTypeComponent_AutoState()
      {
        Explosions = component.Explosions,
        Threshold = component.Threshold
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MobGibbedByExplosionTypeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MobGibbedByExplosionTypeComponent.MobGibbedByExplosionTypeComponent_AutoState current))
        return;
      component.Explosions = current.Explosions;
      component.Threshold = current.Threshold;
    }
  }
}
