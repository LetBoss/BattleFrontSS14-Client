// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.EntitySystems.AttemptAddFizzinessEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Nutrition.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Nutrition.EntitySystems;

[ByRefEvent]
public record struct AttemptAddFizzinessEvent(
  Robust.Shared.GameObjects.Entity<PressurizedSolutionComponent> Entity,
  float Amount)
{
  public bool Cancelled = false;

  public Robust.Shared.GameObjects.Entity<PressurizedSolutionComponent> Entity { get; set; } = Entity;

  public float Amount { get; set; } = Amount;
}
