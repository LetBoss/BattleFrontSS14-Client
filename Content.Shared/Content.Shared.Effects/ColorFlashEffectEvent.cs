using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Effects;

[Serializable]
[NetSerializable]
public sealed class ColorFlashEffectEvent : EntityEventArgs
{
	public Color Color;

	public List<NetEntity> Entities;

	public ColorFlashEffectEvent(Color color, List<NetEntity> entities)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Color = color;
		Entities = entities;
	}
}
