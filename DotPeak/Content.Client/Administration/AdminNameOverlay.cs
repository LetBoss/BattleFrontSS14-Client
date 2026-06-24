// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.AdminNameOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Administration.Systems;
using Content.Client.Stylesheets;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Content.Shared.Ghost;
using Content.Shared.Mind;
using Content.Shared.Roles;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.Administration;

internal sealed class AdminNameOverlay : Overlay
{
  private readonly AdminSystem _system;
  private readonly IEntityManager _entityManager;
  private readonly IEyeManager _eyeManager;
  private readonly EntityLookupSystem _entityLookup;
  private readonly IUserInterfaceManager _userInterfaceManager;
  private readonly SharedRoleSystem _roles;
  private readonly IPrototypeManager _prototypeManager;
  private readonly Font _font;
  private readonly Font _fontBold;
  private AdminOverlayAntagFormat _overlayFormat;
  private AdminOverlayAntagSymbolStyle _overlaySymbolStyle;
  private bool _overlayPlaytime;
  private bool _overlayStartingJob;
  private float _ghostFadeDistance;
  private float _ghostHideDistance;
  private int _overlayStackMax;
  private float _overlayMergeDistance;
  private static readonly FrozenSet<ProtoId<RoleTypePrototype>> Filter = ((IEnumerable<ProtoId<RoleTypePrototype>>) new ProtoId<RoleTypePrototype>[4]
  {
    ProtoId<RoleTypePrototype>.op_Implicit("SoloAntagonist"),
    ProtoId<RoleTypePrototype>.op_Implicit("TeamAntagonist"),
    ProtoId<RoleTypePrototype>.op_Implicit("SiliconAntagonist"),
    ProtoId<RoleTypePrototype>.op_Implicit("FreeAgent")
  }).ToFrozenSet<ProtoId<RoleTypePrototype>>();
  private readonly string _antagLabelClassic = Loc.GetString("admin-overlay-antag-classic");

  public AdminNameOverlay(
    AdminSystem system,
    IEntityManager entityManager,
    IEyeManager eyeManager,
    IResourceCache resourceCache,
    EntityLookupSystem entityLookup,
    IUserInterfaceManager userInterfaceManager,
    IConfigurationManager config,
    SharedRoleSystem roles,
    IPrototypeManager prototypeManager)
  {
    this._system = system;
    this._entityManager = entityManager;
    this._eyeManager = eyeManager;
    this._entityLookup = entityLookup;
    this._userInterfaceManager = userInterfaceManager;
    this._roles = roles;
    this._prototypeManager = prototypeManager;
    this.ZIndex = new int?(200);
    this._font = resourceCache.NotoStack();
    this._fontBold = resourceCache.NotoStack("Bold");
    config.OnValueChanged<string>(CCVars.AdminOverlayAntagFormat, (Action<string>) (show => this._overlayFormat = this.UpdateOverlayFormat(show)), true);
    config.OnValueChanged<string>(CCVars.AdminOverlaySymbolStyle, (Action<string>) (show => this._overlaySymbolStyle = this.UpdateOverlaySymbolStyle(show)), true);
    config.OnValueChanged<bool>(CCVars.AdminOverlayPlaytime, (Action<bool>) (show => this._overlayPlaytime = show), true);
    config.OnValueChanged<bool>(CCVars.AdminOverlayStartingJob, (Action<bool>) (show => this._overlayStartingJob = show), true);
    config.OnValueChanged<int>(CCVars.AdminOverlayGhostHideDistance, (Action<int>) (f => this._ghostHideDistance = (float) f), true);
    config.OnValueChanged<int>(CCVars.AdminOverlayGhostFadeDistance, (Action<int>) (f => this._ghostFadeDistance = (float) f), true);
    config.OnValueChanged<int>(CCVars.AdminOverlayStackMax, (Action<int>) (i => this._overlayStackMax = i), true);
    config.OnValueChanged<float>(CCVars.AdminOverlayMergeDistance, (Action<float>) (f => this._overlayMergeDistance = f), true);
  }

  private AdminOverlayAntagFormat UpdateOverlayFormat(string formatString)
  {
    AdminOverlayAntagFormat result;
    if (!Enum.TryParse<AdminOverlayAntagFormat>(formatString, out result))
      result = AdminOverlayAntagFormat.Binary;
    return result;
  }

