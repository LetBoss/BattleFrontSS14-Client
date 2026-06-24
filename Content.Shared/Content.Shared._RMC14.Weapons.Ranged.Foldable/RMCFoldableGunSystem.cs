using System;
using Content.Shared._RMC14.Weapons.Common;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Weapons.Ranged.Foldable;

public sealed class RMCFoldableGunSystem : EntitySystem
{
	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedHandsSystem _hands;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCFoldableGunComponent, ExaminedEvent>((EntityEventRefHandler<RMCFoldableGunComponent, ExaminedEvent>)OnExamined, new Type[1] { typeof(SharedGunSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFoldableGunComponent, GunShotEvent>((EntityEventRefHandler<RMCFoldableGunComponent, GunShotEvent>)OnGunShoot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFoldableGunComponent, AttemptShootEvent>((EntityEventRefHandler<RMCFoldableGunComponent, AttemptShootEvent>)OnAttemptShoot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFoldableGunComponent, UniqueActionEvent>((EntityEventRefHandler<RMCFoldableGunComponent, UniqueActionEvent>)OnUniqueAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFoldableGunComponent, ActivateInWorldEvent>((EntityEventRefHandler<RMCFoldableGunComponent, ActivateInWorldEvent>)OnActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFoldableGunComponent, RMCFoldableGunDoAfterEvent>((EntityEventRefHandler<RMCFoldableGunComponent, RMCFoldableGunDoAfterEvent>)OnFoldableGunDoAfter, (Type[])null, (Type[])null);
	}

	private void OnExamined(Entity<RMCFoldableGunComponent> ent, ref ExaminedEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		args.PushMarkup(base.Loc.GetString(LocId.op_Implicit(ent.Comp.ExamineText)), 1);
	}

	private void OnGunShoot(Entity<RMCFoldableGunComponent> ent, ref GunShotEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Fired = true;
	}

	private void OnAttemptShoot(Entity<RMCFoldableGunComponent> ent, ref AttemptShootEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && ent.Comp.Fired)
		{
			args.Cancelled = true;
		}
	}

	private void OnUniqueAction(Entity<RMCFoldableGunComponent> ent, ref UniqueActionEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = AttemptFold(ent, args.UserUid);
		}
	}

	private void OnActivate(Entity<RMCFoldableGunComponent> ent, ref ActivateInWorldEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && ent.Comp.OnActivate)
		{
			EntityUid user = args.User;
			if (_hands.IsHolding(Entity<HandsComponent>.op_Implicit(user), ent.Owner))
			{
				((HandledEntityEventArgs)args).Handled = AttemptFold(ent, user);
			}
		}
	}

	private void OnFoldableGunDoAfter(Entity<RMCFoldableGunComponent> ent, ref RMCFoldableGunDoAfterEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled)
		{
			return;
		}
		EntityUid user = args.User;
		if (!((HandledEntityEventArgs)args).Handled)
		{
			string selfText = base.Loc.GetString(LocId.op_Implicit(ent.Comp.FinishText), (ValueTuple<string, object>)("weapon", ent));
			string othersText = base.Loc.GetString(LocId.op_Implicit(ent.Comp.FinishTextOthers), (ValueTuple<string, object>)("user", user), (ValueTuple<string, object>)("weapon", ent));
			string handToUse = _hands.GetActiveHand(Entity<HandsComponent>.op_Implicit(user));
			if (handToUse != null)
			{
				EntityUid newEntity = ((EntitySystem)this).PredictedSpawnNextToOrDrop(EntProtoId.op_Implicit(ent.Comp.FoldedEntity), user, (TransformComponent)null, (ComponentRegistry)null);
				_hands.TryForcePickup(Entity<HandsComponent>.op_Implicit(newEntity), user, handToUse, checkActionBlocker: false);
				_popup.PopupPredicted(selfText, othersText, user, user);
				_audio.PlayPredicted(ent.Comp.ToggleFoldSound, user, (EntityUid?)user, (AudioParams?)null);
				((EntitySystem)this).PredictedQueueDel(ent.Owner);
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	public bool AttemptFold(Entity<RMCFoldableGunComponent> ent, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Fired)
		{
			string popupText = base.Loc.GetString("rmc-gun-foldable-launcher-fold-already-fired-attempt", (ValueTuple<string, object>)("weapon", ent));
			_popup.PopupClient(popupText, user, user, PopupType.SmallCaution);
			return false;
		}
		RMCFoldableGunDoAfterEvent ev = new RMCFoldableGunDoAfterEvent();
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, ent.Comp.FoldDelay, ev, Entity<RMCFoldableGunComponent>.op_Implicit(ent), Entity<RMCFoldableGunComponent>.op_Implicit(ent))
		{
			BreakOnMove = true,
			BreakOnDamage = false,
			MovementThreshold = 0.5f,
			DuplicateCondition = DuplicateConditions.SameEvent,
			CancelDuplicate = true,
			NeedHand = true,
			BreakOnDropItem = true
		};
		if (_doAfter.TryStartDoAfter(doAfter))
		{
			string selfText = base.Loc.GetString(LocId.op_Implicit(ent.Comp.FoldText), (ValueTuple<string, object>)("weapon", ent));
			string othersText = base.Loc.GetString(LocId.op_Implicit(ent.Comp.FoldTextOthers), (ValueTuple<string, object>)("user", user), (ValueTuple<string, object>)("weapon", ent));
			_popup.PopupPredicted(selfText, othersText, user, user);
			return true;
		}
		return false;
	}
}
