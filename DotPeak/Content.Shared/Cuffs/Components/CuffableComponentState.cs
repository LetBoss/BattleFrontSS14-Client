// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cuffs.Components.CuffableComponentState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Cuffs.Components;

[NetSerializable]
[Serializable]
public sealed class CuffableComponentState : ComponentState
{
  public readonly bool CanStillInteract;
  public readonly int NumHandsCuffed;
  public readonly string? RSI;
  public readonly string? IconState;
  public readonly Robust.Shared.Maths.Color? Color;

  public CuffableComponentState(
    int numHandsCuffed,
    bool canStillInteract,
    string? rsiPath,
    string? iconState,
    Robust.Shared.Maths.Color? color)
  {
    this.NumHandsCuffed = numHandsCuffed;
    this.CanStillInteract = canStillInteract;
    this.RSI = rsiPath;
    this.IconState = iconState;
    this.Color = color;
  }
}
