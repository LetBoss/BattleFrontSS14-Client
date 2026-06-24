using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Robust.Shared.Utility;

internal struct FixedArray8<T> : IEquatable<FixedArray8<T>>
{
	public T _00;

	public T _01;

	public T _02;

	public T _03;

	public T _04;

	public T _05;

	public T _06;

	public T _07;

	public Span<T> AsSpan => MemoryMarshal.CreateSpan(ref _00, 8);

	internal FixedArray8(T x0, T x1, T x2, T x3, T x4, T x5, T x6, T x7)
	{
		_00 = x0;
		_01 = x1;
		_02 = x2;
		_03 = x3;
		_04 = x4;
		_05 = x5;
		_06 = x6;
		_07 = x7;
	}

	public bool Equals(FixedArray8<T> other)
	{
		if (EqualityComparer<T>.Default.Equals(_00, other._00) && EqualityComparer<T>.Default.Equals(_01, other._01) && EqualityComparer<T>.Default.Equals(_02, other._02) && EqualityComparer<T>.Default.Equals(_03, other._03) && EqualityComparer<T>.Default.Equals(_04, other._04) && EqualityComparer<T>.Default.Equals(_05, other._05) && EqualityComparer<T>.Default.Equals(_06, other._06))
		{
			return EqualityComparer<T>.Default.Equals(_07, other._07);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is FixedArray8<T> other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(_00, _01, _02, _03, _04, _05, _06, _07);
	}
}
