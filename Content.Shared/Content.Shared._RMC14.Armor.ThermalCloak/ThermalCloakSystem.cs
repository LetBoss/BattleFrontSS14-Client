using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Chemistry;
using Content.Shared._RMC14.NightVision;
using Content.Shared._RMC14.Stealth;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared._RMC14.Xenonids.Devour;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Projectile;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Explosion.EntitySystems;
using Content.Shared.Humanoid;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Mobs;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Armor.ThermalCloak;

public sealed class ThermalCloakSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedHumanoidAppearanceSystem _humanoidSystem;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private INetManager _net;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ThermalCloakComponent, GetItemActionsEvent>((EntityEventRefHandler<ThermalCloakComponent, GetItemActionsEvent>)OnGetItemActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ThermalCloakComponent, ThermalCloakTurnInvisibleActionEvent>((EntityEventRefHandler<ThermalCloakComponent, ThermalCloakTurnInvisibleActionEvent>)OnCloakAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ThermalCloakComponent, GotEquippedEvent>((EntityEventRefHandler<ThermalCloakComponent, GotEquippedEvent>)OnEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ThermalCloakComponent, GotUnequippedEvent>((EntityEventRefHandler<ThermalCloakComponent, GotUnequippedEvent>)OnUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityActiveInvisibleComponent, VaporHitEvent>((EntityEventRefHandler<EntityActiveInvisibleComponent, VaporHitEvent>)OnVaporHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityActiveInvisibleComponent, MobStateChangedEvent>((EntityEventRefHandler<EntityActiveInvisibleComponent, MobStateChangedEvent>)OnMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityActiveInvisibleComponent, XenoDevouredEvent>((EntityEventRefHandler<EntityActiveInvisibleComponent, XenoDevouredEvent>)OnDevour, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityActiveInvisibleComponent, XenoParasiteInfectEvent>((EntityEventRefHandler<EntityActiveInvisibleComponent, XenoParasiteInfectEvent>)OnParasiteInfect, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunComponent, AttemptShootEvent>((EntityEventRefHandler<GunComponent, AttemptShootEvent>)OnAttemptShoot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CancelUseWithCloakComponent, UseInHandEvent>((EntityEventRefHandler<CancelUseWithCloakComponent, UseInHandEvent>)OnTimerUse, new Type[1] { typeof(SharedTriggerSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UncloakOnHitComponent, ProjectileHitEvent>((EntityEventRefHandler<UncloakOnHitComponent, ProjectileHitEvent>)OnAcidProjectile, (Type[])null, (Type[])null);
	}

	private void OnGetItemActions(Entity<ThermalCloakComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		ThermalCloakComponent comp = ent.Comp;
		if (!args.InHands && _inventory.InSlotWithFlags(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(Entity<ThermalCloakComponent>.op_Implicit(ent), null, null)), SlotFlags.BACK))
		{
			args.AddAction(ref comp.Action, EntProtoId.op_Implicit(comp.ActionId));
			((EntitySystem)this).Dirty<ThermalCloakComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnCloakAction(Entity<ThermalCloakComponent> ent, ref ThermalCloakTurnInvisibleActionEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			if (!_whitelist.IsWhitelistPass(ent.Comp.Whitelist, args.Performer))
			{
				string popup = base.Loc.GetString("cm-gun-unskilled", (ValueTuple<string, object>)("gun", ent.Owner));
				_popup.PopupClient(popup, args.Performer, args.Performer, PopupType.SmallCaution);
			}
			else
			{
				SetInvisibility(ent, args.Performer, !ent.Comp.Enabled, forced: false);
			}
		}
	}

	private void OnEquipped(Entity<ThermalCloakComponent> ent, ref GotEquippedEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && _inventory.InSlotWithFlags(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(Entity<ThermalCloakComponent>.op_Implicit(ent), null, null)), SlotFlags.BACK))
		{
			EntityTurnInvisibleComponent comp = ((EntitySystem)this).EnsureComp<EntityTurnInvisibleComponent>(args.Equipee);
			comp.RestrictWeapons = ent.Comp.RestrictWeapons;
			comp.UncloakWeaponLock = ent.Comp.UncloakWeaponLock;
			((EntitySystem)this).Dirty(args.Equipee, (IComponent)(object)comp, (MetaDataComponent)null);
		}
	}

	private void OnUnequipped(Entity<ThermalCloakComponent> ent, ref GotUnequippedEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && !_inventory.InSlotWithFlags(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(Entity<ThermalCloakComponent>.op_Implicit(ent), null, null)), SlotFlags.BACK))
		{
			SetInvisibility(ent, args.Equipee, enabling: false, forced: false);
			((EntitySystem)this).RemCompDeferred<EntityTurnInvisibleComponent>(args.Equipee);
		}
	}

	public void SetInvisibility(Entity<ThermalCloakComponent> ent, EntityUid user, bool enabling, bool forced)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		EntityTurnInvisibleComponent turnInvisible = default(EntityTurnInvisibleComponent);
		if (!((EntitySystem)this).TryComp<EntityTurnInvisibleComponent>(user, ref turnInvisible))
		{
			return;
		}
		if (enabling && !((EntitySystem)this).HasComp<EntityActiveInvisibleComponent>(user))
		{
			EntityActiveInvisibleComponent activeInvisibility = ((EntitySystem)this).EnsureComp<EntityActiveInvisibleComponent>(user);
			activeInvisibility.Opacity = ent.Comp.Opacity;
			((EntitySystem)this).Dirty(user, (IComponent)(object)activeInvisibility, (MetaDataComponent)null);
			ent.Comp.Enabled = true;
			turnInvisible.Enabled = true;
			((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)ent.Comp, (MetaDataComponent)null);
			((EntitySystem)this).Dirty(user, (IComponent)(object)turnInvisible, (MetaDataComponent)null);
			ActionComponent action = default(ActionComponent);
			if (((EntitySystem)this).HasComp<InstantActionComponent>(ent.Comp.Action) && ((EntitySystem)this).TryComp<ActionComponent>(ent.Comp.Action, ref action))
			{
				_actions.SetCooldown(Entity<ActionComponent>.op_Implicit((ent.Comp.Action.Value, action)), _timing.CurTime, _timing.CurTime + ent.Comp.Cooldown);
				_actions.SetUseDelay(Entity<ActionComponent>.op_Implicit((ent.Comp.Action.Value, action)), ent.Comp.Cooldown);
			}
			if (ent.Comp.HideNightVision)
			{
				((EntitySystem)this).RemCompDeferred<RMCNightVisionVisibleComponent>(user);
			}
			if (ent.Comp.BlockFriendlyFire)
			{
				((EntitySystem)this).EnsureComp<EntityIFFComponent>(user);
			}
			turnInvisible.UncloakTime = _timing.CurTime;
			ToggleLayers(user, ent.Comp.CloakedHideLayers, showLayers: false);
			SpawnCloakEffects(user, ent.Comp.CloakEffect);
			string popupOthers = base.Loc.GetString("rmc-cloak-activate-others", (ValueTuple<string, object>)("user", user));
			_popup.PopupPredicted(base.Loc.GetString("rmc-cloak-activate-self"), popupOthers, user, user, PopupType.Medium);
			if (_net.IsServer)
			{
				_audio.PlayPvs(ent.Comp.CloakSound, user, (AudioParams?)null);
			}
		}
		else
		{
			EntityActiveInvisibleComponent invisible = default(EntityActiveInvisibleComponent);
			if (enabling || !((EntitySystem)this).TryComp<EntityActiveInvisibleComponent>(user, ref invisible))
			{
				return;
			}
			invisible.Opacity = 1f;
			((EntitySystem)this).Dirty(user, (IComponent)(object)invisible, (MetaDataComponent)null);
			ent.Comp.Enabled = false;
			turnInvisible.Enabled = false;
			((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)ent.Comp, (MetaDataComponent)null);
			((EntitySystem)this).Dirty(user, (IComponent)(object)turnInvisible, (MetaDataComponent)null);
			if (forced)
			{
				ActionComponent action2 = default(ActionComponent);
				if (((EntitySystem)this).HasComp<InstantActionComponent>(ent.Comp.Action) && ((EntitySystem)this).TryComp<ActionComponent>(ent.Comp.Action, ref action2))
				{
					_actions.SetCooldown(Entity<ActionComponent>.op_Implicit((ent.Comp.Action.Value, action2)), _timing.CurTime, _timing.CurTime + ent.Comp.ForcedCooldown);
					_actions.SetUseDelay(Entity<ActionComponent>.op_Implicit((ent.Comp.Action.Value, action2)), ent.Comp.ForcedCooldown);
				}
				turnInvisible.UncloakTime = _timing.CurTime;
				string forcedPopupOthers = base.Loc.GetString("rmc-cloak-forced-deactivate-others", (ValueTuple<string, object>)("user", user));
				_popup.PopupPredicted(base.Loc.GetString("rmc-cloak-forced-deactivate-self"), forcedPopupOthers, user, user, PopupType.Medium);
			}
			else
			{
				ActionComponent action3 = default(ActionComponent);
				if (((EntitySystem)this).HasComp<InstantActionComponent>(ent.Comp.Action) && ((EntitySystem)this).TryComp<ActionComponent>(ent.Comp.Action, ref action3))
				{
					_actions.SetCooldown(Entity<ActionComponent>.op_Implicit((ent.Comp.Action.Value, action3)), _timing.CurTime, _timing.CurTime + ent.Comp.Cooldown);
					_actions.SetUseDelay(Entity<ActionComponent>.op_Implicit((ent.Comp.Action.Value, action3)), ent.Comp.Cooldown);
				}
				turnInvisible.UncloakTime = _timing.CurTime;
				string popupOthers2 = base.Loc.GetString("rmc-cloak-deactivate-others", (ValueTuple<string, object>)("user", user));
				_popup.PopupPredicted(base.Loc.GetString("rmc-cloak-deactivate-self"), popupOthers2, user, user, PopupType.Medium);
			}
			ToggleLayers(user, ent.Comp.CloakedHideLayers, showLayers: true);
			SpawnCloakEffects(user, ent.Comp.UncloakEffect);
			if (ent.Comp.HideNightVision)
			{
				((EntitySystem)this).EnsureComp<RMCNightVisionVisibleComponent>(user);
			}
			if (ent.Comp.BlockFriendlyFire)
			{
				((EntitySystem)this).RemCompDeferred<EntityIFFComponent>(user);
			}
			((EntitySystem)this).RemCompDeferred<EntityActiveInvisibleComponent>(user);
			if (_net.IsServer)
			{
				_audio.PlayPvs(ent.Comp.UncloakSound, user, (AudioParams?)null);
			}
		}
	}

	public void TrySetInvisibility(EntityUid uid, bool enabling, bool forced, ThermalCloakComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Entity<ThermalCloakComponent>? cloak = FindWornCloak(uid);
		if (cloak.HasValue)
		{
			SetInvisibility(cloak.Value, uid, enabling, forced);
		}
	}

	private void OnAttemptShoot(Entity<GunComponent> ent, ref AttemptShootEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		EntityTurnInvisibleComponent comp = default(EntityTurnInvisibleComponent);
		if (!args.Cancelled && ((EntitySystem)this).TryComp<EntityTurnInvisibleComponent>(args.User, ref comp) && ((comp.RestrictWeapons && comp.Enabled) || comp.UncloakTime + comp.UncloakWeaponLock > _timing.CurTime))
		{
			args.Cancelled = true;
			string popup = base.Loc.GetString("rmc-cloak-attempt-shoot");
			_popup.PopupClient(popup, args.User, args.User, PopupType.SmallCaution);
		}
	}

	private void OnTimerUse(Entity<CancelUseWithCloakComponent> ent, ref UseInHandEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		EntityTurnInvisibleComponent comp = default(EntityTurnInvisibleComponent);
		if (!((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).TryComp<EntityTurnInvisibleComponent>(args.User, ref comp) && ((comp.RestrictWeapons && comp.Enabled) || comp.UncloakTime + comp.UncloakWeaponLock > _timing.CurTime))
		{
			((HandledEntityEventArgs)args).Handled = true;
			string popup = base.Loc.GetString(ent.Comp.CancelMessage);
			_popup.PopupClient(popup, args.User, args.User, PopupType.SmallCaution);
		}
	}

	private void OnAcidProjectile(Entity<UncloakOnHitComponent> ent, ref ProjectileHitEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		TrySetInvisibility(args.Target, enabling: false, forced: true);
	}

	private void OnVaporHit(Entity<EntityActiveInvisibleComponent> ent, ref VaporHitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		TrySetInvisibility(ent.Owner, enabling: false, forced: true);
	}

	private void OnMobStateChanged(Entity<EntityActiveInvisibleComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (args.NewMobState == MobState.Dead)
		{
			TrySetInvisibility(ent.Owner, enabling: false, forced: true);
		}
	}

	private void OnDevour(Entity<EntityActiveInvisibleComponent> ent, ref XenoDevouredEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		TrySetInvisibility(ent.Owner, enabling: false, forced: true);
	}

	private void OnParasiteInfect(Entity<EntityActiveInvisibleComponent> ent, ref XenoParasiteInfectEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		TrySetInvisibility(ent.Owner, enabling: false, forced: true);
	}

	public Entity<ThermalCloakComponent>? FindWornCloak(EntityUid player)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		InventorySystem.InventorySlotEnumerator slots = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(player), SlotFlags.BACK);
		ContainerSlot slot;
		ThermalCloakComponent comp = default(ThermalCloakComponent);
		while (slots.MoveNext(out slot))
		{
			if (((EntitySystem)this).TryComp<ThermalCloakComponent>(slot.ContainedEntity, ref comp))
			{
				return Entity<ThermalCloakComponent>.op_Implicit((slot.ContainedEntity.Value, comp));
			}
		}
		return null;
	}

	private void ToggleLayers(EntityUid equipee, HashSet<HumanoidVisualLayers> layers, bool showLayers)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		foreach (HumanoidVisualLayers layer in layers)
		{
			_humanoidSystem.SetLayerVisibility(Entity<HumanoidAppearanceComponent>.op_Implicit(equipee), layer, showLayers);
		}
	}

	public void SpawnCloakEffects(EntityUid user, EntProtoId cloakProtoId)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			MapCoordinates coordinates = _transform.GetMapCoordinates(user, (TransformComponent)null);
			Angle rotation = _transform.GetWorldRotation(user);
			((EntitySystem)this).Spawn(EntProtoId.op_Implicit(cloakProtoId), coordinates, (ComponentRegistry)null, rotation);
		}
	}
}
