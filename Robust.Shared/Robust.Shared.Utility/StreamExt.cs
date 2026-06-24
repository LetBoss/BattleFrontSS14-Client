using System;
using System.IO;

namespace Robust.Shared.Utility;

public static class StreamExt
{
	public static byte[] CopyToArray(this Stream stream)
	{
		using MemoryStream memoryStream = new MemoryStream();
		stream.CopyTo(memoryStream);
		return memoryStream.ToArray();
	}

	internal static byte[] CopyToPinnedArray(this Stream stream)
	{
		MemoryStream memoryStream = new MemoryStream();
		stream.CopyTo(memoryStream);
		int length = (int)memoryStream.Length;
		byte[] array = GC.AllocateUninitializedArray<byte>(length, pinned: true);
		memoryStream.GetBuffer().AsSpan(0, length).CopyTo(array);
		return array;
	}

	internal static MemoryStream ConsumeToMemoryStream(this Stream stream)
	{
		MemoryStream result = stream.CopyToMemoryStream();
		stream.Dispose();
		return result;
	}

	internal static MemoryStream CopyToMemoryStream(this Stream stream)
	{
		MemoryStream memoryStream = new MemoryStream();
		stream.CopyTo(memoryStream);
		memoryStream.Seek(0L, SeekOrigin.Begin);
		return memoryStream;
	}

	public static byte[] ReadExact(this Stream stream, int amount)
	{
		byte[] array = new byte[amount];
		int num;
		for (int i = 0; i < amount; i += num)
		{
			num = stream.Read(array, i, amount - i);
			if (num == 0)
			{
				throw new EndOfStreamException();
			}
		}
		return array;
	}

	public static void ReadExact(this Stream stream, Span<byte> buffer)
	{
		while (buffer.Length > 0)
		{
			int num = stream.Read(buffer);
			if (num == 0)
			{
				throw new EndOfStreamException();
			}
			int num2 = num;
			buffer = buffer.Slice(num2, buffer.Length - num2);
		}
	}

	public static int ReadToEnd(this Stream stream, Span<byte> buffer)
	{
		int num = 0;
		while (true)
		{
			int num2 = stream.Read(buffer);
			num += num2;
			if (num2 == 0)
			{
				break;
			}
			int num3 = num2;
			buffer = buffer.Slice(num3, buffer.Length - num3);
		}
		return num;
	}

	public static int ReadToEnd(this Stream stream, byte[] buffer)
	{
		int num = 0;
		int num2;
		do
		{
			num2 = stream.Read(buffer, num, buffer.Length - num);
			num += num2;
		}
		while (num2 != 0);
		return num;
	}

	public static Span<byte> AsSpan(this MemoryStream ms)
	{
		return ms.GetBuffer().AsSpan(0, (int)ms.Length);
	}

	public static Memory<byte> AsMemory(this MemoryStream ms)
	{
		return ms.GetBuffer().AsMemory(0, (int)ms.Length);
	}
}
