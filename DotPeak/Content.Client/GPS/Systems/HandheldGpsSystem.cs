// Decompiled with JetBrains decompiler
// Type: Content.Client.GPS.Systems.HandheldGpsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.GPS.UI;
using Content.Client.Items;
using Content.Shared.GPS.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.GPS.Systems;

public sealed class HandheldGpsSystem : EntitySystem
{
  public virtual void Initialize()
  {
    base.Initialize();
    this.Subs.ItemStatus<HandheldGPSComponent>((Func<Entity<HandheldGPSComponent>, Control>) (ent => (Control) new HandheldGpsStatusControl(ent)));
  }
}
