using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Holiday;

public abstract class SharedRMCHolidaySystem : EntitySystem
{
	public List<string> GetActiveHolidays()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		RMCHolidayTrackerComponent holidayTracker = default(RMCHolidayTrackerComponent);
		if (((EntitySystem)this).EntityQueryEnumerator<RMCHolidayTrackerComponent>().MoveNext(ref holidayTracker))
		{
			return holidayTracker.ActiveHolidays;
		}
		return new List<string>();
	}

	public bool IsActiveHoliday(string holidayName)
	{
		return GetActiveHolidays().Contains(holidayName);
	}
}
