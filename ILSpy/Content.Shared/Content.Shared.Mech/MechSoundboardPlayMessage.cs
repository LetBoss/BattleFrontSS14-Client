using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Mech;

[Serializable]
[NetSerializable]
public sealed class MechSoundboardPlayMessage : MechEquipmentUiMessage
{
	public int Sound;

	public MechSoundboardPlayMessage(NetEntity equipment, int sound)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Equipment = equipment;
		Sound = sound;
	}
}
