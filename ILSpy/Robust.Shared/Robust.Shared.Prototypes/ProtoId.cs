using System;
using Robust.Shared.Analyzers;
using Robust.Shared.Toolshed.TypeParsers;

namespace Robust.Shared.Prototypes;

[Serializable]
[PreferOtherType(typeof(EntityPrototype), typeof(EntProtoId))]
public readonly record struct ProtoId<T>(string Id) : IEquatable<string>, IComparable<ProtoId<T>>, IAsType<string> where T : class, IPrototype
{
	public static implicit operator string(ProtoId<T> protoId)
	{
		return protoId.Id;
	}

	public static implicit operator ProtoId<T>(T proto)
	{
		return new ProtoId<T>(proto.ID);
	}

	public static implicit operator ProtoId<T>(string id)
	{
		return new ProtoId<T>(id);
	}

	public static implicit operator ProtoId<T>?(string? id)
	{
		if (id != null)
		{
			return new ProtoId<T>(id);
		}
		return null;
	}

	public bool Equals(string? other)
	{
		return Id == other;
	}

	public int CompareTo(ProtoId<T> other)
	{
		return string.Compare(Id, other.Id, StringComparison.Ordinal);
	}

	public string AsType()
	{
		return Id;
	}

	public override string ToString()
	{
		return Id ?? string.Empty;
	}
}
