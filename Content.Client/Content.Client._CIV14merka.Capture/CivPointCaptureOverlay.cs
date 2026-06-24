using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Content.Client._CIV14merka.UserInterface.Systems.Hud;
using Content.Shared._CIV14merka.Capture;
using Content.Shared._CIV14merka.Teams;
using Content.Shared.Ghost;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client._CIV14merka.Capture;

public sealed class CivPointCaptureOverlay : Overlay
{
	private readonly IEntityManager _entity;

	private readonly IPlayerManager _player;

	private readonly SharedTransformSystem _transform;

	private readonly List<Vector2> _arcPoints = new List<Vector2>();

	public override OverlaySpace Space => (OverlaySpace)4;

	public CivPointCaptureOverlay(IEntityManager entity, IPlayerManager player)
	{
		IoCManager.InjectDependencies<CivPointCaptureOverlay>(this);
		_entity = entity;
		_player = player;
		_transform = entity.System<SharedTransformSystem>();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetViewerTeamId(out var viewerTeamId))
		{
			return;
		}
		AllEntityQueryEnumerator<CivPointCapturePointComponent, TransformComponent> val = _entity.AllEntityQueryEnumerator<CivPointCapturePointComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		CivPointCapturePointComponent civPointCapturePointComponent = default(CivPointCapturePointComponent);
		TransformComponent val3 = default(TransformComponent);
		while (val.MoveNext(ref val2, ref civPointCapturePointComponent, ref val3))
		{
			if (val3.MapID == MapId.Nullspace || val3.MapID != args.MapId)
			{
				continue;
			}
			Vector2 position = _transform.GetMapCoordinates(val3).Position;
			float num = Math.Max(0.5f, civPointCapturePointComponent.CaptureRadius);
			if (CircleIntersectsWorld(args.WorldAABB, position, num + 0.5f))
			{
				Color relationColor = CivPointCaptureColorResolver.GetRelationColor(viewerTeamId, civPointCapturePointComponent.OwnerTeamId);
				bool num2 = civPointCapturePointComponent.CapturingTeamId != 0 && civPointCapturePointComponent.CaptureProgress > 0f;
				((DrawingHandleBase)((OverlayDrawArgs)(ref args)).WorldHandle).DrawCircle(position, num, ((Color)(ref relationColor)).WithAlpha(0.1f), true);
				((DrawingHandleBase)((OverlayDrawArgs)(ref args)).WorldHandle).DrawCircle(position, num, ((Color)(ref relationColor)).WithAlpha(0.95f), false);
				DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
				float num3 = num + 0.08f;
				Color black = Color.Black;
				((DrawingHandleBase)worldHandle).DrawCircle(position, num3, ((Color)(ref black)).WithAlpha(0.65f), false);
				((DrawingHandleBase)((OverlayDrawArgs)(ref args)).WorldHandle).DrawCircle(position, 0.18f, ((Color)(ref relationColor)).WithAlpha(0.82f), true);
				if (num2)
				{
					Color relationColor2 = CivPointCaptureColorResolver.GetRelationColor(viewerTeamId, civPointCapturePointComponent.CapturingTeamId);
					DrawProgressArc(((OverlayDrawArgs)(ref args)).WorldHandle, position, num + 0.18f, civPointCapturePointComponent.CaptureProgress, relationColor2);
				}
			}
		}
	}

	private bool TryGetViewerTeamId(out int viewerTeamId)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		viewerTeamId = 0;
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return false;
		}
		CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
		if (_entity.TryGetComponent<CivTeamMemberComponent>(localEntity.Value, ref civTeamMemberComponent))
		{
			viewerTeamId = civTeamMemberComponent.TeamId;
			return viewerTeamId > 0;
		}
		if (!_entity.HasComponent<GhostComponent>(localEntity.Value))
		{
			return false;
		}
		CivHudEventsSystem civHudEventsSystem = _entity.System<CivHudEventsSystem>();
		viewerTeamId = civHudEventsSystem.LastStatus?.ViewerTeamId ?? 0;
		return viewerTeamId > 0;
	}

	private void DrawProgressArc(DrawingHandleWorld handle, Vector2 center, float radius, float progress, Color color)
	{
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		progress = Math.Clamp(progress, 0f, 1f);
		if (!(progress <= 0f))
		{
			int num = Math.Max(6, (int)MathF.Ceiling(28f * progress));
			_arcPoints.Clear();
			for (int i = 0; i <= num; i++)
			{
				float num2 = progress * (float)i / (float)num;
				float x = -MathF.PI / 2f + MathF.PI * 2f * num2;
				_arcPoints.Add(center + new Vector2(MathF.Cos(x), MathF.Sin(x)) * radius);
			}
			if (_arcPoints.Count >= 2)
			{
				((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)5, (ReadOnlySpan<Vector2>)CollectionsMarshal.AsSpan(_arcPoints), color);
			}
		}
	}

	private static bool CircleIntersectsWorld(Box2 worldBounds, Vector2 center, float radius)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		float x = Math.Clamp(center.X, worldBounds.Left, worldBounds.Right);
		float y = Math.Clamp(center.Y, worldBounds.Bottom, worldBounds.Top);
		return (center - new Vector2(x, y)).LengthSquared() <= radius * radius;
	}
}
