namespace Robust.Shared.Utility;

internal struct Ptr<T> where T : unmanaged
{
	public unsafe T* P;

	public unsafe static implicit operator T*(Ptr<T> t)
	{
		return t.P;
	}

	public unsafe static implicit operator Ptr<T>(T* ptr)
	{
		return new Ptr<T>
		{
			P = ptr
		};
	}
}
