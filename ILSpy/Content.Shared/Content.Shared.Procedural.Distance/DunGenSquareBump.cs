using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Procedural.Distance;

public sealed class DunGenSquareBump : IDunGenDistance, ISerializationGenerated<IDunGenDistance>, ISerializationGenerated, ISerializationGenerated<DunGenSquareBump>
{
	[DataField(null, false, 1, false, false, null)]
	public float BlendWeight { get; set; } = 0.5f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DunGenSquareBump target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<DunGenSquareBump>(this, ref target, hookCtx, false, context))
		{
			float BlendWeightTemp = 0f;
			if (!serialization.TryCustomCopy<float>(BlendWeight, ref BlendWeightTemp, hookCtx, false, context))
			{
				BlendWeightTemp = BlendWeight;
			}
			target.BlendWeight = BlendWeightTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DunGenSquareBump target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DunGenSquareBump cast = (DunGenSquareBump)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IDunGenDistance target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DunGenSquareBump def = (DunGenSquareBump)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IDunGenDistance target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public DunGenSquareBump Instantiate()
	{
		return new DunGenSquareBump();
	}

	IDunGenDistance IDunGenDistance.Instantiate()
	{
		return Instantiate();
	}

	IDunGenDistance ISerializationGenerated<IDunGenDistance>.Instantiate()
	{
		return Instantiate();
	}
}
