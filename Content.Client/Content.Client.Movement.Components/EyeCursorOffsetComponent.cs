using System;
using System.Numerics;
using Content.Shared.Movement.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Client.Movement.Components;

[RegisterComponent]
public sealed class EyeCursorOffsetComponent : SharedEyeCursorOffsetComponent, ISerializationGenerated<EyeCursorOffsetComponent>, ISerializationGenerated
{
	public Vector2 TargetPosition = Vector2.Zero;

	public Vector2 CurrentPosition = Vector2.Zero;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref EyeCursorOffsetComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedEyeCursorOffsetComponent target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (EyeCursorOffsetComponent)target2;
		serialization.TryCustomCopy<EyeCursorOffsetComponent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref EyeCursorOffsetComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SharedEyeCursorOffsetComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EyeCursorOffsetComponent target2 = (EyeCursorOffsetComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EyeCursorOffsetComponent target2 = (EyeCursorOffsetComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EyeCursorOffsetComponent target2 = (EyeCursorOffsetComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override EyeCursorOffsetComponent Instantiate()
	{
		return new EyeCursorOffsetComponent();
	}
}
