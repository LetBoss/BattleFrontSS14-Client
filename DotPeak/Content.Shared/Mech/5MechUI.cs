// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mech.MechGrabberEjectMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.Mech;

[NetSerializable]
[Serializable]
public sealed class MechGrabberEjectMessage : MechEquipmentUiMessage
{
  public NetEntity Item;

  public MechGrabberEjectMessage(NetEntity equipment, NetEntity uid)
  {
    this.Equipment = equipment;
    this.Item = uid;
  }
}
