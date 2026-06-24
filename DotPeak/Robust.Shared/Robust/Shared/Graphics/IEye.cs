// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Graphics.IEye
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System.Numerics;

#nullable disable
namespace Robust.Shared.Graphics;

[NotContentImplementable]
public interface IEye
{
  bool DrawFov { get; set; }

  bool DrawLight { get; set; }

  MapCoordinates Position { get; }

  Vector2 Offset { get; set; }

  Angle Rotation { get; set; }

  Vector2 Zoom { get; set; }

  Vector2 Scale { get; set; }

  void GetViewMatrix(out Matrix3x2 viewMatrix, Vector2 renderScale);

  void GetViewMatrixInv(out Matrix3x2 viewMatrixInv, Vector2 renderScale);

  void GetViewMatrixNoOffset(out Matrix3x2 viewMatrix, Vector2 renderScale);
}
