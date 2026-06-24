using System;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Robust.Shared.Audio;

[Serializable]
[NetSerializable]
public sealed class ResolvedPathSpecifier : ResolvedSoundSpecifier, IEquatable<ResolvedPathSpecifier>
{
	public ResPath Path { get; private set; }

	public override string ToString()
	{
		return $"ResolvedPathSpecifier({Path})";
	}

	private ResolvedPathSpecifier()
	{
	}

	public ResolvedPathSpecifier(ResPath path)
	{
		Path = path;
	}

	public ResolvedPathSpecifier(string path)
		: this(new ResPath(path))
	{
	}

	public bool Equals(ResolvedPathSpecifier? other)
	{
		return Path.Equals(other?.Path);
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as ResolvedPathSpecifier);
	}

	public override int GetHashCode()
	{
		return Path.GetHashCode();
	}
}
