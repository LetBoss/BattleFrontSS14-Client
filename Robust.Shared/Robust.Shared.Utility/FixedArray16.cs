using System;
using System.Runtime.InteropServices;

namespace Robust.Shared.Utility;

internal struct FixedArray16<T>
{
	public T _00;

	public T _01;

	public T _02;

	public T _03;

	public T _04;

	public T _05;

	public T _06;

	public T _07;

	public T _08;

	public T _09;

	public T _10;

	public T _11;

	public T _12;

	public T _13;

	public T _14;

	public T _15;

	public Span<T> AsSpan => MemoryMarshal.CreateSpan(ref _00, 16);
}
