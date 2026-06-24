// Decompiled with JetBrains decompiler
// Type: Content.Shared.CartridgeLoader.Cartridges.PulledAccessLog
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.CartridgeLoader.Cartridges;

[NetSerializable]
[DataRecord]
[Serializable]
public sealed class PulledAccessLog
{
  public readonly TimeSpan Time;
  public readonly string Accessor;

  public PulledAccessLog(TimeSpan time, string accessor)
  {
    this.Time = time;
    this.Accessor = accessor;
  }
}
