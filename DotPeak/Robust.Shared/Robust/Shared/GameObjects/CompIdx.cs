// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.CompIdx
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using System;
using System.Reflection;
using System.Threading;

#nullable enable
namespace Robust.Shared.GameObjects;

public readonly struct CompIdx : IEquatable<CompIdx>
{
  internal readonly int Value;
  private static int _CompIdxMaster = -1;

  internal static CompIdx Index<T>() => CompIdx.Store<T>.Index;

  internal static int ArrayIndex<T>() => CompIdx.Index<T>().Value;

  internal static CompIdx GetIndex(Type type)
  {
    return (CompIdx) typeof (CompIdx.Store<>).MakeGenericType(type).GetField("Index", BindingFlags.Static | BindingFlags.Public).GetValue((object) null);
  }

  internal static void AssignArray<T>(ref T[] array, CompIdx idx, T value)
  {
    CompIdx.RefArray<T>(ref array, idx) = value;
  }

  internal static ref T RefArray<T>(ref T[] array, CompIdx idx)
  {
    if (array.Length <= idx.Value)
    {
      int newSize = MathHelper.NextPowerOfTwo(Math.Max(8, idx.Value + 1));
      Array.Resize<T>(ref array, newSize);
    }
    return ref array[idx.Value];
  }

  internal CompIdx(int value) => this.Value = value;

  public bool Equals(CompIdx other) => this.Value == other.Value;

  public override bool Equals(object? obj) => obj is CompIdx other && this.Equals(other);

  public override int GetHashCode() => this.Value;

  public static bool operator ==(CompIdx left, CompIdx right) => left.Equals(right);

  public static bool operator !=(CompIdx left, CompIdx right) => !left.Equals(right);

  private static class Store<T>
  {
    public static readonly CompIdx Index = new CompIdx(Interlocked.Increment(ref CompIdx._CompIdxMaster));
  }
}
