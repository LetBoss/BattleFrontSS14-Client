// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.AdminFlagsHelper
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared.Administration;

public static class AdminFlagsHelper
{
  private static readonly Dictionary<string, AdminFlags> NameFlagsMap = new Dictionary<string, AdminFlags>();
  private static readonly string[] FlagsNameMap = new string[32 /*0x20*/];
  public static readonly AdminFlags Everything;
  public static readonly IReadOnlyList<AdminFlags> AllFlags;

  static AdminFlagsHelper()
  {
    AdminFlags[] values = (AdminFlags[]) Enum.GetValues(typeof (AdminFlags));
    List<AdminFlags> adminFlagsList = new List<AdminFlags>();
    foreach (AdminFlags adminFlags in values)
    {
      string upper = adminFlags.ToString().ToUpper();
      if (BitOperations.PopCount((uint) adminFlags) == 1)
      {
        adminFlagsList.Add(adminFlags);
        AdminFlagsHelper.Everything |= adminFlags;
        AdminFlagsHelper.NameFlagsMap.Add(upper, adminFlags);
        AdminFlagsHelper.FlagsNameMap[BitOperations.Log2((uint) adminFlags)] = upper;
      }
    }
    AdminFlagsHelper.AllFlags = (IReadOnlyList<AdminFlags>) adminFlagsList.ToArray();
  }

  public static AdminFlags NamesToFlags(IEnumerable<string> names)
  {
    AdminFlags flags = AdminFlags.None;
    foreach (string name in names)
    {
      AdminFlags adminFlags;
      if (!AdminFlagsHelper.NameFlagsMap.TryGetValue(name, out adminFlags))
        throw new ArgumentException("Invalid admin flag name: " + name);
      flags |= adminFlags;
    }
    return flags;
  }

  public static AdminFlags NameToFlag(string name) => AdminFlagsHelper.NameFlagsMap[name];

  public static string[] FlagsToNames(AdminFlags flags)
  {
    string[] names = new string[BitOperations.PopCount((uint) flags)];
    int num1 = BitOperations.LeadingZeroCount((uint) flags);
    int num2 = 0;
    for (int index = 0; index < 32 /*0x20*/ - num1; ++index)
    {
      AdminFlags adminFlags = (AdminFlags) (1 << index);
      if ((flags & adminFlags) != AdminFlags.None)
        names[num2++] = AdminFlagsHelper.FlagsNameMap[index];
    }
    return names;
  }

  public static string PosNegFlagsText(AdminFlags posFlags, AdminFlags negFlags)
  {
    return string.Join<string>(' ', ((IEnumerable<string>) AdminFlagsHelper.FlagsToNames(posFlags)).Select<string, (string, string)>((Func<string, (string, string)>) (f => (f, "+" + f))).Concat<(string, string)>(((IEnumerable<string>) AdminFlagsHelper.FlagsToNames(negFlags)).Select<string, (string, string)>((Func<string, (string, string)>) (f => (f, "-" + f)))).OrderBy<(string, string), string>((Func<(string, string), string>) (f => f.flag)).Select<(string, string), string>((Func<(string, string), string>) (p => p.fText)));
  }
}
