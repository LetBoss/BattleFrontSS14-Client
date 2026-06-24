// Decompiled with JetBrains decompiler
// Type: Content.Client.Singularity.Systems.SingularitySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Singularity.Components;
using Content.Shared.Singularity.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using System;

#nullable enable
namespace Content.Client.Singularity.Systems;

public sealed class SingularitySystem : SharedSingularitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SingularityComponent, ComponentHandleState>(new ComponentEventRefHandler<SingularityComponent, ComponentHandleState>((object) this, __methodptr(HandleSingularityState)), (Type[]) null, (Type[]) null);
  }

  private void HandleSingularityState(
    EntityUid uid,
    SingularityComponent comp,
    ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is SharedSingularitySystem.SingularityComponentState current))
      return;
    this.SetLevel(uid, current.Level, comp);
  }
}
