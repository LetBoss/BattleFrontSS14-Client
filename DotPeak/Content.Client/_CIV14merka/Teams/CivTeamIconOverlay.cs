// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Teams.CivTeamIconOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Teams;
using Content.Shared.Ghost;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Containers;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Teams;

public sealed class CivTeamIconOverlay : Overlay
{
  private static readonly Color TeamColor = Color.FromHex((ReadOnlySpan<char>) "#4da6ff", new Color?());
  private static readonly Color SquadColor = Color.FromHex((ReadOnlySpan<char>) "#54ff72", new Color?());
  private static readonly Color SquadLeaderColor = Color.FromHex((ReadOnlySpan<char>) "#ffd54f", new Color?());
  private static readonly Color GhostBlueTeamColor = Color.FromHex((ReadOnlySpan<char>) "#4da6ff", new Color?());
  private static readonly Color GhostRedTeamColor = Color.FromHex((ReadOnlySpan<char>) "#ff5c5c", new Color?());
  private const float SquadBadgeXOffsetPixels = 2f;
  private const float DoubleDigitSquadBadgeXOffsetMultiplier = 2f;
  [Dependency]
  private IEntityManager _entity;
  [Dependency]
  private IPlayerManager _players;
  [Dependency]
  private IPrototypeManager _prototype;
  private readonly ContainerSystem _container;
  private readonly SharedTransformSystem _transform;
  private readonly SpriteSystem _sprite;
  private readonly MobStateSystem _mobState;
  private readonly ShaderInstance _shader;
  private readonly EntityQuery<TransformComponent> _xformQuery;

  public virtual OverlaySpace Space => (OverlaySpace) 8;

