using System;
using Content.Shared._CIV14merka.GlobalMap;
using Robust.Shared.Maths;

namespace Content.Client._CIV14merka.GlobalMap;

public static class CivGlobalMapColorResolver
{
	public static readonly Color SquadColor = Color.FromHex((ReadOnlySpan<char>)"#6de685", (Color?)null);

	public static readonly Color TeamColor = Color.FromHex((ReadOnlySpan<char>)"#4da6ff", (Color?)null);

	public static readonly Color EnemyTeamColor = Color.FromHex((ReadOnlySpan<char>)"#d85d5d", (Color?)null);

	public static Color GetColor(CivGlobalMapMarkerType type)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(type switch
		{
			CivGlobalMapMarkerType.Attack => Color.FromHex((ReadOnlySpan<char>)"#ff5449", (Color?)null), 
			CivGlobalMapMarkerType.Defense => Color.FromHex((ReadOnlySpan<char>)"#5ca8ff", (Color?)null), 
			CivGlobalMapMarkerType.Enemy => Color.FromHex((ReadOnlySpan<char>)"#ff6d3f", (Color?)null), 
			CivGlobalMapMarkerType.Help => Color.FromHex((ReadOnlySpan<char>)"#ffd85a", (Color?)null), 
			CivGlobalMapMarkerType.Allies => Color.FromHex((ReadOnlySpan<char>)"#6de685", (Color?)null), 
			_ => Color.White, 
		});
	}

	public static Color GetPlayerColor(int viewerTeamId, int viewerSquadId, int playerTeamId, int playerSquadId)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (viewerTeamId == 0)
		{
			if (playerTeamId != 1)
			{
				return EnemyTeamColor;
			}
			return TeamColor;
		}
		if (playerTeamId != viewerTeamId)
		{
			return EnemyTeamColor;
		}
		if (viewerSquadId != 0 && playerSquadId == viewerSquadId)
		{
			return SquadColor;
		}
		return TeamColor;
	}

	public static string GetShortText(CivGlobalMapMarkerType type)
	{
		return type switch
		{
			CivGlobalMapMarkerType.Attack => "A", 
			CivGlobalMapMarkerType.Defense => "D", 
			CivGlobalMapMarkerType.Enemy => "E", 
			CivGlobalMapMarkerType.Help => "H", 
			CivGlobalMapMarkerType.Allies => "F", 
			_ => "?", 
		};
	}
}
