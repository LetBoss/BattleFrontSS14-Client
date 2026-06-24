using System;
using Content.Shared.Interaction.Events;
using Robust.Shared.GameObjects;

namespace Content.Shared._PUBG.Structures;

public sealed class NoRotateSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<NoRotateComponent, ChangeDirectionAttemptEvent>((ComponentEventHandler<NoRotateComponent, ChangeDirectionAttemptEvent>)OnChangeDirectionAttempt, (Type[])null, (Type[])null);
	}

	private void OnChangeDirectionAttempt(EntityUid uid, NoRotateComponent component, ChangeDirectionAttemptEvent args)
	{
		((CancellableEntityEventArgs)args).Cancel();
	}
}
