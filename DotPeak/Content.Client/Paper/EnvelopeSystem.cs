// Decompiled with JetBrains decompiler
// Type: Content.Client.Paper.EnvelopeSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Paper;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Paper;

public sealed class EnvelopeSystem : VisualizerSystem<EnvelopeComponent>
{
  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<EnvelopeComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<EnvelopeComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnAfterAutoHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnAfterAutoHandleState(
    Entity<EnvelopeComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this.UpdateAppearance(ent);
  }

  private void UpdateAppearance(Entity<EnvelopeComponent> ent, SpriteComponent? sprite = null)
  {
    if (!((EntitySystem) this).Resolve<SpriteComponent>(ent.Owner, ref sprite, true))
      return;
    this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, sprite)), (Enum) EnvelopeSystem.EnvelopeVisualLayers.Open, ent.Comp.State == EnvelopeComponent.EnvelopeState.Open);
    this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, sprite)), (Enum) EnvelopeSystem.EnvelopeVisualLayers.Sealed, ent.Comp.State == EnvelopeComponent.EnvelopeState.Sealed);
    this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, sprite)), (Enum) EnvelopeSystem.EnvelopeVisualLayers.Torn, ent.Comp.State == EnvelopeComponent.EnvelopeState.Torn);
  }

  public enum EnvelopeVisualLayers : byte
  {
    Open,
    Sealed,
    Torn,
  }
}
