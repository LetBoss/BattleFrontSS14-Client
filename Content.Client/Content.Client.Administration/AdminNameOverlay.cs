using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

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

	private static readonly FrozenSet<ProtoId<RoleTypePrototype>> Filter = new ProtoId<RoleTypePrototype>[4]
	{
		ProtoId<RoleTypePrototype>.op_Implicit("SoloAntagonist"),
		ProtoId<RoleTypePrototype>.op_Implicit("TeamAntagonist"),
		ProtoId<RoleTypePrototype>.op_Implicit("SiliconAntagonist"),
		ProtoId<RoleTypePrototype>.op_Implicit("FreeAgent")
	}.ToFrozenSet();

	private readonly string _antagLabelClassic = Loc.GetString("admin-overlay-antag-classic");

	public override OverlaySpace Space => (OverlaySpace)2;

	public AdminNameOverlay(AdminSystem system, IEntityManager entityManager, IEyeManager eyeManager, IResourceCache resourceCache, EntityLookupSystem entityLookup, IUserInterfaceManager userInterfaceManager, IConfigurationManager config, SharedRoleSystem roles, IPrototypeManager prototypeManager)
	{
		_system = system;
		_entityManager = entityManager;
		_eyeManager = eyeManager;
		_entityLookup = entityLookup;
		_userInterfaceManager = userInterfaceManager;
		_roles = roles;
		_prototypeManager = prototypeManager;
		((Overlay)this).ZIndex = 200;
		_font = resourceCache.NotoStack();
		_fontBold = resourceCache.NotoStack("Bold");
		config.OnValueChanged<string>(CCVars.AdminOverlayAntagFormat, (Action<string>)delegate(string show)
		{
			_overlayFormat = UpdateOverlayFormat(show);
		}, true);
		config.OnValueChanged<string>(CCVars.AdminOverlaySymbolStyle, (Action<string>)delegate(string show)
		{
			_overlaySymbolStyle = UpdateOverlaySymbolStyle(show);
		}, true);
		config.OnValueChanged<bool>(CCVars.AdminOverlayPlaytime, (Action<bool>)delegate(bool show)
		{
			_overlayPlaytime = show;
		}, true);
		config.OnValueChanged<bool>(CCVars.AdminOverlayStartingJob, (Action<bool>)delegate(bool show)
		{
			_overlayStartingJob = show;
		}, true);
		config.OnValueChanged<int>(CCVars.AdminOverlayGhostHideDistance, (Action<int>)delegate(int f)
		{
			_ghostHideDistance = f;
		}, true);
		config.OnValueChanged<int>(CCVars.AdminOverlayGhostFadeDistance, (Action<int>)delegate(int f)
		{
			_ghostFadeDistance = f;
		}, true);
		config.OnValueChanged<int>(CCVars.AdminOverlayStackMax, (Action<int>)delegate(int i)
		{
			_overlayStackMax = i;
		}, true);
		config.OnValueChanged<float>(CCVars.AdminOverlayMergeDistance, (Action<float>)delegate(float f)
		{
			_overlayMergeDistance = f;
		}, true);
	}

	private AdminOverlayAntagFormat UpdateOverlayFormat(string formatString)
	{
		if (!Enum.TryParse<AdminOverlayAntagFormat>(formatString, out var result))
		{
			return AdminOverlayAntagFormat.Binary;
		}
		return result;
	}

	private AdminOverlayAntagSymbolStyle UpdateOverlaySymbolStyle(string symbolString)
	{
		if (!Enum.TryParse<AdminOverlayAntagSymbolStyle>(symbolString, out var result))
		{
			return AdminOverlayAntagSymbolStyle.Off;
		}
		return result;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		Box2 worldAABB = args.WorldAABB;
		Color white = Color.White;
		float uIScale = ((Control)_userInterfaceManager.RootControl).UIScale;
		Vector2 vector = new Vector2(0f, 14f) * uIScale;
		List<(Vector2, Vector2)> list = new List<(Vector2, Vector2)>();
		List<(PlayerInfo, Box2, EntityUid, Vector2)> list2 = new List<(PlayerInfo, Box2, EntityUid, Vector2)>();
		foreach (PlayerInfo player in _system.PlayerList)
		{
			EntityUid? entity = _entityManager.GetEntity(player.NetEntity);
			if (entity.HasValue && _entityManager.EntityExists(entity) && !(_entityManager.GetComponent<TransformComponent>(entity.Value).MapID != args.MapId))
			{
				Box2 worldAABB2 = _entityLookup.GetWorldAABB(entity.Value, (TransformComponent)null);
				if (((Box2)(ref worldAABB2)).Intersects(ref worldAABB))
				{
					Vector2 item = Vector2Helpers.Rounded(_eyeManager.WorldToScreen(((Box2)(ref worldAABB2)).Center));
					list2.Add((player, worldAABB2, entity.Value, item));
				}
			}
		}
		foreach (var item4 in list2.OrderBy(((PlayerInfo, Box2, EntityUid, Vector2) s) => s.Item4.Y).ToList())
		{
			PlayerInfo item2 = item4.Item1;
			RoleTypePrototype obj = ((!item2.RoleProto.HasValue) ? null : _prototypeManager.Index<RoleTypePrototype>(item2.RoleProto.Value));
			string text = Loc.GetString(LocId.op_Implicit(obj?.Name ?? RoleTypePrototype.FallbackName));
			Color val = obj?.Color ?? RoleTypePrototype.FallbackColor;
			string text2 = obj?.Symbol ?? "";
			Box2 aabb = item4.Item2;
			EntityUid item3 = item4.Item3;
			Vector2 vector2 = item4.Item4;
			Vector2 vector3 = new Vector2(28f, -18f) * uIScale;
			Vector2 vector4 = vector2 + vector3;
			float a = 1f;
			Vector2 vector5 = Vector2.Zero;
			if (_entityManager.HasComponent<GhostComponent>(item3))
			{
				Vector2 center = ((Box2)(ref aabb)).Center;
				Vector2 position = _eyeManager.ScreenToMap(_userInterfaceManager.MousePositionScaled.Position * uIScale).Position;
				float num = Vector2.Distance(center, position);
				if (num < _ghostHideDistance)
				{
					continue;
				}
				a = (white.A = Math.Clamp((num - _ghostHideDistance) / (_ghostFadeDistance - _ghostHideDistance), 0f, 1f));
			}
			List<(Vector2, Vector2)> list3 = list.FindAll(((Vector2, Vector2) x) => Vector2.Distance(_eyeManager.ScreenToMap(x.Item1).Position, ((Box2)(ref aabb)).Center) <= _overlayMergeDistance);
			if (list3.Count > 0)
			{
				vector4 = list3.First().Item1 + vector3;
				vector2 = list3.First().Item1;
				int num2 = 1;
				foreach (var item5 in list3)
				{
					if (num2 <= _overlayStackMax - 1)
					{
						vector5 = vector + item5.Item2;
					}
					num2++;
				}
			}
			Color aquamarine = Color.Aquamarine;
			aquamarine.A = a;
			((OverlayDrawArgs)(ref args)).ScreenHandle.DrawString(_font, vector4 + vector5, (ReadOnlySpan<char>)item2.CharacterName, uIScale, item2.Connected ? aquamarine : white);
			vector5 += vector;
			aquamarine = Color.Yellow;
			aquamarine.A = a;
			((OverlayDrawArgs)(ref args)).ScreenHandle.DrawString(_font, vector4 + vector5, (ReadOnlySpan<char>)item2.Username, uIScale, item2.Connected ? aquamarine : white);
			vector5 += vector;
			if (!string.IsNullOrEmpty(item2.PlaytimeString) && _overlayPlaytime)
			{
				aquamarine = Color.Orange;
				aquamarine.A = a;
				((OverlayDrawArgs)(ref args)).ScreenHandle.DrawString(_font, vector4 + vector5, (ReadOnlySpan<char>)item2.PlaytimeString, uIScale, item2.Connected ? aquamarine : white);
				vector5 += vector;
			}
			if (!string.IsNullOrEmpty(item2.StartingJob) && _overlayStartingJob)
			{
				aquamarine = Color.GreenYellow;
				aquamarine.A = a;
				((OverlayDrawArgs)(ref args)).ScreenHandle.DrawString(_font, vector4 + vector5, (ReadOnlySpan<char>)Loc.GetString(item2.StartingJob), uIScale, item2.Connected ? aquamarine : white);
				vector5 += vector;
			}
			string text3 = _overlaySymbolStyle switch
			{
				AdminOverlayAntagSymbolStyle.Specific => text2, 
				AdminOverlayAntagSymbolStyle.Basic => Loc.GetString("player-tab-antag-prefix"), 
				_ => string.Empty, 
			};
			string text4;
			switch (_overlayFormat)
			{
			case AdminOverlayAntagFormat.Roletype:
				aquamarine = val;
				text3 = (IsFiltered(item2.RoleProto) ? text3 : string.Empty);
				text4 = (IsFiltered(item2.RoleProto) ? text.ToUpper() : string.Empty);
				break;
			case AdminOverlayAntagFormat.Subtype:
				aquamarine = val;
				text3 = (IsFiltered(item2.RoleProto) ? text3 : string.Empty);
				text4 = (IsFiltered(item2.RoleProto) ? _roles.GetRoleSubtypeLabel(LocId.op_Implicit(text), item2.Subtype).ToUpper() : string.Empty);
				break;
			default:
				aquamarine = Color.OrangeRed;
				text3 = (item2.Antag ? text3 : string.Empty);
				text4 = (item2.Antag ? _antagLabelClassic : string.Empty);
				break;
			}
			aquamarine.A = a;
			string text5 = ((!string.IsNullOrEmpty(text3)) ? Loc.GetString("player-tab-character-name-antag-symbol", new(string, object)[2]
			{
				("symbol", text3),
				("name", text4)
			}) : text4);
			((OverlayDrawArgs)(ref args)).ScreenHandle.DrawString(_fontBold, vector4 + vector5, (ReadOnlySpan<char>)text5, uIScale, aquamarine);
			vector5 += vector;
			list.Add((vector2, vector5));
		}
	}

	private static bool IsFiltered(ProtoId<RoleTypePrototype>? roleProtoId)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (!roleProtoId.HasValue)
		{
			return false;
		}
		return Filter.Contains(roleProtoId.Value);
	}
}
