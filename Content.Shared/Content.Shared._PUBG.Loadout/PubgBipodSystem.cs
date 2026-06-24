using System;
using Content.Shared.DoAfter;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._PUBG.Loadout;

public sealed class PubgBipodSystem : EntitySystem
{
	[Dependency]
	private readonly PubgWeaponModulesSystem _modules;

	[Dependency]
	private readonly SharedHandsSystem _hands;

	[Dependency]
	private readonly SharedDoAfterSystem _doAfter;

	[Dependency]
	private readonly SharedGunSystem _gun;

	[Dependency]
	private readonly SharedAudioSystem _audio;

	[Dependency]
	private readonly SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PubgWeaponModulesComponent, PubgToggleBipodActionEvent>((EntityEventRefHandler<PubgWeaponModulesComponent, PubgToggleBipodActionEvent>)OnToggleBipodAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgWeaponModulesComponent, PubgBipodDeployDoAfterEvent>((EntityEventRefHandler<PubgWeaponModulesComponent, PubgBipodDeployDoAfterEvent>)OnDeployDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgBipodDeployedComponent, MoveInputEvent>((EntityEventRefHandler<PubgBipodDeployedComponent, MoveInputEvent>)OnDeployedMoveInput, (Type[])null, (Type[])null);
	}

	private void OnToggleBipodAction(Entity<PubgWeaponModulesComponent> ent, ref PubgToggleBipodActionEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		HandsComponent handsComp = default(HandsComponent);
		PubgBipodComponent bipod = default(PubgBipodComponent);
		if (!((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).TryComp<HandsComponent>(args.Performer, ref handsComp) && _hands.IsHolding(Entity<HandsComponent>.op_Implicit((args.Performer, handsComp)), ent.Owner) && _modules.TryGetInstalledModule(Entity<PubgWeaponModulesComponent>.op_Implicit(ent), PubgModuleSlotType.Underbarrel, out EntityUid module, out PubgWeaponModuleSlotDefinition _, ent.Comp) && ((EntitySystem)this).TryComp<PubgBipodComponent>(module, ref bipod))
		{
			if (bipod.Deployed)
			{
				Undeploy(Entity<PubgBipodComponent>.op_Implicit((module, bipod)), Entity<PubgWeaponModulesComponent>.op_Implicit(ent), args.Performer);
			}
			else
			{
				EntityManager entityManager = base.EntityManager;
				EntityUid performer = args.Performer;
				float deployTime = bipod.DeployTime;
				PubgBipodDeployDoAfterEvent pubgBipodDeployDoAfterEvent = new PubgBipodDeployDoAfterEvent();
				EntityUid? eventTarget = ent.Owner;
				EntityUid? used = ent.Owner;
				DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)entityManager, performer, deployTime, pubgBipodDeployDoAfterEvent, eventTarget, null, used)
				{
					NeedHand = true,
					BreakOnMove = true,
					BreakOnHandChange = true
				};
				_doAfter.TryStartDoAfter(doAfter);
			}
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnDeployDoAfter(Entity<PubgWeaponModulesComponent> ent, ref PubgBipodDeployDoAfterEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		PubgBipodComponent bipod = default(PubgBipodComponent);
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled && _modules.TryGetInstalledModule(Entity<PubgWeaponModulesComponent>.op_Implicit(ent), PubgModuleSlotType.Underbarrel, out EntityUid module, out PubgWeaponModuleSlotDefinition _, ent.Comp) && ((EntitySystem)this).TryComp<PubgBipodComponent>(module, ref bipod))
		{
			if (!bipod.Deployed)
			{
				Deploy(Entity<PubgBipodComponent>.op_Implicit((module, bipod)), Entity<PubgWeaponModulesComponent>.op_Implicit(ent), args.User);
			}
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void Deploy(Entity<PubgBipodComponent> bipod, EntityUid gun, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		bipod.Comp.Deployed = true;
		((EntitySystem)this).Dirty<PubgBipodComponent>(bipod, (MetaDataComponent)null);
		((EntitySystem)this).EnsureComp<PubgBipodDeployedComponent>(user);
		_gun.RefreshModifiers(Entity<GunComponent>.op_Implicit(gun));
		_audio.PlayPredicted(bipod.Comp.DeploySound, gun, (EntityUid?)user, (AudioParams?)null);
		_popup.PopupClient(base.Loc.GetString("pubg-bipod-deploy"), user, user);
	}

	private void Undeploy(Entity<PubgBipodComponent> bipod, EntityUid gun, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (bipod.Comp.Deployed)
		{
			bipod.Comp.Deployed = false;
			((EntitySystem)this).Dirty<PubgBipodComponent>(bipod, (MetaDataComponent)null);
			_gun.RefreshModifiers(Entity<GunComponent>.op_Implicit(gun));
			_audio.PlayPredicted(bipod.Comp.UndeploySound, gun, (EntityUid?)user, (AudioParams?)null);
			_popup.PopupClient(base.Loc.GetString("pubg-bipod-undeploy"), user, user);
			if (!HasDeployedBipodInHands(user))
			{
				((EntitySystem)this).RemCompDeferred<PubgBipodDeployedComponent>(user);
			}
		}
	}

	private void OnDeployedMoveInput(Entity<PubgBipodDeployedComponent> ent, ref MoveInputEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		if (!args.HasDirectionalMovement)
		{
			return;
		}
		HandsComponent hands = default(HandsComponent);
		if (!((EntitySystem)this).TryComp<HandsComponent>(ent.Owner, ref hands))
		{
			((EntitySystem)this).RemCompDeferred<PubgBipodDeployedComponent>(Entity<PubgBipodDeployedComponent>.op_Implicit(ent));
			return;
		}
		PubgWeaponModulesComponent modules = default(PubgWeaponModulesComponent);
		PubgBipodComponent bipod = default(PubgBipodComponent);
		foreach (string handId in hands.Hands.Keys)
		{
			if (_hands.TryGetHeldItem(Entity<HandsComponent>.op_Implicit((ent.Owner, hands)), handId, out var heldItem) && heldItem.HasValue && ((EntitySystem)this).TryComp<PubgWeaponModulesComponent>(heldItem.Value, ref modules) && _modules.TryGetInstalledModule(heldItem.Value, PubgModuleSlotType.Underbarrel, out EntityUid module, out PubgWeaponModuleSlotDefinition _, modules) && ((EntitySystem)this).TryComp<PubgBipodComponent>(module, ref bipod) && bipod.Deployed)
			{
				Undeploy(Entity<PubgBipodComponent>.op_Implicit((module, bipod)), heldItem.Value, ent.Owner);
			}
		}
		((EntitySystem)this).RemCompDeferred<PubgBipodDeployedComponent>(Entity<PubgBipodDeployedComponent>.op_Implicit(ent));
	}

	private bool HasDeployedBipodInHands(EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		HandsComponent hands = default(HandsComponent);
		if (!((EntitySystem)this).TryComp<HandsComponent>(user, ref hands))
		{
			return false;
		}
		PubgWeaponModulesComponent modules = default(PubgWeaponModulesComponent);
		PubgBipodComponent bipod = default(PubgBipodComponent);
		foreach (string handId in hands.Hands.Keys)
		{
			if (_hands.TryGetHeldItem(Entity<HandsComponent>.op_Implicit((user, hands)), handId, out var heldItem) && heldItem.HasValue && ((EntitySystem)this).TryComp<PubgWeaponModulesComponent>(heldItem.Value, ref modules) && _modules.TryGetInstalledModule(heldItem.Value, PubgModuleSlotType.Underbarrel, out EntityUid module, out PubgWeaponModuleSlotDefinition _, modules) && ((EntitySystem)this).TryComp<PubgBipodComponent>(module, ref bipod) && bipod.Deployed)
			{
				return true;
			}
		}
		return false;
	}
}
