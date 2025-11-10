using UnityEngine;

public class ChairColorController : MonoBehaviour
{
    [Header("Chair Warning Materials")]
    public Material warningMaterial_Yellow;
    public Material warningMaterial_Black;

    [Header("Normal Chair Sample")]
    public MeshRenderer normalChairSample;

    [Header("Chairs to Change Color")]
    public MeshRenderer[] yellowChairs;
    public MeshRenderer[] blackChairs;

    private Material normalMaterial;

    void Start()
    {
        normalMaterial = normalChairSample.material;
    }
    
    // 設置為警告圖案
    public void SetWarningPattern()
    {
        foreach (var renderer in yellowChairs)
        {
            renderer.material = warningMaterial_Yellow;
        }
        foreach (var renderer in blackChairs)
        {
            renderer.material = warningMaterial_Black;
        }
    }

    // 恢復正常顏色
    public void SetNormalPattern()
    {
        foreach (var renderer in yellowChairs)
        {
            renderer.material = normalMaterial;
        }
        foreach (var renderer in blackChairs)
        {
            renderer.material = normalMaterial;
        }
    }
}