// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Light.RMCLightOffsetSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Sprite;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Light;

public sealed class RMCLightOffsetSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedRMCSpriteSystem _sprite;
  private readonly HashSet<EntityUid> ToUpdate = new HashSet<EntityUid>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCLightOffsetComponent, MapInitEvent>(new EntityEventRefHandler<RMCLightOffsetComponent, MapInitEvent>(this.OnLightUpdate<MapInitEvent>));
    this.SubscribeLocalEvent<RMCLightOffsetComponent, EntParentChangedMessage>(new EntityEventRefHandler<RMCLightOffsetComponent, EntParentChangedMessage>(this.OnLightUpdate<EntParentChangedMessage>));
  }

  private void OnLightUpdate<T>(Entity<RMCLightOffsetComponent> ent, ref T args)
  {
    MetaDataComponent comp;
    if (!this.TryComp((EntityUid) ent, out comp) || comp.EntityLifeStage < EntityLifeStage.MapInitialized)
      return;
    this.ToUpdate.Add((EntityUid) ent);
    if (this._net.IsClient || this.TerminatingOrDeleted((EntityUid) ent))
      return;
    SpriteSetRenderOrderComponent renderOrderComponent = this.EnsureComp<SpriteSetRenderOrderComponent>((EntityUid) ent);
    Angle localRotation = this.Transform((EntityUid) ent).LocalRotation;
    switch ((int) ((Angle) ref localRotation).GetDir())
    {
      case 0:
        this._sprite.SetOffset((EntityUid) ent, new Vector2(0.45f, -0.32f));
        break;
      case 2:
        this._sprite.SetOffset((EntityUid) ent, new Vector2(0.7f, -1.45f));
        break;
      case 4:
        this._sprite.SetOffset((EntityUid) ent, new Vector2(-0.5f, -1.5f));
        break;
      case 6:
        this._sprite.SetOffset((EntityUid) ent, new Vector2(-0.7f, -0.4f));
        break;
    }
    this.Dirty((EntityUid) ent, (IComponent) renderOrderComponent);
  }
}
