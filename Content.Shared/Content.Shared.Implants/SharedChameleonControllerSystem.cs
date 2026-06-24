using System;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Implants;

public abstract class SharedChameleonControllerSystem : EntitySystem
{
	[Dependency]
	private SharedUserInterfaceSystem _uiSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ChameleonControllerOpenMenuEvent>((EntityEventHandler<ChameleonControllerOpenMenuEvent>)OpenUI, (Type[])null, (Type[])null);
	}

	private void OpenUI(ChameleonControllerOpenMenuEvent ev)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? implant = ev.Action.Comp.Container;
		if (((EntitySystem)this).HasComp<ChameleonControllerImplantComponent>(implant) && _uiSystem.HasUi(implant.Value, (Enum)ChameleonControllerKey.Key, (UserInterfaceComponent)null))
		{
			_uiSystem.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(implant.Value), (Enum)ChameleonControllerKey.Key, (EntityUid?)ev.Performer, false);
		}
	}
}
