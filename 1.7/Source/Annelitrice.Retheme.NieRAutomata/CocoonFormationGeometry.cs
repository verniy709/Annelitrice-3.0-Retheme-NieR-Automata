using System;
using System.Collections.Generic;

namespace Annelitrice.Retheme.NieRAutomata
{
    // All movement is deterministic; the preview exports these same quads.
    public static class CocoonFormationGeometry
    {
        public const int DurationTicks = 90;
        private static float Clamp(float v) { return v < 0 ? 0 : v > 1 ? 1 : v; }
        private static float Smooth(float a, float b, float t)
        { float p=Clamp((t-a)/(b-a));return p*p*(3-2*p); }
        private static float Window(float t,float a,float b,float c,float d)
        { return Smooth(a,b,t)*(1-Smooth(c,d,t)); }
        private static void Rect(List<LogicVirusGeometry.Quad> q,LogicVirusGeometry.Point p,
            float x,float z,float w,float h,float alpha,int layer,bool dark=false,float angle=0)
        {
            if(alpha<.005f || w<=0 || h<=0) return;
            q.Add(new LogicVirusGeometry.Quad {
                x=p.x+x,z=p.z+z,w=w,h=h,angle=angle,
                r=dark?.065f:.91f,g=dark?.070f:.90f,b=dark?.065f:.85f,
                a=Clamp(alpha),layer=layer
            });
        }
        private static void Glow(List<LogicVirusGeometry.Quad> q,LogicVirusGeometry.Point p,
            float x,float z,float w,float h,float alpha,int layer,float angle=0)
        {
            if(alpha<.005f || w<=0 || h<=0) return;
            q.Add(new LogicVirusGeometry.Quad {
                x=p.x+x,z=p.z+z,w=w,h=h,angle=angle,r=1,g=.98f,b=.90f,
                a=Clamp(alpha),layer=layer,glow=true
            });
        }
        private static void Chip(List<LogicVirusGeometry.Quad> q,LogicVirusGeometry.Point p,
            float x,float z,float w,float h,float alpha,int layer,bool dark,float angle=0,bool lit=true)
        {
            Rect(q,p,x,z,w+.016f,h+.016f,alpha*.50f,layer,!dark,angle);
            Rect(q,p,x,z,w,h,alpha,layer+1,dark,angle);
            if(lit && !dark) Glow(q,p,x,z,w+.13f,h+.13f,alpha*.48f,layer+1,angle);
        }
        private static void Collector(List<LogicVirusGeometry.Quad> q,LogicVirusGeometry.Point p,
            float radius,float alpha,float rotation)
        {
            const int count=24;
            for(int i=0;i<count;i++)
            {
                if(i%8>=5) continue;
                float theta=i*(float)(Math.PI*2/count)+rotation;
                float x=(float)Math.Cos(theta)*radius;
                float z=(float)Math.Sin(theta)*radius*.39f-.40f;
                float angle=(float)Math.Atan2(.39f*Math.Cos(theta),-Math.Sin(theta));
                float length=radius*.17f;
                Rect(q,p,x,z,length+.022f,.038f,alpha*.58f,2,true,angle);
                Rect(q,p,x,z,length,.018f,alpha,3,false,angle);
                Glow(q,p,x,z,length+.12f,.11f,alpha*.43f,3,angle);
            }
        }
        private static float RowWidth(int row)
        {
            switch(row) {case 0:return .60f;case 1:return .82f;case 2:return .96f;
                case 3:return 1.04f;case 4:return .98f;default:return .88f;}
        }
        public static void Build(float tick,LogicVirusGeometry.Point origin,List<LogicVirusGeometry.Quad> q)
        {
            q.Clear();
            if(tick<0 || tick>=DurationTicks) return;

            float intake=Window(tick,-2,5,43,65);
            Collector(q,origin,1.02f-.58f*Smooth(0,44,tick),intake*.77f,.12f-tick*.008f);

            // The base mod replaces the pawn immediately. An opaque, stepped data shell
            // bridges the replacement without postponing the actual gameplay event.
            for(int row=0;row<6;row++)
            {
                float z=.43f-row*.168f;
                float width=RowWidth(row);
                float conceal=1-Smooth(2,11,tick);
                float cover=(.23f+.77f*conceal)*(1-Smooth(31+row*3,42+row*4,tick));
                Rect(q,origin,0,z,width,.172f,cover,10,true);
                Rect(q,origin,0,z+.076f,width*.80f,.010f,cover*.26f,11);
                for(int col=0;col<4;col++)
                {
                    float x=(-1.5f+col)*width/4;
                    float block=Window(tick,-2+col,3+col,22+row*2+col,34+row*3+col);
                    if((row+col)%4==0)
                        Rect(q,origin,x,z,width*.13f,.043f,block*.70f,12);
                    else
                        Rect(q,origin,x,z-.032f,width*.10f,.012f,block*.33f,12);
                }
            }
            float ignition=1-Smooth(0,7,tick);
            Glow(q,origin,0,.03f,1.18f,1.24f,ignition*.76f,14);

            // Staggered side plates close like shutters around the data shell.
            for(int row=0;row<3;row++)
            {
                float close=Smooth(row*3,24+row*3,tick);
                float alpha=Window(tick,-2,4,37+row*2,53+row*2);
                for(int side=-1;side<=1;side+=2)
                {
                    float x=side*(.81f-.31f*close);
                    float z=.29f-row*.29f;
                    float angle=side*.16f*(1-close);
                    Chip(q,origin,x,z,.12f,.24f,alpha*.89f,16,(row+(side+1)/2)%3==0,angle);
                    Rect(q,origin,x-side*.085f,z,.15f,.018f,alpha*.76f,18);
                }
            }

            // Strong inward motion: chips spiral towards the shell, with short outer tails.
            for(int i=0;i<36;i++)
            {
                float p=(tick-i%9*1.3f)/(24+i%5*4f);
                if(p<=0 || p>=1) continue;
                float theta=i*2.399963f+.42f*(1-p);
                float start=1.08f+i%4*.10f;
                float end=.22f+i%3*.055f;
                float radius=start+(end-start)*Smooth(0,1,p);
                float x=(float)Math.Cos(theta)*radius;
                float z=(float)Math.Sin(theta)*radius*.77f+.04f;
                float alpha=Smooth(0,.10f,p)*(1-Smooth(.78f,1,p))*.94f;
                float angle=(float)Math.Atan2(.77f*Math.Sin(theta),Math.Cos(theta));
                float size=(.042f+i%4*.014f)*(1-.24f*p);
                if(i%3==0) Rect(q,origin,x+(float)Math.Cos(theta)*.075f,
                    z+(float)Math.Sin(theta)*.058f,.10f,.012f,alpha*.34f,20,false,angle);
                Chip(q,origin,x,z,size,size*(i%3==0?.42f:.76f),alpha,21,i%4==0,
                    i%3==0?angle:0,i%3!=1);
            }

            // One downward assembly scan reveals the existing cocoon a band at a time.
            float scan=Window(tick,28,33,62,70);
            float scanZ=.51f-1.03f*Smooth(31,67,tick);
            Rect(q,origin,0,scanZ,1.05f,.019f,scan*.95f,25);
            Glow(q,origin,0,scanZ,1.28f,.17f,scan*.68f,25);
            Rect(q,origin,-.49f,scanZ+.035f,.09f,.024f,scan*.75f,26,true);
            Rect(q,origin,.49f,scanZ+.035f,.09f,.024f,scan*.75f,26,true);

            // The closing seam has a compact white accent, rather than a second explosion.
            float seal=Window(tick,38,41,43,51);
            Rect(q,origin,0,-.045f,.70f,.026f,seal,28);
            Glow(q,origin,0,-.045f,1.13f,.26f,seal*.92f,28);
            Glow(q,origin,0,.02f,.49f,.61f,seal*.56f,28);
            for(int i=0;i<3;i++)
                Rect(q,origin,(i-1)*.105f,.017f,.055f,.018f,seal*.85f,29);

            // Residual data gets absorbed instead of drifting outward like the activation.
            for(int i=0;i<12;i++)
            {
                float p=(tick-(46+i%4*5))/(19+i%3*3f);
                if(p<=0 || p>=1) continue;
                float theta=i*2.399963f+.8f;
                float radius=.68f-.42f*Smooth(0,1,p);
                float alpha=(float)Math.Sin(Math.PI*p)*(1-p)*.83f;
                Chip(q,origin,(float)Math.Cos(theta)*radius,
                    (float)Math.Sin(theta)*radius*.69f-.03f,
                    .038f*(1-.42f*p),.026f*(1-.42f*p),alpha,30,i%5==0);
            }
        }
    }
}
