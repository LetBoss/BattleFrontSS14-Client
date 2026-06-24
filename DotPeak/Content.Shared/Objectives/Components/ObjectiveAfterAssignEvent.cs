// Decompiled with JetBrains decompiler
// Type: Content.Shared.Objectives.Components.ObjectiveAfterAssignEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Mind;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Objectives.Components;

[ByRefEvent]
public record struct ObjectiveAfterAssignEvent(
  EntityUid MindId,
  MindComponent Mind,
  ObjectiveComponent Objective,
  MetaDataComponent Meta)
;
