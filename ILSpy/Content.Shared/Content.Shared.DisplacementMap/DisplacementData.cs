using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.DisplacementMap;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class DisplacementData : ISerializationGenerated<DisplacementData>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public Dictionary<int, PrototypeLayerData> SizeMaps = new Dictionary<int, PrototypeLayerData>();

	[DataField(null, false, 1, false, false, null)]
	public string? ShaderOverride = "DisplacedDraw";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DisplacementData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<DisplacementData>(this, ref target, hookCtx, false, context))
		{
			Dictionary<int, PrototypeLayerData> SizeMapsTemp = null;
			if (SizeMaps == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<int, PrototypeLayerData>>(SizeMaps, ref SizeMapsTemp, hookCtx, true, context))
			{
				SizeMapsTemp = serialization.CreateCopy<Dictionary<int, PrototypeLayerData>>(SizeMaps, hookCtx, context, false);
			}
			target.SizeMaps = SizeMapsTemp;
			string ShaderOverrideTemp = null;
			if (!serialization.TryCustomCopy<string>(ShaderOverride, ref ShaderOverrideTemp, hookCtx, false, context))
			{
				ShaderOverrideTemp = ShaderOverride;
			}
			target.ShaderOverride = ShaderOverrideTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DisplacementData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DisplacementData cast = (DisplacementData)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public DisplacementData Instantiate()
	{
		return new DisplacementData();
	}
}
