using System;
using System.Linq;
using Content.Shared.Examine;
using Content.Shared.GameTicking;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Clock;

public abstract class SharedClockSystem : EntitySystem
{
	[Dependency]
	private SharedGameTicker _ticker;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<ClockComponent, ExaminedEvent>((EntityEventRefHandler<ClockComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	private void OnExamined(Entity<ClockComponent> ent, ref ExaminedEvent args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (args.IsInDetailsRange)
		{
			args.PushMarkup(base.Loc.GetString("clock-examine", (ValueTuple<string, object>)("time", GetClockTimeText(ent))));
		}
	}

	public string GetClockTimeText(Entity<ClockComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = GetClockTime(ent);
		return ent.Comp.ClockType switch
		{
			ClockType.TwelveHour => time.ToString("h\\:mm"), 
			ClockType.TwentyFourHour => time.ToString("hh\\:mm"), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private TimeSpan GetGlobalTime()
	{
		return (((EntitySystem)this).EntityQuery<GlobalTimeManagerComponent>(false).FirstOrDefault()?.TimeOffset ?? TimeSpan.Zero) + _ticker.RoundDuration();
	}

	public TimeSpan GetClockTime(Entity<ClockComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		ClockComponent comp = ent.Comp;
		if (comp.StuckTime.HasValue)
		{
			return comp.StuckTime.Value;
		}
		TimeSpan time = GetGlobalTime();
		switch (comp.ClockType)
		{
		case ClockType.TwelveHour:
		{
			int adjustedHours = time.Hours % 12;
			if (adjustedHours == 0)
			{
				adjustedHours = 12;
			}
			return new TimeSpan(adjustedHours, time.Minutes, time.Seconds);
		}
		case ClockType.TwentyFourHour:
			return time;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}
}
