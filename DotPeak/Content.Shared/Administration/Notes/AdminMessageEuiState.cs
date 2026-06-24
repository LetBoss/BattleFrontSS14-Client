// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.Notes.AdminMessageEuiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Eui;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Administration.Notes;

[NetSerializable]
[Serializable]
public sealed class AdminMessageEuiState(TimeSpan time, AdminMessageEuiState.Message[] messages) : 
  EuiStateBase
{
  public TimeSpan Time { get; } = time;

  public AdminMessageEuiState.Message[] Messages { get; } = messages;

  [Serializable]
  public sealed class Message(string text, string adminName, DateTime addedOn)
  {
    public string Text = text;
    public string AdminName = adminName;
    public DateTime AddedOn = addedOn;
  }
}
