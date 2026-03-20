using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteAlways]
public class ShineOnEnable : MonoBehaviour
{
    private static readonly int StartTime = Shader.PropertyToID("_StartTime");
    public MaterialPropertyBlock _propertyBlock;
    public Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _propertyBlock = new MaterialPropertyBlock();
    }

    [Button]
    public void ActiveGlow()
    {
        if (_renderer == null) return;
        
        // Get existing property block to preserve other properties
        _renderer.GetPropertyBlock(_propertyBlock);
        
        // Set start time unique to this renderer
        _propertyBlock.SetFloat(StartTime, Time.time);
        
        // Apply - this does NOT create a new material instance
        _renderer.SetPropertyBlock(_propertyBlock);
    }
}