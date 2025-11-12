using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "ScriptableObject/Store/ItemSo")]
public class ItemSO : ScriptableObject
{
    public ItemName itemName;
    public ItemProperty property;

    #region Utility

    [ContextMenu("Set ItemName as FileName")]
    private void SetItemNameAsFileName()
    {
        string val = itemName.ToString();
        //SetFileName(val);
        property.name = itemName;
    }

#if UNITY_EDITOR
    // private void SetFileName(string val)
    // {
    //     string assetPath =  AssetDatabase.GetAssetPath(this.GetInstanceID());
    //     AssetDatabase.RenameAsset(assetPath, val);
    //     AssetDatabase.SaveAssets();
    // }
#endif
    
    #endregion

    
    
}