  private AdminOverlayAntagSymbolStyle UpdateOverlaySymbolStyle(string symbolString)
  {
    AdminOverlayAntagSymbolStyle result;
    if (!Enum.TryParse<AdminOverlayAntagSymbolStyle>(symbolString, out result))
      result = AdminOverlayAntagSymbolStyle.Off;
    return result;
  }

  public virtual OverlaySpace Space => (OverlaySpace) 2;

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    Box2 worldAabb1 = args.WorldAABB;
    Color white = Color.White;
    float uiScale = ((Control) this._userInterfaceManager.RootControl).UIScale;
    Vector2 vector2_1 = new Vector2(0.0f, 14f) * uiScale;
    List<(Vector2, Vector2)> valueTupleList = new List<(Vector2, Vector2)>();
    List<(PlayerInfo, Box2, EntityUid, Vector2)> source = new List<(PlayerInfo, Box2, EntityUid, Vector2)>();
    foreach (PlayerInfo player in (IEnumerable<PlayerInfo>) this._system.PlayerList)
    {
      EntityUid? entity = this._entityManager.GetEntity(player.NetEntity);
      if (entity.HasValue && this._entityManager.EntityExists(entity) && !MapId.op_Inequality(this._entityManager.GetComponent<TransformComponent>(entity.Value).MapID, args.MapId))
      {
        Box2 worldAabb2 = this._entityLookup.GetWorldAABB(entity.Value, (TransformComponent) null);
        if (((Box2) ref worldAabb2).Intersects(ref worldAabb1))
        {
          Vector2 vector2_2 = Vector2Helpers.Rounded(this._eyeManager.WorldToScreen(((Box2) ref worldAabb2).Center));
          source.Add((player, worldAabb2, entity.Value, vector2_2));
        }
      }
    }
    foreach ((PlayerInfo, Box2, EntityUid, Vector2) tuple in source.OrderBy<(PlayerInfo, Box2, EntityUid, Vector2), float>((Func<(PlayerInfo, Box2, EntityUid, Vector2), float>) (s => s.Item4.Y)).ToList<(PlayerInfo, Box2, EntityUid, Vector2)>())
    {
      PlayerInfo playerInfo = tuple.Item1;
      ProtoId<RoleTypePrototype>? roleProto = playerInfo.RoleProto;
      RoleTypePrototype roleTypePrototype;
      if (roleProto.HasValue)
      {
        IPrototypeManager prototypeManager = this._prototypeManager;
        roleProto = playerInfo.RoleProto;
        ProtoId<RoleTypePrototype> protoId = roleProto.Value;
        roleTypePrototype = prototypeManager.Index<RoleTypePrototype>(protoId);
      }
      else
        roleTypePrototype = (RoleTypePrototype) null;
      string str1 = Loc.GetString(LocId.op_Implicit(roleTypePrototype != null ? roleTypePrototype.Name : RoleTypePrototype.FallbackName));
      Color color1 = roleTypePrototype != null ? roleTypePrototype.Color : RoleTypePrototype.FallbackColor;
      string str2 = roleTypePrototype?.Symbol ?? "";
      Box2 aabb = tuple.Item2;
      EntityUid entityUid = tuple.Item3;
      Vector2 vector2_3 = tuple.Item4;
      Vector2 vector2_4 = new Vector2(28f, -18f) * uiScale;
      Vector2 vector2_5 = vector2_3 + vector2_4;
      float num1 = 1f;
      Vector2 vector2_6 = Vector2.Zero;
      if (this._entityManager.HasComponent<GhostComponent>(entityUid))
      {
        float num2 = Vector2.Distance(((Box2) ref aabb).Center, this._eyeManager.ScreenToMap(this._userInterfaceManager.MousePositionScaled.Position * uiScale).Position);
        if ((double) num2 >= (double) this._ghostHideDistance)
        {
          num1 = Math.Clamp((float) (((double) num2 - (double) this._ghostHideDistance) / ((double) this._ghostFadeDistance - (double) this._ghostHideDistance)), 0.0f, 1f);
          white.A = num1;
        }
        else
          continue;
      }
      List<(Vector2, Vector2)> all = valueTupleList.FindAll((Predicate<(Vector2, Vector2)>) (x => (double) Vector2.Distance(this._eyeManager.ScreenToMap(x.Item1).Position, ((Box2) ref aabb).Center) <= (double) this._overlayMergeDistance));
      if (all.Count > 0)
      {
        vector2_5 = all.First<(Vector2, Vector2)>().Item1 + vector2_4;
        vector2_3 = all.First<(Vector2, Vector2)>().Item1;
        int num3 = 1;
        foreach ((Vector2, Vector2) valueTuple in all)
        {
          if (num3 <= this._overlayStackMax - 1)
            vector2_6 = vector2_1 + valueTuple.Item2;
          ++num3;
        }
      }
      Color color2 = Color.Aquamarine;
      color2.A = num1;
      ((OverlayDrawArgs) ref args).ScreenHandle.DrawString(this._font, vector2_5 + vector2_6, (ReadOnlySpan<char>) playerInfo.CharacterName, uiScale, playerInfo.Connected ? color2 : white);
      Vector2 vector2_7 = vector2_6 + vector2_1;
      color2 = Color.Yellow;
      color2.A = num1;
      ((OverlayDrawArgs) ref args).ScreenHandle.DrawString(this._font, vector2_5 + vector2_7, (ReadOnlySpan<char>) playerInfo.Username, uiScale, playerInfo.Connected ? color2 : white);
      Vector2 vector2_8 = vector2_7 + vector2_1;
      if (!string.IsNullOrEmpty(playerInfo.PlaytimeString) && this._overlayPlaytime)
      {
        color2 = Color.Orange;
        color2.A = num1;
        ((OverlayDrawArgs) ref args).ScreenHandle.DrawString(this._font, vector2_5 + vector2_8, (ReadOnlySpan<char>) playerInfo.PlaytimeString, uiScale, playerInfo.Connected ? color2 : white);
        vector2_8 += vector2_1;
      }
      if (!string.IsNullOrEmpty(playerInfo.StartingJob) && this._overlayStartingJob)
      {
        color2 = Color.GreenYellow;
        color2.A = num1;
        ((OverlayDrawArgs) ref args).ScreenHandle.DrawString(this._font, vector2_5 + vector2_8, (ReadOnlySpan<char>) Loc.GetString(playerInfo.StartingJob), uiScale, playerInfo.Connected ? color2 : white);
        vector2_8 += vector2_1;
      }
      string str3;
      switch (this._overlaySymbolStyle)
      {
        case AdminOverlayAntagSymbolStyle.Basic:
          str3 = Loc.GetString("player-tab-antag-prefix");
          break;
        case AdminOverlayAntagSymbolStyle.Specific:
          str3 = str2;
          break;
        default:
          str3 = string.Empty;
          break;
      }
      string str4;
      string str5;
      switch (this._overlayFormat)
      {
        case AdminOverlayAntagFormat.Roletype:
          color2 = color1;
          str4 = AdminNameOverlay.IsFiltered(playerInfo.RoleProto) ? str3 : string.Empty;
          str5 = AdminNameOverlay.IsFiltered(playerInfo.RoleProto) ? str1.ToUpper() : string.Empty;
          break;
        case AdminOverlayAntagFormat.Subtype:
          color2 = color1;
          str4 = AdminNameOverlay.IsFiltered(playerInfo.RoleProto) ? str3 : string.Empty;
          str5 = AdminNameOverlay.IsFiltered(playerInfo.RoleProto) ? this._roles.GetRoleSubtypeLabel(LocId.op_Implicit(str1), playerInfo.Subtype).ToUpper() : string.Empty;
          break;
        default:
          color2 = Color.OrangeRed;
          str4 = playerInfo.Antag ? str3 : string.Empty;
          str5 = playerInfo.Antag ? this._antagLabelClassic : string.Empty;
          break;
      }
      color2.A = num1;
      string str6;
      if (string.IsNullOrEmpty(str4))
        str6 = str5;
      else
        str6 = Loc.GetString("player-tab-character-name-antag-symbol", new (string, object)[2]
        {
          ("symbol", (object) str4),
          ("name", (object) str5)
        });
      string str7 = str6;
      ((OverlayDrawArgs) ref args).ScreenHandle.DrawString(this._fontBold, vector2_5 + vector2_8, (ReadOnlySpan<char>) str7, uiScale, color2);
      Vector2 vector2_9 = vector2_8 + vector2_1;
      valueTupleList.Add((vector2_3, vector2_9));
    }
  }

  private static bool IsFiltered(ProtoId<RoleTypePrototype>? roleProtoId)
  {
    return roleProtoId.HasValue && AdminNameOverlay.Filter.Contains(roleProtoId.Value);
  }
}
