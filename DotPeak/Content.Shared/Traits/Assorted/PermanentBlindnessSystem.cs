// Decompiled with JetBrains decompiler
// Type: Content.Shared.Traits.Assorted.PermanentBlindnessSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Eye.Blinding.Systems;
using Content.Shared.IdentityManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Traits.Assorted;

public sealed class PermanentBlindnessSystem : EntitySystem
{
  [Dependency]
  private BlindableSystem _blinding;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<PermanentBlindnessComponent, MapInitEvent>(new EntityEventRefHandler<PermanentBlindnessComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<PermanentBlindnessComponent, ComponentShutdown>(new EntityEventRefHandler<PermanentBlindnessComponent, ComponentShutdown>(this.OnShutdown));
    this.SubscribeLocalEvent<PermanentBlindnessComponent, ExaminedEvent>(new EntityEventRefHandler<PermanentBlindnessComponent, ExaminedEvent>(this.OnExamined));
  }

  private void OnExamined(Entity<PermanentBlindnessComponent> blindness, ref ExaminedEvent args)
  {
    if (!args.IsInDetailsRange || blindness.Comp.Blindness != 0)
      return;
    args.PushMarkup(this.Loc.GetString("permanent-blindness-trait-examined", ("target", (object) Identity.Entity((EntityUid) blindness, (IEntityManager) this.EntityManager))));
  }

  private void OnShutdown(Entity<PermanentBlindnessComponent> blindness, ref ComponentShutdown args)
  {
    BlindableComponent comp;
    if (!this.TryComp<BlindableComponent>(blindness.Owner, out comp) || comp.MinDamage == 0)
      return;
    this._blinding.SetMinDamage((Entity<BlindableComponent>) (blindness.Owner, comp), 0);
  }

  private void OnMapInit(Entity<PermanentBlindnessComponent> blindness, ref MapInitEvent args)
  {
    BlindableComponent comp;
    if (!this.TryComp<BlindableComponent>(blindness.Owner, out comp))
      return;
    if (blindness.Comp.Blindness != 0)
    {
      this._blinding.SetMinDamage((Entity<BlindableComponent>) (blindness.Owner, comp), blindness.Comp.Blindness);
    }
    else
    {
      int amount = 6;
      this._blinding.SetMinDamage((Entity<BlindableComponent>) (blindness.Owner, comp), amount);
    }
  }
}
