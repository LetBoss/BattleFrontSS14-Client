using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Storage;

[Serializable]
[NetSerializable]
public sealed class AnimateInsertingEntitiesEvent : EntityEventArgs
{
	public readonly NetEntity Storage;

	public readonly List<NetEntity> StoredEntities;

	public readonly List<NetCoordinates> EntityPositions;

	public readonly List<Angle> EntityAngles;

	public AnimateInsertingEntitiesEvent(NetEntity storage, List<NetEntity> storedEntities, List<NetCoordinates> entityPositions, List<Angle> entityAngles)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Storage = storage;
		StoredEntities = storedEntities;
		EntityPositions = entityPositions;
		EntityAngles = entityAngles;
	}
}
