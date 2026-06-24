// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Graphics.TextureSampleParameters
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Utility;
using System;
using YamlDotNet.RepresentationModel;

#nullable enable
namespace Robust.Shared.Graphics;

public struct TextureSampleParameters : IEquatable<TextureSampleParameters>
{
  public static readonly TextureSampleParameters Default = new TextureSampleParameters()
  {
    Filter = false,
    WrapMode = TextureWrapMode.None
  };

  public bool Filter { get; set; }

  public TextureWrapMode WrapMode { get; set; }

  public static TextureSampleParameters FromYaml(YamlMappingNode node)
  {
    TextureWrapMode textureWrapMode = TextureWrapMode.None;
    bool flag = false;
    YamlNode returnNode1;
    if (node.TryGetNode("filter", out returnNode1))
      flag = returnNode1.AsBool();
    YamlNode returnNode2;
    if (node.TryGetNode("wrap", out returnNode2))
    {
      switch (returnNode2.AsString())
      {
        case "none":
          textureWrapMode = TextureWrapMode.None;
          break;
        case "repeat":
          textureWrapMode = TextureWrapMode.Repeat;
          break;
        case "mirrored_repeat":
          textureWrapMode = TextureWrapMode.MirroredRepeat;
          break;
        default:
          throw new ArgumentException("Not a valid wrap mode.");
      }
    }
    return new TextureSampleParameters()
    {
      Filter = flag,
      WrapMode = textureWrapMode
    };
  }

  public bool Equals(TextureSampleParameters other)
  {
    return this.Filter == other.Filter && this.WrapMode == other.WrapMode;
  }

  public override bool Equals(object? obj)
  {
    return obj is TextureSampleParameters other && this.Equals(other);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine<bool, int>(this.Filter, (int) this.WrapMode);
  }

  public static bool operator ==(TextureSampleParameters left, TextureSampleParameters right)
  {
    return left.Equals(right);
  }

  public static bool operator !=(TextureSampleParameters left, TextureSampleParameters right)
  {
    return !left.Equals(right);
  }
}
