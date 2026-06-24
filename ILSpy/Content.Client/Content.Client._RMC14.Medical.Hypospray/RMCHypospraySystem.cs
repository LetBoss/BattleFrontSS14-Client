using System;
using Content.Client.Items;
using Content.Shared._RMC14.Chemistry;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Medical.Hypospray;

public sealed class RMCHypospraySystem : RMCSharedHypospraySystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).Subs.ItemStatus<RMCHyposprayComponent>((Func<Entity<RMCHyposprayComponent>, Control?>)((Entity<RMCHyposprayComponent> ent) => (Control?)(object)new RMCHyposprayStatusControl(ent, _solution, _container)));
	}
}
