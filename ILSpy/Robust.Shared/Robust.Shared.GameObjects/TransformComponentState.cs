using System;
using System.Numerics;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Robust.Shared.GameObjects;

[Serializable]
[NetSerializable]
internal readonly record struct TransformComponentState : IComponentState
{
	public readonly NetEntity ParentID;

	public readonly Vector2 LocalPosition;

	public readonly Angle Rotation;

	public readonly bool NoLocalRotation;

	public readonly bool Anchored;

	public TransformComponentState(Vector2 localPosition, Angle rotation, NetEntity parentId, bool noLocalRotation, bool anchored)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		LocalPosition = localPosition;
		Rotation = rotation;
		ParentID = parentId;
		NoLocalRotation = noLocalRotation;
		Anchored = anchored;
	}
}
