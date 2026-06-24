// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Wounds.WoundTreaterComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Medical.Wounds;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, true)]
[Access(new Type[] {typeof (SharedWoundsSystem)})]
public sealed class WoundTreaterComponent : 
  Component,
  ISerializationGenerated<WoundTreaterComponent>,
  ISerializationGenerated,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public WoundType Wound;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public bool Treats;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public bool Consumable;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public ProtoId<DamageGroupPrototype> Group;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ScalingDoAfter;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> DoAfterSkill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillMedical";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float[] DoAfterSkillMultipliers = new float[5]
  {
    1f,
    1f,
    1f,
    0.75f,
    0.5f
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SelfTargetDoAfterMultiplier = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2? Damage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2? UnskilledDamage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanUseUnskilled;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntProtoId<SkillDefinitionComponent>, int> Skills = new Dictionary<EntProtoId<SkillDefinitionComponent>, int>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? TreatBeginSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? TreatEndSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? UserPopup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? TargetPopup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? OthersPopup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? TargetStartPopup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? UserFinishPopup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? TargetFinishPopup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? OthersFinishPopup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? NoneSelfPopup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? NoneOtherPopup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? NoWoundsOnUserPopup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? NoWoundsOnTargetPopup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ushort? CurrentDoAfter;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WoundTreaterComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (WoundTreaterComponent) target1;
    if (serialization.TryCustomCopy<WoundTreaterComponent>(this, ref target, hookCtx, false, context))
      return;
    WoundType target2 = WoundType.Brute;
    if (!serialization.TryCustomCopy<WoundType>(this.Wound, ref target2, hookCtx, false, context))
      target2 = this.Wound;
    target.Wound = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Treats, ref target3, hookCtx, false, context))
      target3 = this.Treats;
    target.Treats = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Consumable, ref target4, hookCtx, false, context))
      target4 = this.Consumable;
    target.Consumable = target4;
    ProtoId<DamageGroupPrototype> target5 = new ProtoId<DamageGroupPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<DamageGroupPrototype>>(this.Group, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<ProtoId<DamageGroupPrototype>>(this.Group, hookCtx, context);
    target.Group = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ScalingDoAfter, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.ScalingDoAfter, hookCtx, context);
    target.ScalingDoAfter = target6;
    EntProtoId<SkillDefinitionComponent> target7 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.DoAfterSkill, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.DoAfterSkill, hookCtx, context);
    target.DoAfterSkill = target7;
    float[] target8 = (float[]) null;
    if (this.DoAfterSkillMultipliers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<float[]>(this.DoAfterSkillMultipliers, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<float[]>(this.DoAfterSkillMultipliers, hookCtx, context);
    target.DoAfterSkillMultipliers = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SelfTargetDoAfterMultiplier, ref target9, hookCtx, false, context))
      target9 = this.SelfTargetDoAfterMultiplier;
    target.SelfTargetDoAfterMultiplier = target9;
    FixedPoint2? target10 = new FixedPoint2?();
    if (!serialization.TryCustomCopy<FixedPoint2?>(this.Damage, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<FixedPoint2?>(this.Damage, hookCtx, context);
    target.Damage = target10;
    FixedPoint2? target11 = new FixedPoint2?();
    if (!serialization.TryCustomCopy<FixedPoint2?>(this.UnskilledDamage, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<FixedPoint2?>(this.UnskilledDamage, hookCtx, context);
    target.UnskilledDamage = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanUseUnskilled, ref target12, hookCtx, false, context))
      target12 = this.CanUseUnskilled;
    target.CanUseUnskilled = target12;
    Dictionary<EntProtoId<SkillDefinitionComponent>, int> target13 = (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null;
    if (this.Skills == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.Skills, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.Skills, hookCtx, context);
    target.Skills = target13;
    SoundSpecifier target14 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.TreatBeginSound, ref target14, hookCtx, true, context))
      target14 = serialization.CreateCopy<SoundSpecifier>(this.TreatBeginSound, hookCtx, context);
    target.TreatBeginSound = target14;
    SoundSpecifier target15 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.TreatEndSound, ref target15, hookCtx, true, context))
      target15 = serialization.CreateCopy<SoundSpecifier>(this.TreatEndSound, hookCtx, context);
    target.TreatEndSound = target15;
    LocId? target16 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.UserPopup, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<LocId?>(this.UserPopup, hookCtx, context);
    target.UserPopup = target16;
    LocId? target17 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.TargetPopup, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<LocId?>(this.TargetPopup, hookCtx, context);
    target.TargetPopup = target17;
    LocId? target18 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.OthersPopup, ref target18, hookCtx, false, context))
      target18 = serialization.CreateCopy<LocId?>(this.OthersPopup, hookCtx, context);
    target.OthersPopup = target18;
    LocId? target19 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.TargetStartPopup, ref target19, hookCtx, false, context))
      target19 = serialization.CreateCopy<LocId?>(this.TargetStartPopup, hookCtx, context);
    target.TargetStartPopup = target19;
    LocId? target20 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.UserFinishPopup, ref target20, hookCtx, false, context))
      target20 = serialization.CreateCopy<LocId?>(this.UserFinishPopup, hookCtx, context);
    target.UserFinishPopup = target20;
    LocId? target21 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.TargetFinishPopup, ref target21, hookCtx, false, context))
      target21 = serialization.CreateCopy<LocId?>(this.TargetFinishPopup, hookCtx, context);
    target.TargetFinishPopup = target21;
    LocId? target22 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.OthersFinishPopup, ref target22, hookCtx, false, context))
      target22 = serialization.CreateCopy<LocId?>(this.OthersFinishPopup, hookCtx, context);
    target.OthersFinishPopup = target22;
    LocId? target23 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.NoneSelfPopup, ref target23, hookCtx, false, context))
      target23 = serialization.CreateCopy<LocId?>(this.NoneSelfPopup, hookCtx, context);
    target.NoneSelfPopup = target23;
    LocId? target24 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.NoneOtherPopup, ref target24, hookCtx, false, context))
      target24 = serialization.CreateCopy<LocId?>(this.NoneOtherPopup, hookCtx, context);
    target.NoneOtherPopup = target24;
    LocId? target25 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.NoWoundsOnUserPopup, ref target25, hookCtx, false, context))
      target25 = serialization.CreateCopy<LocId?>(this.NoWoundsOnUserPopup, hookCtx, context);
    target.NoWoundsOnUserPopup = target25;
    LocId? target26 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.NoWoundsOnTargetPopup, ref target26, hookCtx, false, context))
      target26 = serialization.CreateCopy<LocId?>(this.NoWoundsOnTargetPopup, hookCtx, context);
    target.NoWoundsOnTargetPopup = target26;
    ushort? target27 = new ushort?();
    if (!serialization.TryCustomCopy<ushort?>(this.CurrentDoAfter, ref target27, hookCtx, false, context))
      target27 = this.CurrentDoAfter;
    target.CurrentDoAfter = target27;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WoundTreaterComponent target,
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
    WoundTreaterComponent target1 = (WoundTreaterComponent) target;
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
    WoundTreaterComponent target1 = (WoundTreaterComponent) target;
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
    WoundTreaterComponent target1 = (WoundTreaterComponent) target;
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

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    WoundTreaterComponent target1 = (WoundTreaterComponent) target;
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
  virtual WoundTreaterComponent Component.Instantiate() => new WoundTreaterComponent();

  IComponentDelta IComponentDelta.Instantiate() => (IComponentDelta) this.Instantiate();

  IComponentDelta ISerializationGenerated<IComponentDelta>.Instantiate()
  {
    return (IComponentDelta) this.Instantiate();
  }

  public GameTick LastFieldUpdate { get; set; } = GameTick.Zero;

  public GameTick[] LastModifiedFields { get; set; } = Array.Empty<GameTick>();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class WoundTreaterComponent_AutoState : IComponentState
  {
    public WoundType Wound;
    public bool Treats;
    public bool Consumable;
    public ProtoId<DamageGroupPrototype> Group;
    public TimeSpan ScalingDoAfter;
    public EntProtoId<SkillDefinitionComponent> DoAfterSkill;
    public float[] DoAfterSkillMultipliers;
    public float SelfTargetDoAfterMultiplier;
    public FixedPoint2? Damage;
    public FixedPoint2? UnskilledDamage;
    public bool CanUseUnskilled;
    public Dictionary<EntProtoId<SkillDefinitionComponent>, int> Skills;
    public SoundSpecifier? TreatBeginSound;
    public SoundSpecifier? TreatEndSound;
    public LocId? UserPopup;
    public LocId? TargetPopup;
    public LocId? OthersPopup;
    public LocId? TargetStartPopup;
    public LocId? UserFinishPopup;
    public LocId? TargetFinishPopup;
    public LocId? OthersFinishPopup;
    public LocId? NoneSelfPopup;
    public LocId? NoneOtherPopup;
    public LocId? NoWoundsOnUserPopup;
    public LocId? NoWoundsOnTargetPopup;
    public ushort? CurrentDoAfter;

    public WoundTreaterComponent.WoundTreaterComponent_AutoState ShallowClone()
    {
      return new WoundTreaterComponent.WoundTreaterComponent_AutoState()
      {
        Wound = this.Wound,
        Treats = this.Treats,
        Consumable = this.Consumable,
        Group = this.Group,
        ScalingDoAfter = this.ScalingDoAfter,
        DoAfterSkill = this.DoAfterSkill,
        DoAfterSkillMultipliers = this.DoAfterSkillMultipliers,
        SelfTargetDoAfterMultiplier = this.SelfTargetDoAfterMultiplier,
        Damage = this.Damage,
        UnskilledDamage = this.UnskilledDamage,
        CanUseUnskilled = this.CanUseUnskilled,
        Skills = this.Skills,
        TreatBeginSound = this.TreatBeginSound,
        TreatEndSound = this.TreatEndSound,
        UserPopup = this.UserPopup,
        TargetPopup = this.TargetPopup,
        OthersPopup = this.OthersPopup,
        TargetStartPopup = this.TargetStartPopup,
        UserFinishPopup = this.UserFinishPopup,
        TargetFinishPopup = this.TargetFinishPopup,
        OthersFinishPopup = this.OthersFinishPopup,
        NoneSelfPopup = this.NoneSelfPopup,
        NoneOtherPopup = this.NoneOtherPopup,
        NoWoundsOnUserPopup = this.NoWoundsOnUserPopup,
        NoWoundsOnTargetPopup = this.NoWoundsOnTargetPopup,
        CurrentDoAfter = this.CurrentDoAfter
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class WoundTreaterComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<WoundTreaterComponent>("Wound", "Treats", "Consumable", "Group", "ScalingDoAfter", "DoAfterSkill", "DoAfterSkillMultipliers", "SelfTargetDoAfterMultiplier", "Damage", "UnskilledDamage", "CanUseUnskilled", "Skills", "TreatBeginSound", "TreatEndSound", "UserPopup", "TargetPopup", "OthersPopup", "TargetStartPopup", "UserFinishPopup", "TargetFinishPopup", "OthersFinishPopup", "NoneSelfPopup", "NoneOtherPopup", "NoWoundsOnUserPopup", "NoWoundsOnTargetPopup", "CurrentDoAfter");
      this.SubscribeLocalEvent<WoundTreaterComponent, ComponentGetState>(new ComponentEventRefHandler<WoundTreaterComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<WoundTreaterComponent, ComponentHandleState>(new ComponentEventRefHandler<WoundTreaterComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      WoundTreaterComponent component,
      ref ComponentGetState args)
    {
      IComponentDelta componentDelta = (IComponentDelta) component;
      if (componentDelta != null && args.FromTick > component.CreationTick && componentDelta.LastFieldUpdate >= args.FromTick)
      {
        switch (this.EntityManager.GetModifiedFields((IComponentDelta) component, args.FromTick))
        {
          case 1:
            args.State = (IComponentState) new WoundTreaterComponent.Wound_FieldComponentState()
            {
              Wound = component.Wound
            };
            return;
          case 2:
            args.State = (IComponentState) new WoundTreaterComponent.Treats_FieldComponentState()
            {
              Treats = component.Treats
            };
            return;
          case 4:
            args.State = (IComponentState) new WoundTreaterComponent.Consumable_FieldComponentState()
            {
              Consumable = component.Consumable
            };
            return;
          case 8:
            args.State = (IComponentState) new WoundTreaterComponent.Group_FieldComponentState()
            {
              Group = component.Group
            };
            return;
          case 16 /*0x10*/:
            args.State = (IComponentState) new WoundTreaterComponent.ScalingDoAfter_FieldComponentState()
            {
              ScalingDoAfter = component.ScalingDoAfter
            };
            return;
          case 32 /*0x20*/:
            args.State = (IComponentState) new WoundTreaterComponent.DoAfterSkill_FieldComponentState()
            {
              DoAfterSkill = component.DoAfterSkill
            };
            return;
          case 64 /*0x40*/:
            args.State = (IComponentState) new WoundTreaterComponent.DoAfterSkillMultipliers_FieldComponentState()
            {
              DoAfterSkillMultipliers = component.DoAfterSkillMultipliers
            };
            return;
          case 128 /*0x80*/:
            args.State = (IComponentState) new WoundTreaterComponent.SelfTargetDoAfterMultiplier_FieldComponentState()
            {
              SelfTargetDoAfterMultiplier = component.SelfTargetDoAfterMultiplier
            };
            return;
          case 256 /*0x0100*/:
            args.State = (IComponentState) new WoundTreaterComponent.Damage_FieldComponentState()
            {
              Damage = component.Damage
            };
            return;
          case 512 /*0x0200*/:
            args.State = (IComponentState) new WoundTreaterComponent.UnskilledDamage_FieldComponentState()
            {
              UnskilledDamage = component.UnskilledDamage
            };
            return;
          case 1024 /*0x0400*/:
            args.State = (IComponentState) new WoundTreaterComponent.CanUseUnskilled_FieldComponentState()
            {
              CanUseUnskilled = component.CanUseUnskilled
            };
            return;
          case 2048 /*0x0800*/:
            args.State = (IComponentState) new WoundTreaterComponent.Skills_FieldComponentState()
            {
              Skills = component.Skills
            };
            return;
          case 4096 /*0x1000*/:
            args.State = (IComponentState) new WoundTreaterComponent.TreatBeginSound_FieldComponentState()
            {
              TreatBeginSound = component.TreatBeginSound
            };
            return;
          case 8192 /*0x2000*/:
            args.State = (IComponentState) new WoundTreaterComponent.TreatEndSound_FieldComponentState()
            {
              TreatEndSound = component.TreatEndSound
            };
            return;
          case 16384 /*0x4000*/:
            args.State = (IComponentState) new WoundTreaterComponent.UserPopup_FieldComponentState()
            {
              UserPopup = component.UserPopup
            };
            return;
          case 32768 /*0x8000*/:
            args.State = (IComponentState) new WoundTreaterComponent.TargetPopup_FieldComponentState()
            {
              TargetPopup = component.TargetPopup
            };
            return;
          case 65536 /*0x010000*/:
            args.State = (IComponentState) new WoundTreaterComponent.OthersPopup_FieldComponentState()
            {
              OthersPopup = component.OthersPopup
            };
            return;
          case 131072 /*0x020000*/:
            args.State = (IComponentState) new WoundTreaterComponent.TargetStartPopup_FieldComponentState()
            {
              TargetStartPopup = component.TargetStartPopup
            };
            return;
          case 262144 /*0x040000*/:
            args.State = (IComponentState) new WoundTreaterComponent.UserFinishPopup_FieldComponentState()
            {
              UserFinishPopup = component.UserFinishPopup
            };
            return;
          case 524288 /*0x080000*/:
            args.State = (IComponentState) new WoundTreaterComponent.TargetFinishPopup_FieldComponentState()
            {
              TargetFinishPopup = component.TargetFinishPopup
            };
            return;
          case 1048576 /*0x100000*/:
            args.State = (IComponentState) new WoundTreaterComponent.OthersFinishPopup_FieldComponentState()
            {
              OthersFinishPopup = component.OthersFinishPopup
            };
            return;
          case 2097152 /*0x200000*/:
            args.State = (IComponentState) new WoundTreaterComponent.NoneSelfPopup_FieldComponentState()
            {
              NoneSelfPopup = component.NoneSelfPopup
            };
            return;
          case 4194304 /*0x400000*/:
            args.State = (IComponentState) new WoundTreaterComponent.NoneOtherPopup_FieldComponentState()
            {
              NoneOtherPopup = component.NoneOtherPopup
            };
            return;
          case 8388608 /*0x800000*/:
            args.State = (IComponentState) new WoundTreaterComponent.NoWoundsOnUserPopup_FieldComponentState()
            {
              NoWoundsOnUserPopup = component.NoWoundsOnUserPopup
            };
            return;
          case 16777216 /*0x01000000*/:
            args.State = (IComponentState) new WoundTreaterComponent.NoWoundsOnTargetPopup_FieldComponentState()
            {
              NoWoundsOnTargetPopup = component.NoWoundsOnTargetPopup
            };
            return;
          case 33554432 /*0x02000000*/:
            args.State = (IComponentState) new WoundTreaterComponent.CurrentDoAfter_FieldComponentState()
            {
              CurrentDoAfter = component.CurrentDoAfter
            };
            return;
        }
      }
      args.State = (IComponentState) new WoundTreaterComponent.WoundTreaterComponent_AutoState()
      {
        Wound = component.Wound,
        Treats = component.Treats,
        Consumable = component.Consumable,
        Group = component.Group,
        ScalingDoAfter = component.ScalingDoAfter,
        DoAfterSkill = component.DoAfterSkill,
        DoAfterSkillMultipliers = component.DoAfterSkillMultipliers,
        SelfTargetDoAfterMultiplier = component.SelfTargetDoAfterMultiplier,
        Damage = component.Damage,
        UnskilledDamage = component.UnskilledDamage,
        CanUseUnskilled = component.CanUseUnskilled,
        Skills = component.Skills,
        TreatBeginSound = component.TreatBeginSound,
        TreatEndSound = component.TreatEndSound,
        UserPopup = component.UserPopup,
        TargetPopup = component.TargetPopup,
        OthersPopup = component.OthersPopup,
        TargetStartPopup = component.TargetStartPopup,
        UserFinishPopup = component.UserFinishPopup,
        TargetFinishPopup = component.TargetFinishPopup,
        OthersFinishPopup = component.OthersFinishPopup,
        NoneSelfPopup = component.NoneSelfPopup,
        NoneOtherPopup = component.NoneOtherPopup,
        NoWoundsOnUserPopup = component.NoWoundsOnUserPopup,
        NoWoundsOnTargetPopup = component.NoWoundsOnTargetPopup,
        CurrentDoAfter = component.CurrentDoAfter
      };
    }

    private void OnHandleState(
      EntityUid uid,
      WoundTreaterComponent component,
      ref ComponentHandleState args)
    {
      switch (args.Current)
      {
        case WoundTreaterComponent.Wound_FieldComponentState fieldComponentState1:
          component.Wound = fieldComponentState1.Wound;
          break;
        case WoundTreaterComponent.Treats_FieldComponentState fieldComponentState2:
          component.Treats = fieldComponentState2.Treats;
          break;
        case WoundTreaterComponent.Consumable_FieldComponentState fieldComponentState3:
          component.Consumable = fieldComponentState3.Consumable;
          break;
        case WoundTreaterComponent.Group_FieldComponentState fieldComponentState4:
          component.Group = fieldComponentState4.Group;
          break;
        case WoundTreaterComponent.ScalingDoAfter_FieldComponentState fieldComponentState5:
          component.ScalingDoAfter = fieldComponentState5.ScalingDoAfter;
          break;
        case WoundTreaterComponent.DoAfterSkill_FieldComponentState fieldComponentState6:
          component.DoAfterSkill = fieldComponentState6.DoAfterSkill;
          break;
        case WoundTreaterComponent.DoAfterSkillMultipliers_FieldComponentState fieldComponentState7:
          component.DoAfterSkillMultipliers = fieldComponentState7.DoAfterSkillMultipliers;
          break;
        case WoundTreaterComponent.SelfTargetDoAfterMultiplier_FieldComponentState fieldComponentState8:
          component.SelfTargetDoAfterMultiplier = fieldComponentState8.SelfTargetDoAfterMultiplier;
          break;
        case WoundTreaterComponent.Damage_FieldComponentState fieldComponentState9:
          component.Damage = fieldComponentState9.Damage;
          break;
        case WoundTreaterComponent.UnskilledDamage_FieldComponentState fieldComponentState10:
          component.UnskilledDamage = fieldComponentState10.UnskilledDamage;
          break;
        case WoundTreaterComponent.CanUseUnskilled_FieldComponentState fieldComponentState11:
          component.CanUseUnskilled = fieldComponentState11.CanUseUnskilled;
          break;
        case WoundTreaterComponent.Skills_FieldComponentState fieldComponentState12:
          Dictionary<EntProtoId<SkillDefinitionComponent>, int> skills = fieldComponentState12.Skills;
          if (skills == null)
          {
            component.Skills = (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null;
            break;
          }
          component.Skills = new Dictionary<EntProtoId<SkillDefinitionComponent>, int>((IDictionary<EntProtoId<SkillDefinitionComponent>, int>) skills);
          break;
        case WoundTreaterComponent.TreatBeginSound_FieldComponentState fieldComponentState13:
          component.TreatBeginSound = fieldComponentState13.TreatBeginSound;
          break;
        case WoundTreaterComponent.TreatEndSound_FieldComponentState fieldComponentState14:
          component.TreatEndSound = fieldComponentState14.TreatEndSound;
          break;
        case WoundTreaterComponent.UserPopup_FieldComponentState fieldComponentState15:
          component.UserPopup = fieldComponentState15.UserPopup;
          break;
        case WoundTreaterComponent.TargetPopup_FieldComponentState fieldComponentState16:
          component.TargetPopup = fieldComponentState16.TargetPopup;
          break;
        case WoundTreaterComponent.OthersPopup_FieldComponentState fieldComponentState17:
          component.OthersPopup = fieldComponentState17.OthersPopup;
          break;
        case WoundTreaterComponent.TargetStartPopup_FieldComponentState fieldComponentState18:
          component.TargetStartPopup = fieldComponentState18.TargetStartPopup;
          break;
        case WoundTreaterComponent.UserFinishPopup_FieldComponentState fieldComponentState19:
          component.UserFinishPopup = fieldComponentState19.UserFinishPopup;
          break;
        case WoundTreaterComponent.TargetFinishPopup_FieldComponentState fieldComponentState20:
          component.TargetFinishPopup = fieldComponentState20.TargetFinishPopup;
          break;
        case WoundTreaterComponent.OthersFinishPopup_FieldComponentState fieldComponentState21:
          component.OthersFinishPopup = fieldComponentState21.OthersFinishPopup;
          break;
        case WoundTreaterComponent.NoneSelfPopup_FieldComponentState fieldComponentState22:
          component.NoneSelfPopup = fieldComponentState22.NoneSelfPopup;
          break;
        case WoundTreaterComponent.NoneOtherPopup_FieldComponentState fieldComponentState23:
          component.NoneOtherPopup = fieldComponentState23.NoneOtherPopup;
          break;
        case WoundTreaterComponent.NoWoundsOnUserPopup_FieldComponentState fieldComponentState24:
          component.NoWoundsOnUserPopup = fieldComponentState24.NoWoundsOnUserPopup;
          break;
        case WoundTreaterComponent.NoWoundsOnTargetPopup_FieldComponentState fieldComponentState25:
          component.NoWoundsOnTargetPopup = fieldComponentState25.NoWoundsOnTargetPopup;
          break;
        case WoundTreaterComponent.CurrentDoAfter_FieldComponentState fieldComponentState26:
          component.CurrentDoAfter = fieldComponentState26.CurrentDoAfter;
          break;
        case WoundTreaterComponent.WoundTreaterComponent_AutoState componentAutoState:
          component.Wound = componentAutoState.Wound;
          component.Treats = componentAutoState.Treats;
          component.Consumable = componentAutoState.Consumable;
          component.Group = componentAutoState.Group;
          component.ScalingDoAfter = componentAutoState.ScalingDoAfter;
          component.DoAfterSkill = componentAutoState.DoAfterSkill;
          component.DoAfterSkillMultipliers = componentAutoState.DoAfterSkillMultipliers;
          component.SelfTargetDoAfterMultiplier = componentAutoState.SelfTargetDoAfterMultiplier;
          component.Damage = componentAutoState.Damage;
          component.UnskilledDamage = componentAutoState.UnskilledDamage;
          component.CanUseUnskilled = componentAutoState.CanUseUnskilled;
          component.Skills = componentAutoState.Skills == null ? (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null : new Dictionary<EntProtoId<SkillDefinitionComponent>, int>((IDictionary<EntProtoId<SkillDefinitionComponent>, int>) componentAutoState.Skills);
          component.TreatBeginSound = componentAutoState.TreatBeginSound;
          component.TreatEndSound = componentAutoState.TreatEndSound;
          component.UserPopup = componentAutoState.UserPopup;
          component.TargetPopup = componentAutoState.TargetPopup;
          component.OthersPopup = componentAutoState.OthersPopup;
          component.TargetStartPopup = componentAutoState.TargetStartPopup;
          component.UserFinishPopup = componentAutoState.UserFinishPopup;
          component.TargetFinishPopup = componentAutoState.TargetFinishPopup;
          component.OthersFinishPopup = componentAutoState.OthersFinishPopup;
          component.NoneSelfPopup = componentAutoState.NoneSelfPopup;
          component.NoneOtherPopup = componentAutoState.NoneOtherPopup;
          component.NoWoundsOnUserPopup = componentAutoState.NoWoundsOnUserPopup;
          component.NoWoundsOnTargetPopup = componentAutoState.NoWoundsOnTargetPopup;
          component.CurrentDoAfter = componentAutoState.CurrentDoAfter;
          break;
      }
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Wound_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public WoundType Wound;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.Wound = this.Wound;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Treats_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool Treats;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.Treats = this.Treats;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Consumable_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool Consumable;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.Consumable = this.Consumable;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Group_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public ProtoId<DamageGroupPrototype> Group;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.Group = this.Group;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class ScalingDoAfter_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan ScalingDoAfter;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.ScalingDoAfter = this.ScalingDoAfter;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class DoAfterSkill_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public EntProtoId<SkillDefinitionComponent> DoAfterSkill;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.DoAfterSkill = this.DoAfterSkill;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class DoAfterSkillMultipliers_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float[] DoAfterSkillMultipliers;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.DoAfterSkillMultipliers = this.DoAfterSkillMultipliers;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class SelfTargetDoAfterMultiplier_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float SelfTargetDoAfterMultiplier;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.SelfTargetDoAfterMultiplier = this.SelfTargetDoAfterMultiplier;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Damage_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public FixedPoint2? Damage;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.Damage = this.Damage;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class UnskilledDamage_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public FixedPoint2? UnskilledDamage;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.UnskilledDamage = this.UnskilledDamage;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class CanUseUnskilled_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool CanUseUnskilled;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.CanUseUnskilled = this.CanUseUnskilled;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Skills_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public Dictionary<EntProtoId<SkillDefinitionComponent>, int> Skills;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.Skills = this.Skills == null ? (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null : new Dictionary<EntProtoId<SkillDefinitionComponent>, int>((IDictionary<EntProtoId<SkillDefinitionComponent>, int>) this.Skills);
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class TreatBeginSound_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public SoundSpecifier? TreatBeginSound;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.TreatBeginSound = this.TreatBeginSound;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class TreatEndSound_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public SoundSpecifier? TreatEndSound;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.TreatEndSound = this.TreatEndSound;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class UserPopup_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public LocId? UserPopup;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.UserPopup = this.UserPopup;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class TargetPopup_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public LocId? TargetPopup;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.TargetPopup = this.TargetPopup;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class OthersPopup_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public LocId? OthersPopup;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.OthersPopup = this.OthersPopup;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class TargetStartPopup_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public LocId? TargetStartPopup;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.TargetStartPopup = this.TargetStartPopup;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class UserFinishPopup_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public LocId? UserFinishPopup;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.UserFinishPopup = this.UserFinishPopup;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class TargetFinishPopup_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public LocId? TargetFinishPopup;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.TargetFinishPopup = this.TargetFinishPopup;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class OthersFinishPopup_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public LocId? OthersFinishPopup;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.OthersFinishPopup = this.OthersFinishPopup;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class NoneSelfPopup_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public LocId? NoneSelfPopup;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.NoneSelfPopup = this.NoneSelfPopup;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class NoneOtherPopup_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public LocId? NoneOtherPopup;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.NoneOtherPopup = this.NoneOtherPopup;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class NoWoundsOnUserPopup_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public LocId? NoWoundsOnUserPopup;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.NoWoundsOnUserPopup = this.NoWoundsOnUserPopup;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class NoWoundsOnTargetPopup_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public LocId? NoWoundsOnTargetPopup;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.NoWoundsOnTargetPopup = this.NoWoundsOnTargetPopup;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class CurrentDoAfter_FieldComponentState : 
    IComponentDeltaState<WoundTreaterComponent.WoundTreaterComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public ushort? CurrentDoAfter;

    public void ApplyToFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      fullState.CurrentDoAfter = this.CurrentDoAfter;
    }

    public WoundTreaterComponent.WoundTreaterComponent_AutoState CreateNewFullState(
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState)
    {
      WoundTreaterComponent.WoundTreaterComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
