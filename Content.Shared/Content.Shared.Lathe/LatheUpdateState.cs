using System;
using System.Collections.Generic;
using Content.Shared.Research.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Lathe;

[Serializable]
[NetSerializable]
public sealed class LatheUpdateState : BoundUserInterfaceState
{
	public List<ProtoId<LatheRecipePrototype>> Recipes;

	public ProtoId<LatheRecipePrototype>[] Queue;

	public ProtoId<LatheRecipePrototype>? CurrentlyProducing;

	public LatheUpdateState(List<ProtoId<LatheRecipePrototype>> recipes, ProtoId<LatheRecipePrototype>[] queue, ProtoId<LatheRecipePrototype>? currentlyProducing = null)
	{
		Recipes = recipes;
		Queue = queue;
		CurrentlyProducing = currentlyProducing;
	}
}
