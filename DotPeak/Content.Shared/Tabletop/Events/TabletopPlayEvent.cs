// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tabletop.Events.TabletopPlayEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Tabletop.Events;

[NetSerializable]
[Serializable]
public sealed class TabletopPlayEvent : EntityEventArgs
{
  public NetEntity TableUid;
  public NetEntity CameraUid;
  public string Title;
  public Vector2i Size;

  public TabletopPlayEvent(NetEntity tableUid, NetEntity cameraUid, string title, Vector2i size)
  {
    this.TableUid = tableUid;
    this.CameraUid = cameraUid;
    this.Title = title;
    this.Size = size;
  }
}
