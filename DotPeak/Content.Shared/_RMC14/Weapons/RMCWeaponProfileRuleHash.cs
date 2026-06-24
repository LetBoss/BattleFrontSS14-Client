// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.RMCWeaponProfileRuleHash
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

#nullable enable
namespace Content.Shared._RMC14.Weapons;

public static class RMCWeaponProfileRuleHash
{
  public static int Compute(string? value, int salt)
  {
    if (string.IsNullOrWhiteSpace(value))
      return 0;
    string str = value.Trim();
    uint num = (uint) (-2128831035 ^ salt);
    for (int index = 0; index < str.Length; ++index)
      num = (num ^ (uint) char.ToUpperInvariant(str[index])) * 16777619U;
    return (int) ((num ^ (uint) str.Length) * 16777619U) & int.MaxValue;
  }
}
