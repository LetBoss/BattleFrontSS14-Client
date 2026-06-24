using System;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Chemistry.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class ReagentTankComponent : Component, ISerializationGenerated<ReagentTankComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public FixedPoint2 TransferAmount { get; set; } = FixedPoint2.New(10);

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public ReagentTankType TankType { get; set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ReagentTankComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ReagentTankComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ReagentTankComponent>(this, ref target, hookCtx, false, context))
		{
			FixedPoint2 TransferAmountTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(TransferAmount, ref TransferAmountTemp, hookCtx, false, context))
			{
				TransferAmountTemp = serialization.CreateCopy<FixedPoint2>(TransferAmount, hookCtx, context, false);
			}
			target.TransferAmount = TransferAmountTemp;
			ReagentTankType TankTypeTemp = ReagentTankType.Unspecified;
			if (!serialization.TryCustomCopy<ReagentTankType>(TankType, ref TankTypeTemp, hookCtx, false, context))
			{
				TankTypeTemp = TankType;
			}
			target.TankType = TankTypeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ReagentTankComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReagentTankComponent cast = (ReagentTankComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReagentTankComponent cast = (ReagentTankComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReagentTankComponent def = (ReagentTankComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ReagentTankComponent Instantiate()
	{
		return new ReagentTankComponent();
	}
}
