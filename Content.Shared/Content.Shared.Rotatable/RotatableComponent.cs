using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Rotatable;

[RegisterComponent]
public sealed class RotatableComponent : Component, ISerializationGenerated<RotatableComponent>, ISerializationGenerated
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("rotateWhileAnchored", false, 1, false, false, null)]
	public bool RotateWhileAnchored { get; private set; }

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("rotateWhilePulling", false, 1, false, false, null)]
	public bool RotateWhilePulling { get; private set; } = true;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("increment", false, 1, false, false, null)]
	public Angle Increment { get; private set; } = Angle.FromDegrees(90.0);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RotatableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RotatableComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RotatableComponent>(this, ref target, hookCtx, false, context))
		{
			bool RotateWhileAnchoredTemp = false;
			if (!serialization.TryCustomCopy<bool>(RotateWhileAnchored, ref RotateWhileAnchoredTemp, hookCtx, false, context))
			{
				RotateWhileAnchoredTemp = RotateWhileAnchored;
			}
			target.RotateWhileAnchored = RotateWhileAnchoredTemp;
			bool RotateWhilePullingTemp = false;
			if (!serialization.TryCustomCopy<bool>(RotateWhilePulling, ref RotateWhilePullingTemp, hookCtx, false, context))
			{
				RotateWhilePullingTemp = RotateWhilePulling;
			}
			target.RotateWhilePulling = RotateWhilePullingTemp;
			Angle IncrementTemp = default(Angle);
			if (!serialization.TryCustomCopy<Angle>(Increment, ref IncrementTemp, hookCtx, false, context))
			{
				IncrementTemp = serialization.CreateCopy<Angle>(Increment, hookCtx, context, false);
			}
			target.Increment = IncrementTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RotatableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RotatableComponent cast = (RotatableComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RotatableComponent cast = (RotatableComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RotatableComponent def = (RotatableComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RotatableComponent Instantiate()
	{
		return new RotatableComponent();
	}
}
