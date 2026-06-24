using System.Collections.Generic;
using Content.Shared.Research.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Shared.Lathe;

public sealed class LatheGetRecipesEvent : EntityEventArgs
{
	public readonly EntityUid Lathe;

	public readonly LatheComponent Comp;

	public bool GetUnavailable;

	public HashSet<ProtoId<LatheRecipePrototype>> Recipes = new HashSet<ProtoId<LatheRecipePrototype>>();

	public LatheGetRecipesEvent(Entity<LatheComponent> lathe, bool forced)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Entity<LatheComponent> val = lathe;
		EntityUid lathe2 = default(EntityUid);
		LatheComponent comp = default(LatheComponent);
		val.Deconstruct(ref lathe2, ref comp);
		Lathe = lathe2;
		Comp = comp;
		GetUnavailable = forced;
	}
}
