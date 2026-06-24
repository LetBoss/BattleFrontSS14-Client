using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.Teams;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderBotOverlay : Overlay
{
	private static readonly Color SelectedRingColor = Color.FromHex((ReadOnlySpan<char>)"#ffd54f", (Color?)null);

	private static readonly Color BoxSelectColor = Color.FromHex((ReadOnlySpan<char>)"#4da6ff", (Color?)null);

	private static readonly Color PatrolColor = Color.FromHex((ReadOnlySpan<char>)"#ff9800", (Color?)null);

	private static readonly Color OrderIdleColor = Color.FromHex((ReadOnlySpan<char>)"#9e9e9e", (Color?)null);

	private static readonly Color OrderMoveColor = Color.FromHex((ReadOnlySpan<char>)"#4caf50", (Color?)null);

	private static readonly Color OrderAttackMoveColor = Color.FromHex((ReadOnlySpan<char>)"#f44336", (Color?)null);

	private static readonly Color OrderHoldColor = Color.FromHex((ReadOnlySpan<char>)"#2196f3", (Color?)null);

	private static readonly Color OrderFollowColor = Color.FromHex((ReadOnlySpan<char>)"#9c27b0", (Color?)null);

	private static readonly Color OrderDefendColor = Color.FromHex((ReadOnlySpan<char>)"#00bcd4", (Color?)null);

	private static readonly Color OrderPatrolColor = Color.FromHex((ReadOnlySpan<char>)"#ff9800", (Color?)null);

	[Dependency]
	private IEyeManager _eye;

	[Dependency]
	private IPlayerManager _player;

	private readonly IEntityManager _entity;

	private readonly CivCommanderBotControlSystem _control;

	private readonly SharedTransformSystem _transform;

	public override OverlaySpace Space => (OverlaySpace)8;

	public CivCommanderBotOverlay(IEntityManager entity, CivCommanderBotControlSystem control)
	{
		IoCManager.InjectDependencies<CivCommanderBotOverlay>(this);
		_entity = entity;
		_control = control;
		_transform = entity.System<SharedTransformSystem>();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
		if (!_entity.TryGetComponent<CivTeamMemberComponent>(valueOrDefault, ref civTeamMemberComponent) || !civTeamMemberComponent.IsCommander)
		{
			return;
		}
		int teamId = civTeamMemberComponent.TeamId;
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		EntityQueryEnumerator<CivCommanderBotComponent, CivTeamMemberComponent, TransformComponent> val = _entity.EntityQueryEnumerator<CivCommanderBotComponent, CivTeamMemberComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		CivCommanderBotComponent botComp = default(CivCommanderBotComponent);
		CivTeamMemberComponent civTeamMemberComponent2 = default(CivTeamMemberComponent);
		TransformComponent val3 = default(TransformComponent);
		MobStateComponent mobStateComponent = default(MobStateComponent);
		while (val.MoveNext(ref val2, ref botComp, ref civTeamMemberComponent2, ref val3))
		{
			if (civTeamMemberComponent2.TeamId == teamId && !(val3.MapID != args.MapId) && (!_entity.TryGetComponent<MobStateComponent>(val2, ref mobStateComponent) || mobStateComponent.CurrentState != MobState.Dead))
			{
				Vector2 position = _transform.GetMapCoordinates(val3).Position;
				if (CircleIntersects(args.WorldAABB, position, 1.5f))
				{
					bool isSelected = _control.SelectedBots.Contains(val2);
					DrawBotIndicator(worldHandle, position, botComp, isSelected);
				}
			}
		}
		if (_control.IsBoxSelecting)
		{
			DrawBoxSelection(in args);
		}
		if (_control.IsPatrolMode && _control.PatrolPoints.Count > 0)
		{
			DrawPatrolPreview(worldHandle);
		}
	}

	private void DrawBotIndicator(DrawingHandleWorld handle, Vector2 pos, CivCommanderBotComponent botComp, bool isSelected)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		float num = (isSelected ? 0.58f : 0.45f);
		Color orderColor = GetOrderColor(botComp.Order);
		Color val = (isSelected ? SelectedRingColor : orderColor);
		if (isSelected)
		{
			((DrawingHandleBase)handle).DrawCircle(pos, num + 0.15f, ((Color)(ref val)).WithAlpha(0.25f), true);
		}
		((DrawingHandleBase)handle).DrawCircle(pos, num, ((Color)(ref val)).WithAlpha(0.9f), false);
		float num2 = num + 0.06f;
		Color val2 = Color.Black;
		((DrawingHandleBase)handle).DrawCircle(pos, num2, ((Color)(ref val2)).WithAlpha(0.65f), false);
		if (botComp.SquadId > 0)
		{
			Vector2 vector = pos + new Vector2(0.35f, 0.35f);
			val2 = Color.Black;
			((DrawingHandleBase)handle).DrawCircle(vector, 0.22f, ((Color)(ref val2)).WithAlpha(0.7f), true);
			if (botComp.IsLeader)
			{
				val2 = Color.Gold;
				((DrawingHandleBase)handle).DrawCircle(vector, 0.13f, ((Color)(ref val2)).WithAlpha(0.9f), true);
			}
		}
		if (!string.IsNullOrEmpty(GetOrderIndicator(botComp.Order)))
		{
			Vector2 vector2 = pos + new Vector2(-0.3f, 0.4f);
			((DrawingHandleBase)handle).DrawCircle(vector2, 0.18f, ((Color)(ref orderColor)).WithAlpha(0.8f), true);
		}
	}

	private void DrawBoxSelection(in OverlayDrawArgs args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		MapCoordinates val = _eye.ScreenToMap(_control.BoxSelectStart);
		MapCoordinates val2 = _eye.ScreenToMap(_control.BoxSelectEnd);
		if (!(val.MapId != args.MapId) && !(val2.MapId != args.MapId))
		{
			Vector2 vector = new Vector2(MathF.Min(val.Position.X, val2.Position.X), MathF.Min(val.Position.Y, val2.Position.Y));
			Vector2 vector2 = new Vector2(MathF.Max(val.Position.X, val2.Position.X), MathF.Max(val.Position.Y, val2.Position.Y));
			Box2 val3 = default(Box2);
			((Box2)(ref val3))._002Ector(vector, vector2);
			((OverlayDrawArgs)(ref args)).WorldHandle.DrawRect(val3, ((Color)(ref BoxSelectColor)).WithAlpha(0.15f), true);
			((OverlayDrawArgs)(ref args)).WorldHandle.DrawRect(val3, ((Color)(ref BoxSelectColor)).WithAlpha(0.8f), false);
		}
	}

	private void DrawPatrolPreview(DrawingHandleWorld handle)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		IReadOnlyList<Vector2> patrolPoints = _control.PatrolPoints;
		if (patrolPoints.Count < 1)
		{
			return;
		}
		for (int i = 0; i < patrolPoints.Count; i++)
		{
			Vector2 vector = patrolPoints[i];
			((DrawingHandleBase)handle).DrawCircle(vector, 0.35f, ((Color)(ref PatrolColor)).WithAlpha(0.6f), true);
			((DrawingHandleBase)handle).DrawCircle(vector, 0.35f, PatrolColor, false);
			if (i > 0)
			{
				Vector2 vector2 = patrolPoints[i - 1];
				((DrawingHandleBase)handle).DrawLine(vector2, vector, ((Color)(ref PatrolColor)).WithAlpha(0.7f));
			}
		}
		if (patrolPoints.Count > 1)
		{
			((DrawingHandleBase)handle).DrawLine(patrolPoints[patrolPoints.Count - 1], patrolPoints[0], ((Color)(ref PatrolColor)).WithAlpha(0.4f));
		}
	}

	private static Color GetOrderColor(CivBotOrderType order)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(order switch
		{
			CivBotOrderType.Idle => OrderIdleColor, 
			CivBotOrderType.Move => OrderMoveColor, 
			CivBotOrderType.AttackMove => OrderAttackMoveColor, 
			CivBotOrderType.HoldPosition => OrderHoldColor, 
			CivBotOrderType.Follow => OrderFollowColor, 
			CivBotOrderType.Defend => OrderDefendColor, 
			CivBotOrderType.Patrol => OrderPatrolColor, 
			_ => Color.White, 
		});
	}

	private static string GetOrderIndicator(CivBotOrderType order)
	{
		return order switch
		{
			CivBotOrderType.AttackMove => "A", 
			CivBotOrderType.HoldPosition => "H", 
			CivBotOrderType.Follow => "F", 
			CivBotOrderType.Defend => "D", 
			CivBotOrderType.Patrol => "P", 
			_ => string.Empty, 
		};
	}

	private static bool CircleIntersects(Box2 world, Vector2 center, float radius)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		float x = Math.Clamp(center.X, world.Left, world.Right);
		float y = Math.Clamp(center.Y, world.Bottom, world.Top);
		return (center - new Vector2(x, y)).LengthSquared() <= radius * radius;
	}
}
