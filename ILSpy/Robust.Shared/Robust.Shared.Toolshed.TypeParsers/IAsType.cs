namespace Robust.Shared.Toolshed.TypeParsers;

public interface IAsType<out T>
{
	T AsType();
}
