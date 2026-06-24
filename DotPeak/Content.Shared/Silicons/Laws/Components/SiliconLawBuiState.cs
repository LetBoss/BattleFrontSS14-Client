// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.Laws.Components.SiliconLawBuiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Silicons.Laws.Components;

[NetSerializable]
[Serializable]
public sealed class SiliconLawBuiState : BoundUserInterfaceState
{
  public List<SiliconLaw> Laws;
  public HashSet<string>? RadioChannels;

  public SiliconLawBuiState(List<SiliconLaw> laws, HashSet<string>? radioChannels)
  {
    this.Laws = laws;
    this.RadioChannels = radioChannels;
  }
}
