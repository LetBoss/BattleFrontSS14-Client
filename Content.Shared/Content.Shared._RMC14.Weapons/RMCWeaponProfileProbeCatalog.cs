using System;
using System.Collections.Generic;

namespace Content.Shared._RMC14.Weapons;

public static class RMCWeaponProfileProbeCatalog
{
	private static readonly string[] PackagedManifestFragmentDirectories = new string[3] { "/Textures/Generated", "/Audio/Generated", "/Generated/Layout" };

	private static readonly string[] PackagedManifestFragmentPrefixes = new string[3] { "atlas", "cache", "layout" };

	private static readonly string[] IntegrityBenignResourceProbePathsValue = new string[3] { "/Textures/Shaders/hcut.swsl", "/Textures/Shaders/outline.swsl", "/Prototypes/audio.yml" };

	public static readonly byte[] MethodLogicalSlots = new byte[15]
	{
		1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
		11, 12, 13, 14, 15
	};

	public static readonly byte[] CatalogSummaryLogicalSlots = new byte[6] { 20, 21, 22, 23, 24, 25 };

	public static readonly byte[] CatalogSummaryVariantLogicalSlots = new byte[6] { 26, 27, 28, 29, 30, 31 };

	public static readonly byte[] ManifestLogicalSlots = new byte[5] { 40, 41, 42, 43, 44 };

	public static readonly byte[] ManifestVariantLogicalSlots = new byte[5] { 45, 46, 47, 48, 49 };

	public static readonly byte[] RuntimeSurfaceLogicalSlots = new byte[10] { 50, 51, 52, 53, 54, 55, 56, 57, 58, 59 };

	public static readonly byte[] AllLogicalSlots = new byte[47]
	{
		1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
		11, 12, 13, 14, 15, 20, 21, 22, 23, 24,
		25, 26, 27, 28, 29, 30, 31, 40, 41, 42,
		43, 44, 45, 46, 47, 48, 49, 50, 51, 52,
		53, 54, 55, 56, 57, 58, 59
	};

	public static IReadOnlyList<string> GetPackagedManifestFragmentPaths(string? buildVersion)
	{
		string normalizedBuild = NormalizeBuildVersion(buildVersion);
		string[] paths = new string[PackagedManifestFragmentDirectories.Length];
		for (int i = 0; i < paths.Length; i++)
		{
			string token = RMCWeaponProfileDigest.ToHexLower(RMCWeaponProfileDigest.ComputeDigest($"{normalizedBuild}|manifest-fragment:{i}|rv4").AsSpan(0, 6));
			paths[i] = $"{PackagedManifestFragmentDirectories[i]}/{PackagedManifestFragmentPrefixes[i]}_{token}.txt";
		}
		return paths;
	}

	public static IReadOnlyList<string> GetIntegrityBenignResourceProbePaths()
	{
		return IntegrityBenignResourceProbePathsValue;
	}

	public static Dictionary<byte, byte> BuildOpaqueProbeMap(string? buildVersion, IReadOnlyList<byte>? logicalSlots = null)
	{
		IReadOnlyList<byte> slots = logicalSlots ?? AllLogicalSlots;
		string normalizedBuild = NormalizeBuildVersion(buildVersion);
		HashSet<byte> used = new HashSet<byte>();
		Dictionary<byte, byte> map = new Dictionary<byte, byte>(slots.Count);
		for (int i = 0; i < slots.Count; i++)
		{
			byte logicalSlot = slots[i];
			byte opaqueId = ResolveOpaqueProbeId(RMCWeaponProfileDigest.ComputeDigest($"{normalizedBuild}|{logicalSlot}|rv2"), used);
			used.Add(opaqueId);
			map[logicalSlot] = opaqueId;
		}
		return map;
	}

	public static Dictionary<byte, byte> BuildLogicalProbeMap(string? buildVersion, IReadOnlyList<byte>? logicalSlots = null)
	{
		Dictionary<byte, byte> dictionary = BuildOpaqueProbeMap(buildVersion, logicalSlots);
		Dictionary<byte, byte> logicalMap = new Dictionary<byte, byte>(dictionary.Count);
		foreach (KeyValuePair<byte, byte> item in dictionary)
		{
			item.Deconstruct(out var key, out var value);
			byte logicalSlot = key;
			byte opaqueId = value;
			logicalMap[opaqueId] = logicalSlot;
		}
		return logicalMap;
	}

	private static byte ResolveOpaqueProbeId(ReadOnlySpan<byte> digest, ISet<byte> used)
	{
		for (int i = 0; i < digest.Length; i++)
		{
			byte candidate = (byte)(1 + digest[i] % 250);
			if (!used.Contains(candidate))
			{
				return candidate;
			}
		}
		for (int j = 1; j <= 250; j++)
		{
			if (!used.Contains((byte)j))
			{
				return (byte)j;
			}
		}
		return 251;
	}

	public static string NormalizeBuildVersion(string? buildVersion)
	{
		if (string.IsNullOrWhiteSpace(buildVersion))
		{
			return "unknown";
		}
		return buildVersion.Trim();
	}

	public static uint RollLiveness(uint current, int sequence)
	{
		int num = (int)(current ^ (uint)(sequence + -1640531527)) * -1640531535;
		int num2 = (num ^ (num >>> 15)) * -2048144777;
		return (uint)(num2 ^ (num2 >>> 13));
	}
}
