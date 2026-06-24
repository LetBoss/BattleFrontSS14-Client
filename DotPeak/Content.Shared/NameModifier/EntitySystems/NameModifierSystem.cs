// Decompiled with JetBrains decompiler
// Type: Content.Shared.NameModifier.EntitySystems.NameModifierSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.NameModifier.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.NameModifier.EntitySystems;

public sealed class NameModifierSystem : EntitySystem
{
  [Dependency]
  private MetaDataSystem _metaData;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<NameModifierComponent, EntityRenamedEvent>(new EntityEventRefHandler<NameModifierComponent, EntityRenamedEvent>(this.OnEntityRenamed));
  }

  private void OnEntityRenamed(Entity<NameModifierComponent> ent, ref EntityRenamedEvent args)
  {
    this.SetBaseName(ent, args.NewName);
    this.RefreshNameModifiers((Entity<NameModifierComponent>) (ent.Owner, ent.Comp));
  }

  private void SetBaseName(Entity<NameModifierComponent> entity, string name)
  {
    if (name == entity.Comp.BaseName)
      return;
    entity.Comp.BaseName = name;
    this.Dirty<NameModifierComponent>(entity);
  }

  public string GetBaseName(Entity<NameModifierComponent?> entity)
  {
    return this.Resolve<NameModifierComponent>((EntityUid) entity, ref entity.Comp, false) ? entity.Comp.BaseName : this.Name((EntityUid) entity);
  }

  public void RefreshNameModifiers(Entity<NameModifierComponent?> entity)
  {
    MetaDataComponent metadata = this.MetaData((EntityUid) entity);
    string baseName = metadata.EntityName;
    if (this.Resolve<NameModifierComponent>((EntityUid) entity, ref entity.Comp, false))
      baseName = entity.Comp.BaseName;
    RefreshNameModifiersEvent args = new RefreshNameModifiersEvent(baseName);
    this.RaiseLocalEvent<RefreshNameModifiersEvent>((EntityUid) entity, ref args);
    if (args.ModifierCount == 0)
    {
      if (entity.Comp == null)
        return;
      this._metaData.SetEntityName((EntityUid) entity, entity.Comp.BaseName, metadata, false);
      this.RemComp<NameModifierComponent>((EntityUid) entity);
    }
    else
    {
      string modifiedName = args.GetModifiedName();
      NameModifierComponent comp;
      if (!this.EnsureComp<NameModifierComponent>((EntityUid) entity, out comp))
        this.SetBaseName((Entity<NameModifierComponent>) ((EntityUid) entity, comp), metadata.EntityName);
      this._metaData.SetEntityName((EntityUid) entity, modifiedName, metadata, false);
    }
  }
}
