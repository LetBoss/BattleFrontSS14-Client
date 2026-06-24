// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.RMCCustomHoliday.CustomHolidayPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

#nullable enable
namespace Content.Shared._RMC14.RMCCustomHoliday;

[Prototype(null, 1)]
public sealed class CustomHolidayPrototype : IPrototype
{
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public string Name { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public int BeginDay { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public string BeginMonth { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public string Description { get; private set; }
}
