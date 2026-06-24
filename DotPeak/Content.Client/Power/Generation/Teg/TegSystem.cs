// Decompiled with JetBrains decompiler
// Type: Content.Client.Power.Generation.Teg.TegSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.Power.Generation.Teg;

public sealed class TegSystem : EntitySystem
{
  private static readonly EntProtoId ArrowPrototype = EntProtoId.op_Implicit("TegCirculatorArrow");

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<TegCirculatorComponent, ClientExaminedEvent>(new ComponentEventHandler<TegCirculatorComponent, ClientExaminedEvent>((object) this, __methodptr(CirculatorExamined)), (Type[]) null, (Type[]) null);
  }

  private void CirculatorExamined(
    EntityUid uid,
    TegCirculatorComponent component,
    ClientExaminedEvent args)
  {
    this.Spawn(EntProtoId.op_Implicit(TegSystem.ArrowPrototype), new EntityCoordinates(uid, 0.0f, 0.0f));
  }
}
