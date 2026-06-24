// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Graphics.TextureLoadParameters
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Utility;
using System;
using YamlDotNet.RepresentationModel;

#nullable enable
namespace Robust.Shared.Graphics;

public struct TextureLoadParameters : IEquatable<TextureLoadParameters>
{
  public static readonly TextureLoadParameters Default = new TextureLoadParameters()
  {
    SampleParameters = TextureSampleParameters.Default,
    Srgb = true,
    Preload = true
  };

  public TextureSampleParameters SampleParameters { get; set; }

  public bool Srgb { get; set; }

  public bool Preload { get; set; }

  public static TextureLoadParameters FromYaml(YamlMappingNode yaml)
  {
    TextureLoadParameters textureLoadParameters = TextureLoadParameters.Default;
    YamlMappingNode returnNode1;
    if (yaml.TryGetNode<YamlMappingNode>("sample", out returnNode1))
      textureLoadParameters.SampleParameters = TextureSampleParameters.FromYaml(returnNode1);
    YamlNode returnNode2;
    if (yaml.TryGetNode("srgb", out returnNode2))
      textureLoadParameters.Srgb = returnNode2.AsBool();
    YamlNode returnNode3;
    if (yaml.TryGetNode("preload", out returnNode3))
      textureLoadParameters.Preload = returnNode3.AsBool();
    return textureLoadParameters;
  }

  public bool Equals(TextureLoadParameters other)
  {
    return this.SampleParameters.Equals(other.SampleParameters) && this.Srgb == other.Srgb && this.Preload == other.Preload;
  }

  public override bool Equals(object? obj)
  {
    return obj is TextureLoadParameters other && this.Equals(other);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine<TextureSampleParameters, bool>(this.SampleParameters, this.Srgb);
  }

  public static bool operator ==(TextureLoadParameters left, TextureLoadParameters right)
  {
    return left.Equals(right);
  }

  public static bool operator !=(TextureLoadParameters left, TextureLoadParameters right)
  {
    return !left.Equals(right);
  }
}
