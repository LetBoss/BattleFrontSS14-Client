using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Player;

namespace Robust.Shared.GameObjects;

public readonly record struct EntityStringRepresentation : IFormattable
{
	public EntityUid Uid { get; init; }

	public NetEntity Nuid { get; init; }

	public bool Deleted { get; init; }

	public string? Name { get; init; }

	public string? Prototype { get; init; }

	public ICommonSession? Session { get; init; }

	public EntityStringRepresentation(EntityUid Uid, NetEntity Nuid, bool Deleted, string? Name = null, string? Prototype = null, ICommonSession? Session = null)
	{
		this.Uid = Uid;
		this.Nuid = Nuid;
		this.Deleted = Deleted;
		this.Name = Name;
		this.Prototype = Prototype;
		this.Session = Session;
	}

	public EntityStringRepresentation(Entity<MetaDataComponent> entity)
		: this(entity.Owner, entity.Comp)
	{
	}

	public EntityStringRepresentation(EntityUid uid, MetaDataComponent meta, ActorComponent? actor = null)
		: this(uid, meta.NetEntity, meta.EntityDeleted, meta.EntityName, meta.EntityPrototype?.ID, actor?.PlayerSession)
	{
	}

	public override string ToString()
	{
		if (Deleted && Name == null)
		{
			return $"{Uid}/n{Nuid}D";
		}
		return $"{Name} ({Uid}/n{Nuid}{((Prototype != null) ? (", " + Prototype) : "")}{((Session != null) ? (", " + Session.Name) : "")}){(Deleted ? "D" : "")}";
	}

	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		return ToString();
	}

	public static implicit operator string(EntityStringRepresentation rep)
	{
		return rep.ToString();
	}

	[CompilerGenerated]
	public void Deconstruct(out EntityUid Uid, out NetEntity Nuid, out bool Deleted, out string? Name, out string? Prototype, out ICommonSession? Session)
	{
		Uid = this.Uid;
		Nuid = this.Nuid;
		Deleted = this.Deleted;
		Name = this.Name;
		Prototype = this.Prototype;
		Session = this.Session;
	}
}
