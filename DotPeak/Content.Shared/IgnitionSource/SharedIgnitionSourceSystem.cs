// Decompiled with JetBrains decompiler
// Type: Content.Shared.IgnitionSource.SharedIgnitionSourceSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Temperature;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.IgnitionSource;

public abstract class SharedIgnitionSourceSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<IgnitionSourceComponent, IsHotEvent>(new EntityEventRefHandler<IgnitionSourceComponent, IsHotEvent>(this.OnIsHot));
    this.SubscribeLocalEvent<ItemToggleHotComponent, ItemToggledEvent>(new EntityEventRefHandler<ItemToggleHotComponent, ItemToggledEvent>(this.OnItemToggle));
    this.SubscribeLocalEvent<IgnitionSourceComponent, IgnitionEvent>(new EntityEventRefHandler<IgnitionSourceComponent, IgnitionEvent>(this.OnIgnitionEvent));
  }

  private void OnIsHot(Entity<IgnitionSourceComponent> ent, ref IsHotEvent args)
  {
    args.IsHot |= ent.Comp.Ignited;
  }

  private void OnItemToggle(Entity<ItemToggleHotComponent> ent, ref ItemToggledEvent args)
  {
    this.SetIgnited((Entity<IgnitionSourceComponent>) ent.Owner, args.Activated);
  }

  private void OnIgnitionEvent(Entity<IgnitionSourceComponent> ent, ref IgnitionEvent args)
  {
    this.SetIgnited((Entity<IgnitionSourceComponent>) (ent.Owner, ent.Comp), args.Ignite);
  }

  public void SetIgnited(Entity<IgnitionSourceComponent?> ent, bool ignited = true)
  {
    if (!this.Resolve<IgnitionSourceComponent>((EntityUid) ent, ref ent.Comp, false))
      return;
    ent.Comp.Ignited = ignited;
    this.Dirty((EntityUid) ent, (IComponent) ent.Comp);
  }
}
