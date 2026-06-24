// Decompiled with JetBrains decompiler
// Type: Content.Shared.CardboardBox.Components.PlayBoxEffectMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.CardboardBox.Components;

[NetSerializable]
[Serializable]
public sealed class PlayBoxEffectMessage : EntityEventArgs
{
  public NetEntity Source;
  public NetEntity Mover;

  public PlayBoxEffectMessage(NetEntity source, NetEntity mover)
  {
    this.Source = source;
    this.Mover = mover;
  }
}
