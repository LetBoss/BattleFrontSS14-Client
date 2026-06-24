using System;
using System.Numerics;
using Content.Shared.Pointing.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Pointing.Components;

[RegisterComponent]
public sealed class PointingArrowComponent : SharedPointingArrowComponent, ISerializationGenerated<PointingArrowComponent>, ISerializationGenerated
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("offset", false, 1, false, false, null)]
	public Vector2 Offset = new Vector2(0f, 0.25f);

	public readonly string AnimationKey = "pointingarrow";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PointingArrowComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedPointingArrowComponent target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (PointingArrowComponent)target2;
		if (!serialization.TryCustomCopy<PointingArrowComponent>(this, ref target, hookCtx, false, context))
		{
			Vector2 offset = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(Offset, ref offset, hookCtx, false, context))
			{
				offset = serialization.CreateCopy<Vector2>(Offset, hookCtx, context, false);
			}
			target.Offset = offset;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PointingArrowComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SharedPointingArrowComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PointingArrowComponent target2 = (PointingArrowComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PointingArrowComponent target2 = (PointingArrowComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PointingArrowComponent target2 = (PointingArrowComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PointingArrowComponent Instantiate()
	{
		return new PointingArrowComponent();
	}
}
