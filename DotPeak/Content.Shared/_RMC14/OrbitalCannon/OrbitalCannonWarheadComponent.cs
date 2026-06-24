// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.OrbitalCannon.OrbitalCannonWarheadComponent
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
namespace Content.Shared._RMC14.OrbitalCannon;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (OrbitalCannonSystem)})]
public sealed class OrbitalCannonWarheadComponent : 
  Component,
  ISerializationGenerated<OrbitalCannonWarheadComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<OrbitalCannonExplosionComponent> Explosion;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsAegis;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int FirstWarningRange = 30;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int SecondWarningRange = 25;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ThirdWarningRange = 15;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 IntelPointsAwarded = FixedPoint2.Zero;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref OrbitalCannonWarheadComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (OrbitalCannonWarheadComponent) target1;
    if (serialization.TryCustomCopy<OrbitalCannonWarheadComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId<OrbitalCannonExplosionComponent> target2 = new EntProtoId<OrbitalCannonExplosionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<OrbitalCannonExplosionComponent>>(this.Explosion, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId<OrbitalCannonExplosionComponent>>(this.Explosion, hookCtx, context);
    target.Explosion = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsAegis, ref target3, hookCtx, false, context))
      target3 = this.IsAegis;
    target.IsAegis = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.FirstWarningRange, ref target4, hookCtx, false, context))
      target4 = this.FirstWarningRange;
    target.FirstWarningRange = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.SecondWarningRange, ref target5, hookCtx, false, context))
      target5 = this.SecondWarningRange;
    target.SecondWarningRange = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.ThirdWarningRange, ref target6, hookCtx, false, context))
      target6 = this.ThirdWarningRange;
    target.ThirdWarningRange = target6;
    FixedPoint2 target7 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.IntelPointsAwarded, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<FixedPoint2>(this.IntelPointsAwarded, hookCtx, context);
    target.IntelPointsAwarded = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref OrbitalCannonWarheadComponent target,
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
    OrbitalCannonWarheadComponent target1 = (OrbitalCannonWarheadComponent) target;
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
    OrbitalCannonWarheadComponent target1 = (OrbitalCannonWarheadComponent) target;
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
    OrbitalCannonWarheadComponent target1 = (OrbitalCannonWarheadComponent) target;
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
  virtual OrbitalCannonWarheadComponent Component.Instantiate()
  {
    return new OrbitalCannonWarheadComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class OrbitalCannonWarheadComponent_AutoState : IComponentState
  {
    public EntProtoId<OrbitalCannonExplosionComponent> Explosion;
    public bool IsAegis;
    public int FirstWarningRange;
    public int SecondWarningRange;
    public int ThirdWarningRange;
    public FixedPoint2 IntelPointsAwarded;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class OrbitalCannonWarheadComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<OrbitalCannonWarheadComponent, ComponentGetState>(new ComponentEventRefHandler<OrbitalCannonWarheadComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<OrbitalCannonWarheadComponent, ComponentHandleState>(new ComponentEventRefHandler<OrbitalCannonWarheadComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      OrbitalCannonWarheadComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new OrbitalCannonWarheadComponent.OrbitalCannonWarheadComponent_AutoState()
      {
        Explosion = component.Explosion,
        IsAegis = component.IsAegis,
        FirstWarningRange = component.FirstWarningRange,
        SecondWarningRange = component.SecondWarningRange,
        ThirdWarningRange = component.ThirdWarningRange,
        IntelPointsAwarded = component.IntelPointsAwarded
      };
    }

    private void OnHandleState(
      EntityUid uid,
      OrbitalCannonWarheadComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is OrbitalCannonWarheadComponent.OrbitalCannonWarheadComponent_AutoState current))
        return;
      component.Explosion = current.Explosion;
      component.IsAegis = current.IsAegis;
      component.FirstWarningRange = current.FirstWarningRange;
      component.SecondWarningRange = current.SecondWarningRange;
      component.ThirdWarningRange = current.ThirdWarningRange;
      component.IntelPointsAwarded = current.IntelPointsAwarded;
    }
  }
}
