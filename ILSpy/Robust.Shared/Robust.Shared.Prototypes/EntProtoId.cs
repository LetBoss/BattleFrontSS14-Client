using System;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Toolshed.TypeParsers;

namespace Robust.Shared.Prototypes;

[Serializable]
[NetSerializable]
public readonly record struct EntProtoId(string Id) : IEquatable<string>, IComparable<EntProtoId>, IAsType<string>, IAsType<ProtoId<EntityPrototype>>
{
	public static implicit operator string(EntProtoId protoId)
	{
		return protoId.Id;
	}

	public static implicit operator EntProtoId(EntityPrototype proto)
	{
		return new EntProtoId(proto.ID);
	}

	public static implicit operator EntProtoId(string id)
	{
		return new EntProtoId(id);
	}

	public static implicit operator EntProtoId?(string? id)
	{
		if (id != null)
		{
			return new EntProtoId(id);
		}
		return null;
	}

	public bool Equals(string? other)
	{
		return Id == other;
	}

	public int CompareTo(EntProtoId other)
	{
		return string.Compare(Id, other.Id, StringComparison.Ordinal);
	}

	string IAsType<string>.AsType()
	{
		return Id;
	}

	ProtoId<EntityPrototype> IAsType<ProtoId<EntityPrototype>>.AsType()
	{
		return new ProtoId<EntityPrototype>(Id);
	}

	public override string ToString()
	{
		return Id ?? string.Empty;
	}
}
[Serializable]
public readonly record struct EntProtoId<T>(string Id) : IEquatable<string>, IComparable<EntProtoId> where T : IComponent, new()
{
	public static implicit operator string(EntProtoId<T> protoId)
	{
		return protoId.Id;
	}

	public static implicit operator EntProtoId(EntProtoId<T> protoId)
	{
		return new EntProtoId(protoId.Id);
	}

	public static implicit operator EntProtoId<T>(string id)
	{
		return new EntProtoId<T>(id);
	}

	public static implicit operator EntProtoId<T>?(string? id)
	{
		if (id != null)
		{
			return new EntProtoId<T>(id);
		}
		return null;
	}

	public bool Equals(string? other)
	{
		return Id == other;
	}

	public int CompareTo(EntProtoId other)
	{
		return string.Compare(Id, other.Id, StringComparison.Ordinal);
	}

	public override string ToString()
	{
		return Id ?? string.Empty;
	}

	public T Get(IPrototypeManager? prototypes, IComponentFactory compFactory)
	{
		if (prototypes == null)
		{
			prototypes = IoCManager.Resolve<IPrototypeManager>();
		}
		EntityPrototype entityPrototype = prototypes.Index(this);
		if (!entityPrototype.TryGetComponent<T>(out T component, compFactory))
		{
			throw new ArgumentException($"{"EntityPrototype"} {entityPrototype.ID} has no {"T"}");
		}
		return component;
	}

	public bool TryGet([NotNullWhen(true)] out T? comp, IPrototypeManager? prototypes, IComponentFactory compFactory)
	{
		comp = default(T);
		if (prototypes == null)
		{
			prototypes = IoCManager.Resolve<IPrototypeManager>();
		}
		if (prototypes.TryIndex((EntProtoId)this, out EntityPrototype prototype))
		{
			return prototype.TryGetComponent<T>(out comp, compFactory);
		}
		return false;
	}
}
