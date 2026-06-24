// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XenoArtifactActivatedEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Xenoarchaeology.Artifact.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact;

[ByRefEvent]
public readonly record struct XenoArtifactActivatedEvent(
  Entity<XenoArtifactComponent> Artifact,
  EntityUid? User,
  EntityUid? Target,
  EntityCoordinates Coordinates)
;
