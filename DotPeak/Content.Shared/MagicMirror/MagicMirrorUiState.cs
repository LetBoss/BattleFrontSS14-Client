// Decompiled with JetBrains decompiler
// Type: Content.Shared.MagicMirror.MagicMirrorUiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Humanoid.Markings;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.MagicMirror;

[NetSerializable]
[Serializable]
public sealed class MagicMirrorUiState : BoundUserInterfaceState
{
  public NetEntity Target;
  public string Species;
  public List<Marking> Hair;
  public int HairSlotTotal;
  public List<Marking> FacialHair;
  public int FacialHairSlotTotal;

  public MagicMirrorUiState(
    string species,
    List<Marking> hair,
    int hairSlotTotal,
    List<Marking> facialHair,
    int facialHairSlotTotal)
  {
    this.Species = species;
    this.Hair = hair;
    this.HairSlotTotal = hairSlotTotal;
    this.FacialHair = facialHair;
    this.FacialHairSlotTotal = facialHairSlotTotal;
  }
}
