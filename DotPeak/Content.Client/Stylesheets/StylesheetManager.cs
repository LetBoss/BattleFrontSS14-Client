// Decompiled with JetBrains decompiler
// Type: Content.Client.Stylesheets.StylesheetManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.Stylesheets;

public sealed class StylesheetManager : IStylesheetManager
{
  [Dependency]
  private IUserInterfaceManager _userInterfaceManager;
  [Dependency]
  private IResourceCache _resourceCache;

  public Stylesheet SheetNano { get; private set; }

  public Stylesheet SheetSpace { get; private set; }

  public void Initialize()
  {
    this.SheetNano = new StyleNano(this._resourceCache).Stylesheet;
    this.SheetSpace = new StyleSpace(this._resourceCache).Stylesheet;
    this._userInterfaceManager.Stylesheet = this.SheetNano;
  }
}
