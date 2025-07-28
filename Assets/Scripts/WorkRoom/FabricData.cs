using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    [CreateAssetMenu(fileName = "NewFabric", menuName = "Custom/Fabric")]
    public class FabricData : ScriptableObject
    {
        public string fabricName;
        public Sprite fabricSprite;
        public MaterialData[] requiredMaterials;
    }
