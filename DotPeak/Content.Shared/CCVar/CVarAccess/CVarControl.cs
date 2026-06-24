// Decompiled with JetBrains decompiler
// Type: Content.Shared.CCVar.CVarAccess.CVarControl
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration;
using Robust.Shared.Reflection;
using System;

#nullable enable
namespace Content.Shared.CCVar.CVarAccess;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
[Reflect(true)]
public sealed class CVarControl : Attribute
{
  public AdminFlags AdminFlags { get; }

  public object? Min { get; }

  public object? Max { get; }

  public CVarControl(AdminFlags adminFlags, object? min = null, object? max = null, string? helpText = null)
  {
    this.AdminFlags = adminFlags;
    this.Min = min;
    this.Max = max;
    if (min != null && max != null && min.GetType() != max.GetType())
      throw new ArgumentException("Min and max must be of the same type.");
    if (min == null && max != null || min != null && max == null)
      throw new ArgumentException("Min and max must both be null or both be set.");
  }
}
