using System;
using Content.Shared.Database;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

public sealed class FlammableReaction : EventEntityEffect<FlammableReaction>, ISerializationGenerated<FlammableReaction>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Multiplier = 0.05f;

	[DataField(null, false, 1, false, false, null)]
	public float MultiplierOnExisting = -1f;

	public override bool ShouldLog => true;

	public override LogImpact LogImpact => LogImpact.Medium;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-flammable-reaction", new(string, object)[1] { ("chance", Probability) });
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FlammableReaction target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EventEntityEffect<FlammableReaction> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (FlammableReaction)definitionCast;
		if (!serialization.TryCustomCopy<FlammableReaction>(this, ref target, hookCtx, false, context))
		{
			float MultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Multiplier, ref MultiplierTemp, hookCtx, false, context))
			{
				MultiplierTemp = Multiplier;
			}
			target.Multiplier = MultiplierTemp;
			float MultiplierOnExistingTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MultiplierOnExisting, ref MultiplierOnExistingTemp, hookCtx, false, context))
			{
				MultiplierOnExistingTemp = MultiplierOnExisting;
			}
			target.MultiplierOnExisting = MultiplierOnExistingTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FlammableReaction target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<FlammableReaction> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FlammableReaction cast = (FlammableReaction)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FlammableReaction cast = (FlammableReaction)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FlammableReaction Instantiate()
	{
		return new FlammableReaction();
	}
}
