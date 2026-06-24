using System;
using Content.Shared.Stunnable;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

public sealed class Paralyze : EntityEffect, ISerializationGenerated<Paralyze>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public double ParalyzeTime = 2.0;

	[DataField(null, false, 1, false, false, null)]
	public bool Refresh = true;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-paralyze", new(string, object)[2]
		{
			("chance", Probability),
			("time", ParalyzeTime)
		});
	}

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		double paralyzeTime = ParalyzeTime;
		if (args is EntityEffectReagentArgs reagentArgs)
		{
			paralyzeTime *= (double)reagentArgs.Scale;
		}
		args.EntityManager.System<SharedStunSystem>().TryParalyze(args.TargetEntity, TimeSpan.FromSeconds(paralyzeTime), Refresh);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Paralyze target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Paralyze)definitionCast;
		if (!serialization.TryCustomCopy<Paralyze>(this, ref target, hookCtx, false, context))
		{
			double ParalyzeTimeTemp = 0.0;
			if (!serialization.TryCustomCopy<double>(ParalyzeTime, ref ParalyzeTimeTemp, hookCtx, false, context))
			{
				ParalyzeTimeTemp = ParalyzeTime;
			}
			target.ParalyzeTime = ParalyzeTimeTemp;
			bool RefreshTemp = false;
			if (!serialization.TryCustomCopy<bool>(Refresh, ref RefreshTemp, hookCtx, false, context))
			{
				RefreshTemp = Refresh;
			}
			target.Refresh = RefreshTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Paralyze target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Paralyze cast = (Paralyze)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Paralyze cast = (Paralyze)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Paralyze Instantiate()
	{
		return new Paralyze();
	}
}
