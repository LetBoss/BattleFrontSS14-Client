using System.Collections.Generic;

namespace Robust.Shared;

public sealed class MountOptions
{
	public List<string> ZipMounts = new List<string>();

	public List<string> DirMounts = new List<string>();

	public MountOptions()
	{
	}

	public MountOptions(List<string> zipMounts, List<string> dirMounts)
	{
		ZipMounts = zipMounts;
		DirMounts = dirMounts;
	}

	public static MountOptions Merge(MountOptions a, MountOptions b)
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		list.AddRange(a.ZipMounts);
		list.AddRange(b.ZipMounts);
		list2.AddRange(a.DirMounts);
		list2.AddRange(b.DirMounts);
		return new MountOptions(list, list2);
	}
}
