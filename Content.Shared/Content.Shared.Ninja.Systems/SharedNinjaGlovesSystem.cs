using System;
using Content.Shared.Clothing.Components;
using Content.Shared.CombatMode;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Ninja.Components;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Ninja.Systems;

public abstract class SharedNinjaGlovesSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedCombatModeSystem _combatMode;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private ItemToggleSystem _toggle;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedSpaceNinjaSystem _ninja;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<NinjaGlovesComponent, ToggleClothingCheckEvent>((EntityEventRefHandler<NinjaGlovesComponent, ToggleClothingCheckEvent>)OnToggleCheck, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NinjaGlovesComponent, ItemToggleActivateAttemptEvent>((EntityEventRefHandler<NinjaGlovesComponent, ItemToggleActivateAttemptEvent>)OnActivateAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NinjaGlovesComponent, ItemToggledEvent>((EntityEventRefHandler<NinjaGlovesComponent, ItemToggledEvent>)OnToggled, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NinjaGlovesComponent, ExaminedEvent>((EntityEventRefHandler<NinjaGlovesComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	private void DisableGloves(Entity<NinjaGlovesComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		Entity<NinjaGlovesComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		NinjaGlovesComponent ninjaGlovesComponent = default(NinjaGlovesComponent);
		val.Deconstruct(ref val2, ref ninjaGlovesComponent);
		EntityUid uid = val2;
		NinjaGlovesComponent comp = ninjaGlovesComponent;
		EntityUid? user = comp.User;
		if (!user.HasValue)
		{
			return;
		}
		EntityUid user2 = user.GetValueOrDefault();
		comp.User = null;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
		foreach (NinjaGloveAbility ability in comp.Abilities)
		{
			base.EntityManager.RemoveComponents(user2, ability.Components);
		}
	}

	private void OnToggleCheck(Entity<NinjaGlovesComponent> ent, ref ToggleClothingCheckEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (!_ninja.IsNinja(args.User))
		{
			args.Cancelled = true;
		}
	}

	private void OnExamined(Entity<NinjaGlovesComponent> ent, ref ExaminedEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (args.IsInDetailsRange)
		{
			string on = (_toggle.IsActivated(Entity<ItemToggleComponent>.op_Implicit(ent.Owner)) ? "on" : "off");
			args.PushText(base.Loc.GetString("ninja-gloves-examine-" + on));
		}
	}

	private void OnActivateAttempt(Entity<NinjaGlovesComponent> ent, ref ItemToggleActivateAttemptEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? user = args.User;
		if (user.HasValue)
		{
			EntityUid user2 = user.GetValueOrDefault();
			SpaceNinjaComponent ninja = default(SpaceNinjaComponent);
			if (_ninja.NinjaQuery.TryComp(user2, ref ninja) && ((EntitySystem)this).HasComp<NinjaSuitComponent>(ninja.Suit))
			{
				return;
			}
		}
		args.Cancelled = true;
		args.Popup = base.Loc.GetString("ninja-gloves-not-wearing-suit");
	}

	private void OnToggled(Entity<NinjaGlovesComponent> ent, ref ItemToggledEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? val = args.User ?? ent.Comp.User;
		if (val.HasValue)
		{
			EntityUid user = val.GetValueOrDefault();
			string message = base.Loc.GetString(args.Activated ? "ninja-gloves-on" : "ninja-gloves-off");
			_popup.PopupClient(message, user, user);
			SpaceNinjaComponent ninja = default(SpaceNinjaComponent);
			if (args.Activated && _ninja.NinjaQuery.TryComp(user, ref ninja))
			{
				EnableGloves(ent, Entity<SpaceNinjaComponent>.op_Implicit((user, ninja)));
			}
			else
			{
				DisableGloves(ent);
			}
		}
	}

	protected virtual void EnableGloves(Entity<NinjaGlovesComponent> ent, Entity<SpaceNinjaComponent> user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		Entity<NinjaGlovesComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		NinjaGlovesComponent ninjaGlovesComponent = default(NinjaGlovesComponent);
		val.Deconstruct(ref val2, ref ninjaGlovesComponent);
		EntityUid uid = val2;
		NinjaGlovesComponent comp = ninjaGlovesComponent;
		comp.User = Entity<SpaceNinjaComponent>.op_Implicit(user);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
		_ninja.AssignGloves(user, uid);
		foreach (NinjaGloveAbility ability in comp.Abilities)
		{
			if (!ability.Objective.HasValue)
			{
				base.EntityManager.AddComponents(Entity<SpaceNinjaComponent>.op_Implicit(user), ability.Components, true);
			}
		}
	}

	public bool AbilityCheck(EntityUid uid, BeforeInteractHandEvent args, out EntityUid target)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		target = args.Target;
		if (_timing.IsFirstTimePredicted && !_combatMode.IsInCombatMode(uid) && !_hands.GetActiveItem(Entity<HandsComponent>.op_Implicit(uid)).HasValue)
		{
			return _interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(uid), Entity<TransformComponent>.op_Implicit(target));
		}
		return false;
	}
}
