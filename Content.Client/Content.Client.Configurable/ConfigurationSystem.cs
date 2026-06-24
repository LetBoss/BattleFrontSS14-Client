using System;
using Content.Client.Configurable.UI;
using Content.Shared.Configurable;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Configurable;

public sealed class ConfigurationSystem : SharedConfigurationSystem
{
	[Dependency]
	private SharedUserInterfaceSystem _uiSystem;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ConfigurationComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<ConfigurationComponent, AfterAutoHandleStateEvent>)OnConfigurationState, (Type[])null, (Type[])null);
	}

	private void OnConfigurationState(Entity<ConfigurationComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		ConfigurationBoundUserInterface configurationBoundUserInterface = default(ConfigurationBoundUserInterface);
		if (_uiSystem.TryGetOpenUi<ConfigurationBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)ConfigurationComponent.ConfigurationUiKey.Key, ref configurationBoundUserInterface))
		{
			configurationBoundUserInterface.Refresh(ent);
		}
	}
}
