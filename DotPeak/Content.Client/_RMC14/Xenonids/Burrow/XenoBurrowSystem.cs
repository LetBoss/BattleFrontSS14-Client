// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Burrow.XenoBurrowSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.NightVision;
using Content.Shared._RMC14.Xenonids.Burrow;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Burrow;

public sealed class XenoBurrowSystem : SharedXenoBurrowSystem
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityQueryEnumerator<XenoBurrowComponent, SpriteComponent, RMCNightVisionVisibleComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoBurrowComponent, SpriteComponent, RMCNightVisionVisibleComponent>();
    EntityUid entityUid1;
    XenoBurrowComponent xenoBurrowComponent;
    SpriteComponent spriteComponent;
    RMCNightVisionVisibleComponent visibleComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid1, ref xenoBurrowComponent, ref spriteComponent, ref visibleComponent))
    {
      EntityUid? nullable = localEntity;
      EntityUid entityUid2 = entityUid1;
      if ((nullable.HasValue ? (EntityUid.op_Inequality(nullable.GetValueOrDefault(), entityUid2) ? 1 : 0) : 1) != 0)
      {
        this._sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((entityUid1, spriteComponent)), !xenoBurrowComponent.Active);
      }
      else
      {
        SpriteSystem sprite = this._sprite;
        Entity<SpriteComponent> entity = Entity<SpriteComponent>.op_Implicit((entityUid1, spriteComponent));
        Color color;
        if (!xenoBurrowComponent.Active)
        {
          color = Color.White;
        }
        else
        {
          Color white = Color.White;
          color = ((Color) ref white).WithAlpha(0.4f);
        }
        sprite.SetColor(entity, color);
      }
      visibleComponent.Transparency = !xenoBurrowComponent.Active ? new float?() : new float?(0.4f);
    }
  }
}
