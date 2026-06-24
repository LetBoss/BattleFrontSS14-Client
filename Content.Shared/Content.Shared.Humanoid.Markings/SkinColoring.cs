using System;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Humanoid.Markings;

public sealed class SkinColoring : LayerColoringType, ISerializationGenerated<SkinColoring>, ISerializationGenerated
{
	public override Color? GetCleanColor(Color? skin, Color? eyes, MarkingSet markingSet)
	{
		return skin;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SkinColoring target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LayerColoringType definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SkinColoring)definitionCast;
		serialization.TryCustomCopy<SkinColoring>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SkinColoring target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref LayerColoringType target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SkinColoring cast = (SkinColoring)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SkinColoring cast = (SkinColoring)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SkinColoring Instantiate()
	{
		return new SkinColoring();
	}
}
