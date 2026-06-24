using System;
using Content.Shared.Electrocution;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

public sealed class Electrocute : EntityEffect, ISerializationGenerated<Electrocute>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int ElectrocuteTime = 2;

	[DataField(null, false, 1, false, false, null)]
	public int ElectrocuteDamageScale = 5;

	[DataField(null, false, 1, false, false, null)]
	public bool Refresh = true;

	public override bool ShouldLog => true;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-electrocute", new(string, object)[2]
		{
			("chance", Probability),
			("time", ElectrocuteTime)
		});
	}

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (args is EntityEffectReagentArgs reagentArgs)
		{
			reagentArgs.EntityManager.System<SharedElectrocutionSystem>().TryDoElectrocution(reagentArgs.TargetEntity, null, Math.Max((reagentArgs.Quantity * ElectrocuteDamageScale).Int(), 1), TimeSpan.FromSeconds(ElectrocuteTime), Refresh, 1f, null, ignoreInsulation: true);
			if (reagentArgs.Reagent != null)
			{
				reagentArgs.Source?.RemoveReagent(reagentArgs.Reagent.ID, reagentArgs.Quantity);
			}
		}
		else
		{
			args.EntityManager.System<SharedElectrocutionSystem>().TryDoElectrocution(args.TargetEntity, null, Math.Max(ElectrocuteDamageScale, 1), TimeSpan.FromSeconds(ElectrocuteTime), Refresh, 1f, null, ignoreInsulation: true);
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Electrocute target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Electrocute)definitionCast;
		if (!serialization.TryCustomCopy<Electrocute>(this, ref target, hookCtx, false, context))
		{
			int ElectrocuteTimeTemp = 0;
			if (!serialization.TryCustomCopy<int>(ElectrocuteTime, ref ElectrocuteTimeTemp, hookCtx, false, context))
			{
				ElectrocuteTimeTemp = ElectrocuteTime;
			}
			target.ElectrocuteTime = ElectrocuteTimeTemp;
			int ElectrocuteDamageScaleTemp = 0;
			if (!serialization.TryCustomCopy<int>(ElectrocuteDamageScale, ref ElectrocuteDamageScaleTemp, hookCtx, false, context))
			{
				ElectrocuteDamageScaleTemp = ElectrocuteDamageScale;
			}
			target.ElectrocuteDamageScale = ElectrocuteDamageScaleTemp;
			bool RefreshTemp = false;
			if (!serialization.TryCustomCopy<bool>(Refresh, ref RefreshTemp, hookCtx, false, context))
			{
				RefreshTemp = Refresh;
			}
			target.Refresh = RefreshTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Electrocute target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Electrocute cast = (Electrocute)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Electrocute cast = (Electrocute)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Electrocute Instantiate()
	{
		return new Electrocute();
	}
}
