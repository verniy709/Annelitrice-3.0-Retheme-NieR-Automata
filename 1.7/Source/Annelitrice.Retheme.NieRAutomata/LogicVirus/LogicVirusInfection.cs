using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Annelitrice.Retheme.NieRAutomata
{
    public sealed class CompProperties_LogicVirusInfection : CompProperties_AbilityEffect
    {
        public CompProperties_LogicVirusInfection()
        {
            compClass = typeof(CompAbilityEffect_LogicVirusInfection);
        }
    }

    public sealed class CompAbilityEffect_LogicVirusInfection : CompAbilityEffect
    {
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            if (parent != null)
                LogicVirusInfection.Start(parent.pawn, target.Pawn);
        }
    }

    public sealed class LogicVirusInfection : MapComponent
    {
        private sealed class Instance
        {
            internal Pawn caster, recipient;
            internal int startTick;
            internal LogicVirusAudioTiming audio;
        }
        private readonly List<Instance> active = new List<Instance>();
        private readonly List<LogicVirusGeometry.Quad> quads = new List<LogicVirusGeometry.Quad>(256);
        private static Material material;
        private static Material glowMaterial;
        private MaterialPropertyBlock properties;

        public LogicVirusInfection(Map map) : base(map) { }

        internal static void Start(Pawn caster, Pawn recipient)
        {
            if (caster == null || recipient == null || !caster.Spawned || !recipient.Spawned ||
                caster.Dead || recipient.Dead || caster.Map == null || caster.Map != recipient.Map)
                return;
            LogicVirusInfection controller = caster.Map.GetComponent<LogicVirusInfection>();
            if (controller == null) return;
            // Bound purely cosmetic work during repeated developer spawning.
            if (controller.active.Count >= 32) controller.active.RemoveAt(0);
            int speed = LogicVirusAudioTiming.MainSpeed(Find.TickManager.TickRateMultiplier);
            controller.active.Add(new Instance {
                caster = caster, recipient = recipient, startTick = Find.TickManager.TicksGame,
                audio = new LogicVirusAudioTiming(Time.realtimeSinceStartup, speed)
            });
            PlaySound(recipient, "Anneli_Retheme_NieRAutomata_LogicVirusInfect" +
                (speed == 1 ? "" : "_Fast" + speed));
        }

        private static void PlaySound(Pawn recipient, string defName)
        {
            SoundDef sound = DefDatabase<SoundDef>.GetNamedSilentFail(defName);
            if (sound != null)
                sound.PlayOneShot(SoundInfo.InMap(new TargetInfo(recipient)));
        }

        private bool Valid(Instance instance)
        {
            return instance.caster != null && instance.recipient != null &&
                instance.caster.Spawned && instance.recipient.Spawned &&
                !instance.caster.Dead && !instance.recipient.Dead &&
                instance.caster.Map == map && instance.recipient.Map == map;
        }

        public override void MapComponentTick()
        {
            // Rendering updates service the short real-time gap even while paused.
            // Tick cleanup prevents off-map sounds being queued until the player returns.
            for (int i = active.Count - 1; i >= 0; i--)
            {
                Instance instance = active[i];
                int age = Find.TickManager.TicksGame - instance.startTick;
                if (!Valid(instance) ||
                    (age >= LogicVirusGeometry.DurationTicks && map != Find.CurrentMap))
                    active.RemoveAt(i);
            }
        }

        public override void MapComponentUpdate()
        {
            if (active.Count == 0) return;
            bool drawingMap = map == Find.CurrentMap && WorldRendererUtility.DrawingMap;
            double now = Time.realtimeSinceStartup;
            for (int i = active.Count - 1; i >= 0; i--)
            {
                Instance instance = active[i];
                if (!Valid(instance)) { active.RemoveAt(i); continue; }
                int age = Find.TickManager.TicksGame - instance.startTick;
                if (instance.audio.Advance(age, now))
                {
                    if (drawingMap)
                        PlaySound(instance.recipient, "Anneli_Retheme_NieRAutomata_LogicVirusFinish");
                }
                if (instance.audio.Finished) active.RemoveAt(i);
            }
            if (active.Count == 0 || !drawingMap) return;
            if (material == null)
                material = MaterialPool.MatFrom(
                    "Anneli_Retheme_NieRAutomata/Anneli_Effect/Solid", ShaderDatabase.Mote);
            if (glowMaterial == null)
                glowMaterial = MaterialPool.MatFrom(
                    "Anneli_Retheme_NieRAutomata/Anneli_Effect/SoftGlow", ShaderDatabase.MoteGlow);
            if (properties == null) properties = new MaterialPropertyBlock();

            float altitude = AltitudeLayer.MoteOverhead.AltitudeFor();
            for (int i = active.Count - 1; i >= 0; i--)
            {
                Instance instance = active[i];
                if (!Valid(instance)) continue;
                Vector3 from = instance.caster.DrawPos;
                Vector3 to = instance.recipient.DrawPos;
                LogicVirusGeometry.Build(Find.TickManager.TicksGame - instance.startTick,
                    new LogicVirusGeometry.Point(from.x, from.z + 0.14f),
                    new LogicVirusGeometry.Point(to.x, to.z + 0.14f), quads);
                for (int n = 0; n < quads.Count; n++)
                {
                    LogicVirusGeometry.Quad quad = quads[n];
                    properties.SetColor(ShaderPropertyIDs.Color,
                        new Color(quad.r, quad.g, quad.b, quad.a));
                    Matrix4x4 matrix = Matrix4x4.TRS(
                        new Vector3(quad.x, altitude + quad.layer * 0.0005f, quad.z),
                        Quaternion.AngleAxis(-quad.angle * Mathf.Rad2Deg, Vector3.up),
                        new Vector3(quad.w, 1f, quad.h));
                    Graphics.DrawMesh(MeshPool.plane10, matrix,
                        quad.glow ? glowMaterial : material, 0, null, 0, properties);
                }
            }
        }
        // Transient visuals intentionally expire on reload; no game state is owned here.
    }
}
