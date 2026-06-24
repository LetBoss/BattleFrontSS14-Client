// Decompiled with JetBrains decompiler
// Type: Content.Shared.Teleportation.Systems.LinkedEntitySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Teleportation.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Shared.Teleportation.Systems;

public sealed class LinkedEntitySystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<LinkedEntityComponent, ComponentShutdown>(new ComponentEventHandler<LinkedEntityComponent, ComponentShutdown>(this.OnLinkShutdown));
  }

  private void OnLinkShutdown(
    EntityUid uid,
    LinkedEntityComponent component,
    ComponentShutdown args)
  {
    foreach (EntityUid entityUid in component.LinkedEntities.ToArray<EntityUid>())
    {
      LinkedEntityComponent comp;
      if (!this.Deleted(entityUid) && this.LifeStage(entityUid) < EntityLifeStage.Terminating && this.TryComp<LinkedEntityComponent>(entityUid, out comp))
        this.TryUnlink(uid, entityUid, component, comp);
    }
  }

  public bool TryLink(EntityUid first, EntityUid second, bool deleteOnEmptyLinks = false)
  {
    LinkedEntityComponent linkedEntityComponent1 = this.EnsureComp<LinkedEntityComponent>(first);
    LinkedEntityComponent linkedEntityComponent2 = this.EnsureComp<LinkedEntityComponent>(second);
    linkedEntityComponent1.DeleteOnEmptyLinks = deleteOnEmptyLinks;
    linkedEntityComponent2.DeleteOnEmptyLinks = deleteOnEmptyLinks;
    this._appearance.SetData(first, (Enum) LinkedEntityVisuals.HasAnyLinks, (object) true);
    this._appearance.SetData(second, (Enum) LinkedEntityVisuals.HasAnyLinks, (object) true);
    this.Dirty(first, (IComponent) linkedEntityComponent1);
    this.Dirty(second, (IComponent) linkedEntityComponent2);
    return linkedEntityComponent1.LinkedEntities.Add(second) && linkedEntityComponent2.LinkedEntities.Add(first);
  }

  public bool OneWayLink(EntityUid source, EntityUid target, bool deleteOnEmptyLinks = false)
  {
    LinkedEntityComponent linkedEntityComponent = this.EnsureComp<LinkedEntityComponent>(source);
    linkedEntityComponent.DeleteOnEmptyLinks = deleteOnEmptyLinks;
    this._appearance.SetData(source, (Enum) LinkedEntityVisuals.HasAnyLinks, (object) true);
    this.Dirty(source, (IComponent) linkedEntityComponent);
    return linkedEntityComponent.LinkedEntities.Add(target);
  }

  public bool TryUnlink(
    EntityUid first,
    EntityUid second,
    LinkedEntityComponent? firstLink = null,
    LinkedEntityComponent? secondLink = null)
  {
    if (!this.Resolve<LinkedEntityComponent>(first, ref firstLink) || !this.Resolve<LinkedEntityComponent>(second, ref secondLink))
      return false;
    int num = !firstLink.LinkedEntities.Remove(second) ? 0 : (secondLink.LinkedEntities.Remove(first) ? 1 : 0);
    this._appearance.SetData(first, (Enum) LinkedEntityVisuals.HasAnyLinks, (object) firstLink.LinkedEntities.Any<EntityUid>());
    this._appearance.SetData(second, (Enum) LinkedEntityVisuals.HasAnyLinks, (object) secondLink.LinkedEntities.Any<EntityUid>());
    this.Dirty(first, (IComponent) firstLink);
    this.Dirty(second, (IComponent) secondLink);
    if (firstLink.LinkedEntities.Count == 0 && firstLink.DeleteOnEmptyLinks)
      this.QueueDel(new EntityUid?(first));
    if (secondLink.LinkedEntities.Count != 0)
      return num != 0;
    if (!secondLink.DeleteOnEmptyLinks)
      return num != 0;
    this.QueueDel(new EntityUid?(second));
    return num != 0;
  }

  public bool GetLink(EntityUid uid, [NotNullWhen(true)] out EntityUid? dest, LinkedEntityComponent? comp = null)
  {
    dest = new EntityUid?();
    if (!this.Resolve<LinkedEntityComponent>(uid, ref comp, false))
      return false;
    EntityUid entityUid = comp.LinkedEntities.FirstOrDefault<EntityUid>();
    if (!(entityUid != new EntityUid()))
      return false;
    dest = new EntityUid?(entityUid);
    return true;
  }
}
