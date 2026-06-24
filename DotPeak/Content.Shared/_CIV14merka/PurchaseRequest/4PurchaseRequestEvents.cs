// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.PurchaseRequest.PurchaseConsoleBuiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._CIV14merka.PurchaseRequest;

[NetSerializable]
[Serializable]
public sealed class PurchaseConsoleBuiState : BoundUserInterfaceState
{
  public Dictionary<string, List<CatalogItem>> Catalog { get; init; } = new Dictionary<string, List<CatalogItem>>();

  public List<Content.Shared._CIV14merka.PurchaseRequest.PurchaseRequest> PendingRequests { get; init; } = new List<Content.Shared._CIV14merka.PurchaseRequest.PurchaseRequest>();

  public bool IsLeader { get; init; }

  public int AvailablePoints { get; init; }
}
