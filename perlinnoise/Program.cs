using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Raylib_cs;
namespace attempte_of_perlin_noise
{
    internal class Program
    {
        public static int globalCOunter = 0;    
        public static Random rnd = new Random();
        public static int seed = rnd.Next(10, 100);
        struct point
        {
            public int x;
            public int y;
            public point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
        struct vector2
        {
            public double x;
            public double y;
            public vector2(double x, double y)
            {
                this.x = x;
                this.y = y;
            }
        }

        static vector2 GetVector(int x, int y, int seed, double time) // return a random unit vector based on a global seed calculating  a local seed
        {
            globalCOunter++;
            int localseed = seed ^ (x * 73856093) ^ (y * 19349663);
            Random rn = new Random(localseed);
            
            double angle1 = rn.NextDouble() * 2 * Math.PI + 2*time;
            double angle = 0;
            if(globalCOunter % 2 ==0)
            {
               angle = angle1*2;
            }
            else angle = angle1;
            return new vector2(Math.Cos(angle1), Math.Sin(angle1));

        }
        static double Interpolate(double a, double b, double w)
        {
            return a + w * (b - a);
        }
        static double DotProduct(vector2 a, vector2 b)
        {
            return a.x * b.x + a.y * b.y;
        }
        static double Fade(double t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }
        static double PerlinNoise(double x, double y, double time)
        {
            double u = Fade(x - Math.Floor(x));
            double v = Fade(y - Math.Floor(y));
            point sb_left = new point((int)Math.Floor(x), (int)Math.Floor(y)); // points of the square
            point sb_right = new point(sb_left.x + 1, sb_left.y);
            point st_left = new point(sb_left.x, sb_left.y + 1);
            point st_right = new point(sb_left.x + 1, sb_left.y + 1);
            vector2 g0 = GetVector(sb_left.x, sb_left.y, seed,time);               // gradinet vectors at each angle
            vector2 g1 = GetVector(sb_right.x, sb_right.y, seed,time);
            vector2 g2 = GetVector(st_left.x, st_left.y, seed,time);
            vector2 g3 = GetVector(st_right.x, st_right.y, seed,time);
            vector2 v0 = new vector2((x - sb_left.x), (y - sb_left.y));         // vectors from the angles to the point
            vector2 v1 = new vector2((x - sb_right.x), (y - sb_right.y));
            vector2 v2 = new vector2((x - st_left.x), (y - st_left.y));
            vector2 v3 = new vector2((x - st_right.x), (y - st_right.y));

            return ((Interpolate(Interpolate(DotProduct(g0, v0), DotProduct(g1, v1), u), Interpolate(DotProduct(g2, v2), DotProduct(g3, v3), u), v)) + 1) / 2; // interpolation  and calculation of the dot products of all 4 pairs resulting single noise value
        }
        public static double FractalNoise(double x, double y, int octaves, double persistence, double time) // Creates fractal noise
        {
        double total = 0;
        double frequency = 1;
        double amplitude = 1;
        double maxValue = 0;  

        for (int i = 0; i < octaves; i++)
        {
       
        total += PerlinNoise(x * frequency, y * frequency, time) * amplitude;

        maxValue += amplitude; 

        amplitude *= persistence; 
        frequency *= 2;  
        }

        return total / maxValue; 
        }
        static void Main(string[] args)
        {
            const int screenWidth = 800;
            const int screenHeight = 600;
            const int scale = 4;
            const int width = screenWidth / scale;
            const int height = screenHeight / scale;
            const int radius = 70;

            Raylib.InitWindow(screenWidth, screenHeight, "perlin noise");
            
            
            Color[] pixels = new Color[width * height];

           
            Image canvas = Raylib.GenImageColor(width, height, Color.Black);
            Texture2D screenTexture = Raylib.LoadTextureFromImage(canvas);
            Raylib.UnloadImage(canvas); 
            double counter = 0;
            double zoom = 0;
            bool animation_off = false;
            while (!Raylib.WindowShouldClose())
            {
                if(Raylib.IsKeyDown(KeyboardKey.F) && animation_off == false) animation_off = true; 
                if(Raylib.IsKeyDown(KeyboardKey.T) && animation_off == true) animation_off = false;
                zoom += Raylib.GetMouseWheelMove();
                counter += 0.05;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        double noise;
                        double noiseValue;
                        if(animation_off)
                        {
                             noiseValue = FractalNoise(x / (50.0+zoom), y / (50.0+zoom), 4, 0.5, 0);
                             noise = PerlinNoise(x / (50.0+zoom) , y / (50.0+zoom) , 0);
                        }
                        else
                        {
                             noiseValue = FractalNoise(x / (50.0+zoom), y / (50.0+zoom), 4, 0.5, counter);
                             noise = PerlinNoise(x / (50.0+zoom) , y / (50.0+zoom) , counter);
                        }

                        
                      
                        int colorV = (int)(noise * 255);
                        pixels[y * width + x] = new Color(colorV, colorV, colorV, 255);
                    }
                }
                
                Raylib.UpdateTexture(screenTexture, pixels);

                
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

               
                Raylib.DrawTextureEx(screenTexture, new System.Numerics.Vector2(0, 0), 0, scale, Color.White);

                Raylib.DrawText($"{counter:F2}", 10, 10, 20, Color.Red);
                Raylib.DrawFPS(10, 35);

                Raylib.EndDrawing();
            }

           
            Raylib.CloseWindow();
            Console.ReadKey();
            while (true)
            {

                
                seed = rnd.Next();
                for (int i = 0; i < Console.WindowHeight; i++)
                {
                    for (int j = 0; j < Console.WindowWidth; j++)
                    {
                        double noise = PerlinNoise(i / 5.0, j / 9.0,0);
                        if (noise > 0.75)
                        {
                            Console.Write('█');
                        }
                        else if (noise > 0.5)
                        {
                            Console.Write('▓');
                        }
                        else if (noise > 0.3)
                        {
                            Console.Write('▒');
                        }
                        else
                        {
                            Console.Write('░');
                        }
                       
                    }

                }
                Console.ReadKey();
            }
        }
    }
}
