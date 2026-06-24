// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Parasite.XenoParasiteComponent
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
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Parasite;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedXenoParasiteSystem)})]
public sealed class XenoParasiteComponent : 
  Component,
  ISerializationGenerated<XenoParasiteComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ManualAttachDelay = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SelfAttachDelay = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ParalyzeTime = TimeSpan.FromMinutes(1.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float InfectRange = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? InfectedVictim;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan FallOffDelay = TimeSpan.FromSeconds(15L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? FallOffAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool FellOff;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoParasiteComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoParasiteComponent) target1;
    if (serialization.TryCustomCopy<XenoParasiteComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ManualAttachDelay, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.ManualAttachDelay, hookCtx, context);
    target.ManualAttachDelay = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SelfAttachDelay, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.SelfAttachDelay, hookCtx, context);
    target.SelfAttachDelay = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ParalyzeTime, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.ParalyzeTime, hookCtx, context);
    target.ParalyzeTime = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.InfectRange, ref target5, hookCtx, false, context))
      target5 = this.InfectRange;
    target.InfectRange = target5;
    EntityUid? target6 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.InfectedVictim, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntityUid?>(this.InfectedVictim, hookCtx, context);
    target.InfectedVictim = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FallOffDelay, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.FallOffDelay, hookCtx, context);
    target.FallOffDelay = target7;
    TimeSpan? target8 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.FallOffAt, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan?>(this.FallOffAt, hookCtx, context);
    target.FallOffAt = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.FellOff, ref target9, hookCtx, false, context))
      target9 = this.FellOff;
    target.FellOff = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoParasiteComponent target,
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
    XenoParasiteComponent target1 = (XenoParasiteComponent) target;
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
    XenoParasiteComponent target1 = (XenoParasiteComponent) target;
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
    XenoParasiteComponent target1 = (XenoParasiteComponent) target;
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
  virtual XenoParasiteComponent Component.Instantiate() => new XenoParasiteComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoParasiteComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoParasiteComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoParasiteComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoParasiteComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.FallOffAt.HasValue)
        component.FallOffAt = new TimeSpan?(component.FallOffAt.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoParasiteComponent_AutoState : IComponentState
  {
    public TimeSpan ManualAttachDelay;
    public TimeSpan SelfAttachDelay;
    public TimeSpan ParalyzeTime;
    public float InfectRange;
    public NetEntity? InfectedVictim;
    public TimeSpan FallOffDelay;
    public TimeSpan? FallOffAt;
    public bool FellOff;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoParasiteComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoParasiteComponent, ComponentGetState>(new ComponentEventRefHandler<XenoParasiteComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoParasiteComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoParasiteComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      XenoParasiteComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoParasiteComponent.XenoParasiteComponent_AutoState()
      {
        ManualAttachDelay = component.ManualAttachDelay,
        SelfAttachDelay = component.SelfAttachDelay,
        ParalyzeTime = component.ParalyzeTime,
        InfectRange = component.InfectRange,
        InfectedVictim = this.GetNetEntity(component.InfectedVictim),
        FallOffDelay = component.FallOffDelay,
        FallOffAt = component.FallOffAt,
        FellOff = component.FellOff
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoParasiteComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoParasiteComponent.XenoParasiteComponent_AutoState current))
        return;
      component.ManualAttachDelay = current.ManualAttachDelay;
      component.SelfAttachDelay = current.SelfAttachDelay;
      component.ParalyzeTime = current.ParalyzeTime;
      component.InfectRange = current.InfectRange;
      component.InfectedVictim = this.EnsureEntity<XenoParasiteComponent>(current.InfectedVictim, uid);
      component.FallOffDelay = current.FallOffDelay;
      component.FallOffAt = current.FallOffAt;
      component.FellOff = current.FellOff;
    }
  }
}
