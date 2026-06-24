using System;
using Content.Shared.Damage;
using Content.Shared.Fax.Components;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Fax.Systems;

public sealed class FaxecuteSystem : EntitySystem
{
	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
	}

	public void Faxecute(EntityUid uid, FaxMachineComponent component, DamageOnFaxecuteEvent? args = null)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? sendEntity = component.PaperSlot.Item;
		FaxecuteComponent faxecute = default(FaxecuteComponent);
		if (sendEntity.HasValue && ((EntitySystem)this).TryComp<FaxecuteComponent>(uid, ref faxecute))
		{
			DamageSpecifier damageSpec = faxecute.Damage;
			_damageable.TryChangeDamage(sendEntity, damageSpec);
			_popupSystem.PopupEntity(base.Loc.GetString("fax-machine-popup-error", (ValueTuple<string, object>)("target", uid)), uid, PopupType.LargeCaution);
		}
	}
}
