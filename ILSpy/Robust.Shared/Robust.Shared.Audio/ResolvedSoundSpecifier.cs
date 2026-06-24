using System;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Robust.Shared.Audio;

[Serializable]
[NetSerializable]
public abstract class ResolvedSoundSpecifier
{
	[Obsolete("String literals for sounds are deprecated, use a SoundSpecifier or ResolvedSoundSpecifier as appropriate instead")]
	public static implicit operator ResolvedSoundSpecifier(string s)
	{
		return new ResolvedPathSpecifier(s);
	}

	[Obsolete("String literals for sounds are deprecated, use a SoundSpecifier or ResolvedSoundSpecifier as appropriate instead")]
	public static implicit operator ResolvedSoundSpecifier(ResPath s)
	{
		return new ResolvedPathSpecifier(s);
	}

	public static bool IsNullOrEmpty(ResolvedSoundSpecifier? s)
	{
		if (s != null)
		{
			if (!(s is ResolvedPathSpecifier { Path: var path }))
			{
				if (s is ResolvedCollectionSpecifier resolvedCollectionSpecifier)
				{
					ProtoId<SoundCollectionPrototype>? collection = resolvedCollectionSpecifier.Collection;
					return string.IsNullOrEmpty(collection.HasValue ? ((string)collection.GetValueOrDefault()) : null);
				}
				throw new ArgumentOutOfRangeException("s", s, "argument is not a ResolvedPathSpecifier or a ResolvedCollectionSpecifier");
			}
			return path.ToString() == "";
		}
		return true;
	}
}
