using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld.Planet;

namespace Annelitrice.Retheme.NieRAutomata
{
    public sealed class SubEffecter_ReconstructionComplete : SubEffecter
    {
        public SubEffecter_ReconstructionComplete(SubEffecterDef def,Effecter parent):base(def,parent) { }
        public override void SubTrigger(TargetInfo A,TargetInfo B,int overrideSpawnTick,bool force)
        {
            Map map=A.Map??B.Map;
            if(map==null) return;
            IntVec3 cell=A.Map==map?A.Cell:B.Cell;
            if(cell.InBounds(map)) ReconstructionComplete.Start(map,cell);
        }
    }
    public sealed class ReconstructionComplete : MapComponent
    {
        private sealed class Instance
        {
            internal Vector3 origin;
            internal Pawn adult;
            internal int startTick;
            internal ReconstructionAudioTiming audio=new ReconstructionAudioTiming();
        }
        private readonly List<Instance> active=new List<Instance>();
        private readonly List<LogicVirusGeometry.Quad> quads=new List<LogicVirusGeometry.Quad>(384);
        private static Material solid,glow;
        private MaterialPropertyBlock properties;
        public ReconstructionComplete(Map map):base(map) { }
        private bool Claimed(Pawn pawn)
        {
            for(int i=0;i<active.Count;i++) if(active[i].adult==pawn) return true;
            return false;
        }
        internal static void Start(Map map,IntVec3 cell)
        {
            ReconstructionComplete controller=map.GetComponent<ReconstructionComplete>();
            if(controller==null) return;
            int now=Find.TickManager.TicksGame;
            Pawn adult=null;
            float best=64f;
            // TryDrop(...Near) has already released the pawn, possibly into a nearby cell.
            // TickSpawned identifies this tick's release; never attach to an older bystander.
            IReadOnlyList<Pawn> pawns=map.mapPawns.AllPawnsSpawned;
            for(int i=0;i<pawns.Count;i++)
            {
                Pawn pawn=pawns[i];
                if(pawn.def.defName!="Annelitrice" || pawn.TickSpawned!=now || controller.Claimed(pawn)) continue;
                float distance=(pawn.Position.ToVector3Shifted()-cell.ToVector3Shifted()).sqrMagnitude;
                if(distance<best) {best=distance;adult=pawn;}
            }
            if(controller.active.Count>=24) controller.active.RemoveAt(0);
            var instance=new Instance {origin=cell.ToVector3Shifted(),adult=adult,startTick=now};
            controller.active.Add(instance);
            controller.PlaySound(instance,"Anneli_Retheme_NieRAutomata_ReconstructionCompleteBody");
        }
        private void PlaySound(Instance instance,string defName)
        {
            SoundDef sound=DefDatabase<SoundDef>.GetNamedSilentFail(defName);
            if(sound==null) return;
            TargetInfo target=instance.adult==null?new TargetInfo(instance.origin.ToIntVec3(),map):new TargetInfo(instance.adult);
            sound.PlayOneShot(SoundInfo.InMap(target));
        }
        private void AdvanceAudio(Instance instance)
        {
            // Consume hidden-map events so returning to this map cannot replay the accent.
            if(instance.audio.Advance(Find.TickManager.TicksGame-instance.startTick) &&
                map==Find.CurrentMap && WorldRendererUtility.DrawingMap)
                PlaySound(instance,"Anneli_Retheme_NieRAutomata_ReconstructionCompleteDismantle");
        }
        private bool Valid(Instance instance)
        {
            int age=Find.TickManager.TicksGame-instance.startTick;
            if(age<0 || age>=ReconstructionCompleteGeometry.DurationTicks) return false;
            return instance.adult==null || (instance.adult.Spawned&&!instance.adult.Dead&&instance.adult.Map==map);
        }
        public override void MapComponentTick()
        {
            for(int i=active.Count-1;i>=0;i--)
                if(!Valid(active[i])) active.RemoveAt(i);
                else AdvanceAudio(active[i]);
        }
        public override void MapComponentUpdate()
        {
            if(active.Count==0 || map!=Find.CurrentMap || !WorldRendererUtility.DrawingMap) return;
            if(solid==null) solid=MaterialPool.MatFrom("Anneli_Retheme_NieRAutomata/Anneli_Effect/Solid",ShaderDatabase.Mote);
            if(glow==null) glow=MaterialPool.MatFrom("Anneli_Retheme_NieRAutomata/Anneli_Effect/SoftGlow",ShaderDatabase.MoteGlow);
            if(properties==null) properties=new MaterialPropertyBlock();
            float altitude=AltitudeLayer.MoteOverhead.AltitudeFor();
            for(int i=0;i<active.Count;i++)
            {
                Instance instance=active[i];
                if(!Valid(instance)) continue;
                AdvanceAudio(instance);
                Vector3 adult=instance.adult==null?instance.origin:instance.adult.DrawPos;
                ReconstructionCompleteGeometry.Build(Find.TickManager.TicksGame-instance.startTick,
                    new LogicVirusGeometry.Point(instance.origin.x,instance.origin.z),
                    new LogicVirusGeometry.Point(adult.x,adult.z+.035f),quads);
                for(int n=0;n<quads.Count;n++)
                {
                    LogicVirusGeometry.Quad quad=quads[n];
                    properties.SetColor(ShaderPropertyIDs.Color,new Color(quad.r,quad.g,quad.b,quad.a));
                    Matrix4x4 matrix=Matrix4x4.TRS(new Vector3(quad.x,altitude+quad.layer*.0005f,quad.z),
                        Quaternion.AngleAxis(-quad.angle*Mathf.Rad2Deg,Vector3.up),new Vector3(quad.w,1,quad.h));
                    Graphics.DrawMesh(MeshPool.plane10,matrix,quad.glow?glow:solid,0,null,0,properties);
                }
            }
        }
    }
}
