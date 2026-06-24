// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Water.RMCWaterSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Map;
using Content.Shared.NameModifier.Components;
using Content.Shared.NameModifier.EntitySystems;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable
namespace Content.Shared._RMC14.Water;

public sealed class RMCWaterSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private EntityWhitelistSystem _entityWhitelist;
  [Dependency]
  private NameModifierSystem _nameModifier;
  [Dependency]
  private RMCMapSystem _rmcMap;
  [Dependency]
  private IGameTiming _timing;
  private readonly List<(EntityUid Id, TimeSpan SpreadAt)> _makeActive = new List<(EntityUid, TimeSpan)>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<PurifiableWaterComponent, MapInitEvent>(new EntityEventRefHandler<PurifiableWaterComponent, MapInitEvent>(this.OnPurifiableWaterMapInit));
    this.SubscribeLocalEvent<PurifiableWaterComponent, RefreshNameModifiersEvent>(new EntityEventRefHandler<PurifiableWaterComponent, RefreshNameModifiersEvent>(this.OnPurifiableWaterRefreshNameModifiers));
  }

  private void OnPurifiableWaterMapInit(Entity<PurifiableWaterComponent> ent, ref MapInitEvent args)
  {
    this.UpdateAppearance(ent);
  }

  private void OnPurifiableWaterRefreshNameModifiers(
    Entity<PurifiableWaterComponent> ent,
    ref RefreshNameModifiersEvent args)
  {
    string str = ent.Comp.Toxic ? "rmc-water-toxic-name" : "rmc-water-purified-name";
    args.AddModifier((LocId) str);
  }

  private void UpdateAppearance(Entity<PurifiableWaterComponent> ent)
  {
    PurifiableWaterVisuals purifiableWaterVisuals = ent.Comp.Toxic ? PurifiableWaterVisuals.Toxic : PurifiableWaterVisuals.Purified;
    this._appearance.SetData(ent.Owner, (Enum) PurifiableWaterLayers.Layer, (object) purifiableWaterVisuals);
    this._nameModifier.RefreshNameModifiers((Entity<NameModifierComponent>) ent.Owner);
  }

  public bool CanCollide(Entity<RMCWaterComponent?> water, EntityUid user)
  {
    if (!this.Resolve<RMCWaterComponent>((EntityUid) water, ref water.Comp, false))
      return true;
    EntityWhitelist cover = water.Comp.Cover;
    if (cover == null)
      return true;
    RMCAnchoredEntitiesEnumerator entitiesEnumerator = this._rmcMap.GetAnchoredEntitiesEnumerator((EntityUid) water, facing: (DirectionFlag) 0);
    EntityUid uid;
    while (entitiesEnumerator.MoveNext(out uid))
    {
      if (this._entityWhitelist.IsWhitelistPass(cover, uid))
        return false;
    }
    return true;
  }

  public override void Update(float frameTime)
  {
    this._makeActive.Clear();
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActiveWaterComponent, PurifiableWaterComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActiveWaterComponent, PurifiableWaterComponent>();
    EntityUid uid1;
    ActiveWaterComponent comp1;
    PurifiableWaterComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid1, out comp1, out comp2))
    {
      if (!(curTime < comp1.SpreadAt))
      {
        ImmutableArray<Direction>.Enumerator enumerator = this._rmcMap.CardinalDirections.GetEnumerator();
label_8:
        while (enumerator.MoveNext())
        {
          Direction current = enumerator.Current;
          RMCAnchoredEntitiesEnumerator entitiesEnumerator = this._rmcMap.GetAnchoredEntitiesEnumerator(uid1, new Direction?(current), (DirectionFlag) 0);
          while (true)
          {
            EntityUid uid2;
            PurifiableWaterComponent comp;
            do
            {
              if (!entitiesEnumerator.MoveNext(out uid2))
                goto label_8;
            }
            while (!this.TryComp<PurifiableWaterComponent>(uid2, out comp) || comp.Toxic == comp2.Toxic);
            comp.Toxic = comp2.Toxic;
            this.Dirty(uid2, (IComponent) comp);
            this.UpdateAppearance((Entity<PurifiableWaterComponent>) (uid2, comp));
            this._makeActive.Add((uid2, curTime + comp.Delay));
          }
        }
      }
    }
    foreach ((EntityUid entityUid, TimeSpan SpreadAt) in this._makeActive)
    {
      ActiveWaterComponent activeWaterComponent = this.EnsureComp<ActiveWaterComponent>(entityUid);
      activeWaterComponent.SpreadAt = SpreadAt;
      this.Dirty(entityUid, (IComponent) activeWaterComponent);
    }
  }
}
