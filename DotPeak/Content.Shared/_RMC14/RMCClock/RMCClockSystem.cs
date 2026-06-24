// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.RMCClock.RMCClockSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clock;
using Content.Shared.Examine;
using Content.Shared.GameTicking;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.RMCClock;

public sealed class RMCClockSystem : EntitySystem
{
  [Dependency]
  private SharedGameTicker _ticker;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCClockComponent, ExaminedEvent>(new EntityEventRefHandler<RMCClockComponent, ExaminedEvent>(this.OnExamined));
  }

  private void OnExamined(Entity<RMCClockComponent> ent, ref ExaminedEvent args)
  {
    EntityUid owner = ent.Owner;
    GlobalTimeManagerComponent managerComponent1 = this.EntityQuery<GlobalTimeManagerComponent>().FirstOrDefault<GlobalTimeManagerComponent>();
    TimeSpan timeSpan = (managerComponent1 != null ? managerComponent1.TimeOffset : TimeSpan.Zero) + this._ticker.RoundDuration();
    GlobalTimeManagerComponent managerComponent2 = this.EntityQuery<GlobalTimeManagerComponent>().FirstOrDefault<GlobalTimeManagerComponent>();
    string str = ((managerComponent2 != null ? managerComponent2.DateOffset : DateTime.Today.AddYears(100)) + timeSpan).ToString("dd MMMM, yyyy - HH:mm");
    args.PushMarkup(this.Loc.GetString("rmc-clock-examine", ("device", (object) owner), ("time", (object) str)));
  }
}
