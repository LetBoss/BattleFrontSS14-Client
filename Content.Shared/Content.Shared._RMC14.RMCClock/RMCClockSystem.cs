using System;
using System.Linq;
using Content.Shared.Clock;
using Content.Shared.Examine;
using Content.Shared.GameTicking;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.RMCClock;

public sealed class RMCClockSystem : EntitySystem
{
	[Dependency]
	private SharedGameTicker _ticker;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCClockComponent, ExaminedEvent>((EntityEventRefHandler<RMCClockComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	private void OnExamined(Entity<RMCClockComponent> ent, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		EntityUid owner = ent.Owner;
		TimeSpan worldTime = (((EntitySystem)this).EntityQuery<GlobalTimeManagerComponent>(false).FirstOrDefault()?.TimeOffset ?? TimeSpan.Zero) + _ticker.RoundDuration();
		string time = ((((EntitySystem)this).EntityQuery<GlobalTimeManagerComponent>(false).FirstOrDefault()?.DateOffset ?? DateTime.Today.AddYears(100)) + worldTime).ToString("dd MMMM, yyyy - HH:mm");
		args.PushMarkup(base.Loc.GetString("rmc-clock-examine", (ValueTuple<string, object>)("device", owner), (ValueTuple<string, object>)("time", time)));
	}
}
