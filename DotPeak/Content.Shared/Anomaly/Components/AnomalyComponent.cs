// Decompiled with JetBrains decompiler
// Type: Content.Shared.Anomaly.Components.AnomalyComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Anomaly.Effects;
using Content.Shared.Anomaly.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Anomaly.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedAnomalySystem), typeof (SharedInnerBodyAnomalySystem)})]
public sealed class AnomalyComponent : 
  Component,
  ISerializationGenerated<AnomalyComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public float Stability;
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public float Severity;
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public float Health = 1f;
  [DataField("decayhreshold", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float DecayThreshold = 0.15f;
  [DataField("healthChangePerSecond", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float HealthChangePerSecond = -0.01f;
  [DataField("growthThreshold", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float GrowthThreshold = 0.5f;
  [DataField("severityGrowthCoefficient", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float SeverityGrowthCoefficient = 0.07f;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan NextPulseTime = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan MinPulseLength = TimeSpan.FromMinutes(2L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan MaxPulseLength = TimeSpan.FromMinutes(4L);
  [DataField(null, false, 1, false, false, null)]
  public float PulseVariation = 0.1f;
  [DataField(null, false, 1, false, false, null)]
  public Vector2 PulseStabilityVariation = new Vector2(-0.1f, 0.15f);
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? PulseSound = (SoundSpecifier) new SoundCollectionSpecifier("RadiationPulse", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? SupercriticalSound = (SoundSpecifier) new SoundCollectionSpecifier("Explosion", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? SupercriticalSoundAtAnimationStart;
  [DataField(null, false, 1, false, false, null)]
  public (float, float) InitialStabilityRange = (0.4f, 0.6f);
  [DataField(null, false, 1, false, false, null)]
  public (float, float) InitialSeverityRange = (0.1f, 0.5f);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public AnomalousParticleType SeverityParticleType;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public AnomalousParticleType DestabilizingParticleType;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public AnomalousParticleType WeakeningParticleType;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public AnomalousParticleType TransformationParticleType;
  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityUid? ConnectedVessel;
  [DataField(null, false, 1, false, false, null)]
  public int MinPointsPerSecond = 10;
  [DataField(null, false, 1, false, false, null)]
  public int MaxPointsPerSecond = 70;
  [DataField(null, false, 1, false, false, null)]
  public float GrowingPointMultiplier = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public EntProtoId? CorePrototype;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public EntProtoId? CoreInertPrototype;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AnomalyBehaviorPrototype>? CurrentBehavior;
  [DataField(null, false, 1, false, false, null)]
  public float Continuity;
  [DataField(null, false, 1, false, false, null)]
  public float MinContituty = 0.1f;
  [DataField(null, false, 1, false, false, null)]
  public float MaxContituty = 1f;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("animationTime", false, 1, false, false, null)]
  public float AnimationTime = 2f;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("offset", false, 1, false, false, null)]
  public Vector2 FloatingOffset = new Vector2(0.0f, 0.0f);
  public readonly string AnimationKey = "anomalyfloat";
  [DataField(null, false, 1, false, false, null)]
  public bool DeleteEntity = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AnomalyComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (AnomalyComponent) component;
    if (serialization.TryCustomCopy<AnomalyComponent>(this, ref target, hookCtx, false, context))
      return;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DecayThreshold, ref num1, hookCtx, false, context))
      num1 = this.DecayThreshold;
    target.DecayThreshold = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HealthChangePerSecond, ref num2, hookCtx, false, context))
      num2 = this.HealthChangePerSecond;
    target.HealthChangePerSecond = num2;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.GrowthThreshold, ref num3, hookCtx, false, context))
      num3 = this.GrowthThreshold;
    target.GrowthThreshold = num3;
    float num4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SeverityGrowthCoefficient, ref num4, hookCtx, false, context))
      num4 = this.SeverityGrowthCoefficient;
    target.SeverityGrowthCoefficient = num4;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextPulseTime, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.NextPulseTime, hookCtx, context, false);
    target.NextPulseTime = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MinPulseLength, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.MinPulseLength, hookCtx, context, false);
    target.MinPulseLength = timeSpan2;
    TimeSpan timeSpan3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MaxPulseLength, ref timeSpan3, hookCtx, false, context))
      timeSpan3 = serialization.CreateCopy<TimeSpan>(this.MaxPulseLength, hookCtx, context, false);
    target.MaxPulseLength = timeSpan3;
    float num5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PulseVariation, ref num5, hookCtx, false, context))
      num5 = this.PulseVariation;
    target.PulseVariation = num5;
    Vector2 vector2_1 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.PulseStabilityVariation, ref vector2_1, hookCtx, false, context))
      vector2_1 = serialization.CreateCopy<Vector2>(this.PulseStabilityVariation, hookCtx, context, false);
    target.PulseStabilityVariation = vector2_1;
    SoundSpecifier soundSpecifier1 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.PulseSound, ref soundSpecifier1, hookCtx, true, context))
      soundSpecifier1 = serialization.CreateCopy<SoundSpecifier>(this.PulseSound, hookCtx, context, false);
    target.PulseSound = soundSpecifier1;
    SoundSpecifier soundSpecifier2 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SupercriticalSound, ref soundSpecifier2, hookCtx, true, context))
      soundSpecifier2 = serialization.CreateCopy<SoundSpecifier>(this.SupercriticalSound, hookCtx, context, false);
    target.SupercriticalSound = soundSpecifier2;
    SoundSpecifier soundSpecifier3 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SupercriticalSoundAtAnimationStart, ref soundSpecifier3, hookCtx, true, context))
      soundSpecifier3 = serialization.CreateCopy<SoundSpecifier>(this.SupercriticalSoundAtAnimationStart, hookCtx, context, false);
    target.SupercriticalSoundAtAnimationStart = soundSpecifier3;
    (float, float) valueTuple1 = ();
    if (!serialization.TryCustomCopy<(float, float)>(this.InitialStabilityRange, ref valueTuple1, hookCtx, false, context))
      valueTuple1 = serialization.CreateCopy<(float, float)>(this.InitialStabilityRange, hookCtx, context, false);
    target.InitialStabilityRange = valueTuple1;
    (float, float) valueTuple2 = ();
    if (!serialization.TryCustomCopy<(float, float)>(this.InitialSeverityRange, ref valueTuple2, hookCtx, false, context))
      valueTuple2 = serialization.CreateCopy<(float, float)>(this.InitialSeverityRange, hookCtx, context, false);
    target.InitialSeverityRange = valueTuple2;
    AnomalousParticleType anomalousParticleType1 = AnomalousParticleType.Delta;
    if (!serialization.TryCustomCopy<AnomalousParticleType>(this.SeverityParticleType, ref anomalousParticleType1, hookCtx, false, context))
      anomalousParticleType1 = this.SeverityParticleType;
    target.SeverityParticleType = anomalousParticleType1;
    AnomalousParticleType anomalousParticleType2 = AnomalousParticleType.Delta;
    if (!serialization.TryCustomCopy<AnomalousParticleType>(this.DestabilizingParticleType, ref anomalousParticleType2, hookCtx, false, context))
      anomalousParticleType2 = this.DestabilizingParticleType;
    target.DestabilizingParticleType = anomalousParticleType2;
    AnomalousParticleType anomalousParticleType3 = AnomalousParticleType.Delta;
    if (!serialization.TryCustomCopy<AnomalousParticleType>(this.WeakeningParticleType, ref anomalousParticleType3, hookCtx, false, context))
      anomalousParticleType3 = this.WeakeningParticleType;
    target.WeakeningParticleType = anomalousParticleType3;
    AnomalousParticleType anomalousParticleType4 = AnomalousParticleType.Delta;
    if (!serialization.TryCustomCopy<AnomalousParticleType>(this.TransformationParticleType, ref anomalousParticleType4, hookCtx, false, context))
      anomalousParticleType4 = this.TransformationParticleType;
    target.TransformationParticleType = anomalousParticleType4;
    int num6 = 0;
    if (!serialization.TryCustomCopy<int>(this.MinPointsPerSecond, ref num6, hookCtx, false, context))
      num6 = this.MinPointsPerSecond;
    target.MinPointsPerSecond = num6;
    int num7 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxPointsPerSecond, ref num7, hookCtx, false, context))
      num7 = this.MaxPointsPerSecond;
    target.MaxPointsPerSecond = num7;
    float num8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.GrowingPointMultiplier, ref num8, hookCtx, false, context))
      num8 = this.GrowingPointMultiplier;
    target.GrowingPointMultiplier = num8;
    EntProtoId? nullable1 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.CorePrototype, ref nullable1, hookCtx, false, context))
      nullable1 = serialization.CreateCopy<EntProtoId?>(this.CorePrototype, hookCtx, context, false);
    target.CorePrototype = nullable1;
    EntProtoId? nullable2 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.CoreInertPrototype, ref nullable2, hookCtx, false, context))
      nullable2 = serialization.CreateCopy<EntProtoId?>(this.CoreInertPrototype, hookCtx, context, false);
    target.CoreInertPrototype = nullable2;
    ProtoId<AnomalyBehaviorPrototype>? nullable3 = new ProtoId<AnomalyBehaviorPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<AnomalyBehaviorPrototype>?>(this.CurrentBehavior, ref nullable3, hookCtx, false, context))
      nullable3 = serialization.CreateCopy<ProtoId<AnomalyBehaviorPrototype>?>(this.CurrentBehavior, hookCtx, context, false);
    target.CurrentBehavior = nullable3;
    float num9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Continuity, ref num9, hookCtx, false, context))
      num9 = this.Continuity;
    target.Continuity = num9;
    float num10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinContituty, ref num10, hookCtx, false, context))
      num10 = this.MinContituty;
    target.MinContituty = num10;
    float num11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxContituty, ref num11, hookCtx, false, context))
      num11 = this.MaxContituty;
    target.MaxContituty = num11;
    float num12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AnimationTime, ref num12, hookCtx, false, context))
      num12 = this.AnimationTime;
    target.AnimationTime = num12;
    Vector2 vector2_2 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.FloatingOffset, ref vector2_2, hookCtx, false, context))
      vector2_2 = serialization.CreateCopy<Vector2>(this.FloatingOffset, hookCtx, context, false);
    target.FloatingOffset = vector2_2;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.DeleteEntity, ref flag, hookCtx, false, context))
      flag = this.DeleteEntity;
    target.DeleteEntity = flag;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AnomalyComponent target,
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
    AnomalyComponent target1 = (AnomalyComponent) target;
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
    AnomalyComponent target1 = (AnomalyComponent) target;
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
    AnomalyComponent target1 = (AnomalyComponent) target;
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
  virtual AnomalyComponent Component.Instantiate() => new AnomalyComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AnomalyComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<AnomalyComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<AnomalyComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      AnomalyComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextPulseTime += args.PausedTime;
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AnomalyComponent_AutoState : IComponentState
  {
    public float Stability;
    public float Severity;
    public float Health;
    public TimeSpan NextPulseTime;
    public AnomalousParticleType SeverityParticleType;
    public AnomalousParticleType DestabilizingParticleType;
    public AnomalousParticleType WeakeningParticleType;
    public AnomalousParticleType TransformationParticleType;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AnomalyComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<AnomalyComponent, ComponentGetState>(new ComponentEventRefHandler<AnomalyComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<AnomalyComponent, ComponentHandleState>(new ComponentEventRefHandler<AnomalyComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, 
    #nullable enable
    AnomalyComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new AnomalyComponent.AnomalyComponent_AutoState()
      {
        Stability = component.Stability,
        Severity = component.Severity,
        Health = component.Health,
        NextPulseTime = component.NextPulseTime,
        SeverityParticleType = component.SeverityParticleType,
        DestabilizingParticleType = component.DestabilizingParticleType,
        WeakeningParticleType = component.WeakeningParticleType,
        TransformationParticleType = component.TransformationParticleType
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AnomalyComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is AnomalyComponent.AnomalyComponent_AutoState current))
        return;
      component.Stability = current.Stability;
      component.Severity = current.Severity;
      component.Health = current.Health;
      component.NextPulseTime = current.NextPulseTime;
      component.SeverityParticleType = current.SeverityParticleType;
      component.DestabilizingParticleType = current.DestabilizingParticleType;
      component.WeakeningParticleType = current.WeakeningParticleType;
      component.TransformationParticleType = current.TransformationParticleType;
    }
  }
}
