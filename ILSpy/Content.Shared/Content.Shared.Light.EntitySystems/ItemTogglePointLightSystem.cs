using System;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Light.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Light.EntitySystems;

public sealed class ItemTogglePointLightSystem : EntitySystem
{
	[Dependency]
	private SharedPointLightSystem _light;

	[Dependency]
	private SharedHandheldLightSystem _handheldLight;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ItemTogglePointLightComponent, ItemToggledEvent>((EntityEventRefHandler<ItemTogglePointLightComponent, ItemToggledEvent>)OnLightToggled, (Type[])null, (Type[])null);
	}

	private void OnLightToggled(Entity<ItemTogglePointLightComponent> ent, ref ItemToggledEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		SharedPointLightComponent light = default(SharedPointLightComponent);
		if (_light.TryGetLight(ent.Owner, ref light))
		{
			_light.SetEnabled(ent.Owner, args.Activated, light, (MetaDataComponent)null);
			HandheldLightComponent handheldLight = default(HandheldLightComponent);
			if (((EntitySystem)this).TryComp<HandheldLightComponent>(ent.Owner, ref handheldLight))
			{
				_handheldLight.SetActivated(ent.Owner, args.Activated, handheldLight);
			}
		}
	}
}
