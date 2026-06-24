using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

public sealed class CureZombieInfection : EventEntityEffect<CureZombieInfection>, ISerializationGenerated<CureZombieInfection>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool Innoculate;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		if (Innoculate)
		{
			return Loc.GetString("reagent-effect-guidebook-innoculate-zombie-infection", new(string, object)[1] { ("chance", Probability) });
		}
		return Loc.GetString("reagent-effect-guidebook-cure-zombie-infection", new(string, object)[1] { ("chance", Probability) });
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CureZombieInfection target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EventEntityEffect<CureZombieInfection> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CureZombieInfection)definitionCast;
		if (!serialization.TryCustomCopy<CureZombieInfection>(this, ref target, hookCtx, false, context))
		{
			bool InnoculateTemp = false;
			if (!serialization.TryCustomCopy<bool>(Innoculate, ref InnoculateTemp, hookCtx, false, context))
			{
				InnoculateTemp = Innoculate;
			}
			target.Innoculate = InnoculateTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CureZombieInfection target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<CureZombieInfection> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CureZombieInfection cast = (CureZombieInfection)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CureZombieInfection cast = (CureZombieInfection)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CureZombieInfection Instantiate()
	{
		return new CureZombieInfection();
	}
}
