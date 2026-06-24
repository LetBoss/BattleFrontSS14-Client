// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.Notes.AdminNoteEuiMsg
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Administration.Notes;

public static class AdminNoteEuiMsg
{
  [NetSerializable]
  [Serializable]
  public sealed class CreateNoteRequest : EuiMessageBase
  {
    public CreateNoteRequest(
      NoteType type,
      string message,
      NoteSeverity? severity,
      bool secret,
      DateTime? expiryTime)
    {
      this.NoteType = type;
      this.Message = message;
      this.NoteSeverity = severity;
      this.Secret = secret;
      this.ExpiryTime = expiryTime;
    }

    public NoteType NoteType { get; set; }

    public string Message { get; set; }

    public NoteSeverity? NoteSeverity { get; set; }

    public bool Secret { get; set; }

    public DateTime? ExpiryTime { get; set; }
  }

  [NetSerializable]
  [Serializable]
  public sealed class DeleteNoteRequest : EuiMessageBase
  {
    public DeleteNoteRequest(int id, NoteType type)
    {
      this.Id = id;
      this.Type = type;
    }

    public int Id { get; set; }

    public NoteType Type { get; set; }
  }

  [NetSerializable]
  [Serializable]
  public sealed class EditNoteRequest : EuiMessageBase
  {
    public EditNoteRequest(
      int id,
      NoteType type,
      string message,
      NoteSeverity? severity,
      bool secret,
      DateTime? expiryTime)
    {
      this.Id = id;
      this.Type = type;
      this.Message = message;
      this.NoteSeverity = severity;
      this.Secret = secret;
      this.ExpiryTime = expiryTime;
    }

    public int Id { get; set; }

    public NoteType Type { get; set; }

    public string Message { get; set; }

    public NoteSeverity? NoteSeverity { get; set; }

    public bool Secret { get; set; }

    public DateTime? ExpiryTime { get; set; }
  }
}
