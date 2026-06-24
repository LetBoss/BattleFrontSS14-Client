// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tag.TagSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Tag;

public sealed class TagSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _proto;
  private Robust.Shared.GameObjects.EntityQuery<TagComponent> _tagQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._tagQuery = this.GetEntityQuery<TagComponent>();
  }

  public bool AddTag(EntityUid entityUid, [ForbidLiteral] ProtoId<TagPrototype> tag)
  {
    return this.AddTag((Entity<TagComponent>) (entityUid, this.EnsureComp<TagComponent>(entityUid)), tag);
  }

  public bool AddTags(EntityUid entityUid, [ForbidLiteral] params ProtoId<TagPrototype>[] tags)
  {
    return this.AddTags(entityUid, (IEnumerable<ProtoId<TagPrototype>>) tags);
  }

  public bool AddTags(EntityUid entityUid, [ForbidLiteral] IEnumerable<ProtoId<TagPrototype>> tags)
  {
    return this.AddTags((Entity<TagComponent>) (entityUid, this.EnsureComp<TagComponent>(entityUid)), tags);
  }

  public bool TryAddTag(EntityUid entityUid, [ForbidLiteral] ProtoId<TagPrototype> tag)
  {
    TagComponent component;
    return this._tagQuery.TryComp(entityUid, out component) && this.AddTag((Entity<TagComponent>) (entityUid, component), tag);
  }

  public bool TryAddTags(EntityUid entityUid, [ForbidLiteral] params ProtoId<TagPrototype>[] tags)
  {
    return this.TryAddTags(entityUid, (IEnumerable<ProtoId<TagPrototype>>) tags);
  }

  public bool TryAddTags(EntityUid entityUid, [ForbidLiteral] IEnumerable<ProtoId<TagPrototype>> tags)
  {
    TagComponent component;
    return this._tagQuery.TryComp(entityUid, out component) && this.AddTags((Entity<TagComponent>) (entityUid, component), tags);
  }

  public bool HasTag(EntityUid entityUid, [ForbidLiteral] ProtoId<TagPrototype> tag)
  {
    TagComponent component;
    return this._tagQuery.TryComp(entityUid, out component) && this.HasTag(component, tag);
  }

  public bool HasAllTags(EntityUid entityUid, ProtoId<TagPrototype> tag)
  {
    return this.HasTag(entityUid, tag);
  }

  public bool HasAllTags(EntityUid entityUid, [ForbidLiteral] params ProtoId<TagPrototype>[] tags)
  {
    TagComponent component;
    return this._tagQuery.TryComp(entityUid, out component) && this.HasAllTags(component, tags);
  }

  public bool HasAllTags(EntityUid entityUid, [ForbidLiteral] HashSet<ProtoId<TagPrototype>> tags)
  {
    TagComponent component;
    return this._tagQuery.TryComp(entityUid, out component) && this.HasAllTags(component, tags);
  }

  public bool HasAllTags(EntityUid entityUid, [ForbidLiteral] List<ProtoId<TagPrototype>> tags)
  {
    TagComponent component;
    return this._tagQuery.TryComp(entityUid, out component) && this.HasAllTags(component, tags);
  }

  public bool HasAllTags(EntityUid entityUid, [ForbidLiteral] IEnumerable<ProtoId<TagPrototype>> tags)
  {
    TagComponent component;
    return this._tagQuery.TryComp(entityUid, out component) && this.HasAllTags(component, tags);
  }

  public bool HasAnyTag(EntityUid entityUid, [ForbidLiteral] ProtoId<TagPrototype> tag)
  {
    return this.HasTag(entityUid, tag);
  }

  public bool HasAnyTag(EntityUid entityUid, [ForbidLiteral] params ProtoId<TagPrototype>[] tags)
  {
    TagComponent component;
    return this._tagQuery.TryComp(entityUid, out component) && this.HasAnyTag(component, tags);
  }

  public bool HasAnyTag(EntityUid entityUid, [ForbidLiteral] HashSet<ProtoId<TagPrototype>> tags)
  {
    TagComponent component;
    return this._tagQuery.TryComp(entityUid, out component) && this.HasAnyTag(component, tags);
  }

  public bool HasAnyTag(EntityUid entityUid, [ForbidLiteral] List<ProtoId<TagPrototype>> tags)
  {
    TagComponent component;
    return this._tagQuery.TryComp(entityUid, out component) && this.HasAnyTag(component, tags);
  }

  public bool HasAnyTag(EntityUid entityUid, [ForbidLiteral] IEnumerable<ProtoId<TagPrototype>> tags)
  {
    TagComponent component;
    return this._tagQuery.TryComp(entityUid, out component) && this.HasAnyTag(component, tags);
  }

  public bool HasTag(TagComponent component, [ForbidLiteral] ProtoId<TagPrototype> tag)
  {
    return component.Tags.Contains(tag);
  }

  public bool HasAllTags(TagComponent component, [ForbidLiteral] ProtoId<TagPrototype> tag)
  {
    return this.HasTag(component, tag);
  }

  public bool HasAllTags(TagComponent component, [ForbidLiteral] params ProtoId<TagPrototype>[] tags)
  {
    foreach (ProtoId<TagPrototype> tag in tags)
    {
      if (!component.Tags.Contains(tag))
        return false;
    }
    return true;
  }

  public bool HasAllTagsArray(TagComponent component, [ForbidLiteral] ProtoId<TagPrototype>[] tags)
  {
    foreach (ProtoId<TagPrototype> tag in tags)
    {
      if (!component.Tags.Contains(tag))
        return false;
    }
    return true;
  }

  public bool HasAllTags(TagComponent component, [ForbidLiteral] List<ProtoId<TagPrototype>> tags)
  {
    foreach (ProtoId<TagPrototype> tag in tags)
    {
      if (!component.Tags.Contains(tag))
        return false;
    }
    return true;
  }

  public bool HasAllTags(TagComponent component, [ForbidLiteral] HashSet<ProtoId<TagPrototype>> tags)
  {
    foreach (ProtoId<TagPrototype> tag in tags)
    {
      if (!component.Tags.Contains(tag))
        return false;
    }
    return true;
  }

  public bool HasAllTags(TagComponent component, [ForbidLiteral] IEnumerable<ProtoId<TagPrototype>> tags)
  {
    foreach (ProtoId<TagPrototype> tag in tags)
    {
      if (!component.Tags.Contains(tag))
        return false;
    }
    return true;
  }

  public bool HasAnyTag(TagComponent component, [ForbidLiteral] ProtoId<TagPrototype> tag)
  {
    return this.HasTag(component, tag);
  }

  public bool HasAnyTag(TagComponent component, [ForbidLiteral] params ProtoId<TagPrototype>[] tags)
  {
    foreach (ProtoId<TagPrototype> tag in tags)
    {
      if (component.Tags.Contains(tag))
        return true;
    }
    return false;
  }

  public bool HasAnyTag(TagComponent component, [ForbidLiteral] HashSet<ProtoId<TagPrototype>> tags)
  {
    foreach (ProtoId<TagPrototype> tag in tags)
    {
      if (component.Tags.Contains(tag))
        return true;
    }
    return false;
  }

  public bool HasAnyTag(TagComponent component, [ForbidLiteral] List<ProtoId<TagPrototype>> tags)
  {
    foreach (ProtoId<TagPrototype> tag in tags)
    {
      if (component.Tags.Contains(tag))
        return true;
    }
    return false;
  }

  public bool HasAnyTag(TagComponent component, [ForbidLiteral] IEnumerable<ProtoId<TagPrototype>> tags)
  {
    foreach (ProtoId<TagPrototype> tag in tags)
    {
      if (component.Tags.Contains(tag))
        return true;
    }
    return false;
  }

  public bool RemoveTag(EntityUid entityUid, [ForbidLiteral] ProtoId<TagPrototype> tag)
  {
    TagComponent component;
    return this._tagQuery.TryComp(entityUid, out component) && this.RemoveTag((Entity<TagComponent>) (entityUid, component), tag);
  }

  public bool RemoveTags(EntityUid entityUid, [ForbidLiteral] params ProtoId<TagPrototype>[] tags)
  {
    return this.RemoveTags(entityUid, (IEnumerable<ProtoId<TagPrototype>>) tags);
  }

  public bool RemoveTags(EntityUid entityUid, [ForbidLiteral] IEnumerable<ProtoId<TagPrototype>> tags)
  {
    TagComponent component;
    return this._tagQuery.TryComp(entityUid, out component) && this.RemoveTags((Entity<TagComponent>) (entityUid, component), tags);
  }

  public bool AddTag(Entity<TagComponent> entity, [ForbidLiteral] ProtoId<TagPrototype> tag)
  {
    if (!entity.Comp.Tags.Add(tag))
      return false;
    this.Dirty<TagComponent>(entity);
    return true;
  }

  public bool AddTags(Entity<TagComponent> entity, [ForbidLiteral] params ProtoId<TagPrototype>[] tags)
  {
    return this.AddTags(entity, (IEnumerable<ProtoId<TagPrototype>>) tags);
  }

  public bool AddTags(Entity<TagComponent> entity, [ForbidLiteral] IEnumerable<ProtoId<TagPrototype>> tags)
  {
    bool flag = false;
    foreach (ProtoId<TagPrototype> tag in tags)
    {
      if (entity.Comp.Tags.Add(tag) && !flag)
        flag = true;
    }
    if (!flag)
      return false;
    this.Dirty<TagComponent>(entity);
    return true;
  }

  public bool RemoveTag(Entity<TagComponent> entity, [ForbidLiteral] ProtoId<TagPrototype> tag)
  {
    if (!entity.Comp.Tags.Remove(tag))
      return false;
    this.Dirty<TagComponent>(entity);
    return true;
  }

  public bool RemoveTags(Entity<TagComponent> entity, [ForbidLiteral] params ProtoId<TagPrototype>[] tags)
  {
    return this.RemoveTags(entity, (IEnumerable<ProtoId<TagPrototype>>) tags);
  }

  public bool RemoveTags(Entity<TagComponent> entity, [ForbidLiteral] IEnumerable<ProtoId<TagPrototype>> tags)
  {
    bool flag = false;
    foreach (ProtoId<TagPrototype> tag in tags)
    {
      if (entity.Comp.Tags.Remove(tag) && !flag)
        flag = true;
    }
    if (!flag)
      return false;
    this.Dirty<TagComponent>(entity);
    return true;
  }

  private void AssertValidTag(string id)
  {
  }
}
