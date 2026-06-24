using System;

namespace Robust.Shared.Utility;

internal struct LineEnumerator(ReadOnlyMemory<char> text)
{
	private readonly ReadOnlyMemory<char> _text = text;

	private int _curPos = 0;

	public bool MoveNext(out int start, out int end)
	{
		if (_curPos == _text.Length)
		{
			start = 0;
			end = 0;
			return false;
		}
		ReadOnlySpan<char> span = _text.Span;
		int curPos = _curPos;
		int num = span.Slice(curPos, span.Length - curPos).IndexOf('\n');
		int num2 = ((num != -1) ? (num + _curPos + 1) : _text.Length);
		start = _curPos;
		end = num2;
		_curPos = num2;
		return true;
	}
}
