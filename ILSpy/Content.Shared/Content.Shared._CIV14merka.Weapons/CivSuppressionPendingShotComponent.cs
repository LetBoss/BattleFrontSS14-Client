using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._CIV14merka.Weapons;

[RegisterComponent]
[Access(new Type[] { typeof(SharedCivSuppressionSystem) })]
public sealed class CivSuppressionPendingShotComponent : Component, ISerializationGenerated<CivSuppressionPendingShotComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntityUid? Shooter;

	[DataField(null, false, 1, false, false, null)]
	public float Intensity;

	[DataField(null, false, 1, false, false, null)]
	public float ShotPenaltyDegrees;

	[DataField(null, false, 1, false, false, null)]
	public float HighStressThreshold;

	[DataField(null, false, 1, false, false, null)]
	public float HighStressShotPenaltyMultiplier;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CivSuppressionPendingShotComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CivSuppressionPendingShotComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<CivSuppressionPendingShotComponent>(this, ref target, hookCtx, false, context))
		{
			EntityUid? ShooterTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(Shooter, ref ShooterTemp, hookCtx, false, context))
			{
				ShooterTemp = serialization.CreateCopy<EntityUid?>(Shooter, hookCtx, context, false);
			}
			target.Shooter = ShooterTemp;
			float IntensityTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Intensity, ref IntensityTemp, hookCtx, false, context))
			{
				IntensityTemp = Intensity;
			}
			target.Intensity = IntensityTemp;
			float ShotPenaltyDegreesTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ShotPenaltyDegrees, ref ShotPenaltyDegreesTemp, hookCtx, false, context))
			{
				ShotPenaltyDegreesTemp = ShotPenaltyDegrees;
			}
			target.ShotPenaltyDegrees = ShotPenaltyDegreesTemp;
			float HighStressThresholdTemp = 0f;
			if (!serialization.TryCustomCopy<float>(HighStressThreshold, ref HighStressThresholdTemp, hookCtx, false, context))
			{
				HighStressThresholdTemp = HighStressThreshold;
			}
			target.HighStressThreshold = HighStressThresholdTemp;
			float HighStressShotPenaltyMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(HighStressShotPenaltyMultiplier, ref HighStressShotPenaltyMultiplierTemp, hookCtx, false, context))
			{
				HighStressShotPenaltyMultiplierTemp = HighStressShotPenaltyMultiplier;
			}
			target.HighStressShotPenaltyMultiplier = HighStressShotPenaltyMultiplierTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CivSuppressionPendingShotComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivSuppressionPendingShotComponent cast = (CivSuppressionPendingShotComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivSuppressionPendingShotComponent cast = (CivSuppressionPendingShotComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivSuppressionPendingShotComponent def = (CivSuppressionPendingShotComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CivSuppressionPendingShotComponent Instantiate()
	{
		return new CivSuppressionPendingShotComponent();
	}
}
