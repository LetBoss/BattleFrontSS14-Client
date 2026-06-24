using System;
using Content.Client.Chemistry.Components;
using Content.Client.Chemistry.UI;
using Content.Client.Items;
using Content.Shared.Chemistry.EntitySystems;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Chemistry.EntitySystems;

public sealed class SolutionItemStatusSystem : EntitySystem
{
	[Dependency]
	private SharedSolutionContainerSystem _solutionContainerSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).Subs.ItemStatus<SolutionItemStatusComponent>((Func<Entity<SolutionItemStatusComponent>, Control?>)((Entity<SolutionItemStatusComponent> entity) => (Control?)(object)new SolutionStatusControl(entity, (IEntityManager)(object)base.EntityManager, _solutionContainerSystem)));
	}
}
