// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Skills.AttemptHyposprayUseEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System;

#nullable disable
namespace Content.Shared._RMC14.Marines.Skills;

[ByRefEvent]
public record struct AttemptHyposprayUseEvent(EntityUid User, EntityUid Target, TimeSpan DoAfter)
{
  public void MaxDoAfter(TimeSpan b)
  {
    if (!(this.DoAfter < b))
      return;
    this.DoAfter = b;
  }
}
