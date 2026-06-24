using System;
using Content.Shared.Administration.Logs;
using Content.Shared.Damage.Components;
using Content.Shared.Database;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Shared.Damage.Systems;

public sealed class DamageOnAttackedSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private DamageableSystem _damageableSystem;

	[Dependency]
	private SharedAudioSystem _audioSystem;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private InventorySystem _inventorySystem;

	[Dependency]
	private SharedHandsSystem _handsSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DamageOnAttackedComponent, AttackedEvent>((EntityEventRefHandler<DamageOnAttackedComponent, AttackedEvent>)OnAttacked, (Type[])null, (Type[])null);
	}

	private void OnAttacked(Entity<DamageOnAttackedComponent> entity, ref AttackedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (!entity.Comp.IsDamageActive)
		{
			return;
		}
		DamageSpecifier totalDamage = entity.Comp.Damage;
		if (!entity.Comp.IgnoreResistances)
		{
			_inventorySystem.TryGetInventoryEntity<DamageOnAttackedProtectionComponent>(Entity<InventoryComponent>.op_Implicit(args.User), out Entity<DamageOnAttackedProtectionComponent> protectiveEntity);
			HandsComponent handsComp = default(HandsComponent);
			DamageOnAttackedProtectionComponent itemProtectComp = default(DamageOnAttackedProtectionComponent);
			if (protectiveEntity.Comp == null && ((EntitySystem)this).TryComp<HandsComponent>(args.User, ref handsComp) && _handsSystem.TryGetActiveItem(Entity<HandsComponent>.op_Implicit((args.User, handsComp)), out var itemInHand) && ((EntitySystem)this).TryComp<DamageOnAttackedProtectionComponent>(itemInHand, ref itemProtectComp) && itemProtectComp.Slots == SlotFlags.NONE)
			{
				protectiveEntity = Entity<DamageOnAttackedProtectionComponent>.op_Implicit((itemInHand.Value, itemProtectComp));
			}
			DamageOnAttackedProtectionComponent protectiveComp = default(DamageOnAttackedProtectionComponent);
			if (protectiveEntity.Comp == null && ((EntitySystem)this).TryComp<DamageOnAttackedProtectionComponent>(args.User, ref protectiveComp))
			{
				protectiveEntity = Entity<DamageOnAttackedProtectionComponent>.op_Implicit((args.User, protectiveComp));
			}
			if (protectiveEntity.Comp != null)
			{
				totalDamage = DamageSpecifier.ApplyModifierSet(totalDamage, protectiveEntity.Comp.DamageProtection);
			}
		}
		totalDamage = _damageableSystem.TryChangeDamage(args.User, totalDamage, entity.Comp.IgnoreResistances, interruptsDoAfters: true, null, Entity<DamageOnAttackedComponent>.op_Implicit(entity));
		if (totalDamage != null && totalDamage.AnyPositive())
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(54, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "user", "ToPrettyString(args.User)");
			handler.AppendLiteral(" injured themselves by attacking ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<DamageOnAttackedComponent>.op_Implicit(entity), (MetaDataComponent)null), "target", "ToPrettyString(entity)");
			handler.AppendLiteral(" and received ");
			handler.AppendFormatted(totalDamage.GetTotal(), "damage", "totalDamage.GetTotal()");
			handler.AppendLiteral(" damage");
			adminLogger.Add(LogType.Damaged, ref handler);
			_audioSystem.PlayPredicted(entity.Comp.InteractSound, Entity<DamageOnAttackedComponent>.op_Implicit(entity), (EntityUid?)args.User, (AudioParams?)null);
			if (entity.Comp.PopupText.HasValue)
			{
				SharedPopupSystem popupSystem = _popupSystem;
				ILocalizationManager loc = base.Loc;
				LocId? popupText = entity.Comp.PopupText;
				popupSystem.PopupClient(loc.GetString(popupText.HasValue ? LocId.op_Implicit(popupText.GetValueOrDefault()) : null), args.User, args.User);
			}
		}
	}

	public void SetIsDamageActiveTo(Entity<DamageOnAttackedComponent> entity, bool mode)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.IsDamageActive != mode)
		{
			entity.Comp.IsDamageActive = mode;
			((EntitySystem)this).Dirty<DamageOnAttackedComponent>(entity, (MetaDataComponent)null);
		}
	}
}
