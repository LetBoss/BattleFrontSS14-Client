// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.EntitySystems.SolutionContainerRelayEvent`1
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Chemistry.EntitySystems;

[ByRefEvent]
public record struct SolutionContainerRelayEvent<TEvent>(
  TEvent Event,
  Entity<SolutionComponent> SolutionEnt,
  string Name)
{
  public readonly Entity<SolutionComponent> SolutionEnt = SolutionEnt;
  public readonly string Name = Name;
  public TEvent Event = Event;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return (EqualityComparer<Entity<SolutionComponent>>.Default.GetHashCode(this.SolutionEnt) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Name)) * -1521134295 + EqualityComparer<TEvent>.Default.GetHashCode(this.Event);
  }

  [CompilerGenerated]
  public readonly bool Equals(SolutionContainerRelayEvent<
  #nullable disable
  TEvent> other)
  {
    return EqualityComparer<Entity<SolutionComponent>>.Default.Equals(this.SolutionEnt, other.SolutionEnt) && EqualityComparer<string>.Default.Equals(this.Name, other.Name) && EqualityComparer<TEvent>.Default.Equals(this.Event, other.Event);
  }

  [CompilerGenerated]
  public readonly void Deconstruct(
    out 
    #nullable enable
    TEvent Event,
    out Entity<SolutionComponent> SolutionEnt,
    out string Name)
  {
    Event = this.Event;
    SolutionEnt = this.SolutionEnt;
    Name = this.Name;
  }
}
