// Decompiled with JetBrains decompiler
// Type: Content.Client.VoiceMask.VoiceMaskBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.VoiceMask;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.VoiceMask;

public sealed class VoiceMaskBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Dependency]
  private IPrototypeManager _protomanager;
  [Robust.Shared.ViewVariables.ViewVariables]
  private VoiceMaskNameChangeWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<VoiceMaskNameChangeWindow>((BoundUserInterface) this);
    this._window.ReloadVerbs(this._protomanager);
    this._window.AddVerbs();
    this._window.OnNameChange += new Action<string>(this.OnNameSelected);
    this._window.OnVerbChange += (Action<string>) (verb => this.SendMessage((BoundUserInterfaceMessage) new VoiceMaskChangeVerbMessage(verb)));
  }

  private void OnNameSelected(string name)
  {
    this.SendMessage((BoundUserInterfaceMessage) new VoiceMaskChangeNameMessage(name));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    if (!(state is VoiceMaskBuiState voiceMaskBuiState) || this._window == null)
      return;
    this._window.UpdateState(voiceMaskBuiState.Name, voiceMaskBuiState.Verb);
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    this._window?.Close();
  }
}
