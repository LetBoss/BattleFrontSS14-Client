// Decompiled with JetBrains decompiler
// Type: Content.Shared.Revenant.Components.RevenantComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.FixedPoint;
using Content.Shared.Store;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Revenant.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RevenantComponent : 
  Component,
  ISerializationGenerated<RevenantComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public FixedPoint2 Essence = (FixedPoint2) 75;
  [DataField("stolenEssenceCurrencyPrototype", false, 1, false, false, typeof (PrototypeIdSerializer<CurrencyPrototype>))]
  public string StolenEssenceCurrencyPrototype = "StolenEssence";
  [DataField("spawnOnDeathPrototype", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string SpawnOnDeathPrototype = "Ectoplasm";
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("maxEssence", false, 1, false, false, null)]
  public FixedPoint2 EssenceRegenCap = (FixedPoint2) 75;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("damageToEssenceCoefficient", false, 1, false, false, null)]
  public float DamageToEssenceCoefficient = 0.75f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("essencePerSecond", false, 1, false, false, null)]
  public FixedPoint2 EssencePerSecond = (FixedPoint2) 0.5f;
  [Robust.Shared.ViewVariables.ViewVariables]
  public float Accumulator;
  [DataField("soulSearchDuration", false, 1, false, false, null)]
  public float SoulSearchDuration = 2.5f;
  [DataField("harvestDebuffs", false, 1, false, false, null)]
  public Vector2 HarvestDebuffs = new Vector2(5f, 5f);
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("maxEssenceUpgradeAmount", false, 1, false, false, null)]
  public float MaxEssenceUpgradeAmount = 10f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("defileCost", false, 1, false, false, null)]
  public FixedPoint2 DefileCost = (FixedPoint2) 30;
  [DataField("defileDebuffs", false, 1, false, false, null)]
  public Vector2 DefileDebuffs = new Vector2(1f, 4f);
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("defileRadius", false, 1, false, false, null)]
  public float DefileRadius = 3.5f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("defileTilePryAmount", false, 1, false, false, null)]
  public int DefileTilePryAmount = 15;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("defileEffectChance", false, 1, false, false, null)]
  public float DefileEffectChance = 0.5f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("overloadCost", false, 1, false, false, null)]
  public FixedPoint2 OverloadCost = (FixedPoint2) 40;
  [DataField("overloadDebuffs", false, 1, false, false, null)]
  public Vector2 OverloadDebuffs = new Vector2(3f, 8f);
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("overloadRadius", false, 1, false, false, null)]
  public float OverloadRadius = 5f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("overloadZapRadius", false, 1, false, false, null)]
  public float OverloadZapRadius = 2f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("blightCost", false, 1, false, false, null)]
  public float BlightCost = 50f;
  [DataField("blightDebuffs", false, 1, false, false, null)]
  public Vector2 BlightDebuffs = new Vector2(2f, 5f);
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("blightRadius", false, 1, false, false, null)]
  public float BlightRadius = 3.5f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("malfunctionCost", false, 1, false, false, null)]
  public FixedPoint2 MalfunctionCost = (FixedPoint2) 60;
  [DataField("malfunctionDebuffs", false, 1, false, false, null)]
  public Vector2 MalfunctionDebuffs = new Vector2(2f, 8f);
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("malfunctionRadius", false, 1, false, false, null)]
  public float MalfunctionRadius = 3.5f;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? MalfunctionWhitelist;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? MalfunctionBlacklist;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertPrototype> EssenceAlert = (ProtoId<AlertPrototype>) nameof (Essence);
  [DataField("state", false, 1, false, false, null)]
  public string State = "idle";
  [DataField("corporealState", false, 1, false, false, null)]
  public string CorporealState = "active";
  [DataField("stunnedState", false, 1, false, false, null)]
  public string StunnedState = "stunned";
  [DataField("harvestingState", false, 1, false, false, null)]
  public string HarvestingState = "harvesting";
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? Action;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RevenantComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RevenantComponent) target1;
    if (serialization.TryCustomCopy<RevenantComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Essence, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.Essence, hookCtx, context);
    target.Essence = target2;
    string target3 = (string) null;
    if (this.StolenEssenceCurrencyPrototype == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.StolenEssenceCurrencyPrototype, ref target3, hookCtx, false, context))
      target3 = this.StolenEssenceCurrencyPrototype;
    target.StolenEssenceCurrencyPrototype = target3;
    string target4 = (string) null;
    if (this.SpawnOnDeathPrototype == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SpawnOnDeathPrototype, ref target4, hookCtx, false, context))
      target4 = this.SpawnOnDeathPrototype;
    target.SpawnOnDeathPrototype = target4;
    FixedPoint2 target5 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.EssenceRegenCap, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<FixedPoint2>(this.EssenceRegenCap, hookCtx, context);
    target.EssenceRegenCap = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DamageToEssenceCoefficient, ref target6, hookCtx, false, context))
      target6 = this.DamageToEssenceCoefficient;
    target.DamageToEssenceCoefficient = target6;
    FixedPoint2 target7 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.EssencePerSecond, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<FixedPoint2>(this.EssencePerSecond, hookCtx, context);
    target.EssencePerSecond = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SoulSearchDuration, ref target8, hookCtx, false, context))
      target8 = this.SoulSearchDuration;
    target.SoulSearchDuration = target8;
    Vector2 target9 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.HarvestDebuffs, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<Vector2>(this.HarvestDebuffs, hookCtx, context);
    target.HarvestDebuffs = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxEssenceUpgradeAmount, ref target10, hookCtx, false, context))
      target10 = this.MaxEssenceUpgradeAmount;
    target.MaxEssenceUpgradeAmount = target10;
    FixedPoint2 target11 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.DefileCost, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<FixedPoint2>(this.DefileCost, hookCtx, context);
    target.DefileCost = target11;
    Vector2 target12 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.DefileDebuffs, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<Vector2>(this.DefileDebuffs, hookCtx, context);
    target.DefileDebuffs = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DefileRadius, ref target13, hookCtx, false, context))
      target13 = this.DefileRadius;
    target.DefileRadius = target13;
    int target14 = 0;
    if (!serialization.TryCustomCopy<int>(this.DefileTilePryAmount, ref target14, hookCtx, false, context))
      target14 = this.DefileTilePryAmount;
    target.DefileTilePryAmount = target14;
    float target15 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DefileEffectChance, ref target15, hookCtx, false, context))
      target15 = this.DefileEffectChance;
    target.DefileEffectChance = target15;
    FixedPoint2 target16 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.OverloadCost, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<FixedPoint2>(this.OverloadCost, hookCtx, context);
    target.OverloadCost = target16;
    Vector2 target17 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.OverloadDebuffs, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<Vector2>(this.OverloadDebuffs, hookCtx, context);
    target.OverloadDebuffs = target17;
    float target18 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.OverloadRadius, ref target18, hookCtx, false, context))
      target18 = this.OverloadRadius;
    target.OverloadRadius = target18;
    float target19 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.OverloadZapRadius, ref target19, hookCtx, false, context))
      target19 = this.OverloadZapRadius;
    target.OverloadZapRadius = target19;
    float target20 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BlightCost, ref target20, hookCtx, false, context))
      target20 = this.BlightCost;
    target.BlightCost = target20;
    Vector2 target21 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.BlightDebuffs, ref target21, hookCtx, false, context))
      target21 = serialization.CreateCopy<Vector2>(this.BlightDebuffs, hookCtx, context);
    target.BlightDebuffs = target21;
    float target22 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BlightRadius, ref target22, hookCtx, false, context))
      target22 = this.BlightRadius;
    target.BlightRadius = target22;
    FixedPoint2 target23 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MalfunctionCost, ref target23, hookCtx, false, context))
      target23 = serialization.CreateCopy<FixedPoint2>(this.MalfunctionCost, hookCtx, context);
    target.MalfunctionCost = target23;
    Vector2 target24 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.MalfunctionDebuffs, ref target24, hookCtx, false, context))
      target24 = serialization.CreateCopy<Vector2>(this.MalfunctionDebuffs, hookCtx, context);
    target.MalfunctionDebuffs = target24;
    float target25 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MalfunctionRadius, ref target25, hookCtx, false, context))
      target25 = this.MalfunctionRadius;
    target.MalfunctionRadius = target25;
    EntityWhitelist target26 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.MalfunctionWhitelist, ref target26, hookCtx, false, context))
    {
      if (this.MalfunctionWhitelist == null)
        target26 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.MalfunctionWhitelist, ref target26, hookCtx, context);
    }
    target.MalfunctionWhitelist = target26;
    EntityWhitelist target27 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.MalfunctionBlacklist, ref target27, hookCtx, false, context))
    {
      if (this.MalfunctionBlacklist == null)
        target27 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.MalfunctionBlacklist, ref target27, hookCtx, context);
    }
    target.MalfunctionBlacklist = target27;
    ProtoId<AlertPrototype> target28 = new ProtoId<AlertPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(this.EssenceAlert, ref target28, hookCtx, false, context))
      target28 = serialization.CreateCopy<ProtoId<AlertPrototype>>(this.EssenceAlert, hookCtx, context);
    target.EssenceAlert = target28;
    string target29 = (string) null;
    if (this.State == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.State, ref target29, hookCtx, false, context))
      target29 = this.State;
    target.State = target29;
    string target30 = (string) null;
    if (this.CorporealState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.CorporealState, ref target30, hookCtx, false, context))
      target30 = this.CorporealState;
    target.CorporealState = target30;
    string target31 = (string) null;
    if (this.StunnedState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.StunnedState, ref target31, hookCtx, false, context))
      target31 = this.StunnedState;
    target.StunnedState = target31;
    string target32 = (string) null;
    if (this.HarvestingState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.HarvestingState, ref target32, hookCtx, false, context))
      target32 = this.HarvestingState;
    target.HarvestingState = target32;
    EntityUid? target33 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Action, ref target33, hookCtx, false, context))
      target33 = serialization.CreateCopy<EntityUid?>(this.Action, hookCtx, context);
    target.Action = target33;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RevenantComponent target,
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
    RevenantComponent target1 = (RevenantComponent) target;
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
    RevenantComponent target1 = (RevenantComponent) target;
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
    RevenantComponent target1 = (RevenantComponent) target;
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
  virtual RevenantComponent Component.Instantiate() => new RevenantComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RevenantComponent_AutoState : IComponentState
  {
    public FixedPoint2 Essence;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RevenantComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RevenantComponent, ComponentGetState>(new ComponentEventRefHandler<RevenantComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RevenantComponent, ComponentHandleState>(new ComponentEventRefHandler<RevenantComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, RevenantComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new RevenantComponent.RevenantComponent_AutoState()
      {
        Essence = component.Essence
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RevenantComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RevenantComponent.RevenantComponent_AutoState current))
        return;
      component.Essence = current.Essence;
    }
  }
}
