using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Robust.Shared.Utility;

internal static class ImageOps
{
	public static void Blit<T>(Image<T> source, UIBox2i sourceRect, Image<T> destination, Vector2i destinationOffset) where T : unmanaged, IPixel<T>
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Blit(GetPixelSpan<T>(source), ((Image)source).Width, sourceRect, destination, destinationOffset);
	}

	public static void Blit<T>(ReadOnlySpan<T> source, int sourceWidth, UIBox2i sourceRect, Image<T> destination, Vector2i destinationOffset) where T : unmanaged, IPixel<T>
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Span<T> pixelSpan = GetPixelSpan<T>(destination);
		int width = ((Image)destination).Width;
		int height = ((UIBox2i)(ref sourceRect)).Height;
		int width2 = ((UIBox2i)(ref sourceRect)).Width;
		Vector2i val = destinationOffset;
		Unsafe.SkipInit(out int num);
		Unsafe.SkipInit(out int num2);
		((Vector2i)(ref val)).Deconstruct(ref num, ref num2);
		int num3 = num;
		int num4 = num2;
		for (int i = 0; i < height; i++)
		{
			int num5 = sourceWidth * (i + sourceRect.Top) + sourceRect.Left;
			int num6 = width * (i + num4) + num3;
			num2 = num5;
			ReadOnlySpan<T> readOnlySpan = source.Slice(num2, num5 + width2 - num2);
			num2 = num6;
			Span<T> destination2 = pixelSpan.Slice(num2, num6 + width2 - num2);
			readOnlySpan.CopyTo(destination2);
		}
	}

	public static Span<T> GetPixelSpan<T>(Image<T> image) where T : unmanaged, IPixel<T>
	{
		Unsafe.SkipInit(out Memory<T> memory);
		if (!image.DangerousTryGetSinglePixelMemory(ref memory))
		{
			throw new ArgumentException("Image is not backed by a single buffer, cannot fetch span.");
		}
		return memory.Span;
	}
}
