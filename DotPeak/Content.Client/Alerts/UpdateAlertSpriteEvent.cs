// Decompiled with JetBrains decompiler
// Type: Content.Client.Alerts.UpdateAlertSpriteEvent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Alert;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Alerts;

[ByRefEvent]
public record struct UpdateAlertSpriteEvent(
  Entity<SpriteComponent> spriteViewEnt,
  EntityUid viewerEnt,
  AlertPrototype alert)
{
  public Entity<SpriteComponent> SpriteViewEnt = spriteViewEnt;
  public EntityUid ViewerEnt = viewerEnt;
  public AlertPrototype Alert = alert;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return (EqualityComparer<Entity<SpriteComponent>>.Default.GetHashCode(this.SpriteViewEnt) * -1521134295 + EqualityComparer<EntityUid>.Default.GetHashCode(this.ViewerEnt)) * -1521134295 + EqualityComparer<AlertPrototype>.Default.GetHashCode(this.Alert);
  }

  [CompilerGenerated]
  public readonly bool Equals(UpdateAlertSpriteEvent other)
  {
    return EqualityComparer<Entity<SpriteComponent>>.Default.Equals(this.SpriteViewEnt, other.SpriteViewEnt) && EqualityComparer<EntityUid>.Default.Equals(this.ViewerEnt, other.ViewerEnt) && EqualityComparer<AlertPrototype>.Default.Equals(this.Alert, other.Alert);
  }
}
