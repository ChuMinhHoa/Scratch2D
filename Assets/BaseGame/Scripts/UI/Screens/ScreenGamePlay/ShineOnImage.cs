using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ShineOnImage : MonoBehaviour
{
    private static readonly int StartTime = Shader.PropertyToID("_StartTime");
    public Image _renderer;
    
    [Button]
    public void ActiveGlow()
    {
        if (_renderer == null) return;
    
        // Create material instance if not already created
        if (_renderer.material == _renderer.defaultMaterial)
        {
            _renderer.material = new Material(_renderer.material);
        }

        // Set start time unique to this renderer
        _renderer.material.SetFloat(StartTime, Time.time);
    }
}
