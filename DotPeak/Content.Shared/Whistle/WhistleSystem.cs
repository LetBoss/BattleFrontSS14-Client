// Decompiled with JetBrains decompiler
// Type: Content.Shared.Whistle.WhistleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Stealth;
using Content.Shared.Coordinates;
using Content.Shared.Humanoid;
using Content.Shared.Interaction.Events;
using Content.Shared.Stealth.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Shared.Whistle;

public sealed class WhistleSystem : EntitySystem
{
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<WhistleComponent, UseInHandEvent>(new ComponentEventHandler<WhistleComponent, UseInHandEvent>(this.OnUseInHand));
  }

  private void ExclamateTarget(EntityUid target, WhistleComponent component)
  {
    this.SpawnAttachedTo((string) component.Effect, target.ToCoordinates(), rotation: new Angle());
  }

  public void OnUseInHand(EntityUid uid, WhistleComponent component, UseInHandEvent args)
  {
    if (args.Handled || !this._timing.IsFirstTimePredicted)
      return;
    args.Handled = this.TryMakeLoudWhistle(uid, args.User, component);
  }

  public bool TryMakeLoudWhistle(EntityUid uid, EntityUid owner, WhistleComponent? component = null)
  {
    if (!this.Resolve<WhistleComponent>(uid, ref component, false) || (double) component.Distance <= 0.0)
      return false;
    this.MakeLoudWhistle(uid, owner, component);
    return true;
  }

  private void MakeLoudWhistle(EntityUid uid, EntityUid owner, WhistleComponent component)
  {
    StealthComponent comp = (StealthComponent) null;
    foreach (Entity<HumanoidAppearanceComponent> entity in this._entityLookup.GetEntitiesInRange<HumanoidAppearanceComponent>(this._transform.GetMapCoordinates(uid), component.Distance))
    {
      if ((!this.TryComp<StealthComponent>((EntityUid) entity, out comp) || !comp.Enabled) && !(entity.Owner == owner) && !this.HasComp<EntityActiveInvisibleComponent>((EntityUid) entity))
        this.ExclamateTarget((EntityUid) entity, component);
    }
  }
}
