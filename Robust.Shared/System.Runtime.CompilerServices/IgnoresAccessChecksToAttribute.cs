namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
internal sealed class IgnoresAccessChecksToAttribute : Attribute
{
	private readonly string assemblyName;

	public string AssemblyName => assemblyName;

	public IgnoresAccessChecksToAttribute(string assemblyName)
	{
		this.assemblyName = assemblyName;
	}
}
