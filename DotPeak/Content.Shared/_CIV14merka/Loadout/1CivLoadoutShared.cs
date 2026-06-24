// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Loadout.CivLoadoutKeys
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._CIV14merka.Teams;

#nullable enable
namespace Content.Shared._CIV14merka.Loadout;

public static class CivLoadoutKeys
{
  public const string BonusSlot = "bonus";

  public static string Item(string slot, string proto) => $"{slot}:{proto}";

  public static string Combo(string faction, CivTdmClass cls) => $"{faction}:{cls.ToString()}";

  public static bool TryParseItem(string key, out string slot, out string proto)
  {
    int length = key.IndexOf(':');
    if (length <= 0 || length >= key.Length - 1)
    {
      slot = string.Empty;
      proto = string.Empty;
      return false;
    }
    slot = key.Substring(0, length);
    proto = key.Substring(length + 1);
    return true;
  }

  public static string SlotChoice(string slot, string proto) => $"{slot}={proto}";

  public static bool TryParseSlotChoice(string entry, out string slot, out string proto)
  {
    int length = entry.IndexOf('=');
    if (length <= 0 || length >= entry.Length - 1)
    {
      slot = string.Empty;
      proto = string.Empty;
      return false;
    }
    slot = entry.Substring(0, length);
    proto = entry.Substring(length + 1);
    return true;
  }

  public static string Owned(string faction, int count, string proto)
  {
    return $"{faction}|{count}|{proto}";
  }

  public static bool TryParseOwned(
    string entry,
    out string faction,
    out int count,
    out string proto)
  {
    faction = string.Empty;
    count = 0;
    proto = string.Empty;
    int length = entry.IndexOf('|');
    if (length < 0)
    {
      proto = entry;
      return !string.IsNullOrEmpty(proto);
    }
    int num = entry.IndexOf('|', length + 1);
    if (num < 0)
    {
      faction = entry.Substring(0, length);
      proto = entry.Substring(length + 1);
      return !string.IsNullOrEmpty(proto);
    }
    faction = entry.Substring(0, length);
    int.TryParse(entry.Substring(length + 1, num - length - 1), out count);
    proto = entry.Substring(num + 1);
    return !string.IsNullOrEmpty(proto);
  }
}
