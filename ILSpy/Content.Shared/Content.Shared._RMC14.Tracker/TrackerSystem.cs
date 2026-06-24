using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Movement.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Shared._RMC14.Tracker;

public sealed class TrackerSystem : EntitySystem
{
	[Dependency]
	private SharedTransformSystem _transform;

	public static readonly short CenterSeverity = 1;

	private static readonly Dictionary<Direction, short> AlertSeverity = new Dictionary<Direction, short>
	{
		{
			(Direction)(-1),
			0
		},
		{
			(Direction)0,
			2
		},
		{
			(Direction)1,
			3
		},
		{
			(Direction)2,
			4
		},
		{
			(Direction)3,
			5
		},
		{
			(Direction)4,
			6
		},
		{
			(Direction)5,
			7
		},
		{
			(Direction)6,
			8
		},
		{
			(Direction)7,
			9
		}
	};

	private EntityQuery<InputMoverComponent> _inputMoverQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_inputMoverQuery = ((EntitySystem)this).GetEntityQuery<InputMoverComponent>();
	}

	public short GetAlertSeverity(EntityUid ent, MapCoordinates tracked)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		MapCoordinates pos = _transform.GetMapCoordinates(ent, (TransformComponent)null);
		if (pos.MapId != tracked.MapId)
		{
			return CenterSeverity;
		}
		Vector2 vec = tracked.Position - pos.Position;
		if (vec.Length() < 1f)
		{
			return CenterSeverity;
		}
		InputMoverComponent inputMover = default(InputMoverComponent);
		Angle val;
		if (_inputMoverQuery.TryComp(ent, ref inputMover) && inputMover.RelativeRotation != Angle.Zero)
		{
			val = -inputMover.RelativeRotation;
			vec = ((Angle)(ref val)).RotateVec(ref vec);
		}
		val = DirectionExtensions.ToWorldAngle(vec);
		Direction dir = ((Angle)(ref val)).GetDir();
		return AlertSeverity.GetValueOrDefault(dir, CenterSeverity);
	}
}
