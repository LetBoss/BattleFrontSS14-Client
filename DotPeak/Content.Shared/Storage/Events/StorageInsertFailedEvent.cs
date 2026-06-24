// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.Events.StorageInsertFailedEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Storage.Events;

[ByRefEvent]
public record struct StorageInsertFailedEvent(
  Entity<StorageComponent?> Storage,
  Entity<HandsComponent?> Player)
;
