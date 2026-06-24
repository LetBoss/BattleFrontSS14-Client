// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.QuickDialogEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Administration;

[NetSerializable]
[Serializable]
public sealed class QuickDialogEntry
{
  public string FieldId;
  public QuickDialogEntryType Type;
  public string Prompt;
  public string? Placeholder;

  public QuickDialogEntry(
    string fieldId,
    QuickDialogEntryType type,
    string prompt,
    string? placeholder = null)
  {
    this.FieldId = fieldId;
    this.Type = type;
    this.Prompt = prompt;
    this.Placeholder = placeholder;
  }
}
