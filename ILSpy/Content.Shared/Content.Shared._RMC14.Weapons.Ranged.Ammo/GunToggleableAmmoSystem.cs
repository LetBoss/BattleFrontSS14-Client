using System;
using System.Runtime.InteropServices;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Weapons.Common;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Damage;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Weapons.Ranged.Ammo;

public sealed class GunToggleableAmmoSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private CMArmorSystem _cmArmor;

	[Dependency]
	private SharedPopupSystem _popup;

	private EntityQuery<ProjectileComponent> _projectileQuery;

	private EntityQuery<CMArmorPiercingComponent> _armorPiercingQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_projectileQuery = ((EntitySystem)this).GetEntityQuery<ProjectileComponent>();
		_armorPiercingQuery = ((EntitySystem)this).GetEntityQuery<CMArmorPiercingComponent>();
		((EntitySystem)this).SubscribeLocalEvent<GunToggleableAmmoComponent, GetItemActionsEvent>((EntityEventRefHandler<GunToggleableAmmoComponent, GetItemActionsEvent>)OnGetItemActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunToggleableAmmoComponent, GunToggleAmmoActionEvent>((EntityEventRefHandler<GunToggleableAmmoComponent, GunToggleAmmoActionEvent>)OnToggleAmmoAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunToggleableAmmoComponent, AmmoShotEvent>((EntityEventRefHandler<GunToggleableAmmoComponent, AmmoShotEvent>)OnAmmoShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunToggleableAmmoComponent, UniqueActionEvent>((EntityEventRefHandler<GunToggleableAmmoComponent, UniqueActionEvent>)OnUniqueAction, (Type[])null, (Type[])null);
	}

	private void OnGetItemActions(Entity<GunToggleableAmmoComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		args.AddAction(ref ent.Comp.Action, EntProtoId.op_Implicit(ent.Comp.ActionId));
		((EntitySystem)this).Dirty<GunToggleableAmmoComponent>(ent, (MetaDataComponent)null);
	}

	private void OnToggleAmmoAction(Entity<GunToggleableAmmoComponent> ent, ref GunToggleAmmoActionEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		if (ToggleAmmo(ent, args.Performer))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnAmmoShot(Entity<GunToggleableAmmoComponent> ent, ref AmmoShotEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		int settingIndex = ent.Comp.Setting;
		if (settingIndex < 0 || settingIndex >= ent.Comp.Settings.Count)
		{
			return;
		}
		ref GunToggleableAmmoSetting setting = ref CollectionsMarshal.AsSpan(ent.Comp.Settings)[settingIndex];
		ProjectileComponent projectileComp = default(ProjectileComponent);
		CMArmorPiercingComponent armorPiercing = default(CMArmorPiercingComponent);
		foreach (EntityUid projectile in args.FiredProjectiles)
		{
			if (_projectileQuery.TryComp(projectile, ref projectileComp))
			{
				projectileComp.Damage = new DamageSpecifier(setting.Damage);
				((EntitySystem)this).Dirty(projectile, (IComponent)(object)projectileComp, (MetaDataComponent)null);
			}
			if (_armorPiercingQuery.TryComp(projectile, ref armorPiercing))
			{
				_cmArmor.SetArmorPiercing(Entity<CMArmorPiercingComponent>.op_Implicit((projectile, armorPiercing)), setting.ArmorPiercing);
			}
		}
	}

	private void OnUniqueAction(Entity<GunToggleableAmmoComponent> ent, ref UniqueActionEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && ToggleAmmo(ent, args.UserUid))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private bool ToggleAmmo(Entity<GunToggleableAmmoComponent> ent, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Settings.Count == 0)
		{
			return false;
		}
		ref int settingIndex = ref ent.Comp.Setting;
		settingIndex++;
		if (settingIndex >= ent.Comp.Settings.Count)
		{
			settingIndex = 0;
		}
		GunToggleableAmmoSetting setting = ent.Comp.Settings[settingIndex];
		string popup = base.Loc.GetString("rmc-toggleable-ammo-firing", (ValueTuple<string, object>)("ammo", base.Loc.GetString(LocId.op_Implicit(setting.Name))));
		_popup.PopupClient(popup, user, user, PopupType.Large);
		_audio.PlayPredicted(ent.Comp.ToggleSound, Entity<GunToggleableAmmoComponent>.op_Implicit(ent), (EntityUid?)user, (AudioParams?)null);
		EntityUid? action = ent.Comp.Action;
		if (action.HasValue)
		{
			EntityUid action2 = action.GetValueOrDefault();
			_actions.SetIcon(Entity<ActionComponent>.op_Implicit(action2), (SpriteSpecifier?)(object)setting.Icon);
		}
		((EntitySystem)this).Dirty<GunToggleableAmmoComponent>(ent, (MetaDataComponent)null);
		return true;
	}
}
