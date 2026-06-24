// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.EntitySystems.SolutionTransferAttemptEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Chemistry.EntitySystems;

[ByRefEvent]
public record struct SolutionTransferAttemptEvent(
  EntityUid From,
  Entity<SolutionComponent> FromSolution,
  EntityUid To,
  Entity<SolutionComponent> ToSolution,
  string? CancelReason = null)
{
  public void Cancel(string reason) => this.CancelReason = reason;
}
