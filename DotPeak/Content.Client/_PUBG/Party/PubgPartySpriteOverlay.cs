// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Party.PubgPartySpriteOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Party;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.Party;

public sealed class PubgPartySpriteOverlay : Overlay
{
  private readonly IEntityManager _ent;
  private readonly IPlayerManager _player;
  private readonly PubgPartyClientSystem _party;
  private readonly SharedTransformSystem _xform;
  private readonly SpriteSystem _spriteSystem;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public float Opacity { get; set; } = 1f;

  public PubgPartySpriteOverlay(
    IEntityManager ent,
    IPlayerManager player,
    PubgPartyClientSystem party)
  {
    this._ent = ent;
    this._player = player;
    this._party = party;
    this._xform = ent.System<SharedTransformSystem>();
    this._spriteSystem = ent.System<SpriteSystem>();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if ((double) this.Opacity <= 0.0)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    NetEntity netEntity = this._ent.GetNetEntity(localEntity.Value, (MetaDataComponent) null);
    IEye eye = args.Viewport.Eye;
    Angle angle1 = eye != null ? eye.Rotation : Angle.Zero;
    foreach (PubgPartyMemberState member in (IEnumerable<PubgPartyMemberState>) this._party.Members)
    {
      if (!NetEntity.op_Equality(member.Entity, netEntity))
      {
        EntityUid entity = this._ent.GetEntity(member.Entity);
        TransformComponent transformComponent;
        SpriteComponent spriteComponent;
        if (this._ent.TryGetComponent<TransformComponent>(entity, ref transformComponent) && this._ent.TryGetComponent<SpriteComponent>(entity, ref spriteComponent) && !MapId.op_Inequality(transformComponent.MapID, args.MapId))
        {
          (Vector2 vector2, Angle angle2) = this._xform.GetWorldPositionRotation(transformComponent);
          if ((double) this.Opacity < 0.99900001287460327)
          {
            Color color = spriteComponent.Color;
            this._spriteSystem.SetColor(Entity<SpriteComponent>.op_Implicit((entity, spriteComponent)), ((Color) ref color).WithAlpha(color.A * this.Opacity));
            this._spriteSystem.RenderSprite(Entity<SpriteComponent>.op_Implicit((entity, spriteComponent)), ((OverlayDrawArgs) ref args).WorldHandle, angle1, angle2, vector2);
            this._spriteSystem.SetColor(Entity<SpriteComponent>.op_Implicit((entity, spriteComponent)), color);
          }
          else
            this._spriteSystem.RenderSprite(Entity<SpriteComponent>.op_Implicit((entity, spriteComponent)), ((OverlayDrawArgs) ref args).WorldHandle, angle1, angle2, vector2);
        }
      }
    }
  }
}
