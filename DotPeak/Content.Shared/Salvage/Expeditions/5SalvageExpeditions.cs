// Decompiled with JetBrains decompiler
// Type: Content.Shared.Salvage.Expeditions.SalvageMission
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Salvage.Expeditions;

public sealed record SalvageMission(
  int Seed,
  string Dungeon,
  string Faction,
  string Biome,
  string Air,
  float Temperature,
  Robust.Shared.Maths.Color? Color,
  TimeSpan Duration,
  List<string> Modifiers)
{
  public readonly int Seed = Seed;
  public readonly string Dungeon = Dungeon;
  public readonly string Faction = Faction;
  public readonly string Biome = Biome;
  public readonly string Air = Air;
  public readonly float Temperature = Temperature;
  public readonly Robust.Shared.Maths.Color? Color = Color;
  public TimeSpan Duration = Duration;
  public List<string> Modifiers = Modifiers;

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return ((((((((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.Seed)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Dungeon)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Faction)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Biome)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Air)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.Temperature)) * -1521134295 + EqualityComparer<Robust.Shared.Maths.Color?>.Default.GetHashCode(this.Color)) * -1521134295 + EqualityComparer<TimeSpan>.Default.GetHashCode(this.Duration)) * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(this.Modifiers);
  }

  [CompilerGenerated]
  public bool Equals(SalvageMission? other)
  {
    if ((object) this == (object) other)
      return true;
    return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<int>.Default.Equals(this.Seed, other.Seed) && EqualityComparer<string>.Default.Equals(this.Dungeon, other.Dungeon) && EqualityComparer<string>.Default.Equals(this.Faction, other.Faction) && EqualityComparer<string>.Default.Equals(this.Biome, other.Biome) && EqualityComparer<string>.Default.Equals(this.Air, other.Air) && EqualityComparer<float>.Default.Equals(this.Temperature, other.Temperature) && EqualityComparer<Robust.Shared.Maths.Color?>.Default.Equals(this.Color, other.Color) && EqualityComparer<TimeSpan>.Default.Equals(this.Duration, other.Duration) && EqualityComparer<List<string>>.Default.Equals(this.Modifiers, other.Modifiers);
  }

  [CompilerGenerated]
  public void Deconstruct(
    out int Seed,
    out string Dungeon,
    out string Faction,
    out string Biome,
    out string Air,
    out float Temperature,
    out Robust.Shared.Maths.Color? Color,
    out TimeSpan Duration,
    out List<string> Modifiers)
  {
    Seed = this.Seed;
    Dungeon = this.Dungeon;
    Faction = this.Faction;
    Biome = this.Biome;
    Air = this.Air;
    Temperature = this.Temperature;
    Color = this.Color;
    Duration = this.Duration;
    Modifiers = this.Modifiers;
  }
}
