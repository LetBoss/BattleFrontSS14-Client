// Decompiled with JetBrains decompiler
// Type: Content.Client.Revolutionary.UI.DeconvertedEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eui;

#nullable enable
namespace Content.Client.Revolutionary.UI;

public sealed class DeconvertedEui : BaseEui
{
  private readonly DeconvertedMenu _menu;

  public DeconvertedEui() => this._menu = new DeconvertedMenu();

  public override void Opened() => this._menu.OpenCentered();

  public override void Closed()
  {
    base.Closed();
    this._menu.Close();
  }
}
