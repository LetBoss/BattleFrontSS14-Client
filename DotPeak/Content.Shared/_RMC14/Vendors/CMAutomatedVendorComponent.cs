// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vendors.CMAutomatedVendorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Roles.Ranks;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Access;
using Content.Shared.Roles;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Vendors;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Robust.Shared.Analyzers.Access(new Type[] {typeof (SharedCMAutomatedVendorSystem)})]
public sealed class CMAutomatedVendorComponent : 
  Component,
  ISerializationGenerated<CMAutomatedVendorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? PointsType;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<ProtoId<JobPrototype>> Jobs = new List<ProtoId<JobPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<ProtoId<RankPrototype>> Ranks = new List<ProtoId<RankPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<CMVendorSection> Sections = new List<CMVendorSection>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 MinOffset = new Vector2(-0.2f, -0.2f);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 MaxOffset = new Vector2(0.2f, 0.2f);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Hackable;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Hacked;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> HackSkill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillEngineer";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int HackSkillLevel = 2;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan HackDelay = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<ProtoId<AccessLevelPrototype>> Access = new List<ProtoId<AccessLevelPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Scaling = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int? RandomUnstockAmount;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float? RandomEmptyChance;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? Sound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi? BaseSprite;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi? AnimationSprite;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool EjectContentsOnDestruction;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CMAutomatedVendorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CMAutomatedVendorComponent) target1;
    if (serialization.TryCustomCopy<CMAutomatedVendorComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.PointsType, ref target2, hookCtx, false, context))
      target2 = this.PointsType;
    target.PointsType = target2;
    List<ProtoId<JobPrototype>> target3 = (List<ProtoId<JobPrototype>>) null;
    if (this.Jobs == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<JobPrototype>>>(this.Jobs, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<ProtoId<JobPrototype>>>(this.Jobs, hookCtx, context);
    target.Jobs = target3;
    List<ProtoId<RankPrototype>> target4 = (List<ProtoId<RankPrototype>>) null;
    if (this.Ranks == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<RankPrototype>>>(this.Ranks, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<ProtoId<RankPrototype>>>(this.Ranks, hookCtx, context);
    target.Ranks = target4;
    List<CMVendorSection> target5 = (List<CMVendorSection>) null;
    if (this.Sections == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<CMVendorSection>>(this.Sections, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<List<CMVendorSection>>(this.Sections, hookCtx, context);
    target.Sections = target5;
    Vector2 target6 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.MinOffset, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<Vector2>(this.MinOffset, hookCtx, context);
    target.MinOffset = target6;
    Vector2 target7 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.MaxOffset, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<Vector2>(this.MaxOffset, hookCtx, context);
    target.MaxOffset = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.Hackable, ref target8, hookCtx, false, context))
      target8 = this.Hackable;
    target.Hackable = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.Hacked, ref target9, hookCtx, false, context))
      target9 = this.Hacked;
    target.Hacked = target9;
    EntProtoId<SkillDefinitionComponent> target10 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.HackSkill, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.HackSkill, hookCtx, context);
    target.HackSkill = target10;
    int target11 = 0;
    if (!serialization.TryCustomCopy<int>(this.HackSkillLevel, ref target11, hookCtx, false, context))
      target11 = this.HackSkillLevel;
    target.HackSkillLevel = target11;
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.HackDelay, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.HackDelay, hookCtx, context);
    target.HackDelay = target12;
    List<ProtoId<AccessLevelPrototype>> target13 = (List<ProtoId<AccessLevelPrototype>>) null;
    if (this.Access == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<AccessLevelPrototype>>>(this.Access, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<List<ProtoId<AccessLevelPrototype>>>(this.Access, hookCtx, context);
    target.Access = target13;
    bool target14 = false;
    if (!serialization.TryCustomCopy<bool>(this.Scaling, ref target14, hookCtx, false, context))
      target14 = this.Scaling;
    target.Scaling = target14;
    int? target15 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.RandomUnstockAmount, ref target15, hookCtx, false, context))
      target15 = this.RandomUnstockAmount;
    target.RandomUnstockAmount = target15;
    float? target16 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.RandomEmptyChance, ref target16, hookCtx, false, context))
      target16 = this.RandomEmptyChance;
    target.RandomEmptyChance = target16;
    SoundSpecifier target17 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target17, hookCtx, true, context))
      target17 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target17;
    SpriteSpecifier.Rsi target18 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.BaseSprite, ref target18, hookCtx, false, context))
    {
      if (this.BaseSprite == null)
        target18 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.BaseSprite, ref target18, hookCtx, context);
    }
    target.BaseSprite = target18;
    SpriteSpecifier.Rsi target19 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.AnimationSprite, ref target19, hookCtx, false, context))
    {
      if (this.AnimationSprite == null)
        target19 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.AnimationSprite, ref target19, hookCtx, context);
    }
    target.AnimationSprite = target19;
    bool target20 = false;
    if (!serialization.TryCustomCopy<bool>(this.EjectContentsOnDestruction, ref target20, hookCtx, false, context))
      target20 = this.EjectContentsOnDestruction;
    target.EjectContentsOnDestruction = target20;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CMAutomatedVendorComponent target,
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
    CMAutomatedVendorComponent target1 = (CMAutomatedVendorComponent) target;
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
    CMAutomatedVendorComponent target1 = (CMAutomatedVendorComponent) target;
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
    CMAutomatedVendorComponent target1 = (CMAutomatedVendorComponent) target;
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
  virtual CMAutomatedVendorComponent Component.Instantiate() => new CMAutomatedVendorComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CMAutomatedVendorComponent_AutoState : IComponentState
  {
    public string? PointsType;
    public List<ProtoId<JobPrototype>> Jobs;
    public List<ProtoId<RankPrototype>> Ranks;
    public List<CMVendorSection> Sections;
    public Vector2 MinOffset;
    public Vector2 MaxOffset;
    public bool Hackable;
    public bool Hacked;
    public EntProtoId<SkillDefinitionComponent> HackSkill;
    public int HackSkillLevel;
    public TimeSpan HackDelay;
    public List<ProtoId<AccessLevelPrototype>> Access;
    public bool Scaling;
    public int? RandomUnstockAmount;
    public float? RandomEmptyChance;
    public SoundSpecifier? Sound;
    public SpriteSpecifier.Rsi? BaseSprite;
    public SpriteSpecifier.Rsi? AnimationSprite;
    public bool EjectContentsOnDestruction;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CMAutomatedVendorComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CMAutomatedVendorComponent, ComponentGetState>(new ComponentEventRefHandler<CMAutomatedVendorComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CMAutomatedVendorComponent, ComponentHandleState>(new ComponentEventRefHandler<CMAutomatedVendorComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CMAutomatedVendorComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CMAutomatedVendorComponent.CMAutomatedVendorComponent_AutoState()
      {
        PointsType = component.PointsType,
        Jobs = component.Jobs,
        Ranks = component.Ranks,
        Sections = component.Sections,
        MinOffset = component.MinOffset,
        MaxOffset = component.MaxOffset,
        Hackable = component.Hackable,
        Hacked = component.Hacked,
        HackSkill = component.HackSkill,
        HackSkillLevel = component.HackSkillLevel,
        HackDelay = component.HackDelay,
        Access = component.Access,
        Scaling = component.Scaling,
        RandomUnstockAmount = component.RandomUnstockAmount,
        RandomEmptyChance = component.RandomEmptyChance,
        Sound = component.Sound,
        BaseSprite = component.BaseSprite,
        AnimationSprite = component.AnimationSprite,
        EjectContentsOnDestruction = component.EjectContentsOnDestruction
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CMAutomatedVendorComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CMAutomatedVendorComponent.CMAutomatedVendorComponent_AutoState current))
        return;
      component.PointsType = current.PointsType;
      component.Jobs = current.Jobs == null ? (List<ProtoId<JobPrototype>>) null : new List<ProtoId<JobPrototype>>((IEnumerable<ProtoId<JobPrototype>>) current.Jobs);
      component.Ranks = current.Ranks == null ? (List<ProtoId<RankPrototype>>) null : new List<ProtoId<RankPrototype>>((IEnumerable<ProtoId<RankPrototype>>) current.Ranks);
      component.Sections = current.Sections == null ? (List<CMVendorSection>) null : new List<CMVendorSection>((IEnumerable<CMVendorSection>) current.Sections);
      component.MinOffset = current.MinOffset;
      component.MaxOffset = current.MaxOffset;
      component.Hackable = current.Hackable;
      component.Hacked = current.Hacked;
      component.HackSkill = current.HackSkill;
      component.HackSkillLevel = current.HackSkillLevel;
      component.HackDelay = current.HackDelay;
      component.Access = current.Access == null ? (List<ProtoId<AccessLevelPrototype>>) null : new List<ProtoId<AccessLevelPrototype>>((IEnumerable<ProtoId<AccessLevelPrototype>>) current.Access);
      component.Scaling = current.Scaling;
      component.RandomUnstockAmount = current.RandomUnstockAmount;
      component.RandomEmptyChance = current.RandomEmptyChance;
      component.Sound = current.Sound;
      component.BaseSprite = current.BaseSprite;
      component.AnimationSprite = current.AnimationSprite;
      component.EjectContentsOnDestruction = current.EjectContentsOnDestruction;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, CMAutomatedVendorComponent>(uid, component, ref args1);
    }
  }
}
