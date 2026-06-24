using System;
using System.Collections.Generic;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.ReagentSpeed;

[RegisterComponent]
[Access(new Type[] { typeof(ReagentSpeedSystem) })]
public sealed class ReagentSpeedComponent : Component, ISerializationGenerated<ReagentSpeedComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public string Solution = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 Cost = 5;

	[DataField(null, false, 1, true, false, null)]
	public Dictionary<ProtoId<ReagentPrototype>, float> Modifiers = new Dictionary<ProtoId<ReagentPrototype>, float>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ReagentSpeedComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ReagentSpeedComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ReagentSpeedComponent>(this, ref target, hookCtx, false, context))
		{
			string SolutionTemp = null;
			if (Solution == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Solution, ref SolutionTemp, hookCtx, false, context))
			{
				SolutionTemp = Solution;
			}
			target.Solution = SolutionTemp;
			FixedPoint2 CostTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(Cost, ref CostTemp, hookCtx, false, context))
			{
				CostTemp = serialization.CreateCopy<FixedPoint2>(Cost, hookCtx, context, false);
			}
			target.Cost = CostTemp;
			Dictionary<ProtoId<ReagentPrototype>, float> ModifiersTemp = null;
			if (Modifiers == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<ProtoId<ReagentPrototype>, float>>(Modifiers, ref ModifiersTemp, hookCtx, true, context))
			{
				ModifiersTemp = serialization.CreateCopy<Dictionary<ProtoId<ReagentPrototype>, float>>(Modifiers, hookCtx, context, false);
			}
			target.Modifiers = ModifiersTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ReagentSpeedComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReagentSpeedComponent cast = (ReagentSpeedComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReagentSpeedComponent cast = (ReagentSpeedComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReagentSpeedComponent def = (ReagentSpeedComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ReagentSpeedComponent Instantiate()
	{
		return new ReagentSpeedComponent();
	}
}
