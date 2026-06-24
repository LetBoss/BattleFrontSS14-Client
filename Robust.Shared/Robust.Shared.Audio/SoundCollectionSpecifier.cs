using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Robust.Shared.Audio;

[Serializable]
[NetSerializable]
public sealed class SoundCollectionSpecifier : SoundSpecifier, ISerializationGenerated<SoundCollectionSpecifier>, ISerializationGenerated
{
	public const string Node = "collection";

	[DataField("collection", false, 1, true, false, typeof(PrototypeIdSerializer<SoundCollectionPrototype>))]
	public string? Collection { get; private set; }

	public override string ToString()
	{
		return "SoundCollectionSpecifier(" + Collection + ")";
	}

	public SoundCollectionSpecifier()
	{
	}

	public SoundCollectionSpecifier(string collection, AudioParams? @params = null)
	{
		Collection = collection;
		if (@params.HasValue)
		{
			base.Params = @params.Value;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SoundCollectionSpecifier target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SoundSpecifier target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (SoundCollectionSpecifier)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			string target3 = null;
			if (!serialization.TryCustomCopy(Collection, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = Collection;
			}
			target.Collection = target3;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SoundCollectionSpecifier target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SoundSpecifier target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SoundCollectionSpecifier target2 = (SoundCollectionSpecifier)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SoundCollectionSpecifier target2 = (SoundCollectionSpecifier)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SoundCollectionSpecifier Instantiate()
	{
		return new SoundCollectionSpecifier();
	}
}
