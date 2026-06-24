// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.NamedItems.NamedItemsUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.LinkAccount;
using Content.Shared._RMC14.LinkAccount;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client._RMC14.NamedItems;

public sealed class NamedItemsUIController : UIController
{
  [Dependency]
  private LinkAccountManager _linkAccount;

  public bool Available
  {
    get
    {
      SharedRMCPatronTier tier = this._linkAccount.Tier;
      return (object) tier != null && tier.NamedItems;
    }
  }
}
