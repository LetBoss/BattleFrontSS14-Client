// Decompiled with JetBrains decompiler
// Type: Content.Shared.Crayon.CrayonComponentState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Crayon;

[NetSerializable]
[Serializable]
public sealed class CrayonComponentState : ComponentState
{
  public readonly Color Color;
  public readonly string State;
  public readonly int Charges;
  public readonly int Capacity;

  public CrayonComponentState(Color color, string state, int charges, int capacity)
  {
    this.Color = color;
    this.State = state;
    this.Charges = charges;
    this.Capacity = capacity;
  }
}
