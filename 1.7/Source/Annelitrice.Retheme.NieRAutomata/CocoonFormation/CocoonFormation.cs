using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld.Planet;

namespace Annelitrice.Retheme.NieRAutomata
{
    public sealed class SubEffecter_CocoonFormation : SubEffecter
    {
        public SubEffecter_CocoonFormation(SubEffecterDef def,Effecter parent):base(def,parent) { }
        public override void SubTrigger(TargetInfo A,TargetInfo B,int overrideSpawnTick,bool force)
        {
            Map map=A.Map??B.Map;
            if(map==null) return;
            IntVec3 cell=A.Map==map?A.Cell:B.Cell;
            if(cell.InBounds(map)) CocoonFormation.Start(map,cell);
        }
    }
    public sealed class CocoonFormation : MapComponent
    {
        private sealed class Instance
        {
            internal IntVec3 cell;
            internal Thing cocoon;
            internal int startTick;
            internal CocoonFormationAudioTiming audio=new CocoonFormationAudioTiming();
        }
        private readonly List<Instance> active=new List<Instance>();
        private readonly List<LogicVirusGeometry.Quad> quads=new List<LogicVirusGeometry.Quad>(320);
        private static Material solid,glow;
        private MaterialPropertyBlock properties;
        public CocoonFormation(Map map):base(map) { }
        internal static void Start(Map map,IntVec3 cell)
        {
            CocoonFormation controller=map.GetComponent<CocoonFormation>();
            if(controller==null) return;
            if(controller.active.Count>=32) controller.active.RemoveAt(0);
            controller.active.Add(new Instance {cell=cell,startTick=Find.TickManager.TicksGame});
            controller.PlaySound(cell,"Anneli_Retheme_NieRAutomata_CocoonFormationBody");
        }
        private void PlaySound(IntVec3 cell,string defName)
        {
            SoundDef sound=DefDatabase<SoundDef>.GetNamedSilentFail(defName);
            if(sound!=null) sound.PlayOneShot(SoundInfo.InMap(new TargetInfo(cell,map)));
        }
        private void AdvanceAudio(Instance instance)
        {
            // Consume the event even off-screen so returning to this map cannot replay it.
            if(instance.audio.Advance(Find.TickManager.TicksGame-instance.startTick) &&
                map==Find.CurrentMap && WorldRendererUtility.DrawingMap)
                PlaySound(instance.cell,"Anneli_Retheme_NieRAutomata_CocoonFormationScan");
        }
        private bool Valid(Instance instance)
        {
            int age=Find.TickManager.TicksGame-instance.startTick;
            if(age<0 || age>=CocoonFormationGeometry.DurationTicks) return false;
            // CompLarva spawns the cocoon after triggering the effect, in the same call.
            // Bind after that call; do not keep the larva reference that it destroys.
            if(instance.cocoon==null)
            {
                List<Thing> things=map.thingGrid.ThingsListAtFast(instance.cell);
                for(int i=0;i<things.Count;i++)
                    if(things[i].def.defName=="Anneli_Pupa") {instance.cocoon=things[i];break;}
                if(instance.cocoon==null) return age<2;
            }
            return instance.cocoon.Spawned && !instance.cocoon.Destroyed && instance.cocoon.Map==map;
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
                Vector3 origin=instance.cocoon==null?instance.cell.ToVector3Shifted():instance.cocoon.DrawPos;
                CocoonFormationGeometry.Build(Find.TickManager.TicksGame-instance.startTick,
                    new LogicVirusGeometry.Point(origin.x,origin.z),quads);
                for(int n=0;n<quads.Count;n++)
                {
                    LogicVirusGeometry.Quad quad=quads[n];
                    properties.SetColor(ShaderPropertyIDs.Color,new Color(quad.r,quad.g,quad.b,quad.a));
                    Matrix4x4 matrix=Matrix4x4.TRS(
                        new Vector3(quad.x,altitude+quad.layer*.0005f,quad.z),
                        Quaternion.AngleAxis(-quad.angle*Mathf.Rad2Deg,Vector3.up),
                        new Vector3(quad.w,1,quad.h));
                    Graphics.DrawMesh(MeshPool.plane10,matrix,quad.glow?glow:solid,0,null,0,properties);
                }
            }
        }
    }
}
