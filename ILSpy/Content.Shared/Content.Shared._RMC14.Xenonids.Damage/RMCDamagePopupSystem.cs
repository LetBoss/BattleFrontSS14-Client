using System;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Shared._RMC14.Xenonids.Damage;

public sealed class RMCDamagePopupSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<DamagePopupComponent, DamageDealtEvent>((EntityEventRefHandler<DamagePopupComponent, DamageDealtEvent>)OnDamagePopup, (Type[])null, (Type[])null);
	}

	private void OnDamagePopup(Entity<DamagePopupComponent> ent, ref DamageDealtEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		DamageableComponent damageable = default(DamageableComponent);
		if (((EntitySystem)this).TryComp<DamageableComponent>(Entity<DamagePopupComponent>.op_Implicit(ent), ref damageable))
		{
			ShowClientDamagePopup(Entity<DamagePopupComponent>.op_Implicit(ent), damageable.TotalDamage, ent.Comp.Type, args.Origin, args.DamageDelta);
		}
	}

	private void ShowClientDamagePopup(EntityUid target, FixedPoint2 damageTotal, DamagePopupType type, EntityUid? origin, DamageSpecifier? damageDelta)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		if (damageDelta != null)
		{
			FixedPoint2 delta = damageDelta.GetTotal();
			string msg = type switch
			{
				DamagePopupType.Delta => delta.ToString(), 
				DamagePopupType.Total => damageTotal.ToString(), 
				DamagePopupType.Combined => delta.ToString() + " | " + damageTotal, 
				DamagePopupType.Hit => "!", 
				_ => "Invalid type", 
			};
			if (origin.HasValue && _net.IsServer)
			{
				_popupSystem.PopupEntity(msg, target, origin.Value);
			}
		}
	}
}
