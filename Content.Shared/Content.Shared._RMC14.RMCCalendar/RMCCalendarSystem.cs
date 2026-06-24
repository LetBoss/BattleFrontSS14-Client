using System;
using System.Linq;
using Content.Shared._RMC14.RMCCustomHoliday;
using Content.Shared.Clock;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.RMCCalendar;

public sealed class RMCCalendarSystem : EntitySystem
{
	[Dependency]
	private RMCCustomHolidaySystem _customHolidaySystem;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCCalendarComponent, ExaminedEvent>((EntityEventRefHandler<RMCCalendarComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	private void OnExamined(Entity<RMCCalendarComponent> ent, ref ExaminedEvent args)
	{
		DateTime worldDate = ((EntitySystem)this).EntityQuery<GlobalTimeManagerComponent>(false).FirstOrDefault()?.DateOffset ?? DateTime.Today.AddYears(100);
		string time = worldDate.ToString("dd MMMM, yyyy");
		foreach (CustomHolidayPrototype holiday in (from h in _customHolidaySystem.GetCustomHolidays()
			where h.BeginDay == worldDate.Day && h.BeginMonth.Equals(worldDate.ToString("MMMM"), StringComparison.OrdinalIgnoreCase)
			select h).ToList())
		{
			args.PushMarkup(base.Loc.GetString("rmc-calendar-holiday-examine", (ValueTuple<string, object>)("holidayname", holiday.Name), (ValueTuple<string, object>)("holidaydescription", holiday.Description)));
		}
		args.PushMarkup(base.Loc.GetString("rmc-calendar-examine", (ValueTuple<string, object>)("time", time)));
	}
}
