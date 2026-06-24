using System;
using System.Numerics;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.WorldEdit;

[Serializable]
[NetSerializable]
public sealed class WorldEditPreviewEntityData
{
	public string PrototypeId { get; }

	public Vector2 RelativePosition { get; }

	public Angle Rotation { get; }

	public WorldEditPreviewEntityData(string prototypeId, Vector2 relativePosition, Angle rotation)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		PrototypeId = prototypeId;
		RelativePosition = relativePosition;
		Rotation = rotation;
	}
}
