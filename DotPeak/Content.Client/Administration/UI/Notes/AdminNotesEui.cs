// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.UI.Notes.AdminNotesEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eui;
using Content.Shared.Administration.Notes;
using Content.Shared.Database;
using Content.Shared.Eui;
using System;

#nullable enable
namespace Content.Client.Administration.UI.Notes;

public sealed class AdminNotesEui : BaseEui
{
  public AdminNotesEui()
  {
    this.NoteWindow = new AdminNotesWindow();
    this.NoteControl = this.NoteWindow.Notes;
    this.NoteControl.NoteChanged += (Action<int, NoteType, string, NoteSeverity?, bool, DateTime?>) ((id, type, text, severity, secret, expiryTime) => this.SendMessage((EuiMessageBase) new AdminNoteEuiMsg.EditNoteRequest(id, type, text, severity, secret, expiryTime)));
    this.NoteControl.NewNoteEntered += (Action<NoteType, string, NoteSeverity?, bool, DateTime?>) ((type, text, severity, secret, expiryTime) => this.SendMessage((EuiMessageBase) new AdminNoteEuiMsg.CreateNoteRequest(type, text, severity, secret, expiryTime)));
    this.NoteControl.NoteDeleted += (Action<int, NoteType>) ((id, type) => this.SendMessage((EuiMessageBase) new AdminNoteEuiMsg.DeleteNoteRequest(id, type)));
    this.NoteWindow.OnClose += (Action) (() => this.SendMessage((EuiMessageBase) new CloseEuiMessage()));
  }

  public override void Closed()
  {
    base.Closed();
    this.NoteWindow.Close();
  }

  private AdminNotesWindow NoteWindow { get; }

  private AdminNotesControl NoteControl { get; }

  public override void HandleState(EuiStateBase state)
  {
    if (!(state is AdminNotesEuiState adminNotesEuiState))
      return;
    this.NoteWindow.SetTitlePlayer(adminNotesEuiState.NotedPlayerName);
    this.NoteControl.SetPlayerName(adminNotesEuiState.NotedPlayerName);
    this.NoteControl.SetNotes(adminNotesEuiState.Notes);
    this.NoteControl.SetPermissions(adminNotesEuiState.CanCreate, adminNotesEuiState.CanDelete, adminNotesEuiState.CanEdit);
  }

  public override void Opened() => this.NoteWindow.OpenCentered();
}
