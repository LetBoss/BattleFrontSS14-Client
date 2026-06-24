using System;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Humanoid;

[Serializable]
[DataDefinition]
[NetSerializable]
public readonly struct CustomBaseLayerInfo : ISerializationGenerated<CustomBaseLayerInfo>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public ProtoId<HumanoidSpeciesSpriteLayer>? Id { get; init; }

	[DataField(null, false, 1, false, false, null)]
	public Color? Color { get; init; }

	public CustomBaseLayerInfo(string? id, Color? color = null)
	{
		Id = ProtoId<HumanoidSpeciesSpriteLayer>.op_Implicit(id);
		Color = color;
	}

	public CustomBaseLayerInfo()
	{
		Id = null;
		Color = null;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CustomBaseLayerInfo target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<CustomBaseLayerInfo>(this, ref target, hookCtx, false, context))
		{
			ProtoId<HumanoidSpeciesSpriteLayer>? IdTemp = null;
			if (!serialization.TryCustomCopy<ProtoId<HumanoidSpeciesSpriteLayer>?>(Id, ref IdTemp, hookCtx, false, context))
			{
				IdTemp = serialization.CreateCopy<ProtoId<HumanoidSpeciesSpriteLayer>?>(Id, hookCtx, context, false);
			}
			Color? ColorTemp = null;
			if (!serialization.TryCustomCopy<Color?>(Color, ref ColorTemp, hookCtx, false, context))
			{
				ColorTemp = serialization.CreateCopy<Color?>(Color, hookCtx, context, false);
			}
			CustomBaseLayerInfo customBaseLayerInfo = target;
			customBaseLayerInfo.Id = IdTemp;
			customBaseLayerInfo.Color = ColorTemp;
			target = customBaseLayerInfo;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CustomBaseLayerInfo target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CustomBaseLayerInfo cast = (CustomBaseLayerInfo)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public CustomBaseLayerInfo Instantiate()
	{
		return new CustomBaseLayerInfo();
	}
}
