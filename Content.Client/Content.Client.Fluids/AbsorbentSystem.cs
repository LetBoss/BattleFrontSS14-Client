using System;
using Content.Client.Fluids.UI;
using Content.Client.Items;
using Content.Shared.Fluids;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client.Fluids;

public sealed class AbsorbentSystem : SharedAbsorbentSystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).Subs.ItemStatus<AbsorbentComponent>((Func<Entity<AbsorbentComponent>, Control?>)((Entity<AbsorbentComponent> ent) => (Control?)(object)new AbsorbentItemStatus(Entity<AbsorbentComponent>.op_Implicit(ent), (IEntityManager)(object)((EntitySystem)this).EntityManager)));
	}
}
