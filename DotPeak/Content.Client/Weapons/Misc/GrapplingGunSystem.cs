// Decompiled with JetBrains decompiler
// Type: Content.Client.Weapons.Misc.GrapplingGunSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Hands.Systems;
using Content.Shared.CombatMode;
using Content.Shared.Weapons.Misc;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics.Joints;
using Robust.Shared.Player;

#nullable enable
namespace Content.Client.Weapons.Misc;

public sealed class GrapplingGunSystem : SharedGrapplingGunSystem
{
  [Dependency]
  private HandsSystem _hands;
  [Dependency]
  private InputSystem _input;
  [Dependency]
  private IPlayerManager _player;

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this.Timing.IsFirstTimePredicted)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid? activeHandEntity = this._hands.GetActiveHandEntity();
    GrapplingGunComponent grapplingGunComponent;
    JointComponent jointComponent;
    Joint joint;
    if (!this.TryComp<GrapplingGunComponent>(activeHandEntity, ref grapplingGunComponent) || !this.TryComp<JointComponent>(activeHandEntity, ref jointComponent) || !jointComponent.GetJoints.TryGetValue("grappling", out joint) || !(joint is DistanceJoint distanceJoint) || (double) distanceJoint.MaxLength <= (double) distanceJoint.MinLength)
      return;
    bool reeling = this._input.CmdStates.GetState(EngineKeyFunctions.UseSecondary) == 1;
    CombatModeComponent combatModeComponent;
    if (!this.TryComp<CombatModeComponent>(localEntity, ref combatModeComponent) || !combatModeComponent.IsInCombatMode)
      reeling = false;
    if (grapplingGunComponent.Reeling == reeling)
      return;
    this.RaisePredictiveEvent<SharedGrapplingGunSystem.RequestGrapplingReelMessage>(new SharedGrapplingGunSystem.RequestGrapplingReelMessage(reeling));
  }
}
