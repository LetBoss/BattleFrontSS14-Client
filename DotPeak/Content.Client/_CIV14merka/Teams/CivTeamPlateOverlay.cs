// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Teams.CivTeamPlateOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.CombatMode;
using Content.Client.Stylesheets;
using Content.Shared._CIV14merka.Teams;
using Content.Shared.CCVar;
using Content.Shared.Ghost;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Teams;

public sealed class CivTeamPlateOverlay : Overlay
{
  [Dependency]
  private IEyeManager _eye;
  [Dependency]
  private IResourceCache _resourceCache;
  [Dependency]
  private IUserInterfaceManager _userInterface;
  [Dependency]
  private IConfigurationManager _cfg;
  private readonly IEntityManager _entity;
  private readonly IPlayerManager _player;
  private readonly CivTeamVisualsSystem _teams;
  private readonly SharedTransformSystem _transform;
  private readonly SpriteSystem _sprite;
  private readonly MobStateSystem _mobState;
  private readonly CombatModeSystem _combatMode;
  private readonly Font _font;
  private readonly EntityQuery<TransformComponent> _xformQuery;
  private const float NearbyRangeTiles = 3f;
  private readonly Dictionary<int, string> _squadLeaderPrefixes = new Dictionary<int, string>();
  private readonly Dictionary<int, string> _squadPrefixes = new Dictionary<int, string>();
  private readonly Dictionary<EntityUid, (string Prefix, string Name, string Label)> _labelCache = new Dictionary<EntityUid, (string, string, string)>();

  private static string PrefixCommander => Loc.GetString("civ-ui-teamplate-prefix-commander");

  private static string PrefixReserve => Loc.GetString("civ-ui-teamplate-prefix-reserve");

  public virtual OverlaySpace Space => (OverlaySpace) 2;

  public CivTeamPlateOverlay(
    IEntityManager entity,
    IPlayerManager player,
    CivTeamVisualsSystem teams)
  {
    IoCManager.InjectDependencies<CivTeamPlateOverlay>(this);
    this._entity = entity;
    this._player = player;
    this._teams = teams;
    this._transform = entity.System<SharedTransformSystem>();
    this._sprite = entity.System<SpriteSystem>();
    this._mobState = entity.System<MobStateSystem>();
    this._combatMode = entity.System<CombatModeSystem>();
    this._font = this._resourceCache.NotoStack("Bold");
    this._xformQuery = entity.GetEntityQuery<TransformComponent>();
    this.ZIndex = new int?(150);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    TransformComponent transformComponent1;
    if (!localEntity.HasValue || !this._entity.HasComponent<CivTeamMemberComponent>(localEntity.Value) && !this._entity.HasComponent<GhostComponent>(localEntity.Value) || !this._cfg.GetCVar<bool>(CCVars.Civ14ShowNearbyNames) || this._combatMode.IsInCombatMode(new EntityUid?(localEntity.Value)) || !this._entity.TryGetComponent<TransformComponent>(localEntity.Value, ref transformComponent1))
      return;
    Vector2 worldPosition1 = this._transform.GetWorldPosition(transformComponent1, this._xformQuery);
    float uiScale = ((Control) this._userInterface.RootControl).UIScale;
    Vector2 vector2_1 = new Vector2(6f, 3f) * uiScale;
    AllEntityQueryEnumerator<CivTeamMemberComponent, SpriteComponent, TransformComponent, MetaDataComponent> entityQueryEnumerator = this._entity.AllEntityQueryEnumerator<CivTeamMemberComponent, SpriteComponent, TransformComponent, MetaDataComponent>();
    EntityUid entityUid;
    CivTeamMemberComponent member;
    SpriteComponent spriteComponent;
    TransformComponent transformComponent2;
    MetaDataComponent metaDataComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref member, ref spriteComponent, ref transformComponent2, ref metaDataComponent))
    {
      Color color;
      MobStateComponent component;
      if (!EntityUid.op_Equality(entityUid, localEntity.Value) && !MapId.op_Inequality(transformComponent2.MapID, args.MapId) && spriteComponent.Visible && this._teams.TryGetRelationColor(entityUid, out color) && (!this._entity.TryGetComponent<MobStateComponent>(entityUid, ref component) || !this._mobState.IsDead(entityUid, component)))
      {
        Vector2 worldPosition2 = this._transform.GetWorldPosition(transformComponent2, this._xformQuery);
        if ((double) (worldPosition2 - worldPosition1).LengthSquared() <= 9.0)
        {
          string entityName = metaDataComponent.EntityName;
          if (!string.IsNullOrWhiteSpace(entityName))
          {
            string prefix = this.GetPrefix(member);
            (string, string, string) valueTuple;
            if (!this._labelCache.TryGetValue(entityUid, out valueTuple) || valueTuple.Item1 != prefix || valueTuple.Item2 != entityName)
            {
              valueTuple = (prefix, entityName, $"{prefix} {entityName}");
              this._labelCache[entityUid] = valueTuple;
            }
            string str = valueTuple.Item3;
            Box2 localBounds = this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)));
            Box2 box2 = ((Box2) ref localBounds).Translated(worldPosition2);
            if (((Box2) ref box2).Intersects(ref args.WorldAABB))
            {
              Vector2 dimensions = ((OverlayDrawArgs) ref args).ScreenHandle.GetDimensions(this._font, (ReadOnlySpan<char>) str, uiScale);
              Vector2 vector2_2 = Vector2Helpers.Rounded(this._eye.WorldToScreen(worldPosition2));
              float num = (float) ((double) ((Box2) ref localBounds).Height * 32.0 / 2.0 + 20.0 * (double) uiScale);
              Vector2 vector2_3 = new Vector2(vector2_2.X - dimensions.X / 2f, vector2_2.Y - num - dimensions.Y);
              UIBox2 uiBox2 = UIBox2.FromDimensions(vector2_3 - vector2_1, dimensions + vector2_1 * 2f);
              ((OverlayDrawArgs) ref args).ScreenHandle.DrawRect(uiBox2, new Color((byte) 12, (byte) 18, (byte) 26, (byte) 190), true);
              ((OverlayDrawArgs) ref args).ScreenHandle.DrawRect(uiBox2, ((Color) ref color).WithAlpha(0.95f), false);
              ((OverlayDrawArgs) ref args).ScreenHandle.DrawString(this._font, vector2_3, (ReadOnlySpan<char>) str, uiScale, color);
            }
          }
        }
      }
    }
  }

  private string GetPrefix(CivTeamMemberComponent member)
  {
    if (member.IsCommander)
      return CivTeamPlateOverlay.PrefixCommander;
    if (member.SquadId == 0)
      return CivTeamPlateOverlay.PrefixReserve;
    Dictionary<int, string> dictionary = member.IsSquadLeader ? this._squadLeaderPrefixes : this._squadPrefixes;
    string prefix1;
    if (dictionary.TryGetValue(member.SquadId, out prefix1))
      return prefix1;
    string str;
    if (!member.IsSquadLeader)
      str = $"[S{member.SquadId}]";
    else
      str = $"[LS{member.SquadId}]";
    string prefix2 = str;
    dictionary[member.SquadId] = prefix2;
    return prefix2;
  }
}
