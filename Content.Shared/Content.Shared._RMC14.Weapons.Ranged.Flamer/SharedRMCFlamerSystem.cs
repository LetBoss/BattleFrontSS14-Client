using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared._RMC14.Fluids;
using Content.Shared._RMC14.Line;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.OnCollide;
using Content.Shared._RMC14.Weapons.Common;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Content.Shared.Temperature;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Weapons.Ranged.Flamer;

public abstract class SharedRMCFlamerSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _action;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private ExamineSystemShared _examine;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private LineSystem _line;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedRMCFlammableSystem _rmcFlammable;

	[Dependency]
	private SharedRMCSpraySystem _rmcSpray;

	[Dependency]
	private SharedSolutionContainerSystem _solution;

	[Dependency]
	private SolutionTransferSystem _solutionTransfer;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private RMCMapSystem _rmcMap;

	[Dependency]
	private RMCReagentSystem _reagent;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCFlamerAmmoProviderComponent, MapInitEvent>((EntityEventRefHandler<RMCFlamerAmmoProviderComponent, MapInitEvent>)OnMapInit, (Type[])null, new Type[1] { typeof(SharedSolutionContainerSystem) });
		((EntitySystem)this).SubscribeLocalEvent<RMCFlamerAmmoProviderComponent, TakeAmmoEvent>((EntityEventRefHandler<RMCFlamerAmmoProviderComponent, TakeAmmoEvent>)OnTakeAmmo, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFlamerAmmoProviderComponent, GetAmmoCountEvent>((EntityEventRefHandler<RMCFlamerAmmoProviderComponent, GetAmmoCountEvent>)OnGetAmmoCount, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFlamerAmmoProviderComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<RMCFlamerAmmoProviderComponent, EntInsertedIntoContainerMessage>)OnInsertedIntoContainer, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFlamerAmmoProviderComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<RMCFlamerAmmoProviderComponent, EntRemovedFromContainerMessage>)OnRemovedFromContainer, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFlamerAmmoProviderComponent, AttemptShootEvent>((EntityEventRefHandler<RMCFlamerAmmoProviderComponent, AttemptShootEvent>)OnAttemptShoot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFlamerTankComponent, BeforeRangedInteractEvent>((EntityEventRefHandler<RMCFlamerTankComponent, BeforeRangedInteractEvent>)OnFlamerTankBeforeRangedInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFlamerTankComponent, GetVerbsEvent<ExamineVerb>>((EntityEventRefHandler<RMCFlamerTankComponent, GetVerbsEvent<ExamineVerb>>)OnFlamerTankVerbExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSprayAmmoProviderComponent, TakeAmmoEvent>((EntityEventRefHandler<RMCSprayAmmoProviderComponent, TakeAmmoEvent>)OnSprayTakeAmmo, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSprayAmmoProviderComponent, GetAmmoCountEvent>((EntityEventRefHandler<RMCSprayAmmoProviderComponent, GetAmmoCountEvent>)OnSprayGetAmmoCount, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCIgniterComponent, MapInitEvent>((EntityEventRefHandler<RMCIgniterComponent, MapInitEvent>)OnIgniterMapInit, (Type[])null, new Type[1] { typeof(SharedSolutionContainerSystem) });
		((EntitySystem)this).SubscribeLocalEvent<RMCIgniterComponent, UniqueActionEvent>((EntityEventRefHandler<RMCIgniterComponent, UniqueActionEvent>)OnIgniterUniqueAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCIgniterComponent, IsHotEvent>((EntityEventRefHandler<RMCIgniterComponent, IsHotEvent>)OnIgniterToggle, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCIgniterComponent, AttemptShootEvent>((EntityEventRefHandler<RMCIgniterComponent, AttemptShootEvent>)OnIgniterAttemptShoot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCIgniterComponent, ExaminedEvent>((EntityEventRefHandler<RMCIgniterComponent, ExaminedEvent>)OnIgniterUniqueActionExamine, new Type[1] { typeof(SharedGunSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCBroilerComponent, GetItemActionsEvent>((EntityEventRefHandler<RMCBroilerComponent, GetItemActionsEvent>)OnBroilerGetItemActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCBroilerComponent, RMCBroilerActionEvent>((EntityEventRefHandler<RMCBroilerComponent, RMCBroilerActionEvent>)OnBroilerAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCCanUseBroilerComponent, UniqueActionEvent>((EntityEventRefHandler<RMCCanUseBroilerComponent, UniqueActionEvent>)OnBroilerUniqueAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCCanUseBroilerComponent, ExaminedEvent>((EntityEventRefHandler<RMCCanUseBroilerComponent, ExaminedEvent>)OnBroilerUniqueActionExamine, new Type[1] { typeof(SharedGunSystem) }, (Type[])null);
	}

	private void OnMapInit(Entity<RMCFlamerAmmoProviderComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(ent);
	}

	private void OnTakeAmmo(Entity<RMCFlamerAmmoProviderComponent> ent, ref TakeAmmoEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		args.Ammo.Add((Entity<RMCFlamerAmmoProviderComponent>.op_Implicit(ent), ent.Comp));
	}

	private void OnGetAmmoCount(Entity<RMCFlamerAmmoProviderComponent> ent, ref GetAmmoCountEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetTankSolution(ent, out Entity<SolutionComponent>? solutionEnt, out Entity<RMCFlamerTankComponent>? _))
		{
			Solution solution = solutionEnt.Value.Comp.Solution;
			args.Count = solution.Volume.Int();
			args.Capacity = solution.MaxVolume.Int();
		}
	}

	private void OnInsertedIntoContainer(Entity<RMCFlamerAmmoProviderComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ContainerModifiedMessage)args).Container.ID != ent.Comp.ContainerId))
		{
			UpdateAppearance(ent);
		}
	}

	private void OnRemovedFromContainer(Entity<RMCFlamerAmmoProviderComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ContainerModifiedMessage)args).Container.ID != ent.Comp.ContainerId))
		{
			UpdateAppearance(ent);
		}
	}

	private void OnAttemptShoot(Entity<RMCFlamerAmmoProviderComponent> ent, ref AttemptShootEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates? toCoordinates = args.ToCoordinates;
		if (!toCoordinates.HasValue)
		{
			return;
		}
		EntityCoordinates toCoordinates2 = toCoordinates.GetValueOrDefault();
		if (!CanShootFlamer(ent, args.FromCoordinates, toCoordinates2, out List<LineTile> _, out Entity<SolutionComponent> _, out ReagentPrototype _, out Entity<RMCFlamerTankComponent>? _))
		{
			args.Cancelled = true;
			args.ResetCooldown = true;
			TimeSpan time = _timing.CurTime;
			if (!(time < ent.Comp.CantShootPopupLast + ent.Comp.CantShootPopupCooldown))
			{
				ent.Comp.CantShootPopupLast = time;
				((EntitySystem)this).Dirty<RMCFlamerAmmoProviderComponent>(ent, (MetaDataComponent)null);
				args.Message = base.Loc.GetString("rmc-flamer-too-close");
			}
		}
	}

	private void OnFlamerTankBeforeRangedInteract(Entity<RMCFlamerTankComponent> tank, ref BeforeRangedInteractEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<RMCFlamerAmmoProviderComponent>(Entity<RMCFlamerTankComponent>.op_Implicit(tank)))
		{
			RefillTank(tank, ref args);
			return;
		}
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		if (!_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(tank.Owner), tank.Comp.SolutionId, out Entity<SolutionComponent>? tankSolutionEnt, out Solution solution))
		{
			return;
		}
		Entity<SolutionComponent> targetSolutionEnt;
		RMCFlamerTankComponent targetTank = default(RMCFlamerTankComponent);
		Entity<SolutionComponent>? targetTankSolution;
		RMCFlamerBackpackComponent backpack = default(RMCFlamerBackpackComponent);
		Entity<SolutionComponent>? backpackSolution;
		if (_solution.TryGetDrainableSolution(Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(target2), out Entity<SolutionComponent>? drainable, out solution))
		{
			targetSolutionEnt = drainable.Value;
		}
		else if (((EntitySystem)this).TryComp<RMCFlamerTankComponent>(target2, ref targetTank) && _solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(target2), targetTank.SolutionId, out targetTankSolution))
		{
			targetSolutionEnt = targetTankSolution.Value;
		}
		else if (((EntitySystem)this).TryComp<RMCFlamerBackpackComponent>(target2, ref backpack) && _solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(target2), backpack.SolutionId, out backpackSolution))
		{
			targetSolutionEnt = backpackSolution.Value;
		}
		else
		{
			if (!((EntitySystem)this).HasComp<ReagentTankComponent>(target2) || !_solution.TryGetDrainableSolution(Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(target2), out Entity<SolutionComponent>? reagentTankSolutionEnt, out solution))
			{
				return;
			}
			targetSolutionEnt = reagentTankSolutionEnt.Value;
		}
		((HandledEntityEventArgs)args).Handled = true;
		Transfer(target2, targetSolutionEnt, tank, tankSolutionEnt.Value, args.User);
	}

	private void OnFlamerTankVerbExamine(Entity<RMCFlamerTankComponent> tank, ref GetVerbsEvent<ExamineVerb> args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		if (!args.CanInteract || !args.CanAccess || ((EntitySystem)this).HasComp<XenoComponent>(user))
		{
			return;
		}
		FormattedMessage msg = new FormattedMessage();
		List<int> values = new List<int>(new _003C_003Ez__ReadOnlyArray<int>(new int[3]
		{
			tank.Comp.MaxIntensity,
			tank.Comp.MaxDuration,
			tank.Comp.MaxRange
		}));
		for (int i = 0; i < values.Count; i++)
		{
			msg.AddMarkupPermissive(base.Loc.GetString("rmc-flamer-tank-examine-line-" + i, (ValueTuple<string, object>)("value", values[i])));
			if (i + 1 != values.Count)
			{
				msg.PushNewline();
			}
		}
		_examine.AddDetailedExamineVerb(args, (Component)(object)Entity<RMCFlamerTankComponent>.op_Implicit(tank), msg, base.Loc.GetString("rmc-flamer-tank-examine-short"), tank.Comp.ExamineIcon, base.Loc.GetString("rmc-flamer-tank-examine"));
	}

	private void OnSprayTakeAmmo(Entity<RMCSprayAmmoProviderComponent> ent, ref TakeAmmoEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		args.Ammo.Add((Entity<RMCSprayAmmoProviderComponent>.op_Implicit(ent), ent.Comp));
	}

	private void OnSprayGetAmmoCount(Entity<RMCSprayAmmoProviderComponent> ent, ref GetAmmoCountEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.SolutionId, out Entity<SolutionComponent>? solutionEnt, out Solution _))
		{
			Solution solution2 = solutionEnt.Value.Comp.Solution;
			args.Count = solution2.Volume.Int();
			args.Capacity = solution2.MaxVolume.Int();
		}
	}

	private void OnIgniterMapInit(Entity<RMCIgniterComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(Entity<RMCIgniterComponent>.op_Implicit(ent), (Enum)RMCIgniterVisuals.Ignited, (object)ent.Comp.Enabled, (AppearanceComponent)null);
	}

	private void OnIgniterUniqueAction(Entity<RMCIgniterComponent> ent, ref UniqueActionEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && !ent.Comp.Locked)
		{
			((HandledEntityEventArgs)args).Handled = true;
			ent.Comp.Enabled = !ent.Comp.Enabled;
			((EntitySystem)this).Dirty<RMCIgniterComponent>(ent, (MetaDataComponent)null);
			_audio.PlayPredicted((SoundSpecifier)(object)ent.Comp.Sound, Entity<RMCIgniterComponent>.op_Implicit(ent), (EntityUid?)args.UserUid, (AudioParams?)null);
			_appearance.SetData(Entity<RMCIgniterComponent>.op_Implicit(ent), (Enum)RMCIgniterVisuals.Ignited, (object)ent.Comp.Enabled, (AppearanceComponent)null);
		}
	}

	private void OnIgniterToggle(Entity<RMCIgniterComponent> ent, ref IsHotEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		args.IsHot = ent.Comp.Enabled;
	}

	protected virtual void OnIgniterAttemptShoot(Entity<RMCIgniterComponent> ent, ref AttemptShootEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !ent.Comp.Enabled)
		{
			args.Cancelled = true;
		}
	}

	private void OnIgniterUniqueActionExamine(Entity<RMCIgniterComponent> ent, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.Locked)
		{
			args.PushMarkup(base.Loc.GetString(LocId.op_Implicit(ent.Comp.ExamineText)), 1);
		}
	}

	private void UpdateAppearance(Entity<RMCFlamerAmmoProviderComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent appearance = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(Entity<RMCFlamerAmmoProviderComponent>.op_Implicit(ent), ref appearance))
		{
			FixedPoint2 volume = FixedPoint2.Zero;
			FixedPoint2 maxVolume = FixedPoint2.Zero;
			bool tank = false;
			if (TryGetTankSolution(ent, out Entity<SolutionComponent>? solutionEnt, out Entity<RMCFlamerTankComponent>? _, display: true))
			{
				Solution solution = solutionEnt.Value.Comp.Solution;
				volume = solution.Volume;
				maxVolume = solution.MaxVolume;
				tank = true;
			}
			_appearance.SetData(Entity<RMCFlamerAmmoProviderComponent>.op_Implicit(ent), (Enum)AmmoVisuals.HasAmmo, (object)(volume > FixedPoint2.Zero), appearance);
			_appearance.SetData(Entity<RMCFlamerAmmoProviderComponent>.op_Implicit(ent), (Enum)AmmoVisuals.AmmoCount, (object)volume.Int(), appearance);
			_appearance.SetData(Entity<RMCFlamerAmmoProviderComponent>.op_Implicit(ent), (Enum)AmmoVisuals.AmmoMax, (object)maxVolume.Int(), appearance);
			_appearance.SetData(Entity<RMCFlamerAmmoProviderComponent>.op_Implicit(ent), (Enum)AmmoVisuals.MagLoaded, (object)tank, appearance);
			_appearance.SetData(Entity<RMCFlamerAmmoProviderComponent>.op_Implicit(ent), (Enum)RMCFlamerVisualLayers.Strip, (object)tank, appearance);
		}
	}

	public void ShootFlamer(Entity<RMCFlamerAmmoProviderComponent> flamer, Entity<GunComponent> gun, EntityUid? user, EntityCoordinates fromCoordinates, EntityCoordinates toCoordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		if (CanShootFlamer(flamer, fromCoordinates, toCoordinates, out List<LineTile> tiles, out Entity<SolutionComponent> solution, out ReagentPrototype reagent, out Entity<RMCFlamerTankComponent>? tank))
		{
			_audio.PlayPredicted(gun.Comp.SoundGunshotModified, Entity<GunComponent>.op_Implicit(gun), user, (AudioParams?)null);
			int cost = tiles.Count;
			if (reagent.FireSpread && cost > 2)
			{
				cost = (int)Math.Ceiling((float)cost / 3f);
			}
			solution.Comp.Solution.RemoveSolution(flamer.Comp.CostPer * cost);
			_solution.UpdateChemicals(solution);
			if (!_net.IsClient)
			{
				EntityUid chain = ((EntitySystem)this).Spawn((string)null, (ComponentRegistry)null, true);
				RMCFlamerChainComponent chainComp = ((EntitySystem)this).EnsureComp<RMCFlamerChainComponent>(chain);
				chainComp.Spawn = reagent.FireEntity;
				chainComp.Tiles = tiles;
				chainComp.Reagent = ProtoId<ReagentPrototype>.op_Implicit(reagent.ID);
				chainComp.MaxIntensity = tank.Value.Comp.MaxIntensity;
				chainComp.MaxDuration = tank.Value.Comp.MaxDuration;
				((EntitySystem)this).Dirty(chain, (IComponent)(object)chainComp, (MetaDataComponent)null);
			}
		}
	}

	private bool CanShootFlamer(Entity<RMCFlamerAmmoProviderComponent> flamer, EntityCoordinates fromCoordinates, EntityCoordinates toCoordinates, [NotNullWhen(true)] out List<LineTile>? tiles, out Entity<SolutionComponent> solution, [NotNullWhen(true)] out ReagentPrototype? reagent, [NotNullWhen(true)] out Entity<RMCFlamerTankComponent>? tank)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		tiles = null;
		solution = default(Entity<SolutionComponent>);
		reagent = null;
		if (!TryGetTankSolution(flamer, out Entity<SolutionComponent>? solutionEnt, out tank))
		{
			return false;
		}
		FixedPoint2 volume = solutionEnt.Value.Comp.Solution.Volume;
		if (volume <= flamer.Comp.CostPer)
		{
			return false;
		}
		Vector2 delta = default(Vector2);
		if (!((EntityCoordinates)(ref fromCoordinates)).TryDelta((IEntityManager)(object)base.EntityManager, _transform, toCoordinates, ref delta))
		{
			return false;
		}
		if (Vector2Helpers.IsLengthZero(delta))
		{
			return false;
		}
		Vector2 normalized = -Vector2Helpers.Normalized(delta);
		fromCoordinates = ((EntityCoordinates)(ref fromCoordinates)).Offset(normalized * 0.23f);
		ReagentQuantity? firstReagent = default(ReagentQuantity?);
		if (!Extensions.TryFirstOrNull<ReagentQuantity>((IEnumerable<ReagentQuantity>)solutionEnt.Value.Comp.Solution, ref firstReagent))
		{
			return false;
		}
		reagent = _reagent.Index(ProtoId<ReagentPrototype>.op_Implicit(firstReagent.Value.Reagent.Prototype));
		int maxRange = Math.Min(tank.Value.Comp.MaxRange, reagent.Radius);
		int range = Math.Min((volume / flamer.Comp.CostPer).Int(), maxRange);
		if (delta.Length() > (float)maxRange)
		{
			toCoordinates = ((EntityCoordinates)(ref fromCoordinates)).Offset(normalized * range);
		}
		tiles = _line.DrawLine(fromCoordinates, toCoordinates, flamer.Comp.DelayPer, maxRange, out var _, hitBlocker: true, reagent.FireSpread);
		if (tiles.Count == 0)
		{
			tiles = null;
			return false;
		}
		solution = solutionEnt.Value;
		return true;
	}

	public void ShootSpray(Entity<RMCSprayAmmoProviderComponent> spray, Entity<GunComponent> gun, EntityUid? user, EntityCoordinates fromCoordinates, EntityCoordinates toCoordinates)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (user.HasValue)
		{
			_rmcSpray.Spray(Entity<RMCSprayAmmoProviderComponent>.op_Implicit(spray), user.Value, _transform.ToMapCoordinates(toCoordinates, true));
		}
	}

	private bool TryGetTankSolution(Entity<RMCFlamerAmmoProviderComponent> flamer, [NotNullWhen(true)] out Entity<SolutionComponent>? solutionEnt, [NotNullWhen(true)] out Entity<RMCFlamerTankComponent>? tankEnt, bool display = false)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		solutionEnt = null;
		tankEnt = null;
		RMCFlamerTankComponent tankComp = default(RMCFlamerTankComponent);
		BaseContainer container = default(BaseContainer);
		EntityUid? tankId = default(EntityUid?);
		if (((EntitySystem)this).TryComp<RMCFlamerTankComponent>(Entity<RMCFlamerAmmoProviderComponent>.op_Implicit(flamer), ref tankComp))
		{
			tankEnt = Entity<RMCFlamerTankComponent>.op_Implicit((Entity<RMCFlamerAmmoProviderComponent>.op_Implicit(flamer), tankComp));
		}
		else if (_container.TryGetContainer(Entity<RMCFlamerAmmoProviderComponent>.op_Implicit(flamer), flamer.Comp.ContainerId, ref container, (ContainerManagerComponent)null) && Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>)container.ContainedEntities, ref tankId) && ((EntitySystem)this).TryComp<RMCFlamerTankComponent>(tankId, ref tankComp))
		{
			tankEnt = Entity<RMCFlamerTankComponent>.op_Implicit((tankId.Value, tankComp));
		}
		else if (!display && ((EntitySystem)this).HasComp<RMCCanUseBroilerComponent>(Entity<RMCFlamerAmmoProviderComponent>.op_Implicit(flamer)))
		{
			BaseContainer holder = default(BaseContainer);
			if (!_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(flamer.Owner, null)), ref holder))
			{
				return false;
			}
			InventorySystem.InventorySlotEnumerator inventoryEnumerator = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(holder.Owner));
			ContainerSlot slot;
			RMCBroilerComponent broiler = default(RMCBroilerComponent);
			BaseContainer activeTankContainer = default(BaseContainer);
			while (inventoryEnumerator.MoveNext(out slot))
			{
				if (!((EntitySystem)this).TryComp<RMCBroilerComponent>(slot.ContainedEntity, ref broiler))
				{
					continue;
				}
				Entity<RMCBroilerComponent> broilerEnt = Entity<RMCBroilerComponent>.op_Implicit((slot.ContainedEntity.Value, broiler));
				List<string> containers = BroilerListTanks(broilerEnt);
				if (containers.Count > broiler.ActiveTank)
				{
					string activeTankContainerName = containers[broiler.ActiveTank];
					if (_container.TryGetContainer(Entity<RMCBroilerComponent>.op_Implicit(broilerEnt), activeTankContainerName, ref activeTankContainer, (ContainerManagerComponent)null) && Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>)activeTankContainer.ContainedEntities, ref tankId) && ((EntitySystem)this).TryComp<RMCFlamerTankComponent>(tankId, ref tankComp))
					{
						tankEnt = Entity<RMCFlamerTankComponent>.op_Implicit((tankId.Value, tankComp));
						break;
					}
				}
			}
		}
		Entity<RMCFlamerTankComponent>? val = tankEnt;
		if (val.HasValue)
		{
			Entity<RMCFlamerTankComponent> tankValue = val.GetValueOrDefault();
			Solution solution;
			return _solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(tankValue.Owner), tankValue.Comp.SolutionId, out solutionEnt, out solution);
		}
		return false;
	}

	private void Transfer(EntityUid source, Entity<SolutionComponent> sourceSolutionEnt, Entity<RMCFlamerTankComponent> target, Entity<SolutionComponent> targetSolutionEnt, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		Solution tankSolution = targetSolutionEnt.Comp.Solution;
		foreach (ReagentQuantity content in sourceSolutionEnt.Comp.Solution.Contents)
		{
			List<ProtoId<ReagentPrototype>> whitelist = target.Comp.ReagentWhitelist;
			if (whitelist != null && !whitelist.Contains(ProtoId<ReagentPrototype>.op_Implicit(content.Reagent.Prototype)))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-flamer-tank-not-whitelisted", (ValueTuple<string, object>)("tank", target)), source, user);
				return;
			}
			if (_reagent.TryIndex(ProtoId<ReagentPrototype>.op_Implicit(content.Reagent.Prototype), out Reagent reagent) && (reagent.Intensity <= 0 || reagent.Duration <= 0 || reagent.Radius <= 0))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-flamer-tank-not-potent-enough"), source, user);
				return;
			}
		}
		if (_solutionTransfer.Transfer(user, source, sourceSolutionEnt, Entity<RMCFlamerTankComponent>.op_Implicit(target), targetSolutionEnt, tankSolution.AvailableVolume) > FixedPoint2.Zero)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-flamer-refill", (ValueTuple<string, object>)("refilled", target)), source, user);
		}
	}

	private void RefillTank(Entity<RMCFlamerTankComponent> tank, ref BeforeRangedInteractEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = args.Target;
		if (target.HasValue)
		{
			EntityUid target2 = target.GetValueOrDefault();
			if (_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(tank.Owner), tank.Comp.SolutionId, out Entity<SolutionComponent>? tankSolutionEnt, out Solution solution) && ((EntitySystem)this).HasComp<ReagentTankComponent>(target2) && _solution.TryGetDrainableSolution(Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(target2), out Entity<SolutionComponent>? reagentTankSolutionEnt, out solution))
			{
				Entity<SolutionComponent> targetSolutionEnt = reagentTankSolutionEnt.Value;
				((HandledEntityEventArgs)args).Handled = true;
				Transfer(target2, targetSolutionEnt, tank, tankSolutionEnt.Value, args.User);
			}
		}
	}

	private void OnBroilerGetItemActions(Entity<RMCBroilerComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		if (args.SlotFlags.HasValue && (args.SlotFlags & ent.Comp.Slot) != 0)
		{
			args.AddAction(ref ent.Comp.Action, EntProtoId.op_Implicit(ent.Comp.ActionId), Entity<RMCBroilerComponent>.op_Implicit(ent));
			EntityUid? action = ent.Comp.Action;
			if (action.HasValue)
			{
				EntityUid action2 = action.GetValueOrDefault();
				int n = ent.Comp.ActiveTank + 1;
				_action.SetIcon(Entity<ActionComponent>.op_Implicit(action2), (SpriteSpecifier?)new Rsi(ent.Comp.NumberingResource, n.ToString()));
			}
		}
	}

	private unsafe List<string> BroilerListTanks(Entity<RMCBroilerComponent> ent)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		List<string> list = new List<string>();
		AllContainersEnumerable allContainers = _container.GetAllContainers(Entity<RMCBroilerComponent>.op_Implicit(ent), (ContainerManagerComponent)null);
		AllContainersEnumerator enumerator = ((AllContainersEnumerable)(ref allContainers)).GetEnumerator();
		try
		{
			while (((AllContainersEnumerator)(ref enumerator)).MoveNext())
			{
				string name = ((AllContainersEnumerator)(ref enumerator)).Current.ID;
				if (name.StartsWith(ent.Comp.ContainerPrefix))
				{
					list.Add(name);
				}
			}
			return list;
		}
		finally
		{
			((IDisposable)(*(AllContainersEnumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private void OnBroilerAction(Entity<RMCBroilerComponent> ent, ref RMCBroilerActionEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		((HandledEntityEventArgs)args).Handled = true;
		ent.Comp.ActiveTank = (ent.Comp.ActiveTank + 1) % BroilerListTanks(ent).Count;
		((EntitySystem)this).Dirty<RMCBroilerComponent>(ent, (MetaDataComponent)null);
		int n = ent.Comp.ActiveTank + 1;
		EntityUid? action = ent.Comp.Action;
		if (action.HasValue)
		{
			EntityUid action2 = action.GetValueOrDefault();
			_action.SetIcon(Entity<ActionComponent>.op_Implicit(action2), (SpriteSpecifier?)new Rsi(ent.Comp.NumberingResource, n.ToString()));
		}
		_popup.PopupClient(base.Loc.GetString("rmc-broiler-switch-tank", (ValueTuple<string, object>)("n", n)), Entity<RMCBroilerComponent>.op_Implicit(ent), args.Performer);
	}

	public void OnBroilerUniqueAction(Entity<RMCCanUseBroilerComponent> ent, ref UniqueActionEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		InventorySystem.InventorySlotEnumerator inventoryEnumerator = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(args.UserUid));
		ContainerSlot slot;
		RMCBroilerComponent rMCBroilerComponent = default(RMCBroilerComponent);
		while (inventoryEnumerator.MoveNext(out slot))
		{
			if (((EntitySystem)this).TryComp<RMCBroilerComponent>(slot.ContainedEntity, ref rMCBroilerComponent))
			{
				((HandledEntityEventArgs)args).Handled = true;
				RMCBroilerActionEvent ev = new RMCBroilerActionEvent();
				ev.Performer = args.UserUid;
				((EntitySystem)this).RaiseLocalEvent<RMCBroilerActionEvent>(slot.ContainedEntity.Value, ev, false);
				break;
			}
		}
	}

	public void OnBroilerUniqueActionExamine(Entity<RMCCanUseBroilerComponent> ent, ref ExaminedEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		args.PushMarkup(base.Loc.GetString(LocId.op_Implicit(ent.Comp.ExamineText)), 1);
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<RMCFlamerChainComponent> chains = ((EntitySystem)this).EntityQueryEnumerator<RMCFlamerChainComponent>();
		EntityUid uid = default(EntityUid);
		RMCFlamerChainComponent comp = default(RMCFlamerChainComponent);
		while (chains.MoveNext(ref uid, ref comp))
		{
			if (comp.Tiles.Count == 0)
			{
				((EntitySystem)this).QueueDel((EntityUid?)uid);
				continue;
			}
			foreach (LineTile tile in comp.Tiles)
			{
				if (time >= tile.At)
				{
					comp.Tiles.Remove(tile);
					EntityUid fire = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(comp.Spawn), tile.Coordinates, (ComponentRegistry)null, default(Angle));
					if (_rmcMap.HasAnchoredEntityEnumerator<TileFireComponent>(_transform.ToCoordinates(Entity<TransformComponent>.op_Implicit(fire), tile.Coordinates), out Entity<TileFireComponent> oldTileFire, (Direction?)null, (DirectionFlag)0) && oldTileFire.Owner.Id != fire.Id)
					{
						((EntitySystem)this).QueueDel((EntityUid?)Entity<TileFireComponent>.op_Implicit(oldTileFire));
					}
					if (_reagent.TryIndex(comp.Reagent, out Reagent reagent))
					{
						int intensity = Math.Min(comp.MaxIntensity, reagent.Intensity);
						int duration = Math.Min(comp.MaxDuration, reagent.Duration);
						_rmcFlammable.SetIntensityDuration(Entity<RMCIgniteOnCollideComponent, DamageOnCollideComponent>.op_Implicit(fire), intensity, duration);
					}
					break;
				}
			}
		}
	}
}
