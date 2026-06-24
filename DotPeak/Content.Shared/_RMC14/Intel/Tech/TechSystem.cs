// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Intel.Tech.TechSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Dropship.Fabricator;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared._RMC14.Requisitions;
using Content.Shared._RMC14.Scaling;
using Content.Shared.FixedPoint;
using Content.Shared.GameTicking;
using Content.Shared.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Intel.Tech;

public sealed class TechSystem : EntitySystem
{
  [Dependency]
  private DropshipFabricatorSystem _dropshipFabricator;
  [Dependency]
  private SharedGameTicker _ticker;
  [Dependency]
  private IntelSystem _intel;
  [Dependency]
  private SharedMarineAnnounceSystem _marineAnnounce;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedRequisitionsSystem _requisitions;
  [Dependency]
  private ScalingSystem _scaling;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<TechAnnounceEvent>(new EntityEventHandler<TechAnnounceEvent>(this.OnTechAnnounce));
    this.SubscribeLocalEvent<TechUnlockTierEvent>(new EntityEventHandler<TechUnlockTierEvent>(this.OnTechUnlockTier));
    this.SubscribeLocalEvent<TechRequisitionsBudgetEvent>(new EntityEventHandler<TechRequisitionsBudgetEvent>(this.OnTechRequisitionsBudget));
    this.SubscribeLocalEvent<TechDropshipBudgetEvent>(new EntityEventHandler<TechDropshipBudgetEvent>(this.OnTechDropshipBudget));
    this.SubscribeLocalEvent<TechLogisticsDeliveryEvent>(new EntityEventHandler<TechLogisticsDeliveryEvent>(this.OnTechLogisticsDelivery));
    this.SubscribeLocalEvent<TechControlConsoleComponent, BeforeActivatableUIOpenEvent>(new EntityEventRefHandler<TechControlConsoleComponent, BeforeActivatableUIOpenEvent>(this.OnControlConsoleBeforeOpen));
    this.Subs.BuiEvents<TechControlConsoleComponent>((object) TechControlConsoleUI.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<TechControlConsoleComponent>) (subs => subs.Event<TechPurchaseOptionBuiMsg>(new EntityEventRefHandler<TechControlConsoleComponent, TechPurchaseOptionBuiMsg>(this.OnPurchaseOptionMsg))));
  }

  private void OnTechAnnounce(TechAnnounceEvent ev)
  {
    this._marineAnnounce.AnnounceToMarines(this.Loc.GetString("rmc-announcement-message-raw", ("author", (object) ev.Author), ("message", (object) ev.Message)), ev.Sound);
  }

  private void OnTechUnlockTier(TechUnlockTierEvent ev)
  {
    Entity<IntelTechTreeComponent> ent = this._intel.EnsureTechTree();
    ent.Comp.Tree.Tier = ev.Tier;
    this.Dirty<IntelTechTreeComponent>(ent);
  }

  private void OnTechRequisitionsBudget(TechRequisitionsBudgetEvent ev)
  {
    int num = Math.Max(1, this._scaling.GetAliveHumanoids() / 50);
    this._requisitions.ChangeBudget(ev.Amount * num);
  }

  private void OnTechDropshipBudget(TechDropshipBudgetEvent ev)
  {
    this._dropshipFabricator.ChangeBudget(ev.Amount);
  }

  private void OnTechLogisticsDelivery(TechLogisticsDeliveryEvent ev)
  {
    this._requisitions.CreateSpecialDelivery(ev.Object);
  }

  private void OnControlConsoleBeforeOpen(
    Entity<TechControlConsoleComponent> ent,
    ref BeforeActivatableUIOpenEvent args)
  {
    if (this._net.IsClient)
      return;
    ent.Comp.Tree = this._intel.EnsureTechTree().Comp.Tree;
    this.Dirty<TechControlConsoleComponent>(ent);
  }

  private void OnPurchaseOptionMsg(
    Entity<TechControlConsoleComponent> ent,
    ref TechPurchaseOptionBuiMsg args)
  {
    if (this._net.IsClient)
      return;
    Entity<IntelTechTreeComponent> tree = this._intel.EnsureTechTree();
    List<TechOption> list;
    if (tree.Comp.Tree.Tier < args.Tier || !tree.Comp.Tree.Options.TryGetValue<List<TechOption>>(args.Tier, out list))
    {
      this.Log.Warning($"{this.ToPrettyString((Entity<MetaDataComponent>) args.Actor)} tried to buy tech option with invalid tier {args.Tier}");
    }
    else
    {
      TechOption techOption;
      if (args.Index < 0 || !list.TryGetValue<TechOption>(args.Index, out techOption))
      {
        this.Log.Warning($"{this.ToPrettyString((Entity<MetaDataComponent>) args.Actor)} tried to buy tech option with invalid index {args.Index}");
      }
      else
      {
        if (techOption.TimeLock > this._ticker.RoundDuration() || techOption.Purchased && !techOption.Repurchasable || !this._intel.TryUsePoints((FixedPoint2) techOption.CurrentCost))
          return;
        list[args.Index] = techOption with
        {
          CurrentCost = techOption.CurrentCost + techOption.Increase,
          Purchased = true
        };
        this.Dirty<TechControlConsoleComponent>(ent);
        foreach (object message in techOption.Events)
          this.RaiseLocalEvent(message);
        this._intel.UpdateTree(tree);
      }
    }
  }
}
