namespace IDontKnowWhatImDoingPort
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using OpenTK.Graphics;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Input;

    class MyApplication
    {
        private static GLControl GlControl = new GLControl();

        private static double angle;

        private static Color FirstVertexColor = Color.MidnightBlue;

        private static Random Rnd = new Random();

        private static readonly object MapLock = new object();

        #region private void Render()

        private static void Render()
        {
            Matrix4 lookat = Matrix4.LookAt(0, 5, 5, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);

            GL.Rotate(angle, 0.0f, 1.0f, 0.0f);
            angle += 0.5f;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            DrawCube();

            GlControl.SwapBuffers();
        }

        #endregion

        #region private void DrawCube()

        private static void DrawCube()
        {
            GL.Begin(BeginMode.Quads);

            GL.Color3(Color.Silver);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, -1.0f);

            GL.Color3(Color.Honeydew);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);

            GL.Color3(Color.Moccasin);

            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);

            GL.Color3(Color.IndianRed);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);

            GL.Color3(Color.PaleVioletRed);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);

            GL.Color3(Color.ForestGreen);
            GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);

            GL.End();
        }

        #endregion

        [STAThread]
        public static void Main()
        {
            var map = new Map();
            using (var game = new GameWindow())
            {
                game.Load += (sender, e) =>
                {
                    // setup settings, load textures, sounds
                    game.VSync = VSyncMode.On;
                    InitColors(map);
                };

                game.Resize += (sender, e) =>
                {
                    GL.Viewport(0, 0, game.Width, game.Height);
                };

                game.UpdateFrame += (sender, e) =>
                    {
                        // add game logic, input handling
                        if (game.Keyboard[Key.Escape])
                        {
                            game.Exit();
                        }
                        if (game.Keyboard[Key.Enter])
                        {
                            InitColors(map);
                        }
                        var newMap = new Map();
                        /*int maxParallelism = Math.Max(1, Environment.ProcessorCount - 1);
                        int cellsPerTask = Map.XSize * Map.YSize / maxParallelism;

                        Map map2 = map;
                        Task.Factory.StartNew(() =>
                            {
                                for (int i = 0; i < newMap.Cells.Length / 2; i++)
                                {
                                    newMap.Cells[i].Color = Neighbourhood(map2, map2.Cells[i].X, map2.Cells[i].Y);
                                }
                            });
                        Map map1 = map;
                        Task.Factory.StartNew(()=>
                            {
                                for (int i = newMap.Cells.Length / 2; i < newMap.Cells.Length; i++)
                                {
                                    newMap.Cells[i].Color = Neighbourhood(map1, map1.Cells[i].X, map1.Cells[i].Y);
                                }
                            });
                        var startParallelTime = DateTime.Now;
                        Task.WaitAll();
                        Console.WriteLine("Finished parallelising, took {0}ms", DateTime.Now.Subtract(startParallelTime).Milliseconds);*/

                        for (int i = 0; i < newMap.Cells.Length; i++)
                            {
                                //var startParallelTime = DateTime.Now;
                                newMap.Cells[i].Color = Neighbourhood(map, map.Cells[i].X, map.Cells[i].Y);
                                //Task.Factory.StartNew(() =>
                                //    Parallel.For((long)0, maxParallelism,
                                //        new ParallelOptions { MaxDegreeOfParallelism = maxParallelism },
                                //        (taskIndex) =>
                                //        {
                                //            for (long i = taskIndex * cellsPerTask; i < (taskIndex + 1) * cellsPerTask; i++)
                                //            {
                                //                newMap.Cells[i].Color = Neighbourhood(map, map.Cells[i].X, map.Cells[i].Y);
                                //            }
                                //            //Console.WriteLine("Task {0} has finished...", taskIndex);
                                //        })
                                //);
                                //Console.WriteLine("Finished parallelising, took {0}ms", DateTime.Now.Subtract(startParallelTime).Milliseconds);
                            }
                        map = newMap;
                    };

                game.RenderFrame += (sender, e) =>
                    {
                    // render graphics
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    GL.MatrixMode(MatrixMode.Projection);
                    GL.LoadIdentity();
                    GL.Ortho(0, 1.0, 0, 1.0, 0.0, 1.0);

                    float cellHeight = (float)game.Height / Map.YSize;
                    float cellWidth = (float)game.Width / Map.XSize;

                    GL.Begin(PrimitiveType.Quads);

                    for (int y = 0; y < Map.YSize; y++)
                    {
                        for (int x = 0; x < Map.XSize; x++)
                        {
                            var cell = map.Cells[(y * Map.YSize) + x];
                            var bottomLeft = new Vector2(cellWidth * x / game.Width, cellHeight * y / game.Height);
                            var bottomRight = new Vector2(
                                cellWidth * (x + 1) / game.Width,
                                cellHeight * y / game.Height);
                            var topLeft = new Vector2(
                                cellWidth * x / game.Width,
                                cellHeight * (y + 1) / game.Height);
                            var topRight = new Vector2(
                                cellWidth * (x + 1) / game.Width,
                                cellHeight * (y + 1) / game.Height);

                            GL.Color3(cell.Color);

                            GL.Vertex2(bottomLeft);
                            GL.Vertex2(topLeft);

                            GL.Vertex2(topRight);
                            GL.Vertex2(bottomRight);
                        }
                    }

                    GL.End();
                    game.SwapBuffers();
                    game.Title = string.Format("FPS: {0} | Cells: {1}", game.RenderFrequency, (Map.XSize * Map.YSize).ToString("N0"));
                };

                // Run the game at 60 updates per second
                game.Run(10);
            }
        }

        private static void InitColors(Map map)
        {
            var colorConverter = new ColorConverter();
            for (int i = 0; i < Map.XSize * Map.YSize; i++)
            {
                map.Cells[i].Color = Color.FromArgb(Rnd.Next(0, 255), Rnd.Next(0, 255), Rnd.Next(0, 255));
            }
        }
        private static Color Neighbourhood(Map map, int x, int y)
        {
            Color[] values = new Color[9];

            int startPosX = (x - 1 < 0) ? x : x - 1;
            int startPosY = (y - 1 < 0) ? y : y - 1;
            int endPosX = (x >= Map.XSize - 1) ? x : x + 1;
            int endPosY = (y >= Map.YSize - 1) ? y : y + 1;


            // See how many are alive
            int arrayIndex = 0;
            for (int rowNum = startPosX; rowNum <= endPosX; rowNum++)
            {
                for (int colNum = startPosY; colNum <= endPosY; colNum++, arrayIndex++)
                {
                    values[arrayIndex] = map.Cells[(rowNum * Map.YSize) + colNum].Color;
                }
            }
            //var mostCommon = values.GroupBy(v => v).OrderByDescending(g => g.Count()).FirstOrDefault();
            //return mostCommon != null ? (int)Math.Round(mostCommon.First() + (((double)Random.Next(-1, 2)) * 0.6)) : cells[x, y].C;
            //int average = 0;
            //for (int i = 0; i < arrayIndex; i++)
            //{
            //    average += values[i].ToArgb();
            //}
            //average = (int)Math.Round(average / ((double)arrayIndex + 1));
            return Color.FromArgb((int)Math.Round(values.Select(v => v.ToArgb()).Average()));
        }
    }
}