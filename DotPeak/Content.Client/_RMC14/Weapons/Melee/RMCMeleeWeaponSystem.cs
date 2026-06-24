// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Weapons.Melee.RMCMeleeWeaponSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Weapons.Melee;
using Content.Shared._RMC14.Input;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared.Weapons.Melee;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Player;

#nullable enable
namespace Content.Client._RMC14.Weapons.Melee;

public sealed class RMCMeleeWeaponSystem : SharedRMCMeleeWeaponSystem
{
  [Dependency]
  private IEyeManager _eye;
  [Dependency]
  private IInputManager _input;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private MapSystem _map;
  [Dependency]
  private MeleeWeaponSystem _melee;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private TransformSystem _transform;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(CMKeyFunctions.CMXenoWideSwing, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__7_0)), (StateInputCmdDelegate) null, false, true)).Register<RMCMeleeWeaponSystem>();
  }

  private void TryPrimaryHeavyAttack()
  {
    MapCoordinates map = this._eye.PixelToMap(this._input.MouseScreenPosition);
    EntityUid entityUid1;
    MapGridComponent mapGridComponent;
    EntityUid entityUid2;
    if (this._mapManager.TryFindGridAt(map, ref entityUid1, ref mapGridComponent))
    {
      entityUid2 = entityUid1;
    }
    else
    {
      EntityUid? nullable;
      if (!((SharedMapSystem) this._map).TryGetMap(new MapId?(map.MapId), ref nullable))
        return;
      entityUid2 = nullable.Value;
    }
    EntityCoordinates coordinates = ((SharedTransformSystem) this._transform).ToCoordinates(Entity<TransformComponent>.op_Implicit(entityUid2), map);
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid valueOrDefault = localEntity.GetValueOrDefault();
    EntityUid weaponUid;
    MeleeWeaponComponent melee;
    if (!this._melee.TryGetWeapon(valueOrDefault, out weaponUid, out melee) || !melee.WidePrimary)
      return;
    this._melee.ClientHeavyAttack(valueOrDefault, coordinates, weaponUid, melee);
  }
}
