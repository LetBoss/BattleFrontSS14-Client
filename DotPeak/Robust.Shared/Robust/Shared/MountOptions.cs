// Decompiled with JetBrains decompiler
// Type: Robust.Shared.MountOptions
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;

#nullable enable
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
    this.ZipMounts = zipMounts;
    this.DirMounts = dirMounts;
  }

  public static MountOptions Merge(MountOptions a, MountOptions b)
  {
    List<string> zipMounts = new List<string>();
    List<string> dirMounts = new List<string>();
    zipMounts.AddRange((IEnumerable<string>) a.ZipMounts);
    zipMounts.AddRange((IEnumerable<string>) b.ZipMounts);
    dirMounts.AddRange((IEnumerable<string>) a.DirMounts);
    dirMounts.AddRange((IEnumerable<string>) b.DirMounts);
    return new MountOptions(zipMounts, dirMounts);
  }
}
