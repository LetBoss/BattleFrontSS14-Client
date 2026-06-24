// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.Components.StaminaComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Damage.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
public sealed class StaminaComponent : 
  Component,
  ISerializationGenerated<StaminaComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Critical;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Decay = 3f;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Cooldown = 3f;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float StaminaDamage;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float CritThreshold = 100f;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan StunTime = TimeSpan.FromSeconds(6L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextUpdate = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertPrototype> StaminaAlert = ProtoId<AlertPrototype>.op_Implicit("Stamina");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ShowAlert;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AfterCritical;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float AfterCritDecayMultiplier = 5f;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<FixedPoint2, float> StunModifierThresholds = new Dictionary<FixedPoint2, float>()
  {
    {
      (FixedPoint2) 0,
      1f
    },
    {
      (FixedPoint2) 60,
      0.7f
    },
    {
      (FixedPoint2) 80 /*0x50*/,
      0.5f
    }
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StaminaComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (StaminaComponent) component;
    if (serialization.TryCustomCopy<StaminaComponent>(this, ref target, hookCtx, false, context))
      return;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.Critical, ref flag1, hookCtx, false, context))
      flag1 = this.Critical;
    target.Critical = flag1;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Decay, ref num1, hookCtx, false, context))
      num1 = this.Decay;
    target.Decay = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Cooldown, ref num2, hookCtx, false, context))
      num2 = this.Cooldown;
    target.Cooldown = num2;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StaminaDamage, ref num3, hookCtx, false, context))
      num3 = this.StaminaDamage;
    target.StaminaDamage = num3;
    float num4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CritThreshold, ref num4, hookCtx, false, context))
      num4 = this.CritThreshold;
    target.CritThreshold = num4;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StunTime, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.StunTime, hookCtx, context, false);
    target.StunTime = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextUpdate, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.NextUpdate, hookCtx, context, false);
    target.NextUpdate = timeSpan2;
    ProtoId<AlertPrototype> protoId = new ProtoId<AlertPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(this.StaminaAlert, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<AlertPrototype>>(this.StaminaAlert, hookCtx, context, false);
    target.StaminaAlert = protoId;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowAlert, ref flag2, hookCtx, false, context))
      flag2 = this.ShowAlert;
    target.ShowAlert = flag2;
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this.AfterCritical, ref flag3, hookCtx, false, context))
      flag3 = this.AfterCritical;
    target.AfterCritical = flag3;
    float num5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AfterCritDecayMultiplier, ref num5, hookCtx, false, context))
      num5 = this.AfterCritDecayMultiplier;
    target.AfterCritDecayMultiplier = num5;
    Dictionary<FixedPoint2, float> dictionary = (Dictionary<FixedPoint2, float>) null;
    if (this.StunModifierThresholds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<FixedPoint2, float>>(this.StunModifierThresholds, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<FixedPoint2, float>>(this.StunModifierThresholds, hookCtx, context, false);
    target.StunModifierThresholds = dictionary;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StaminaComponent target,
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
    StaminaComponent target1 = (StaminaComponent) target;
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
    StaminaComponent target1 = (StaminaComponent) target;
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
    StaminaComponent target1 = (StaminaComponent) target;
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
  virtual StaminaComponent Component.Instantiate() => new StaminaComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class StaminaComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<StaminaComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<StaminaComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      StaminaComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextUpdate += args.PausedTime;
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class StaminaComponent_AutoState : IComponentState
  {
    public bool Critical;
    public float Decay;
    public float Cooldown;
    public float StaminaDamage;
    public float CritThreshold;
    public TimeSpan NextUpdate;
    public bool ShowAlert;
    public bool AfterCritical;
    public float AfterCritDecayMultiplier;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class StaminaComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<StaminaComponent, ComponentGetState>(new ComponentEventRefHandler<StaminaComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<StaminaComponent, ComponentHandleState>(new ComponentEventRefHandler<StaminaComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, 
    #nullable enable
    StaminaComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new StaminaComponent.StaminaComponent_AutoState()
      {
        Critical = component.Critical,
        Decay = component.Decay,
        Cooldown = component.Cooldown,
        StaminaDamage = component.StaminaDamage,
        CritThreshold = component.CritThreshold,
        NextUpdate = component.NextUpdate,
        ShowAlert = component.ShowAlert,
        AfterCritical = component.AfterCritical,
        AfterCritDecayMultiplier = component.AfterCritDecayMultiplier
      };
    }

    private void OnHandleState(
      EntityUid uid,
      StaminaComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is StaminaComponent.StaminaComponent_AutoState current))
        return;
      component.Critical = current.Critical;
      component.Decay = current.Decay;
      component.Cooldown = current.Cooldown;
      component.StaminaDamage = current.StaminaDamage;
      component.CritThreshold = current.CritThreshold;
      component.NextUpdate = current.NextUpdate;
      component.ShowAlert = current.ShowAlert;
      component.AfterCritical = current.AfterCritical;
      component.AfterCritDecayMultiplier = current.AfterCritDecayMultiplier;
      AfterAutoHandleStateEvent handleStateEvent;
      // ISSUE: explicit constructor call
      ((AfterAutoHandleStateEvent) ref handleStateEvent).\u002Ector(((ComponentHandleState) ref args).Current);
      ((IDirectedEventBus) this.EntityManager.EventBus).RaiseComponentEvent<AfterAutoHandleStateEvent, StaminaComponent>(uid, component, ref handleStateEvent);
    }
  }
}
