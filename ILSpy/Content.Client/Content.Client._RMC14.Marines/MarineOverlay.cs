using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Marines;

public sealed class MarineOverlay : Overlay
{
	[Dependency]
	private IEntityManager _entity;

	[Dependency]
	private IPlayerManager _players;

	[Dependency]
	private IPrototypeManager _prototype;

	private static readonly Rsi FireteamOneRsi = new Rsi(new ResPath("_RMC14/Interface/marine_hud.rsi"), "hudsquad_ft1");

	private static readonly Rsi FireteamTwoRsi = new Rsi(new ResPath("_RMC14/Interface/marine_hud.rsi"), "hudsquad_ft2");

	private static readonly Rsi FireteamThreeRsi = new Rsi(new ResPath("_RMC14/Interface/marine_hud.rsi"), "hudsquad_ft3");

	private static readonly Rsi FireteamLeaderRsi = new Rsi(new ResPath("_RMC14/Interface/marine_hud.rsi"), "hudsquad_ftl");

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

	public override OverlaySpace Space => (OverlaySpace)8;

	public MarineOverlay()
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<MarineOverlay>(this);
		_npcFaction = _entity.System<NpcFactionSystem>();
		_container = _entity.System<ContainerSystem>();
		_marine = _entity.System<MarineSystem>();
		_sprite = _entity.System<SpriteSystem>();
		_transform = _entity.System<TransformSystem>();
		_npcFactionMemberQuery = _entity.GetEntityQuery<NpcFactionMemberComponent>();
		_fireteamLeaderQuery = _entity.GetEntityQuery<FireteamLeaderComponent>();
		_fireteamMemberQuery = _entity.GetEntityQuery<FireteamMemberComponent>();
		_invisQuery = _entity.GetEntityQuery<EntityActiveInvisibleComponent>();
		_marineIconsQuery = _entity.GetEntityQuery<ShowMarineIconsComponent>();
		_paraDroppingQuery = _entity.GetEntityQuery<ParaDroppingComponent>();
		_crashLandingQuery = _entity.GetEntityQuery<CrashLandingComponent>();
		_shader = _prototype.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("shaded")).Instance();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		ShowMarineIconsComponent showMarineIconsComponent = default(ShowMarineIconsComponent);
		if (!_marineIconsQuery.TryComp(((ISharedPlayerManager)_players).LocalEntity, ref showMarineIconsComponent))
		{
			return;
		}
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		IEye eye = args.Viewport.Eye;
		_003F val = ((eye != null) ? eye.Rotation : default(Angle));
		EntityQuery<TransformComponent> entityQuery = _entity.GetEntityQuery<TransformComponent>();
		Matrix3x2 value = Matrix3x2.CreateScale(new Vector2(1f, 1f));
		Matrix3x2 value2 = Matrix3Helpers.CreateRotation(Angle.op_Implicit(-(Angle)val));
		((DrawingHandleBase)worldHandle).UseShader(_shader);
		Texture val2 = _sprite.Frame0((SpriteSpecifier)(object)FireteamOneRsi);
		Texture val3 = _sprite.Frame0((SpriteSpecifier)(object)FireteamTwoRsi);
		Texture val4 = _sprite.Frame0((SpriteSpecifier)(object)FireteamThreeRsi);
		Texture val5 = _sprite.Frame0((SpriteSpecifier)(object)FireteamLeaderRsi);
		AllEntityQueryEnumerator<MarineComponent, StatusIconComponent, SpriteComponent, TransformComponent> val6 = _entity.AllEntityQueryEnumerator<MarineComponent, StatusIconComponent, SpriteComponent, TransformComponent>();
		EntityUid val7 = default(EntityUid);
		MarineComponent marineComponent = default(MarineComponent);
		StatusIconComponent statusIconComponent = default(StatusIconComponent);
		SpriteComponent val8 = default(SpriteComponent);
		TransformComponent val9 = default(TransformComponent);
		NpcFactionMemberComponent npcFactionMemberComponent = default(NpcFactionMemberComponent);
		FireteamMemberComponent fireteamMemberComponent = default(FireteamMemberComponent);
		while (val6.MoveNext(ref val7, ref marineComponent, ref statusIconComponent, ref val8, ref val9))
		{
			if (val9.MapID != args.MapId)
			{
				continue;
			}
			Box2 val10 = (Box2)(((_003F?)statusIconComponent.Bounds) ?? _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((val7, val8))));
			Vector2 worldPosition = ((SharedTransformSystem)_transform).GetWorldPosition(val9, entityQuery);
			Box2 val11 = ((Box2)(ref val10)).Translated(worldPosition);
			if (!((Box2)(ref val11)).Intersects(ref args.WorldAABB) || ((SharedContainerSystem)_container).IsEntityOrParentInContainer(val7, (MetaDataComponent)null, (TransformComponent)null) || _invisQuery.HasComp(val7))
			{
				continue;
			}
			Matrix3x2 value3 = Matrix3x2.CreateTranslation(worldPosition);
			Matrix3x2 value4 = Matrix3x2.Multiply(value, value3);
			Matrix3x2 matrix3x = Matrix3x2.Multiply(value2, value4);
			((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
			GetMarineIconEvent marineIcon = _marine.GetMarineIcon(val7);
			Dictionary<ProtoId<NpcFactionPrototype>, SpriteSpecifier> factionIcons = _marine.GetFactionIcons(val7);
			if (showMarineIconsComponent.Factions != null && !_npcFaction.IsMemberOfAny(Entity<NpcFactionMemberComponent>.op_Implicit(val7), showMarineIconsComponent.Factions) && factionIcons != null && _npcFactionMemberQuery.TryComp(val7, ref npcFactionMemberComponent) && factionIcons.TryGetValue(npcFactionMemberComponent.Factions.First(), out var value5))
			{
				marineIcon.Background = null;
				marineIcon.Icon = value5;
			}
			if (marineIcon.Icon != null)
			{
				Texture val12 = _sprite.Frame0(marineIcon.Icon);
				float y = 0.1f + (((Box2)(ref val10)).Height + val8.Offset.Y) / 2f - (float)val12.Height / 32f;
				float x = 0.1f + (((Box2)(ref val10)).Width + val8.Offset.X) / 2f - (float)val12.Width / 32f;
				if (_crashLandingQuery.HasComp(val7) || _paraDroppingQuery.HasComp(val7))
				{
					y = 0.1f + val8.Offset.Y;
					x = 0.1f + val8.Offset.X;
				}
				Vector2 vector = new Vector2(x, y);
				if (marineIcon.Icon != null && marineIcon.Background != null)
				{
					Texture val13 = _sprite.Frame0(marineIcon.Background);
					((DrawingHandleBase)worldHandle).DrawTexture(val13, vector, marineIcon.BackgroundColor);
				}
				((DrawingHandleBase)worldHandle).DrawTexture(val12, vector, (Color?)null);
			}
			if (_fireteamMemberQuery.TryComp(val7, ref fireteamMemberComponent))
			{
				Texture val14 = (Texture)(fireteamMemberComponent.Fireteam switch
				{
					0 => val2, 
					1 => val3, 
					2 => val4, 
					_ => null, 
				});
				if (val14 != null)
				{
					float num = (0f - (float)val2.Height) / 2f / 32f;
					float y2 = 0.1f + (((Box2)(ref val10)).Height + val8.Offset.Y + num) / 2f - (float)val14.Height / 32f;
					float x2 = (((Box2)(ref val10)).Width + val8.Offset.X) / 2f - (float)val14.Width / 32f;
					Vector2 vector2 = new Vector2(x2, y2);
					((DrawingHandleBase)worldHandle).DrawTexture(val14, vector2, marineIcon.BackgroundColor);
				}
			}
			if (_fireteamLeaderQuery.HasComp(val7))
			{
				Texture val15 = val5;
				float num2 = (0f - (float)val2.Height) / 2f / 32f;
				float y3 = 0.1f + (((Box2)(ref val10)).Height + val8.Offset.Y + num2) / 2f - (float)val15.Height / 32f;
				float x3 = (((Box2)(ref val10)).Width + val8.Offset.X) / 2f - (float)val15.Width / 32f;
				Vector2 vector3 = new Vector2(x3, y3);
				((DrawingHandleBase)worldHandle).DrawTexture(val15, vector3, marineIcon.BackgroundColor);
			}
		}
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)worldHandle).SetTransform(ref identity);
		((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
	}
}
