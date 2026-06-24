using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

public sealed class ResetNarcolepsy : EventEntityEffect<ResetNarcolepsy>, ISerializationGenerated<ResetNarcolepsy>, ISerializationGenerated
{
	[DataField("TimerReset", false, 1, false, false, null)]
	public int TimerReset = 600;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-reset-narcolepsy", new(string, object)[1] { ("chance", Probability) });
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ResetNarcolepsy target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EventEntityEffect<ResetNarcolepsy> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ResetNarcolepsy)definitionCast;
		if (!serialization.TryCustomCopy<ResetNarcolepsy>(this, ref target, hookCtx, false, context))
		{
			int TimerResetTemp = 0;
			if (!serialization.TryCustomCopy<int>(TimerReset, ref TimerResetTemp, hookCtx, false, context))
			{
				TimerResetTemp = TimerReset;
			}
			target.TimerReset = TimerResetTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ResetNarcolepsy target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<ResetNarcolepsy> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ResetNarcolepsy cast = (ResetNarcolepsy)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ResetNarcolepsy cast = (ResetNarcolepsy)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ResetNarcolepsy Instantiate()
	{
		return new ResetNarcolepsy();
	}
}
