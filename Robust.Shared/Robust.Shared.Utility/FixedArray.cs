using System;

namespace Robust.Shared.Utility;

internal static class FixedArray
{
	public static Span<T> Alloc2<T>(out FixedArray2<T> discard)
	{
		discard = default(FixedArray2<T>);
		return discard.AsSpan;
	}

	public static Span<T> Alloc4<T>(out FixedArray4<T> discard)
	{
		discard = default(FixedArray4<T>);
		return discard.AsSpan;
	}

	public static Span<T> Alloc8<T>(out FixedArray8<T> discard)
	{
		discard = default(FixedArray8<T>);
		return discard.AsSpan;
	}

	public static Span<T> Alloc16<T>(out FixedArray16<T> discard)
	{
		discard = default(FixedArray16<T>);
		return discard.AsSpan;
	}

	public static Span<T> Alloc32<T>(out FixedArray32<T> discard)
	{
		discard = default(FixedArray32<T>);
		return discard.AsSpan;
	}

	public static Span<T> Alloc64<T>(out FixedArray64<T> discard)
	{
		discard = default(FixedArray64<T>);
		return discard.AsSpan;
	}

	public static Span<T> Alloc128<T>(out FixedArray128<T> discard)
	{
		discard = default(FixedArray128<T>);
		return discard.AsSpan;
	}
}
