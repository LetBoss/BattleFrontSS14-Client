using System;
using Robust.Shared.Audio;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Shared.Audio;

public static class AudioHelpers
{
	private static readonly float[] SemitoneMultipliers = new float[25]
	{
		0.5f,
		0.5297273f,
		0.56122726f,
		0.5946137f,
		0.6299545f,
		0.6674091f,
		0.7071136f,
		0.7491591f,
		0.79370457f,
		0.84088635f,
		49f / 55f,
		0.94386363f,
		1f,
		1.0594546f,
		1.1224545f,
		1.1892046f,
		1.2599318f,
		1.3348409f,
		1.4142046f,
		1.4983182f,
		1.5874091f,
		1.6817955f,
		1.7817954f,
		1.8877499f,
		2f
	};

	[Obsolete("Use AudioParams.Variation data-field")]
	public static AudioParams WithVariation(float amplitude)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return WithVariation(amplitude, null);
	}

	[Obsolete("Use AudioParams.Variation data-field")]
	public static AudioParams WithVariation(float amplitude, IRobustRandom? rand)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.Resolve<IRobustRandom>(ref rand);
		float scale = (float)RandomExtensions.NextGaussian(rand, 1.0, (double)amplitude);
		return ((AudioParams)(ref AudioParams.Default)).WithPitchScale(scale);
	}

	public static AudioParams ShiftSemitone(AudioParams @params, int shift)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		shift = MathHelper.Clamp(shift, -12, 12);
		float pitchMult = SemitoneMultipliers[shift + 12];
		return ((AudioParams)(ref @params)).WithPitchScale(pitchMult);
	}

	public static AudioParams WithSemitoneVariation(AudioParams @params, int variation, IRobustRandom rand)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.Resolve<IRobustRandom>(ref rand);
		variation = Math.Clamp(variation, 0, 12);
		return ShiftSemitone(@params, rand.Next(-variation, variation));
	}
}
