// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.TacticalMap.TacticalMapUpdateCanvasMsg
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.TacticalMap;

[NetSerializable]
[Serializable]
public sealed class TacticalMapUpdateCanvasMsg(
  List<TacticalMapLine> lines,
  Dictionary<Vector2i, string> labels) : BoundUserInterfaceMessage
{
  public readonly List<TacticalMapLine> Lines = lines;
  public readonly Dictionary<Vector2i, string> Labels = labels;
}
