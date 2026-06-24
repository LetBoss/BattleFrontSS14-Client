// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.BroadphaseData
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable disable
namespace Robust.Shared.GameObjects;

internal record struct BroadphaseData(EntityUid Uid, bool CanCollide, bool Static)
{
  public static readonly BroadphaseData Invalid;

  public bool IsValid() => this.Uid.IsValid();

  public bool Valid => this.IsValid();
}
