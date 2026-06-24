// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Egg.XenoFragileEggComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Egg;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoFragileEggComponent : 
  Component,
  ISerializationGenerated<XenoFragileEggComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? ExpireAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? ShortExpireAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? CheckSustainAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? SustainedBy;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SustainRange = 14f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? BurstAt;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan BurstDelay = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan SustainDuration = TimeSpan.FromMinutes(1L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan SustainCheckEvery = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool InRange = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoFragileEggComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoFragileEggComponent) target1;
    if (serialization.TryCustomCopy<XenoFragileEggComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan? target2 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.ExpireAt, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan?>(this.ExpireAt, hookCtx, context);
    target.ExpireAt = target2;
    TimeSpan? target3 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.ShortExpireAt, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan?>(this.ShortExpireAt, hookCtx, context);
    target.ShortExpireAt = target3;
    TimeSpan? target4 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.CheckSustainAt, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan?>(this.CheckSustainAt, hookCtx, context);
    target.CheckSustainAt = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.SustainedBy, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.SustainedBy, hookCtx, context);
    target.SustainedBy = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SustainRange, ref target6, hookCtx, false, context))
      target6 = this.SustainRange;
    target.SustainRange = target6;
    TimeSpan? target7 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.BurstAt, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan?>(this.BurstAt, hookCtx, context);
    target.BurstAt = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BurstDelay, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.BurstDelay, hookCtx, context);
    target.BurstDelay = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SustainDuration, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.SustainDuration, hookCtx, context);
    target.SustainDuration = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SustainCheckEvery, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.SustainCheckEvery, hookCtx, context);
    target.SustainCheckEvery = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.InRange, ref target11, hookCtx, false, context))
      target11 = this.InRange;
    target.InRange = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoFragileEggComponent target,
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
    XenoFragileEggComponent target1 = (XenoFragileEggComponent) target;
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
    XenoFragileEggComponent target1 = (XenoFragileEggComponent) target;
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
    XenoFragileEggComponent target1 = (XenoFragileEggComponent) target;
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
  virtual XenoFragileEggComponent Component.Instantiate() => new XenoFragileEggComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoFragileEggComponent_AutoState : IComponentState
  {
    public TimeSpan? ExpireAt;
    public TimeSpan? ShortExpireAt;
    public TimeSpan? CheckSustainAt;
    public NetEntity? SustainedBy;
    public float SustainRange;
    public TimeSpan? BurstAt;
    public bool InRange;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoFragileEggComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoFragileEggComponent, ComponentGetState>(new ComponentEventRefHandler<XenoFragileEggComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoFragileEggComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoFragileEggComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoFragileEggComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoFragileEggComponent.XenoFragileEggComponent_AutoState()
      {
        ExpireAt = component.ExpireAt,
        ShortExpireAt = component.ShortExpireAt,
        CheckSustainAt = component.CheckSustainAt,
        SustainedBy = this.GetNetEntity(component.SustainedBy),
        SustainRange = component.SustainRange,
        BurstAt = component.BurstAt,
        InRange = component.InRange
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoFragileEggComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoFragileEggComponent.XenoFragileEggComponent_AutoState current))
        return;
      component.ExpireAt = current.ExpireAt;
      component.ShortExpireAt = current.ShortExpireAt;
      component.CheckSustainAt = current.CheckSustainAt;
      component.SustainedBy = this.EnsureEntity<XenoFragileEggComponent>(current.SustainedBy, uid);
      component.SustainRange = current.SustainRange;
      component.BurstAt = current.BurstAt;
      component.InRange = current.InRange;
    }
  }
}
