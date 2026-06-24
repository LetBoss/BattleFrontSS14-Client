using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Tag;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Weapons.Ranged.Upgrades.Components;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared.Weapons.Ranged.Upgrades;

public sealed class GunUpgradeSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedGunSystem _gun;

	[Dependency]
	private EntityWhitelistSystem _entityWhitelist;

	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<UpgradeableGunComponent, ComponentInit>((EntityEventRefHandler<UpgradeableGunComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UpgradeableGunComponent, AfterInteractUsingEvent>((EntityEventRefHandler<UpgradeableGunComponent, AfterInteractUsingEvent>)OnAfterInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UpgradeableGunComponent, ExaminedEvent>((EntityEventRefHandler<UpgradeableGunComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UpgradeableGunComponent, GunRefreshModifiersEvent>((EntityEventRefHandler<UpgradeableGunComponent, GunRefreshModifiersEvent>)RelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UpgradeableGunComponent, GunShotEvent>((EntityEventRefHandler<UpgradeableGunComponent, GunShotEvent>)RelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunUpgradeFireRateComponent, GunRefreshModifiersEvent>((EntityEventRefHandler<GunUpgradeFireRateComponent, GunRefreshModifiersEvent>)OnFireRateRefresh, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunUpgradeSpeedComponent, GunRefreshModifiersEvent>((EntityEventRefHandler<GunUpgradeSpeedComponent, GunRefreshModifiersEvent>)OnSpeedRefresh, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunUpgradeDamageComponent, GunShotEvent>((EntityEventRefHandler<GunUpgradeDamageComponent, GunShotEvent>)OnDamageGunShot, (Type[])null, (Type[])null);
	}

	private void RelayEvent<T>(Entity<UpgradeableGunComponent> ent, ref T args) where T : notnull
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		foreach (Entity<GunUpgradeComponent> upgrade in GetCurrentUpgrades(ent))
		{
			((EntitySystem)this).RaiseLocalEvent<T>(Entity<GunUpgradeComponent>.op_Implicit(upgrade), ref args, false);
		}
	}

	private void OnExamine(Entity<UpgradeableGunComponent> ent, ref ExaminedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("UpgradeableGunComponent"))
		{
			foreach (Entity<GunUpgradeComponent> upgrade in GetCurrentUpgrades(ent))
			{
				args.PushMarkup(base.Loc.GetString(LocId.op_Implicit(upgrade.Comp.ExamineText)));
			}
		}
	}

	private void OnInit(Entity<UpgradeableGunComponent> ent, ref ComponentInit args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_container.EnsureContainer<Container>(Entity<UpgradeableGunComponent>.op_Implicit(ent), ent.Comp.UpgradesContainerId, (ContainerManagerComponent)null);
	}

	private void OnAfterInteractUsing(Entity<UpgradeableGunComponent> ent, ref AfterInteractUsingEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		GunUpgradeComponent upgradeComponent = default(GunUpgradeComponent);
		if (((HandledEntityEventArgs)args).Handled || !args.CanReach || !((EntitySystem)this).TryComp<GunUpgradeComponent>(args.Used, ref upgradeComponent))
		{
			return;
		}
		if (GetCurrentUpgrades(ent).Count >= ent.Comp.MaxUpgradeCount)
		{
			_popup.PopupPredicted(base.Loc.GetString("upgradeable-gun-popup-upgrade-limit"), Entity<UpgradeableGunComponent>.op_Implicit(ent), args.User);
		}
		else if (!_entityWhitelist.IsWhitelistFail(ent.Comp.Whitelist, args.Used))
		{
			if (GetCurrentUpgradeTags(ent).ToHashSet().IsSupersetOf(upgradeComponent.Tags))
			{
				_popup.PopupPredicted(base.Loc.GetString("upgradeable-gun-popup-already-present"), Entity<UpgradeableGunComponent>.op_Implicit(ent), args.User);
				return;
			}
			_audio.PlayPredicted(ent.Comp.InsertSound, Entity<UpgradeableGunComponent>.op_Implicit(ent), (EntityUid?)args.User, (AudioParams?)null);
			_popup.PopupClient(base.Loc.GetString("gun-upgrade-popup-insert", (ValueTuple<string, object>)("upgrade", args.Used), (ValueTuple<string, object>)("gun", ent.Owner)), args.User);
			_gun.RefreshModifiers(Entity<GunComponent>.op_Implicit(ent.Owner));
			((HandledEntityEventArgs)args).Handled = _container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(args.Used), _container.GetContainer(Entity<UpgradeableGunComponent>.op_Implicit(ent), ent.Comp.UpgradesContainerId, (ContainerManagerComponent)null), (TransformComponent)null, false);
			ISharedAdminLogManager adminLog = _adminLog;
			LogStringHandler handler = new LogStringHandler(29, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "player", "ToPrettyString(args.User)");
			handler.AppendLiteral(" inserted gun upgrade ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.Used)), "ToPrettyString(args.Used)");
			handler.AppendLiteral(" into ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(ent.Owner)), "ToPrettyString(ent.Owner)");
			handler.AppendLiteral(".");
			adminLog.Add(LogType.Action, LogImpact.Low, ref handler);
		}
	}

	private void OnFireRateRefresh(Entity<GunUpgradeFireRateComponent> ent, ref GunRefreshModifiersEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		args.FireRate *= ent.Comp.Coefficient;
	}

	private void OnSpeedRefresh(Entity<GunUpgradeSpeedComponent> ent, ref GunRefreshModifiersEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		args.ProjectileSpeed *= ent.Comp.Coefficient;
	}

	private void OnDamageGunShot(Entity<GunUpgradeDamageComponent> ent, ref GunShotEvent args)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		ProjectileComponent proj = default(ProjectileComponent);
		foreach (var item in args.Ammo)
		{
			EntityUid? ammo = item.Uid;
			if (((EntitySystem)this).TryComp<ProjectileComponent>(ammo, ref proj))
			{
				proj.Damage += ent.Comp.Damage;
			}
		}
	}

	public HashSet<Entity<GunUpgradeComponent>> GetCurrentUpgrades(Entity<UpgradeableGunComponent> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainer(Entity<UpgradeableGunComponent>.op_Implicit(ent), ent.Comp.UpgradesContainerId, ref container, (ContainerManagerComponent)null))
		{
			return new HashSet<Entity<GunUpgradeComponent>>();
		}
		HashSet<Entity<GunUpgradeComponent>> upgrades = new HashSet<Entity<GunUpgradeComponent>>();
		GunUpgradeComponent upgradeComp = default(GunUpgradeComponent);
		foreach (EntityUid contained in container.ContainedEntities)
		{
			if (((EntitySystem)this).TryComp<GunUpgradeComponent>(contained, ref upgradeComp))
			{
				upgrades.Add(Entity<GunUpgradeComponent>.op_Implicit((contained, upgradeComp)));
			}
		}
		return upgrades;
	}

	public IEnumerable<ProtoId<TagPrototype>> GetCurrentUpgradeTags(Entity<UpgradeableGunComponent> ent)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		foreach (Entity<GunUpgradeComponent> currentUpgrade in GetCurrentUpgrades(ent))
		{
			foreach (ProtoId<TagPrototype> tag in currentUpgrade.Comp.Tags)
			{
				yield return tag;
			}
		}
	}
}
