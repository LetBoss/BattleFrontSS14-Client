// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleTurretSlotIds
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public static class VehicleTurretSlotIds
{
  public const string Separator = "::";

  public static string Compose(string parentSlotId, string childSlotId)
  {
    return $"{parentSlotId}::{childSlotId}";
  }

  public static bool TryParse(string slotId, out string parentSlotId, out string childSlotId)
  {
    parentSlotId = string.Empty;
    childSlotId = string.Empty;
    if (string.IsNullOrWhiteSpace(slotId))
      return false;
    int length = slotId.IndexOf("::", StringComparison.Ordinal);
    if (length <= 0 || length >= slotId.Length - "::".Length)
      return false;
    parentSlotId = slotId.Substring(0, length);
    ref string local = ref childSlotId;
    string str1 = slotId;
    int startIndex = length + "::".Length;
    string str2 = str1.Substring(startIndex, str1.Length - startIndex);
    local = str2;
    return !string.IsNullOrWhiteSpace(parentSlotId) && !string.IsNullOrWhiteSpace(childSlotId);
  }
}
