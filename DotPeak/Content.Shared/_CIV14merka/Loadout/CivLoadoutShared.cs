// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Loadout.CivLoadoutItemInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

#nullable enable
namespace Content.Shared._CIV14merka.Loadout;

public sealed class CivLoadoutItemInfo
{
  public readonly string Slot;
  public readonly string Proto;
  public readonly int Count;

  public CivLoadoutItemInfo(string slot, string proto, int count)
  {
    this.Slot = slot;
    this.Proto = proto;
    this.Count = count;
  }

  public string Key => CivLoadoutKeys.Item(this.Slot, this.Proto);
}
