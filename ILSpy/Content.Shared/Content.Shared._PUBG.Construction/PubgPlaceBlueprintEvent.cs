using System.Runtime.CompilerServices;
using Content.Shared._RMC14.Construction.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Shared._PUBG.Construction;

[ByRefEvent]
public readonly record struct PubgPlaceBlueprintEvent(ProtoId<RMCConstructionPrototype> Recipe, EntityUid User, EntityCoordinates Coordinates, Direction Direction)
{
	[CompilerGenerated]
	public void Deconstruct(out ProtoId<RMCConstructionPrototype> Recipe, out EntityUid User, out EntityCoordinates Coordinates, out Direction Direction)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected I4, but got Unknown
		Recipe = this.Recipe;
		User = this.User;
		Coordinates = this.Coordinates;
		Direction = (Direction)(int)this.Direction;
	}
}
