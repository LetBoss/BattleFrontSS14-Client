// Decompiled with JetBrains decompiler
// Type: Content.Shared.Alert.AlertState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Alert;

[NetSerializable]
[Serializable]
public struct AlertState
{
  public short? Severity;
  public (TimeSpan, TimeSpan)? Cooldown;
  public string? DynamicMessage;
  public bool AutoRemove;
  public bool ShowCooldown;
  public ProtoId<AlertPrototype> Type;
}
