// Decompiled with JetBrains decompiler
// Type: Content.Shared.Instruments.SharedInstrumentSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Instruments;

public abstract class SharedInstrumentSystem : EntitySystem
{
  public abstract bool ResolveInstrument(EntityUid uid, ref SharedInstrumentComponent? component);

  public virtual void SetupRenderer(
    EntityUid uid,
    bool fromStateChange,
    SharedInstrumentComponent? instrument = null)
  {
  }

  public virtual void EndRenderer(
    EntityUid uid,
    bool fromStateChange,
    SharedInstrumentComponent? instrument = null)
  {
  }

  public void SetInstrumentProgram(
    EntityUid uid,
    SharedInstrumentComponent component,
    byte program,
    byte bank)
  {
    component.InstrumentBank = bank;
    component.InstrumentProgram = program;
    this.Dirty(uid, (IComponent) component);
  }
}
