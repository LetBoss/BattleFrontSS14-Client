// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.DebugTools
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Utility;

public static class DebugTools
{
  [Conditional("DEBUG")]
  [DoesNotReturn]
  public static void Assert(string message) => throw new DebugAssertException(message);

  [Conditional("DEBUG")]
  public static void Assert([DoesNotReturnIf(false)] bool condition)
  {
    if (!condition)
      throw new DebugAssertException();
  }

  [Conditional("DEBUG")]
  public static void AssertEqual(object? objA, object? objB)
  {
    if (objA != objB && (objA == null || !objA.Equals(objB)))
      throw new DebugAssertException($"Expected: {objB ?? (object) "null"} but was {objA ?? (object) "null"}");
  }

  [Conditional("DEBUG")]
  public static void AssertEqual(object? objA, object? objB, string message)
  {
    if (objA != objB && (objA == null || !objA.Equals(objB)))
      throw new DebugAssertException($"{message}\nExpected: {objB ?? (object) "null"} but was {objA ?? (object) "null"}");
  }

  [Conditional("DEBUG")]
  public static void AssertEqual<T>(T? objA, T? objB) where T : IEquatable<T>
  {
    if (((object) objA != null || (object) objB != null) && ((object) objA == null || !objA.Equals(objB)))
      throw new DebugAssertException($"Expected: {objB?.ToString() ?? "null"} but was {objA?.ToString() ?? "null"}");
  }

  [Conditional("DEBUG")]
  public static void AssertEqual<T>(T? objA, T? objB, string message) where T : IEquatable<T>
  {
    if (((object) objA != null || (object) objB != null) && ((object) objA == null || !objA.Equals(objB)))
      throw new DebugAssertException($"{message}\nExpected: {objB?.ToString() ?? "null"} but was {objA?.ToString() ?? "null"}");
  }

  [Conditional("DEBUG")]
  public static void AssertNotEqual(object? objA, object? objB)
  {
    if (objA == objB)
      throw new DebugAssertException($"Expected: not {objB ?? (object) "null"}");
    if (objA != null && objA.Equals(objB))
      throw new DebugAssertException($"Expected: not {objB}");
  }

  [Conditional("DEBUG")]
  public static void AssertNotEqual(object? objA, object? objB, string message)
  {
    if (objA == objB)
      throw new DebugAssertException($"{message}\nExpected: not {objB ?? (object) "null"}");
    if (objA != null && objA.Equals(objB))
      throw new DebugAssertException($"{message}\nExpected: not {objB}");
  }

  [Conditional("DEBUG")]
  public static void AssertNotEqual<T>(T? objA, T? objB) where T : IEquatable<T>
  {
    if ((object) objA == null && (object) objB == null)
      throw new DebugAssertException("Expected: not null");
    if ((object) objA != null && objA.Equals(objB))
      throw new DebugAssertException("Expected: not " + (objB?.ToString() ?? "null"));
  }

  [Conditional("DEBUG")]
  public static void AssertNotEqual<T>(T? objA, T? objB, string message) where T : IEquatable<T>
  {
    if ((object) objA == null && (object) objB == null)
      throw new DebugAssertException(message + "\nExpected: not null");
    if ((object) objA != null && objA.Equals(objB))
      throw new DebugAssertException($"{message}\nExpected: not {objB?.ToString() ?? "null"}");
  }

  [Conditional("DEBUG")]
  public static void AssertOwner(EntityUid? uid, IComponent? component)
  {
    if (component == null)
      return;
    if (!uid.HasValue)
      throw new DebugAssertException("Null entity uid cannot own a component. Component: " + component.GetType().Name);
    EntityUid owner = component.Owner;
    EntityUid? nullable = uid;
    if ((nullable.HasValue ? (owner != nullable.GetValueOrDefault() ? 1 : 0) : 1) != 0)
      throw new DebugAssertException($"Entity {uid} is not the owner of the component. Component: {component.GetType().Name}");
  }

  [Conditional("DEBUG")]
  public static void Assert([DoesNotReturnIf(false)] bool condition, string message)
  {
    if (!condition)
      throw new DebugAssertException(message);
  }

  [Conditional("DEBUG")]
  public static void Assert(
    [DoesNotReturnIf(false)] bool condition,
    [InterpolatedStringHandlerArgument("condition")] ref DebugTools.AssertInterpolatedStringHandler message)
  {
    if (!condition)
      throw new DebugAssertException(message.ToStringAndClear());
  }

  [Conditional("DEBUG")]
  public static void AssertNotNull([NotNull] object? arg, string? message = null)
  {
    if (arg == null)
      throw new DebugAssertException(message ?? "value cannot be null");
  }

  [Conditional("DEBUG")]
  public static void AssertNull(object? arg, string? message = null)
  {
    if (arg != null)
      throw new DebugAssertException(message ?? "value should be null");
  }

  public static void Break() => Debugger.Break();

  [InterpolatedStringHandler]
  public ref struct AssertInterpolatedStringHandler
  {
    private DefaultInterpolatedStringHandler _handler;

    public AssertInterpolatedStringHandler(
      int literalLength,
      int formattedCount,
      bool condition,
      out bool shouldAppend)
    {
      if (condition)
      {
        shouldAppend = false;
        this._handler = new DefaultInterpolatedStringHandler();
      }
      else
      {
        shouldAppend = true;
        this._handler = new DefaultInterpolatedStringHandler(literalLength, formattedCount);
      }
    }

    public string ToStringAndClear() => this._handler.ToStringAndClear();

    public override string ToString() => this._handler.ToString();

    public void AppendLiteral(string value) => this._handler.AppendLiteral(value);

    public void AppendFormatted<T>(T value) => this._handler.AppendFormatted<T>(value);

    public void AppendFormatted<T>(T value, string? format)
    {
      this._handler.AppendFormatted<T>(value, format);
    }
  }
}
