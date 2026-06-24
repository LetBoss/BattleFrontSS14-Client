// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.OrbitalCannon.OrbitalCannonExplosionComponent
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
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.OrbitalCannon;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[EntityCategory(new string[] {"Spawner"})]
[Access(new Type[] {typeof (OrbitalCannonSystem)})]
public sealed class OrbitalCannonExplosionComponent : 
  Component,
  ISerializationGenerated<OrbitalCannonExplosionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Laser;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<OrbitalCannonExplosion> Steps = new List<OrbitalCannonExplosion>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Current;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LastAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Step;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LastStepAt;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref OrbitalCannonExplosionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (OrbitalCannonExplosionComponent) target1;
    if (serialization.TryCustomCopy<OrbitalCannonExplosionComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Laser, ref target2, hookCtx, false, context))
      target2 = this.Laser;
    target.Laser = target2;
    List<OrbitalCannonExplosion> target3 = (List<OrbitalCannonExplosion>) null;
    if (this.Steps == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<OrbitalCannonExplosion>>(this.Steps, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<OrbitalCannonExplosion>>(this.Steps, hookCtx, context);
    target.Steps = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.Current, ref target4, hookCtx, false, context))
      target4 = this.Current;
    target.Current = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastAt, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.LastAt, hookCtx, context);
    target.LastAt = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.Step, ref target6, hookCtx, false, context))
      target6 = this.Step;
    target.Step = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastStepAt, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.LastStepAt, hookCtx, context);
    target.LastStepAt = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref OrbitalCannonExplosionComponent target,
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
    OrbitalCannonExplosionComponent target1 = (OrbitalCannonExplosionComponent) target;
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
    OrbitalCannonExplosionComponent target1 = (OrbitalCannonExplosionComponent) target;
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
    OrbitalCannonExplosionComponent target1 = (OrbitalCannonExplosionComponent) target;
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
  virtual OrbitalCannonExplosionComponent Component.Instantiate()
  {
    return new OrbitalCannonExplosionComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class OrbitalCannonExplosionComponent_AutoState : IComponentState
  {
    public bool Laser;
    public List<OrbitalCannonExplosion> Steps;
    public int Current;
    public TimeSpan LastAt;
    public int Step;
    public TimeSpan LastStepAt;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class OrbitalCannonExplosionComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<OrbitalCannonExplosionComponent, ComponentGetState>(new ComponentEventRefHandler<OrbitalCannonExplosionComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<OrbitalCannonExplosionComponent, ComponentHandleState>(new ComponentEventRefHandler<OrbitalCannonExplosionComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      OrbitalCannonExplosionComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new OrbitalCannonExplosionComponent.OrbitalCannonExplosionComponent_AutoState()
      {
        Laser = component.Laser,
        Steps = component.Steps,
        Current = component.Current,
        LastAt = component.LastAt,
        Step = component.Step,
        LastStepAt = component.LastStepAt
      };
    }

    private void OnHandleState(
      EntityUid uid,
      OrbitalCannonExplosionComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is OrbitalCannonExplosionComponent.OrbitalCannonExplosionComponent_AutoState current))
        return;
      component.Laser = current.Laser;
      component.Steps = current.Steps == null ? (List<OrbitalCannonExplosion>) null : new List<OrbitalCannonExplosion>((IEnumerable<OrbitalCannonExplosion>) current.Steps);
      component.Current = current.Current;
      component.LastAt = current.LastAt;
      component.Step = current.Step;
      component.LastStepAt = current.LastStepAt;
    }
  }
}
