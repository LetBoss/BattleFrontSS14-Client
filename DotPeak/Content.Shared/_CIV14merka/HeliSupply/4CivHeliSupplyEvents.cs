// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.HeliSupply.CivHeliStateMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._CIV14merka.HeliSupply;

[NetSerializable]
[Serializable]
public sealed class CivHeliStateMessage : EntityEventArgs
{
  public string SideId = string.Empty;
  public List<CivHeliStockSection> Sections = new List<CivHeliStockSection>();
  public List<CivHeliCargoEntry> Cargo = new List<CivHeliCargoEntry>();
  public int CargoCount;
  public int MaxCargo;
  public int MaxWaypoints;
  public bool HasOrigin;
  public Vector2 Origin;
  public MapId OriginMapId;
}
