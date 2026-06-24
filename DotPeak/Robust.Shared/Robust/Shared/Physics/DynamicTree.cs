// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.DynamicTree
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Physics;

public abstract class DynamicTree
{
  public const int MinimumCapacity = 16 /*0x10*/;
  protected const float AabbMultiplier = 2f;
  protected readonly float AabbExtendSize;
  protected readonly Func<int, int> GrowthFunc;

  protected DynamicTree(float aabbExtendSize, Func<int, int>? growthFunc)
  {
    this.AabbExtendSize = aabbExtendSize;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this.GrowthFunc = growthFunc ?? DynamicTree.\u003C\u003EO.\u003C0\u003E__DefaultGrowthFunc ?? (DynamicTree.\u003C\u003EO.\u003C0\u003E__DefaultGrowthFunc = new Func<int, int>(DynamicTree.DefaultGrowthFunc));
  }

  private static int DefaultGrowthFunc(int x) => x * 2;

  [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
  public readonly struct Proxy(int v) : IEquatable<DynamicTree.Proxy>, IComparable<DynamicTree.Proxy>
  {
    private readonly int _value = v;

    public static DynamicTree.Proxy Free
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new DynamicTree.Proxy(-1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(DynamicTree.Proxy other) => this._value == other._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(DynamicTree.Proxy other) => this._value.CompareTo(other._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj)
    {
      return obj is DynamicTree.Proxy other && this.Equals(other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => this._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator int(DynamicTree.Proxy n) => n._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator DynamicTree.Proxy(int v) => new DynamicTree.Proxy(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(DynamicTree.Proxy a, DynamicTree.Proxy b)
    {
      return a._value == b._value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(DynamicTree.Proxy a, DynamicTree.Proxy b)
    {
      return a._value != b._value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(DynamicTree.Proxy a, DynamicTree.Proxy b) => a._value > b._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(DynamicTree.Proxy a, DynamicTree.Proxy b) => a._value < b._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(DynamicTree.Proxy a, DynamicTree.Proxy b)
    {
      return a._value >= b._value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(DynamicTree.Proxy a, DynamicTree.Proxy b)
    {
      return a._value <= b._value;
    }

    public override string ToString() => this._value.ToString();
  }
}
