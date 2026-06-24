// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.RobustILGenerator
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

#nullable enable
namespace Robust.Shared.Utility;

public sealed class RobustILGenerator
{
  private ILGenerator _generator;
  private List<(object, object?)> _log = new List<(object, object)>();
  private List<(Type, int)> _locals = new List<(Type, int)>();

  public RobustILGenerator(ILGenerator generator) => this._generator = generator;

  public string[] GetStringLog()
  {
    List<string> list = this._locals.Select<(Type, int), string>((Func<(Type, int), string>) (e => $"LOCAL: {e.Item1} at {e.Item2}")).ToList<string>();
    list.Add("===== CODE =====");
    list.AddRange(this._log.Select<(object, object), string>((Func<(object, object), string>) (e => $"{e.Item1} - {e.Item2}")));
    return list.ToArray();
  }

  private void Log(object opcode, object? arg = null) => this._log.Add((opcode, arg));

  public LocalBuilder DeclareLocal(Type localType) => this.DeclareLocal(localType, false);

  public LocalBuilder DeclareLocal(Type localType, bool pinned)
  {
    LocalBuilder localBuilder = this._generator.DeclareLocal(localType, pinned);
    this._locals.Add((localType, localBuilder.LocalIndex));
    return localBuilder;
  }

  public void ThrowException([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type excType)
  {
    this._generator.ThrowException(excType);
  }

  public Label DefineLabel() => this._generator.DefineLabel();

  public void MarkLabel(Label loc)
  {
    this._generator.MarkLabel(loc);
    this.Log((object) nameof (MarkLabel), (object) loc.GetHashCode());
  }

  public void Emit(OpCode opcode)
  {
    this._generator.Emit(opcode);
    this.Log((object) opcode);
  }

  public void Emit(OpCode opcode, byte arg)
  {
    this._generator.Emit(opcode, arg);
    this.Log((object) opcode, (object) arg);
  }

  public void Emit(OpCode opcode, sbyte arg)
  {
    this._generator.Emit(opcode, arg);
    this.Log((object) opcode, (object) arg);
  }

  public void Emit(OpCode opcode, short arg)
  {
    this._generator.Emit(opcode, arg);
    this.Log((object) opcode, (object) arg);
  }

  public void Emit(OpCode opcode, int arg)
  {
    this._generator.Emit(opcode, arg);
    this.Log((object) opcode, (object) arg);
  }

  public void Emit(OpCode opcode, MethodInfo meth)
  {
    this._generator.Emit(opcode, meth);
    this.Log((object) opcode, (object) meth);
  }

  public void EmitCall(OpCode opcode, MethodInfo methodInfo, params Type[]? optionalParameterTypes)
  {
    this._generator.EmitCall(opcode, methodInfo, optionalParameterTypes);
    this.Log((object) opcode, (object) methodInfo);
  }

  public void Emit(OpCode opcode, SignatureHelper signature)
  {
    this._generator.Emit(opcode, signature);
    this.Log((object) opcode, (object) signature);
  }

  public void Emit(OpCode opcode, ConstructorInfo con)
  {
    this._generator.Emit(opcode, con);
    this.Log((object) opcode, (object) con);
  }

  public void Emit(OpCode opcode, Type cls)
  {
    this._generator.Emit(opcode, cls);
    this.Log((object) opcode, (object) cls);
  }

  public void Emit(OpCode opcode, long arg)
  {
    this._generator.Emit(opcode, arg);
    this.Log((object) opcode, (object) arg);
  }

  public void Emit(OpCode opcode, float arg)
  {
    this._generator.Emit(opcode, arg);
    this.Log((object) opcode, (object) arg);
  }

  public void Emit(OpCode opcode, double arg)
  {
    this._generator.Emit(opcode, arg);
    this.Log((object) opcode, (object) arg);
  }

  public void Emit(OpCode opcode, Label label)
  {
    this._generator.Emit(opcode, label);
    this.Log((object) opcode, (object) label.GetHashCode());
  }

  public void Emit(OpCode opcode, Label[] labels)
  {
    this._generator.Emit(opcode, labels);
    this.Log((object) opcode, (object) labels);
  }

  public void Emit(OpCode opcode, FieldInfo field)
  {
    this._generator.Emit(opcode, field);
    this.Log((object) opcode, (object) field);
  }

  public void Emit(OpCode opcode, string str)
  {
    this._generator.Emit(opcode, str);
    this.Log((object) opcode, (object) str);
  }

  public void Emit(OpCode opcode, LocalBuilder local)
  {
    this._generator.Emit(opcode, local);
    this.Log((object) opcode, (object) local);
  }
}
