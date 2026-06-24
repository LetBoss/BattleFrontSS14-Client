using System;
using System.Numerics;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Deploy;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class RMCDeploySetup : ISerializationHooks, ISerializationGenerated<RMCDeploySetup>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public EntProtoId Prototype;

	[DataField(null, false, 1, false, false, null)]
	public RMCDeploySetupMode Mode;

	[DataField(null, false, 1, false, false, null)]
	public bool NeverRedeployableSetup;

	[DataField(null, false, 1, false, false, null)]
	public bool StorageOriginalEntity;

	[DataField(null, false, 1, false, false, null)]
	public Vector2 Offset = Vector2.Zero;

	[DataField(null, false, 1, false, false, null)]
	public float Angle;

	[DataField(null, false, 1, false, false, null)]
	public bool Anchor = true;

	void ISerializationHooks.AfterDeserialization()
	{
		if (StorageOriginalEntity)
		{
			StorageOriginalEntity = false;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCDeploySetup target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<RMCDeploySetup>(this, ref target, hookCtx, true, context))
		{
			EntProtoId PrototypeTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Prototype, ref PrototypeTemp, hookCtx, false, context))
			{
				PrototypeTemp = serialization.CreateCopy<EntProtoId>(Prototype, hookCtx, context, false);
			}
			target.Prototype = PrototypeTemp;
			RMCDeploySetupMode ModeTemp = RMCDeploySetupMode.Default;
			if (!serialization.TryCustomCopy<RMCDeploySetupMode>(Mode, ref ModeTemp, hookCtx, false, context))
			{
				ModeTemp = Mode;
			}
			target.Mode = ModeTemp;
			bool NeverRedeployableSetupTemp = false;
			if (!serialization.TryCustomCopy<bool>(NeverRedeployableSetup, ref NeverRedeployableSetupTemp, hookCtx, false, context))
			{
				NeverRedeployableSetupTemp = NeverRedeployableSetup;
			}
			target.NeverRedeployableSetup = NeverRedeployableSetupTemp;
			bool StorageOriginalEntityTemp = false;
			if (!serialization.TryCustomCopy<bool>(StorageOriginalEntity, ref StorageOriginalEntityTemp, hookCtx, false, context))
			{
				StorageOriginalEntityTemp = StorageOriginalEntity;
			}
			target.StorageOriginalEntity = StorageOriginalEntityTemp;
			Vector2 OffsetTemp = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(Offset, ref OffsetTemp, hookCtx, false, context))
			{
				OffsetTemp = serialization.CreateCopy<Vector2>(Offset, hookCtx, context, false);
			}
			target.Offset = OffsetTemp;
			float AngleTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Angle, ref AngleTemp, hookCtx, false, context))
			{
				AngleTemp = Angle;
			}
			target.Angle = AngleTemp;
			bool AnchorTemp = false;
			if (!serialization.TryCustomCopy<bool>(Anchor, ref AnchorTemp, hookCtx, false, context))
			{
				AnchorTemp = Anchor;
			}
			target.Anchor = AnchorTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCDeploySetup target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCDeploySetup cast = (RMCDeploySetup)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public RMCDeploySetup Instantiate()
	{
		return new RMCDeploySetup();
	}
}
