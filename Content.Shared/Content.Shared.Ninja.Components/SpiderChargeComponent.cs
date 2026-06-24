using System;
using Content.Shared.Ninja.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Ninja.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedSpiderChargeSystem) })]
public sealed class SpiderChargeComponent : Component, ISerializationGenerated<SpiderChargeComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Range = 10f;

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? Planter;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SpiderChargeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SpiderChargeComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SpiderChargeComponent>(this, ref target, hookCtx, false, context))
		{
			float RangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Range, ref RangeTemp, hookCtx, false, context))
			{
				RangeTemp = Range;
			}
			target.Range = RangeTemp;
			EntityUid? PlanterTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(Planter, ref PlanterTemp, hookCtx, false, context))
			{
				PlanterTemp = serialization.CreateCopy<EntityUid?>(Planter, hookCtx, context, false);
			}
			target.Planter = PlanterTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SpiderChargeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpiderChargeComponent cast = (SpiderChargeComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpiderChargeComponent cast = (SpiderChargeComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpiderChargeComponent def = (SpiderChargeComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SpiderChargeComponent Instantiate()
	{
		return new SpiderChargeComponent();
	}
}
