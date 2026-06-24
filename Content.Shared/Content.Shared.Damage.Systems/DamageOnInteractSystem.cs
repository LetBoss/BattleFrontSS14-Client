using System;
using Content.Shared._RMC14.Light;
using Content.Shared.Administration.Logs;
using Content.Shared.Damage.Components;
using Content.Shared.Database;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Damage.Systems;

public sealed class DamageOnInteractSystem : EntitySystem
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
	private ThrowingSystem _throwingSystem;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private SharedStunSystem _stun;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DamageOnInteractComponent, InteractHandEvent>((EntityEventRefHandler<DamageOnInteractComponent, InteractHandEvent>)OnHandInteract, (Type[])null, (Type[])null);
	}

	private void OnHandInteract(Entity<DamageOnInteractComponent> entity, ref InteractHandEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		if (_gameTiming.CurTime < entity.Comp.NextInteraction)
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
		else
		{
			if (!entity.Comp.IsDamageActive)
			{
				return;
			}
			DamageSpecifier totalDamage = entity.Comp.Damage;
			if (!entity.Comp.IgnoreResistances)
			{
				_inventorySystem.TryGetInventoryEntity<DamageOnInteractProtectionComponent>(Entity<InventoryComponent>.op_Implicit(args.User), out Entity<DamageOnInteractProtectionComponent> protectiveEntity);
				DamageOnInteractProtectionComponent protectiveComp = default(DamageOnInteractProtectionComponent);
				if (protectiveEntity.Comp == null && ((EntitySystem)this).TryComp<DamageOnInteractProtectionComponent>(args.User, ref protectiveComp))
				{
					protectiveEntity = Entity<DamageOnInteractProtectionComponent>.op_Implicit((args.User, protectiveComp));
				}
				if (protectiveEntity.Comp != null)
				{
					totalDamage = DamageSpecifier.ApplyModifierSet(totalDamage, protectiveEntity.Comp.DamageProtection);
				}
				else
				{
					LightBurnHandAttemptEvent ev = new LightBurnHandAttemptEvent(args.User, Entity<DamageOnInteractComponent>.op_Implicit(entity));
					((EntitySystem)this).RaiseLocalEvent<LightBurnHandAttemptEvent>(ref ev);
					if (ev.Cancelled)
					{
						return;
					}
				}
			}
			totalDamage = _damageableSystem.TryChangeDamage(args.User, totalDamage, ignoreResistances: false, interruptsDoAfters: true, null, args.Target);
			if (totalDamage != null && totalDamage.AnyPositive())
			{
				entity.Comp.LastInteraction = _gameTiming.CurTime;
				entity.Comp.NextInteraction = _gameTiming.CurTime + TimeSpan.FromSeconds(entity.Comp.InteractTimer);
				((HandledEntityEventArgs)args).Handled = true;
				ISharedAdminLogManager adminLogger = _adminLogger;
				LogStringHandler handler = new LogStringHandler(61, 3);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "user", "ToPrettyString(args.User)");
				handler.AppendLiteral(" injured their hand by interacting with ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.Target)), "target", "ToPrettyString(args.Target)");
				handler.AppendLiteral(" and received ");
				handler.AppendFormatted(totalDamage.GetTotal(), "damage", "totalDamage.GetTotal()");
				handler.AppendLiteral(" damage");
				adminLogger.Add(LogType.Damaged, ref handler);
				_audioSystem.PlayPredicted(entity.Comp.InteractSound, args.Target, (EntityUid?)args.User, (AudioParams?)null);
				if (entity.Comp.PopupText.HasValue)
				{
					SharedPopupSystem popupSystem = _popupSystem;
					ILocalizationManager loc = base.Loc;
					LocId? popupText = entity.Comp.PopupText;
					popupSystem.PopupClient(loc.GetString(popupText.HasValue ? LocId.op_Implicit(popupText.GetValueOrDefault()) : null), args.User, args.User);
				}
				if (RandomExtensions.Prob(_random, entity.Comp.StunChance))
				{
					_stun.TryParalyze(args.User, TimeSpan.FromSeconds(entity.Comp.StunSeconds), refresh: true);
				}
			}
			PullableComponent pullComp = default(PullableComponent);
			if (entity.Comp.Throw && ((EntitySystem)this).TryComp<PullableComponent>(Entity<DamageOnInteractComponent>.op_Implicit(entity), ref pullComp) && !pullComp.BeingPulled)
			{
				_throwingSystem.TryThrow(Entity<DamageOnInteractComponent>.op_Implicit(entity), _random.NextVector2(1f), entity.Comp.ThrowSpeed);
			}
		}
	}

	public void SetIsDamageActiveTo(Entity<DamageOnInteractComponent> entity, bool mode)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.IsDamageActive != mode)
		{
			entity.Comp.IsDamageActive = mode;
			((EntitySystem)this).Dirty<DamageOnInteractComponent>(entity, (MetaDataComponent)null);
		}
	}
}
