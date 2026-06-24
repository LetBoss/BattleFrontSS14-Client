// Decompiled with JetBrains decompiler
// Type: Content.Client.Fax.UI.FaxBoundUi
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Fax;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.IO;
using System.Runtime.ExceptionServices;

#nullable enable
namespace Content.Client.Fax.UI;

public sealed class FaxBoundUi(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Dependency]
  private IFileDialogManager _fileDialogManager;
  [Robust.Shared.ViewVariables.ViewVariables]
  private FaxWindow? _window;
  private bool _dialogIsOpen;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<FaxWindow>((BoundUserInterface) this);
    this._window.FileButtonPressed += new Action(this.OnFileButtonPressed);
    this._window.CopyButtonPressed += new Action(this.OnCopyButtonPressed);
    this._window.SendButtonPressed += new Action(this.OnSendButtonPressed);
    this._window.RefreshButtonPressed += new Action(this.OnRefreshButtonPressed);
    this._window.PeerSelected += new Action<string>(this.OnPeerSelected);
  }

  private async void OnFileButtonPressed()
  {
    FaxBoundUi faxBoundUi = this;
    if (faxBoundUi._dialogIsOpen)
      return;
    faxBoundUi._dialogIsOpen = true;
    FileDialogFilters fileDialogFilters = new FileDialogFilters(new FileDialogFilters.Group[1]
    {
      new FileDialogFilters.Group(new string[1]{ "txt" })
    });
    Stream file = await faxBoundUi._fileDialogManager.OpenFile(fileDialogFilters, FileAccess.ReadWrite, new FileShare?());
    object obj = (object) null;
    int num = 0;
    StreamReader reader;
    string firstLine;
    string label;
    try
    {
      faxBoundUi._dialogIsOpen = false;
      if (faxBoundUi._window != null && !((Control) faxBoundUi._window).Disposed && file != null)
      {
        reader = new StreamReader(file);
        try
        {
          firstLine = await reader.ReadLineAsync();
          label = (string) null;
          string str1 = await reader.ReadToEndAsync();
          if (firstLine != null)
          {
            if (firstLine.StartsWith('#'))
            {
              string str2 = firstLine;
              label = str2.Substring(1, str2.Length - 1).Trim();
            }
            else
              str1 = $"{firstLine}\n{str1}";
          }
          faxBoundUi.SendMessage((BoundUserInterfaceMessage) new FaxFileMessage(label?.Substring(0, Math.Min(label.Length, 50)), str1.Substring(0, Math.Min(str1.Length, 10000)), faxBoundUi._window.OfficePaper));
        }
        finally
        {
          reader?.Dispose();
        }
      }
      num = 1;
    }
    catch (object ex)
    {
      obj = ex;
    }
    if (file != null)
      await file.DisposeAsync();
    object obj1 = obj;
    if (obj1 != null)
    {
      if (!(obj1 is Exception source))
        throw obj1;
      ExceptionDispatchInfo.Capture(source).Throw();
    }
    if (num != 1)
    {
      obj = (object) null;
      file = (Stream) null;
      reader = (StreamReader) null;
      firstLine = (string) null;
      label = (string) null;
      throw null;
    }
  }

  private void OnSendButtonPressed()
  {
    this.SendMessage((BoundUserInterfaceMessage) new FaxSendMessage());
  }

  private void OnCopyButtonPressed()
  {
    this.SendMessage((BoundUserInterfaceMessage) new FaxCopyMessage());
  }

  private void OnRefreshButtonPressed()
  {
    this.SendMessage((BoundUserInterfaceMessage) new FaxRefreshMessage());
  }

  private void OnPeerSelected(string address)
  {
    this.SendMessage((BoundUserInterfaceMessage) new FaxDestinationMessage(address));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (this._window == null || !(state is FaxUiState state1))
      return;
    this._window.UpdateState(state1);
  }
}
