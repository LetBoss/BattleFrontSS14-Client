using System;
using Content.Shared._RMC14.Chemistry.SmartFridge;
using Content.Shared._RMC14.UserInterface;
using Robust.Client.Timing;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Client._RMC14.Chemistry.SmartFridge;

public sealed class RMCSmartFridgeSystem : SharedRMCSmartFridgeSystem
{
	[Dependency]
	private RMCUserInterfaceSystem _rmcUI;

	[Dependency]
	private IClientGameTiming _timing;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCSmartFridgeComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<RMCSmartFridgeComponent, AfterAutoHandleStateEvent>)OnState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSmartFridgeComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<RMCSmartFridgeComponent, EntInsertedIntoContainerMessage>)OnInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSmartFridgeComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<RMCSmartFridgeComponent, EntRemovedFromContainerMessage>)OnRemoved, (Type[])null, (Type[])null);
	}

	private void OnState(Entity<RMCSmartFridgeComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!(((IGameTiming)_timing).CurTick != _timing.LastRealTick))
		{
			RefreshUIs(ent);
		}
	}

	private void OnInserted(Entity<RMCSmartFridgeComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RefreshUIs(ent);
	}

	private void OnRemoved(Entity<RMCSmartFridgeComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RefreshUIs(ent);
	}

	private void RefreshUIs(Entity<RMCSmartFridgeComponent> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_rmcUI.RefreshUIs<RMCSmartFridgeBui>(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner));
	}
}
