using System.Numerics;
using System.Threading.Tasks;

namespace Content.Client.Parallax.Managers;

public interface IParallaxManager
{
	Vector2 ParallaxAnchor { get; set; }

	bool IsLoaded(string name);

	ParallaxLayerPrepared[] GetParallaxLayers(string name);

	void LoadDefaultParallax();

	Task LoadParallaxByName(string name);

	void UnloadParallax(string name);
}
