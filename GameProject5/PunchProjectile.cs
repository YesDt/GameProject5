using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using GameProject5.Collisions;
using GameProject5;

namespace GameProject5
{
    public enum state
    {
        traveling,
        connected
    }

    public class PunchProjectile
    {
        private static Texture2D _texture;

        public Vector2 Position;

        private BoundingRectangle _bounds;

        private double _animationTimer;

        private short _animationFrame;

        public static SoundEffect _collide;


        public double ProjTimer;

        public float Speed = 300;

        public state projState = state.traveling;

        public bool Flipped;

        public bool Expired = false;

        public bool Collided = true;

       // public Vector2 Position => _position;

        public BoundingRectangle Bounds => _bounds;

        public PunchProjectile(Vector2 pos, mcSprite mc)
        {
            Position = pos;
            if (mc.Flipped) this.Flipped = true;
        }


        public static void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("Sprite_PunchProjectile ");
            _collide = content.Load<SoundEffect>("PunchCollide");


        }

        public void update(GameTime gameTime)
        {
            if (Flipped)
            {
                Position -= new Vector2(Speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                if (projState == state.connected)
                {

                    Speed = 0;
                    Position -= Vector2.Zero;
                    _bounds = new BoundingRectangle(new Vector2(100000000, 100000000), 0, 0);
                }
            }
            else
            {
                Position += new Vector2(Speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                if (projState == state.connected)
                {
                    Speed = 0;
                    Position += Vector2.Zero;
                    _bounds = new BoundingRectangle(new Vector2(100000000, 100000000), 0, 0);
                }
            }

            

            _bounds = new BoundingRectangle(new Vector2(Position.X, Position.Y), 48, 56);
            ProjTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (ProjTimer >= 1)
            {
                Destroy(this);
            }
            if (projState == state.connected && Collided)
            {
                _collide.Play();
                Collided = false;
            }

        }

        public void Destroy(PunchProjectile p)
        {

            p._bounds = new BoundingRectangle(new Vector2(100000000, 100000000), 0, 0);
            Expired = true;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteEffects spriteEffects = (Flipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (projState == state.traveling)
            {
                if (ProjTimer < 0.6)
                {
                    _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (_animationTimer > 0.1)
                    {
                        _animationFrame++;
                        if (_animationFrame > 1) _animationFrame = 0;
                        _animationTimer -= 0.1;
                    }

                }
                else
                {
                    _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (_animationTimer > 0.2)
                    {
                        _animationFrame++;
                        if (_animationFrame > 3) _animationFrame = 3;
                        _animationTimer -= 0.2;
                    }
                }

            }
            if(projState == state.connected)
            {
                
                _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (_animationTimer > 0.1)
                {
                    _animationFrame++;
                   
                    _animationTimer -= 0.1;
                }
            }
            var source = new Rectangle(_animationFrame * 250, (int)projState * 506, 268, 440);
            if (projState == state.connected) spriteBatch.Draw(_texture, Position, source, Color.White, 0f, new Vector2(80, 80), 0.5f, spriteEffects, 0);
            else spriteBatch.Draw(_texture, Position, source, Color.White, 0f, new Vector2(80, 120), 0.5f, spriteEffects, 0);
        }
    }
}