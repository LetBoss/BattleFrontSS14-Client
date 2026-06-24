// Decompiled with JetBrains decompiler
// Type: Content.Shared.Body.Components.BloodstreamComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.Body.Systems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Timing;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Body.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, true)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedBloodstreamSystem)})]
public sealed class BloodstreamComponent : 
  Component,
  ISerializationGenerated<BloodstreamComponent>,
  ISerializationGenerated,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  public const string DefaultChemicalsSolutionName = "chemicals";
  public const string DefaultBloodSolutionName = "bloodstream";
  public const string DefaultBloodTemporarySolutionName = "bloodstreamTemporary";
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextUpdate;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan UpdateInterval = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float UpdateIntervalMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BleedAmount;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BleedReductionAmount = 0.33f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MaxBleedAmount = 10f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BloodlossThreshold = 0.9f;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier BloodlossDamage = new DamageSpecifier();
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier BloodlossHealDamage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 BloodRefreshAmount = (FixedPoint2) 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 BleedPuddleThreshold = (FixedPoint2) 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<DamageModifierSetPrototype> DamageBleedModifiers = ProtoId<DamageModifierSetPrototype>.op_Implicit("BloodlossHuman");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier InstantBloodSound = (SoundSpecifier) new SoundCollectionSpecifier("blood", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier BloodHealedSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/lightburn.ogg", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  public float BloodHealedSoundThreshold = -0.1f;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 ChemicalMaxVolume = FixedPoint2.New(250);
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 BloodMaxVolume = FixedPoint2.New(300);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ReagentPrototype> BloodReagent = ProtoId<ReagentPrototype>.op_Implicit("Blood");
  [DataField(null, false, 1, false, false, null)]
  public string BloodSolutionName = "bloodstream";
  [DataField(null, false, 1, false, false, null)]
  public string ChemicalSolutionName = "chemicals";
  [DataField(null, false, 1, false, false, null)]
  public string BloodTemporarySolutionName = "bloodstreamTemporary";
  [Robust.Shared.ViewVariables.ViewVariables]
  public Entity<SolutionComponent>? BloodSolution;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Entity<SolutionComponent>? ChemicalSolution;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Entity<SolutionComponent>? TemporarySolution;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan StatusTime;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertPrototype> BleedingAlert = ProtoId<AlertPrototype>.op_Implicit("Bleed");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool SpillChemicals;

  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan AdjustedUpdateInterval
  {
    get => this.UpdateInterval * (double) this.UpdateIntervalMultiplier;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BloodstreamComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (BloodstreamComponent) component;
    if (serialization.TryCustomCopy<BloodstreamComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextUpdate, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.NextUpdate, hookCtx, context, false);
    target.NextUpdate = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UpdateInterval, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.UpdateInterval, hookCtx, context, false);
    target.UpdateInterval = timeSpan2;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.UpdateIntervalMultiplier, ref num1, hookCtx, false, context))
      num1 = this.UpdateIntervalMultiplier;
    target.UpdateIntervalMultiplier = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BleedAmount, ref num2, hookCtx, false, context))
      num2 = this.BleedAmount;
    target.BleedAmount = num2;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BleedReductionAmount, ref num3, hookCtx, false, context))
      num3 = this.BleedReductionAmount;
    target.BleedReductionAmount = num3;
    float num4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxBleedAmount, ref num4, hookCtx, false, context))
      num4 = this.MaxBleedAmount;
    target.MaxBleedAmount = num4;
    float num5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BloodlossThreshold, ref num5, hookCtx, false, context))
      num5 = this.BloodlossThreshold;
    target.BloodlossThreshold = num5;
    DamageSpecifier damageSpecifier1 = (DamageSpecifier) null;
    if (this.BloodlossDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.BloodlossDamage, ref damageSpecifier1, hookCtx, false, context))
    {
      if (this.BloodlossDamage == null)
        damageSpecifier1 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.BloodlossDamage, ref damageSpecifier1, hookCtx, context, true);
    }
    target.BloodlossDamage = damageSpecifier1;
    DamageSpecifier damageSpecifier2 = (DamageSpecifier) null;
    if (this.BloodlossHealDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.BloodlossHealDamage, ref damageSpecifier2, hookCtx, false, context))
    {
      if (this.BloodlossHealDamage == null)
        damageSpecifier2 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.BloodlossHealDamage, ref damageSpecifier2, hookCtx, context, true);
    }
    target.BloodlossHealDamage = damageSpecifier2;
    FixedPoint2 fixedPoint2_1 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.BloodRefreshAmount, ref fixedPoint2_1, hookCtx, false, context))
      fixedPoint2_1 = serialization.CreateCopy<FixedPoint2>(this.BloodRefreshAmount, hookCtx, context, false);
    target.BloodRefreshAmount = fixedPoint2_1;
    FixedPoint2 fixedPoint2_2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.BleedPuddleThreshold, ref fixedPoint2_2, hookCtx, false, context))
      fixedPoint2_2 = serialization.CreateCopy<FixedPoint2>(this.BleedPuddleThreshold, hookCtx, context, false);
    target.BleedPuddleThreshold = fixedPoint2_2;
    ProtoId<DamageModifierSetPrototype> protoId1 = new ProtoId<DamageModifierSetPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<DamageModifierSetPrototype>>(this.DamageBleedModifiers, ref protoId1, hookCtx, false, context))
      protoId1 = serialization.CreateCopy<ProtoId<DamageModifierSetPrototype>>(this.DamageBleedModifiers, hookCtx, context, false);
    target.DamageBleedModifiers = protoId1;
    SoundSpecifier soundSpecifier1 = (SoundSpecifier) null;
    if (this.InstantBloodSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.InstantBloodSound, ref soundSpecifier1, hookCtx, true, context))
      soundSpecifier1 = serialization.CreateCopy<SoundSpecifier>(this.InstantBloodSound, hookCtx, context, false);
    target.InstantBloodSound = soundSpecifier1;
    SoundSpecifier soundSpecifier2 = (SoundSpecifier) null;
    if (this.BloodHealedSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BloodHealedSound, ref soundSpecifier2, hookCtx, true, context))
      soundSpecifier2 = serialization.CreateCopy<SoundSpecifier>(this.BloodHealedSound, hookCtx, context, false);
    target.BloodHealedSound = soundSpecifier2;
    float num6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BloodHealedSoundThreshold, ref num6, hookCtx, false, context))
      num6 = this.BloodHealedSoundThreshold;
    target.BloodHealedSoundThreshold = num6;
    FixedPoint2 fixedPoint2_3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.ChemicalMaxVolume, ref fixedPoint2_3, hookCtx, false, context))
      fixedPoint2_3 = serialization.CreateCopy<FixedPoint2>(this.ChemicalMaxVolume, hookCtx, context, false);
    target.ChemicalMaxVolume = fixedPoint2_3;
    FixedPoint2 fixedPoint2_4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.BloodMaxVolume, ref fixedPoint2_4, hookCtx, false, context))
      fixedPoint2_4 = serialization.CreateCopy<FixedPoint2>(this.BloodMaxVolume, hookCtx, context, false);
    target.BloodMaxVolume = fixedPoint2_4;
    ProtoId<ReagentPrototype> protoId2 = new ProtoId<ReagentPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ReagentPrototype>>(this.BloodReagent, ref protoId2, hookCtx, false, context))
      protoId2 = serialization.CreateCopy<ProtoId<ReagentPrototype>>(this.BloodReagent, hookCtx, context, false);
    target.BloodReagent = protoId2;
    string str1 = (string) null;
    if (this.BloodSolutionName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BloodSolutionName, ref str1, hookCtx, false, context))
      str1 = this.BloodSolutionName;
    target.BloodSolutionName = str1;
    string str2 = (string) null;
    if (this.ChemicalSolutionName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ChemicalSolutionName, ref str2, hookCtx, false, context))
      str2 = this.ChemicalSolutionName;
    target.ChemicalSolutionName = str2;
    string str3 = (string) null;
    if (this.BloodTemporarySolutionName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BloodTemporarySolutionName, ref str3, hookCtx, false, context))
      str3 = this.BloodTemporarySolutionName;
    target.BloodTemporarySolutionName = str3;
    TimeSpan timeSpan3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StatusTime, ref timeSpan3, hookCtx, false, context))
      timeSpan3 = serialization.CreateCopy<TimeSpan>(this.StatusTime, hookCtx, context, false);
    target.StatusTime = timeSpan3;
    ProtoId<AlertPrototype> protoId3 = new ProtoId<AlertPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(this.BleedingAlert, ref protoId3, hookCtx, false, context))
      protoId3 = serialization.CreateCopy<ProtoId<AlertPrototype>>(this.BleedingAlert, hookCtx, context, false);
    target.BleedingAlert = protoId3;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.SpillChemicals, ref flag, hookCtx, false, context))
      flag = this.SpillChemicals;
    target.SpillChemicals = flag;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BloodstreamComponent target,
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
    BloodstreamComponent target1 = (BloodstreamComponent) target;
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
    BloodstreamComponent target1 = (BloodstreamComponent) target;
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
    BloodstreamComponent target1 = (BloodstreamComponent) target;
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

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BloodstreamComponent target1 = (BloodstreamComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponentDelta) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual BloodstreamComponent Component.Instantiate() => new BloodstreamComponent();

  IComponentDelta IComponentDelta.Instantiate() => (IComponentDelta) this.Instantiate();

  IComponentDelta ISerializationGenerated<IComponentDelta>.Instantiate()
  {
    return (IComponentDelta) this.Instantiate();
  }

  public GameTick LastFieldUpdate { get; set; } = GameTick.Zero;

  public GameTick[] LastModifiedFields { get; set; } = Array.Empty<GameTick>();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BloodstreamComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<BloodstreamComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<BloodstreamComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      BloodstreamComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextUpdate += args.PausedTime;
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class BloodstreamComponent_AutoState : IComponentState
  {
    public TimeSpan NextUpdate;
    public TimeSpan UpdateInterval;
    public float UpdateIntervalMultiplier;
    public float BleedAmount;
    public float BleedReductionAmount;
    public float MaxBleedAmount;
    public float BloodlossThreshold;
    public 
    #nullable enable
    DamageSpecifier BloodlossDamage;
    public DamageSpecifier BloodlossHealDamage;
    public FixedPoint2 BloodRefreshAmount;
    public FixedPoint2 BleedPuddleThreshold;
    public ProtoId<DamageModifierSetPrototype> DamageBleedModifiers;
    public SoundSpecifier InstantBloodSound;
    public ProtoId<ReagentPrototype> BloodReagent;
    public TimeSpan StatusTime;
    public bool SpillChemicals;

    public BloodstreamComponent.BloodstreamComponent_AutoState ShallowClone()
    {
      return new BloodstreamComponent.BloodstreamComponent_AutoState()
      {
        NextUpdate = this.NextUpdate,
        UpdateInterval = this.UpdateInterval,
        UpdateIntervalMultiplier = this.UpdateIntervalMultiplier,
        BleedAmount = this.BleedAmount,
        BleedReductionAmount = this.BleedReductionAmount,
        MaxBleedAmount = this.MaxBleedAmount,
        BloodlossThreshold = this.BloodlossThreshold,
        BloodlossDamage = this.BloodlossDamage,
        BloodlossHealDamage = this.BloodlossHealDamage,
        BloodRefreshAmount = this.BloodRefreshAmount,
        BleedPuddleThreshold = this.BleedPuddleThreshold,
        DamageBleedModifiers = this.DamageBleedModifiers,
        InstantBloodSound = this.InstantBloodSound,
        BloodReagent = this.BloodReagent,
        StatusTime = this.StatusTime,
        SpillChemicals = this.SpillChemicals
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BloodstreamComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<BloodstreamComponent>(new string[16 /*0x10*/]
      {
        "NextUpdate",
        "UpdateInterval",
        "UpdateIntervalMultiplier",
        "BleedAmount",
        "BleedReductionAmount",
        "MaxBleedAmount",
        "BloodlossThreshold",
        "BloodlossDamage",
        "BloodlossHealDamage",
        "BloodRefreshAmount",
        "BleedPuddleThreshold",
        "DamageBleedModifiers",
        "InstantBloodSound",
        "BloodReagent",
        "StatusTime",
        "SpillChemicals"
      });
      // ISSUE: method pointer
      this.SubscribeLocalEvent<BloodstreamComponent, ComponentGetState>(new ComponentEventRefHandler<BloodstreamComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<BloodstreamComponent, ComponentHandleState>(new ComponentEventRefHandler<BloodstreamComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      BloodstreamComponent component,
      ref ComponentGetState args)
    {
      IComponentDelta icomponentDelta = (IComponentDelta) component;
      if (icomponentDelta != null && GameTick.op_GreaterThan(((ComponentGetState) ref args).FromTick, component.CreationTick) && GameTick.op_GreaterThanOrEqual(icomponentDelta.LastFieldUpdate, ((ComponentGetState) ref args).FromTick))
      {
        switch (this.EntityManager.GetModifiedFields((IComponentDelta) component, ((ComponentGetState) ref args).FromTick))
        {
          case 1:
            ((ComponentGetState) ref args).State = (IComponentState) new BloodstreamComponent.NextUpdate_FieldComponentState()
            {
              NextUpdate = component.NextUpdate
            };
            return;
          case 2:
            ((ComponentGetState) ref args).State = (IComponentState) new BloodstreamComponent.UpdateInterval_FieldComponentState()
            {
              UpdateInterval = component.UpdateInterval
            };
            return;
          case 4:
            ((ComponentGetState) ref args).State = (IComponentState) new BloodstreamComponent.UpdateIntervalMultiplier_FieldComponentState()
            {
              UpdateIntervalMultiplier = component.UpdateIntervalMultiplier
            };
            return;
          case 8:
            ((ComponentGetState) ref args).State = (IComponentState) new BloodstreamComponent.BleedAmount_FieldComponentState()
            {
              BleedAmount = component.BleedAmount
            };
            return;
          case 16 /*0x10*/:
            ((ComponentGetState) ref args).State = (IComponentState) new BloodstreamComponent.BleedReductionAmount_FieldComponentState()
            {
              BleedReductionAmount = component.BleedReductionAmount
            };
            return;
          case 32 /*0x20*/:
            ((ComponentGetState) ref args).State = (IComponentState) new BloodstreamComponent.MaxBleedAmount_FieldComponentState()
            {
              MaxBleedAmount = component.MaxBleedAmount
            };
            return;
          case 64 /*0x40*/:
            ((ComponentGetState) ref args).State = (IComponentState) new BloodstreamComponent.BloodlossThreshold_FieldComponentState()
            {
              BloodlossThreshold = component.BloodlossThreshold
            };
            return;
          case 128 /*0x80*/:
            ((ComponentGetState) ref args).State = (IComponentState) new BloodstreamComponent.BloodlossDamage_FieldComponentState()
            {
              BloodlossDamage = component.BloodlossDamage
            };
            return;
          case 256 /*0x0100*/:
            ((ComponentGetState) ref args).State = (IComponentState) new BloodstreamComponent.BloodlossHealDamage_FieldComponentState()
            {
              BloodlossHealDamage = component.BloodlossHealDamage
            };
            return;
          case 512 /*0x0200*/:
            ((ComponentGetState) ref args).State = (IComponentState) new BloodstreamComponent.BloodRefreshAmount_FieldComponentState()
            {
              BloodRefreshAmount = component.BloodRefreshAmount
            };
            return;
          case 1024 /*0x0400*/:
            ((ComponentGetState) ref args).State = (IComponentState) new BloodstreamComponent.BleedPuddleThreshold_FieldComponentState()
            {
              BleedPuddleThreshold = component.BleedPuddleThreshold
            };
            return;
          case 2048 /*0x0800*/:
            ((ComponentGetState) ref args).State = (IComponentState) new BloodstreamComponent.DamageBleedModifiers_FieldComponentState()
            {
              DamageBleedModifiers = component.DamageBleedModifiers
            };
            return;
          case 4096 /*0x1000*/:
            ((ComponentGetState) ref args).State = (IComponentState) new BloodstreamComponent.InstantBloodSound_FieldComponentState()
            {
              InstantBloodSound = component.InstantBloodSound
            };
            return;
          case 8192 /*0x2000*/:
            ((ComponentGetState) ref args).State = (IComponentState) new BloodstreamComponent.BloodReagent_FieldComponentState()
            {
              BloodReagent = component.BloodReagent
            };
            return;
          case 16384 /*0x4000*/:
            ((ComponentGetState) ref args).State = (IComponentState) new BloodstreamComponent.StatusTime_FieldComponentState()
            {
              StatusTime = component.StatusTime
            };
            return;
          case 32768 /*0x8000*/:
            ((ComponentGetState) ref args).State = (IComponentState) new BloodstreamComponent.SpillChemicals_FieldComponentState()
            {
              SpillChemicals = component.SpillChemicals
            };
            return;
        }
      }
      ((ComponentGetState) ref args).State = (IComponentState) new BloodstreamComponent.BloodstreamComponent_AutoState()
      {
        NextUpdate = component.NextUpdate,
        UpdateInterval = component.UpdateInterval,
        UpdateIntervalMultiplier = component.UpdateIntervalMultiplier,
        BleedAmount = component.BleedAmount,
        BleedReductionAmount = component.BleedReductionAmount,
        MaxBleedAmount = component.MaxBleedAmount,
        BloodlossThreshold = component.BloodlossThreshold,
        BloodlossDamage = component.BloodlossDamage,
        BloodlossHealDamage = component.BloodlossHealDamage,
        BloodRefreshAmount = component.BloodRefreshAmount,
        BleedPuddleThreshold = component.BleedPuddleThreshold,
        DamageBleedModifiers = component.DamageBleedModifiers,
        InstantBloodSound = component.InstantBloodSound,
        BloodReagent = component.BloodReagent,
        StatusTime = component.StatusTime,
        SpillChemicals = component.SpillChemicals
      };
    }

    private void OnHandleState(
      EntityUid uid,
      BloodstreamComponent component,
      ref ComponentHandleState args)
    {
      switch (((ComponentHandleState) ref args).Current)
      {
        case BloodstreamComponent.NextUpdate_FieldComponentState fieldComponentState1:
          component.NextUpdate = fieldComponentState1.NextUpdate;
          break;
        case BloodstreamComponent.UpdateInterval_FieldComponentState fieldComponentState2:
          component.UpdateInterval = fieldComponentState2.UpdateInterval;
          break;
        case BloodstreamComponent.UpdateIntervalMultiplier_FieldComponentState fieldComponentState3:
          component.UpdateIntervalMultiplier = fieldComponentState3.UpdateIntervalMultiplier;
          break;
        case BloodstreamComponent.BleedAmount_FieldComponentState fieldComponentState4:
          component.BleedAmount = fieldComponentState4.BleedAmount;
          break;
        case BloodstreamComponent.BleedReductionAmount_FieldComponentState fieldComponentState5:
          component.BleedReductionAmount = fieldComponentState5.BleedReductionAmount;
          break;
        case BloodstreamComponent.MaxBleedAmount_FieldComponentState fieldComponentState6:
          component.MaxBleedAmount = fieldComponentState6.MaxBleedAmount;
          break;
        case BloodstreamComponent.BloodlossThreshold_FieldComponentState fieldComponentState7:
          component.BloodlossThreshold = fieldComponentState7.BloodlossThreshold;
          break;
        case BloodstreamComponent.BloodlossDamage_FieldComponentState fieldComponentState8:
          component.BloodlossDamage = fieldComponentState8.BloodlossDamage;
          break;
        case BloodstreamComponent.BloodlossHealDamage_FieldComponentState fieldComponentState9:
          component.BloodlossHealDamage = fieldComponentState9.BloodlossHealDamage;
          break;
        case BloodstreamComponent.BloodRefreshAmount_FieldComponentState fieldComponentState10:
          component.BloodRefreshAmount = fieldComponentState10.BloodRefreshAmount;
          break;
        case BloodstreamComponent.BleedPuddleThreshold_FieldComponentState fieldComponentState11:
          component.BleedPuddleThreshold = fieldComponentState11.BleedPuddleThreshold;
          break;
        case BloodstreamComponent.DamageBleedModifiers_FieldComponentState fieldComponentState12:
          component.DamageBleedModifiers = fieldComponentState12.DamageBleedModifiers;
          break;
        case BloodstreamComponent.InstantBloodSound_FieldComponentState fieldComponentState13:
          component.InstantBloodSound = fieldComponentState13.InstantBloodSound;
          break;
        case BloodstreamComponent.BloodReagent_FieldComponentState fieldComponentState14:
          component.BloodReagent = fieldComponentState14.BloodReagent;
          break;
        case BloodstreamComponent.StatusTime_FieldComponentState fieldComponentState15:
          component.StatusTime = fieldComponentState15.StatusTime;
          break;
        case BloodstreamComponent.SpillChemicals_FieldComponentState fieldComponentState16:
          component.SpillChemicals = fieldComponentState16.SpillChemicals;
          break;
        case BloodstreamComponent.BloodstreamComponent_AutoState componentAutoState:
          component.NextUpdate = componentAutoState.NextUpdate;
          component.UpdateInterval = componentAutoState.UpdateInterval;
          component.UpdateIntervalMultiplier = componentAutoState.UpdateIntervalMultiplier;
          component.BleedAmount = componentAutoState.BleedAmount;
          component.BleedReductionAmount = componentAutoState.BleedReductionAmount;
          component.MaxBleedAmount = componentAutoState.MaxBleedAmount;
          component.BloodlossThreshold = componentAutoState.BloodlossThreshold;
          component.BloodlossDamage = componentAutoState.BloodlossDamage;
          component.BloodlossHealDamage = componentAutoState.BloodlossHealDamage;
          component.BloodRefreshAmount = componentAutoState.BloodRefreshAmount;
          component.BleedPuddleThreshold = componentAutoState.BleedPuddleThreshold;
          component.DamageBleedModifiers = componentAutoState.DamageBleedModifiers;
          component.InstantBloodSound = componentAutoState.InstantBloodSound;
          component.BloodReagent = componentAutoState.BloodReagent;
          component.StatusTime = componentAutoState.StatusTime;
          component.SpillChemicals = componentAutoState.SpillChemicals;
          break;
      }
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class NextUpdate_FieldComponentState : 
    IComponentDeltaState<BloodstreamComponent.BloodstreamComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan NextUpdate;

    public void ApplyToFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      fullState.NextUpdate = this.NextUpdate;
    }

    public BloodstreamComponent.BloodstreamComponent_AutoState CreateNewFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      BloodstreamComponent.BloodstreamComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class UpdateInterval_FieldComponentState : 
    IComponentDeltaState<BloodstreamComponent.BloodstreamComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan UpdateInterval;

    public void ApplyToFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      fullState.UpdateInterval = this.UpdateInterval;
    }

    public BloodstreamComponent.BloodstreamComponent_AutoState CreateNewFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      BloodstreamComponent.BloodstreamComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class UpdateIntervalMultiplier_FieldComponentState : 
    IComponentDeltaState<BloodstreamComponent.BloodstreamComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float UpdateIntervalMultiplier;

    public void ApplyToFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      fullState.UpdateIntervalMultiplier = this.UpdateIntervalMultiplier;
    }

    public BloodstreamComponent.BloodstreamComponent_AutoState CreateNewFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      BloodstreamComponent.BloodstreamComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class BleedAmount_FieldComponentState : 
    IComponentDeltaState<BloodstreamComponent.BloodstreamComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float BleedAmount;

    public void ApplyToFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      fullState.BleedAmount = this.BleedAmount;
    }

    public BloodstreamComponent.BloodstreamComponent_AutoState CreateNewFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      BloodstreamComponent.BloodstreamComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class BleedReductionAmount_FieldComponentState : 
    IComponentDeltaState<BloodstreamComponent.BloodstreamComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float BleedReductionAmount;

    public void ApplyToFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      fullState.BleedReductionAmount = this.BleedReductionAmount;
    }

    public BloodstreamComponent.BloodstreamComponent_AutoState CreateNewFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      BloodstreamComponent.BloodstreamComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class MaxBleedAmount_FieldComponentState : 
    IComponentDeltaState<BloodstreamComponent.BloodstreamComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float MaxBleedAmount;

    public void ApplyToFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      fullState.MaxBleedAmount = this.MaxBleedAmount;
    }

    public BloodstreamComponent.BloodstreamComponent_AutoState CreateNewFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      BloodstreamComponent.BloodstreamComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class BloodlossThreshold_FieldComponentState : 
    IComponentDeltaState<BloodstreamComponent.BloodstreamComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float BloodlossThreshold;

    public void ApplyToFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      fullState.BloodlossThreshold = this.BloodlossThreshold;
    }

    public BloodstreamComponent.BloodstreamComponent_AutoState CreateNewFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      BloodstreamComponent.BloodstreamComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class BloodlossDamage_FieldComponentState : 
    IComponentDeltaState<BloodstreamComponent.BloodstreamComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public DamageSpecifier BloodlossDamage;

    public void ApplyToFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      fullState.BloodlossDamage = this.BloodlossDamage;
    }

    public BloodstreamComponent.BloodstreamComponent_AutoState CreateNewFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      BloodstreamComponent.BloodstreamComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class BloodlossHealDamage_FieldComponentState : 
    IComponentDeltaState<BloodstreamComponent.BloodstreamComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public DamageSpecifier BloodlossHealDamage;

    public void ApplyToFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      fullState.BloodlossHealDamage = this.BloodlossHealDamage;
    }

    public BloodstreamComponent.BloodstreamComponent_AutoState CreateNewFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      BloodstreamComponent.BloodstreamComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class BloodRefreshAmount_FieldComponentState : 
    IComponentDeltaState<BloodstreamComponent.BloodstreamComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public FixedPoint2 BloodRefreshAmount;

    public void ApplyToFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      fullState.BloodRefreshAmount = this.BloodRefreshAmount;
    }

    public BloodstreamComponent.BloodstreamComponent_AutoState CreateNewFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      BloodstreamComponent.BloodstreamComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class BleedPuddleThreshold_FieldComponentState : 
    IComponentDeltaState<BloodstreamComponent.BloodstreamComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public FixedPoint2 BleedPuddleThreshold;

    public void ApplyToFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      fullState.BleedPuddleThreshold = this.BleedPuddleThreshold;
    }

    public BloodstreamComponent.BloodstreamComponent_AutoState CreateNewFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      BloodstreamComponent.BloodstreamComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class DamageBleedModifiers_FieldComponentState : 
    IComponentDeltaState<BloodstreamComponent.BloodstreamComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public ProtoId<DamageModifierSetPrototype> DamageBleedModifiers;

    public void ApplyToFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      fullState.DamageBleedModifiers = this.DamageBleedModifiers;
    }

    public BloodstreamComponent.BloodstreamComponent_AutoState CreateNewFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      BloodstreamComponent.BloodstreamComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class InstantBloodSound_FieldComponentState : 
    IComponentDeltaState<BloodstreamComponent.BloodstreamComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public SoundSpecifier InstantBloodSound;

    public void ApplyToFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      fullState.InstantBloodSound = this.InstantBloodSound;
    }

    public BloodstreamComponent.BloodstreamComponent_AutoState CreateNewFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      BloodstreamComponent.BloodstreamComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class BloodReagent_FieldComponentState : 
    IComponentDeltaState<BloodstreamComponent.BloodstreamComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public ProtoId<ReagentPrototype> BloodReagent;

    public void ApplyToFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      fullState.BloodReagent = this.BloodReagent;
    }

    public BloodstreamComponent.BloodstreamComponent_AutoState CreateNewFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      BloodstreamComponent.BloodstreamComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class StatusTime_FieldComponentState : 
    IComponentDeltaState<BloodstreamComponent.BloodstreamComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan StatusTime;

    public void ApplyToFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      fullState.StatusTime = this.StatusTime;
    }

    public BloodstreamComponent.BloodstreamComponent_AutoState CreateNewFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      BloodstreamComponent.BloodstreamComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class SpillChemicals_FieldComponentState : 
    IComponentDeltaState<BloodstreamComponent.BloodstreamComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool SpillChemicals;

    public void ApplyToFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      fullState.SpillChemicals = this.SpillChemicals;
    }

    public BloodstreamComponent.BloodstreamComponent_AutoState CreateNewFullState(
      BloodstreamComponent.BloodstreamComponent_AutoState fullState)
    {
      BloodstreamComponent.BloodstreamComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
