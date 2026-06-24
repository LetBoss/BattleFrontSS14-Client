using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Roles.FindParasite;

[Serializable]
[NetSerializable]
public sealed class SpawnerData
{
	public string Name;

	public NetEntity Spawner;

	public SpawnerData(string name, NetEntity spawner)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Name = name;
		Spawner = spawner;
	}
}
