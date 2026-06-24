// Decompiled with JetBrains decompiler
// Type: Content.Client.Beam.BeamSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Beam;
using Content.Shared.Beam.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Beam;

public sealed class BeamSystem : SharedBeamSystem
{
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<BeamVisualizerEvent>(new EntityEventHandler<BeamVisualizerEvent>(this.BeamVisualizerMessage), (Type[]) null, (Type[]) null);
  }

  private void BeamVisualizerMessage(BeamVisualizerEvent args)
  {
    EntityUid entity = this.GetEntity(args.Beam);
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(entity, ref spriteComponent))
      return;
    this._sprite.SetRotation(Entity<SpriteComponent>.op_Implicit((entity, spriteComponent)), args.UserAngle);
    if (args.BodyState == null)
      return;
    this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity, spriteComponent)), 0, RSI.StateId.op_Implicit(args.BodyState));
    spriteComponent.LayerSetShader(0, args.Shader);
  }
}
