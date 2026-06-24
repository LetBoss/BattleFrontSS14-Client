// Decompiled with JetBrains decompiler
// Type: Content.Shared.Buckle.Components.BuckleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Buckle.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedBuckleSystem)})]
public sealed class BuckleComponent : 
  Component,
  ISerializationGenerated<BuckleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float Range = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DontCollide;
  [DataField(null, false, 1, false, false, null)]
  public bool PullStrap;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Delay = TimeSpan.FromSeconds(0.25);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  [AutoNetworkedField]
  public TimeSpan? BuckleTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? BuckledTo;
  [DataField(null, false, 1, false, false, null)]
  public int Size = 100;
  [Robust.Shared.ViewVariables.ViewVariables]
  public int? OriginalDrawDepth;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float? BuckleDelay;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ClickUnbuckle = true;

  [MemberNotNullWhen(true, "BuckledTo")]
  public bool Buckled
  {
    [MemberNotNullWhen(true, "BuckledTo")] get => this.BuckledTo.HasValue;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BuckleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (BuckleComponent) component;
    if (serialization.TryCustomCopy<BuckleComponent>(this, ref target, hookCtx, false, context))
      return;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref num1, hookCtx, false, context))
      num1 = this.Range;
    target.Range = num1;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.DontCollide, ref flag1, hookCtx, false, context))
      flag1 = this.DontCollide;
    target.DontCollide = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.PullStrap, ref flag2, hookCtx, false, context))
      flag2 = this.PullStrap;
    target.PullStrap = flag2;
    TimeSpan timeSpan = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref timeSpan, hookCtx, false, context))
      timeSpan = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context, false);
    target.Delay = timeSpan;
    TimeSpan? nullable1 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.BuckleTime, ref nullable1, hookCtx, false, context))
      nullable1 = serialization.CreateCopy<TimeSpan?>(this.BuckleTime, hookCtx, context, false);
    target.BuckleTime = nullable1;
    EntityUid? nullable2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.BuckledTo, ref nullable2, hookCtx, false, context))
      nullable2 = serialization.CreateCopy<EntityUid?>(this.BuckledTo, hookCtx, context, false);
    target.BuckledTo = nullable2;
    int num2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Size, ref num2, hookCtx, false, context))
      num2 = this.Size;
    target.Size = num2;
    float? nullable3 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.BuckleDelay, ref nullable3, hookCtx, false, context))
      nullable3 = this.BuckleDelay;
    target.BuckleDelay = nullable3;
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this.ClickUnbuckle, ref flag3, hookCtx, false, context))
      flag3 = this.ClickUnbuckle;
    target.ClickUnbuckle = flag3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BuckleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BuckleComponent target1 = (BuckleComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BuckleComponent target1 = (BuckleComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BuckleComponent target1 = (BuckleComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual BuckleComponent Component.Instantiate() => new BuckleComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BuckleComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<BuckleComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<BuckleComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      BuckleComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.BuckleTime.HasValue)
        component.BuckleTime = new TimeSpan?(component.BuckleTime.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class BuckleComponent_AutoState : IComponentState
  {
    public bool DontCollide;
    public TimeSpan? BuckleTime;
    public NetEntity? BuckledTo;
    public float? BuckleDelay;
    public bool ClickUnbuckle;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BuckleComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<BuckleComponent, ComponentGetState>(new ComponentEventRefHandler<BuckleComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<BuckleComponent, ComponentHandleState>(new ComponentEventRefHandler<BuckleComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, 
    #nullable enable
    BuckleComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new BuckleComponent.BuckleComponent_AutoState()
      {
        DontCollide = component.DontCollide,
        BuckleTime = component.BuckleTime,
        BuckledTo = this.GetNetEntity(component.BuckledTo, (MetaDataComponent) null),
        BuckleDelay = component.BuckleDelay,
        ClickUnbuckle = component.ClickUnbuckle
      };
    }

    private void OnHandleState(
      EntityUid uid,
      BuckleComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is BuckleComponent.BuckleComponent_AutoState current))
        return;
      component.DontCollide = current.DontCollide;
      component.BuckleTime = current.BuckleTime;
      component.BuckledTo = this.EnsureEntity<BuckleComponent>(current.BuckledTo, uid);
      component.BuckleDelay = current.BuckleDelay;
      component.ClickUnbuckle = current.ClickUnbuckle;
      AfterAutoHandleStateEvent handleStateEvent;
      // ISSUE: explicit constructor call
      ((AfterAutoHandleStateEvent) ref handleStateEvent).\u002Ector(((ComponentHandleState) ref args).Current);
      ((IDirectedEventBus) this.EntityManager.EventBus).RaiseComponentEvent<AfterAutoHandleStateEvent, BuckleComponent>(uid, component, ref handleStateEvent);
    }
  }
}
