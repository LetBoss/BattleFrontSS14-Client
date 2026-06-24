// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Buckle.RMCBuckleVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Sprite;
using Content.Client._RMC14.Xenonids;
using Content.Shared._RMC14.Buckle;
using Content.Shared._RMC14.Sprite;
using Content.Shared.Buckle.Components;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._RMC14.Buckle;

public sealed class RMCBuckleVisualsSystem : EntitySystem
{
  [Dependency]
  private RMCSpriteSystem _rmcSprite;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BuckleComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<BuckleComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnBuckleState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RMCBuckleDrawDepthComponent, GetDrawDepthEvent>(new EntityEventRefHandler<RMCBuckleDrawDepthComponent, GetDrawDepthEvent>((object) this, __methodptr(OnGetDrawDepth)), (Type[]) null, new Type[1]
    {
      typeof (XenoVisualizerSystem)
    });
  }

  private void OnBuckleState(Entity<BuckleComponent> ent, ref AfterAutoHandleStateEvent args)
  {
    int num = (int) this._rmcSprite.UpdateDrawDepth(ent.Owner);
  }

  private void OnGetDrawDepth(Entity<RMCBuckleDrawDepthComponent> ent, ref GetDrawDepthEvent args)
  {
    BuckleComponent buckleComponent;
    StrapComponent strapComponent;
    if (!this.TryComp<BuckleComponent>(Entity<RMCBuckleDrawDepthComponent>.op_Implicit(ent), ref buckleComponent) || !this.TryComp<StrapComponent>(buckleComponent.BuckledTo, ref strapComponent))
      return;
    int? drawDepth = this.GetDrawDepth(Entity<BuckleComponent>.op_Implicit((Entity<RMCBuckleDrawDepthComponent>.op_Implicit(ent), buckleComponent)), Entity<StrapComponent>.op_Implicit((buckleComponent.BuckledTo.Value, strapComponent)));
    if (!drawDepth.HasValue)
      return;
    int valueOrDefault = drawDepth.GetValueOrDefault();
    args.DrawDepth = (Content.Shared.DrawDepth.DrawDepth) valueOrDefault;
  }

  private int? GetDrawDepth(Entity<BuckleComponent> buckle, Entity<StrapComponent> strap)
  {
    Content.Shared.DrawDepth.DrawDepth? buckleDepth = (Content.Shared.DrawDepth.DrawDepth?) this.CompOrNull<RMCBuckleDrawDepthComponent>(Entity<BuckleComponent>.op_Implicit(buckle))?.BuckleDepth;
    int? drawDepth = buckleDepth.HasValue ? new int?((int) buckleDepth.GetValueOrDefault()) : new int?();
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(Entity<StrapComponent>.op_Implicit(strap), ref spriteComponent))
      return new int?();
    if (this.HasComp<RMCStrapNoDrawDepthChangeComponent>(Entity<StrapComponent>.op_Implicit(strap)) && !drawDepth.HasValue)
      drawDepth = new int?(spriteComponent.DrawDepth + 1);
    if (!drawDepth.HasValue)
    {
      Angle localRotation = this.Transform(Entity<StrapComponent>.op_Implicit(strap)).LocalRotation;
      drawDepth = ((Angle) ref localRotation).GetCardinalDir() != 4 ? new int?(spriteComponent.DrawDepth + 1) : new int?(spriteComponent.DrawDepth - 1);
    }
    return drawDepth;
  }
}
