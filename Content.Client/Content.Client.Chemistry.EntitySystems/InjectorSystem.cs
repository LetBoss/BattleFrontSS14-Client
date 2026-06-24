using System;
using Content.Client.Chemistry.UI;
using Content.Client.Items;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client.Chemistry.EntitySystems;

public sealed class InjectorSystem : SharedInjectorSystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).Subs.ItemStatus<InjectorComponent>((Func<Entity<InjectorComponent>, Control?>)((Entity<InjectorComponent> ent) => (Control?)(object)new InjectorStatusControl(ent, SolutionContainers)));
	}
}
