// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Math.RngCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Random;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class RngCommand : ToolshedCommand
{
  [Dependency]
  private readonly IRobustRandom _random;

  [CommandImplementation("to")]
  public int To([PipedArgument] int from, int to) => this._random.Next(from, to);

  [CommandImplementation("from")]
  public int From([PipedArgument] int to, int from) => this._random.Next(from, to);

  [CommandImplementation("to")]
  public float To([PipedArgument] float from, float to) => this._random.NextFloat(from, to);

  [CommandImplementation("from")]
  public float From([PipedArgument] float to, float from) => this._random.NextFloat(from, to);

  [CommandImplementation("prob")]
  public bool Prob([PipedArgument] float prob) => this._random.Prob(prob);
}
