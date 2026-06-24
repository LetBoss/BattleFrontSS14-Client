// Decompiled with JetBrains decompiler
// Type: Content.Shared.Item.ItemToggle.ComponentTogglerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Item.ItemToggle.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Item.ItemToggle;

public sealed class ComponentTogglerSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ComponentTogglerComponent, ItemToggledEvent>(new EntityEventRefHandler<ComponentTogglerComponent, ItemToggledEvent>(this.OnToggled));
  }

  private void OnToggled(Entity<ComponentTogglerComponent> ent, ref ItemToggledEvent args)
  {
    if (args.Activated)
    {
      EntityUid entityUid = ent.Comp.Parent ? this.Transform((EntityUid) ent).ParentUid : ent.Owner;
      if (this.TerminatingOrDeleted(entityUid))
        return;
      ent.Comp.Target = new EntityUid?(entityUid);
      this.EntityManager.AddComponents(entityUid, ent.Comp.Components, true);
    }
    else
    {
      if (!ent.Comp.Target.HasValue || this.TerminatingOrDeleted(ent.Comp.Target.Value))
        return;
      this.EntityManager.RemoveComponents(ent.Comp.Target.Value, ent.Comp.RemoveComponents ?? ent.Comp.Components);
    }
  }
}
