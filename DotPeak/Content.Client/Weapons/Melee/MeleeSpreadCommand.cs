// Decompiled with JetBrains decompiler
// Type: Content.Client.Weapons.Melee.MeleeSpreadCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CombatMode;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.Weapons.Melee;

public sealed class MeleeSpreadCommand : LocalizedEntityCommands
{
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IOverlayManager _overlay;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private MeleeWeaponSystem _meleeSystem;
  [Dependency]
  private SharedCombatModeSystem _combatSystem;
  [Dependency]
  private SharedTransformSystem _transformSystem;

  public virtual string Command => "showmeleespread";

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (this._overlay.RemoveOverlay<MeleeArcOverlay>())
      return;
    this._overlay.AddOverlay((Overlay) new MeleeArcOverlay((IEntityManager) this.EntityManager, this._eyeManager, this._inputManager, this._playerManager, this._meleeSystem, this._combatSystem, this._transformSystem));
  }
}
