// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.Notes.SharedAdminNote
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Database;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Administration.Notes;

[NetSerializable]
[Serializable]
public sealed record SharedAdminNote(
  int Id,
  NetUserId Player,
  int? Round,
  string? ServerName,
  TimeSpan PlaytimeAtNote,
  NoteType NoteType,
  string Message,
  Content.Shared.Database.NoteSeverity? NoteSeverity,
  bool Secret,
  string CreatedByName,
  string EditedByName,
  DateTime CreatedAt,
  DateTime? LastEditedAt,
  DateTime? ExpiryTime,
  string[]? BannedRoles,
  DateTime? UnbannedTime,
  string? UnbannedByName,
  bool? Seen)
;
