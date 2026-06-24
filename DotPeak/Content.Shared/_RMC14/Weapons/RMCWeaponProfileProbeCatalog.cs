// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.RMCWeaponProfileProbeCatalog
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Weapons;

public static class RMCWeaponProfileProbeCatalog
{
  private static readonly string[] PackagedManifestFragmentDirectories = new string[3]
  {
    "/Textures/Generated",
    "/Audio/Generated",
    "/Generated/Layout"
  };
  private static readonly string[] PackagedManifestFragmentPrefixes = new string[3]
  {
    "atlas",
    "cache",
    "layout"
  };
  private static readonly string[] IntegrityBenignResourceProbePathsValue = new string[3]
  {
    "/Textures/Shaders/hcut.swsl",
    "/Textures/Shaders/outline.swsl",
    "/Prototypes/audio.yml"
  };
  public static readonly byte[] MethodLogicalSlots = new byte[15]
  {
    (byte) 1,
    (byte) 2,
    (byte) 3,
    (byte) 4,
    (byte) 5,
    (byte) 6,
    (byte) 7,
    (byte) 8,
    (byte) 9,
    (byte) 10,
    (byte) 11,
    (byte) 12,
    (byte) 13,
    (byte) 14,
    (byte) 15
  };
  public static readonly byte[] CatalogSummaryLogicalSlots = new byte[6]
  {
    (byte) 20,
    (byte) 21,
    (byte) 22,
    (byte) 23,
    (byte) 24,
    (byte) 25
  };
  public static readonly byte[] CatalogSummaryVariantLogicalSlots = new byte[6]
  {
    (byte) 26,
    (byte) 27,
    (byte) 28,
    (byte) 29,
    (byte) 30,
    (byte) 31 /*0x1F*/
  };
  public static readonly byte[] ManifestLogicalSlots = new byte[5]
  {
    (byte) 40,
    (byte) 41,
    (byte) 42,
    (byte) 43,
    (byte) 44
  };
  public static readonly byte[] ManifestVariantLogicalSlots = new byte[5]
  {
    (byte) 45,
    (byte) 46,
    (byte) 47,
    (byte) 48 /*0x30*/,
    (byte) 49
  };
  public static readonly byte[] RuntimeSurfaceLogicalSlots = new byte[10]
  {
    (byte) 50,
    (byte) 51,
    (byte) 52,
    (byte) 53,
    (byte) 54,
    (byte) 55,
    (byte) 56,
    (byte) 57,
    (byte) 58,
    (byte) 59
  };
  public static readonly byte[] AllLogicalSlots = new byte[47]
  {
    (byte) 1,
    (byte) 2,
    (byte) 3,
    (byte) 4,
    (byte) 5,
    (byte) 6,
    (byte) 7,
    (byte) 8,
    (byte) 9,
    (byte) 10,
    (byte) 11,
    (byte) 12,
    (byte) 13,
    (byte) 14,
    (byte) 15,
    (byte) 20,
    (byte) 21,
    (byte) 22,
    (byte) 23,
    (byte) 24,
    (byte) 25,
    (byte) 26,
    (byte) 27,
    (byte) 28,
    (byte) 29,
    (byte) 30,
    (byte) 31 /*0x1F*/,
    (byte) 40,
    (byte) 41,
    (byte) 42,
    (byte) 43,
    (byte) 44,
    (byte) 45,
    (byte) 46,
    (byte) 47,
    (byte) 48 /*0x30*/,
    (byte) 49,
    (byte) 50,
    (byte) 51,
    (byte) 52,
    (byte) 53,
    (byte) 54,
    (byte) 55,
    (byte) 56,
    (byte) 57,
    (byte) 58,
    (byte) 59
  };

  public static IReadOnlyList<string> GetPackagedManifestFragmentPaths(string? buildVersion)
  {
    string str = RMCWeaponProfileProbeCatalog.NormalizeBuildVersion(buildVersion);
    string[] manifestFragmentPaths = new string[RMCWeaponProfileProbeCatalog.PackagedManifestFragmentDirectories.Length];
    for (int index = 0; index < manifestFragmentPaths.Length; ++index)
    {
      string hexLower = RMCWeaponProfileDigest.ToHexLower((ReadOnlySpan<byte>) RMCWeaponProfileDigest.ComputeDigest($"{str}|manifest-fragment:{index}|rv4").AsSpan<byte>(0, 6));
      manifestFragmentPaths[index] = $"{RMCWeaponProfileProbeCatalog.PackagedManifestFragmentDirectories[index]}/{RMCWeaponProfileProbeCatalog.PackagedManifestFragmentPrefixes[index]}_{hexLower}.txt";
    }
    return (IReadOnlyList<string>) manifestFragmentPaths;
  }

  public static IReadOnlyList<string> GetIntegrityBenignResourceProbePaths()
  {
    return (IReadOnlyList<string>) RMCWeaponProfileProbeCatalog.IntegrityBenignResourceProbePathsValue;
  }

  public static Dictionary<byte, byte> BuildOpaqueProbeMap(
    string? buildVersion,
    IReadOnlyList<byte>? logicalSlots = null)
  {
    IReadOnlyList<byte> byteList = (IReadOnlyList<byte>) ((object) logicalSlots ?? (object) RMCWeaponProfileProbeCatalog.AllLogicalSlots);
    string str = RMCWeaponProfileProbeCatalog.NormalizeBuildVersion(buildVersion);
    HashSet<byte> used = new HashSet<byte>();
    Dictionary<byte, byte> dictionary = new Dictionary<byte, byte>(byteList.Count);
    for (int index = 0; index < byteList.Count; ++index)
    {
      byte key = byteList[index];
      byte num = RMCWeaponProfileProbeCatalog.ResolveOpaqueProbeId((ReadOnlySpan<byte>) RMCWeaponProfileDigest.ComputeDigest($"{str}|{key}|rv2"), (ISet<byte>) used);
      used.Add(num);
      dictionary[key] = num;
    }
    return dictionary;
  }

  public static Dictionary<byte, byte> BuildLogicalProbeMap(
    string? buildVersion,
    IReadOnlyList<byte>? logicalSlots = null)
  {
    Dictionary<byte, byte> dictionary1 = RMCWeaponProfileProbeCatalog.BuildOpaqueProbeMap(buildVersion, logicalSlots);
    Dictionary<byte, byte> dictionary2 = new Dictionary<byte, byte>(dictionary1.Count);
    foreach ((byte key1, byte key2) in dictionary1)
      dictionary2[key2] = key1;
    return dictionary2;
  }

  private static byte ResolveOpaqueProbeId(ReadOnlySpan<byte> digest, ISet<byte> used)
  {
    for (int index = 0; index < digest.Length; ++index)
    {
      byte num = (byte) (1 + (int) digest[index] % 250);
      if (!used.Contains(num))
        return num;
    }
    for (int index = 1; index <= 250; ++index)
    {
      if (!used.Contains((byte) index))
        return (byte) index;
    }
    return 251;
  }

  public static string NormalizeBuildVersion(string? buildVersion)
  {
    return string.IsNullOrWhiteSpace(buildVersion) ? "unknown" : buildVersion.Trim();
  }

  public static uint RollLiveness(uint current, int sequence)
  {
    int num1 = ((int) current ^ sequence - 1640531527) * -1640531535;
    int num2 = (num1 ^ num1 >>> 15) * -2048144777;
    return (uint) (num2 ^ num2 >>> 13);
  }
}
