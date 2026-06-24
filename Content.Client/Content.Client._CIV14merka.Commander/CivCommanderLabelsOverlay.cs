using System;
using System.Numerics;
using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.Teams;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderLabelsOverlay : Overlay
{
	[Dependency]
	private IEyeManager _eye;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IResourceCache _cache;

	private readonly IEntityManager _entity;

	private readonly CivCommanderLinesSystem _system;

	private readonly Font _font;

	private readonly Font _lineFont;

	private const float LineLabelStep = 5f;

	public override OverlaySpace Space => (OverlaySpace)2;

	public CivCommanderLabelsOverlay(IEntityManager entity, CivCommanderLinesSystem system)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		IoCManager.InjectDependencies<CivCommanderLabelsOverlay>(this);
		_entity = entity;
		_system = system;
		_font = (Font)new VectorFont(_cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Bold.ttf", true), 18);
		_lineFont = (Font)new VectorFont(_cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Bold.ttf", true), 11);
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
		if (!_entity.TryGetComponent<CivTeamMemberComponent>(valueOrDefault, ref civTeamMemberComponent) || civTeamMemberComponent.TeamId == 0)
		{
			return;
		}
		int teamId = civTeamMemberComponent.TeamId;
		DrawingHandleScreen screenHandle = ((OverlayDrawArgs)(ref args)).ScreenHandle;
		MapId mapId = args.MapId;
		if (mapId == MapId.Nullspace)
		{
			return;
		}
		foreach (CivCommanderLabelState value in _system.Labels.Values)
		{
			if (value.TeamId == teamId && !(value.MapId != mapId))
			{
				DrawLabel(screenHandle, value.Position, value.Rotation, value.Text, CivCommanderLinesOverlay.GetColor(value.Color), 1f);
			}
		}
		if (_system.IsCommander() && _system.IsPlacingLabel)
		{
			Vector2 cursorWorldPosition = _system.GetCursorWorldPosition();
			Color color = CivCommanderLinesOverlay.GetColor(_system.SelectedColor);
			DrawLabel(screenHandle, cursorWorldPosition, _system.PendingLabelRotation, _system.PendingLabelText, color, 0.7f);
		}
		foreach (CivCommanderLineState value2 in _system.Lines.Values)
		{
			if (value2.TeamId != teamId || value2.MapId != mapId)
			{
				continue;
			}
			Vector2 vector = value2.End - value2.Start;
			float num = vector.Length();
			if (!(num < 0.1f))
			{
				string label = CivCommanderLinesOverlay.GetLabel(value2.Color);
				Color color2 = CivCommanderLinesOverlay.GetColor(value2.Color);
				int num2 = Math.Max(1, (int)(num / 5f));
				for (int i = 0; i < num2; i++)
				{
					float num3 = ((float)i + 0.5f) / (float)num2;
					Vector2 worldPos = value2.Start + vector * num3;
					DrawLineTypeLabel(screenHandle, worldPos, label, color2);
				}
			}
		}
	}

	private void DrawLabel(DrawingHandleScreen screen, Vector2 worldPos, float rotation, string text, Color color, float alpha)
	{
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrEmpty(text))
		{
			Vector2 vector = _eye.WorldToScreen(worldPos);
			Vector2 dimensions = screen.GetDimensions(_font, (ReadOnlySpan<char>)text, 1f);
			if (!(dimensions.X <= 0f))
			{
				float num = dimensions.X * 0.5f;
				float num2 = dimensions.Y * 0.5f;
				float num3 = MathF.Cos(rotation);
				float num4 = MathF.Sin(rotation);
				Matrix3x2 matrix3x = new Matrix3x2(num3, num4, 0f - num4, num3, vector.X, vector.Y);
				Matrix3x2 transform = ((DrawingHandleBase)screen).GetTransform();
				((DrawingHandleBase)screen).SetTransform(ref matrix3x);
				float num5 = 4f;
				UIBox2 val = default(UIBox2);
				((UIBox2)(ref val))._002Ector(0f - num - num5, 0f - num2 - num5, num + num5, num2 + num5);
				UIBox2 val2 = val;
				Color black = Color.Black;
				screen.DrawRect(val2, ((Color)(ref black)).WithAlpha(0.55f * alpha), true);
				screen.DrawRect(val, ((Color)(ref color)).WithAlpha(alpha), false);
				Vector2 vector2 = new Vector2(0f - num, 0f - num2);
				screen.DrawString(_font, vector2, (ReadOnlySpan<char>)text, 1f, ((Color)(ref color)).WithAlpha(alpha));
				((DrawingHandleBase)screen).SetTransform(ref transform);
			}
		}
	}

	private void DrawLineTypeLabel(DrawingHandleScreen screen, Vector2 worldPos, string text, Color color)
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrEmpty(text))
		{
			Vector2 vector = _eye.WorldToScreen(worldPos);
			Vector2 dimensions = screen.GetDimensions(_lineFont, (ReadOnlySpan<char>)text, 1f);
			if (!(dimensions.X <= 0f))
			{
				float num = dimensions.X * 0.5f;
				float num2 = dimensions.Y * 0.5f;
				UIBox2 val = default(UIBox2);
				((UIBox2)(ref val))._002Ector(vector.X - num - 3f, vector.Y - num2 - 3f, vector.X + num + 3f, vector.Y + num2 + 3f);
				UIBox2 val2 = val;
				Color black = Color.Black;
				screen.DrawRect(val2, ((Color)(ref black)).WithAlpha(0.6f), true);
				screen.DrawRect(val, ((Color)(ref color)).WithAlpha(0.6f), false);
				screen.DrawString(_lineFont, new Vector2(vector.X - num, vector.Y - num2), (ReadOnlySpan<char>)text, 1f, color);
			}
		}
	}
}
