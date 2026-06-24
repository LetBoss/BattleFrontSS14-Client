// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Light.CMPoweredLightSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.Light;

public sealed class CMPoweredLightSystem : EntitySystem
{
  [Dependency]
  private SharedPointLightSystem _pointLight;
  [Dependency]
  private SharedPopupSystem _popup;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<LightBurnHandAttemptEvent>(new EntityEventRefHandler<LightBurnHandAttemptEvent>(this.OnLightBurnHandAttempt));
    this.SubscribeLocalEvent<PreventAttackLightOffComponent, GettingAttackedAttemptEvent>(new EntityEventRefHandler<PreventAttackLightOffComponent, GettingAttackedAttemptEvent>(this.OnPreventAttackLightOffAttackedAttempt));
  }

  private void OnLightBurnHandAttempt(ref LightBurnHandAttemptEvent ev)
  {
    ev.Cancelled = true;
    if (this.HasComp<XenoComponent>(ev.User))
      return;
    this._popup.PopupClient(this.Loc.GetString("cm-light-failed"), ev.Light, new EntityUid?(ev.User));
  }

  private void OnPreventAttackLightOffAttackedAttempt(
    Entity<PreventAttackLightOffComponent> ent,
    ref GettingAttackedAttemptEvent args)
  {
    if (args.Cancelled || !this.IsOff((EntityUid) ent))
      return;
    args.Cancelled = true;
  }

  public bool IsOff(EntityUid light)
  {
    SharedPointLightComponent component;
    return !this._pointLight.TryGetLight(light, out component) || !component.Enabled;
  }
}
