using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Robust.Shared.Audio;

[Serializable]
[ImplicitDataDefinitionForInheritors]
[NetSerializable]
public abstract class SoundSpecifier : ISerializationGenerated<SoundSpecifier>, ISerializationGenerated
{
	[DataField("params", false, 1, false, false, null)]
	public AudioParams Params { get; set; } = AudioParams.Default;

	public SoundSpecifier()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref SoundSpecifier target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			AudioParams target2 = default(AudioParams);
			if (!serialization.TryCustomCopy(Params, ref target2, hookCtx, hasHooks: false, context))
			{
				serialization.CopyTo(Params, ref target2, hookCtx, context);
			}
			target.Params = target2;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref SoundSpecifier target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SoundSpecifier target2 = (SoundSpecifier)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual SoundSpecifier Instantiate()
	{
		throw new NotImplementedException();
	}
}
