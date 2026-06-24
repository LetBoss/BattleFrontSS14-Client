using System;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Weapons.Ranged.Ammo.BulletBox;

public sealed class BulletBoxSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedGunSystem _gun;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedHandsSystem _hands;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<BulletBoxComponent, MapInitEvent>((EntityEventRefHandler<BulletBoxComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BulletBoxComponent, ExaminedEvent>((EntityEventRefHandler<BulletBoxComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BulletBoxComponent, InteractUsingEvent>((EntityEventRefHandler<BulletBoxComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BulletBoxComponent, BulletBoxTransferDoAfterEvent>((EntityEventRefHandler<BulletBoxComponent, BulletBoxTransferDoAfterEvent>)OnTransferDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BulletBoxComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<BulletBoxComponent, GetVerbsEvent<AlternativeVerb>>)OnGetAlternativeVerbs, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<BulletBoxComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(ent);
	}

	private void OnExamined(Entity<BulletBoxComponent> ent, ref ExaminedEvent args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("BulletBoxComponent"))
		{
			args.PushText(base.Loc.GetString("rmc-bullet-box-amount", (ValueTuple<string, object>)("amount", ent.Comp.Amount)));
		}
	}

	private void OnGetAlternativeVerbs(Entity<BulletBoxComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		if (!_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(user), out var usedId))
		{
			return;
		}
		Entity<RefillableByBulletBoxComponent?, BallisticAmmoProviderComponent?> used = new Entity<RefillableByBulletBoxComponent, BallisticAmmoProviderComponent>(usedId.Value, (RefillableByBulletBoxComponent)null, (BallisticAmmoProviderComponent)null);
		if (!((EntitySystem)this).Resolve<RefillableByBulletBoxComponent, BallisticAmmoProviderComponent>(Entity<RefillableByBulletBoxComponent, BallisticAmmoProviderComponent>.op_Implicit(used), ref used.Comp1, ref used.Comp2, false))
		{
			return;
		}
		args.Verbs.Add(new AlternativeVerb
		{
			Act = delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				if (CanTransferPopup(ent, user, ref used, transferToBox: true))
				{
					BulletBoxTransferDoAfterEvent bulletBoxTransferDoAfterEvent = new BulletBoxTransferDoAfterEvent(toFrom: true);
					TimeSpan delay = ent.Comp.Delay;
					DoAfterArgs args2 = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, delay, bulletBoxTransferDoAfterEvent, Entity<BulletBoxComponent>.op_Implicit(ent), Entity<BulletBoxComponent>.op_Implicit(ent), usedId)
					{
						BreakOnMove = true,
						BreakOnDropItem = true,
						NeedHand = true
					};
					_doAfter.TryStartDoAfter(args2);
				}
			},
			Text = base.Loc.GetString("rmc-bullet-box-transferto"),
			Impact = LogImpact.Low
		});
	}

	private void OnInteractUsing(Entity<BulletBoxComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		Entity<RefillableByBulletBoxComponent, BallisticAmmoProviderComponent> used = default(Entity<RefillableByBulletBoxComponent, BallisticAmmoProviderComponent>);
		used._002Ector(args.Used, (RefillableByBulletBoxComponent)null, (BallisticAmmoProviderComponent)null);
		if (((EntitySystem)this).Resolve<RefillableByBulletBoxComponent, BallisticAmmoProviderComponent>(Entity<RefillableByBulletBoxComponent, BallisticAmmoProviderComponent>.op_Implicit(used), ref used.Comp1, ref used.Comp2, false))
		{
			((HandledEntityEventArgs)args).Handled = true;
			EntityUid user = args.User;
			if (CanTransferPopup(ent, user, ref used, transferToBox: false))
			{
				BulletBoxTransferDoAfterEvent ev = new BulletBoxTransferDoAfterEvent(toFrom: false);
				TimeSpan delay = ent.Comp.Delay;
				DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, delay, ev, Entity<BulletBoxComponent>.op_Implicit(ent), Entity<BulletBoxComponent>.op_Implicit(ent), args.Used)
				{
					BreakOnMove = true,
					BreakOnDropItem = true,
					NeedHand = true
				};
				_doAfter.TryStartDoAfter(doAfter);
			}
		}
	}

	private void OnTransferDoAfter(Entity<BulletBoxComponent> ent, ref BulletBoxTransferDoAfterEvent args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid? used = args.Used;
		if (!used.HasValue)
		{
			return;
		}
		EntityUid usedId = used.GetValueOrDefault();
		((HandledEntityEventArgs)args).Handled = true;
		EntityUid user = args.User;
		Entity<RefillableByBulletBoxComponent, BallisticAmmoProviderComponent> used2 = default(Entity<RefillableByBulletBoxComponent, BallisticAmmoProviderComponent>);
		used2._002Ector(usedId, (RefillableByBulletBoxComponent)null, (BallisticAmmoProviderComponent)null);
		bool transferToBox = args.ToBox;
		if (!CanTransferPopup(ent, user, ref used2, transferToBox) || used2.Comp2 == null)
		{
			return;
		}
		int transfer;
		if (!transferToBox)
		{
			transfer = used2.Comp2.Capacity - used2.Comp2.Count;
			if (transfer <= 0)
			{
				return;
			}
			transfer = Math.Min(transfer, ent.Comp.Amount);
			_gun.SetBallisticUnspawned(Entity<BallisticAmmoProviderComponent>.op_Implicit((Entity<RefillableByBulletBoxComponent, BallisticAmmoProviderComponent>.op_Implicit(used2), used2.Comp2)), used2.Comp2.UnspawnedCount + transfer);
			ent.Comp.Amount -= transfer;
		}
		else
		{
			transfer = ent.Comp.Max - ent.Comp.Amount;
			if (transfer <= 0)
			{
				return;
			}
			transfer = Math.Min(transfer, used2.Comp2.Count);
			_gun.SetBallisticUnspawned(Entity<BallisticAmmoProviderComponent>.op_Implicit((Entity<RefillableByBulletBoxComponent, BallisticAmmoProviderComponent>.op_Implicit(used2), used2.Comp2)), used2.Comp2.UnspawnedCount - transfer);
			ent.Comp.Amount += transfer;
		}
		_popup.PopupClient(base.Loc.GetString("rmc-bullet-box-transfer-done", (ValueTuple<string, object>)("amount", transfer), (ValueTuple<string, object>)("used", ent)), Entity<BulletBoxComponent>.op_Implicit(ent), user);
		((EntitySystem)this).Dirty<BulletBoxComponent>(ent, (MetaDataComponent)null);
		UpdateAppearance(ent);
	}

	private bool CanTransferPopup(Entity<BulletBoxComponent> box, EntityUid user, ref Entity<RefillableByBulletBoxComponent?, BallisticAmmoProviderComponent?> used, bool transferToBox)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<RefillableByBulletBoxComponent, BallisticAmmoProviderComponent>(Entity<RefillableByBulletBoxComponent, BallisticAmmoProviderComponent>.op_Implicit(used), ref used.Comp1, ref used.Comp2, false))
		{
			return false;
		}
		string popup = null;
		EntProtoId bulletType = box.Comp.BulletType;
		EntProtoId? bulletType2 = used.Comp1.BulletType;
		if (!bulletType2.HasValue || bulletType != bulletType2.GetValueOrDefault())
		{
			popup = base.Loc.GetString("rmc-bullet-box-wrong-rounds");
		}
		if (!transferToBox)
		{
			if (used.Comp2.Count >= used.Comp2.Capacity)
			{
				popup = base.Loc.GetString("rmc-bullet-box-mag-full");
			}
			if (box.Comp.Amount <= 0)
			{
				popup = base.Loc.GetString("rmc-bullet-box-box-empty");
			}
		}
		else
		{
			if (used.Comp2.Count <= 0)
			{
				popup = base.Loc.GetString("rmc-bullet-box-mag-empty");
			}
			if (box.Comp.Amount >= box.Comp.Max)
			{
				popup = base.Loc.GetString("rmc-bullet-box-box-full");
			}
		}
		if (popup != null)
		{
			_popup.PopupClient(popup, Entity<BulletBoxComponent>.op_Implicit(box), user);
			return false;
		}
		return true;
	}

	public bool TryConsume(Entity<BulletBoxComponent?> box, int amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BulletBoxComponent>(Entity<BulletBoxComponent>.op_Implicit(box), ref box.Comp, false))
		{
			return false;
		}
		if (amount <= 0)
		{
			return true;
		}
		if (box.Comp.Amount < amount)
		{
			return false;
		}
		box.Comp.Amount -= amount;
		((EntitySystem)this).Dirty<BulletBoxComponent>(box, (MetaDataComponent)null);
		UpdateAppearance(Entity<BulletBoxComponent>.op_Implicit((Entity<BulletBoxComponent>.op_Implicit(box), box.Comp)));
		return true;
	}

	public bool TrySetAmount(Entity<BulletBoxComponent?> box, int amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BulletBoxComponent>(Entity<BulletBoxComponent>.op_Implicit(box), ref box.Comp, false))
		{
			return false;
		}
		box.Comp.Amount = Math.Clamp(amount, 0, box.Comp.Max);
		((EntitySystem)this).Dirty<BulletBoxComponent>(box, (MetaDataComponent)null);
		UpdateAppearance(Entity<BulletBoxComponent>.op_Implicit((Entity<BulletBoxComponent>.op_Implicit(box), box.Comp)));
		return true;
	}

	private void UpdateAppearance(Entity<BulletBoxComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		double num = (double)ent.Comp.Amount / (double)ent.Comp.Max;
		BulletBoxVisuals bulletBoxVisuals = ((num >= 1.0) ? BulletBoxVisuals.Full : ((num >= 0.66) ? BulletBoxVisuals.High : ((num >= 0.33) ? BulletBoxVisuals.Medium : ((num > 0.0) ? BulletBoxVisuals.Low : BulletBoxVisuals.Empty))));
		BulletBoxVisuals visual = bulletBoxVisuals;
		_appearance.SetData(Entity<BulletBoxComponent>.op_Implicit(ent), (Enum)BulletBoxLayers.Fill, (object)visual, (AppearanceComponent)null);
	}
}
