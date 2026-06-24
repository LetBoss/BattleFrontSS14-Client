using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Robust.Shared.Utility;

internal struct FixedArray2<T> : IEquatable<FixedArray2<T>>
{
	public T _00;

	public T _01;

	public Span<T> AsSpan => MemoryMarshal.CreateSpan(ref _00, 2);

	internal FixedArray2(T x0, T x1)
	{
		_00 = x0;
		_01 = x1;
	}

	public bool Equals(FixedArray2<T> other)
	{
		if (EqualityComparer<T>.Default.Equals(_00, other._00))
		{
			return EqualityComparer<T>.Default.Equals(_01, other._01);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is FixedArray2<T> other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(_00, _01);
	}
}
