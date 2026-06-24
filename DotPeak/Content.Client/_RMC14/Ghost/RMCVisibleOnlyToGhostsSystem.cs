// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Ghost.RMCVisibleOnlyToGhostsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Ghost;
using Content.Shared.Ghost;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client._RMC14.Ghost;

public sealed class RMCVisibleOnlyToGhostsSystem : EntitySystem
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Update(float frameTime)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityQueryEnumerator<RMCVisibleToGhostsOnlyComponent, SpriteComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCVisibleToGhostsOnlyComponent, SpriteComponent>();
    bool flag = this.HasComp<GhostComponent>(localEntity);
    EntityUid entityUid;
    RMCVisibleToGhostsOnlyComponent ghostsOnlyComponent;
    SpriteComponent spriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref ghostsOnlyComponent, ref spriteComponent))
    {
      int num;
      if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), (Enum) RMCGhostVisibleOnlyVisualLayers.Base, ref num, true))
        this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), num, flag);
    }
  }
}
