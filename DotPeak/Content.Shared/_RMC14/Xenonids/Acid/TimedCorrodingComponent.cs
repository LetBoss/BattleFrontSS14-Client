// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Acid.TimedCorrodingComponent
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
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Acid;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedXenoAcidSystem)})]
public sealed class TimedCorrodingComponent : 
  Component,
  ISerializationGenerated<TimedCorrodingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid Acid;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId AcidPrototype;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public XenoAcidStrength Strength = XenoAcidStrength.Normal;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan CorrodesAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Dps;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float LightDps;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TimedCorrodingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TimedCorrodingComponent) target1;
    if (serialization.TryCustomCopy<TimedCorrodingComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid target2 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.Acid, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid>(this.Acid, hookCtx, context);
    target.Acid = target2;
    EntProtoId target3 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.AcidPrototype, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId>(this.AcidPrototype, hookCtx, context);
    target.AcidPrototype = target3;
    XenoAcidStrength target4 = (XenoAcidStrength) 0;
    if (!serialization.TryCustomCopy<XenoAcidStrength>(this.Strength, ref target4, hookCtx, false, context))
      target4 = this.Strength;
    target.Strength = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CorrodesAt, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.CorrodesAt, hookCtx, context);
    target.CorrodesAt = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Dps, ref target6, hookCtx, false, context))
      target6 = this.Dps;
    target.Dps = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LightDps, ref target7, hookCtx, false, context))
      target7 = this.LightDps;
    target.LightDps = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TimedCorrodingComponent target,
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
    TimedCorrodingComponent target1 = (TimedCorrodingComponent) target;
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
    TimedCorrodingComponent target1 = (TimedCorrodingComponent) target;
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
    TimedCorrodingComponent target1 = (TimedCorrodingComponent) target;
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
  virtual TimedCorrodingComponent Component.Instantiate() => new TimedCorrodingComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TimedCorrodingComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TimedCorrodingComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<TimedCorrodingComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      TimedCorrodingComponent component,
      ref EntityUnpausedEvent args)
    {
      component.CorrodesAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class TimedCorrodingComponent_AutoState : IComponentState
  {
    public NetEntity Acid;
    public EntProtoId AcidPrototype;
    public XenoAcidStrength Strength;
    public TimeSpan CorrodesAt;
    public float Dps;
    public float LightDps;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TimedCorrodingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TimedCorrodingComponent, ComponentGetState>(new ComponentEventRefHandler<TimedCorrodingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<TimedCorrodingComponent, ComponentHandleState>(new ComponentEventRefHandler<TimedCorrodingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      TimedCorrodingComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new TimedCorrodingComponent.TimedCorrodingComponent_AutoState()
      {
        Acid = this.GetNetEntity(component.Acid),
        AcidPrototype = component.AcidPrototype,
        Strength = component.Strength,
        CorrodesAt = component.CorrodesAt,
        Dps = component.Dps,
        LightDps = component.LightDps
      };
    }

    private void OnHandleState(
      EntityUid uid,
      TimedCorrodingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is TimedCorrodingComponent.TimedCorrodingComponent_AutoState current))
        return;
      component.Acid = this.EnsureEntity<TimedCorrodingComponent>(current.Acid, uid);
      component.AcidPrototype = current.AcidPrototype;
      component.Strength = current.Strength;
      component.CorrodesAt = current.CorrodesAt;
      component.Dps = current.Dps;
      component.LightDps = current.LightDps;
    }
  }
}
