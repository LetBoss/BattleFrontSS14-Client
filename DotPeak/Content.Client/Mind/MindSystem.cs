// Decompiled with JetBrains decompiler
// Type: Content.Client.Mind.MindSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Mind;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Mind;

public sealed class MindSystem : SharedMindSystem
{
  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MindComponent, AfterAutoHandleStateEvent>(new ComponentEventRefHandler<MindComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnHandleState(
    EntityUid uid,
    MindComponent component,
    ref AfterAutoHandleStateEvent args)
  {
    foreach ((NetUserId key, EntityUid entityUid) in this.UserMinds)
    {
      if (EntityUid.op_Equality(entityUid, uid))
        this.UserMinds.Remove(key);
    }
    NetUserId? userId = component.UserId;
    if (!userId.HasValue)
      return;
    Dictionary<NetUserId, EntityUid> userMinds = this.UserMinds;
    userId = component.UserId;
    NetUserId key1 = userId.Value;
    EntityUid entityUid1 = uid;
    userMinds[key1] = entityUid1;
  }
}
