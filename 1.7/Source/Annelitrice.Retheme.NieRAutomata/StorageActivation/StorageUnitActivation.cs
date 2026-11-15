using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld.Planet;

namespace Annelitrice.Retheme.NieRAutomata
{
    public sealed class SubEffecter_StorageUnitActivation : SubEffecter
    {
        public SubEffecter_StorageUnitActivation(SubEffecterDef def, Effecter parent) : base(def,parent) { }
        public override void SubTrigger(TargetInfo A, TargetInfo B, int overrideSpawnTick, bool force)
        {
            Map map=A.Map ?? B.Map;
            if (map==null) return;
            IntVec3 cell=A.Map==map?A.Cell:B.Cell;
            if (cell.InBounds(map)) StorageUnitActivation.Start(map,cell);
        }
    }

    public sealed class StorageUnitActivation : MapComponent
    {
        private sealed class Instance
        {
            internal Vector3 origin;
            internal Pawn unit;
            internal int startTick;
        }
        private readonly List<Instance> active=new List<Instance>();
        private readonly List<LogicVirusGeometry.Quad> quads=new List<LogicVirusGeometry.Quad>(192);
        private static Material solid, glow;
        private MaterialPropertyBlock properties;

        public StorageUnitActivation(Map map):base(map) { }
        internal static void Start(Map map,IntVec3 cell)
        {
            Pawn unit=null;
            List<Thing> things=map.thingGrid.ThingsListAtFast(cell);
            for(int i=0;i<things.Count;i++)
                if(things[i] is Pawn pawn && pawn.def.defName=="AnnelitriceLarvaAsAnimal")
                { unit=pawn;break; }
            var controller=map.GetComponent<StorageUnitActivation>();
            if(controller==null) return;
            if(controller.active.Count>=32) controller.active.RemoveAt(0);
            controller.active.Add(new Instance {
                origin=cell.ToVector3Shifted(),unit=unit,startTick=Find.TickManager.TicksGame
            });
        }
        private bool Valid(Instance instance)
        {
            if(Find.TickManager.TicksGame-instance.startTick>=StorageActivationGeometry.DurationTicks) return false;
            return instance.unit==null || (instance.unit.Spawned && !instance.unit.Dead && instance.unit.Map==map);
        }
        public override void MapComponentTick()
        {
            for(int i=active.Count-1;i>=0;i--)
                if(!Valid(active[i])) active.RemoveAt(i);
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
                Vector3 unit=instance.unit==null?instance.origin:instance.unit.DrawPos;
                StorageActivationGeometry.Build(Find.TickManager.TicksGame-instance.startTick,
                    new LogicVirusGeometry.Point(instance.origin.x,instance.origin.z),
                    new LogicVirusGeometry.Point(unit.x,unit.z+.025f),quads);
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
