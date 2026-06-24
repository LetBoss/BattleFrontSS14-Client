using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Atmos;

[Serializable]
[DataDefinition]
public sealed class GasMixture : IEquatable<GasMixture>, ISerializationHooks, IEnumerable<(Gas gas, float moles)>, IEnumerable, ISerializationGenerated<GasMixture>, ISerializationGenerated
{
	public struct GasEnumerator(GasMixture mixture) : IEnumerator<(Gas gas, float moles)>, IEnumerator, IDisposable
	{
		private int _idx = -1;

		public (Gas gas, float moles) Current => (gas: (Gas)_idx, moles: mixture.Moles[_idx]);

		object? IEnumerator.Current => Current;

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			return ++_idx < 9;
		}

		public void Reset()
		{
			_idx = -1;
		}
	}

	[Access(/*Could not decode attribute arguments.*/)]
	[DataField(null, false, 1, false, false, null)]
	public float[] Moles = new float[12];

	[DataField("temperature", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	private float _temperature = 2.7f;

	[ViewVariables]
	public readonly float[] ReactionResults = new float[1];

	public static GasMixture SpaceGas => new GasMixture
	{
		Volume = 2500f,
		Temperature = 2.7f,
		Immutable = true
	};

	public float this[int gas] => Moles[gas];

	[DataField("immutable", false, 1, false, false, null)]
	public bool Immutable { get; private set; }

	[ViewVariables]
	public float TotalMoles
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return NumericsHelpers.HorizontalAdd((ReadOnlySpan<float>)Moles);
		}
	}

	[ViewVariables]
	public float Pressure
	{
		get
		{
			if (Volume <= 0f)
			{
				return 0f;
			}
			return TotalMoles * 8.314463f * Temperature / Volume;
		}
	}

	[ViewVariables]
	public float Temperature
	{
		get
		{
			return _temperature;
		}
		set
		{
			if (!Immutable)
			{
				_temperature = MathF.Min(MathF.Max(value, 2.7f), 262144f);
			}
		}
	}

	[DataField("volume", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float Volume { get; set; }

	public GasMixture()
	{
	}

	public GasMixture(float volume = 0f)
	{
		if (volume < 0f)
		{
			volume = 0f;
		}
		Volume = volume;
	}

	public GasMixture(float[] moles, float temp, float volume = 2500f)
	{
		if (moles.Length != 12)
		{
			throw new InvalidOperationException("Invalid mole array length");
		}
		if (volume < 0f)
		{
			volume = 0f;
		}
		_temperature = temp;
		Moles = moles;
		Volume = volume;
	}

	public GasMixture(GasMixture toClone)
	{
		CopyFrom(toClone);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void MarkImmutable()
	{
		Immutable = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float GetMoles(int gasId)
	{
		return Moles[gasId];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float GetMoles(Gas gas)
	{
		return GetMoles((int)gas);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetMoles(int gasId, float quantity)
	{
		if (!float.IsFinite(quantity) || float.IsNegative(quantity))
		{
			throw new ArgumentException($"Invalid quantity \"{quantity}\" specified!", "quantity");
		}
		if (!Immutable)
		{
			Moles[gasId] = quantity;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetMoles(Gas gas, float quantity)
	{
		SetMoles((int)gas, quantity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AdjustMoles(int gasId, float quantity)
	{
		if (!Immutable)
		{
			if (!float.IsFinite(quantity))
			{
				throw new ArgumentException($"Invalid quantity \"{quantity}\" specified!", "quantity");
			}
			ref float reference = ref Moles[gasId];
			reference = MathF.Max(reference + quantity, 0f);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AdjustMoles(Gas gas, float moles)
	{
		AdjustMoles((int)gas, moles);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public GasMixture Remove(float amount)
	{
		return RemoveRatio(amount / TotalMoles);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public GasMixture RemoveRatio(float ratio)
	{
		if (!(ratio <= 0f))
		{
			if (ratio > 1f)
			{
				ratio = 1f;
			}
			GasMixture removed = new GasMixture(Volume)
			{
				Temperature = Temperature
			};
			Moles.CopyTo(removed.Moles.AsSpan());
			NumericsHelpers.Multiply((Span<float>)removed.Moles, ratio);
			if (!Immutable)
			{
				NumericsHelpers.Sub((Span<float>)Moles, (ReadOnlySpan<float>)removed.Moles);
			}
			for (int i = 0; i < Moles.Length; i++)
			{
				float moles = Moles[i];
				float otherMoles = removed.Moles[i];
				if ((moles < 5E-08f || float.IsNaN(moles)) && !Immutable)
				{
					Moles[i] = 0f;
				}
				if (otherMoles < 5E-08f || float.IsNaN(otherMoles))
				{
					removed.Moles[i] = 0f;
				}
			}
			return removed;
		}
		return new GasMixture(Volume)
		{
			Temperature = Temperature
		};
	}

	public GasMixture RemoveVolume(float vol)
	{
		return RemoveRatio(vol / Volume);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void CopyFrom(GasMixture sample)
	{
		if (!Immutable)
		{
			Volume = sample.Volume;
			sample.Moles.CopyTo(Moles, 0);
			Temperature = sample.Temperature;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear()
	{
		if (!Immutable)
		{
			Array.Clear(Moles, 0, 9);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Multiply(float multiplier)
	{
		if (!Immutable)
		{
			NumericsHelpers.Multiply((Span<float>)Moles, multiplier);
		}
	}

	void ISerializationHooks.AfterDeserialization()
	{
		Array.Resize(ref Moles, 12);
	}

	public GasMixtureStringRepresentation ToPrettyString()
	{
		Dictionary<string, float> molesPerGas = new Dictionary<string, float>();
		for (int i = 0; i < Moles.Length; i++)
		{
			if (Moles[i] != 0f)
			{
				molesPerGas.Add(((Gas)i/*cast due to constrained. prefix*/).ToString(), Moles[i]);
			}
		}
		return new GasMixtureStringRepresentation(TotalMoles, Temperature, Pressure, molesPerGas);
	}

	private GasEnumerator GetEnumerator()
	{
		return new GasEnumerator(this);
	}

	IEnumerator<(Gas gas, float moles)> IEnumerable<(Gas, float)>.GetEnumerator()
	{
		return GetEnumerator();
	}

	public override bool Equals(object? obj)
	{
		if (obj is GasMixture mix)
		{
			return Equals(mix);
		}
		return false;
	}

	public bool Equals(GasMixture? other)
	{
		if (this == other)
		{
			return true;
		}
		if (other == null)
		{
			return false;
		}
		if (Enumerable.SequenceEqual(Moles, other.Moles) && _temperature.Equals(other._temperature) && Enumerable.SequenceEqual(ReactionResults, other.ReactionResults) && Immutable == other.Immutable)
		{
			return Volume.Equals(other.Volume);
		}
		return false;
	}

	public override int GetHashCode()
	{
		HashCode hashCode = default(HashCode);
		for (int i = 0; i < 9; i++)
		{
			float moles = Moles[i];
			hashCode.Add(moles);
		}
		hashCode.Add(_temperature);
		hashCode.Add(Immutable);
		hashCode.Add(Volume);
		return hashCode.ToHashCode();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public GasMixture Clone()
	{
		if (Immutable)
		{
			return this;
		}
		return new GasMixture
		{
			Moles = (float[])Moles.Clone(),
			_temperature = _temperature,
			Volume = Volume
		};
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GasMixture target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<GasMixture>(this, ref target, hookCtx, true, context))
		{
			float[] MolesTemp = null;
			if (Moles == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<float[]>(Moles, ref MolesTemp, hookCtx, true, context))
			{
				MolesTemp = serialization.CreateCopy<float[]>(Moles, hookCtx, context, false);
			}
			target.Moles = MolesTemp;
			float _temperatureTemp = 0f;
			if (!serialization.TryCustomCopy<float>(_temperature, ref _temperatureTemp, hookCtx, false, context))
			{
				_temperatureTemp = _temperature;
			}
			target._temperature = _temperatureTemp;
			bool ImmutableTemp = false;
			if (!serialization.TryCustomCopy<bool>(Immutable, ref ImmutableTemp, hookCtx, false, context))
			{
				ImmutableTemp = Immutable;
			}
			target.Immutable = ImmutableTemp;
			float VolumeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Volume, ref VolumeTemp, hookCtx, false, context))
			{
				VolumeTemp = Volume;
			}
			target.Volume = VolumeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GasMixture target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GasMixture cast = (GasMixture)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public GasMixture Instantiate()
	{
		return new GasMixture();
	}
}
