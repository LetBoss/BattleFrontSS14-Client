using System;
using System.Numerics;

namespace Robust.Shared.Noise;

[Obsolete("Use FastNoiseLite")]
public sealed class NoiseGenerator
{
	public enum NoiseType : byte
	{
		Fbm,
		Ridged
	}

	private readonly FastNoise _fastNoiseInstance;

	private float _periodX;

	private float _periodY;

	public NoiseGenerator(NoiseType type)
	{
		_fastNoiseInstance = new FastNoise();
		_fastNoiseInstance.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
		_fastNoiseInstance.SetFractalLacunarity((float)Math.PI * 2f / 3f);
		switch (type)
		{
		case NoiseType.Fbm:
			_fastNoiseInstance.SetFractalType(FastNoise.FractalType.FBM);
			break;
		case NoiseType.Ridged:
			_fastNoiseInstance.SetFractalType(FastNoise.FractalType.RigidMulti);
			break;
		default:
			throw new ArgumentOutOfRangeException("type", type, null);
		}
	}

	public void SetFrequency(float frequency)
	{
		_fastNoiseInstance.SetFrequency(frequency);
	}

	public void SetLacunarity(float lacunarity)
	{
		_fastNoiseInstance.SetFractalLacunarity(lacunarity);
	}

	public void SetPersistence(float persistence)
	{
		_fastNoiseInstance.SetFractalGain(persistence);
	}

	public void SetPeriodX(float periodX)
	{
		_periodX = periodX;
	}

	public void SetPeriodY(float periodY)
	{
		_periodY = periodY;
	}

	public void SetOctaves(uint octaves)
	{
		_fastNoiseInstance.SetFractalOctaves((int)octaves);
	}

	public void SetSeed(uint seed)
	{
		_fastNoiseInstance.SetSeed((int)seed);
	}

	public float GetNoiseTiled(float x, float y)
	{
		return GetNoiseTiled(new Vector2(x, y));
	}

	public float GetNoiseTiled(Vector2 vec)
	{
		float num = vec.X / _periodX;
		float num2 = vec.Y / _periodY;
		float x = 0f + (float)Math.Cos(num * ((float)Math.PI * 2f)) * (1f / (2f * (float)Math.PI));
		float y = 0f + (float)Math.Cos(num2 * ((float)Math.PI * 2f)) * (1f / (2f * (float)Math.PI));
		float z = 0f + (float)Math.Sin(num * ((float)Math.PI * 2f)) * (1f / (2f * (float)Math.PI));
		float w = 0f + (float)Math.Sin(num2 * ((float)Math.PI * 2f)) * (1f / (2f * (float)Math.PI));
		return GetNoise(x, y, z, w);
	}

	public float GetNoise(float x)
	{
		return GetNoise(new Vector2(x, 0f));
	}

	public float GetNoise(float x, float y)
	{
		return _fastNoiseInstance.GetSimplexFractal(x, y);
	}

	public float GetNoise(Vector2 vector)
	{
		return GetNoise(vector.X, vector.Y);
	}

	public float GetNoise(float x, float y, float z)
	{
		return _fastNoiseInstance.GetSimplexFractal(x, y, z);
	}

	public float GetNoise(Vector3 vector)
	{
		return GetNoise(vector.X, vector.Y, vector.Z);
	}

	public float GetNoise(float x, float y, float z, float w)
	{
		return _fastNoiseInstance.GetSimplex(x, y, z, w);
	}

	public float GetNoise(Vector4 vector)
	{
		return GetNoise(vector.X, vector.Y, vector.Z, vector.W);
	}
}
