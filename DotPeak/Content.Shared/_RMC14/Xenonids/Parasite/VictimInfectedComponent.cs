// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Parasite.VictimInfectedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Medical.Surgery;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Damage;
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
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Parasite;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedXenoParasiteSystem)})]
public sealed class VictimInfectedComponent : 
  Component,
  ISerializationGenerated<VictimInfectedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier[] InfectedIcons = new SpriteSpecifier[7]
  {
    (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_hud.rsi"), "infected0"),
    (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_hud.rsi"), "infected1"),
    (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_hud.rsi"), "infected2"),
    (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_hud.rsi"), "infected3"),
    (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_hud.rsi"), "infected4"),
    (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_hud.rsi"), "infected5"),
    (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_hud.rsi"), "infected6")
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string LarvaContainerId = "rmc_larva_container";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? SpawnedLarva;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan BurstDelay = TimeSpan.FromMinutes(8L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan AutoBurstTime = TimeSpan.FromSeconds(60L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan BurstAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float IncubationMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId BurstSpawn = (EntProtoId) "CMXenoLarva";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier BurstSound = (SoundSpecifier) new SoundCollectionSpecifier("XenoChestBurst");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Access(new Type[] {typeof (SharedCMSurgerySystem)})]
  public bool RootsCut;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Hive;
  [DataField(null, false, 1, false, false, null)]
  public int FinalStage = 6;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int CurrentStage;
  [DataField(null, false, 1, false, false, null)]
  public int InitialSymptomsStart = 2;
  [DataField(null, false, 1, false, false, null)]
  public int MiddlingSymptomsStart = 3;
  [DataField(null, false, 1, false, false, null)]
  public int FinalSymptomsStart = 4;
  [DataField(null, false, 1, false, false, null)]
  public int BurstWarningStart = 6;
  [DataField(null, false, 1, false, false, null)]
  public float ShakesChance = 0.08f;
  [DataField(null, false, 1, false, false, null)]
  public float MinorPainChance = 0.03f;
  [DataField(null, false, 1, false, false, null)]
  public float ThroatPainChance = 0.015f;
  [DataField(null, false, 1, false, false, null)]
  public float MuscleAcheChance = 0.015f;
  [DataField(null, false, 1, false, false, null)]
  public float SneezeCoughChance = 0.015f;
  [DataField(null, false, 1, false, false, null)]
  public float MajorPainChance = 0.1f;
  [DataField(null, false, 1, false, false, null)]
  public float InsanePainChance = 0.15f;
  [DataField(null, false, 1, false, false, null)]
  public bool DidBurstWarning;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsBursting;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan BaseKnockdownTime = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan JitterTime = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<EmotePrototype> SneezeId = (ProtoId<EmotePrototype>) "Sneeze";
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<EmotePrototype> CoughId = (ProtoId<EmotePrototype>) "Cough";
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<EmotePrototype> ScreamId = (ProtoId<EmotePrototype>) "Scream";
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier InfectionDamage = new DamageSpecifier()
  {
    DamageDict = new Dictionary<string, FixedPoint2>()
    {
      {
        "Blunt",
        (FixedPoint2) 1
      }
    }
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan BurstDoAfterDelay = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan LarvaInvincibilityTime = TimeSpan.FromSeconds(1L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VictimInfectedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VictimInfectedComponent) target1;
    if (serialization.TryCustomCopy<VictimInfectedComponent>(this, ref target, hookCtx, false, context))
      return;
    SpriteSpecifier[] target2 = (SpriteSpecifier[]) null;
    if (this.InfectedIcons == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier[]>(this.InfectedIcons, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SpriteSpecifier[]>(this.InfectedIcons, hookCtx, context);
    target.InfectedIcons = target2;
    string target3 = (string) null;
    if (this.LarvaContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.LarvaContainerId, ref target3, hookCtx, false, context))
      target3 = this.LarvaContainerId;
    target.LarvaContainerId = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.SpawnedLarva, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.SpawnedLarva, hookCtx, context);
    target.SpawnedLarva = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BurstDelay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.BurstDelay, hookCtx, context);
    target.BurstDelay = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AutoBurstTime, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.AutoBurstTime, hookCtx, context);
    target.AutoBurstTime = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BurstAt, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.BurstAt, hookCtx, context);
    target.BurstAt = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.IncubationMultiplier, ref target8, hookCtx, false, context))
      target8 = this.IncubationMultiplier;
    target.IncubationMultiplier = target8;
    EntProtoId target9 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.BurstSpawn, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<EntProtoId>(this.BurstSpawn, hookCtx, context);
    target.BurstSpawn = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (this.BurstSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BurstSound, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.BurstSound, hookCtx, context);
    target.BurstSound = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.RootsCut, ref target11, hookCtx, false, context))
      target11 = this.RootsCut;
    target.RootsCut = target11;
    EntityUid? target12 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Hive, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<EntityUid?>(this.Hive, hookCtx, context);
    target.Hive = target12;
    int target13 = 0;
    if (!serialization.TryCustomCopy<int>(this.FinalStage, ref target13, hookCtx, false, context))
      target13 = this.FinalStage;
    target.FinalStage = target13;
    int target14 = 0;
    if (!serialization.TryCustomCopy<int>(this.CurrentStage, ref target14, hookCtx, false, context))
      target14 = this.CurrentStage;
    target.CurrentStage = target14;
    int target15 = 0;
    if (!serialization.TryCustomCopy<int>(this.InitialSymptomsStart, ref target15, hookCtx, false, context))
      target15 = this.InitialSymptomsStart;
    target.InitialSymptomsStart = target15;
    int target16 = 0;
    if (!serialization.TryCustomCopy<int>(this.MiddlingSymptomsStart, ref target16, hookCtx, false, context))
      target16 = this.MiddlingSymptomsStart;
    target.MiddlingSymptomsStart = target16;
    int target17 = 0;
    if (!serialization.TryCustomCopy<int>(this.FinalSymptomsStart, ref target17, hookCtx, false, context))
      target17 = this.FinalSymptomsStart;
    target.FinalSymptomsStart = target17;
    int target18 = 0;
    if (!serialization.TryCustomCopy<int>(this.BurstWarningStart, ref target18, hookCtx, false, context))
      target18 = this.BurstWarningStart;
    target.BurstWarningStart = target18;
    float target19 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ShakesChance, ref target19, hookCtx, false, context))
      target19 = this.ShakesChance;
    target.ShakesChance = target19;
    float target20 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinorPainChance, ref target20, hookCtx, false, context))
      target20 = this.MinorPainChance;
    target.MinorPainChance = target20;
    float target21 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ThroatPainChance, ref target21, hookCtx, false, context))
      target21 = this.ThroatPainChance;
    target.ThroatPainChance = target21;
    float target22 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MuscleAcheChance, ref target22, hookCtx, false, context))
      target22 = this.MuscleAcheChance;
    target.MuscleAcheChance = target22;
    float target23 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SneezeCoughChance, ref target23, hookCtx, false, context))
      target23 = this.SneezeCoughChance;
    target.SneezeCoughChance = target23;
    float target24 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MajorPainChance, ref target24, hookCtx, false, context))
      target24 = this.MajorPainChance;
    target.MajorPainChance = target24;
    float target25 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.InsanePainChance, ref target25, hookCtx, false, context))
      target25 = this.InsanePainChance;
    target.InsanePainChance = target25;
    bool target26 = false;
    if (!serialization.TryCustomCopy<bool>(this.DidBurstWarning, ref target26, hookCtx, false, context))
      target26 = this.DidBurstWarning;
    target.DidBurstWarning = target26;
    bool target27 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsBursting, ref target27, hookCtx, false, context))
      target27 = this.IsBursting;
    target.IsBursting = target27;
    TimeSpan target28 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BaseKnockdownTime, ref target28, hookCtx, false, context))
      target28 = serialization.CreateCopy<TimeSpan>(this.BaseKnockdownTime, hookCtx, context);
    target.BaseKnockdownTime = target28;
    TimeSpan target29 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.JitterTime, ref target29, hookCtx, false, context))
      target29 = serialization.CreateCopy<TimeSpan>(this.JitterTime, hookCtx, context);
    target.JitterTime = target29;
    ProtoId<EmotePrototype> target30 = new ProtoId<EmotePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>>(this.SneezeId, ref target30, hookCtx, false, context))
      target30 = serialization.CreateCopy<ProtoId<EmotePrototype>>(this.SneezeId, hookCtx, context);
    target.SneezeId = target30;
    ProtoId<EmotePrototype> target31 = new ProtoId<EmotePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>>(this.CoughId, ref target31, hookCtx, false, context))
      target31 = serialization.CreateCopy<ProtoId<EmotePrototype>>(this.CoughId, hookCtx, context);
    target.CoughId = target31;
    ProtoId<EmotePrototype> target32 = new ProtoId<EmotePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>>(this.ScreamId, ref target32, hookCtx, false, context))
      target32 = serialization.CreateCopy<ProtoId<EmotePrototype>>(this.ScreamId, hookCtx, context);
    target.ScreamId = target32;
    DamageSpecifier target33 = (DamageSpecifier) null;
    if (this.InfectionDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.InfectionDamage, ref target33, hookCtx, false, context))
    {
      if (this.InfectionDamage == null)
        target33 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.InfectionDamage, ref target33, hookCtx, context, true);
    }
    target.InfectionDamage = target33;
    TimeSpan target34 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BurstDoAfterDelay, ref target34, hookCtx, false, context))
      target34 = serialization.CreateCopy<TimeSpan>(this.BurstDoAfterDelay, hookCtx, context);
    target.BurstDoAfterDelay = target34;
    TimeSpan target35 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LarvaInvincibilityTime, ref target35, hookCtx, false, context))
      target35 = serialization.CreateCopy<TimeSpan>(this.LarvaInvincibilityTime, hookCtx, context);
    target.LarvaInvincibilityTime = target35;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VictimInfectedComponent target,
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
    VictimInfectedComponent target1 = (VictimInfectedComponent) target;
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
    VictimInfectedComponent target1 = (VictimInfectedComponent) target;
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
    VictimInfectedComponent target1 = (VictimInfectedComponent) target;
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
  virtual VictimInfectedComponent Component.Instantiate() => new VictimInfectedComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VictimInfectedComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VictimInfectedComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<VictimInfectedComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      VictimInfectedComponent component,
      ref EntityUnpausedEvent args)
    {
      component.BurstAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class VictimInfectedComponent_AutoState : IComponentState
  {
    public 
    #nullable enable
    SpriteSpecifier[] InfectedIcons;
    public string LarvaContainerId;
    public NetEntity? SpawnedLarva;
    public TimeSpan BurstDelay;
    public TimeSpan AutoBurstTime;
    public TimeSpan BurstAt;
    public float IncubationMultiplier;
    public EntProtoId BurstSpawn;
    public SoundSpecifier BurstSound;
    public bool RootsCut;
    public NetEntity? Hive;
    public int CurrentStage;
    public bool IsBursting;
    public TimeSpan BurstDoAfterDelay;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VictimInfectedComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VictimInfectedComponent, ComponentGetState>(new ComponentEventRefHandler<VictimInfectedComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<VictimInfectedComponent, ComponentHandleState>(new ComponentEventRefHandler<VictimInfectedComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      VictimInfectedComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new VictimInfectedComponent.VictimInfectedComponent_AutoState()
      {
        InfectedIcons = component.InfectedIcons,
        LarvaContainerId = component.LarvaContainerId,
        SpawnedLarva = this.GetNetEntity(component.SpawnedLarva),
        BurstDelay = component.BurstDelay,
        AutoBurstTime = component.AutoBurstTime,
        BurstAt = component.BurstAt,
        IncubationMultiplier = component.IncubationMultiplier,
        BurstSpawn = component.BurstSpawn,
        BurstSound = component.BurstSound,
        RootsCut = component.RootsCut,
        Hive = this.GetNetEntity(component.Hive),
        CurrentStage = component.CurrentStage,
        IsBursting = component.IsBursting,
        BurstDoAfterDelay = component.BurstDoAfterDelay
      };
    }

    private void OnHandleState(
      EntityUid uid,
      VictimInfectedComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is VictimInfectedComponent.VictimInfectedComponent_AutoState current))
        return;
      component.InfectedIcons = current.InfectedIcons;
      component.LarvaContainerId = current.LarvaContainerId;
      component.SpawnedLarva = this.EnsureEntity<VictimInfectedComponent>(current.SpawnedLarva, uid);
      component.BurstDelay = current.BurstDelay;
      component.AutoBurstTime = current.AutoBurstTime;
      component.BurstAt = current.BurstAt;
      component.IncubationMultiplier = current.IncubationMultiplier;
      component.BurstSpawn = current.BurstSpawn;
      component.BurstSound = current.BurstSound;
      component.RootsCut = current.RootsCut;
      component.Hive = this.EnsureEntity<VictimInfectedComponent>(current.Hive, uid);
      component.CurrentStage = current.CurrentStage;
      component.IsBursting = current.IsBursting;
      component.BurstDoAfterDelay = current.BurstDoAfterDelay;
    }
  }
}
