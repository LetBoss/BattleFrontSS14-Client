using System;
using Content.Shared._RMC14.Marines.Access;
using Content.Shared._RMC14.UserInterface;
using Robust.Client.Timing;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Client._RMC14.Access;

public sealed class IdModificationConsoleUISystem : EntitySystem
{
	[Dependency]
	private RMCUserInterfaceSystem _rmcUI;

	[Dependency]
	private IClientGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<IdModificationConsoleComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<IdModificationConsoleComponent, AfterAutoHandleStateEvent>)OnState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IdModificationConsoleComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<IdModificationConsoleComponent, EntInsertedIntoContainerMessage>)OnInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IdModificationConsoleComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<IdModificationConsoleComponent, EntRemovedFromContainerMessage>)OnRemoved, (Type[])null, (Type[])null);
	}

	private void OnState(Entity<IdModificationConsoleComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!(((IGameTiming)_timing).CurTick != _timing.LastRealTick))
		{
			RefreshUIs(ent);
		}
	}

	private void OnInserted(Entity<IdModificationConsoleComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RefreshUIs(ent);
	}

	private void OnRemoved(Entity<IdModificationConsoleComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RefreshUIs(ent);
	}

	private void RefreshUIs(Entity<IdModificationConsoleComponent> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_rmcUI.RefreshUIs<IdModificationConsoleBui>(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner));
	}
}
