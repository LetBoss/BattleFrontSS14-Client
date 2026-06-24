using System;
using System.Runtime.InteropServices;

namespace Robust.Shared.Utility;

internal struct FixedArray64<T>
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

	public T _16;

	public T _17;

	public T _18;

	public T _19;

	public T _20;

	public T _21;

	public T _22;

	public T _23;

	public T _24;

	public T _25;

	public T _26;

	public T _27;

	public T _28;

	public T _29;

	public T _30;

	public T _31;

	public T _32;

	public T _33;

	public T _34;

	public T _35;

	public T _36;

	public T _37;

	public T _38;

	public T _39;

	public T _40;

	public T _41;

	public T _42;

	public T _43;

	public T _44;

	public T _45;

	public T _46;

	public T _47;

	public T _48;

	public T _49;

	public T _50;

	public T _51;

	public T _52;

	public T _53;

	public T _54;

	public T _55;

	public T _56;

	public T _57;

	public T _58;

	public T _59;

	public T _60;

	public T _61;

	public T _62;

	public T _63;

	public Span<T> AsSpan => MemoryMarshal.CreateSpan(ref _00, 64);
}
