using System;
using System.Runtime.CompilerServices;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.GameObjects;

[Serializable]
[NetSerializable]
[CopyByRef]
public readonly struct NetEntity(int id) : IEquatable<NetEntity>, IComparable<NetEntity>, ISpanFormattable, IFormattable
{
	public readonly int Id = id;

	public const int ClientEntity = 1073741824;

	public static readonly NetEntity Invalid = new NetEntity(0);

	public static readonly NetEntity First = new NetEntity(1);

	public bool Valid => IsValid();

	[ViewVariables]
	private string Representation
	{
		get
		{
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			return entityManager.ToPrettyString(entityManager.GetEntity(this));
		}
	}

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
				IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
				entityManager.System<MetaDataSystem>().SetEntityName(entityManager.GetEntity(this), value, metaData);
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
				IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
				entityManager.System<MetaDataSystem>().SetEntityDescription(entityManager.GetEntity(this), value, metaData);
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
	private MetaDataComponent? MetaData
	{
		get
		{
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			return entityManager.GetComponentOrNull<MetaDataComponent>(entityManager.GetEntity(this));
		}
	}

	[ViewVariables]
	private TransformComponent? Transform
	{
		get
		{
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			return entityManager.GetComponentOrNull<TransformComponent>(entityManager.GetEntity(this));
		}
	}

	[ViewVariables]
	private EntityUid _uid => IoCManager.Resolve<IEntityManager>().GetEntity(this);

	[ViewVariables]
	private NetEntity _netId => this;

	public static NetEntity Parse(ReadOnlySpan<char> uid)
	{
		if (uid.Length == 0)
		{
			throw new FormatException("An empty string is not a valid NetEntity");
		}
		if (uid[0] != 'c')
		{
			return new NetEntity(int.Parse(uid));
		}
		if (uid.Length == 1)
		{
			throw new FormatException("'c' is not a valid NetEntity");
		}
		return new NetEntity(int.Parse(uid.Slice(1, uid.Length - 1)) | 0x40000000);
	}

	public static bool TryParse(ReadOnlySpan<char> uid, out NetEntity entity)
	{
		entity = Invalid;
		if (uid.Length == 0)
		{
			return false;
		}
		int result;
		if (uid[0] != 'c')
		{
			if (!int.TryParse(uid, out result))
			{
				return false;
			}
			entity = new NetEntity(result);
			return true;
		}
		if (uid.Length == 1)
		{
			return false;
		}
		if (!int.TryParse(uid.Slice(1, uid.Length - 1), out result))
		{
			return false;
		}
		entity = new NetEntity(result | 0x40000000);
		return true;
	}

	public bool IsValid()
	{
		return Id > 0;
	}

	public bool Equals(NetEntity other)
	{
		return Id == other.Id;
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj is NetEntity other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Id;
	}

	public static bool operator ==(NetEntity a, NetEntity b)
	{
		return a.Id == b.Id;
	}

	public static bool operator !=(NetEntity a, NetEntity b)
	{
		return !(a == b);
	}

	public static explicit operator int(NetEntity self)
	{
		return self.Id;
	}

	public override string ToString()
	{
		if (IsClientSide())
		{
			return $"c{Id & -1073741825}";
		}
		return Id.ToString();
	}

	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		return ToString();
	}

	public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
	{
		if (IsClientSide())
		{
			Unsafe.SkipInit(out BufferInterpolatedStringHandler val);
			((BufferInterpolatedStringHandler)(ref val))._002Ector(1, 1, destination);
			((BufferInterpolatedStringHandler)(ref val)).AppendLiteral("c");
			((BufferInterpolatedStringHandler)(ref val)).AppendFormatted<int>(Id & -1073741825);
			return FormatHelpers.TryFormatInto(destination, ref charsWritten, ref val);
		}
		return Id.TryFormat(destination, out charsWritten);
	}

	public int CompareTo(NetEntity other)
	{
		return Id.CompareTo(other.Id);
	}

	public bool IsClientSide()
	{
		return (Id & 0x40000000) == 1073741824;
	}
}
