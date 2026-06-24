using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Utility;

namespace Robust.Shared.Audio;

[Serializable]
[NetSerializable]
public sealed class SoundPathSpecifier : SoundSpecifier, ISerializationGenerated<SoundPathSpecifier>, ISerializationGenerated
{
	public const string Node = "path";

	[DataField("path", false, 1, true, false, typeof(ResPathSerializer))]
	public ResPath Path { get; private set; }

	public override string ToString()
	{
		return $"SoundPathSpecifier({Path})";
	}

	private SoundPathSpecifier()
	{
	}

	public SoundPathSpecifier(string path, AudioParams? @params = null)
		: this(new ResPath(path), @params)
	{
	}

	public SoundPathSpecifier(ResPath path, AudioParams? @params = null)
	{
		Path = path;
		if (@params.HasValue)
		{
			base.Params = @params.Value;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SoundPathSpecifier target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SoundSpecifier target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (SoundPathSpecifier)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			ResPath resPath = default(ResPath);
			resPath = serialization.CreateCopy<ResPath, ResPathSerializer>(Path, hookCtx, context);
			target.Path = resPath;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SoundPathSpecifier target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SoundSpecifier target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SoundPathSpecifier target2 = (SoundPathSpecifier)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SoundPathSpecifier target2 = (SoundPathSpecifier)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SoundPathSpecifier Instantiate()
	{
		return new SoundPathSpecifier();
	}
}
