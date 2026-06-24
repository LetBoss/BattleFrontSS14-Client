// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Actions.XenoActionsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Actions;

public sealed class XenoActionsSystem : EntitySystem
{
  [Dependency]
  private XenoSystem _xeno;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoOffensiveActionComponent, ActionValidateEvent>(new EntityEventRefHandler<XenoOffensiveActionComponent, ActionValidateEvent>(this.OnValidateActionEntityTarget));
  }

  private void OnValidateActionEntityTarget(
    Entity<XenoOffensiveActionComponent> ent,
    ref ActionValidateEvent args)
  {
    if (args.Invalid)
      return;
    EntityUid? entity = this.GetEntity(args.Input.EntityTarget);
    if (!entity.HasValue)
      return;
    EntityUid valueOrDefault = entity.GetValueOrDefault();
    if (this._xeno.CanAbilityAttackTarget(args.User, valueOrDefault, canAttackWindows: true))
      return;
    args.Invalid = true;
  }
}
