using System.Collections.Generic;
using Content.Shared.Physics;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.RCD;

[Prototype("rcd", 1)]
public sealed class RCDPrototype : IPrototype
{
	private Box2? _collisionBounds;

	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, true, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public RcdMode Mode { get; private set; }

	[DataField("name", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string SetName { get; private set; } = "Unknown";

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string Category { get; private set; } = "Undefined";

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public SpriteSpecifier? Sprite { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string? Prototype { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int Cost { get; private set; } = 1;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float Delay { get; private set; } = 1f;

	[DataField("fx", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public EntProtoId? Effect { get; private set; }

	[DataField("rules", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public HashSet<RcdConstructionRule> ConstructionRules { get; private set; } = new HashSet<RcdConstructionRule>();

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public CollisionGroup CollisionMask { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Box2? CollisionBounds
	{
		get
		{
			return _collisionBounds;
		}
		private set
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			_collisionBounds = value;
			if (_collisionBounds.HasValue)
			{
				PolygonShape poly = new PolygonShape();
				poly.SetAsBox(_collisionBounds.Value);
				CollisionPolygon = poly;
			}
		}
	}

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public PolygonShape? CollisionPolygon { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public RcdRotation Rotation { get; private set; } = RcdRotation.User;
}
