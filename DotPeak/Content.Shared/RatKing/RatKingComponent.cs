// Decompiled with JetBrains decompiler
// Type: Content.Shared.RatKing.RatKingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.RatKing;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedRatKingSystem)})]
[AutoGenerateComponentState(false, false)]
public sealed class RatKingComponent : 
  Component,
  ISerializationGenerated<RatKingComponent>,
  ISerializationGenerated
{
  [DataField("actionRaiseArmy", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string ActionRaiseArmy = "ActionRatKingRaiseArmy";
  [DataField("actionRaiseArmyEntity", false, 1, false, false, null)]
  public EntityUid? ActionRaiseArmyEntity;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("hungerPerArmyUse", false, 1, true, false, null)]
  public float HungerPerArmyUse = 25f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("armyMobSpawnId", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string ArmyMobSpawnId = "MobRatServant";
  [DataField("actionDomain", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string ActionDomain = "ActionRatKingDomain";
  [DataField("actionDomainEntity", false, 1, false, false, null)]
  public EntityUid? ActionDomainEntity;
  [DataField("hungerPerDomainUse", false, 1, true, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float HungerPerDomainUse = 50f;
  [DataField("molesAmmoniaPerDomain", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float MolesAmmoniaPerDomain = 200f;
  [DataField("currentOrders", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public RatKingOrderType CurrentOrder = RatKingOrderType.Loose;
  [DataField("servants", false, 1, false, false, null)]
  public HashSet<EntityUid> Servants = new HashSet<EntityUid>();
  [DataField("actionOrderStay", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string ActionOrderStay = "ActionRatKingOrderStay";
  [DataField("actionOrderStayEntity", false, 1, false, false, null)]
  public EntityUid? ActionOrderStayEntity;
  [DataField("actionOrderFollow", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string ActionOrderFollow = "ActionRatKingOrderFollow";
  [DataField("actionOrderFollowEntity", false, 1, false, false, null)]
  public EntityUid? ActionOrderFollowEntity;
  [DataField("actionOrderCheeseEm", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string ActionOrderCheeseEm = "ActionRatKingOrderCheeseEm";
  [DataField("actionOrderCheeseEmEntity", false, 1, false, false, null)]
  public EntityUid? ActionOrderCheeseEmEntity;
  [DataField("actionOrderLoose", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string ActionOrderLoose = "ActionRatKingOrderLoose";
  [DataField("actionOrderLooseEntity", false, 1, false, false, null)]
  public EntityUid? ActionOrderLooseEntity;
  [DataField("orderCallouts", false, 1, false, false, null)]
  public Dictionary<RatKingOrderType, string> OrderCallouts = new Dictionary<RatKingOrderType, string>()
  {
    {
      RatKingOrderType.Stay,
      "RatKingCommandStay"
    },
    {
      RatKingOrderType.Follow,
      "RatKingCommandFollow"
    },
    {
      RatKingOrderType.CheeseEm,
      "RatKingCommandCheeseEm"
    },
    {
      RatKingOrderType.Loose,
      "RatKingCommandLoose"
    }
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RatKingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RatKingComponent) target1;
    if (serialization.TryCustomCopy<RatKingComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.ActionRaiseArmy == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ActionRaiseArmy, ref target2, hookCtx, false, context))
      target2 = this.ActionRaiseArmy;
    target.ActionRaiseArmy = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ActionRaiseArmyEntity, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.ActionRaiseArmyEntity, hookCtx, context);
    target.ActionRaiseArmyEntity = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HungerPerArmyUse, ref target4, hookCtx, false, context))
      target4 = this.HungerPerArmyUse;
    target.HungerPerArmyUse = target4;
    string target5 = (string) null;
    if (this.ArmyMobSpawnId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ArmyMobSpawnId, ref target5, hookCtx, false, context))
      target5 = this.ArmyMobSpawnId;
    target.ArmyMobSpawnId = target5;
    string target6 = (string) null;
    if (this.ActionDomain == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ActionDomain, ref target6, hookCtx, false, context))
      target6 = this.ActionDomain;
    target.ActionDomain = target6;
    EntityUid? target7 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ActionDomainEntity, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntityUid?>(this.ActionDomainEntity, hookCtx, context);
    target.ActionDomainEntity = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HungerPerDomainUse, ref target8, hookCtx, false, context))
      target8 = this.HungerPerDomainUse;
    target.HungerPerDomainUse = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MolesAmmoniaPerDomain, ref target9, hookCtx, false, context))
      target9 = this.MolesAmmoniaPerDomain;
    target.MolesAmmoniaPerDomain = target9;
    RatKingOrderType target10 = RatKingOrderType.Stay;
    if (!serialization.TryCustomCopy<RatKingOrderType>(this.CurrentOrder, ref target10, hookCtx, false, context))
      target10 = this.CurrentOrder;
    target.CurrentOrder = target10;
    HashSet<EntityUid> target11 = (HashSet<EntityUid>) null;
    if (this.Servants == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.Servants, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<HashSet<EntityUid>>(this.Servants, hookCtx, context);
    target.Servants = target11;
    string target12 = (string) null;
    if (this.ActionOrderStay == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ActionOrderStay, ref target12, hookCtx, false, context))
      target12 = this.ActionOrderStay;
    target.ActionOrderStay = target12;
    EntityUid? target13 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ActionOrderStayEntity, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<EntityUid?>(this.ActionOrderStayEntity, hookCtx, context);
    target.ActionOrderStayEntity = target13;
    string target14 = (string) null;
    if (this.ActionOrderFollow == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ActionOrderFollow, ref target14, hookCtx, false, context))
      target14 = this.ActionOrderFollow;
    target.ActionOrderFollow = target14;
    EntityUid? target15 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ActionOrderFollowEntity, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<EntityUid?>(this.ActionOrderFollowEntity, hookCtx, context);
    target.ActionOrderFollowEntity = target15;
    string target16 = (string) null;
    if (this.ActionOrderCheeseEm == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ActionOrderCheeseEm, ref target16, hookCtx, false, context))
      target16 = this.ActionOrderCheeseEm;
    target.ActionOrderCheeseEm = target16;
    EntityUid? target17 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ActionOrderCheeseEmEntity, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<EntityUid?>(this.ActionOrderCheeseEmEntity, hookCtx, context);
    target.ActionOrderCheeseEmEntity = target17;
    string target18 = (string) null;
    if (this.ActionOrderLoose == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ActionOrderLoose, ref target18, hookCtx, false, context))
      target18 = this.ActionOrderLoose;
    target.ActionOrderLoose = target18;
    EntityUid? target19 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ActionOrderLooseEntity, ref target19, hookCtx, false, context))
      target19 = serialization.CreateCopy<EntityUid?>(this.ActionOrderLooseEntity, hookCtx, context);
    target.ActionOrderLooseEntity = target19;
    Dictionary<RatKingOrderType, string> target20 = (Dictionary<RatKingOrderType, string>) null;
    if (this.OrderCallouts == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<RatKingOrderType, string>>(this.OrderCallouts, ref target20, hookCtx, true, context))
      target20 = serialization.CreateCopy<Dictionary<RatKingOrderType, string>>(this.OrderCallouts, hookCtx, context);
    target.OrderCallouts = target20;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RatKingComponent target,
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
    RatKingComponent target1 = (RatKingComponent) target;
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
    RatKingComponent target1 = (RatKingComponent) target;
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
    RatKingComponent target1 = (RatKingComponent) target;
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
  virtual RatKingComponent Component.Instantiate() => new RatKingComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RatKingComponent_AutoState : IComponentState
  {
    public RatKingOrderType CurrentOrder;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RatKingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RatKingComponent, ComponentGetState>(new ComponentEventRefHandler<RatKingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RatKingComponent, ComponentHandleState>(new ComponentEventRefHandler<RatKingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, RatKingComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new RatKingComponent.RatKingComponent_AutoState()
      {
        CurrentOrder = component.CurrentOrder
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RatKingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RatKingComponent.RatKingComponent_AutoState current))
        return;
      component.CurrentOrder = current.CurrentOrder;
    }
  }
}
