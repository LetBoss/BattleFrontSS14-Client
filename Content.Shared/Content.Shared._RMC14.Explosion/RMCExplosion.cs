using System;
using Content.Shared.Explosion;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Explosion;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class RMCExplosion : ISerializationGenerated<RMCExplosion>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public ProtoId<ExplosionPrototype> Type = ProtoId<ExplosionPrototype>.op_Implicit("RMC");

	[DataField(null, false, 1, false, false, null)]
	public float Total;

	[DataField(null, false, 1, false, false, null)]
	public float Slope;

	[DataField(null, false, 1, false, false, null)]
	public float Max;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCExplosion target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<RMCExplosion>(this, ref target, hookCtx, false, context))
		{
			ProtoId<ExplosionPrototype> TypeTemp = default(ProtoId<ExplosionPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<ExplosionPrototype>>(Type, ref TypeTemp, hookCtx, false, context))
			{
				TypeTemp = serialization.CreateCopy<ProtoId<ExplosionPrototype>>(Type, hookCtx, context, false);
			}
			target.Type = TypeTemp;
			float TotalTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Total, ref TotalTemp, hookCtx, false, context))
			{
				TotalTemp = Total;
			}
			target.Total = TotalTemp;
			float SlopeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Slope, ref SlopeTemp, hookCtx, false, context))
			{
				SlopeTemp = Slope;
			}
			target.Slope = SlopeTemp;
			float MaxTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Max, ref MaxTemp, hookCtx, false, context))
			{
				MaxTemp = Max;
			}
			target.Max = MaxTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCExplosion target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCExplosion cast = (RMCExplosion)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public RMCExplosion Instantiate()
	{
		return new RMCExplosion();
	}
}
