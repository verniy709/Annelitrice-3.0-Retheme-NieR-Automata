using System;
using System.Collections.Generic;

namespace Annelitrice.Retheme.NieRAutomata
{
    public static class ReconstructionCompleteGeometry
    {
        public const int DurationTicks=108;
        private static float Clamp(float v) {return v<0?0:v>1?1:v;}
        private static float Smooth(float a,float b,float t)
        {float p=Clamp((t-a)/(b-a));return p*p*(3-2*p);}
        private static float Window(float t,float a,float b,float c,float d)
        {return Smooth(a,b,t)*(1-Smooth(c,d,t));}
        private static void Rect(List<LogicVirusGeometry.Quad> q,LogicVirusGeometry.Point p,
            float x,float z,float w,float h,float alpha,int layer,bool dark=false,float angle=0)
        {
            if(alpha<.005f || w<=0 || h<=0) return;
            q.Add(new LogicVirusGeometry.Quad {x=p.x+x,z=p.z+z,w=w,h=h,angle=angle,
                r=dark?.065f:.91f,g=dark?.070f:.90f,b=dark?.065f:.85f,a=Clamp(alpha),layer=layer});
        }
        private static void Glow(List<LogicVirusGeometry.Quad> q,LogicVirusGeometry.Point p,
            float x,float z,float w,float h,float alpha,int layer,float angle=0)
        {
            if(alpha<.005f || w<=0 || h<=0) return;
            q.Add(new LogicVirusGeometry.Quad {x=p.x+x,z=p.z+z,w=w,h=h,angle=angle,
                r=1,g=.98f,b=.90f,a=Clamp(alpha),layer=layer,glow=true});
        }
        private static void Chip(List<LogicVirusGeometry.Quad> q,LogicVirusGeometry.Point p,
            float x,float z,float w,float h,float alpha,int layer,bool dark,float angle=0,bool lit=true)
        {
            Rect(q,p,x,z,w+.016f,h+.016f,alpha*.54f,layer,!dark,angle);
            Rect(q,p,x,z,w,h,alpha,layer+1,dark,angle);
            if(lit&&!dark) Glow(q,p,x,z,w+.13f,h+.13f,alpha*.50f,layer+1,angle);
        }
        private static void Ripple(List<LogicVirusGeometry.Quad> q,LogicVirusGeometry.Point p,
            float radius,float alpha,int count,float rotation,int layer,float thickness)
        {
            if(alpha<.005f) return;
            for(int i=0;i<count;i++)
            {
                if(i%6==4 || i%6==5) continue;
                float theta=i*(float)(Math.PI*2/count)+rotation;
                float x=(float)Math.Cos(theta)*radius;
                float z=(float)Math.Sin(theta)*radius*.40f-.43f;
                float angle=(float)Math.Atan2(.40f*Math.Cos(theta),-Math.Sin(theta));
                float length=radius*(float)(Math.PI*2/count)*.68f;
                Rect(q,p,x,z,length+.022f,thickness+.022f,alpha*.55f,layer,true,angle);
                Rect(q,p,x,z,length,thickness,alpha,layer+1,false,angle);
                Glow(q,p,x,z,length+.13f,thickness+.12f,alpha*.55f,layer+1,angle);
            }
        }
        private static void Corner(List<LogicVirusGeometry.Quad> q,LogicVirusGeometry.Point p,
            int sx,int sz,float x,float z,float alpha)
        {
            Rect(q,p,sx*x,sz*z,.018f,.13f,alpha,29);
            Rect(q,p,sx*(x-.055f),sz*(z+.055f),.11f,.018f,alpha,29);
            Glow(q,p,sx*x,sz*z,.14f,.24f,alpha*.34f,29);
        }
        public static void Build(float tick,LogicVirusGeometry.Point origin,
            LogicVirusGeometry.Point adult,List<LogicVirusGeometry.Quad> q)
        {
            q.Clear();
            if(tick<0 || tick>=DurationTicks) return;

            // The ground marks the old cocoon site; it stays fixed if the adult walks.
            float first=Window(tick,1,6,27,49);
            Ripple(q,origin,.20f+.87f*Smooth(2,31,tick),first*.95f,24,.12f,2,.026f);
            float second=Window(tick,10,17,39,66);
            Ripple(q,origin,.34f+.99f*Smooth(10,48,tick),second*.55f,18,-.14f,5,.017f);

            // Short-lived black/ivory casing splits sideways, bridging the instant release.
            float split=Smooth(2,19,tick);
            float casing=1-Smooth(4,23,tick);
            for(int row=0;row<6;row++)
            {
                float width=row==0?.29f:row==5?.31f:.36f;
                float z=.50f-row*.21f;
                for(int side=-1;side<=1;side+=2)
                {
                    float x=side*(width*.5f+(.33f+row*.018f)*split);
                    float rise=(row<3?1:-1)*.07f*split;
                    float angle=side*(row-2.5f)*.035f*split;
                    Rect(q,adult,x,z+rise,width,.215f,casing,20,true,angle);
                    Rect(q,adult,x-side*width*.40f,z+rise,.018f,.18f,casing*.86f,21,false,angle);
                    if(row%2==0) Rect(q,adult,x,z+rise+.035f,width*.46f,.022f,casing*.70f,21,false,angle);
                }
            }

            // One compact local burst, with a narrow seam and a soft body-shaped bloom.
            float initial=1-Smooth(0,9,tick);
            float unlock=Window(tick,2,6,9,19);
            Glow(q,adult,0,.06f,1.42f,1.65f,initial*.76f+unlock*.42f,24);
            Glow(q,adult,0,.055f,.42f,1.59f,unlock*.90f,24);
            Rect(q,adult,0,.055f,.023f,1.08f,unlock*.90f,25);
            Glow(q,adult,0,.055f,1.65f,.21f,unlock*.84f,25);
            Rect(q,adult,0,.055f,.84f,.019f,unlock*.84f,25);

            // Two luminous release edges sweep apart; the adult is revealed between them.
            float edge=Window(tick,3,7,22,32);
            float edgeX=.03f+.43f*Smooth(4,24,tick);
            for(int side=-1;side<=1;side+=2)
            {
                Rect(q,adult,side*edgeX,.04f,.015f,1.10f,edge*.80f,27);
                Glow(q,adult,side*edgeX,.04f,.18f,1.27f,edge*.59f,27);
            }
            float corners=Window(tick,6,11,26,42);
            float cornerX=.34f+.14f*Smooth(8,32,tick);
            for(int sx=-1;sx<=1;sx+=2)
                for(int sz=-1;sz<=1;sz+=2) Corner(q,adult,sx,sz,cornerX,.50f,corners*.63f);

            // Larger rectangular shell fragments break away at the original cocoon site.
            for(int i=0;i<12;i++)
            {
                float p=(tick-(3+i%4*1.5f))/(24+i%3*5f);
                if(p<=0 || p>=1) continue;
                float side=i%2==0?-1:1;
                float x=side*(.28f+.53f*(1-(1-p)*(1-p)));
                float z=.39f-(i/2)*.155f+.18f*p;
                float alpha=Smooth(0,.10f,p)*(1-Smooth(.25f,1,p))*.83f;
                Chip(q,origin,x,z,.11f*(1-.25f*p),.17f*(1-.35f*p),alpha,10,
                    i%3!=0,side*(.08f+.55f*p),i%3==0);
            }

            // A denser, outward burst distinguishes completion from the inward formation.
            for(int i=0;i<52;i++)
            {
                float p=(tick-(3+i%8*1.05f))/(24+i%5*4f);
                if(p<=0 || p>=1) continue;
                float theta=i*2.399963f+.17f;
                float radius=.13f+(.77f+i%4*.14f)*(1-(1-p)*(1-p));
                float x=(float)Math.Cos(theta)*radius;
                float z=(float)Math.Sin(theta)*radius*.82f+.17f*p;
                float angle=(float)Math.Atan2(.82f*Math.Sin(theta),Math.Cos(theta));
                float alpha=Smooth(0,.08f,p)*(1-Smooth(.30f,1,p))*.96f;
                float size=(.041f+i%4*.014f)*(1-.27f*p);
                if(i%4==0) Rect(q,origin,x-(float)Math.Cos(theta)*.075f,
                    z-(float)Math.Sin(theta)*.061f,.115f,.013f,alpha*.38f,14,false,angle);
                Chip(q,origin,x,z,size,size*(i%4==0?.38f:.77f),alpha,15,i%5==0,
                    i%4==0?angle:0,i%3!=1);
            }

            // Quiet rising fragments and a restrained confirmation bloom complete the release.
            float settled=Window(tick,24,31,37,51);
            Glow(q,adult,0,.10f,.66f,1.10f,settled*.16f,31);
            for(int i=0;i<18;i++)
            {
                float p=(tick-(36+i%6*4))/(30+i%3*6f);
                if(p<=0 || p>=1) continue;
                float theta=i*2.399963f+.8f;
                float radius=.39f+.33f*p;
                float alpha=(float)Math.Sin(Math.PI*p)*(1-p)*.73f;
                Chip(q,adult,(float)Math.Cos(theta)*radius,
                    (float)Math.Sin(theta)*radius*.72f+.33f*p,
                    .037f*(1-.45f*p),.029f*(1-.45f*p),alpha,33,i%5==0,0,i%3!=1);
            }
        }
    }
}
