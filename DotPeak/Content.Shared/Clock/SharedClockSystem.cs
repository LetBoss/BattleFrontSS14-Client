// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clock.SharedClockSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Content.Shared.GameTicking;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Linq;

#nullable enable
namespace Content.Shared.Clock;

public abstract class SharedClockSystem : EntitySystem
{
  [Dependency]
  private SharedGameTicker _ticker;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ClockComponent, ExaminedEvent>(new EntityEventRefHandler<ClockComponent, ExaminedEvent>((object) this, __methodptr(OnExamined)), (Type[]) null, (Type[]) null);
  }

  private void OnExamined(Entity<ClockComponent> ent, ref ExaminedEvent args)
  {
    if (!args.IsInDetailsRange)
      return;
    args.PushMarkup(this.Loc.GetString("clock-examine", ("time", (object) this.GetClockTimeText(ent))));
  }

  public string GetClockTimeText(Entity<ClockComponent> ent)
  {
    TimeSpan clockTime = this.GetClockTime(ent);
    switch (ent.Comp.ClockType)
    {
      case ClockType.TwelveHour:
        return clockTime.ToString("h\\:mm");
      case ClockType.TwentyFourHour:
        return clockTime.ToString("hh\\:mm");
      default:
        throw new ArgumentOutOfRangeException();
    }
  }

  private TimeSpan GetGlobalTime()
  {
    GlobalTimeManagerComponent managerComponent = this.EntityQuery<GlobalTimeManagerComponent>(false).FirstOrDefault<GlobalTimeManagerComponent>();
    return (managerComponent != null ? managerComponent.TimeOffset : TimeSpan.Zero) + this._ticker.RoundDuration();
  }

  public TimeSpan GetClockTime(Entity<ClockComponent> ent)
  {
    ClockComponent comp = ent.Comp;
    if (comp.StuckTime.HasValue)
      return comp.StuckTime.Value;
    TimeSpan globalTime = this.GetGlobalTime();
    switch (comp.ClockType)
    {
      case ClockType.TwelveHour:
        int hours = globalTime.Hours % 12;
        if (hours == 0)
          hours = 12;
        return new TimeSpan(hours, globalTime.Minutes, globalTime.Seconds);
      case ClockType.TwentyFourHour:
        return globalTime;
      default:
        throw new ArgumentOutOfRangeException();
    }
  }
}
