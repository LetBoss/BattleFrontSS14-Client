// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Prototypes.CMPrototypeExtensions
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Prototypes;

public static class CMPrototypeExtensions
{
  public static bool FilterCM = true;

  public static IEnumerable<T> EnumerateCM<T>(this IPrototypeManager prototypes) where T : class, IPrototype, ICMSpecific
  {
    IEnumerable<T> source = prototypes.EnumeratePrototypes<T>();
    if (CMPrototypeExtensions.FilterCM)
      source = source.Where<T>((Func<T, bool>) (p => p.IsCM));
    return source;
  }

  public static bool TryCM<T>(this IPrototypeManager prototypes, string id, [NotNullWhen(true)] out T? prototype) where T : class, IPrototype, ICMSpecific
  {
    prototype = default (T);
    T prototype1;
    if (!prototypes.TryIndex<T>(id, out prototype1) || CMPrototypeExtensions.FilterCM && !prototype1.IsCM)
      return false;
    prototype = prototype1;
    return true;
  }
}
