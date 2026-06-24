using System;
using Content.Shared.Chemistry.Components;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;

namespace Content.Shared.EntityEffects.Effects;

public sealed class MovespeedModifier : EntityEffect, ISerializationGenerated<MovespeedModifier>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float StatusLifetime = 2f;

	[DataField(null, false, 1, false, false, null)]
	public float WalkSpeedModifier { get; set; } = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float SprintSpeedModifier { get; set; } = 1f;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-movespeed-modifier", new(string, object)[3]
		{
			("chance", Probability),
			("walkspeed", WalkSpeedModifier),
			("time", StatusLifetime)
		});
	}

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		MovespeedModifierMetabolismComponent status = args.EntityManager.EnsureComponent<MovespeedModifierMetabolismComponent>(args.TargetEntity);
		bool num = !status.WalkSpeedModifier.Equals(WalkSpeedModifier) || !status.SprintSpeedModifier.Equals(SprintSpeedModifier);
		status.WalkSpeedModifier = WalkSpeedModifier;
		status.SprintSpeedModifier = SprintSpeedModifier;
		float statusLifetime = StatusLifetime;
		if (args is EntityEffectReagentArgs reagentArgs)
		{
			statusLifetime *= reagentArgs.Scale.Float();
		}
		IncreaseTimer(status, statusLifetime, args.EntityManager, args.TargetEntity);
		if (num)
		{
			args.EntityManager.System<MovementSpeedModifierSystem>().RefreshMovementSpeedModifiers(args.TargetEntity);
		}
	}

	private void IncreaseTimer(MovespeedModifierMetabolismComponent status, float time, IEntityManager entityManager, EntityUid uid)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		IGameTiming gameTiming = IoCManager.Resolve<IGameTiming>();
		double offsetTime = Math.Max(status.ModifierTimer.TotalSeconds, gameTiming.CurTime.TotalSeconds);
		status.ModifierTimer = TimeSpan.FromSeconds(offsetTime + (double)time);
		entityManager.Dirty(uid, (IComponent)(object)status, (MetaDataComponent)null);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MovespeedModifier target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MovespeedModifier)definitionCast;
		if (!serialization.TryCustomCopy<MovespeedModifier>(this, ref target, hookCtx, false, context))
		{
			float WalkSpeedModifierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(WalkSpeedModifier, ref WalkSpeedModifierTemp, hookCtx, false, context))
			{
				WalkSpeedModifierTemp = WalkSpeedModifier;
			}
			target.WalkSpeedModifier = WalkSpeedModifierTemp;
			float SprintSpeedModifierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SprintSpeedModifier, ref SprintSpeedModifierTemp, hookCtx, false, context))
			{
				SprintSpeedModifierTemp = SprintSpeedModifier;
			}
			target.SprintSpeedModifier = SprintSpeedModifierTemp;
			float StatusLifetimeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(StatusLifetime, ref StatusLifetimeTemp, hookCtx, false, context))
			{
				StatusLifetimeTemp = StatusLifetime;
			}
			target.StatusLifetime = StatusLifetimeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MovespeedModifier target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MovespeedModifier cast = (MovespeedModifier)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MovespeedModifier cast = (MovespeedModifier)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MovespeedModifier Instantiate()
	{
		return new MovespeedModifier();
	}
}
