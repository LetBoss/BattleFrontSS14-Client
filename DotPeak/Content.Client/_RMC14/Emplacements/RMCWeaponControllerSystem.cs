// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Emplacements.RMCWeaponControllerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Emplacements;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Client._RMC14.Emplacements;

public sealed class RMCWeaponControllerSystem : RMCSharedWeaponControllerSystem
{
  [Dependency]
  private IPlayerManager _playerManager;

  public bool TryGetControllingWeapon([NotNullWhen(true)] out EntityUid? weapon)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    weapon = new EntityUid?();
    return localEntity.HasValue && this.TryGetControlledWeapon(localEntity.Value, out weapon, out GunComponent _);
  }
}
