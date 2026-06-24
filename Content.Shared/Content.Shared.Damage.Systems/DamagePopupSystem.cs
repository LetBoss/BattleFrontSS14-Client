using System;
using Content.Shared.Damage.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Damage.Systems;

public sealed class DamagePopupSystem : EntitySystem
{
	[Dependency]
	private SharedPopupSystem _popupSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DamagePopupComponent, DamageChangedEvent>((EntityEventRefHandler<DamagePopupComponent, DamageChangedEvent>)OnDamageChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DamagePopupComponent, InteractHandEvent>((EntityEventRefHandler<DamagePopupComponent, InteractHandEvent>)OnInteractHand, (Type[])null, (Type[])null);
	}

	private void OnDamageChange(Entity<DamagePopupComponent> ent, ref DamageChangedEvent args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		if (args.DamageDelta != null)
		{
			FixedPoint2 damageTotal = args.Damageable.TotalDamage;
			FixedPoint2 damageDelta = args.DamageDelta.GetTotal();
			string msg = ent.Comp.Type switch
			{
				DamagePopupType.Delta => damageDelta.ToString(), 
				DamagePopupType.Total => damageTotal.ToString(), 
				DamagePopupType.Combined => damageDelta.ToString() + " | " + damageTotal, 
				DamagePopupType.Hit => "!", 
				_ => "Invalid type", 
			};
			_popupSystem.PopupPredicted(msg, ent.Owner, args.Origin);
		}
	}

	private void OnInteractHand(Entity<DamagePopupComponent> ent, ref InteractHandEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.AllowTypeChange)
		{
			DamagePopupType next = (DamagePopupType)((int)(ent.Comp.Type + 1) % Enum.GetValues<DamagePopupType>().Length);
			ent.Comp.Type = next;
			((EntitySystem)this).Dirty<DamagePopupComponent>(ent, (MetaDataComponent)null);
			_popupSystem.PopupPredicted(base.Loc.GetString("damage-popup-component-switched", (ValueTuple<string, object>)("setting", ent.Comp.Type)), ent.Owner, args.User);
		}
	}
}
