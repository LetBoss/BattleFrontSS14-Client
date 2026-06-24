using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Tag;

public sealed class TagSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _proto;

	private EntityQuery<TagComponent> _tagQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_tagQuery = ((EntitySystem)this).GetEntityQuery<TagComponent>();
	}

	public bool AddTag(EntityUid entityUid, [ForbidLiteral] ProtoId<TagPrototype> tag)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return AddTag(Entity<TagComponent>.op_Implicit((entityUid, ((EntitySystem)this).EnsureComp<TagComponent>(entityUid))), tag);
	}

	public bool AddTags(EntityUid entityUid, [ForbidLiteral] params ProtoId<TagPrototype>[] tags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return AddTags(entityUid, (IEnumerable<ProtoId<TagPrototype>>)tags);
	}

	public bool AddTags(EntityUid entityUid, [ForbidLiteral] IEnumerable<ProtoId<TagPrototype>> tags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return AddTags(Entity<TagComponent>.op_Implicit((entityUid, ((EntitySystem)this).EnsureComp<TagComponent>(entityUid))), tags);
	}

	public bool TryAddTag(EntityUid entityUid, [ForbidLiteral] ProtoId<TagPrototype> tag)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		TagComponent component = default(TagComponent);
		if (_tagQuery.TryComp(entityUid, ref component))
		{
			return AddTag(Entity<TagComponent>.op_Implicit((entityUid, component)), tag);
		}
		return false;
	}

	public bool TryAddTags(EntityUid entityUid, [ForbidLiteral] params ProtoId<TagPrototype>[] tags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return TryAddTags(entityUid, (IEnumerable<ProtoId<TagPrototype>>)tags);
	}

	public bool TryAddTags(EntityUid entityUid, [ForbidLiteral] IEnumerable<ProtoId<TagPrototype>> tags)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		TagComponent component = default(TagComponent);
		if (_tagQuery.TryComp(entityUid, ref component))
		{
			return AddTags(Entity<TagComponent>.op_Implicit((entityUid, component)), tags);
		}
		return false;
	}

	public bool HasTag(EntityUid entityUid, [ForbidLiteral] ProtoId<TagPrototype> tag)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		TagComponent component = default(TagComponent);
		if (_tagQuery.TryComp(entityUid, ref component))
		{
			return HasTag(component, tag);
		}
		return false;
	}

	public bool HasAllTags(EntityUid entityUid, ProtoId<TagPrototype> tag)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return HasTag(entityUid, tag);
	}

	public bool HasAllTags(EntityUid entityUid, [ForbidLiteral] params ProtoId<TagPrototype>[] tags)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		TagComponent component = default(TagComponent);
		if (_tagQuery.TryComp(entityUid, ref component))
		{
			return HasAllTags(component, tags);
		}
		return false;
	}

	public bool HasAllTags(EntityUid entityUid, [ForbidLiteral] HashSet<ProtoId<TagPrototype>> tags)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		TagComponent component = default(TagComponent);
		if (_tagQuery.TryComp(entityUid, ref component))
		{
			return HasAllTags(component, tags);
		}
		return false;
	}

	public bool HasAllTags(EntityUid entityUid, [ForbidLiteral] List<ProtoId<TagPrototype>> tags)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		TagComponent component = default(TagComponent);
		if (_tagQuery.TryComp(entityUid, ref component))
		{
			return HasAllTags(component, tags);
		}
		return false;
	}

	public bool HasAllTags(EntityUid entityUid, [ForbidLiteral] IEnumerable<ProtoId<TagPrototype>> tags)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		TagComponent component = default(TagComponent);
		if (_tagQuery.TryComp(entityUid, ref component))
		{
			return HasAllTags(component, tags);
		}
		return false;
	}

	public bool HasAnyTag(EntityUid entityUid, [ForbidLiteral] ProtoId<TagPrototype> tag)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return HasTag(entityUid, tag);
	}

	public bool HasAnyTag(EntityUid entityUid, [ForbidLiteral] params ProtoId<TagPrototype>[] tags)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		TagComponent component = default(TagComponent);
		if (_tagQuery.TryComp(entityUid, ref component))
		{
			return HasAnyTag(component, tags);
		}
		return false;
	}

	public bool HasAnyTag(EntityUid entityUid, [ForbidLiteral] HashSet<ProtoId<TagPrototype>> tags)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		TagComponent component = default(TagComponent);
		if (_tagQuery.TryComp(entityUid, ref component))
		{
			return HasAnyTag(component, tags);
		}
		return false;
	}

	public bool HasAnyTag(EntityUid entityUid, [ForbidLiteral] List<ProtoId<TagPrototype>> tags)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		TagComponent component = default(TagComponent);
		if (_tagQuery.TryComp(entityUid, ref component))
		{
			return HasAnyTag(component, tags);
		}
		return false;
	}

	public bool HasAnyTag(EntityUid entityUid, [ForbidLiteral] IEnumerable<ProtoId<TagPrototype>> tags)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		TagComponent component = default(TagComponent);
		if (_tagQuery.TryComp(entityUid, ref component))
		{
			return HasAnyTag(component, tags);
		}
		return false;
	}

	public bool HasTag(TagComponent component, [ForbidLiteral] ProtoId<TagPrototype> tag)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return component.Tags.Contains(tag);
	}

	public bool HasAllTags(TagComponent component, [ForbidLiteral] ProtoId<TagPrototype> tag)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return HasTag(component, tag);
	}

	public bool HasAllTags(TagComponent component, [ForbidLiteral] params ProtoId<TagPrototype>[] tags)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		foreach (ProtoId<TagPrototype> tag in tags)
		{
			if (!component.Tags.Contains(tag))
			{
				return false;
			}
		}
		return true;
	}

	public bool HasAllTagsArray(TagComponent component, [ForbidLiteral] ProtoId<TagPrototype>[] tags)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		foreach (ProtoId<TagPrototype> tag in tags)
		{
			if (!component.Tags.Contains(tag))
			{
				return false;
			}
		}
		return true;
	}

	public bool HasAllTags(TagComponent component, [ForbidLiteral] List<ProtoId<TagPrototype>> tags)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		foreach (ProtoId<TagPrototype> tag in tags)
		{
			if (!component.Tags.Contains(tag))
			{
				return false;
			}
		}
		return true;
	}

	public bool HasAllTags(TagComponent component, [ForbidLiteral] HashSet<ProtoId<TagPrototype>> tags)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		foreach (ProtoId<TagPrototype> tag in tags)
		{
			if (!component.Tags.Contains(tag))
			{
				return false;
			}
		}
		return true;
	}

	public bool HasAllTags(TagComponent component, [ForbidLiteral] IEnumerable<ProtoId<TagPrototype>> tags)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		foreach (ProtoId<TagPrototype> tag in tags)
		{
			if (!component.Tags.Contains(tag))
			{
				return false;
			}
		}
		return true;
	}

	public bool HasAnyTag(TagComponent component, [ForbidLiteral] ProtoId<TagPrototype> tag)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return HasTag(component, tag);
	}

	public bool HasAnyTag(TagComponent component, [ForbidLiteral] params ProtoId<TagPrototype>[] tags)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		foreach (ProtoId<TagPrototype> tag in tags)
		{
			if (component.Tags.Contains(tag))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasAnyTag(TagComponent component, [ForbidLiteral] HashSet<ProtoId<TagPrototype>> tags)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		foreach (ProtoId<TagPrototype> tag in tags)
		{
			if (component.Tags.Contains(tag))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasAnyTag(TagComponent component, [ForbidLiteral] List<ProtoId<TagPrototype>> tags)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		foreach (ProtoId<TagPrototype> tag in tags)
		{
			if (component.Tags.Contains(tag))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasAnyTag(TagComponent component, [ForbidLiteral] IEnumerable<ProtoId<TagPrototype>> tags)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		foreach (ProtoId<TagPrototype> tag in tags)
		{
			if (component.Tags.Contains(tag))
			{
				return true;
			}
		}
		return false;
	}

	public bool RemoveTag(EntityUid entityUid, [ForbidLiteral] ProtoId<TagPrototype> tag)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		TagComponent component = default(TagComponent);
		if (_tagQuery.TryComp(entityUid, ref component))
		{
			return RemoveTag(Entity<TagComponent>.op_Implicit((entityUid, component)), tag);
		}
		return false;
	}

	public bool RemoveTags(EntityUid entityUid, [ForbidLiteral] params ProtoId<TagPrototype>[] tags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return RemoveTags(entityUid, (IEnumerable<ProtoId<TagPrototype>>)tags);
	}

	public bool RemoveTags(EntityUid entityUid, [ForbidLiteral] IEnumerable<ProtoId<TagPrototype>> tags)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		TagComponent component = default(TagComponent);
		if (_tagQuery.TryComp(entityUid, ref component))
		{
			return RemoveTags(Entity<TagComponent>.op_Implicit((entityUid, component)), tags);
		}
		return false;
	}

	public bool AddTag(Entity<TagComponent> entity, [ForbidLiteral] ProtoId<TagPrototype> tag)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!entity.Comp.Tags.Add(tag))
		{
			return false;
		}
		((EntitySystem)this).Dirty<TagComponent>(entity, (MetaDataComponent)null);
		return true;
	}

	public bool AddTags(Entity<TagComponent> entity, [ForbidLiteral] params ProtoId<TagPrototype>[] tags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return AddTags(entity, (IEnumerable<ProtoId<TagPrototype>>)tags);
	}

	public bool AddTags(Entity<TagComponent> entity, [ForbidLiteral] IEnumerable<ProtoId<TagPrototype>> tags)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		bool update = false;
		foreach (ProtoId<TagPrototype> tag in tags)
		{
			if (entity.Comp.Tags.Add(tag) && !update)
			{
				update = true;
			}
		}
		if (!update)
		{
			return false;
		}
		((EntitySystem)this).Dirty<TagComponent>(entity, (MetaDataComponent)null);
		return true;
	}

	public bool RemoveTag(Entity<TagComponent> entity, [ForbidLiteral] ProtoId<TagPrototype> tag)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!entity.Comp.Tags.Remove(tag))
		{
			return false;
		}
		((EntitySystem)this).Dirty<TagComponent>(entity, (MetaDataComponent)null);
		return true;
	}

	public bool RemoveTags(Entity<TagComponent> entity, [ForbidLiteral] params ProtoId<TagPrototype>[] tags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return RemoveTags(entity, (IEnumerable<ProtoId<TagPrototype>>)tags);
	}

	public bool RemoveTags(Entity<TagComponent> entity, [ForbidLiteral] IEnumerable<ProtoId<TagPrototype>> tags)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		bool update = false;
		foreach (ProtoId<TagPrototype> tag in tags)
		{
			if (entity.Comp.Tags.Remove(tag) && !update)
			{
				update = true;
			}
		}
		if (!update)
		{
			return false;
		}
		((EntitySystem)this).Dirty<TagComponent>(entity, (MetaDataComponent)null);
		return true;
	}

	private void AssertValidTag(string id)
	{
	}
}
