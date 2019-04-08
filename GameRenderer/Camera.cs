using System;
using GameModel;
using GlmNet;
using OpenTK.Input;

namespace GameRenderer
{
    public class Camera
    {
        public vec3 Target { get; set; }
        public float Radius { get; set; } = 10.0f;
        public float Theta { get; set; }
        public float Alpha { get; set; }
        public vec3 Position
        {
            get
            {
                vec3 v = new vec3(
                    (float) Math.Sin(Theta) * Radius,
                    (float) Math.Sin(Alpha) * Radius,
                    (float) Math.Cos(Theta) * Radius);

                return Target + v; 
                
            }
        }

        public vec3 Up { get; set; } = new vec3(0.0f, 1.0f, 0.0f);
        public vec3 Forward => Target - Position;
        
        public vec3 Right => glm.normalize(glm.cross(Up, Forward));
        public float Width { get; set; }
        public float Height { get; set; }

        public float Speed { get; set; } = 5.5f;
        
        public float RotationSpeed { get; set; } = 0.005f;

        public mat4 ProjectionMatrix => glm.perspective((float) Math.PI / 2, Width / Height, 0.1f, 1000.0f);
        public mat4 ViewMatrix => glm.lookAt(Position, Target, Up);

        private bool rotating;
        
        public void Move(KeyboardState key, float deltaTime)
        {
            vec3 velocity = new vec3(0);

            if (key[Key.W])
            {
                velocity += Forward;
            }
            
            if (key[Key.S])
            {
                velocity -= Forward;
            }
            
            if (key[Key.A])
            {
                velocity += Right;
            }
            
            if (key[Key.D])
            {
                velocity -= Right;
            }

            velocity.y = 0.0f;
            
            if (key[Key.Q])
            {
                Radius -= Speed * deltaTime;
            } else if (key[Key.E])
            {
                Radius += Speed * deltaTime;
            }

            if (velocity.x != 0.0f || velocity.z != 0.0f)
            {
                velocity = glm.normalize(velocity);
            }

            Target +=  velocity * deltaTime * Speed;
        }
        
        public void OnMouseMove(MouseMoveEventArgs e)
        {
            if (rotating)
            {
                Alpha += e.YDelta * RotationSpeed;
                Theta += e.XDelta * RotationSpeed;

                float hpi = (float) Math.PI / 2.0f;
                float pi = (float) Math.PI;
            }
        }

        public void OnMouseDown(MouseButtonEventArgs e)
        {
            rotating = true;
        }

        public void OnMouseUp(MouseButtonEventArgs e)
        {
            rotating = false;
        }

        public (vec3 start, vec3 dir) CastRay(float x1, float y1)
        {
            float x = 2.0f * x1 / Width - 1.0f;
            float y = 1.0f - (2.0f * y1) / Height;
            float z = 0.0f;

            var rayNds = new vec3(x, y, z);
            var rayClip = new vec4(rayNds.x, rayNds.y, -1.0f, 1.0f);
            var rayEye =  glm.inverse(ProjectionMatrix) * rayClip;
            rayEye = new vec4(rayEye.x, rayEye.y, -1.0f, 0.0f);
            var rayWor = glm.inverse(ViewMatrix) * rayEye;

            var origin = y * Up + x * Right;
            var start = new vec3( Position.x + origin.x, Position.y + origin.y, Position.z + origin.z);

            return (start, new vec3(rayWor));
        }
    }
}