// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Marines.MarineOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.CrashLand;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Stealth;
using Content.Shared._RMC14.Tracker.SquadLeader;
using Content.Shared.NPC.Components;
using Content.Shared.NPC.Prototypes;
using Content.Shared.NPC.Systems;
using Content.Shared.ParaDrop;
using Content.Shared.StatusIcon.Components;
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
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Marines;

public sealed class MarineOverlay : Overlay
{
  [Dependency]
  private IEntityManager _entity;
  [Dependency]
  private IPlayerManager _players;
  [Dependency]
  private IPrototypeManager _prototype;
  private static readonly SpriteSpecifier.Rsi FireteamOneRsi = new SpriteSpecifier.Rsi(new ResPath("_RMC14/Interface/marine_hud.rsi"), "hudsquad_ft1");
  private static readonly SpriteSpecifier.Rsi FireteamTwoRsi = new SpriteSpecifier.Rsi(new ResPath("_RMC14/Interface/marine_hud.rsi"), "hudsquad_ft2");
  private static readonly SpriteSpecifier.Rsi FireteamThreeRsi = new SpriteSpecifier.Rsi(new ResPath("_RMC14/Interface/marine_hud.rsi"), "hudsquad_ft3");
  private static readonly SpriteSpecifier.Rsi FireteamLeaderRsi = new SpriteSpecifier.Rsi(new ResPath("_RMC14/Interface/marine_hud.rsi"), "hudsquad_ftl");
  private readonly NpcFactionSystem _npcFaction;
  private readonly ContainerSystem _container;
  private readonly MarineSystem _marine;
  private readonly SpriteSystem _sprite;
  private readonly TransformSystem _transform;
  private readonly ShaderInstance _shader;
  private readonly EntityQuery<NpcFactionMemberComponent> _npcFactionMemberQuery;
  private readonly EntityQuery<FireteamLeaderComponent> _fireteamLeaderQuery;
  private readonly EntityQuery<FireteamMemberComponent> _fireteamMemberQuery;
  private readonly EntityQuery<EntityActiveInvisibleComponent> _invisQuery;
  private readonly EntityQuery<ShowMarineIconsComponent> _marineIconsQuery;
  private readonly EntityQuery<ParaDroppingComponent> _paraDroppingQuery;
  private readonly EntityQuery<CrashLandingComponent> _crashLandingQuery;

  public virtual OverlaySpace Space => (OverlaySpace) 8;

