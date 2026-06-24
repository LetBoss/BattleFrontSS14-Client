// Decompiled with JetBrains decompiler
// Type: Content.Shared.Doors.Electronics.DoorElectronicsConfigurationState
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
namespace Content.Shared.Doors.Electronics;

[NetSerializable]
[Serializable]
public sealed class DoorElectronicsConfigurationState : BoundUserInterfaceState
{
  public List<ProtoId<AccessLevelPrototype>> AccessList;

  public DoorElectronicsConfigurationState(List<ProtoId<AccessLevelPrototype>> accessList)
  {
    this.AccessList = accessList;
  }
}
