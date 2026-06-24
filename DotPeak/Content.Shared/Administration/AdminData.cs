// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.AdminData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

#nullable enable
namespace Content.Shared.Administration;

public sealed class AdminData
{
  public bool Active;
  public bool Stealth;
  public string? Title;
  public AdminFlags Flags;

  public bool HasFlag(AdminFlags flag, bool includeDeAdmin = false)
  {
    return (includeDeAdmin || this.Active) && (this.Flags & flag) == flag;
  }

  public bool CanAdminPlace() => this.HasFlag(AdminFlags.Spawn);

  public bool CanScript() => this.HasFlag(AdminFlags.Host);

  public bool CanAdminMenu() => this.HasFlag(AdminFlags.Admin);

  public bool CanStealth() => this.HasFlag(AdminFlags.Stealth);

  public bool CanAdminReloadPrototypes() => this.HasFlag(AdminFlags.Host);
}
