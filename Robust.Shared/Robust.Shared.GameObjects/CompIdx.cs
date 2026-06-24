using System;
using System.Reflection;
using System.Threading;
using Robust.Shared.Maths;

namespace Robust.Shared.GameObjects;

public readonly struct CompIdx : IEquatable<CompIdx>
{
	private static class Store<T>
	{
		public static readonly CompIdx Index = new CompIdx(Interlocked.Increment(ref _CompIdxMaster));
	}

	internal readonly int Value;

	private static int _CompIdxMaster = -1;

	internal static CompIdx Index<T>()
	{
		return Store<T>.Index;
	}

	internal static int ArrayIndex<T>()
	{
		return Index<T>().Value;
	}

	internal static CompIdx GetIndex(Type type)
	{
		return (CompIdx)typeof(Store<>).MakeGenericType(type).GetField("Index", BindingFlags.Static | BindingFlags.Public).GetValue(null);
	}

	internal static void AssignArray<T>(ref T[] array, CompIdx idx, T value)
	{
		RefArray(ref array, idx) = value;
	}

	internal static ref T RefArray<T>(ref T[] array, CompIdx idx)
	{
		if (array.Length <= idx.Value)
		{
			int newSize = MathHelper.NextPowerOfTwo(Math.Max(8, idx.Value + 1));
			Array.Resize(ref array, newSize);
		}
		return ref array[idx.Value];
	}

	internal CompIdx(int value)
	{
		Value = value;
	}

	public bool Equals(CompIdx other)
	{
		return Value == other.Value;
	}

	public override bool Equals(object? obj)
	{
		if (obj is CompIdx other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Value;
	}

	public static bool operator ==(CompIdx left, CompIdx right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(CompIdx left, CompIdx right)
	{
		return !left.Equals(right);
	}
}
