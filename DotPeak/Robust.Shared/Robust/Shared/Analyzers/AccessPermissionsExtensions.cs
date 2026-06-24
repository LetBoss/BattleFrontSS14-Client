// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Analyzers.AccessPermissionsExtensions
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Analyzers;

public static class AccessPermissionsExtensions
{
  public static string ToUnixPermissions(this AccessPermissions permissions)
  {
    switch (permissions)
    {
      case AccessPermissions.None:
        return "---";
      case AccessPermissions.Read:
        return "r--";
      case AccessPermissions.Write:
        return "-w-";
      case AccessPermissions.ReadWrite:
        return "rw-";
      case AccessPermissions.Execute:
        return "--x";
      case AccessPermissions.ReadExecute:
        return "r-x";
      case AccessPermissions.WriteExecute:
        return "-wx";
      case AccessPermissions.ReadWriteExecute:
        return "rwx";
      default:
        throw new ArgumentOutOfRangeException(nameof (permissions), (object) permissions, (string) null);
    }
  }
}
