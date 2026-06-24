// Decompiled with JetBrains decompiler
// Type: Content.Shared.Climbing.Events.BeforeClimbEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Climbing.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Climbing.Events;

public abstract class BeforeClimbEvent : CancellableEntityEventArgs
{
  public readonly EntityUid GettingPutOnTable;
  public readonly EntityUid PuttingOnTable;
  public readonly Entity<ClimbableComponent> BeingClimbedOn;

  public BeforeClimbEvent(
    EntityUid gettingPutOntable,
    EntityUid puttingOnTable,
    Entity<ClimbableComponent> beingClimbedOn)
  {
    this.GettingPutOnTable = gettingPutOntable;
    this.PuttingOnTable = puttingOnTable;
    this.BeingClimbedOn = beingClimbedOn;
  }
}
