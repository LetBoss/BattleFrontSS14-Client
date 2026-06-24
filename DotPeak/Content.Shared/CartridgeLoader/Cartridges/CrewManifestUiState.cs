// Decompiled with JetBrains decompiler
// Type: Content.Shared.CartridgeLoader.Cartridges.CrewManifestUiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.CrewManifest;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.CartridgeLoader.Cartridges;

[NetSerializable]
[Serializable]
public sealed class CrewManifestUiState : BoundUserInterfaceState
{
  public string StationName;
  public CrewManifestEntries? Entries;

  public CrewManifestUiState(string stationName, CrewManifestEntries? entries)
  {
    this.StationName = stationName;
    this.Entries = entries;
  }
}
