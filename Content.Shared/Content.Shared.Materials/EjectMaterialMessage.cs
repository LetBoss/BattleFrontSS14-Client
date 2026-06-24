using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Materials;

[Serializable]
[NetSerializable]
public sealed class EjectMaterialMessage : EntityEventArgs
{
	public NetEntity Entity;

	public string Material;

	public int SheetsToExtract;

	public EjectMaterialMessage(NetEntity entity, string material, int sheetsToExtract)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Entity = entity;
		Material = material;
		SheetsToExtract = sheetsToExtract;
	}
}
