// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.Notes.UserNotesEuiState
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
public sealed class UserNotesEuiState : EuiStateBase
{
  public UserNotesEuiState(Dictionary<(int, NoteType), SharedAdminNote> notes)
  {
    this.Notes = notes;
  }

  public Dictionary<(int, NoteType), SharedAdminNote> Notes { get; }
}
