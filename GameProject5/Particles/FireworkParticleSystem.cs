using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProject5.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using SharpDX.Direct3D11;

namespace GameProject5.Particles
{
    public class FireworkParticleSystem : ParticleSystem
    {
        Color[] colors = new Color[] {
            Color.Fuchsia,
            Color.Red,
            Color.Crimson,
            Color.CadetBlue,
            Color.Aqua,
            Color.HotPink,
            Color.LimeGreen
        };

        Color color;

        public FireworkParticleSystem(Game game, int maxExplosions) : base(game, maxExplosions * 25)
        {

        }

        protected override void InitializeConstants()
        {
            textureFilename = "circle";

            minNumParticles = 2;
            maxNumParticles = 6;

            blendState = BlendState.Additive;
            DrawOrder = AdditiveBlendDrawOrder;
        }

        protected override void InitializeParticle(ref Particle p, Vector2 where)
        {

            var velocity = RandomHelper.NextDirection() * RandomHelper.NextFloat(40, 200);

            var lifetime = RandomHelper.NextFloat(0.5f, 1.0f);

            var acceleration = -velocity / lifetime;

            var rotation = RandomHelper.NextFloat(0, MathHelper.TwoPi);

            var angularVelocity = RandomHelper.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);

            var scale = RandomHelper.NextFloat(4, 6);

            p.Initialize(where, velocity, acceleration, color, lifetime: lifetime, rotation: rotation, angularVelocity: angularVelocity, scale: scale);
        }


        protected override void UpdateParticle(ref Particle particle, float dt)
        {
            base.UpdateParticle(ref particle, dt);

            float normalizedLifetime = particle.TimeSinceStart / particle.Lifetime;

            particle.Scale = .5f + .25f * normalizedLifetime;
        }

        public void placeFirework(Vector2 where)
        {
            color = colors[RandomHelper.Next(colors.Length)];
            AddParticles(where);
        }
    }
}