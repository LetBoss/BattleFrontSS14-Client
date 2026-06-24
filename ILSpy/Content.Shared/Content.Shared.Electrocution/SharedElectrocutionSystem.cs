using System;
using Content.Shared.Inventory;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Electrocution;

public abstract class SharedElectrocutionSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<InsulatedComponent, ElectrocutionAttemptEvent>((ComponentEventHandler<InsulatedComponent, ElectrocutionAttemptEvent>)OnInsulatedElectrocutionAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InsulatedComponent, InventoryRelayedEvent<ElectrocutionAttemptEvent>>((ComponentEventHandler<InsulatedComponent, InventoryRelayedEvent<ElectrocutionAttemptEvent>>)delegate(EntityUid e, InsulatedComponent c, InventoryRelayedEvent<ElectrocutionAttemptEvent> ev)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			OnInsulatedElectrocutionAttempt(e, c, ev.Args);
		}, (Type[])null, (Type[])null);
	}

	public void SetInsulatedSiemensCoefficient(EntityUid uid, float siemensCoefficient, InsulatedComponent? insulated = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<InsulatedComponent>(uid, ref insulated, true))
		{
			insulated.Coefficient = siemensCoefficient;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)insulated, (MetaDataComponent)null);
		}
	}

	public void SetElectrified(Entity<ElectrifiedComponent> ent, bool value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Enabled != value)
		{
			ent.Comp.Enabled = value;
			((EntitySystem)this).Dirty(Entity<ElectrifiedComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp, (MetaDataComponent)null);
			_appearance.SetData(ent.Owner, (Enum)ElectrifiedVisuals.IsElectrified, (object)value, (AppearanceComponent)null);
		}
	}

	public void SetElectrifiedWireCut(Entity<ElectrifiedComponent> ent, bool value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.IsWireCut != value)
		{
			ent.Comp.IsWireCut = value;
			((EntitySystem)this).Dirty<ElectrifiedComponent>(ent, (MetaDataComponent)null);
		}
	}

	public virtual bool TryDoElectrocution(EntityUid uid, EntityUid? sourceUid, int shockDamage, TimeSpan time, bool refresh, float siemensCoefficient = 1f, StatusEffectsComponent? statusEffects = null, bool ignoreInsulation = false)
	{
		return false;
	}

	private void OnInsulatedElectrocutionAttempt(EntityUid uid, InsulatedComponent insulated, ElectrocutionAttemptEvent args)
	{
		args.SiemensCoefficient *= insulated.Coefficient;
	}
}
