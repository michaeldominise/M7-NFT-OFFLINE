namespace M7.FX.VFX.Scripts
{
    public struct VFXData
    {
        public VFXData(VfxTargetData vfxTargetData, int instanceCount)
        {
            VfxTargetData = vfxTargetData;
            InstanceCount = instanceCount;
        }

        public VfxTargetData VfxTargetData { get; }

        public int InstanceCount { get; }
    }
}