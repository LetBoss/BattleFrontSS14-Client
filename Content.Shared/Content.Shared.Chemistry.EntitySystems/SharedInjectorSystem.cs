using System;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.CombatMode;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Chemistry.EntitySystems;

public abstract class SharedInjectorSystem : EntitySystem
{
	public static readonly FixedPoint2[] TransferAmounts = new FixedPoint2[4] { 1, 5, 10, 15 };

	[Dependency]
	protected SharedPopupSystem Popup;

	[Dependency]
	protected SharedSolutionContainerSystem SolutionContainers;

	[Dependency]
	protected MobStateSystem MobState;

	[Dependency]
	protected SharedCombatModeSystem Combat;

	[Dependency]
	protected SharedDoAfterSystem DoAfter;

	[Dependency]
	protected ISharedAdminLogManager AdminLogger;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<InjectorComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<InjectorComponent, GetVerbsEvent<AlternativeVerb>>)AddSetTransferVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InjectorComponent, ComponentStartup>((EntityEventRefHandler<InjectorComponent, ComponentStartup>)OnInjectorStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InjectorComponent, UseInHandEvent>((EntityEventRefHandler<InjectorComponent, UseInHandEvent>)OnInjectorUse, (Type[])null, (Type[])null);
	}

	private void AddSetTransferVerbs(Entity<InjectorComponent> entity, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanAccess || !args.CanInteract || args.Hands == null)
		{
			return;
		}
		EntityUid user = args.User;
		Entity<InjectorComponent> val = entity;
		EntityUid val2 = default(EntityUid);
		InjectorComponent injectorComponent = default(InjectorComponent);
		val.Deconstruct(ref val2, ref injectorComponent);
		InjectorComponent component = injectorComponent;
		FixedPoint2 min = component.MinimumTransferAmount;
		FixedPoint2 max = component.MaximumTransferAmount;
		FixedPoint2 cur = component.TransferAmount;
		FixedPoint2 toggleAmount = ((cur == max) ? min : max);
		int priority = 0;
		AlternativeVerb toggleVerb = new AlternativeVerb
		{
			Text = base.Loc.GetString("comp-solution-transfer-verb-toggle", (ValueTuple<string, object>)("amount", toggleAmount)),
			Category = VerbCategory.SetTransferAmount,
			Act = delegate
			{
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				component.TransferAmount = toggleAmount;
				Popup.PopupClient(base.Loc.GetString("comp-solution-transfer-set-amount", (ValueTuple<string, object>)("amount", toggleAmount)), user, user);
				((EntitySystem)this).Dirty<InjectorComponent>(entity, (MetaDataComponent)null);
			},
			Priority = priority
		};
		args.Verbs.Add(toggleVerb);
		priority--;
		FixedPoint2[] transferAmounts = TransferAmounts;
		foreach (FixedPoint2 amount in transferAmounts)
		{
			if (!(amount < component.MinimumTransferAmount) && !(amount > component.MaximumTransferAmount))
			{
				AlternativeVerb verb = new AlternativeVerb
				{
					Text = base.Loc.GetString("comp-solution-transfer-verb-amount", (ValueTuple<string, object>)("amount", amount)),
					Category = VerbCategory.SetTransferAmount,
					Act = delegate
					{
						//IL_005b: Unknown result type (might be due to invalid IL or missing references)
						//IL_0066: Unknown result type (might be due to invalid IL or missing references)
						//IL_0087: Unknown result type (might be due to invalid IL or missing references)
						component.TransferAmount = amount;
						Popup.PopupClient(base.Loc.GetString("comp-solution-transfer-set-amount", (ValueTuple<string, object>)("amount", amount)), user, user);
						((EntitySystem)this).Dirty<InjectorComponent>(entity, (MetaDataComponent)null);
					},
					Priority = priority
				};
				priority--;
				args.Verbs.Add(verb);
			}
		}
	}

	private void OnInjectorStartup(Entity<InjectorComponent> entity, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Dirty<InjectorComponent>(entity, (MetaDataComponent)null);
	}

	private void OnInjectorUse(Entity<InjectorComponent> entity, ref UseInHandEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			Toggle(entity, args.User);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void Toggle(Entity<InjectorComponent> injector, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (injector.Comp.InjectOnly || !SolutionContainers.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(injector.Owner), injector.Comp.SolutionName, out Entity<SolutionComponent>? _, out Solution solution))
		{
			return;
		}
		string msg;
		switch (injector.Comp.ToggleState)
		{
		case InjectorToggleMode.Inject:
			if (solution.AvailableVolume > 0)
			{
				SetMode(injector, InjectorToggleMode.Draw);
				msg = "injector-component-drawing-text";
			}
			else
			{
				msg = "injector-component-cannot-toggle-draw-message";
			}
			break;
		case InjectorToggleMode.Draw:
			if (solution.Volume > 0)
			{
				SetMode(injector, InjectorToggleMode.Inject);
				msg = "injector-component-injecting-text";
			}
			else
			{
				msg = "injector-component-cannot-toggle-inject-message";
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		Popup.PopupClient(base.Loc.GetString(msg), Entity<InjectorComponent>.op_Implicit(injector), user);
	}

	public void SetMode(Entity<InjectorComponent> injector, InjectorToggleMode mode)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		injector.Comp.ToggleState = mode;
		((EntitySystem)this).Dirty<InjectorComponent>(injector, (MetaDataComponent)null);
	}
}
