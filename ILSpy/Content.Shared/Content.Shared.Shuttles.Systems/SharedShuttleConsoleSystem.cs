using System;
using Content.Shared.ActionBlocker;
using Content.Shared.Movement.Events;
using Content.Shared.Shuttles.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.Systems;

public abstract class SharedShuttleConsoleSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	protected sealed class PilotComponentState : ComponentState
	{
		public NetEntity? Console { get; }

		public PilotComponentState(NetEntity? uid)
		{
			Console = uid;
		}
	}

	[Dependency]
	protected ActionBlockerSystem ActionBlockerSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PilotComponent, UpdateCanMoveEvent>((ComponentEventHandler<PilotComponent, UpdateCanMoveEvent>)HandleMovementBlock, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PilotComponent, ComponentStartup>((ComponentEventHandler<PilotComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PilotComponent, ComponentShutdown>((ComponentEventHandler<PilotComponent, ComponentShutdown>)HandlePilotShutdown, (Type[])null, (Type[])null);
	}

	protected virtual void HandlePilotShutdown(EntityUid uid, PilotComponent component, ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		ActionBlockerSystem.UpdateCanMove(uid);
	}

	private void OnStartup(EntityUid uid, PilotComponent component, ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		ActionBlockerSystem.UpdateCanMove(uid);
	}

	private void HandleMovementBlock(EntityUid uid, PilotComponent component, UpdateCanMoveEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)((Component)component).LifeStage <= 6 && component.Console.HasValue)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}
}
