using System;
using System.Numerics;
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
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client._CIV14merka.Teams;

public sealed class CivTeamIconOverlay : Overlay
{
	private static readonly Color TeamColor = Color.FromHex((ReadOnlySpan<char>)"#4da6ff", (Color?)null);

	private static readonly Color SquadColor = Color.FromHex((ReadOnlySpan<char>)"#54ff72", (Color?)null);

	private static readonly Color SquadLeaderColor = Color.FromHex((ReadOnlySpan<char>)"#ffd54f", (Color?)null);

	private static readonly Color GhostBlueTeamColor = Color.FromHex((ReadOnlySpan<char>)"#4da6ff", (Color?)null);

	private static readonly Color GhostRedTeamColor = Color.FromHex((ReadOnlySpan<char>)"#ff5c5c", (Color?)null);

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

	public override OverlaySpace Space => (OverlaySpace)8;

	public CivTeamIconOverlay()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<CivTeamIconOverlay>(this);
		_container = _entity.System<ContainerSystem>();
		_transform = _entity.System<SharedTransformSystem>();
		_sprite = _entity.System<SpriteSystem>();
		_mobState = _entity.System<MobStateSystem>();
		_shader = _prototype.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("shaded")).Instance();
		_xformQuery = _entity.GetEntityQuery<TransformComponent>();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_players).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		bool flag = _entity.HasComponent<GhostComponent>(localEntity.Value);
		CivTeamMemberComponent civTeamMemberComponent = null;
		if (!flag && !_entity.TryGetComponent<CivTeamMemberComponent>(localEntity.Value, ref civTeamMemberComponent))
		{
			return;
		}
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		IEye eye = args.Viewport.Eye;
		_003F val = ((eye != null) ? eye.Rotation : default(Angle));
		Matrix3x2 value = Matrix3x2.CreateScale(Vector2.One);
		Matrix3x2 value2 = Matrix3Helpers.CreateRotation(Angle.op_Implicit(-(Angle)val));
		((DrawingHandleBase)worldHandle).UseShader(_shader);
		AllEntityQueryEnumerator<CivTeamMemberComponent, SpriteComponent, TransformComponent> val2 = _entity.AllEntityQueryEnumerator<CivTeamMemberComponent, SpriteComponent, TransformComponent>();
		EntityUid val3 = default(EntityUid);
		CivTeamMemberComponent civTeamMemberComponent2 = default(CivTeamMemberComponent);
		SpriteComponent val4 = default(SpriteComponent);
		TransformComponent val5 = default(TransformComponent);
		MobStateComponent component = default(MobStateComponent);
		while (val2.MoveNext(ref val3, ref civTeamMemberComponent2, ref val4, ref val5))
		{
			if (val5.MapID != args.MapId || !val4.Visible || ((SharedContainerSystem)_container).IsEntityOrParentInContainer(val3, (MetaDataComponent)null, val5) || (_entity.TryGetComponent<MobStateComponent>(val3, ref component) && _mobState.IsDead(val3, component)))
			{
				continue;
			}
			Box2 localBounds = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((val3, val4)));
			Vector2 worldPosition = _transform.GetWorldPosition(val5, _xformQuery);
			Box2 val6 = ((Box2)(ref localBounds)).Translated(worldPosition);
			if (!((Box2)(ref val6)).Intersects(ref args.WorldAABB))
			{
				continue;
			}
			Color val7;
			if (!flag)
			{
				if (civTeamMemberComponent == null || civTeamMemberComponent.TeamId <= 0 || civTeamMemberComponent2.TeamId <= 0 || civTeamMemberComponent.TeamId != civTeamMemberComponent2.TeamId)
				{
					continue;
				}
				val7 = ((civTeamMemberComponent.SquadId == 0 || civTeamMemberComponent.SquadId != civTeamMemberComponent2.SquadId) ? TeamColor : (civTeamMemberComponent2.IsSquadLeader ? SquadLeaderColor : SquadColor));
			}
			else
			{
				val7 = (Color)(civTeamMemberComponent2.TeamId switch
				{
					1 => GhostBlueTeamColor, 
					2 => GhostRedTeamColor, 
					_ => Color.White, 
				});
				if (civTeamMemberComponent2.TeamId <= 0 || val7 == Color.White)
				{
					continue;
				}
			}
			Matrix3x2 value3 = Matrix3x2.CreateTranslation(worldPosition);
			Matrix3x2 value4 = Matrix3x2.Multiply(value, value3);
			Matrix3x2 matrix3x = Matrix3x2.Multiply(value2, value4);
			((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
			Texture val8 = _sprite.Frame0((SpriteSpecifier)(object)CivTeamIconResolver.GetClassBadgeBackground(civTeamMemberComponent2.TeamId));
			Color classBadgeColor = CivTeamIconResolver.GetClassBadgeColor(civTeamMemberComponent2.TeamId);
			Texture val9 = _sprite.Frame0((SpriteSpecifier)(object)CivTeamIconResolver.GetClassIcon(civTeamMemberComponent2.Class, civTeamMemberComponent2.TeamId));
			float y = 0.1f + (((Box2)(ref localBounds)).Height + val4.Offset.Y) / 2f - (float)val9.Height / 32f;
			float x = 0.1f + (((Box2)(ref localBounds)).Width + val4.Offset.X) / 2f - (float)val9.Width / 32f;
			Vector2 vector = new Vector2(x, y);
			((DrawingHandleBase)worldHandle).DrawTexture(val8, vector, (Color?)classBadgeColor);
			((DrawingHandleBase)worldHandle).DrawTexture(val9, vector, (Color?)null);
			Rsi teamFlag = CivTeamIconResolver.GetTeamFlag(civTeamMemberComponent2.TeamId);
			if (teamFlag != null)
			{
				Texture val10 = _sprite.Frame0((SpriteSpecifier)(object)teamFlag);
				float y2 = (((Box2)(ref localBounds)).Height + val4.Offset.Y) / 2f - (float)val10.Height / 32f;
				float x2 = (0f - (((Box2)(ref localBounds)).Width + val4.Offset.X)) / 2f;
				((DrawingHandleBase)worldHandle).DrawTexture(val10, new Vector2(x2, y2), (Color?)null);
			}
			Rsi squadBadge = CivTeamIconResolver.GetSquadBadge(civTeamMemberComponent2.SquadId);
			if (squadBadge != null)
			{
				Texture val11 = _sprite.Frame0((SpriteSpecifier)(object)squadBadge);
				float num = (0f - (float)val11.Height) / 2f / 32f;
				float y3 = 0.1f + (((Box2)(ref localBounds)).Height + val4.Offset.Y + num) / 2f - (float)val11.Height / 32f;
				float num2 = (((Box2)(ref localBounds)).Width + val4.Offset.X) / 2f - (float)val11.Width / 32f;
				if (civTeamMemberComponent2.SquadId >= 10)
				{
					float num3 = 0.125f;
					num2 += num3;
				}
				Vector2 vector2 = new Vector2(num2, y3);
				((DrawingHandleBase)worldHandle).DrawTexture(val11, vector2, (Color?)val7);
			}
		}
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)worldHandle).SetTransform(ref identity);
		((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
	}
}
