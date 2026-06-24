// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.Attenuation
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable disable
namespace Robust.Shared.Audio;

public enum Attenuation
{
  Invalid = 0,
  NoAttenuation = 1,
  InverseDistance = 2,
  InverseDistanceClamped = 4,
  LinearDistance = 8,
  LinearDistanceClamped = 16, // 0x00000010
  ExponentDistance = 32, // 0x00000020
  ExponentDistanceClamped = 64, // 0x00000040
}
