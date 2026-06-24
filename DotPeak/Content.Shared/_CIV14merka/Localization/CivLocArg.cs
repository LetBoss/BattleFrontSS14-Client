// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Localization.CivLocArg
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._CIV14merka.Localization;

[NetSerializable]
[Serializable]
public struct CivLocArg
{
  public string Name;
  public string Value;
  public bool IsLoc;
  public CivLocMessage? Sub;

  public static CivLocArg Text(string name, object? value)
  {
    return new CivLocArg()
    {
      Name = name,
      Value = value?.ToString() ?? string.Empty,
      IsLoc = false
    };
  }

  public static CivLocArg LocRef(string name, string locId)
  {
    return new CivLocArg()
    {
      Name = name,
      Value = locId,
      IsLoc = true
    };
  }

  public static CivLocArg Msg(string name, CivLocMessage sub)
  {
    return new CivLocArg() { Name = name, Sub = sub };
  }

  public object ResolveValue()
  {
    if (this.Sub != null)
      return (object) this.Sub.Resolve();
    return !this.IsLoc ? (object) this.Value : (object) Loc.GetString(this.Value);
  }
}
