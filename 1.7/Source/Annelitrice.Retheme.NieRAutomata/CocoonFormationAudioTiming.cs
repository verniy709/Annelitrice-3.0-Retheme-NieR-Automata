namespace Annelitrice.Retheme.NieRAutomata
{
    public sealed class CocoonFormationAudioTiming
    {
        // The scan geometry fades in from tick28 and first becomes visible at tick29.
        public const int ScanStartTick=29;
        public bool ScanStarted {get;private set;}
        public bool Advance(int ageTicks)
        {
            if(ScanStarted || ageTicks<ScanStartTick || ageTicks>=CocoonFormationGeometry.DurationTicks) return false;
            ScanStarted=true;
            return true;
        }
    }
}
