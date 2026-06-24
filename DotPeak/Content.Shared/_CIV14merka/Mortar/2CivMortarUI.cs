// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Mortar.CivMortarBuiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._CIV14merka.Mortar;

[NetSerializable]
[Serializable]
public sealed class CivMortarBuiState(
  Vector2i target,
  Vector2i dial,
  List<CivMortarQueuedRequest> requests) : BoundUserInterfaceState
{
  public Vector2i Target = target;
  public Vector2i Dial = dial;
  public List<CivMortarQueuedRequest> Requests = requests;
}
