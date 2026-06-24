// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.GetEquipmentVisualsEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Clothing;

public sealed class GetEquipmentVisualsEvent : EntityEventArgs
{
  public readonly EntityUid Equipee;
  public readonly string Slot;
  public List<(string, PrototypeLayerData)> Layers = new List<(string, PrototypeLayerData)>();

  public GetEquipmentVisualsEvent(EntityUid equipee, string slot)
  {
    this.Equipee = equipee;
    this.Slot = slot;
  }
}
