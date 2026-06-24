using System;
using Content.Shared.Nutrition.AnimalHusbandry;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Nutrition.EntitySystems;

public sealed class InfantSystem : EntitySystem
{
	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<InfantComponent, ComponentStartup>((ComponentEventHandler<InfantComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InfantComponent, ComponentShutdown>((ComponentEventHandler<InfantComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
	}

	private void OnStartup(EntityUid uid, InfantComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val))
		{
			component.DefaultScale = val.Scale;
			_sprite.SetScale(Entity<SpriteComponent>.op_Implicit((uid, val)), component.VisualScale);
		}
	}

	private void OnShutdown(EntityUid uid, InfantComponent component, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			_sprite.SetScale(Entity<SpriteComponent>.op_Implicit((uid, item)), component.DefaultScale);
		}
	}
}
