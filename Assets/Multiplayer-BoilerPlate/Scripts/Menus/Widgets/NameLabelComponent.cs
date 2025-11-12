using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NameLabelComponent : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameLabel;

    public void SetName(string nameToSet)
    {
        _nameLabel.text = $"-> {nameToSet}";
    }
}
