// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.RMCCalendar.RMCCalendarSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.RMCCustomHoliday;
using Content.Shared.Clock;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.RMCCalendar;

public sealed class RMCCalendarSystem : EntitySystem
{
  [Dependency]
  private RMCCustomHolidaySystem _customHolidaySystem;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCCalendarComponent, ExaminedEvent>(new EntityEventRefHandler<RMCCalendarComponent, ExaminedEvent>(this.OnExamined));
  }

  private void OnExamined(Entity<RMCCalendarComponent> ent, ref ExaminedEvent args)
  {
    GlobalTimeManagerComponent managerComponent = this.EntityQuery<GlobalTimeManagerComponent>().FirstOrDefault<GlobalTimeManagerComponent>();
    DateTime worldDate = managerComponent != null ? managerComponent.DateOffset : DateTime.Today.AddYears(100);
    string str = worldDate.ToString("dd MMMM, yyyy");
    foreach (CustomHolidayPrototype holidayPrototype in this._customHolidaySystem.GetCustomHolidays().Where<CustomHolidayPrototype>((Func<CustomHolidayPrototype, bool>) (h => h.BeginDay == worldDate.Day && h.BeginMonth.Equals(worldDate.ToString("MMMM"), StringComparison.OrdinalIgnoreCase))).ToList<CustomHolidayPrototype>())
      args.PushMarkup(this.Loc.GetString("rmc-calendar-holiday-examine", ("holidayname", (object) holidayPrototype.Name), ("holidaydescription", (object) holidayPrototype.Description)));
    args.PushMarkup(this.Loc.GetString("rmc-calendar-examine", ("time", (object) str)));
  }
}
