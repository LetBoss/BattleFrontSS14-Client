// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.NewPlayer.NewPlayerVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.NewPlayer;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Utility;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.NewPlayer;

public sealed class NewPlayerVisualizerSystem : VisualizerSystem<NewPlayerLabelComponent>
{
  [Dependency]
  private IPlayerManager _player;
  private EntityQuery<SeeNewPlayersComponent> _seeNewPlayersQuery;

  public virtual void Initialize()
  {
    base.Initialize();
    this._seeNewPlayersQuery = ((EntitySystem) this).GetEntityQuery<SeeNewPlayersComponent>();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<SeeNewPlayersComponent, LocalPlayerAttachedEvent>(new EntityEventRefHandler<SeeNewPlayersComponent, LocalPlayerAttachedEvent>((object) this, __methodptr(OnSeeNewPlayersLocalAttached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<SeeNewPlayersComponent, LocalPlayerDetachedEvent>(new EntityEventRefHandler<SeeNewPlayersComponent, LocalPlayerDetachedEvent>((object) this, __methodptr(OnSeeNewPlayersLocalDetached)), (Type[]) null, (Type[]) null);
  }

  private void OnSeeNewPlayersLocalAttached(
    Entity<SeeNewPlayersComponent> ent,
    ref LocalPlayerAttachedEvent args)
  {
    this.UpdateAllAppearance();
  }

  private void OnSeeNewPlayersLocalDetached(
    Entity<SeeNewPlayersComponent> ent,
    ref LocalPlayerDetachedEvent args)
  {
    this.UpdateAllAppearance();
  }

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    NewPlayerLabelComponent component,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    this.UpdateAppearance(Entity<AppearanceComponent, SpriteComponent>.op_Implicit((uid, args.Component, args.Sprite)));
  }

  private void UpdateAllAppearance()
  {
    AllEntityQueryEnumerator<NewPlayerLabelComponent, AppearanceComponent, SpriteComponent> entityQueryEnumerator = ((EntitySystem) this).AllEntityQuery<NewPlayerLabelComponent, AppearanceComponent, SpriteComponent>();
    EntityUid entityUid;
    NewPlayerLabelComponent playerLabelComponent;
    AppearanceComponent appearanceComponent;
    SpriteComponent spriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref playerLabelComponent, ref appearanceComponent, ref spriteComponent))
      this.UpdateAppearance(Entity<AppearanceComponent, SpriteComponent>.op_Implicit((entityUid, appearanceComponent, spriteComponent)));
  }

  private void UpdateAppearance(Entity<AppearanceComponent, SpriteComponent> ent)
  {
    int num;
    if (!this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((ent.Owner, ent.Comp2)), (Enum) NewPlayerLayers.Layer, ref num, false))
      return;
    SeeNewPlayersComponent playersComponent;
    NewPlayerVisuals newPlayerVisuals;
    if (!this._seeNewPlayersQuery.TryComp(((ISharedPlayerManager) this._player).LocalEntity, ref playersComponent) || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<NewPlayerVisuals>(Entity<AppearanceComponent, SpriteComponent>.op_Implicit(ent), (Enum) NewPlayerLayers.Layer, ref newPlayerVisuals, Entity<AppearanceComponent, SpriteComponent>.op_Implicit(ent)))
    {
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, ent.Comp2)), num, false);
    }
    else
    {
      SpriteSpecifier.Rsi rsi1;
      switch (newPlayerVisuals)
      {
        case NewPlayerVisuals.One:
          rsi1 = playersComponent.OneLabel;
          break;
        case NewPlayerVisuals.Two:
          rsi1 = playersComponent.TwoLabel;
          break;
        case NewPlayerVisuals.Three:
          rsi1 = playersComponent.ThreeLabel;
          break;
        case NewPlayerVisuals.Four:
          rsi1 = playersComponent.FourLabel;
          break;
        default:
          rsi1 = (SpriteSpecifier.Rsi) null;
          break;
      }
      SpriteSpecifier.Rsi rsi2 = rsi1;
      if (rsi2 == null)
        return;
      this.SpriteSystem.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((ent.Owner, ent.Comp2)), num, (SpriteSpecifier) rsi2);
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, ent.Comp2)), num, true);
      this.SpriteSystem.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((ent.Owner, ent.Comp2)), num, new Vector2(0.0f, 0.21f));
    }
  }
}
