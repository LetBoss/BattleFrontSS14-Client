using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Magic;

public sealed class TargetInFront : MagicInstantSpawnData, ISerializationGenerated<TargetInFront>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int Width = 3;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref TargetInFront target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MagicInstantSpawnData definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (TargetInFront)definitionCast;
		if (!serialization.TryCustomCopy<TargetInFront>(this, ref target, hookCtx, false, context))
		{
			int WidthTemp = 0;
			if (!serialization.TryCustomCopy<int>(Width, ref WidthTemp, hookCtx, false, context))
			{
				WidthTemp = Width;
			}
			target.Width = WidthTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref TargetInFront target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref MagicInstantSpawnData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TargetInFront cast = (TargetInFront)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TargetInFront cast = (TargetInFront)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override TargetInFront Instantiate()
	{
		return new TargetInFront();
	}
}
