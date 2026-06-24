using Robust.Shared.Log;

namespace Robust.Shared.EntitySerialization;

public record struct SerializationOptions()
{
	public static readonly SerializationOptions Default = new SerializationOptions();

	public MissingEntityBehaviour MissingEntityBehaviour = MissingEntityBehaviour.IncludeNullspace;

	public EntityExceptionBehaviour EntityExceptionBehaviour = EntityExceptionBehaviour.Rethrow;

	public bool ErrorOnOrphan = true;

	public LogLevel? LogAutoInclude = LogLevel.Info;

	public bool ExpectPreInit = false;

	public FileCategory Category = FileCategory.Unknown;
}
