using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.Machines.Components;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class MachinePart : ISerializationGenerated<MachinePart>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, typeof(ComponentNameSerializer))]
	public string Component = "";

	[DataField(null, false, 1, true, false, null)]
	public Vector2i Offset;

	[DataField(null, false, 1, false, false, null)]
	public bool Optional;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId? GhostProto;

	[DataField(null, false, 1, false, false, null)]
	public Angle Rotation = Angle.Zero;

	public NetEntity? NetEntity;

	[NonSerialized]
	[DataField(null, false, 1, false, false, null)]
	public EntityUid? Entity;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId Graph;

	[DataField(null, false, 1, false, false, null)]
	public string ExpectedNode;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MachinePart target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<MachinePart>(this, ref target, hookCtx, false, context))
		{
			string ComponentTemp = null;
			if (Component == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Component, ref ComponentTemp, hookCtx, false, context))
			{
				ComponentTemp = Component;
			}
			target.Component = ComponentTemp;
			Vector2i OffsetTemp = default(Vector2i);
			if (!serialization.TryCustomCopy<Vector2i>(Offset, ref OffsetTemp, hookCtx, false, context))
			{
				OffsetTemp = serialization.CreateCopy<Vector2i>(Offset, hookCtx, context, false);
			}
			target.Offset = OffsetTemp;
			bool OptionalTemp = false;
			if (!serialization.TryCustomCopy<bool>(Optional, ref OptionalTemp, hookCtx, false, context))
			{
				OptionalTemp = Optional;
			}
			target.Optional = OptionalTemp;
			EntProtoId? GhostProtoTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId?>(GhostProto, ref GhostProtoTemp, hookCtx, false, context))
			{
				GhostProtoTemp = serialization.CreateCopy<EntProtoId?>(GhostProto, hookCtx, context, false);
			}
			target.GhostProto = GhostProtoTemp;
			Angle RotationTemp = default(Angle);
			if (!serialization.TryCustomCopy<Angle>(Rotation, ref RotationTemp, hookCtx, false, context))
			{
				RotationTemp = serialization.CreateCopy<Angle>(Rotation, hookCtx, context, false);
			}
			target.Rotation = RotationTemp;
			EntityUid? EntityTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(Entity, ref EntityTemp, hookCtx, false, context))
			{
				EntityTemp = serialization.CreateCopy<EntityUid?>(Entity, hookCtx, context, false);
			}
			target.Entity = EntityTemp;
			EntProtoId GraphTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Graph, ref GraphTemp, hookCtx, false, context))
			{
				GraphTemp = serialization.CreateCopy<EntProtoId>(Graph, hookCtx, context, false);
			}
			target.Graph = GraphTemp;
			string ExpectedNodeTemp = null;
			if (ExpectedNode == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ExpectedNode, ref ExpectedNodeTemp, hookCtx, false, context))
			{
				ExpectedNodeTemp = ExpectedNode;
			}
			target.ExpectedNode = ExpectedNodeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MachinePart target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MachinePart cast = (MachinePart)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public MachinePart Instantiate()
	{
		return new MachinePart();
	}
}
