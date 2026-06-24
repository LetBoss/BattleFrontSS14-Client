// Decompiled with JetBrains decompiler
// Type: Content.Shared.Popups.PopupEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Popups;

[NetSerializable]
[Serializable]
public abstract class PopupEvent : EntityEventArgs
{
  public string Message { get; }

  public PopupType Type { get; }

  protected PopupEvent(string message, PopupType type)
  {
    this.Message = message;
    this.Type = type;
  }
}
