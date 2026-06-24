using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Actions.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedActionsSystem) })]
[EntityCategory(new string[] { "Actions" })]
public sealed class TargetActionComponent : Component, ISerializationGenerated<TargetActionComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool Repeat;

	[DataField(null, false, 1, false, false, null)]
	public bool DeselectOnMiss;

	[DataField(null, false, 1, false, false, null)]
	public bool CheckCanAccess = true;

	[DataField(null, false, 1, false, false, null)]
	public float Range = 1.5f;

	[DataField(null, false, 1, false, false, null)]
	public bool InteractOnMiss;

	[DataField(null, false, 1, false, false, null)]
	public bool TargetingIndicator = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref TargetActionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (TargetActionComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<TargetActionComponent>(this, ref target, hookCtx, false, context))
		{
			bool RepeatTemp = false;
			if (!serialization.TryCustomCopy<bool>(Repeat, ref RepeatTemp, hookCtx, false, context))
			{
				RepeatTemp = Repeat;
			}
			target.Repeat = RepeatTemp;
			bool DeselectOnMissTemp = false;
			if (!serialization.TryCustomCopy<bool>(DeselectOnMiss, ref DeselectOnMissTemp, hookCtx, false, context))
			{
				DeselectOnMissTemp = DeselectOnMiss;
			}
			target.DeselectOnMiss = DeselectOnMissTemp;
			bool CheckCanAccessTemp = false;
			if (!serialization.TryCustomCopy<bool>(CheckCanAccess, ref CheckCanAccessTemp, hookCtx, false, context))
			{
				CheckCanAccessTemp = CheckCanAccess;
			}
			target.CheckCanAccess = CheckCanAccessTemp;
			float RangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Range, ref RangeTemp, hookCtx, false, context))
			{
				RangeTemp = Range;
			}
			target.Range = RangeTemp;
			bool InteractOnMissTemp = false;
			if (!serialization.TryCustomCopy<bool>(InteractOnMiss, ref InteractOnMissTemp, hookCtx, false, context))
			{
				InteractOnMissTemp = InteractOnMiss;
			}
			target.InteractOnMiss = InteractOnMissTemp;
			bool TargetingIndicatorTemp = false;
			if (!serialization.TryCustomCopy<bool>(TargetingIndicator, ref TargetingIndicatorTemp, hookCtx, false, context))
			{
				TargetingIndicatorTemp = TargetingIndicator;
			}
			target.TargetingIndicator = TargetingIndicatorTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref TargetActionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TargetActionComponent cast = (TargetActionComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TargetActionComponent cast = (TargetActionComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TargetActionComponent def = (TargetActionComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override TargetActionComponent Instantiate()
	{
		return new TargetActionComponent();
	}
}
