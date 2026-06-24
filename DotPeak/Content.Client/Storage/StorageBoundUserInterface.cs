// Decompiled with JetBrains decompiler
// Type: Content.Client.Storage.StorageBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Systems.Storage;
using Content.Client.UserInterface.Systems.Storage.Controls;
using Content.Shared.Storage;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Storage;

public sealed class StorageBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private StorageWindow? _window;

  public Vector2? Position => ((Control) this._window)?.Position;

  protected virtual void Open()
  {
    base.Open();
    this._window = IoCManager.Resolve<IUserInterfaceManager>().GetUIController<StorageUIController>().CreateStorageWindow(this);
    StorageComponent storageComponent;
    if (this.EntMan.TryGetComponent<StorageComponent>(this.Owner, ref storageComponent))
      this._window.UpdateContainer(new Entity<StorageComponent>?(Entity<StorageComponent>.op_Implicit((this.Owner, storageComponent))));
    this._window.OnClose += new Action(((BoundUserInterface) this).Close);
    this._window.FlagDirty();
  }

  public void Refresh() => this._window?.FlagDirty();

  public void Reclaim()
  {
    if (this._window == null)
      return;
    this._window.OnClose -= new Action(((BoundUserInterface) this).Close);
    ((Control) this._window).Orphan();
    this._window = (StorageWindow) null;
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    this.Reclaim();
  }

  public void CloseWindow(Vector2 position)
  {
    if (this._window == null)
      return;
    LayoutContainer.SetPosition((Control) this._window, position);
    this._window?.Close();
  }

  public void Hide()
  {
    if (this._window == null)
      return;
    ((Control) this._window).Visible = false;
  }

  public void Show()
  {
    if (this._window == null)
      return;
    ((Control) this._window).Visible = true;
  }

  public void Show(Vector2 position)
  {
    if (this._window == null)
      return;
    this.Show();
    LayoutContainer.SetPosition((Control) this._window, position);
  }
}
