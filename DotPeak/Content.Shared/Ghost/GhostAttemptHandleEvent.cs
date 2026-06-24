// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ghost.GhostAttemptHandleEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Mind;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Ghost;

public sealed class GhostAttemptHandleEvent(MindComponent mind, bool canReturnGlobal) : 
  HandledEntityEventArgs
{
  public MindComponent Mind { get; } = mind;

  public bool CanReturnGlobal { get; } = canReturnGlobal;

  public bool Result { get; set; }
}
