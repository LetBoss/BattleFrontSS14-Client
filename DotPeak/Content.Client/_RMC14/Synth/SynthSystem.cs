// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Synth.SynthSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Damage;
using Content.Shared._RMC14.Synth;
using Content.Shared.Damage.Prototypes;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client._RMC14.Synth;

public sealed class SynthSystem : SharedSynthSystem
{
  [Dependency]
  private DamageVisualsSystem _damageVisuals;
  private static readonly ProtoId<DamageGroupPrototype> GroupToChange = ProtoId<DamageGroupPrototype>.op_Implicit("Brute");

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SynthComponent, ComponentStartup>(new EntityEventRefHandler<SynthComponent, ComponentStartup>((object) this, __methodptr(OnCompStartup)), (Type[]) null, (Type[]) null);
  }

  protected override void MakeSynth(Entity<SynthComponent> ent) => base.MakeSynth(ent);

  private void OnCompStartup(Entity<SynthComponent> ent, ref ComponentStartup args)
  {
    SpriteComponent spriteComponent;
    DamageVisualsComponent damageVisuals;
    if (!this.TryComp<SpriteComponent>(ent.Owner, ref spriteComponent) || !this.TryComp<DamageVisualsComponent>(ent.Owner, ref damageVisuals))
      return;
    this._damageVisuals.ChangeDamageGroupColor(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), damageVisuals, ProtoId<DamageGroupPrototype>.op_Implicit(SynthSystem.GroupToChange), ent.Comp.DamageVisualsColor);
  }
}
