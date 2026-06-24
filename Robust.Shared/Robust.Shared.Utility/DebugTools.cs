using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Robust.Shared.Utility;

public static class DebugTools
{
	[InterpolatedStringHandler]
	public ref struct AssertInterpolatedStringHandler
	{
		private DefaultInterpolatedStringHandler _handler;

		public AssertInterpolatedStringHandler(int literalLength, int formattedCount, bool condition, out bool shouldAppend)
		{
			if (condition)
			{
				shouldAppend = false;
				_handler = default(DefaultInterpolatedStringHandler);
			}
			else
			{
				shouldAppend = true;
				_handler = new DefaultInterpolatedStringHandler(literalLength, formattedCount);
			}
		}

		public string ToStringAndClear()
		{
			return _handler.ToStringAndClear();
		}

		public override string ToString()
		{
			return _handler.ToString();
		}

		public void AppendLiteral(string value)
		{
			_handler.AppendLiteral(value);
		}

		public void AppendFormatted<T>(T value)
		{
			_handler.AppendFormatted(value);
		}

		public void AppendFormatted<T>(T value, string? format)
		{
			_handler.AppendFormatted(value, format);
		}
	}

	[Conditional("DEBUG")]
	[DoesNotReturn]
	public static void Assert(string message)
	{
		throw new DebugAssertException(message);
	}

	[Conditional("DEBUG")]
	public static void Assert([DoesNotReturnIf(false)] bool condition)
	{
		if (!condition)
		{
			throw new DebugAssertException();
		}
	}

	[Conditional("DEBUG")]
	public static void AssertEqual(object? objA, object? objB)
	{
		if (objA == objB || (objA != null && objA.Equals(objB)))
		{
			return;
		}
		throw new DebugAssertException($"Expected: {objB ?? "null"} but was {objA ?? "null"}");
	}

	[Conditional("DEBUG")]
	public static void AssertEqual(object? objA, object? objB, string message)
	{
		if (objA == objB || (objA != null && objA.Equals(objB)))
		{
			return;
		}
		throw new DebugAssertException($"{message}\nExpected: {objB ?? "null"} but was {objA ?? "null"}");
	}

	[Conditional("DEBUG")]
	public static void AssertEqual<T>(T? objA, T? objB) where T : IEquatable<T>
	{
		if (objA == null && objB == null)
		{
			return;
		}
		if (objA != null)
		{
			T? other = objB;
			if (objA.Equals(other))
			{
				return;
			}
		}
		throw new DebugAssertException("Expected: " + (objB?.ToString() ?? "null") + " but was " + (objA?.ToString() ?? "null"));
	}

	[Conditional("DEBUG")]
	public static void AssertEqual<T>(T? objA, T? objB, string message) where T : IEquatable<T>
	{
		if (objA == null && objB == null)
		{
			return;
		}
		if (objA != null)
		{
			T? other = objB;
			if (objA.Equals(other))
			{
				return;
			}
		}
		throw new DebugAssertException($"{message}\nExpected: {objB?.ToString() ?? "null"} but was {objA?.ToString() ?? "null"}");
	}

	[Conditional("DEBUG")]
	public static void AssertNotEqual(object? objA, object? objB)
	{
		if (objA == objB)
		{
			throw new DebugAssertException($"Expected: not {objB ?? "null"}");
		}
		if (objA == null || !objA.Equals(objB))
		{
			return;
		}
		throw new DebugAssertException($"Expected: not {objB}");
	}

	[Conditional("DEBUG")]
	public static void AssertNotEqual(object? objA, object? objB, string message)
	{
		if (objA == objB)
		{
			throw new DebugAssertException($"{message}\nExpected: not {objB ?? "null"}");
		}
		if (objA == null || !objA.Equals(objB))
		{
			return;
		}
		throw new DebugAssertException($"{message}\nExpected: not {objB}");
	}

	[Conditional("DEBUG")]
	public static void AssertNotEqual<T>(T? objA, T? objB) where T : IEquatable<T>
	{
		if (objA == null && objB == null)
		{
			throw new DebugAssertException("Expected: not null");
		}
		if (objA == null || !objA.Equals(objB))
		{
			return;
		}
		throw new DebugAssertException("Expected: not " + (objB?.ToString() ?? "null"));
	}

	[Conditional("DEBUG")]
	public static void AssertNotEqual<T>(T? objA, T? objB, string message) where T : IEquatable<T>
	{
		if (objA == null && objB == null)
		{
			throw new DebugAssertException(message + "\nExpected: not null");
		}
		if (objA == null || !objA.Equals(objB))
		{
			return;
		}
		throw new DebugAssertException(message + "\nExpected: not " + (objB?.ToString() ?? "null"));
	}

	[Conditional("DEBUG")]
	public static void AssertOwner(EntityUid? uid, IComponent? component)
	{
		if (component != null)
		{
			if (!uid.HasValue)
			{
				throw new DebugAssertException("Null entity uid cannot own a component. Component: " + component.GetType().Name);
			}
			EntityUid owner = component.Owner;
			EntityUid? entityUid = uid;
			if (owner != entityUid)
			{
				throw new DebugAssertException($"Entity {uid} is not the owner of the component. Component: {component.GetType().Name}");
			}
		}
	}

	[Conditional("DEBUG")]
	public static void Assert([DoesNotReturnIf(false)] bool condition, string message)
	{
		if (!condition)
		{
			throw new DebugAssertException(message);
		}
	}

	[Conditional("DEBUG")]
	public static void Assert([DoesNotReturnIf(false)] bool condition, [InterpolatedStringHandlerArgument("condition")] ref AssertInterpolatedStringHandler message)
	{
		if (!condition)
		{
			throw new DebugAssertException(message.ToStringAndClear());
		}
	}

	[Conditional("DEBUG")]
	public static void AssertNotNull([NotNull] object? arg, string? message = null)
	{
		if (arg == null)
		{
			throw new DebugAssertException(message ?? "value cannot be null");
		}
	}

	[Conditional("DEBUG")]
	public static void AssertNull(object? arg, string? message = null)
	{
		if (arg != null)
		{
			throw new DebugAssertException(message ?? "value should be null");
		}
	}

	public static void Break()
	{
		Debugger.Break();
	}
}
