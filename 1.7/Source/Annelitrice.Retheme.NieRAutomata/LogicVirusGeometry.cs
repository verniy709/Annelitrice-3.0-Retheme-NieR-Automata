using System;
using System.Collections.Generic;

namespace Annelitrice.Retheme.NieRAutomata
{
    // Pure geometry: both the in-game renderer and the offline preview use this file.
    public static class LogicVirusGeometry
    {
        public const int DurationTicks = 80;
        public struct Point
        {
            public float x, z;
            public Point(float x, float z) { this.x = x; this.z = z; }
        }
        public struct Quad
        {
            public float x, z, w, h, angle, r, g, b, a;
            public int layer;
            public bool glow;
        }
        private struct Ink
        {
            internal float r, g, b;
            internal Ink(float r, float g, float b) { this.r = r; this.g = g; this.b = b; }
        }
        private static readonly Ink Ivory = new Ink(0.91f, 0.90f, 0.85f);
        private static readonly Ink Black = new Ink(0.07f, 0.075f, 0.07f);
        private static readonly Ink Red = new Ink(0.67f, 0.12f, 0.16f);
        private static float Clamp(float v) { return v < 0 ? 0 : v > 1 ? 1 : v; }
        private static float Smooth(float a, float b, float t)
        {
            float v = Clamp((t - a) / (b - a));
            return v * v * (3 - 2 * v);
        }
        private static float Window(float t, float a, float b, float c, float d)
        { return Smooth(a, b, t) * (1 - Smooth(c, d, t)); }
        private static void Rect(List<Quad> q, Point p, float x, float z,
            float w, float h, Ink ink, float alpha, int layer, float angle = 0)
        {
            if (alpha < 0.005f || w <= 0 || h <= 0) return;
            q.Add(new Quad { x = p.x + x, z = p.z + z, w = w, h = h,
                r = ink.r, g = ink.g, b = ink.b, a = Clamp(alpha), angle = angle, layer = layer });
        }
        private static void Chip(List<Quad> q, Point p, float x, float z, float w,
            float h, float angle, float alpha, bool dark, int layer)
        {
            // A restrained contrasting border keeps a small chip readable on both floors.
            Rect(q, p, x, z, w + 0.018f, h + 0.018f, dark ? Ivory : Black,
                alpha * 0.55f, layer, angle);
            Rect(q, p, x, z, w, h, dark ? Black : Ivory, alpha, layer + 1, angle);
        }
        private static void Corner(List<Quad> q, Point p, float x, float z,
            int sx, int sz, float length, float alpha, int layer)
        {
            Rect(q, p, x + sx * length / 2, z, length + 0.026f, 0.051f,
                Black, alpha * 0.7f, layer);
            Rect(q, p, x, z + sz * length / 2, 0.051f, length + 0.026f,
                Black, alpha * 0.7f, layer);
            Rect(q, p, x + sx * length / 2, z, length, 0.022f,
                Ivory, alpha, layer + 1);
            Rect(q, p, x, z + sz * length / 2, 0.022f, length,
                Ivory, alpha, layer + 1);
        }

        private static void Bloom(List<Quad> q, Point p, float x, float z,
            float w, float h, float alpha, int layer, float angle = 0,
            float r = 1, float g = 0.98f, float b = 0.90f)
        {
            if (alpha < 0.005f) return;
            q.Add(new Quad { x = p.x + x, z = p.z + z, w = w, h = h,
                r = r, g = g, b = b, a = Clamp(alpha), angle = angle,
                layer = layer, glow = true });
        }

