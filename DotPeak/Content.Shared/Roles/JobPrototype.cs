// Decompiled with JetBrains decompiler
// Type: Content.Shared.Roles.JobUIComparer
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Roles;

public sealed class JobUIComparer : IComparer<JobPrototype>
{
  public static readonly JobUIComparer Instance = new JobUIComparer();

  public int Compare(JobPrototype? x, JobPrototype? y)
  {
    if (x == y)
      return 0;
    if (y == null)
      return 1;
    if (x == null)
      return -1;
    int num = -x.RealDisplayWeight.CompareTo(y.RealDisplayWeight);
    return num != 0 ? num : string.Compare(x.ID, y.ID, StringComparison.Ordinal);
  }
}
