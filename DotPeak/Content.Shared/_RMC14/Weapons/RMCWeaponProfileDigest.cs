// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.RMCWeaponProfileDigest
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;
using System.Text;

#nullable enable
namespace Content.Shared._RMC14.Weapons;

public static class RMCWeaponProfileDigest
{
  private const ulong SeedA = 14695981039346656037;
  private const ulong SeedB = 11400713685563067188;
  private const ulong SeedC = 7809847782465536322;
  private const ulong SeedD = 9650029242287828579;

  public static byte[] ComputeDigest(ReadOnlySpan<byte> data)
  {
    ulong num1 = 14695981039346656037;
    ulong num2 = 11400713685563067188;
    ulong num3 = 7809847782465536322;
    ulong num4 = 9650029242287828579;
    for (int index = 0; index < data.Length; ++index)
    {
      ulong num5 = (ulong) ((int) data[index] + 1);
      num1 = (num1 ^ num5 + (ulong) index) * 1099511628211UL /*0x0100000001B3*/;
      num2 = RMCWeaponProfileDigest.RotateLeft(num2 + (num5 + (num1 >> 11)) ^ num1 * 11400714785074694791UL, 7);
      num3 = RMCWeaponProfileDigest.RotateLeft((num3 ^ num2 + num5 * 1099511628211UL /*0x0100000001B3*/) * 14029467366897019727UL, 13);
      num4 = RMCWeaponProfileDigest.RotateLeft((num4 + (num3 ^ num5 << (index & 7) * 8)) * 7046029254386353131UL, 17);
    }
    ulong num6 = num1 ^ (ulong) data.Length;
    ulong num7 = num2 ^ num6 >> 29;
    ulong num8 = num3 ^ num7 >> 31 /*0x1F*/;
    ulong num9 = num4 ^ num8 >> 27;
    ulong num10 = RMCWeaponProfileDigest.Avalanche(num6 ^ num9);
    ulong num11 = RMCWeaponProfileDigest.Avalanche(num7 ^ num10);
    ulong num12 = RMCWeaponProfileDigest.Avalanche(num8 ^ num11);
    ulong num13 = RMCWeaponProfileDigest.Avalanche(num9 ^ num12);
    byte[] buffer = new byte[32 /*0x20*/];
    RMCWeaponProfileDigest.WriteUInt64(buffer, 0, num10);
    RMCWeaponProfileDigest.WriteUInt64(buffer, 8, num11);
    RMCWeaponProfileDigest.WriteUInt64(buffer, 16 /*0x10*/, num12);
    RMCWeaponProfileDigest.WriteUInt64(buffer, 24, num13);
    return buffer;
  }

  public static byte[] ComputeDigest(string value)
  {
    return RMCWeaponProfileDigest.ComputeDigest((ReadOnlySpan<byte>) Encoding.UTF8.GetBytes(value));
  }

  public static void ApplyChallengeMask(byte[] digest, int challengeSalt)
  {
    byte[] numArray = new byte[4];
    RMCWeaponProfileDigest.WriteInt32(numArray, 0, challengeSalt);
    byte[] digest1 = RMCWeaponProfileDigest.ComputeDigest((ReadOnlySpan<byte>) numArray);
    for (int index = 0; index < digest.Length; ++index)
      digest[index] ^= digest1[index % digest1.Length];
  }

  public static string ToHexLower(ReadOnlySpan<byte> bytes)
  {
    char[] chArray = new char[bytes.Length * 2];
    for (int index = 0; index < bytes.Length; ++index)
    {
      byte num = bytes[index];
      chArray[index * 2] = "0123456789abcdef"[(int) num >> 4];
      chArray[index * 2 + 1] = "0123456789abcdef"[(int) num & 15];
    }
    return new string(chArray);
  }

  public static byte[] ParseHex(ReadOnlySpan<char> value)
  {
    if (value.Length % 2 != 0)
      return Array.Empty<byte>();
    byte[] hex = new byte[value.Length / 2];
    for (int index = 0; index < hex.Length; ++index)
    {
      int num1;
      int num2;
      if (!RMCWeaponProfileDigest.TryParseHexNibble(value[index * 2], out num1) || !RMCWeaponProfileDigest.TryParseHexNibble(value[index * 2 + 1], out num2))
        return Array.Empty<byte>();
      hex[index] = (byte) (num1 << 4 | num2);
    }
    return hex;
  }

  private static bool TryParseHexNibble(char ch, out int value)
  {
    if (ch >= '0' && ch <= '9')
    {
      value = (int) ch - 48 /*0x30*/;
      return true;
    }
    if (ch >= 'a' && ch <= 'f')
    {
      value = (int) ch - 97 + 10;
      return true;
    }
    if (ch >= 'A' && ch <= 'F')
    {
      value = (int) ch - 65 + 10;
      return true;
    }
    value = 0;
    return false;
  }

  private static ulong RotateLeft(ulong value, int shift)
  {
    return value << shift | value >> 64 /*0x40*/ - shift;
  }

  private static ulong Avalanche(ulong value)
  {
    value ^= value >> 33;
    value *= 18397679294719823053UL;
    value ^= value >> 33;
    value *= 14181476777654086739UL;
    value ^= value >> 33;
    return value;
  }

  private static void WriteUInt64(byte[] buffer, int offset, ulong value)
  {
    buffer[offset] = (byte) value;
    buffer[offset + 1] = (byte) (value >> 8);
    buffer[offset + 2] = (byte) (value >> 16 /*0x10*/);
    buffer[offset + 3] = (byte) (value >> 24);
    buffer[offset + 4] = (byte) (value >> 32 /*0x20*/);
    buffer[offset + 5] = (byte) (value >> 40);
    buffer[offset + 6] = (byte) (value >> 48 /*0x30*/);
    buffer[offset + 7] = (byte) (value >> 56);
  }

  private static void WriteInt32(byte[] buffer, int offset, int value)
  {
    buffer[offset] = (byte) value;
    buffer[offset + 1] = (byte) (value >> 8);
    buffer[offset + 2] = (byte) (value >> 16 /*0x10*/);
    buffer[offset + 3] = (byte) (value >> 24);
  }
}
