using System;
using System.Numerics;
using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.Teams;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderLinesOverlay : Overlay
{
	[Dependency]
	private IPlayerManager _player;

	private readonly IEntityManager _entity;

	private readonly CivCommanderLinesSystem _system;

	private const float LineThickness = 0.36f;

	private const float EndpointRadius = 0.22f;

	public override OverlaySpace Space => (OverlaySpace)8;

	public CivCommanderLinesOverlay(IEntityManager entity, CivCommanderLinesSystem system)
	{
		IoCManager.InjectDependencies<CivCommanderLinesOverlay>(this);
		_entity = entity;
		_system = system;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
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
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		foreach (CivCommanderLineState value in _system.Lines.Values)
		{
			if (value.TeamId == teamId && !(value.MapId != args.MapId))
			{
				Color color = GetColor(value.Color);
				DrawThickLine(worldHandle, value.Start, value.End, 0.36f, color);
				((DrawingHandleBase)worldHandle).DrawCircle(value.Start, 0.22f, color, true);
				((DrawingHandleBase)worldHandle).DrawCircle(value.End, 0.22f, color, true);
			}
		}
		if (_system.IsCommander() && _system.IsDrawing && _system.DrawStart.MapId == args.MapId)
		{
			Vector2 cursorWorldPosition = _system.GetCursorWorldPosition();
			Color color2 = GetColor(_system.SelectedColor);
			DrawThickLine(worldHandle, _system.DrawStart.Position, cursorWorldPosition, 0.36f, ((Color)(ref color2)).WithAlpha(0.65f));
			((DrawingHandleBase)worldHandle).DrawCircle(_system.DrawStart.Position, 0.22f, color2, true);
			((DrawingHandleBase)worldHandle).DrawCircle(cursorWorldPosition, 0.22f, ((Color)(ref color2)).WithAlpha(0.7f), true);
		}
	}

	private static void DrawThickLine(DrawingHandleWorld handle, Vector2 a, Vector2 b, float thickness, Color color)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = b - a;
		float num = vector.Length();
		if (!(num < 0.001f))
		{
			Vector2 vector2 = vector / num;
			Vector2 vector3 = new Vector2(0f - vector2.Y, vector2.X);
			float num2 = thickness * 0.5f;
			((DrawingHandleBase)handle).DrawLine(a, b, color);
			int num3 = (int)MathF.Ceiling(num2 / 0.04f);
			for (int i = 1; i <= num3; i++)
			{
				float num4 = MathF.Min(num2, (float)i * 0.04f);
				Vector2 vector4 = vector3 * num4;
				((DrawingHandleBase)handle).DrawLine(a + vector4, b + vector4, color);
				((DrawingHandleBase)handle).DrawLine(a - vector4, b - vector4, color);
			}
		}
	}

	public static Color GetColor(CivCommanderLineColor color)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(color switch
		{
			CivCommanderLineColor.Attack => Color.FromHex((ReadOnlySpan<char>)"#E63946", (Color?)null), 
			CivCommanderLineColor.Defense => Color.FromHex((ReadOnlySpan<char>)"#4DA6FF", (Color?)null), 
			CivCommanderLineColor.Route => Color.FromHex((ReadOnlySpan<char>)"#FFD23F", (Color?)null), 
			CivCommanderLineColor.Border => Color.FromHex((ReadOnlySpan<char>)"#F5F5F5", (Color?)null), 
			_ => Color.White, 
		});
	}

	public static string GetLabel(CivCommanderLineColor color)
	{
		return color switch
		{
			CivCommanderLineColor.Attack => Loc.GetString("civ-cmd-lines-color-attack"), 
			CivCommanderLineColor.Defense => Loc.GetString("civ-cmd-lines-color-defense"), 
			CivCommanderLineColor.Route => Loc.GetString("civ-cmd-lines-color-route"), 
			CivCommanderLineColor.Border => Loc.GetString("civ-cmd-lines-color-border"), 
			_ => color.ToString(), 
		};
	}
}
