using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Robust.Shared.Utility;

internal struct FixedArray4<T> : IEquatable<FixedArray4<T>>
{
	public T _00;

	public T _01;

	public T _02;

	public T _03;

	public Span<T> AsSpan => MemoryMarshal.CreateSpan(ref _00, 4);

	internal FixedArray4(T x0, T x1, T x2, T x3)
	{
		_00 = x0;
		_01 = x1;
		_02 = x2;
		_03 = x3;
	}

	public bool Equals(FixedArray4<T> other)
	{
		if (EqualityComparer<T>.Default.Equals(_00, other._00) && EqualityComparer<T>.Default.Equals(_01, other._01) && EqualityComparer<T>.Default.Equals(_02, other._02))
		{
			return EqualityComparer<T>.Default.Equals(_03, other._03);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is FixedArray4<T> other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(_00, _01, _02, _03);
	}
}
