using System;
using System.Collections.Generic;

namespace Annelitrice.Retheme.NieRAutomata
{
    // Warm-white initialization: a short flash, opening data, a scan, then settling light.
    public static class StorageActivationGeometry
    {
        public const int DurationTicks = 64;
        private static float Clamp(float v) { return v < 0 ? 0 : v > 1 ? 1 : v; }
        private static float Smooth(float a, float b, float t)
        { float p = Clamp((t - a) / (b - a)); return p * p * (3 - 2 * p); }
        private static float Window(float t, float a, float b, float c, float d)
        { return Smooth(a,b,t) * (1-Smooth(c,d,t)); }

        private static void Rect(List<LogicVirusGeometry.Quad> q, LogicVirusGeometry.Point p,
            float x, float z, float w, float h, float alpha, int layer,
            bool dark = false, float angle = 0)
        {
            if (alpha < .005f || w <= 0 || h <= 0) return;
            q.Add(new LogicVirusGeometry.Quad {
                x = p.x+x, z = p.z+z, w = w, h = h, angle = angle,
                r = dark ? .065f : .91f, g = dark ? .070f : .90f,
                b = dark ? .065f : .85f, a = Clamp(alpha), layer = layer
            });
        }
        private static void Glow(List<LogicVirusGeometry.Quad> q, LogicVirusGeometry.Point p,
            float x, float z, float w, float h, float alpha, int layer, float angle = 0)
        {
            if (alpha < .005f) return;
            q.Add(new LogicVirusGeometry.Quad {
                x = p.x+x, z = p.z+z, w = w, h = h, angle = angle,
                r = 1, g = .98f, b = .90f, a = Clamp(alpha), layer = layer, glow = true
            });
        }
        private static void Chip(List<LogicVirusGeometry.Quad> q, LogicVirusGeometry.Point p,
            float x, float z, float w, float h, float alpha, bool dark, int layer, float angle = 0)
        {
            Rect(q,p,x,z,w+.016f,h+.016f,alpha*.48f,layer,!dark,angle);
            Rect(q,p,x,z,w,h,alpha,layer+1,dark,angle);
            if (!dark) Glow(q,p,x,z,w+.12f,h+.12f,alpha*.36f,layer+1,angle);
        }
        private static void Ring(List<LogicVirusGeometry.Quad> q, LogicVirusGeometry.Point p,
            float radius, float alpha, int count, float rotation, int layer, float thickness)
        {
            if (alpha < .005f) return;
            const float flat = .43f;
            for (int i=0;i<count;i++)
            {
                // Three open sectors keep the small ring readable as a machine pulse.
                if (i % 6 >= 4) continue;
                float theta = (float)(i*Math.PI*2/count)+rotation;
                float x=(float)Math.Cos(theta)*radius;
                float z=(float)Math.Sin(theta)*radius*flat-.34f;
                float angle=(float)Math.Atan2(flat*Math.Cos(theta),-Math.Sin(theta));
                float length=radius*(float)(Math.PI*2/count)*.68f;
                Rect(q,p,x,z,length+.016f,thickness+.020f,alpha*.42f,layer,true,angle);
                Rect(q,p,x,z,length,thickness,alpha,layer+1,false,angle);
                Glow(q,p,x,z,length+.12f,thickness+.09f,alpha*.45f,layer+1,angle);
            }
        }

        public static void Build(float tick, LogicVirusGeometry.Point origin,
            LogicVirusGeometry.Point unit, List<LogicVirusGeometry.Quad> q)
        {
            q.Clear();
            if (tick < 0 || tick >= DurationTicks) return;
            float firstRing=Window(tick,1,7,28,47);
            float radius=.18f+.43f*Smooth(1,22,tick);
            Ring(q,origin,radius,firstRing*.88f,18,.08f+tick*.006f,2,.021f);
            float secondRing=Window(tick,12,20,35,54);
            Ring(q,origin,.29f+.51f*Smooth(12,42,tick),secondRing*.40f,12,-.14f,4,.014f);

            // GenSpawn has already replaced the item: this first-frame veil bridges it.
            float cover=1-Smooth(3,13,tick);
            for (int i=0;i<4;i++)
            {
                float opening=Smooth(3,15,tick);
                float side=i<2?-1:1;
                float x=(i-1.5f)*.18f+side*.14f*opening;
                float z=.015f+(i%2==0?.035f:-.035f)*opening;
                Rect(q,unit,x,z,.176f,.82f,cover*.83f,10,i%2==0);
                Rect(q,unit,x,z+.41f,.16f,.016f,cover*.9f,11);
            }
            float ignition=1-Smooth(0,9,tick);
            Glow(q,unit,0,.01f,.95f,1.12f,ignition*.92f,14);
            Glow(q,unit,0,.03f,.70f,.18f,ignition*.68f,15);

            // Two short staves unfold with the initialization, then give the body room.
            float brace=Window(tick,0,3,10,19);
            float braceX=.26f+.18f*Smooth(1,17,tick);
            for (int side=-1;side<=1;side+=2)
            {
                Rect(q,unit,side*braceX,0,.018f,.55f,brace*.73f,16);
                Rect(q,unit,side*(braceX-.065f),.275f,.13f,.018f,brace*.73f,16);
                Glow(q,unit,side*braceX,0,.14f,.65f,brace*.27f,16);
            }

            // Broad sheets become a few clear data chips rather than a smoke cloud.
            for (int i=0;i<24;i++)
            {
                float p=(tick-(4+i%5*1.8f))/(18+i%4*4f);
                if (p<=0 || p>=1) continue;
                float theta=i*2.399963f+.22f;
                float r=.16f+(.37f+i%3*.07f)*(1-(1-p)*(1-p));
                float x=(float)Math.Cos(theta)*r;
                float z=(float)Math.Sin(theta)*r*.72f+.11f*p;
                float alpha=Smooth(0,.12f,p)*(1-Smooth(.25f,1,p))*.88f;
                float size=(.040f+i%4*.013f)*(1-.28f*p);
                Chip(q,unit,x,z,size,size*(i%3==0?.42f:.8f),alpha,
                    i%4==0,20,i%3==0?theta*.20f:0);
            }

            // One upward scan implies activation, with the real unit fully revealed early.
            float scan=Window(tick,6,10,25,31);
            float scanZ=-.32f+.72f*Smooth(7,26,tick);
            Rect(q,unit,0,scanZ,.65f,.015f,scan*.85f,24);
            Glow(q,unit,0,scanZ,.90f,.17f,scan*.55f,24);
            for (int i=0;i<4;i++)
                Rect(q,unit,-.24f+i*.16f,scanZ+.035f,.065f,.015f,scan*.42f,25,true);

            // The last particles lift gently and disappear, leaving a clear small machine.
            for (int i=0;i<8;i++)
            {
                float p=(tick-(20+i%3*3))/(24+i%2*8f);
                if (p<=0 || p>=1) continue;
                float theta=i*2.399963f+.5f;
                float r=.33f+.18f*p;
                float alpha=(float)Math.Sin(Math.PI*p)*(1-p)*.65f;
                Chip(q,unit,(float)Math.Cos(theta)*r,
                    (float)Math.Sin(theta)*r*.75f+.22f*p,
                    .032f*(1-.4f*p),.025f*(1-.4f*p),alpha,i%4==0,28);
            }
        }
    }
}
