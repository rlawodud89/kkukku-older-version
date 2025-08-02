using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    [CreateAssetMenu(fileName = "NewBlanket", menuName = "Custom/Blanket")]
    public class BlanketData : ScriptableObject
    {
        public string BlanketName;
        public Sprite BlanketSprite;
        public MaterialInventoryEntry[] requiredMaterials;
        public Sprite Fabric;
        public int FabricCount;
    }
