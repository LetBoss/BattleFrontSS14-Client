using System;
using Content.Shared.Disposal.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Disposal;

public sealed class CMDisposalSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<UndisposableComponent, ContainerGettingInsertedAttemptEvent>((EntityEventRefHandler<UndisposableComponent, ContainerGettingInsertedAttemptEvent>)OnUndisposableInsertedAttempt, (Type[])null, (Type[])null);
	}

	private void OnUndisposableInsertedAttempt(Entity<UndisposableComponent> ent, ref ContainerGettingInsertedAttemptEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		DisposalUnitComponent unit = default(DisposalUnitComponent);
		if (((EntitySystem)this).TryComp<DisposalUnitComponent>(((ContainerAttemptEventBase)args).Container.Owner, ref unit) && ((ContainerAttemptEventBase)args).Container.ID == ((BaseContainer)unit.Container).ID)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}
}
