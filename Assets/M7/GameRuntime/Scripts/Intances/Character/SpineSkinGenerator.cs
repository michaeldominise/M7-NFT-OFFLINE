using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using Sirenix.OdinInspector;
using System.Linq;
using M7.CDN.Addressable;

namespace M7.GameRuntime
{
    [System.Serializable]
    public class SpineSkinGenerator
    {
        const string DefaultSkinName = "naomi";

        [SerializeField] SkeletonRenderer skeletonRenderer;
        [SerializeField] Renderer renderer;

        Skeleton Skeleton => skeletonRenderer?.skeleton;
        Material SourceMaterial => renderer?.material;
		
        [Button]
        public void SetSkin(params GameData.EquipmentItem[] equipementItems)
        {
            var assetReferenceCount = 0;
            var assetReferenceLoadCount = 0;
            foreach (var equipmentItem in equipementItems)
                if(equipmentItem != null)
                    foreach (var bodyPart in equipmentItem.BodyParts)
                        assetReferenceCount++;

            foreach (var equipmentItem in equipementItems)
                if (equipmentItem != null)
                    foreach (var bodyPart in equipmentItem.BodyParts)
                    {
                        bodyPart.Sprite?.LoadAssetAsync(result =>
                            {
                                assetReferenceLoadCount++;
                                if (assetReferenceCount == assetReferenceLoadCount)
	                                OnAssetReferencesLoaded(equipementItems);
                            });
                    }
        }

        void OnAssetReferencesLoaded(GameData.EquipmentItem[] equipementItems)
        {
            var templateSkin = Skeleton?.Skin;
            if (templateSkin == null)
                return;
	        //templateSkin.Clear();

            var generatedSkin = new Skin("GeneratedSkin");
            foreach (var equipmentItem in equipementItems)
            {
                foreach (var bodyPart in equipmentItem.BodyParts)
                {
                    if(!bodyPart.Sprite.Asset)
                        continue;
                    
                    var slot = Skeleton.FindSlot(bodyPart.Slot);
                    if (slot?.Data == null)
                        continue;
	                var attachment = templateSkin.GetAttachment(slot.Data.Index, bodyPart.SlotKey);
	                // solution for overlapping textures.

                    var newAttachment = attachment.GetRemappedClone(bodyPart.Sprite.Asset as Sprite, SourceMaterial, pivotShiftsMeshUVCoords: false, useOriginalRegionSize: true, useOriginalRegionScale: true);
                    generatedSkin.SetAttachment(slot.Data.Index, bodyPart.SlotKey, newAttachment);
                }
            }

            templateSkin.AddSkin(generatedSkin);
            Skeleton.SetSlotsToSetupPose();
            //skeletonRenderer.Update(0);
        }
    }
}
