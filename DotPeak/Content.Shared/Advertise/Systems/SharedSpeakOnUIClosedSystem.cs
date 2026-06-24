// Decompiled with JetBrains decompiler
// Type: Content.Shared.Advertise.Systems.SharedSpeakOnUIClosedSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Advertise.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Advertise.Systems;

public abstract class SharedSpeakOnUIClosedSystem : EntitySystem
{
  public bool TrySetFlag(Entity<SpeakOnUIClosedComponent?> entity, bool value = true)
  {
    if (!this.Resolve<SpeakOnUIClosedComponent>(Entity<SpeakOnUIClosedComponent>.op_Implicit(entity), ref entity.Comp, true))
      return false;
    entity.Comp.Flag = value;
    this.Dirty<SpeakOnUIClosedComponent>(entity, (MetaDataComponent) null);
    return true;
  }
}
