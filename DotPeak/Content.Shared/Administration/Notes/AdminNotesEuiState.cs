// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.Notes.AdminNotesEuiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Administration.Notes;

[NetSerializable]
[Serializable]
public sealed class AdminNotesEuiState : EuiStateBase
{
  public AdminNotesEuiState(
    string notedPlayerName,
    Dictionary<(int, NoteType), SharedAdminNote> notes,
    bool canCreate,
    bool canDelete,
    bool canEdit)
  {
    this.NotedPlayerName = notedPlayerName;
    this.Notes = notes;
    this.CanCreate = canCreate;
    this.CanDelete = canDelete;
    this.CanEdit = canEdit;
  }

  public string NotedPlayerName { get; }

  public Dictionary<(int noteId, NoteType noteType), SharedAdminNote> Notes { get; }

  public bool CanCreate { get; }

  public bool CanDelete { get; }

  public bool CanEdit { get; }
}
