using System.Runtime.CompilerServices;
using Serilog.Events;

namespace Robust.Shared.Log;

public static class LogExt
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static LogLevel ToRobust(this LogEventLevel level)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Expected I4, but got Unknown
		return (LogLevel)level;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static LogEventLevel ToSerilog(this LogLevel level)
	{
		return (LogEventLevel)level;
	}
}