  public CivTeamIconOverlay()
  {
    IoCManager.InjectDependencies<CivTeamIconOverlay>(this);
    this._container = this._entity.System<ContainerSystem>();
    this._transform = this._entity.System<SharedTransformSystem>();
    this._sprite = this._entity.System<SpriteSystem>();
    this._mobState = this._entity.System<MobStateSystem>();
    this._shader = this._prototype.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("shaded")).Instance();
    this._xformQuery = this._entity.GetEntityQuery<TransformComponent>();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._players).LocalEntity;
    if (!localEntity.HasValue)
      return;
    bool flag = this._entity.HasComponent<GhostComponent>(localEntity.Value);
    CivTeamMemberComponent teamMemberComponent1 = (CivTeamMemberComponent) null;
    if (!flag && !this._entity.TryGetComponent<CivTeamMemberComponent>(localEntity.Value, ref teamMemberComponent1))
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    IEye eye = args.Viewport.Eye;
    Angle angle = eye != null ? eye.Rotation : new Angle();
    Matrix3x2 scale = Matrix3x2.CreateScale(Vector2.One);
    Matrix3x2 rotation = Matrix3Helpers.CreateRotation(Angle.op_Implicit(Angle.op_UnaryNegation(angle)));
    ((DrawingHandleBase) worldHandle).UseShader(this._shader);
    AllEntityQueryEnumerator<CivTeamMemberComponent, SpriteComponent, TransformComponent> entityQueryEnumerator = this._entity.AllEntityQueryEnumerator<CivTeamMemberComponent, SpriteComponent, TransformComponent>();
    EntityUid target;
    CivTeamMemberComponent teamMemberComponent2;
    SpriteComponent spriteComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref target, ref teamMemberComponent2, ref spriteComponent, ref transformComponent))
    {
      MobStateComponent component;
      if (!MapId.op_Inequality(transformComponent.MapID, args.MapId) && spriteComponent.Visible && !((SharedContainerSystem) this._container).IsEntityOrParentInContainer(target, (MetaDataComponent) null, transformComponent) && (!this._entity.TryGetComponent<MobStateComponent>(target, ref component) || !this._mobState.IsDead(target, component)))
      {
        Box2 localBounds = this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((target, spriteComponent)));
        Vector2 worldPosition = this._transform.GetWorldPosition(transformComponent, this._xformQuery);
        Box2 box2 = ((Box2) ref localBounds).Translated(worldPosition);
        if (((Box2) ref box2).Intersects(ref args.WorldAABB))
        {
          Color color1;
          if (!flag)
          {
            if (teamMemberComponent1 != null && teamMemberComponent1.TeamId > 0 && teamMemberComponent2.TeamId > 0 && teamMemberComponent1.TeamId == teamMemberComponent2.TeamId)
              color1 = teamMemberComponent1.SquadId == 0 || teamMemberComponent1.SquadId != teamMemberComponent2.SquadId ? CivTeamIconOverlay.TeamColor : (teamMemberComponent2.IsSquadLeader ? CivTeamIconOverlay.SquadLeaderColor : CivTeamIconOverlay.SquadColor);
            else
              continue;
          }
          else
          {
            Color color2;
            switch (teamMemberComponent2.TeamId)
            {
              case 1:
                color2 = CivTeamIconOverlay.GhostBlueTeamColor;
                break;
              case 2:
                color2 = CivTeamIconOverlay.GhostRedTeamColor;
                break;
              default:
                color2 = Color.White;
                break;
            }
            color1 = color2;
            if (teamMemberComponent2.TeamId <= 0 || Color.op_Equality(color1, Color.White))
              continue;
          }
          Matrix3x2 translation = Matrix3x2.CreateTranslation(worldPosition);
          Matrix3x2 matrix3x2_1 = Matrix3x2.Multiply(scale, translation);
          Matrix3x2 matrix3x2_2 = Matrix3x2.Multiply(rotation, matrix3x2_1);
          ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2_2);
          Texture texture1 = this._sprite.Frame0((SpriteSpecifier) CivTeamIconResolver.GetClassBadgeBackground(teamMemberComponent2.TeamId));
          Color classBadgeColor = CivTeamIconResolver.GetClassBadgeColor(teamMemberComponent2.TeamId);
          Texture texture2 = this._sprite.Frame0((SpriteSpecifier) CivTeamIconResolver.GetClassIcon(teamMemberComponent2.Class, teamMemberComponent2.TeamId));
          float y1 = (float) (0.10000000149011612 + ((double) ((Box2) ref localBounds).Height + (double) spriteComponent.Offset.Y) / 2.0 - (double) texture2.Height / 32.0);
          Vector2 vector2_1 = new Vector2((float) (0.10000000149011612 + ((double) ((Box2) ref localBounds).Width + (double) spriteComponent.Offset.X) / 2.0 - (double) texture2.Width / 32.0), y1);
          ((DrawingHandleBase) worldHandle).DrawTexture(texture1, vector2_1, new Color?(classBadgeColor));
          ((DrawingHandleBase) worldHandle).DrawTexture(texture2, vector2_1, new Color?());
          SpriteSpecifier.Rsi teamFlag = CivTeamIconResolver.GetTeamFlag(teamMemberComponent2.TeamId);
          if (teamFlag != null)
          {
            Texture texture3 = this._sprite.Frame0((SpriteSpecifier) teamFlag);
            float y2 = (float) (((double) ((Box2) ref localBounds).Height + (double) spriteComponent.Offset.Y) / 2.0 - (double) texture3.Height / 32.0);
            float x = (float) (-((double) ((Box2) ref localBounds).Width + (double) spriteComponent.Offset.X) / 2.0);
            ((DrawingHandleBase) worldHandle).DrawTexture(texture3, new Vector2(x, y2), new Color?());
          }
          SpriteSpecifier.Rsi squadBadge = CivTeamIconResolver.GetSquadBadge(teamMemberComponent2.SquadId);
          if (squadBadge != null)
          {
            Texture texture4 = this._sprite.Frame0((SpriteSpecifier) squadBadge);
            float num1 = (float) (-(double) texture4.Height / 2.0 / 32.0);
            float y3 = (float) (0.10000000149011612 + ((double) ((Box2) ref localBounds).Height + (double) spriteComponent.Offset.Y + (double) num1) / 2.0 - (double) texture4.Height / 32.0);
            float x = (float) (((double) ((Box2) ref localBounds).Width + (double) spriteComponent.Offset.X) / 2.0 - (double) texture4.Width / 32.0);
            if (teamMemberComponent2.SquadId >= 10)
            {
              float num2 = 0.125f;
              x += num2;
            }
            Vector2 vector2_2 = new Vector2(x, y3);
            ((DrawingHandleBase) worldHandle).DrawTexture(texture4, vector2_2, new Color?(color1));
          }
        }
      }
    }
    DrawingHandleWorld drawingHandleWorld = worldHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local = ref identity;
    ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local);
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }
}
