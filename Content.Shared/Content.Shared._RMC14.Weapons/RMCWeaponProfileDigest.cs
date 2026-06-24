using System;
using System.Text;

namespace Content.Shared._RMC14.Weapons;

public static class RMCWeaponProfileDigest
{
	private const ulong SeedA = 14695981039346656037uL;

	private const ulong SeedB = 11400713685563067188uL;

	private const ulong SeedC = 7809847782465536322uL;

	private const ulong SeedD = 9650029242287828579uL;

	public static byte[] ComputeDigest(ReadOnlySpan<byte> data)
	{
		ulong a = 14695981039346656037uL;
		ulong b = 11400713685563067188uL;
		ulong c = 7809847782465536322uL;
		ulong d = 9650029242287828579uL;
		for (int i = 0; i < data.Length; i++)
		{
			ulong value = (ulong)(data[i] + 1);
			a ^= (ulong)((long)value + (long)i);
			a *= 1099511628211L;
			b += value + (a >> 11);
			b ^= (ulong)((long)a * -7046029288634856825L);
			b = RotateLeft(b, 7);
			c ^= b + value * 1099511628211L;
			c *= 14029467366897019727uL;
			c = RotateLeft(c, 13);
			d += c ^ (value << (i & 7) * 8);
			d *= 7046029254386353131L;
			d = RotateLeft(d, 17);
		}
		a ^= (ulong)data.Length;
		b ^= a >> 29;
		c ^= b >> 31;
		d ^= c >> 27;
		a = Avalanche(a ^ d);
		b = Avalanche(b ^ a);
		c = Avalanche(c ^ b);
		d = Avalanche(d ^ c);
		byte[] array = new byte[32];
		WriteUInt64(array, 0, a);
		WriteUInt64(array, 8, b);
		WriteUInt64(array, 16, c);
		WriteUInt64(array, 24, d);
		return array;
	}

	public static byte[] ComputeDigest(string value)
	{
		return ComputeDigest(Encoding.UTF8.GetBytes(value));
	}

	public static void ApplyChallengeMask(byte[] digest, int challengeSalt)
	{
		byte[] array = new byte[4];
		WriteInt32(array, 0, challengeSalt);
		byte[] mask = ComputeDigest(array);
		for (int i = 0; i < digest.Length; i++)
		{
			digest[i] ^= mask[i % mask.Length];
		}
	}

	public static string ToHexLower(ReadOnlySpan<byte> bytes)
	{
		char[] chars = new char[bytes.Length * 2];
		for (int i = 0; i < bytes.Length; i++)
		{
			byte value = bytes[i];
			chars[i * 2] = "0123456789abcdef"[value >> 4];
			chars[i * 2 + 1] = "0123456789abcdef"[value & 0xF];
		}
		return new string(chars);
	}

	public static byte[] ParseHex(ReadOnlySpan<char> value)
	{
		if (value.Length % 2 != 0)
		{
			return Array.Empty<byte>();
		}
		byte[] bytes = new byte[value.Length / 2];
		for (int i = 0; i < bytes.Length; i++)
		{
			if (!TryParseHexNibble(value[i * 2], out var hi) || !TryParseHexNibble(value[i * 2 + 1], out var lo))
			{
				return Array.Empty<byte>();
			}
			bytes[i] = (byte)((hi << 4) | lo);
		}
		return bytes;
	}

	private static bool TryParseHexNibble(char ch, out int value)
	{
		if (ch >= '0' && ch <= '9')
		{
			value = ch - 48;
			return true;
		}
		if (ch >= 'a' && ch <= 'f')
		{
			value = ch - 97 + 10;
			return true;
		}
		if (ch >= 'A' && ch <= 'F')
		{
			value = ch - 65 + 10;
			return true;
		}
		value = 0;
		return false;
	}

	private static ulong RotateLeft(ulong value, int shift)
	{
		return (value << shift) | (value >> 64 - shift);
	}

	private static ulong Avalanche(ulong value)
	{
		value ^= value >> 33;
		value *= 18397679294719823053uL;
		value ^= value >> 33;
		value *= 14181476777654086739uL;
		value ^= value >> 33;
		return value;
	}

	private static void WriteUInt64(byte[] buffer, int offset, ulong value)
	{
		buffer[offset] = (byte)value;
		buffer[offset + 1] = (byte)(value >> 8);
		buffer[offset + 2] = (byte)(value >> 16);
		buffer[offset + 3] = (byte)(value >> 24);
		buffer[offset + 4] = (byte)(value >> 32);
		buffer[offset + 5] = (byte)(value >> 40);
		buffer[offset + 6] = (byte)(value >> 48);
		buffer[offset + 7] = (byte)(value >> 56);
	}

	private static void WriteInt32(byte[] buffer, int offset, int value)
	{
		buffer[offset] = (byte)value;
		buffer[offset + 1] = (byte)(value >> 8);
		buffer[offset + 2] = (byte)(value >> 16);
		buffer[offset + 3] = (byte)(value >> 24);
	}
}
