// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.EntitySystems.BlockAnchorOnSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Construction.Components;
using Content.Shared.Popups;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Shared.Construction.EntitySystems;

public sealed class BlockAnchorOnSystem : EntitySystem
{
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedTransformSystem _xform;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlockAnchorOnComponent, AnchorStateChangedEvent>(new EntityEventRefHandler<BlockAnchorOnComponent, AnchorStateChangedEvent>((object) this, __methodptr(OnAnchorStateChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlockAnchorOnComponent, AnchorAttemptEvent>(new EntityEventRefHandler<BlockAnchorOnComponent, AnchorAttemptEvent>((object) this, __methodptr(OnAnchorAttempt)), (Type[]) null, (Type[]) null);
  }

  private void OnAnchorStateChanged(
    Entity<BlockAnchorOnComponent> ent,
    ref AnchorStateChangedEvent args)
  {
    if (!((AnchorStateChangedEvent) ref args).Anchored || !this.HasOverlap(Entity<BlockAnchorOnComponent, TransformComponent>.op_Implicit((Entity<BlockAnchorOnComponent>.op_Implicit(ent), ent.Comp, this.Transform(Entity<BlockAnchorOnComponent>.op_Implicit(ent))))))
      return;
    this._popup.PopupPredicted(this.Loc.GetString("anchored-already-present"), Entity<BlockAnchorOnComponent>.op_Implicit(ent), new EntityUid?());
    this._xform.Unanchor(Entity<BlockAnchorOnComponent>.op_Implicit(ent), this.Transform(Entity<BlockAnchorOnComponent>.op_Implicit(ent)), true);
  }

  private void OnAnchorAttempt(Entity<BlockAnchorOnComponent> ent, ref AnchorAttemptEvent args)
  {
    if (args.Cancelled || !this.HasOverlap(Entity<BlockAnchorOnComponent, TransformComponent>.op_Implicit((Entity<BlockAnchorOnComponent>.op_Implicit(ent), ent.Comp, this.Transform(Entity<BlockAnchorOnComponent>.op_Implicit(ent))))))
      return;
    this._popup.PopupPredicted(this.Loc.GetString("anchored-already-present"), Entity<BlockAnchorOnComponent>.op_Implicit(ent), new EntityUid?(args.User));
    args.Cancel();
  }

  private bool HasOverlap(
    Entity<BlockAnchorOnComponent, TransformComponent> ent)
  {
    EntityUid? gridUid = ent.Comp2.GridUid;
    if (gridUid.HasValue)
    {
      EntityUid valueOrDefault = gridUid.GetValueOrDefault();
      MapGridComponent mapGridComponent;
      if (this.TryComp<MapGridComponent>(valueOrDefault, ref mapGridComponent))
      {
        Vector2i vector2i = this._map.TileIndicesFor(valueOrDefault, mapGridComponent, ent.Comp2.Coordinates);
        AnchoredEntitiesEnumerator entitiesEnumerator = this._map.GetAnchoredEntitiesEnumerator(valueOrDefault, mapGridComponent, vector2i);
        EntityUid? uid;
        while (((AnchoredEntitiesEnumerator) ref entitiesEnumerator).MoveNext(ref uid))
        {
          EntityUid? nullable = uid;
          EntityUid entityUid = Entity<BlockAnchorOnComponent, TransformComponent>.op_Implicit(ent);
          if ((nullable.HasValue ? (EntityUid.op_Equality(nullable.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0 && !this._whitelist.CheckBoth(uid, ent.Comp1.Blacklist, ent.Comp1.Whitelist))
            return true;
        }
        return false;
      }
    }
    return false;
  }
}
