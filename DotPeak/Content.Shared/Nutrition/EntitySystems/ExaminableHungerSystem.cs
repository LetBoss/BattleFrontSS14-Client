// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.EntitySystems.ExaminableHungerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Nutrition.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

#nullable enable
namespace Content.Shared.Nutrition.EntitySystems;

public sealed class ExaminableHungerSystem : EntitySystem
{
  [Dependency]
  private HungerSystem _hunger;
  private Robust.Shared.GameObjects.EntityQuery<HungerComponent> _hungerQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._hungerQuery = this.GetEntityQuery<HungerComponent>();
    this.SubscribeLocalEvent<ExaminableHungerComponent, ExaminedEvent>(new EntityEventRefHandler<ExaminableHungerComponent, ExaminedEvent>(this.OnExamine));
  }

  private void OnExamine(Entity<ExaminableHungerComponent> entity, ref ExaminedEvent args)
  {
    EntityUid entityUid = Identity.Entity((EntityUid) entity, (IEntityManager) this.EntityManager);
    HungerComponent component;
    LocId hungerDescription;
    if (!this._hungerQuery.TryComp((EntityUid) entity, out component) || !entity.Comp.Descriptions.TryGetValue(this._hunger.GetHungerThreshold(component), out hungerDescription))
      hungerDescription = entity.Comp.NoHungerDescription;
    string markup = this.Loc.GetString((string) hungerDescription, (nameof (entity), (object) entityUid));
    args.PushMarkup(markup);
  }
}
