// Decompiled with JetBrains decompiler
// Type: Content.Shared.SubFloor.SharedTrayScannerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Inventory.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using System;

#nullable enable
namespace Content.Shared.SubFloor;

public abstract class SharedTrayScannerSystem : EntitySystem
{
  [Dependency]
  private INetManager _netMan;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedEyeSystem _eye;
  public const float SubfloorRevealAlpha = 0.8f;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<TrayScannerComponent, ComponentGetState>(new ComponentEventRefHandler<TrayScannerComponent, ComponentGetState>(this.OnTrayScannerGetState));
    this.SubscribeLocalEvent<TrayScannerComponent, ComponentHandleState>(new ComponentEventRefHandler<TrayScannerComponent, ComponentHandleState>(this.OnTrayScannerHandleState));
    this.SubscribeLocalEvent<TrayScannerComponent, ActivateInWorldEvent>(new ComponentEventHandler<TrayScannerComponent, ActivateInWorldEvent>(this.OnTrayScannerActivate));
    this.SubscribeLocalEvent<TrayScannerComponent, GotEquippedHandEvent>(new EntityEventRefHandler<TrayScannerComponent, GotEquippedHandEvent>(this.OnTrayHandEquipped));
    this.SubscribeLocalEvent<TrayScannerComponent, GotUnequippedHandEvent>(new EntityEventRefHandler<TrayScannerComponent, GotUnequippedHandEvent>(this.OnTrayHandUnequipped));
    this.SubscribeLocalEvent<TrayScannerComponent, GotEquippedEvent>(new EntityEventRefHandler<TrayScannerComponent, GotEquippedEvent>(this.OnTrayEquipped));
    this.SubscribeLocalEvent<TrayScannerComponent, GotUnequippedEvent>(new EntityEventRefHandler<TrayScannerComponent, GotUnequippedEvent>(this.OnTrayUnequipped));
    this.SubscribeLocalEvent<TrayScannerUserComponent, GetVisMaskEvent>(new EntityEventRefHandler<TrayScannerUserComponent, GetVisMaskEvent>(this.OnUserGetVis));
  }

  private void OnUserGetVis(Entity<TrayScannerUserComponent> ent, ref GetVisMaskEvent args)
  {
    args.VisibilityMask |= 4;
  }

  private void OnEquip(EntityUid user)
  {
    if (this._netMan.IsClient)
      return;
    TrayScannerUserComponent scannerUserComponent = this.EnsureComp<TrayScannerUserComponent>(user);
    ++scannerUserComponent.Count;
    if (scannerUserComponent.Count > 1)
      return;
    this._eye.RefreshVisibilityMask((Entity<EyeComponent>) user);
  }

  private void OnUnequip(EntityUid user)
  {
    TrayScannerUserComponent comp;
    if (this._netMan.IsClient || !this.TryComp<TrayScannerUserComponent>(user, out comp))
      return;
    --comp.Count;
    if (comp.Count > 0)
      return;
    this.RemComp<TrayScannerUserComponent>(user);
    this._eye.RefreshVisibilityMask((Entity<EyeComponent>) user);
  }

  private void OnTrayHandUnequipped(
    Entity<TrayScannerComponent> ent,
    ref GotUnequippedHandEvent args)
  {
    this.OnUnequip(args.User);
  }

  private void OnTrayHandEquipped(Entity<TrayScannerComponent> ent, ref GotEquippedHandEvent args)
  {
    this.OnEquip(args.User);
  }

  private void OnTrayUnequipped(Entity<TrayScannerComponent> ent, ref GotUnequippedEvent args)
  {
    this.OnUnequip(args.Equipee);
  }

  private void OnTrayEquipped(Entity<TrayScannerComponent> ent, ref GotEquippedEvent args)
  {
    this.OnEquip(args.Equipee);
  }

  private void OnTrayScannerActivate(
    EntityUid uid,
    TrayScannerComponent scanner,
    ActivateInWorldEvent args)
  {
    if (args.Handled || !args.Complex)
      return;
    this.SetScannerEnabled(uid, !scanner.Enabled, scanner);
    args.Handled = true;
  }

  private void SetScannerEnabled(EntityUid uid, bool enabled, TrayScannerComponent? scanner = null)
  {
    if (!this.Resolve<TrayScannerComponent>(uid, ref scanner) || scanner.Enabled == enabled)
      return;
    scanner.Enabled = enabled;
    this.Dirty(uid, (IComponent) scanner);
    AppearanceComponent comp;
    if (!this.TryComp<AppearanceComponent>(uid, out comp))
      return;
    this._appearance.SetData(uid, (Enum) TrayScannerVisual.Visual, (object) (TrayScannerVisual) (scanner.Enabled ? 1 : 2), comp);
  }

  private void OnTrayScannerGetState(
    EntityUid uid,
    TrayScannerComponent scanner,
    ref ComponentGetState args)
  {
    args.State = (IComponentState) new TrayScannerState(scanner.Enabled, scanner.Range);
  }

  private void OnTrayScannerHandleState(
    EntityUid uid,
    TrayScannerComponent scanner,
    ref ComponentHandleState args)
  {
    if (!(args.Current is TrayScannerState current))
      return;
    scanner.Range = current.Range;
    this.SetScannerEnabled(uid, current.Enabled, scanner);
  }
}
