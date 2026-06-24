using System;
using System.Collections.Generic;
using Content.Shared.DisplacementMap;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;

namespace Content.Client.DisplacementMap;

public sealed class DisplacementMapSystem : EntitySystem
{
	[Dependency]
	private ISerializationManager _serialization;

	[Dependency]
	private SpriteSystem _sprite;

	public bool TryAddDisplacement(DisplacementData data, Entity<SpriteComponent> sprite, int index, object key, out string displacementKey)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		displacementKey = $"{key}-displacement";
		if (key.ToString() == null)
		{
			return false;
		}
		if (data.ShaderOverride != null)
		{
			sprite.Comp.LayerSetShader(index, data.ShaderOverride);
		}
		_sprite.RemoveLayer(sprite.AsNullable(), displacementKey, false);
		foreach (KeyValuePair<int, PrototypeLayerData> sizeMap in data.SizeMaps)
		{
			PrototypeLayerData value = sizeMap.Value;
			if (value.CopyToShaderParameters == null)
			{
				value.CopyToShaderParameters = new PrototypeCopyToShaderParameters
				{
					LayerKey = "dummy",
					ParameterTexture = "displacementMap",
					ParameterUV = "displacementUV"
				};
			}
		}
		if (!data.SizeMaps.ContainsKey(32))
		{
			((EntitySystem)this).Log.Error("DISPLACEMENT: " + displacementKey + " don't have 32x32 default displacement map");
			return false;
		}
		PrototypeLayerData val = data.SizeMaps[32];
		RSI val2 = _sprite.LayerGetEffectiveRsi(sprite.AsNullable(), index);
		if (val2 != null)
		{
			if (val2.Size.X != val2.Size.Y)
			{
				((EntitySystem)this).Log.Warning("DISPLACEMENT: " + displacementKey + " has a resolution that is not 1:1, things can look crooked");
			}
			int x = val2.Size.X;
			if (data.SizeMaps.TryGetValue(x, out PrototypeLayerData value2))
			{
				val = value2;
			}
		}
		PrototypeLayerData val3 = _serialization.CreateCopy<PrototypeLayerData>(val, (ISerializationContext)null, false, true);
		val3.CopyToShaderParameters.LayerKey = key.ToString() ?? "this is impossible";
		_sprite.AddLayer(sprite.AsNullable(), val3, (int?)index);
		_sprite.LayerMapSet(sprite.AsNullable(), displacementKey, index);
		return true;
	}

	[Obsolete("Use the Entity<SpriteComponent> overload")]
	public bool TryAddDisplacement(DisplacementData data, SpriteComponent sprite, int index, object key, out string displacementKey)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return TryAddDisplacement(data, Entity<SpriteComponent>.op_Implicit((((Component)sprite).Owner, sprite)), index, key, out displacementKey);
	}
}