        private static void AddGlow(float tick, Point source, Point target, List<Quad> q)
        {
            int normalCount = q.Count;
            for (int i = 0; i < normalCount; i++)
            {
                Quad core = q[i];
                if (core.a < 0.12f) continue;
                // Glow belongs to the white cores, not the contrasting outlines or black chips.
                bool whiteCore = core.r > 0.8f && (core.layer == 11 || core.layer == 17 ||
                    core.layer == 21 || core.layer == 24 || core.layer == 31 ||
                    core.layer == 33 || core.layer == 35);
                if (whiteCore)
                {
                    float strength = core.layer == 21 ? 0.26f : 0.44f;
                    float padding = core.layer == 24 ? 0.15f : 0.13f;
                    Bloom(q, new Point(core.x, core.z), 0, 0,
                        core.w + padding, core.h + padding,
                        core.a * strength, core.layer, core.angle);
                }
                else if (core.layer == 30 && core.r > 0.5f && core.g < 0.3f)
                {
                    Bloom(q, new Point(core.x, core.z), 0, 0,
                        core.w + 0.11f, core.h + 0.10f,
                        core.a * 0.24f, core.layer, core.angle, 0.94f, 0.13f, 0.18f);
                }
            }
            float write = Window(tick, 30, 34, 38, 46);
            Bloom(q, target, 0, 0.03f, 0.85f, 1.00f, write * 0.14f, 36);
            Bloom(q, target, 0, 0.03f, 0.88f, 0.12f, write * 0.30f, 37);
            float sourceLight = Window(tick, 1, 6, 16, 25);
            Bloom(q, source, 0, 0, 0.26f, 0.26f, sourceLight * 0.16f, 12);
        }
        public static void Build(float tick, Point source, Point target, List<Quad> q)
        {
            q.Clear();
            if (tick < 0 || tick >= DurationTicks) return;
            float dx = target.x - source.x, dz = target.z - source.z;
            float distance = (float)Math.Sqrt(dx * dx + dz * dz);
            float ux = distance > 0.05f ? dx / distance : 1;
            float uz = distance > 0.05f ? dz / distance : 0;
            float nx = -uz, nz = ux;
            float angle = (float)Math.Atan2(dz, dx);
            float transfer = Window(tick, 3, 9, 30, 41);

            // Short, fine dashed guide; a close-contact action, not a ranged laser.
            if (distance > 0.42f && transfer > 0)
            {
                int count = Math.Min(12, (int)(distance / 0.14f));
                for (int i = 1; i < count; i++)
                {
                    float t = (float)i / count;
                    Rect(q, source, dx * t, dz * t, 0.055f, 0.012f,
                        Ivory, transfer * 0.24f, 0, angle);
                }
            }

            // Fine satellite particles make the existing packet stream more visible.
            // Their slightly wider lanes converge at the receiving end.
            for (int i = 0; i < 24; i++)
            {
                float p = (tick - (3 + i * 0.95f)) / (15 + i % 4 * 2);
                if (p <= 0 || p >= 1 || distance < 0.20f) continue;
                float side = i % 2 == 0 ? 1 : -1;
                float lane = side * (0.085f + i % 4 * 0.024f);
                float drift = lane * (float)Math.Sin(Math.PI * p);
                float t = 0.10f + p * 0.80f;
                float x = dx * t + nx * drift, z = dz * t + nz * drift;
                float alpha = (float)Math.Sin(Math.PI * p) * 0.68f;
                float size = 0.024f + i % 3 * 0.009f;
                Rect(q, source, x - ux * 0.043f, z - uz * 0.043f,
                    size * 1.7f, 0.012f, Ivory, alpha * 0.28f, 6, angle);
                Rect(q, source, x, z, size, size * (i % 3 == 0 ? 1 : 0.65f),
                    i % 5 == 0 ? Black : Ivory, alpha, 7, angle);
            }

            // Twelve staggered main packets retain the original rhythm and hierarchy.
            for (int i = 0; i < 12; i++)
            {
                float p = (tick - (5 + i * 1.9f)) / (16 + i % 3 * 2);
                if (p <= 0 || p >= 1 || distance < 0.20f) continue;
                float lane = (i % 3 - 1) * 0.065f;
                float drift = lane * (float)Math.Sin(Math.PI * p);
                float t = 0.16f + 0.68f * p;
                float alpha = (float)Math.Sin(Math.PI * p) * 0.94f;
                Chip(q, source, dx * t + nx * drift, dz * t + nz * drift,
                    i % 4 == 0 ? 0.125f : 0.075f, 0.031f + i % 2 * 0.008f,
                    angle, alpha, i % 3 == 0, 10);
            }

            // Source handshake opens briefly; target braces close in to show acquisition.
            float handshake = Window(tick, 0, 5, 18, 30);
            for (int side = -1; side <= 1; side += 2)
            {
                Rect(q, source, nx * side * 0.12f, nz * side * 0.12f,
                    0.075f, 0.024f, Ivory, handshake * 0.7f, 12, angle);
            }

            // Peripheral data specks gather toward the frame before the write pulse.
            for (int i = 0; i < 12; i++)
            {
                float p = (tick - (6 + i * 0.9f)) / (18 + i % 3 * 2);
                if (p <= 0 || p >= 1) continue;
                float theta = i * 2.399963f + p * 0.22f;
                float radius = 0.66f - 0.35f * Smooth(0, 1, p);
                float alpha = (float)Math.Sin(Math.PI * p) * 0.72f;
                Chip(q, target, (float)Math.Cos(theta) * radius,
                    (float)Math.Sin(theta) * radius * 0.86f,
                    0.032f + i % 3 * 0.009f, 0.024f, theta * 0.18f,
                    alpha, i % 4 == 0, 16);
            }
            float lockAlpha = Window(tick, 2, 12, 48, 65);
            float spread = 0.10f * (1 - Smooth(4, 28, tick));
            float rx = 0.30f + spread, rz = 0.36f + spread;
            for (int sx = -1; sx <= 1; sx += 2)
                for (int sz = -1; sz <= 1; sz += 2)
                    Corner(q, target, sx * rx, sz * rz, -sx, -sz,
                        0.105f, lockAlpha * 0.83f, 20);

            // A narrow scan moves across the recipient once. The body stays visible.
            float scan = Window(tick, 21, 26, 40, 45);
            float scanX = -0.25f + 0.50f * Smooth(22, 42, tick);
            Rect(q, target, scanX, 0.015f, 0.055f, 0.66f, Ivory, scan * 0.09f, 23);
            Rect(q, target, scanX, 0.015f, 0.014f, 0.66f, Ivory, scan * 0.63f, 24);
            for (int i = 0; i < 5; i++)
                Rect(q, target, scanX + (i % 2 == 0 ? 0.035f : -0.045f),
                    -0.24f + i * 0.12f, 0.085f, 0.017f, Black, scan * 0.62f, 25);

            // One red fault event, two offset phases; no screen-wide flicker or red wash.
            float fault = Window(tick, 29, 33, 43, 50);
            float offset = tick < 37 ? -0.025f : 0.035f;
            Rect(q, target, 0.22f + offset, 0.24f, 0.11f, 0.031f, Red, fault, 30);
            Rect(q, target, 0.28f + offset, 0.18f, 0.028f, 0.13f, Red, fault, 30);
            Rect(q, target, -0.20f - offset, -0.22f, 0.075f, 0.025f, Red, fault * 0.85f, 30);
            Rect(q, target, -0.12f - offset, -0.22f, 0.027f, 0.025f, Ivory, fault * 0.8f, 30);

            // A broken, very short pulse expands outside the lock frame at the write.
            float pulse = (tick - 31) / 16f;
            if (pulse > 0 && pulse < 1)
            {
                float radius = 0.36f + 0.27f * pulse;
                float alpha = (float)Math.Sin(Math.PI * pulse) * 0.58f;
                for (int i = 0; i < 8; i++)
                {
                    if (i == 2 || i == 5) continue;
                    float theta = i * (float)Math.PI / 4 + 0.16f;
                    Rect(q, target, (float)Math.Cos(theta) * radius,
                        (float)Math.Sin(theta) * radius * 0.88f,
                        0.065f * (1 - pulse * 0.35f), 0.017f,
                        Ivory, alpha, 31, theta + (float)Math.PI / 2);
                }
            }

            // A second, larger burst gives the successful write a visible particle peak.
            for (int i = 0; i < 18; i++)
            {
                float p = (tick - (31 + i % 4 * 1.5f)) / (25 + i % 3 * 5);
                if (p <= 0 || p >= 1) continue;
                float theta = i * 2.399963f + 0.35f;
                float outward = 1 - (1 - p) * (1 - p);
                float radius = 0.26f + (0.35f + i % 3 * 0.07f) * outward;
                float alpha = Smooth(0, 0.15f, p) * (1 - Smooth(0.32f, 1, p)) * 0.79f;
                float w = (0.030f + i % 4 * 0.011f) * (1 - p * 0.35f);
                float h = i % 4 == 0 ? w * 0.35f : w * 0.75f;
                Chip(q, target, (float)Math.Cos(theta) * radius,
                    (float)Math.Sin(theta) * radius * 0.88f + 0.06f * p,
                    w, h, i % 4 == 0 ? theta : 0,
                    alpha, i % 4 == 0, 32);
            }

            // Original slow fragments remain as a softer second layer of afterglow.
            for (int i = 0; i < 9; i++)
            {
                float p = (tick - (34 + i % 3 * 3)) / (29 + i % 2 * 5);
                if (p <= 0 || p >= 1) continue;
                float theta = i * 2.399963f;
                float radius = 0.20f + 0.31f * p;
                float size = (0.035f + i % 3 * 0.01f) * (1 - 0.45f * p);
                float alpha = (float)Math.Sin(Math.PI * p) * (1 - p) * 0.9f;
                Chip(q, target, (float)Math.Cos(theta) * radius,
                    (float)Math.Sin(theta) * radius + p * 0.07f,
                    size, size * (i % 2 == 0 ? 0.65f : 1),
                    0, alpha, i % 3 == 0, 34);
            }
            AddGlow(tick, source, target, q);
        }
    }
}
