// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Party.PubgPartyWorldOverlay
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
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.Party;

public sealed class PubgPartyWorldOverlay : Overlay
{
  private readonly IEntityManager _entityManager;
  private readonly IPlayerManager _player;
  private readonly SharedTransformSystem _transform;
  private readonly SpriteSystem _spriteSystem;
  private readonly PubgPartyClientSystem _party;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public PubgPartyWorldOverlay(
    IEntityManager entityManager,
    IPlayerManager player,
    SharedTransformSystem transform,
    PubgPartyClientSystem party)
  {
    this._entityManager = entityManager;
    this._player = player;
    this._transform = transform;
    this._spriteSystem = entityManager.System<SpriteSystem>();
    this._party = party;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    NetEntity netEntity = this._entityManager.GetNetEntity(localEntity.Value, (MetaDataComponent) null);
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    foreach (PubgPartyMemberState member in (IEnumerable<PubgPartyMemberState>) this._party.Members)
    {
      if (!NetEntity.op_Equality(member.Entity, netEntity))
      {
        EntityUid entity1 = this._entityManager.GetEntity(member.Entity);
        TransformComponent transformComponent;
        SpriteComponent spriteComponent;
        if (this._entityManager.TryGetComponent<TransformComponent>(entity1, ref transformComponent) && this._entityManager.TryGetComponent<SpriteComponent>(entity1, ref spriteComponent) && !MapId.op_Inequality(transformComponent.MapID, args.MapId))
        {
          (Vector2 vector2_1, Angle angle1) = this._transform.GetWorldPositionRotation(transformComponent);
          SpriteSystem spriteSystem = this._spriteSystem;
          Entity<SpriteComponent> entity2 = Entity<SpriteComponent>.op_Implicit((entity1, spriteComponent));
          Vector2 vector2_2 = vector2_1;
          Angle angle2 = angle1;
          IEye eye = args.Viewport.Eye;
          Angle angle3 = eye != null ? eye.Rotation : new Angle();
          Box2Rotated bounds = spriteSystem.CalculateBounds(entity2, vector2_2, angle2, angle3);
          Color partyColor = PubgPartyWorldOverlay.GetPartyColor(member.SlotIndex);
          worldHandle.DrawRect(ref bounds, ((Color) ref partyColor).WithAlpha(0.8f), false);
        }
      }
    }
  }

  private static Color GetPartyColor(int slotIndex)
  {
    Color partyColor;
    switch (slotIndex)
    {
      case 1:
        partyColor = Color.FromHex((ReadOnlySpan<char>) "#00bcd4", new Color?());
        break;
      case 2:
        partyColor = Color.FromHex((ReadOnlySpan<char>) "#ffeb3b", new Color?());
        break;
      case 3:
        partyColor = Color.FromHex((ReadOnlySpan<char>) "#ff9800", new Color?());
        break;
      default:
        partyColor = Color.FromHex((ReadOnlySpan<char>) "#4caf50", new Color?());
        break;
    }
    return partyColor;
  }
}
