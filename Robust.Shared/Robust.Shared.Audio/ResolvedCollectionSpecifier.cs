using System;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Robust.Shared.Audio;

[Serializable]
[NetSerializable]
public sealed class ResolvedCollectionSpecifier : ResolvedSoundSpecifier, IEquatable<ResolvedCollectionSpecifier>
{
	public ProtoId<SoundCollectionPrototype>? Collection { get; private set; }

	public int Index { get; private set; }

	public override string ToString()
	{
		return $"ResolvedCollectionSpecifier({Collection}, {Index})";
	}

	private ResolvedCollectionSpecifier()
	{
	}

	public ResolvedCollectionSpecifier(string collection, int index)
	{
		Collection = collection;
		Index = index;
	}

	public bool Equals(ResolvedCollectionSpecifier? other)
	{
		if (Collection.Equals(other?.Collection))
		{
			return Index.Equals(other?.Index);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as ResolvedCollectionSpecifier);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Collection, Index);
	}
}
