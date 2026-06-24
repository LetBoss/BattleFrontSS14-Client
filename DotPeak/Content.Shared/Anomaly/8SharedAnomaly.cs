// Decompiled with JetBrains decompiler
// Type: Content.Shared.Anomaly.AnomalyScannerUserInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.Anomaly;

[NetSerializable]
[Serializable]
public sealed class AnomalyScannerUserInterfaceState : BoundUserInterfaceState
{
  public FormattedMessage Message;
  public TimeSpan? NextPulseTime;

  public AnomalyScannerUserInterfaceState(FormattedMessage message, TimeSpan? nextPulseTime)
  {
    this.Message = message;
    this.NextPulseTime = nextPulseTime;
  }
}
