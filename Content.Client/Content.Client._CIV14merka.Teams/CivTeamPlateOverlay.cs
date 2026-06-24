using System;
using System.Collections.Generic;
using System.Numerics;
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
using Robust.Shared.Maths;
using Robust.Shared.Player;

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

	public override OverlaySpace Space => (OverlaySpace)2;

	public CivTeamPlateOverlay(IEntityManager entity, IPlayerManager player, CivTeamVisualsSystem teams)
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<CivTeamPlateOverlay>(this);
		_entity = entity;
		_player = player;
		_teams = teams;
		_transform = entity.System<SharedTransformSystem>();
		_sprite = entity.System<SpriteSystem>();
		_mobState = entity.System<MobStateSystem>();
		_combatMode = entity.System<CombatModeSystem>();
		_font = _resourceCache.NotoStack("Bold");
		_xformQuery = entity.GetEntityQuery<TransformComponent>();
		((Overlay)this).ZIndex = 150;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		TransformComponent val = default(TransformComponent);
		if (!localEntity.HasValue || (!_entity.HasComponent<CivTeamMemberComponent>(localEntity.Value) && !_entity.HasComponent<GhostComponent>(localEntity.Value)) || !_cfg.GetCVar<bool>(CCVars.Civ14ShowNearbyNames) || _combatMode.IsInCombatMode(localEntity.Value) || !_entity.TryGetComponent<TransformComponent>(localEntity.Value, ref val))
		{
			return;
		}
		Vector2 worldPosition = _transform.GetWorldPosition(val, _xformQuery);
		float uIScale = ((Control)_userInterface.RootControl).UIScale;
		Vector2 vector = new Vector2(6f, 3f) * uIScale;
		AllEntityQueryEnumerator<CivTeamMemberComponent, SpriteComponent, TransformComponent, MetaDataComponent> val2 = _entity.AllEntityQueryEnumerator<CivTeamMemberComponent, SpriteComponent, TransformComponent, MetaDataComponent>();
		EntityUid val3 = default(EntityUid);
		CivTeamMemberComponent member = default(CivTeamMemberComponent);
		SpriteComponent val4 = default(SpriteComponent);
		TransformComponent val5 = default(TransformComponent);
		MetaDataComponent val6 = default(MetaDataComponent);
		MobStateComponent component = default(MobStateComponent);
		while (val2.MoveNext(ref val3, ref member, ref val4, ref val5, ref val6))
		{
			if (val3 == localEntity.Value || val5.MapID != args.MapId || !val4.Visible || !_teams.TryGetRelationColor(val3, out var color) || (_entity.TryGetComponent<MobStateComponent>(val3, ref component) && _mobState.IsDead(val3, component)))
			{
				continue;
			}
			Vector2 worldPosition2 = _transform.GetWorldPosition(val5, _xformQuery);
			if ((worldPosition2 - worldPosition).LengthSquared() > 9f)
			{
				continue;
			}
			string entityName = val6.EntityName;
			if (!string.IsNullOrWhiteSpace(entityName))
			{
				string prefix = GetPrefix(member);
				if (!_labelCache.TryGetValue(val3, out (string, string, string) value) || value.Item1 != prefix || value.Item2 != entityName)
				{
					value = (prefix, entityName, prefix + " " + entityName);
					_labelCache[val3] = value;
				}
				string item = value.Item3;
				Box2 localBounds = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((val3, val4)));
				Box2 val7 = ((Box2)(ref localBounds)).Translated(worldPosition2);
				if (((Box2)(ref val7)).Intersects(ref args.WorldAABB))
				{
					Vector2 dimensions = ((OverlayDrawArgs)(ref args)).ScreenHandle.GetDimensions(_font, (ReadOnlySpan<char>)item, uIScale);
					Vector2 vector2 = Vector2Helpers.Rounded(_eye.WorldToScreen(worldPosition2));
					float num = ((Box2)(ref localBounds)).Height * 32f / 2f + 20f * uIScale;
					Vector2 vector3 = new Vector2(vector2.X - dimensions.X / 2f, vector2.Y - num - dimensions.Y);
					UIBox2 val8 = UIBox2.FromDimensions(vector3 - vector, dimensions + vector * 2f);
					((OverlayDrawArgs)(ref args)).ScreenHandle.DrawRect(val8, new Color((byte)12, (byte)18, (byte)26, (byte)190), true);
					((OverlayDrawArgs)(ref args)).ScreenHandle.DrawRect(val8, ((Color)(ref color)).WithAlpha(0.95f), false);
					((OverlayDrawArgs)(ref args)).ScreenHandle.DrawString(_font, vector3, (ReadOnlySpan<char>)item, uIScale, color);
				}
			}
		}
	}

	private string GetPrefix(CivTeamMemberComponent member)
	{
		if (member.IsCommander)
		{
			return PrefixCommander;
		}
		if (member.SquadId == 0)
		{
			return PrefixReserve;
		}
		Dictionary<int, string> dictionary = (member.IsSquadLeader ? _squadLeaderPrefixes : _squadPrefixes);
		if (dictionary.TryGetValue(member.SquadId, out var value))
		{
			return value;
		}
		string text = (member.IsSquadLeader ? $"[LS{member.SquadId}]" : $"[S{member.SquadId}]");
		dictionary[member.SquadId] = text;
		return text;
	}
}
