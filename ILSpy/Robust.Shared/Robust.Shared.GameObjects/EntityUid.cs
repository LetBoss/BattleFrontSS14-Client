using System;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.GameObjects;

[CopyByRef]
public readonly struct EntityUid(int id) : IEquatable<EntityUid>, IComparable<EntityUid>, ISpanFormattable, IFormattable
{
	public readonly int Id = id;

	public static readonly EntityUid Invalid = new EntityUid(0);

	public static readonly EntityUid FirstUid = new EntityUid(1);

	public bool Valid => IsValid();

	[ViewVariables]
	private string Representation => IoCManager.Resolve<IEntityManager>().ToPrettyString(this);

	[ViewVariables(VVAccess.ReadWrite)]
	private string Name
	{
		get
		{
			return MetaData?.EntityName ?? string.Empty;
		}
		set
		{
			MetaDataComponent metaData = MetaData;
			if (metaData != null)
			{
				IoCManager.Resolve<IEntityManager>().System<MetaDataSystem>().SetEntityName(this, value, metaData);
			}
		}
	}

	[ViewVariables(VVAccess.ReadWrite)]
	private string Description
	{
		get
		{
			return MetaData?.EntityDescription ?? string.Empty;
		}
		set
		{
			MetaDataComponent metaData = MetaData;
			if (metaData != null)
			{
				IoCManager.Resolve<IEntityManager>().System<MetaDataSystem>().SetEntityDescription(this, value, metaData);
			}
		}
	}

	[ViewVariables]
	private EntityPrototype? Prototype => MetaData?.EntityPrototype;

	[ViewVariables]
	private GameTick LastModifiedTick => MetaData?.EntityLastModifiedTick ?? GameTick.Zero;

	[ViewVariables]
	private bool Paused => MetaData?.EntityPaused ?? false;

	[ViewVariables]
	private EntityLifeStage LifeStage => MetaData?.EntityLifeStage ?? EntityLifeStage.Deleted;

	[ViewVariables]
	private MetaDataComponent? MetaData => IoCManager.Resolve<IEntityManager>().GetComponentOrNull<MetaDataComponent>(this);

	[ViewVariables]
	private TransformComponent? Transform => IoCManager.Resolve<IEntityManager>().GetComponentOrNull<TransformComponent>(this);

	[ViewVariables]
	private EntityUid _uid => this;

	[ViewVariables]
	private NetEntity _netId => IoCManager.Resolve<IEntityManager>().GetNetEntity(this);

	public static EntityUid Parse(ReadOnlySpan<char> uid)
	{
		return new EntityUid(int.Parse(uid));
	}

	public static bool TryParse(ReadOnlySpan<char> uid, out EntityUid entityUid)
	{
		if (!int.TryParse(uid, out var result))
		{
			entityUid = default(EntityUid);
			return false;
		}
		entityUid = new EntityUid(result);
		return true;
	}

	public bool IsValid()
	{
		return Id > 0;
	}

	public bool Equals(EntityUid other)
	{
		return Id == other.Id;
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj is EntityUid other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Id.GetHashCode() * 397;
	}

	public static bool operator ==(EntityUid a, EntityUid b)
	{
		return a.Id == b.Id;
	}

	public static bool operator !=(EntityUid a, EntityUid b)
	{
		return !(a == b);
	}

	public static explicit operator int(EntityUid self)
	{
		return self.Id;
	}

	public override string ToString()
	{
		return Id.ToString();
	}

	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		return ToString();
	}

	public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
	{
		return Id.TryFormat(destination, out charsWritten);
	}

	public int CompareTo(EntityUid other)
	{
		return Id.CompareTo(other.Id);
	}
}
