// Decompiled with JetBrains decompiler
// Type: Content.Shared.Lightning.SharedLightningSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

#nullable enable
namespace Content.Shared.Lightning;

public abstract class SharedLightningSystem : EntitySystem
{
  [Dependency]
  private IRobustRandom _random;

  public string LightningRandomizer() => "lightning_" + this._random.Next(1, 12).ToString();
}
