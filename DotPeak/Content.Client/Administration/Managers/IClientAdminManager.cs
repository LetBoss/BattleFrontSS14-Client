// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.Managers.IClientAdminManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Administration;
using System;

#nullable enable
namespace Content.Client.Administration.Managers;

public interface IClientAdminManager
{
  event Action AdminStatusUpdated;

  AdminData? GetAdminData(bool includeDeAdmin = false);

  bool IsActive();

  bool HasFlag(AdminFlags flag);

  bool CanCommand(string cmdName);

  bool CanViewVar();

  bool CanAdminPlace();

  bool CanScript();

  bool CanAdminMenu();

  void Initialize();

  bool IsAdmin(bool includeDeAdmin = false) => this.GetAdminData(includeDeAdmin) != null;
}