  public MarineOverlay()
  {
    IoCManager.InjectDependencies<MarineOverlay>(this);
    this._npcFaction = this._entity.System<NpcFactionSystem>();
    this._container = this._entity.System<ContainerSystem>();
    this._marine = this._entity.System<MarineSystem>();
    this._sprite = this._entity.System<SpriteSystem>();
    this._transform = this._entity.System<TransformSystem>();
    this._npcFactionMemberQuery = this._entity.GetEntityQuery<NpcFactionMemberComponent>();
    this._fireteamLeaderQuery = this._entity.GetEntityQuery<FireteamLeaderComponent>();
    this._fireteamMemberQuery = this._entity.GetEntityQuery<FireteamMemberComponent>();
    this._invisQuery = this._entity.GetEntityQuery<EntityActiveInvisibleComponent>();
    this._marineIconsQuery = this._entity.GetEntityQuery<ShowMarineIconsComponent>();
    this._paraDroppingQuery = this._entity.GetEntityQuery<ParaDroppingComponent>();
    this._crashLandingQuery = this._entity.GetEntityQuery<CrashLandingComponent>();
    this._shader = this._prototype.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("shaded")).Instance();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    ShowMarineIconsComponent marineIconsComponent;
    if (!this._marineIconsQuery.TryComp(((ISharedPlayerManager) this._players).LocalEntity, ref marineIconsComponent))
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    IEye eye = args.Viewport.Eye;
    Angle angle = eye != null ? eye.Rotation : new Angle();
    EntityQuery<TransformComponent> entityQuery = this._entity.GetEntityQuery<TransformComponent>();
    Matrix3x2 scale = Matrix3x2.CreateScale(new Vector2(1f, 1f));
    Matrix3x2 rotation = Matrix3Helpers.CreateRotation(Angle.op_Implicit(Angle.op_UnaryNegation(angle)));
    ((DrawingHandleBase) worldHandle).UseShader(this._shader);
    Texture texture1 = this._sprite.Frame0((SpriteSpecifier) MarineOverlay.FireteamOneRsi);
    Texture texture2 = this._sprite.Frame0((SpriteSpecifier) MarineOverlay.FireteamTwoRsi);
    Texture texture3 = this._sprite.Frame0((SpriteSpecifier) MarineOverlay.FireteamThreeRsi);
    Texture texture4 = this._sprite.Frame0((SpriteSpecifier) MarineOverlay.FireteamLeaderRsi);
    AllEntityQueryEnumerator<MarineComponent, StatusIconComponent, SpriteComponent, TransformComponent> entityQueryEnumerator = this._entity.AllEntityQueryEnumerator<MarineComponent, StatusIconComponent, SpriteComponent, TransformComponent>();
    EntityUid uid;
    MarineComponent marineComponent;
    StatusIconComponent statusIconComponent;
    SpriteComponent spriteComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref uid, ref marineComponent, ref statusIconComponent, ref spriteComponent, ref transformComponent))
    {
      if (!MapId.op_Inequality(transformComponent.MapID, args.MapId))
      {
        Box2 box2_1 = statusIconComponent.Bounds ?? this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)));
        Vector2 worldPosition = ((SharedTransformSystem) this._transform).GetWorldPosition(transformComponent, entityQuery);
        Box2 box2_2 = ((Box2) ref box2_1).Translated(worldPosition);
        if (((Box2) ref box2_2).Intersects(ref args.WorldAABB) && !((SharedContainerSystem) this._container).IsEntityOrParentInContainer(uid, (MetaDataComponent) null, (TransformComponent) null) && !this._invisQuery.HasComp(uid))
        {
          Matrix3x2 translation = Matrix3x2.CreateTranslation(worldPosition);
          Matrix3x2 matrix3x2_1 = Matrix3x2.Multiply(scale, translation);
          Matrix3x2 matrix3x2_2 = Matrix3x2.Multiply(rotation, matrix3x2_1);
          ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2_2);
          GetMarineIconEvent marineIcon = this._marine.GetMarineIcon(uid);
          Dictionary<ProtoId<NpcFactionPrototype>, SpriteSpecifier> factionIcons = this._marine.GetFactionIcons(uid);
          NpcFactionMemberComponent factionMemberComponent;
          SpriteSpecifier spriteSpecifier;
          if (marineIconsComponent.Factions != null && !this._npcFaction.IsMemberOfAny(Entity<NpcFactionMemberComponent>.op_Implicit(uid), (IEnumerable<ProtoId<NpcFactionPrototype>>) marineIconsComponent.Factions) && factionIcons != null && this._npcFactionMemberQuery.TryComp(uid, ref factionMemberComponent) && factionIcons.TryGetValue(factionMemberComponent.Factions.First<ProtoId<NpcFactionPrototype>>(), out spriteSpecifier))
          {
            marineIcon.Background = (SpriteSpecifier) null;
            marineIcon.Icon = spriteSpecifier;
          }
          if (marineIcon.Icon != null)
          {
            Texture texture5 = this._sprite.Frame0(marineIcon.Icon);
            float y = (float) (0.10000000149011612 + ((double) ((Box2) ref box2_1).Height + (double) spriteComponent.Offset.Y) / 2.0 - (double) texture5.Height / 32.0);
            float x = (float) (0.10000000149011612 + ((double) ((Box2) ref box2_1).Width + (double) spriteComponent.Offset.X) / 2.0 - (double) texture5.Width / 32.0);
            if (this._crashLandingQuery.HasComp(uid) || this._paraDroppingQuery.HasComp(uid))
            {
              y = 0.1f + spriteComponent.Offset.Y;
              x = 0.1f + spriteComponent.Offset.X;
            }
            Vector2 vector2 = new Vector2(x, y);
            if (marineIcon.Icon != null && marineIcon.Background != null)
            {
              Texture texture6 = this._sprite.Frame0(marineIcon.Background);
              ((DrawingHandleBase) worldHandle).DrawTexture(texture6, vector2, marineIcon.BackgroundColor);
            }
            ((DrawingHandleBase) worldHandle).DrawTexture(texture5, vector2, new Color?());
          }
          FireteamMemberComponent fireteamMemberComponent;
          if (this._fireteamMemberQuery.TryComp(uid, ref fireteamMemberComponent))
          {
            Texture texture7;
            switch (fireteamMemberComponent.Fireteam)
            {
              case 0:
                texture7 = texture1;
                break;
              case 1:
                texture7 = texture2;
                break;
              case 2:
                texture7 = texture3;
                break;
              default:
                texture7 = (Texture) null;
                break;
            }
            Texture texture8 = texture7;
            if (texture8 != null)
            {
              float num = (float) (-(double) texture1.Height / 2.0 / 32.0);
              float y = (float) (0.10000000149011612 + ((double) ((Box2) ref box2_1).Height + (double) spriteComponent.Offset.Y + (double) num) / 2.0 - (double) texture8.Height / 32.0);
              Vector2 vector2 = new Vector2((float) (((double) ((Box2) ref box2_1).Width + (double) spriteComponent.Offset.X) / 2.0 - (double) texture8.Width / 32.0), y);
              ((DrawingHandleBase) worldHandle).DrawTexture(texture8, vector2, marineIcon.BackgroundColor);
            }
          }
          if (this._fireteamLeaderQuery.HasComp(uid))
          {
            Texture texture9 = texture4;
            float num = (float) (-(double) texture1.Height / 2.0 / 32.0);
            float y = (float) (0.10000000149011612 + ((double) ((Box2) ref box2_1).Height + (double) spriteComponent.Offset.Y + (double) num) / 2.0 - (double) texture9.Height / 32.0);
            Vector2 vector2 = new Vector2((float) (((double) ((Box2) ref box2_1).Width + (double) spriteComponent.Offset.X) / 2.0 - (double) texture9.Width / 32.0), y);
            ((DrawingHandleBase) worldHandle).DrawTexture(texture9, vector2, marineIcon.BackgroundColor);
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
