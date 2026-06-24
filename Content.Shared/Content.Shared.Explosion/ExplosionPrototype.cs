using System;
using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Explosion;

[Prototype(null, 1)]
public sealed class ExplosionPrototype : IPrototype
{
	[DataField("damagePerIntensity", false, 1, true, false, null)]
	public DamageSpecifier DamagePerIntensity;

	[DataField(null, false, 1, false, false, null)]
	public float? FireStacks;

	[DataField("tileBreakChance", false, 1, false, false, null)]
	public float[] _tileBreakChance = new float[2] { 0f, 1f };

	[DataField("tileBreakIntensity", false, 1, false, false, null)]
	public float[] _tileBreakIntensity = new float[2] { 0f, 15f };

	[DataField("tileBreakRerollReduction", false, 1, false, false, null)]
	public float TileBreakRerollReduction = 10f;

	[DataField("lightColor", false, 1, false, false, null)]
	public Color LightColor = Color.Orange;

	[DataField("fireColor", false, 1, false, false, null)]
	public Color? FireColor;

	[DataField("smallSoundIterationThreshold", false, 1, false, false, null)]
	public int SmallSoundIterationThreshold = 6;

	[DataField(null, false, 1, false, false, null)]
	public float MaxCombineDistance = 1f;

	[DataField("sound", false, 1, false, false, null)]
	public SoundSpecifier Sound = (SoundSpecifier)new SoundCollectionSpecifier("Explosion", (AudioParams?)null);

	[DataField("smallSound", false, 1, false, false, null)]
	public SoundSpecifier SmallSound = (SoundSpecifier)new SoundCollectionSpecifier("ExplosionSmall", (AudioParams?)null);

	[DataField("soundFar", false, 1, false, false, null)]
	public SoundSpecifier SoundFar = (SoundSpecifier)new SoundCollectionSpecifier("ExplosionFar", (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVolume(2f));

	[DataField("smallSoundFar", false, 1, false, false, null)]
	public SoundSpecifier SmallSoundFar = (SoundSpecifier)new SoundCollectionSpecifier("ExplosionSmallFar", (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVolume(2f));

	[DataField("texturePath", false, 1, false, false, null)]
	public ResPath TexturePath = new ResPath("/Textures/Effects/fire.rsi");

	[DataField("intensityPerState", false, 1, false, false, null)]
	public float IntensityPerState = 12f;

	[DataField("fireStates", false, 1, false, false, null)]
	public int FireStates = 3;

	[IdDataField(1, null)]
	public string ID { get; private set; }

	public float TileBreakChance(float intensity)
	{
		if (intensity >= _tileBreakIntensity[^1] || _tileBreakIntensity.Length == 1)
		{
			return _tileBreakChance[^1];
		}
		if (intensity <= _tileBreakIntensity[0])
		{
			return _tileBreakChance[0];
		}
		int i = Array.FindIndex(_tileBreakIntensity, (float k) => k >= intensity);
		float slope = (_tileBreakChance[i] - _tileBreakChance[i - 1]) / (_tileBreakIntensity[i] - _tileBreakIntensity[i - 1]);
		return _tileBreakChance[i - 1] + slope * (intensity - _tileBreakIntensity[i - 1]);
	}
}
