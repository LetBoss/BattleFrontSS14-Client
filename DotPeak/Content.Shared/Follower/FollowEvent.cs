// Decompiled with JetBrains decompiler
// Type: Content.Shared.Follower.FollowEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;

#nullable disable
namespace Content.Shared.Follower;

public abstract class FollowEvent : EntityEventArgs
{
  public EntityUid Following;
  public EntityUid Follower;

  protected FollowEvent(EntityUid following, EntityUid follower)
  {
    this.Following = following;
    this.Follower = follower;
  }
}
