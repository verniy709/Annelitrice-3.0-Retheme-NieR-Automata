namespace Annelitrice.Retheme.NieRAutomata
{
    public sealed class ReconstructionAudioTiming
    {
        // Tick6 is the unlock-flash peak and the first strong casing separation.
        public const int DismantleStartTick=6;
        public bool DismantleStarted {get;private set;}
        public bool Advance(int ageTicks)
        {
            if(DismantleStarted || ageTicks<DismantleStartTick || ageTicks>=ReconstructionCompleteGeometry.DurationTicks) return false;
            DismantleStarted=true;
            return true;
        }
    }
}
