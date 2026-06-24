using System;
using Content.Shared.Chemistry.Components;
using Content.Shared.Explosion.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Explosion.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedSmokeOnTriggerSystem) })]
public sealed class SmokeOnTriggerComponent : Component, ISerializationGenerated<SmokeOnTriggerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float Duration = 10f;

	[DataField(null, false, 1, true, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int SpreadAmount;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public EntProtoId SmokePrototype = EntProtoId.op_Implicit("Smoke");

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Solution Solution = new Solution();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SmokeOnTriggerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SmokeOnTriggerComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<SmokeOnTriggerComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		float DurationTemp = 0f;
		if (!serialization.TryCustomCopy<float>(Duration, ref DurationTemp, hookCtx, false, context))
		{
			DurationTemp = Duration;
		}
		target.Duration = DurationTemp;
		int SpreadAmountTemp = 0;
		if (!serialization.TryCustomCopy<int>(SpreadAmount, ref SpreadAmountTemp, hookCtx, false, context))
		{
			SpreadAmountTemp = SpreadAmount;
		}
		target.SpreadAmount = SpreadAmountTemp;
		EntProtoId SmokePrototypeTemp = default(EntProtoId);
		if (!serialization.TryCustomCopy<EntProtoId>(SmokePrototype, ref SmokePrototypeTemp, hookCtx, false, context))
		{
			SmokePrototypeTemp = serialization.CreateCopy<EntProtoId>(SmokePrototype, hookCtx, context, false);
		}
		target.SmokePrototype = SmokePrototypeTemp;
		Solution SolutionTemp = null;
		if (Solution == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<Solution>(Solution, ref SolutionTemp, hookCtx, true, context))
		{
			if (Solution == null)
			{
				SolutionTemp = null;
			}
			else
			{
				serialization.CopyTo<Solution>(Solution, ref SolutionTemp, hookCtx, context, true);
			}
		}
		target.Solution = SolutionTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SmokeOnTriggerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SmokeOnTriggerComponent cast = (SmokeOnTriggerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SmokeOnTriggerComponent cast = (SmokeOnTriggerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SmokeOnTriggerComponent def = (SmokeOnTriggerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SmokeOnTriggerComponent Instantiate()
	{
		return new SmokeOnTriggerComponent();
	}
}
