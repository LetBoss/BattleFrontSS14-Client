using System;
using Content.Client.Chemistry.UI;
using Content.Client.Items;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Chemistry.EntitySystems;

public sealed class HyposprayStatusControlSystem : EntitySystem
{
	[Dependency]
	private SharedSolutionContainerSystem _solutionContainers;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).Subs.ItemStatus<HyposprayComponent>((Func<Entity<HyposprayComponent>, Control?>)((Entity<HyposprayComponent> ent) => (Control?)(object)new HyposprayStatusControl(ent, _solutionContainers)));
	}
}
