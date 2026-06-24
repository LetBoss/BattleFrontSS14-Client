// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mobs.Events.BeforeAlertSeverityCheckEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Mobs.Events;

[NetSerializable]
[Serializable]
public sealed class BeforeAlertSeverityCheckEvent(
  ProtoId<AlertPrototype> currentAlert,
  short severity) : EntityEventArgs
{
  public bool CancelUpdate;
  public ProtoId<AlertPrototype> CurrentAlert = currentAlert;
  public short Severity = severity;
}
