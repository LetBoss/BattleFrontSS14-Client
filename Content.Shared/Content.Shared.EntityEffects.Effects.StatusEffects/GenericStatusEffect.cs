using System;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.EntityEffects.Effects.StatusEffects;

[Obsolete("Use ModifyStatusEffect with StatusEffectNewSystem instead")]
public sealed class GenericStatusEffect : EntityEffect, ISerializationGenerated<GenericStatusEffect>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public string Key;

	[DataField(null, false, 1, false, false, null)]
	public string Component = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public float Time = 2f;

	[DataField(null, false, 1, false, false, null)]
	public bool Refresh = true;

	[DataField(null, false, 1, false, false, null)]
	public StatusEffectMetabolismType Type;

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		StatusEffectsSystem statusSys = args.EntityManager.EntitySysManager.GetEntitySystem<StatusEffectsSystem>();
		float time = Time;
		if (args is EntityEffectReagentArgs reagentArgs)
		{
			time *= reagentArgs.Scale.Float();
		}
		if (Type == StatusEffectMetabolismType.Add && Component != string.Empty)
		{
			statusSys.TryAddStatusEffect(args.TargetEntity, Key, TimeSpan.FromSeconds(time), Refresh, Component);
		}
		else if (Type == StatusEffectMetabolismType.Remove)
		{
			statusSys.TryRemoveTime(args.TargetEntity, Key, TimeSpan.FromSeconds(time));
		}
		else if (Type == StatusEffectMetabolismType.Set)
		{
			statusSys.TrySetTime(args.TargetEntity, Key, TimeSpan.FromSeconds(time));
		}
	}

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-status-effect", new(string, object)[4]
		{
			("chance", Probability),
			("type", Type),
			("time", Time),
			("key", "reagent-effect-status-effect-" + Key)
		});
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GenericStatusEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GenericStatusEffect)definitionCast;
		if (!serialization.TryCustomCopy<GenericStatusEffect>(this, ref target, hookCtx, false, context))
		{
			string KeyTemp = null;
			if (Key == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Key, ref KeyTemp, hookCtx, false, context))
			{
				KeyTemp = Key;
			}
			target.Key = KeyTemp;
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
			float TimeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Time, ref TimeTemp, hookCtx, false, context))
			{
				TimeTemp = Time;
			}
			target.Time = TimeTemp;
			bool RefreshTemp = false;
			if (!serialization.TryCustomCopy<bool>(Refresh, ref RefreshTemp, hookCtx, false, context))
			{
				RefreshTemp = Refresh;
			}
			target.Refresh = RefreshTemp;
			StatusEffectMetabolismType TypeTemp = StatusEffectMetabolismType.Add;
			if (!serialization.TryCustomCopy<StatusEffectMetabolismType>(Type, ref TypeTemp, hookCtx, false, context))
			{
				TypeTemp = Type;
			}
			target.Type = TypeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GenericStatusEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GenericStatusEffect cast = (GenericStatusEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GenericStatusEffect cast = (GenericStatusEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GenericStatusEffect Instantiate()
	{
		return new GenericStatusEffect();
	}
}
