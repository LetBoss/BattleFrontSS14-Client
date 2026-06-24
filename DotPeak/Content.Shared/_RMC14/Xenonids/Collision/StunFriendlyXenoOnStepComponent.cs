// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Collision.StunFriendlyXenoOnStepComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.StatusEffect;
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
namespace Content.Shared._RMC14.Xenonids.Collision;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoCollisionSystem)})]
public sealed class StunFriendlyXenoOnStepComponent : 
  Component,
  ISerializationGenerated<StunFriendlyXenoOnStepComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Ratio = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Cooldown = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Duration = TimeSpan.FromSeconds(0.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<StatusEffectPrototype> DisableStatus = (ProtoId<StatusEffectPrototype>) "KnockedDown";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StunFriendlyXenoOnStepComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StunFriendlyXenoOnStepComponent) target1;
    if (serialization.TryCustomCopy<StunFriendlyXenoOnStepComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target2, hookCtx, false, context))
      target2 = this.Enabled;
    target.Enabled = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Ratio, ref target3, hookCtx, false, context))
      target3 = this.Ratio;
    target.Ratio = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context);
    target.Cooldown = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target5;
    ProtoId<StatusEffectPrototype> target6 = new ProtoId<StatusEffectPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<StatusEffectPrototype>>(this.DisableStatus, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<ProtoId<StatusEffectPrototype>>(this.DisableStatus, hookCtx, context);
    target.DisableStatus = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StunFriendlyXenoOnStepComponent target,
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
    StunFriendlyXenoOnStepComponent target1 = (StunFriendlyXenoOnStepComponent) target;
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
    StunFriendlyXenoOnStepComponent target1 = (StunFriendlyXenoOnStepComponent) target;
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
    StunFriendlyXenoOnStepComponent target1 = (StunFriendlyXenoOnStepComponent) target;
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
  virtual StunFriendlyXenoOnStepComponent Component.Instantiate()
  {
    return new StunFriendlyXenoOnStepComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class StunFriendlyXenoOnStepComponent_AutoState : IComponentState
  {
    public bool Enabled;
    public float Ratio;
    public TimeSpan Cooldown;
    public TimeSpan Duration;
    public ProtoId<StatusEffectPrototype> DisableStatus;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class StunFriendlyXenoOnStepComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<StunFriendlyXenoOnStepComponent, ComponentGetState>(new ComponentEventRefHandler<StunFriendlyXenoOnStepComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<StunFriendlyXenoOnStepComponent, ComponentHandleState>(new ComponentEventRefHandler<StunFriendlyXenoOnStepComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      StunFriendlyXenoOnStepComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new StunFriendlyXenoOnStepComponent.StunFriendlyXenoOnStepComponent_AutoState()
      {
        Enabled = component.Enabled,
        Ratio = component.Ratio,
        Cooldown = component.Cooldown,
        Duration = component.Duration,
        DisableStatus = component.DisableStatus
      };
    }

    private void OnHandleState(
      EntityUid uid,
      StunFriendlyXenoOnStepComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is StunFriendlyXenoOnStepComponent.StunFriendlyXenoOnStepComponent_AutoState current))
        return;
      component.Enabled = current.Enabled;
      component.Ratio = current.Ratio;
      component.Cooldown = current.Cooldown;
      component.Duration = current.Duration;
      component.DisableStatus = current.DisableStatus;
    }
  }
}
