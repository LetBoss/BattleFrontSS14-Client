// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Weeds.DamageOffWeedsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Weeds;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedXenoWeedsSystem)})]
public sealed class DamageOffWeedsComponent : 
  Component,
  ISerializationGenerated<DamageOffWeedsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier Damage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? DamageAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Every = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RestingStopsDamage = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DamageOffWeedsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DamageOffWeedsComponent) target1;
    if (serialization.TryCustomCopy<DamageOffWeedsComponent>(this, ref target, hookCtx, false, context))
      return;
    DamageSpecifier target2 = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target2, hookCtx, false, context))
    {
      if (this.Damage == null)
        target2 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target2, hookCtx, context, true);
    }
    target.Damage = target2;
    TimeSpan? target3 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.DamageAt, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan?>(this.DamageAt, hookCtx, context);
    target.DamageAt = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Every, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.Every, hookCtx, context);
    target.Every = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.RestingStopsDamage, ref target5, hookCtx, false, context))
      target5 = this.RestingStopsDamage;
    target.RestingStopsDamage = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DamageOffWeedsComponent target,
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
    DamageOffWeedsComponent target1 = (DamageOffWeedsComponent) target;
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
    DamageOffWeedsComponent target1 = (DamageOffWeedsComponent) target;
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
    DamageOffWeedsComponent target1 = (DamageOffWeedsComponent) target;
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
  virtual DamageOffWeedsComponent Component.Instantiate() => new DamageOffWeedsComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DamageOffWeedsComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DamageOffWeedsComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<DamageOffWeedsComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      DamageOffWeedsComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.DamageAt.HasValue)
        component.DamageAt = new TimeSpan?(component.DamageAt.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DamageOffWeedsComponent_AutoState : IComponentState
  {
    public 
    #nullable enable
    DamageSpecifier Damage;
    public TimeSpan? DamageAt;
    public TimeSpan Every;
    public bool RestingStopsDamage;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DamageOffWeedsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DamageOffWeedsComponent, ComponentGetState>(new ComponentEventRefHandler<DamageOffWeedsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DamageOffWeedsComponent, ComponentHandleState>(new ComponentEventRefHandler<DamageOffWeedsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DamageOffWeedsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DamageOffWeedsComponent.DamageOffWeedsComponent_AutoState()
      {
        Damage = component.Damage,
        DamageAt = component.DamageAt,
        Every = component.Every,
        RestingStopsDamage = component.RestingStopsDamage
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DamageOffWeedsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DamageOffWeedsComponent.DamageOffWeedsComponent_AutoState current))
        return;
      component.Damage = current.Damage;
      component.DamageAt = current.DamageAt;
      component.Every = current.Every;
      component.RestingStopsDamage = current.RestingStopsDamage;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, DamageOffWeedsComponent>(uid, component, ref args1);
    }
  }
}
