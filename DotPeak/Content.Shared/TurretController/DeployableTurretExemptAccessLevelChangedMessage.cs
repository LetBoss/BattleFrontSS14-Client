// Decompiled with JetBrains decompiler
// Type: Content.Shared.TurretController.DeployableTurretExemptAccessLevelChangedMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.TurretController;

[NetSerializable]
[Serializable]
public sealed class DeployableTurretExemptAccessLevelChangedMessage : BoundUserInterfaceMessage
{
  public HashSet<ProtoId<AccessLevelPrototype>> AccessLevels;
  public bool Enabled;

  public DeployableTurretExemptAccessLevelChangedMessage(
    HashSet<ProtoId<AccessLevelPrototype>> accessLevels,
    bool enabled)
  {
    this.AccessLevels = accessLevels;
    this.Enabled = enabled;
  }
}
