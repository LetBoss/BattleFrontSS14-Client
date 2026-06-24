using System;
using Content.Shared._CIV14merka.Teams;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client._CIV14merka.Teams;

public static class CivTeamIconResolver
{
	private const int BlueTeamId = 1;

	private const int RedTeamId = 2;

	private const int MaxSquadBadgeId = 20;

	private static readonly ResPath MapBlipsRsi = new ResPath("/Textures/_RMC14/Interface/map_blips.rsi");

	private static readonly ResPath ClassIconsRsi = new ResPath("/Textures/_RMC14/Interface/cm_job_icons.rsi");

	private static readonly ResPath MarineHudRsi = new ResPath("/Textures/_RMC14/Interface/marine_hud.rsi");

	private static readonly Rsi ClassBadgeBackground = new Rsi(ClassIconsRsi, "hudsquad");

	private static readonly Rsi GenericSquadBadge = new Rsi(MarineHudRsi, "hudsquad");

	private static readonly ResPath FlagsRsi = new ResPath("/Textures/_CIV14merka/Interface/team_flags.rsi");

	private static readonly Color DeltaColor = Color.FromHex((ReadOnlySpan<char>)"#4148C8", (Color?)null);

	private static readonly Color AlphaColor = Color.FromHex((ReadOnlySpan<char>)"#E61919", (Color?)null);

	public static Rsi GetClassBadgeBackground()
	{
		return ClassBadgeBackground;
	}

	public static Rsi GetClassBadgeBackground(int teamId)
	{
		return ClassBadgeBackground;
	}

	public static Color GetClassBadgeColor(int teamId)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(teamId switch
		{
			1 => DeltaColor, 
			2 => AlphaColor, 
			_ => Color.White, 
		});
	}

	public static Rsi GetTeamBadge(int teamId)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		return new Rsi(MapBlipsRsi, teamId switch
		{
			1 => "gm_shield", 
			2 => "gm_star", 
			_ => "gm_star", 
		});
	}

	public static Rsi GetGenericSquadBadge()
	{
		return GenericSquadBadge;
	}

	public static Rsi GetClassIcon(CivTdmClass roleClass, int teamId)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		return new Rsi(ClassIconsRsi, roleClass switch
		{
			CivTdmClass.SquadLeader => "hudsquad_leader", 
			CivTdmClass.Specialist => "hudsquad_spec_sniper", 
			CivTdmClass.Medic => "hudsquad_med", 
			CivTdmClass.MachineGunner => "hudsquad_gun", 
			CivTdmClass.Engineer => "hudsquad_engi", 
			CivTdmClass.EngineerLeader => "hudsquad_ce", 
			CivTdmClass.MedicLeader => "hudsquad_cmo", 
			CivTdmClass.Logist => "hudsquad_ct", 
			CivTdmClass.Scout => "cmb_obs", 
			_ => "hudsquad_rifleman", 
		});
	}

	public static Rsi? GetSquadBadge(int squadId)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		if (squadId <= 0 || squadId > 20)
		{
			return null;
		}
		return new Rsi(MarineHudRsi, $"hudsquad_ft{squadId}");
	}

	public static Rsi? GetTeamFlag(int teamId)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		string text = teamId switch
		{
			1 => "flag_usa", 
			2 => "flag_russia", 
			_ => null, 
		};
		if (text == null)
		{
			return null;
		}
		return new Rsi(FlagsRsi, text);
	}
}
