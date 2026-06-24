// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Damage.DamageOverTimeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chat.Prototypes;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Physics;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Damage;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCDamageableSystem)})]
public sealed class DamageOverTimeComponent : 
  Component,
  ISerializationGenerated<DamageOverTimeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier? Damage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier? ArmorPiercingDamage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier? BarricadeDamage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? BarricadeSound = (SoundSpecifier) new SoundCollectionSpecifier("XenoAcidSizzle", new AudioParams?(AudioParams.Default.WithVolume(-3f)));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? Sound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DamageEvery = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan NextDamageAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AffectsDead;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AffectsInfectedNested;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AffectsCrit = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Acidic = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<ProtoId<EmotePrototype>>? Emotes = new List<ProtoId<EmotePrototype>>()
  {
    (ProtoId<EmotePrototype>) "Cough"
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? Popup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<DamageOverTimeComponent.DamageMultiplier>? Multipliers;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public CollisionGroup Collision = CollisionGroup.LargeMobLayer | CollisionGroup.Impassable;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool InitDamaged;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? DuplicateId;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DamageOverTimeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DamageOverTimeComponent) target1;
    if (serialization.TryCustomCopy<DamageOverTimeComponent>(this, ref target, hookCtx, false, context))
      return;
    DamageSpecifier target2 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target2, hookCtx, false, context))
    {
      if (this.Damage == null)
        target2 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target2, hookCtx, context);
    }
    target.Damage = target2;
    DamageSpecifier target3 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.ArmorPiercingDamage, ref target3, hookCtx, false, context))
    {
      if (this.ArmorPiercingDamage == null)
        target3 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.ArmorPiercingDamage, ref target3, hookCtx, context);
    }
    target.ArmorPiercingDamage = target3;
    DamageSpecifier target4 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.BarricadeDamage, ref target4, hookCtx, false, context))
    {
      if (this.BarricadeDamage == null)
        target4 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.BarricadeDamage, ref target4, hookCtx, context);
    }
    target.BarricadeDamage = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BarricadeSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.BarricadeSound, hookCtx, context);
    target.BarricadeSound = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DamageEvery, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.DamageEvery, hookCtx, context);
    target.DamageEvery = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextDamageAt, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.NextDamageAt, hookCtx, context);
    target.NextDamageAt = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.AffectsDead, ref target9, hookCtx, false, context))
      target9 = this.AffectsDead;
    target.AffectsDead = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.AffectsInfectedNested, ref target10, hookCtx, false, context))
      target10 = this.AffectsInfectedNested;
    target.AffectsInfectedNested = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.AffectsCrit, ref target11, hookCtx, false, context))
      target11 = this.AffectsCrit;
    target.AffectsCrit = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.Acidic, ref target12, hookCtx, false, context))
      target12 = this.Acidic;
    target.Acidic = target12;
    List<ProtoId<EmotePrototype>> target13 = (List<ProtoId<EmotePrototype>>) null;
    if (!serialization.TryCustomCopy<List<ProtoId<EmotePrototype>>>(this.Emotes, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<List<ProtoId<EmotePrototype>>>(this.Emotes, hookCtx, context);
    target.Emotes = target13;
    string target14 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Popup, ref target14, hookCtx, false, context))
      target14 = this.Popup;
    target.Popup = target14;
    EntityWhitelist target15 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target15, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target15 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target15, hookCtx, context);
    }
    target.Whitelist = target15;
    List<DamageOverTimeComponent.DamageMultiplier> target16 = (List<DamageOverTimeComponent.DamageMultiplier>) null;
    if (!serialization.TryCustomCopy<List<DamageOverTimeComponent.DamageMultiplier>>(this.Multipliers, ref target16, hookCtx, true, context))
      target16 = serialization.CreateCopy<List<DamageOverTimeComponent.DamageMultiplier>>(this.Multipliers, hookCtx, context);
    target.Multipliers = target16;
    CollisionGroup target17 = CollisionGroup.None;
    if (!serialization.TryCustomCopy<CollisionGroup>(this.Collision, ref target17, hookCtx, false, context))
      target17 = this.Collision;
    target.Collision = target17;
    bool target18 = false;
    if (!serialization.TryCustomCopy<bool>(this.InitDamaged, ref target18, hookCtx, false, context))
      target18 = this.InitDamaged;
    target.InitDamaged = target18;
    EntProtoId? target19 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.DuplicateId, ref target19, hookCtx, false, context))
      target19 = serialization.CreateCopy<EntProtoId?>(this.DuplicateId, hookCtx, context);
    target.DuplicateId = target19;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DamageOverTimeComponent target,
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
    DamageOverTimeComponent target1 = (DamageOverTimeComponent) target;
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
    DamageOverTimeComponent target1 = (DamageOverTimeComponent) target;
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
    DamageOverTimeComponent target1 = (DamageOverTimeComponent) target;
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
  virtual DamageOverTimeComponent Component.Instantiate() => new DamageOverTimeComponent();

  [DataRecord]
  [NetSerializable]
  [Serializable]
  public readonly record struct DamageMultiplier(FixedPoint2 Multiplier, EntityWhitelist Whitelist);

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DamageOverTimeComponent_AutoState : IComponentState
  {
    public DamageSpecifier? Damage;
    public DamageSpecifier? ArmorPiercingDamage;
    public DamageSpecifier? BarricadeDamage;
    public SoundSpecifier? BarricadeSound;
    public SoundSpecifier? Sound;
    public TimeSpan DamageEvery;
    public TimeSpan NextDamageAt;
    public bool AffectsDead;
    public bool AffectsInfectedNested;
    public bool AffectsCrit;
    public bool Acidic;
    public List<ProtoId<EmotePrototype>>? Emotes;
    public string? Popup;
    public EntityWhitelist? Whitelist;
    public List<DamageOverTimeComponent.DamageMultiplier>? Multipliers;
    public CollisionGroup Collision;
    public bool InitDamaged;
    public EntProtoId? DuplicateId;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DamageOverTimeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DamageOverTimeComponent, ComponentGetState>(new ComponentEventRefHandler<DamageOverTimeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DamageOverTimeComponent, ComponentHandleState>(new ComponentEventRefHandler<DamageOverTimeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DamageOverTimeComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DamageOverTimeComponent.DamageOverTimeComponent_AutoState()
      {
        Damage = component.Damage,
        ArmorPiercingDamage = component.ArmorPiercingDamage,
        BarricadeDamage = component.BarricadeDamage,
        BarricadeSound = component.BarricadeSound,
        Sound = component.Sound,
        DamageEvery = component.DamageEvery,
        NextDamageAt = component.NextDamageAt,
        AffectsDead = component.AffectsDead,
        AffectsInfectedNested = component.AffectsInfectedNested,
        AffectsCrit = component.AffectsCrit,
        Acidic = component.Acidic,
        Emotes = component.Emotes,
        Popup = component.Popup,
        Whitelist = component.Whitelist,
        Multipliers = component.Multipliers,
        Collision = component.Collision,
        InitDamaged = component.InitDamaged,
        DuplicateId = component.DuplicateId
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DamageOverTimeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DamageOverTimeComponent.DamageOverTimeComponent_AutoState current))
        return;
      component.Damage = current.Damage;
      component.ArmorPiercingDamage = current.ArmorPiercingDamage;
      component.BarricadeDamage = current.BarricadeDamage;
      component.BarricadeSound = current.BarricadeSound;
      component.Sound = current.Sound;
      component.DamageEvery = current.DamageEvery;
      component.NextDamageAt = current.NextDamageAt;
      component.AffectsDead = current.AffectsDead;
      component.AffectsInfectedNested = current.AffectsInfectedNested;
      component.AffectsCrit = current.AffectsCrit;
      component.Acidic = current.Acidic;
      component.Emotes = current.Emotes == null ? (List<ProtoId<EmotePrototype>>) null : new List<ProtoId<EmotePrototype>>((IEnumerable<ProtoId<EmotePrototype>>) current.Emotes);
      component.Popup = current.Popup;
      component.Whitelist = current.Whitelist;
      component.Multipliers = current.Multipliers == null ? (List<DamageOverTimeComponent.DamageMultiplier>) null : new List<DamageOverTimeComponent.DamageMultiplier>((IEnumerable<DamageOverTimeComponent.DamageMultiplier>) current.Multipliers);
      component.Collision = current.Collision;
      component.InitDamaged = current.InitDamaged;
      component.DuplicateId = current.DuplicateId;
    }
  }
}
