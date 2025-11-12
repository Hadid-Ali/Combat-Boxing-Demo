using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class RenderObject
{
    public ItemName Iname;
    public RenderTexture Texture;
}
[CreateAssetMenu(fileName = "RenderRegistry", menuName = "ScriptableObjects/RenderRegistry", order = 1)]
public class RenderRegistry : ScriptableObject
{
    public List<RenderObject> RenderTextures = new List<RenderObject>();

    public RenderTexture GetRenderTexture(ItemName name)
    {
        return RenderTextures.Find(x=> x.Iname == name).Texture;
    }
}
