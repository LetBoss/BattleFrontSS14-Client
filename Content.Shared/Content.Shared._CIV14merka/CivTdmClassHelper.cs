using Content.Shared._CIV14merka.Teams;
using Robust.Shared.Localization;

namespace Content.Shared._CIV14merka;

public static class CivTdmClassHelper
{
	public static string GetDisplayName(CivTdmClass classType)
	{
		return classType switch
		{
			CivTdmClass.SquadLeader => Loc.GetString("civ-eq-class-squad-leader"), 
			CivTdmClass.Specialist => Loc.GetString("civ-eq-class-specialist"), 
			CivTdmClass.Medic => Loc.GetString("civ-eq-class-medic"), 
			CivTdmClass.MachineGunner => Loc.GetString("civ-eq-class-machine-gunner"), 
			CivTdmClass.Rifleman => Loc.GetString("civ-eq-class-rifleman"), 
			CivTdmClass.Engineer => Loc.GetString("civ-eq-class-engineer"), 
			CivTdmClass.EngineerLeader => Loc.GetString("civ-eq-class-engineer-leader"), 
			CivTdmClass.MedicLeader => Loc.GetString("civ-eq-class-medic-leader"), 
			CivTdmClass.Logist => Loc.GetString("civ-eq-class-logist"), 
			CivTdmClass.Scout => Loc.GetString("civ-eq-class-scout"), 
			_ => Loc.GetString("civ-eq-class-unknown"), 
		};
	}

	public static string GetShortTag(CivTdmClass classType)
	{
		return classType switch
		{
			CivTdmClass.SquadLeader => "SL", 
			CivTdmClass.Specialist => "SP", 
			CivTdmClass.Medic => "M", 
			CivTdmClass.MachineGunner => "MG", 
			CivTdmClass.Rifleman => "R", 
			CivTdmClass.Engineer => "ENG", 
			CivTdmClass.EngineerLeader => "ENL", 
			CivTdmClass.MedicLeader => "ML", 
			CivTdmClass.Logist => "LOG", 
			CivTdmClass.Scout => "SC", 
			_ => "?", 
		};
	}
}
