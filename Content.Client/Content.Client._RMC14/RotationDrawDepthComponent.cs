using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Client._RMC14;

[RegisterComponent]
public sealed class RotationDrawDepthComponent : Component, ISerializationGenerated<RotationDrawDepthComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, typeof(ConstantSerializer<DrawDepth>))]
	public int DefaultDrawDepth;

	[DataField(null, false, 1, false, false, typeof(ConstantSerializer<DrawDepth>))]
	public int SouthDrawDepth;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RotationDrawDepthComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (RotationDrawDepthComponent)(object)val;
		if (!serialization.TryCustomCopy<RotationDrawDepthComponent>(this, ref target, hookCtx, false, context))
		{
			int defaultDrawDepth = 0;
			if (!serialization.TryCustomCopy<int>(DefaultDrawDepth, ref defaultDrawDepth, hookCtx, false, context))
			{
				defaultDrawDepth = DefaultDrawDepth;
			}
			target.DefaultDrawDepth = defaultDrawDepth;
			int southDrawDepth = 0;
			if (!serialization.TryCustomCopy<int>(SouthDrawDepth, ref southDrawDepth, hookCtx, false, context))
			{
				southDrawDepth = SouthDrawDepth;
			}
			target.SouthDrawDepth = southDrawDepth;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RotationDrawDepthComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RotationDrawDepthComponent target2 = (RotationDrawDepthComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RotationDrawDepthComponent target2 = (RotationDrawDepthComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RotationDrawDepthComponent target2 = (RotationDrawDepthComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RotationDrawDepthComponent Instantiate()
	{
		return new RotationDrawDepthComponent();
	}
}
