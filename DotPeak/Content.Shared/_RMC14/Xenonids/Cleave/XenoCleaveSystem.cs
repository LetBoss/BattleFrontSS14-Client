// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Cleave.XenoCleaveSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Shields;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared.Coordinates;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using System;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Cleave;

public sealed class XenoCleaveSystem : EntitySystem
{
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private VanguardShieldSystem _vanguard;
  [Dependency]
  private RMCPullingSystem _rmcPulling;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedRMCMeleeWeaponSystem _rmcMelee;
  [Dependency]
  private RMCSlowSystem _slow;
  [Dependency]
  private RMCSizeStunSystem _sizeStun;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoCleaveComponent, XenoCleaveActionEvent>(new EntityEventRefHandler<XenoCleaveComponent, XenoCleaveActionEvent>(this.OnCleaveAction));
  }

  private void OnCleaveAction(Entity<XenoCleaveComponent> xeno, ref XenoCleaveActionEvent args)
  {
    if (!this._xeno.CanAbilityAttackTarget((EntityUid) xeno, args.Target) || args.Handled)
      return;
    if (this._net.IsServer)
      this._audio.PlayPvs(xeno.Comp.Sound, (EntityUid) xeno);
    bool flag = this._vanguard.ShieldBuff((EntityUid) xeno);
    args.Handled = true;
    this._rmcMelee.DoLunge((EntityUid) xeno, args.Target);
    if (args.Flings)
    {
      float num = flag ? xeno.Comp.FlingDistanceBuffed : xeno.Comp.FlingDistance;
      RMCSizes size;
      if (this._sizeStun.TryGetSize(args.Target, out size) && size >= RMCSizes.Big)
        num *= 0.1f;
      this._rmcPulling.TryStopAllPullsFromAndOn(args.Target);
      MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) xeno);
      this._sizeStun.KnockBack(args.Target, new MapCoordinates?(mapCoordinates), num, num, 10f, true);
      if (!this._net.IsServer)
        return;
      this.SpawnAttachedTo((string) xeno.Comp.FlingEffect, args.Target.ToCoordinates(), rotation: new Angle());
    }
    else
    {
      TimeSpan baseDuration = flag ? xeno.Comp.RootTimeBuffed : xeno.Comp.RootTime;
      this._slow.TryRoot(args.Target, this._xeno.TryApplyXenoDebuffMultiplier(args.Target, baseDuration));
      if (!this._net.IsServer)
        return;
      this.SpawnAttachedTo((string) xeno.Comp.RootEffect, args.Target.ToCoordinates(), rotation: new Angle());
    }
  }
}
