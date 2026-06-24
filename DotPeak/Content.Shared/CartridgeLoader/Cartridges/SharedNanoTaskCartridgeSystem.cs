// Decompiled with JetBrains decompiler
// Type: Content.Shared.CartridgeLoader.Cartridges.SharedNanoTaskCartridgeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Shared.CartridgeLoader.Cartridges;

public abstract class SharedNanoTaskCartridgeSystem : EntitySystem
{
  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<NanoTaskCartridgeComponent, CartridgeAddedEvent>(new EntityEventRefHandler<NanoTaskCartridgeComponent, CartridgeAddedEvent>((object) this, __methodptr(OnCartridgeAdded)), (Type[]) null, (Type[]) null);
  }

  private void OnCartridgeAdded(
    Entity<NanoTaskCartridgeComponent> ent,
    ref CartridgeAddedEvent args)
  {
    this.EnsureComp<NanoTaskInteractionComponent>(args.Loader);
  }
}
