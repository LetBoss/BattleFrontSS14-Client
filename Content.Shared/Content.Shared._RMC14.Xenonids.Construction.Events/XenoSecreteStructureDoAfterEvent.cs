using System;
using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Construction.Events;

[Serializable]
[NetSerializable]
public sealed class XenoSecreteStructureDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<XenoSecreteStructureDoAfterEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public NetCoordinates Coordinates;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId StructureId = EntProtoId.op_Implicit("WallXenoResin");

	[DataField(null, false, 1, false, false, null)]
	public NetEntity? Effect;

	public XenoSecreteStructureDoAfterEvent(NetCoordinates coordinates, EntProtoId structureId, NetEntity? effect = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		Coordinates = coordinates;
		StructureId = structureId;
		Effect = effect;
	}

	public XenoSecreteStructureDoAfterEvent()
	{
	}//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	//IL_000b: Unknown result type (might be due to invalid IL or missing references)


	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoSecreteStructureDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoSecreteStructureDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<XenoSecreteStructureDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			NetCoordinates CoordinatesTemp = default(NetCoordinates);
			if (!serialization.TryCustomCopy<NetCoordinates>(Coordinates, ref CoordinatesTemp, hookCtx, false, context))
			{
				CoordinatesTemp = serialization.CreateCopy<NetCoordinates>(Coordinates, hookCtx, context, false);
			}
			target.Coordinates = CoordinatesTemp;
			EntProtoId StructureIdTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(StructureId, ref StructureIdTemp, hookCtx, false, context))
			{
				StructureIdTemp = serialization.CreateCopy<EntProtoId>(StructureId, hookCtx, context, false);
			}
			target.StructureId = StructureIdTemp;
			NetEntity? EffectTemp = null;
			if (!serialization.TryCustomCopy<NetEntity?>(Effect, ref EffectTemp, hookCtx, false, context))
			{
				EffectTemp = serialization.CreateCopy<NetEntity?>(Effect, hookCtx, context, false);
			}
			target.Effect = EffectTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoSecreteStructureDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoSecreteStructureDoAfterEvent cast = (XenoSecreteStructureDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoSecreteStructureDoAfterEvent cast = (XenoSecreteStructureDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoSecreteStructureDoAfterEvent Instantiate()
	{
		return new XenoSecreteStructureDoAfterEvent();
	}
}
