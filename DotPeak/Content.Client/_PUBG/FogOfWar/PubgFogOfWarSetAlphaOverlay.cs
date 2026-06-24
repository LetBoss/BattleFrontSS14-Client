// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.FogOfWar.PubgFogOfWarSetAlphaOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._PUBG.FogOfWar;

public sealed class PubgFogOfWarSetAlphaOverlay : Overlay
{
  [Dependency]
  private IEntityManager _ent;
  private readonly PubgFogOfWarHideSystem _hide;
  private readonly PubgFogOfWarOccludableTreeSystem _tree;
  private readonly SpriteSystem _sprite;
  private readonly EntityQuery<SpriteComponent> _spriteQuery;
  private readonly HashSet<EntityUid> _seen = new HashSet<EntityUid>();
  private bool _ready;

  private static bool QueryCallback(
    ref PubgFogOfWarSetAlphaOverlay.QueryState state,
    in ComponentTreeEntry<PubgFogOfWarOccludableComponent> entry)
  {
    EntityUid uid = entry.Uid;
    SpriteComponent sprite;
    if (!state.Seen.Add(uid) || !state.SpriteQuery.TryComp(uid, ref sprite))
      return true;
    float targetAlpha = state.HideSystem.GetTargetAlpha(uid, sprite, entry.Transform);
    if ((double) MathF.Abs(sprite.Color.A - targetAlpha) <= 1.0 / 1000.0)
      return true;
    Entity<SpriteComponent> entity1 = Entity<SpriteComponent>.op_Implicit((uid, sprite));
    state.HideSystem.CachedBaseAlphas.Add((entity1, sprite.Color.A));
    SpriteSystem spriteSystem = state.SpriteSystem;
    Entity<SpriteComponent> entity2 = entity1;
    Color color1 = sprite.Color;
    Color color2 = ((Color) ref color1).WithAlpha(targetAlpha);
    spriteSystem.SetColor(entity2, color2);
    return true;
  }

  public virtual OverlaySpace Space => (OverlaySpace) 64 /*0x40*/;

  public PubgFogOfWarSetAlphaOverlay()
  {
    IoCManager.InjectDependencies<PubgFogOfWarSetAlphaOverlay>(this);
    this._hide = this._ent.System<PubgFogOfWarHideSystem>();
    this._tree = this._ent.System<PubgFogOfWarOccludableTreeSystem>();
    this._sprite = this._ent.System<SpriteSystem>();
    this._spriteQuery = this._ent.GetEntityQuery<SpriteComponent>();
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    this._ready = this._hide.TryPrepare();
    if (!this._ready)
      return false;
    this._hide.CachedBaseAlphas.Clear();
    this._seen.Clear();
    return true;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (!this._ready)
      return;
    PubgFogOfWarSetAlphaOverlay.QueryState queryState = new PubgFogOfWarSetAlphaOverlay.QueryState()
    {
      Seen = this._seen,
      SpriteQuery = this._spriteQuery,
      SpriteSystem = this._sprite,
      HideSystem = this._hide
    };
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    this._tree.QueryAabb<PubgFogOfWarSetAlphaOverlay.QueryState>(ref queryState, PubgFogOfWarSetAlphaOverlay.\u003C\u003EO.\u003C0\u003E__QueryCallback ?? (PubgFogOfWarSetAlphaOverlay.\u003C\u003EO.\u003C0\u003E__QueryCallback = new DynamicTree<ComponentTreeEntry<PubgFogOfWarOccludableComponent>>.QueryCallbackDelegate<PubgFogOfWarSetAlphaOverlay.QueryState>((object) null, __methodptr(QueryCallback))), args.MapId, args.WorldBounds, true);
  }

  private struct QueryState
  {
    public HashSet<EntityUid> Seen;
    public EntityQuery<SpriteComponent> SpriteQuery;
    public SpriteSystem SpriteSystem;
    public PubgFogOfWarHideSystem HideSystem;
  }
}
