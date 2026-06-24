// Decompiled with JetBrains decompiler
// Type: Content.Shared.Climbing.Events.TargetBeforeClimbEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Climbing.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Climbing.Events;

public sealed class TargetBeforeClimbEvent(
  EntityUid gettingPutOntable,
  EntityUid puttingOnTable,
  Entity<ClimbableComponent> beingClimbedOn) : BeforeClimbEvent(gettingPutOntable, puttingOnTable, beingClimbedOn)
{
}
