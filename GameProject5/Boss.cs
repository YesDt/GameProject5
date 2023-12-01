using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProject5.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace GameProject5
{
    public enum BossAction
    {
        Idle = 0,
        ShoulderCharge = 1,
        FingerFlick = 2,
        
    }
    public class Boss
    {
        #region privateFields
        private Texture2D _texture;

        private BoundingRectangle _bounds;

        

        private double _animationTimer;

        public double _attackingTimer;

        private short _animationFrame;

        private double _passiveTimer = 0;

        private Vector2 _direction;

        private bool _hasShot = false;

        private Random random = new Random();



        #endregion

        #region publicFields
        public Vector2 Position = new Vector2();

        public double Attackingtimer = 0;

        public bool Flipped = false;

        public BossAction Action = BossAction.Idle;

        public float BoundaryOne;

        public float BoundaryTwo;

        public float Health = 2000;

        public bool Attacking = false;

        public List<bullet> BulletList = new List<bullet>();

        public short AnimationFrame => _animationFrame;


        public BoundingRectangle Bounds => _bounds;

   


        #endregion

        #region publicMethods

        public Boss(Vector2 pos)
        {
            Position = pos;
            _bounds = new BoundingRectangle(new Vector2(Position.X - 32, Position.Y - 16), 48, 130);

        }

        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("Sprite_boss");
            bullet.LoadContent(content);
        }

        public void Update(GameTime gameTime)
        {
            if (Action == BossAction.Idle)
            {
                _passiveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_passiveTimer > 2)
                {
                    int randomNum = random.Next(1, 3);
                    if (randomNum == 1)
                    {
                        Action = BossAction.ShoulderCharge;
                        randomNum = 0;
                    }
                    else
                    {
                        Action = BossAction.FingerFlick;
                        randomNum = 0;
                    }
                    _passiveTimer = 0;
                }
            }
            if (Action == BossAction.ShoulderCharge)
            {
                _attackingTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_attackingTimer > 3)
                {
                    if (Flipped) Position -= new Vector2(300 * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                    
                }
                else
                {
                    Position += new Vector2(300 * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                }
            }
        }

        public void addFF()
        {
            
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteEffects spriteEffects = (Flipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            
            

            var source = new Rectangle(_animationFrame * 250, (int)Action * 512, 268, 512);
            //if (Attacking && _attackingTimer >= 2.5 && _attackingTimer <= 3) spriteBatch.Draw(_texture, _position, source, Color.Red, 0f, new Vector2(80, 120), 0.5f, spriteEffects, 0);
            //else 
            spriteBatch.Draw(_texture, Position, source, Color.White, 0f, new Vector2(80, 120), 0.5f, spriteEffects, 0);

            foreach (var proj in BulletList)
            {
                proj.Draw(gameTime, spriteBatch);

            }
            #endregion
        }
    }
}
