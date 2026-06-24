// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.GasMixture
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Atmos;

[DataDefinition]
[Serializable]
public sealed class GasMixture : 
  IEquatable<GasMixture>,
  ISerializationHooks,
  IEnumerable<(Gas gas, float moles)>,
  IEnumerable,
  ISerializationGenerated<GasMixture>,
  ISerializationGenerated
{
  [Access(new Type[] {typeof (SharedAtmosphereSystem), typeof (SharedAtmosDebugOverlaySystem), typeof (GasMixture.GasEnumerator)})]
  [DataField(null, false, 1, false, false, null)]
  public float[] Moles = new float[12];
  [DataField("temperature", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  private float _temperature = 2.7f;
  [Robust.Shared.ViewVariables.ViewVariables]
  public readonly float[] ReactionResults = new float[1];

  public static GasMixture SpaceGas
  {
    get
    {
      return new GasMixture()
      {
        Volume = 2500f,
        Temperature = 2.7f,
        Immutable = true
      };
    }
  }

  public float this[int gas] => this.Moles[gas];

  [DataField("immutable", false, 1, false, false, null)]
  public bool Immutable { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public float TotalMoles
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)] get
    {
      return NumericsHelpers.HorizontalAdd((ReadOnlySpan<float>) this.Moles);
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public float Pressure
  {
    get
    {
      return (double) this.Volume <= 0.0 ? 0.0f : this.TotalMoles * 8.314463f * this.Temperature / this.Volume;
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public float Temperature
  {
    get => this._temperature;
    set
    {
      if (this.Immutable)
        return;
      this._temperature = MathF.Min(MathF.Max(value, 2.7f), 262144f);
    }
  }

  [DataField("volume", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float Volume { get; set; }

  public GasMixture()
  {
  }

  public GasMixture(float volume = 0.0f)
  {
    if ((double) volume < 0.0)
      volume = 0.0f;
    this.Volume = volume;
  }

  public GasMixture(float[] moles, float temp, float volume = 2500f)
  {
    if (moles.Length != 12)
      throw new InvalidOperationException("Invalid mole array length");
    if ((double) volume < 0.0)
      volume = 0.0f;
    this._temperature = temp;
    this.Moles = moles;
    this.Volume = volume;
  }

  public GasMixture(GasMixture toClone) => this.CopyFrom(toClone);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void MarkImmutable() => this.Immutable = true;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public float GetMoles(int gasId) => this.Moles[gasId];

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public float GetMoles(Gas gas) => this.GetMoles((int) gas);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void SetMoles(int gasId, float quantity)
  {
    if (!float.IsFinite(quantity) || float.IsNegative(quantity))
      throw new ArgumentException($"Invalid quantity \"{quantity}\" specified!", nameof (quantity));
    if (this.Immutable)
      return;
    this.Moles[gasId] = quantity;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void SetMoles(Gas gas, float quantity) => this.SetMoles((int) gas, quantity);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void AdjustMoles(int gasId, float quantity)
  {
    if (this.Immutable)
      return;
    if (!float.IsFinite(quantity))
      throw new ArgumentException($"Invalid quantity \"{quantity}\" specified!", nameof (quantity));
    ref float local = ref this.Moles[gasId];
    local = MathF.Max(local + quantity, 0.0f);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void AdjustMoles(Gas gas, float moles) => this.AdjustMoles((int) gas, moles);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public GasMixture Remove(float amount) => this.RemoveRatio(amount / this.TotalMoles);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public GasMixture RemoveRatio(float ratio)
  {
    if ((double) ratio > 0.0)
    {
      if ((double) ratio > 1.0)
        ratio = 1f;
      GasMixture gasMixture = new GasMixture(this.Volume)
      {
        Temperature = this.Temperature
      };
      this.Moles.CopyTo<float>(gasMixture.Moles.AsSpan<float>());
      NumericsHelpers.Multiply((Span<float>) gasMixture.Moles, ratio);
      if (!this.Immutable)
        NumericsHelpers.Sub((Span<float>) this.Moles, (ReadOnlySpan<float>) gasMixture.Moles);
      for (int index = 0; index < this.Moles.Length; ++index)
      {
        float mole1 = this.Moles[index];
        float mole2 = gasMixture.Moles[index];
        if (((double) mole1 < 5.0000000584304871E-08 || float.IsNaN(mole1)) && !this.Immutable)
          this.Moles[index] = 0.0f;
        if ((double) mole2 < 5.0000000584304871E-08 || float.IsNaN(mole2))
          gasMixture.Moles[index] = 0.0f;
      }
      return gasMixture;
    }
    return new GasMixture(this.Volume)
    {
      Temperature = this.Temperature
    };
  }

  public GasMixture RemoveVolume(float vol) => this.RemoveRatio(vol / this.Volume);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void CopyFrom(GasMixture sample)
  {
    if (this.Immutable)
      return;
    this.Volume = sample.Volume;
    sample.Moles.CopyTo((Array) this.Moles, 0);
    this.Temperature = sample.Temperature;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Clear()
  {
    if (this.Immutable)
      return;
    Array.Clear((Array) this.Moles, 0, 9);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Multiply(float multiplier)
  {
    if (this.Immutable)
      return;
    NumericsHelpers.Multiply((Span<float>) this.Moles, multiplier);
  }

  void ISerializationHooks.AfterDeserialization() => Array.Resize<float>(ref this.Moles, 12);

  public GasMixtureStringRepresentation ToPrettyString()
  {
    Dictionary<string, float> MolesPerGas = new Dictionary<string, float>();
    for (int index = 0; index < this.Moles.Length; ++index)
    {
      if ((double) this.Moles[index] != 0.0)
        MolesPerGas.Add(((Gas) index).ToString(), this.Moles[index]);
    }
    return new GasMixtureStringRepresentation(this.TotalMoles, this.Temperature, this.Pressure, MolesPerGas);
  }

  private GasMixture.GasEnumerator GetEnumerator() => new GasMixture.GasEnumerator(this);

  IEnumerator<(Gas gas, float moles)> IEnumerable<(Gas gas, float moles)>.System\u002ECollections\u002EGeneric\u002EIEnumerable\u003CSystem\u002EValueTuple\u003CContent\u002EShared\u002EAtmos\u002EGas\u002CSystem\u002ESingle\u003E\u003E\u002EGetEnumerator()
  {
    return (IEnumerator<(Gas, float)>) this.GetEnumerator();
  }

  public override bool Equals(object? obj) => obj is GasMixture other && this.Equals(other);

  public bool Equals(GasMixture? other)
  {
    if (this == other)
      return true;
    return other != null && ((IEnumerable<float>) this.Moles).SequenceEqual<float>((IEnumerable<float>) other.Moles) && this._temperature.Equals(other._temperature) && ((IEnumerable<float>) this.ReactionResults).SequenceEqual<float>((IEnumerable<float>) other.ReactionResults) && this.Immutable == other.Immutable && this.Volume.Equals(other.Volume);
  }

  public override int GetHashCode()
  {
    HashCode hashCode = new HashCode();
    for (int index = 0; index < 9; ++index)
    {
      float mole = this.Moles[index];
      hashCode.Add<float>(mole);
    }
    hashCode.Add<float>(this._temperature);
    hashCode.Add<bool>(this.Immutable);
    hashCode.Add<float>(this.Volume);
    return hashCode.ToHashCode();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public GasMixture Clone()
  {
    if (this.Immutable)
      return this;
    return new GasMixture()
    {
      Moles = (float[]) this.Moles.Clone(),
      _temperature = this._temperature,
      Volume = this.Volume
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GasMixture target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<GasMixture>(this, ref target, hookCtx, true, context))
      return;
    float[] numArray = (float[]) null;
    if (this.Moles == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<float[]>(this.Moles, ref numArray, hookCtx, true, context))
      numArray = serialization.CreateCopy<float[]>(this.Moles, hookCtx, context, false);
    target.Moles = numArray;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this._temperature, ref num1, hookCtx, false, context))
      num1 = this._temperature;
    target._temperature = num1;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Immutable, ref flag, hookCtx, false, context))
      flag = this.Immutable;
    target.Immutable = flag;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Volume, ref num2, hookCtx, false, context))
      num2 = this.Volume;
    target.Volume = num2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GasMixture target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GasMixture target1 = (GasMixture) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public GasMixture Instantiate() => new GasMixture();

  public struct GasEnumerator(GasMixture mixture) : 
    IEnumerator<(Gas gas, float moles)>,
    IEnumerator,
    IDisposable
  {
    private int _idx = -1;

    public void Dispose()
    {
    }

    public bool MoveNext() => ++this._idx < 9;

    public void Reset() => this._idx = -1;

    public (Gas gas, float moles) Current => ((Gas) this._idx, mixture.Moles[this._idx]);

    object? IEnumerator.Current => (object) this.Current;
  }
}
