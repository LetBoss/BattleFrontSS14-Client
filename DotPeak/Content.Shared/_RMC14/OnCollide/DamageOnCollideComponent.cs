// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.OnCollide.DamageOnCollideComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Atmos;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Damage;
using Content.Shared.Physics;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.OnCollide;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedOnCollideSystem)})]
public sealed class DamageOnCollideComponent : 
  Component,
  ISerializationGenerated<DamageOnCollideComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool InitDamaged;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? Chain;
  [DataField(null, false, 1, true, false, null)]
  [Access(new Type[] {typeof (SharedOnCollideSystem), typeof (SharedRMCFlammableSystem)})]
  public DamageSpecifier Damage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier ChainDamage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  public HashSet<EntityUid> Damaged = new HashSet<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  public bool DamageDead;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<EmotePrototype>? Emote = (ProtoId<EmotePrototype>?) "Scream";
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<EmotePrototype>? XenoEmote = (ProtoId<EmotePrototype>?) "Hiss";
  [DataField(null, false, 1, false, false, null)]
  public bool Acidic;
  [DataField(null, false, 1, false, false, null)]
  public bool Fire;
  [DataField(null, false, 1, false, false, null)]
  public CollisionGroup Collision = CollisionGroup.FullTileLayer;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan AcidComboDuration;
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier? AcidComboDamage;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan AcidComboParalyze;
  [DataField(null, false, 1, false, false, null)]
  public int AcidComboResists;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Paralyze;
  [DataField(null, false, 1, false, false, null)]
  public bool IgnoreResistances;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DamageOnCollideComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DamageOnCollideComponent) target1;
    if (serialization.TryCustomCopy<DamageOnCollideComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.InitDamaged, ref target2, hookCtx, false, context))
      target2 = this.InitDamaged;
    target.InitDamaged = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Chain, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.Chain, hookCtx, context);
    target.Chain = target3;
    DamageSpecifier target4 = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target4, hookCtx, false, context))
    {
      if (this.Damage == null)
        target4 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target4, hookCtx, context, true);
    }
    target.Damage = target4;
    DamageSpecifier target5 = (DamageSpecifier) null;
    if (this.ChainDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.ChainDamage, ref target5, hookCtx, false, context))
    {
      if (this.ChainDamage == null)
        target5 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.ChainDamage, ref target5, hookCtx, context, true);
    }
    target.ChainDamage = target5;
    HashSet<EntityUid> target6 = (HashSet<EntityUid>) null;
    if (this.Damaged == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.Damaged, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<HashSet<EntityUid>>(this.Damaged, hookCtx, context);
    target.Damaged = target6;
    EntityWhitelist target7 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target7, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target7 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target7, hookCtx, context);
    }
    target.Whitelist = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.DamageDead, ref target8, hookCtx, false, context))
      target8 = this.DamageDead;
    target.DamageDead = target8;
    ProtoId<EmotePrototype>? target9 = new ProtoId<EmotePrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>?>(this.Emote, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<ProtoId<EmotePrototype>?>(this.Emote, hookCtx, context);
    target.Emote = target9;
    ProtoId<EmotePrototype>? target10 = new ProtoId<EmotePrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>?>(this.XenoEmote, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<ProtoId<EmotePrototype>?>(this.XenoEmote, hookCtx, context);
    target.XenoEmote = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.Acidic, ref target11, hookCtx, false, context))
      target11 = this.Acidic;
    target.Acidic = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.Fire, ref target12, hookCtx, false, context))
      target12 = this.Fire;
    target.Fire = target12;
    CollisionGroup target13 = CollisionGroup.None;
    if (!serialization.TryCustomCopy<CollisionGroup>(this.Collision, ref target13, hookCtx, false, context))
      target13 = this.Collision;
    target.Collision = target13;
    TimeSpan target14 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AcidComboDuration, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<TimeSpan>(this.AcidComboDuration, hookCtx, context);
    target.AcidComboDuration = target14;
    DamageSpecifier target15 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.AcidComboDamage, ref target15, hookCtx, false, context))
    {
      if (this.AcidComboDamage == null)
        target15 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.AcidComboDamage, ref target15, hookCtx, context);
    }
    target.AcidComboDamage = target15;
    TimeSpan target16 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AcidComboParalyze, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<TimeSpan>(this.AcidComboParalyze, hookCtx, context);
    target.AcidComboParalyze = target16;
    int target17 = 0;
    if (!serialization.TryCustomCopy<int>(this.AcidComboResists, ref target17, hookCtx, false, context))
      target17 = this.AcidComboResists;
    target.AcidComboResists = target17;
    TimeSpan target18 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Paralyze, ref target18, hookCtx, false, context))
      target18 = serialization.CreateCopy<TimeSpan>(this.Paralyze, hookCtx, context);
    target.Paralyze = target18;
    bool target19 = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnoreResistances, ref target19, hookCtx, false, context))
      target19 = this.IgnoreResistances;
    target.IgnoreResistances = target19;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DamageOnCollideComponent target,
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
    DamageOnCollideComponent target1 = (DamageOnCollideComponent) target;
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
    DamageOnCollideComponent target1 = (DamageOnCollideComponent) target;
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
    DamageOnCollideComponent target1 = (DamageOnCollideComponent) target;
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
  virtual DamageOnCollideComponent Component.Instantiate() => new DamageOnCollideComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DamageOnCollideComponent_AutoState : IComponentState
  {
    public bool InitDamaged;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DamageOnCollideComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DamageOnCollideComponent, ComponentGetState>(new ComponentEventRefHandler<DamageOnCollideComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DamageOnCollideComponent, ComponentHandleState>(new ComponentEventRefHandler<DamageOnCollideComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DamageOnCollideComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DamageOnCollideComponent.DamageOnCollideComponent_AutoState()
      {
        InitDamaged = component.InitDamaged
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DamageOnCollideComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DamageOnCollideComponent.DamageOnCollideComponent_AutoState current))
        return;
      component.InitDamaged = current.InitDamaged;
    }
  }
}
