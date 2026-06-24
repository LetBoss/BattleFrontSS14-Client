// Decompiled with JetBrains decompiler
// Type: Content.Shared.Singularity.Events.SingularityLevelChangedEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Singularity.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Singularity.Events;

public sealed class SingularityLevelChangedEvent : EntityEventArgs
{
  public readonly byte NewValue;
  public readonly byte OldValue;
  public readonly SingularityComponent Singularity;

  public SingularityLevelChangedEvent(
    byte newValue,
    byte oldValue,
    SingularityComponent singularity)
  {
    this.NewValue = newValue;
    this.OldValue = oldValue;
    this.Singularity = singularity;
  }
}
