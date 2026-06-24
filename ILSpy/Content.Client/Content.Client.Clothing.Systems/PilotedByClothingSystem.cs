using System;
using Content.Shared.Clothing.Components;
using Robust.Client.Physics;
using Robust.Shared.GameObjects;

namespace Content.Client.Clothing.Systems;

public sealed class PilotedByClothingSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PilotedByClothingComponent, UpdateIsPredictedEvent>((EntityEventRefHandler<PilotedByClothingComponent, UpdateIsPredictedEvent>)OnUpdatePredicted, (Type[])null, (Type[])null);
	}

	private void OnUpdatePredicted(Entity<PilotedByClothingComponent> entity, ref UpdateIsPredictedEvent args)
	{
		args.BlockPrediction = true;
	}
}
