using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Dropship.Fabricator;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared._RMC14.Requisitions;
using Content.Shared._RMC14.Scaling;
using Content.Shared.GameTicking;
using Content.Shared.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Utility;

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
		((EntitySystem)this).SubscribeLocalEvent<TechAnnounceEvent>((EntityEventHandler<TechAnnounceEvent>)OnTechAnnounce, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TechUnlockTierEvent>((EntityEventHandler<TechUnlockTierEvent>)OnTechUnlockTier, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TechRequisitionsBudgetEvent>((EntityEventHandler<TechRequisitionsBudgetEvent>)OnTechRequisitionsBudget, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TechDropshipBudgetEvent>((EntityEventHandler<TechDropshipBudgetEvent>)OnTechDropshipBudget, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TechLogisticsDeliveryEvent>((EntityEventHandler<TechLogisticsDeliveryEvent>)OnTechLogisticsDelivery, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TechControlConsoleComponent, BeforeActivatableUIOpenEvent>((EntityEventRefHandler<TechControlConsoleComponent, BeforeActivatableUIOpenEvent>)OnControlConsoleBeforeOpen, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<TechControlConsoleComponent>(((EntitySystem)this).Subs, (object)TechControlConsoleUI.Key, (BuiEventSubscriber<TechControlConsoleComponent>)delegate(Subscriber<TechControlConsoleComponent> subs)
		{
			subs.Event<TechPurchaseOptionBuiMsg>((EntityEventRefHandler<TechControlConsoleComponent, TechPurchaseOptionBuiMsg>)OnPurchaseOptionMsg);
		});
	}

	private void OnTechAnnounce(TechAnnounceEvent ev)
	{
		string msg = base.Loc.GetString("rmc-announcement-message-raw", (ValueTuple<string, object>)("author", ev.Author), (ValueTuple<string, object>)("message", ev.Message));
		_marineAnnounce.AnnounceToMarines(msg, ev.Sound);
	}

	private void OnTechUnlockTier(TechUnlockTierEvent ev)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Entity<IntelTechTreeComponent> tree = _intel.EnsureTechTree();
		tree.Comp.Tree.Tier = ev.Tier;
		((EntitySystem)this).Dirty<IntelTechTreeComponent>(tree, (MetaDataComponent)null);
	}

	private void OnTechRequisitionsBudget(TechRequisitionsBudgetEvent ev)
	{
		int scaling = _scaling.GetAliveHumanoids() / 50;
		scaling = Math.Max(1, scaling);
		_requisitions.ChangeBudget(ev.Amount * scaling);
	}

	private void OnTechDropshipBudget(TechDropshipBudgetEvent ev)
	{
		_dropshipFabricator.ChangeBudget(ev.Amount);
	}

	private void OnTechLogisticsDelivery(TechLogisticsDeliveryEvent ev)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_requisitions.CreateSpecialDelivery(ev.Object);
	}

	private void OnControlConsoleBeforeOpen(Entity<TechControlConsoleComponent> ent, ref BeforeActivatableUIOpenEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			ent.Comp.Tree = _intel.EnsureTechTree().Comp.Tree;
			((EntitySystem)this).Dirty<TechControlConsoleComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnPurchaseOptionMsg(Entity<TechControlConsoleComponent> ent, ref TechPurchaseOptionBuiMsg args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		Entity<IntelTechTreeComponent> tree = _intel.EnsureTechTree();
		List<TechOption> tier = default(List<TechOption>);
		TechOption option = default(TechOption);
		if (tree.Comp.Tree.Tier < args.Tier || !Extensions.TryGetValue<List<TechOption>>((IList<List<TechOption>>)tree.Comp.Tree.Options, args.Tier, ref tier))
		{
			((EntitySystem)this).Log.Warning($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor))} tried to buy tech option with invalid tier {args.Tier}");
		}
		else if (args.Index < 0 || !Extensions.TryGetValue<TechOption>((IList<TechOption>)tier, args.Index, ref option))
		{
			((EntitySystem)this).Log.Warning($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor))} tried to buy tech option with invalid index {args.Index}");
		}
		else
		{
			if (option.TimeLock > _ticker.RoundDuration() || (option.Purchased && !option.Repurchasable) || !_intel.TryUsePoints(option.CurrentCost))
			{
				return;
			}
			tier[args.Index] = option with
			{
				CurrentCost = option.CurrentCost + option.Increase,
				Purchased = true
			};
			((EntitySystem)this).Dirty<TechControlConsoleComponent>(ent, (MetaDataComponent)null);
			foreach (object ev in option.Events)
			{
				((EntitySystem)this).RaiseLocalEvent(ev);
			}
			_intel.UpdateTree(tree);
		}
	}
}
