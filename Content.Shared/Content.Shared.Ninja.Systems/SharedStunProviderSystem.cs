using Content.Shared.Ninja.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Ninja.Systems;

public abstract class SharedStunProviderSystem : EntitySystem
{
	public void SetBattery(Entity<StunProviderComponent?> ent, EntityUid? battery)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<StunProviderComponent>(Entity<StunProviderComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			EntityUid? batteryUid = ent.Comp.BatteryUid;
			EntityUid? val = battery;
			if (batteryUid.HasValue != val.HasValue || (batteryUid.HasValue && !(batteryUid.GetValueOrDefault() == val.GetValueOrDefault())))
			{
				ent.Comp.BatteryUid = battery;
				((EntitySystem)this).Dirty(Entity<StunProviderComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp, (MetaDataComponent)null);
			}
		}
	}
}
