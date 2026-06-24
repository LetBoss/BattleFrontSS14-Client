// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.BuiPreTickUpdateSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.UserInterface;

public sealed class BuiPreTickUpdateSystem : EntitySystem
{
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private UserInterfaceSystem _uiSystem;
  [Dependency]
  private IGameTiming _gameTiming;
  private EntityQuery<UserInterfaceUserComponent> _userQuery;

  public virtual void Initialize()
  {
    base.Initialize();
    this._userQuery = this.GetEntityQuery<UserInterfaceUserComponent>();
  }

  public void RunUpdates()
  {
    if (!this._gameTiming.IsFirstTimePredicted)
      return;
    EntityUid? attachedEntity = (EntityUid?) ((ISharedPlayerManager) this._playerManager).LocalSession?.AttachedEntity;
    UserInterfaceUserComponent interfaceUserComponent;
    if (!attachedEntity.HasValue || !this._userQuery.TryGetComponent(attachedEntity.GetValueOrDefault(), ref interfaceUserComponent))
      return;
    foreach ((EntityUid key, List<Enum> enumList) in interfaceUserComponent.OpenInterfaces)
    {
      foreach (Enum @enum in enumList)
      {
        BoundUserInterface boundUserInterface;
        if (((SharedUserInterfaceSystem) this._uiSystem).TryGetOpenUi(Entity<UserInterfaceComponent>.op_Implicit(key), @enum, ref boundUserInterface) && boundUserInterface is IBuiPreTickUpdate buiPreTickUpdate)
          buiPreTickUpdate.PreTickUpdate();
      }
    }
  }
}
