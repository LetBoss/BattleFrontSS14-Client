// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Holiday.SharedRMCHolidaySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Holiday;

public abstract class SharedRMCHolidaySystem : EntitySystem
{
  public List<string> GetActiveHolidays()
  {
    RMCHolidayTrackerComponent comp1;
    return this.EntityQueryEnumerator<RMCHolidayTrackerComponent>().MoveNext(out comp1) ? comp1.ActiveHolidays : new List<string>();
  }

  public bool IsActiveHoliday(string holidayName) => this.GetActiveHolidays().Contains(holidayName);
}
