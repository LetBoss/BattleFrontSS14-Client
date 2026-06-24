using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;

namespace Content.Shared.Salvage.Expeditions;

public sealed record SalvageMission(int Seed, string Dungeon, string Faction, string Biome, string Air, float Temperature, Color? Color, TimeSpan Duration, List<string> Modifiers)
{
	public readonly int Seed = Seed;

	public readonly string Dungeon = Dungeon;

	public readonly string Faction = Faction;

	public readonly string Biome = Biome;

	public readonly string Air = Air;

	public readonly float Temperature = Temperature;

	public readonly Color? Color = Color;

	public TimeSpan Duration = Duration;

	public List<string> Modifiers = Modifiers;

	[CompilerGenerated]
	public void Deconstruct(out int Seed, out string Dungeon, out string Faction, out string Biome, out string Air, out float Temperature, out Color? Color, out TimeSpan Duration, out List<string> Modifiers)
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
