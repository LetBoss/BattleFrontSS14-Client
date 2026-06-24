// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Infected.XenoParasiteSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Sprite;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Hide;
using Content.Shared._RMC14.Xenonids.Leap;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.Mobs.Components;
using Content.Shared.Movement.Components;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Content.Shared.Throwing;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Infected;

public sealed class XenoParasiteSystem : SharedXenoParasiteSystem
{
  [Dependency]
  private XenoVisualizerSystem _xenoVisualizer;
  [Dependency]
  private TagSystem _tags;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<XenoParasiteComponent, GetDrawDepthEvent>(new EntityEventRefHandler<XenoParasiteComponent, GetDrawDepthEvent>((object) this, __methodptr(OnGetParasiteDrawDepth)), new Type[1]
    {
      typeof (XenoHideSystem)
    }, (Type[]) null);
  }

  private void OnGetParasiteDrawDepth(
    Entity<XenoParasiteComponent> parasite,
    ref GetDrawDepthEvent args)
  {
    if (this._tags.HasTag(Entity<XenoParasiteComponent>.op_Implicit(parasite), this.ParasiteIsPreparingLeapProtoID) || this.HasComp<XenoLeapingComponent>(Entity<XenoParasiteComponent>.op_Implicit(parasite)))
      args.DrawDepth = Content.Shared.DrawDepth.DrawDepth.Overdoors;
    else
      args.DrawDepth = Content.Shared.DrawDepth.DrawDepth.Mobs;
  }

  public virtual void FrameUpdate(float frameTime)
  {
    base.FrameUpdate(frameTime);
    EntityQueryEnumerator<XenoComponent, ThrownItemComponent, SpriteComponent, AppearanceComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoComponent, ThrownItemComponent, SpriteComponent, AppearanceComponent>();
    EntityUid entityUid;
    XenoComponent xenoComponent;
    ThrownItemComponent thrownItemComponent;
    SpriteComponent spriteComponent;
    AppearanceComponent appearanceComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref xenoComponent, ref thrownItemComponent, ref spriteComponent, ref appearanceComponent))
      this._xenoVisualizer.UpdateSprite(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit((entityUid, spriteComponent, (MobStateComponent) null, appearanceComponent, (InputMoverComponent) null, thrownItemComponent)));
  }
}
