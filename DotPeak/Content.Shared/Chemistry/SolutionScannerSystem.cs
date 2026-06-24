// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.SolutionScannerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Shared.Chemistry;

public sealed class SolutionScannerSystem : EntitySystem
{
  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SolutionScannerComponent, SolutionScanEvent>(new ComponentEventHandler<SolutionScannerComponent, SolutionScanEvent>((object) this, __methodptr(OnSolutionScanAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SolutionScannerComponent, InventoryRelayedEvent<SolutionScanEvent>>(new ComponentEventHandler<SolutionScannerComponent, InventoryRelayedEvent<SolutionScanEvent>>((object) this, __methodptr(\u003CInitialize\u003Eb__0_0)), (Type[]) null, (Type[]) null);
  }

  private void OnSolutionScanAttempt(
    EntityUid eid,
    SolutionScannerComponent component,
    SolutionScanEvent args)
  {
    args.CanScan = true;
  }
}
