/*
	SetRenderQueue.cs
 
	Sets the RenderQueue of an object's materials on Awake. This will instance
	the materials, so the script won't interfere with other renderers that
	reference the same materials.
*/

using UnityEngine;

namespace Assets.Scripts
{
    [AddComponentMenu("Rendering/SetRenderQueue")]

    public class SetRenderQueue : MonoBehaviour
    {

        [SerializeField]
        protected int[] m_queues = new int[] { 3000 };

        private Renderer _Renderer;
        private Renderer Renderer
        {
            get
            {
                if (_Renderer == null) { _Renderer = GetComponent<Renderer>(); }
                return _Renderer;
            }
        }
        protected void Awake()
        {
            Material[] materials = Renderer.materials;
            for (int i = 0; i < materials.Length && i < m_queues.Length; ++i)
            {
                materials[i].renderQueue = m_queues[i];
            }
        }
    }
}