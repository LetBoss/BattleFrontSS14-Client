using System;
using Content.Shared.Power.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Power.EntitySystems;

public abstract class SharedPowerNetSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	public abstract bool IsPoweredCalculate(SharedApcPowerReceiverComponent comp);

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AppearanceComponent, PowerChangedEvent>((EntityEventRefHandler<AppearanceComponent, PowerChangedEvent>)OnPowerAppearance, (Type[])null, (Type[])null);
	}

	private void OnPowerAppearance(Entity<AppearanceComponent> ent, ref PowerChangedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(Entity<AppearanceComponent>.op_Implicit(ent), (Enum)PowerDeviceVisuals.Powered, (object)args.Powered, ent.Comp);
	}
}
