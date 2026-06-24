using System;
using System.Numerics;
using Content.Shared._CIV14merka.Factions;
using Content.Shared._CIV14merka.Pvo;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client._CIV14merka.Pvo;

public sealed class CivPvoOverlay : Overlay
{
	private static readonly Color FallbackColor = Color.FromHex((ReadOnlySpan<char>)"#C8C8C8", (Color?)null);

	private const int FilledCircleSegments = 40;

	private const int OutlineSegments = 80;

	private readonly IEntityManager _entityManager;

	private readonly SharedTransformSystem _transform;

	private readonly IPrototypeManager _prototype;

	private readonly Vector2[] _filledTriangle = new Vector2[3];

	private readonly Vector2[] _outlinePoints = new Vector2[81];

	public override OverlaySpace Space => (OverlaySpace)4;

	public CivPvoOverlay(IEntityManager entityManager, SharedTransformSystem transform, IPrototypeManager prototype)
	{
		_entityManager = entityManager;
		_transform = transform;
		_prototype = prototype;
		((Overlay)this).ZIndex = -5;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		EntityQueryEnumerator<CivPvoComponent, TransformComponent> val = _entityManager.EntityQueryEnumerator<CivPvoComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		CivPvoComponent civPvoComponent = default(CivPvoComponent);
		TransformComponent val3 = default(TransformComponent);
		while (val.MoveNext(ref val2, ref civPvoComponent, ref val3))
		{
			if (!(val3.MapID != args.MapId) && !(civPvoComponent.Radius <= 0f))
			{
				Vector2 position = _transform.GetMapCoordinates(val3).Position;
				Color color = GetColor(civPvoComponent.SideId);
				DrawFilledCircle(worldHandle, position, civPvoComponent.Radius, ((Color)(ref color)).WithAlpha(0.12f));
				DrawCircleOutline(worldHandle, position, civPvoComponent.Radius, ((Color)(ref color)).WithAlpha(0.9f));
			}
		}
	}

	private void DrawFilledCircle(DrawingHandleWorld handle, Vector2 center, float radius, Color color)
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		_filledTriangle[0] = center;
		for (int i = 0; i < 40; i++)
		{
			float x = (float)i / 40f * MathF.PI * 2f;
			float x2 = (float)(i + 1) / 40f * MathF.PI * 2f;
			_filledTriangle[1] = new Vector2(center.X + MathF.Cos(x) * radius, center.Y + MathF.Sin(x) * radius);
			_filledTriangle[2] = new Vector2(center.X + MathF.Cos(x2) * radius, center.Y + MathF.Sin(x2) * radius);
			((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)1, (ReadOnlySpan<Vector2>)_filledTriangle, color);
		}
	}

	private void DrawCircleOutline(DrawingHandleWorld handle, Vector2 center, float radius, Color color)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i <= 80; i++)
		{
			float x = (float)i / 80f * MathF.PI * 2f;
			_outlinePoints[i] = new Vector2(center.X + MathF.Cos(x) * radius, center.Y + MathF.Sin(x) * radius);
		}
		((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)5, (ReadOnlySpan<Vector2>)_outlinePoints, color);
	}

	private Color GetColor(string sideId)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		CivFactionPrototype civFactionPrototype = default(CivFactionPrototype);
		if (!string.IsNullOrWhiteSpace(sideId) && _prototype.TryIndex<CivFactionPrototype>(sideId, ref civFactionPrototype))
		{
			return civFactionPrototype.Color;
		}
		return FallbackColor;
	}
}
