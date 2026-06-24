using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.CartridgeLoader.Cartridges;

public abstract class SharedNanoTaskCartridgeSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<NanoTaskCartridgeComponent, CartridgeAddedEvent>((EntityEventRefHandler<NanoTaskCartridgeComponent, CartridgeAddedEvent>)OnCartridgeAdded, (Type[])null, (Type[])null);
	}

	private void OnCartridgeAdded(Entity<NanoTaskCartridgeComponent> ent, ref CartridgeAddedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).EnsureComp<NanoTaskInteractionComponent>(args.Loader);
	}
}
