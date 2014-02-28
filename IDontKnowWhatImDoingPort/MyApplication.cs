﻿namespace IDontKnowWhatImDoingPort
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

    class MyApplication : GameWindow
    {
        private const int XSize = 200;
        private const int YSize = 200;
        private static Map map = new Map(XSize,YSize);
        private static Map mapBuffer = new Map(XSize, YSize);

        private static readonly Random Rnd = new Random();

        [STAThread]
        public static void Main()
        {
            using (var game = new GameWindow())
            {
                //game.WindowBorder = WindowBorder.Hidden;
                game.WindowState = WindowState.Maximized;

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

                game.UpdateFrame += OnUpdate;

                game.RenderFrame += OnRender;

                // Run the game at 60 updates per second
                game.Run(1);
            }
        }

        #region Render
        private static void OnRender(object sender, FrameEventArgs e)
        {
            // render graphics
            var game = (GameWindow)sender;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();
            //GL.Ortho(0, 1.0, 0, 1.0, 0.0, 1.0);

            float cellHeight = (float)game.Height / YSize;
            float cellWidth = (float)game.Width / XSize;

            GL.Begin(PrimitiveType.Triangles);

            for (int y = 0; y < YSize; y++)
            {
                for (int x = 0; x < XSize; x++)
                {
                    var cell = map.Cells[(y * YSize) + x];
                    var bottomLeft = new Vector2(cellWidth * x / game.Width * 2 - 1,
                        cellHeight * y / game.Height * 2 - 1);
                    var bottomRight = new Vector2(
                        cellWidth * (x + 1) / game.Width * 2 - 1,
                        cellHeight * y / game.Height * 2 - 1);
                    var topLeft = new Vector2(
                        cellWidth * x / game.Width * 2 - 1,
                        cellHeight * (y + 1) / game.Height * 2 - 1);
                    var topRight = new Vector2(
                        cellWidth * (x + 1) / game.Width * 2 - 1,
                        cellHeight * (y + 1) / game.Height * 2 - 1);

                    GL.Color3(cell.Color);

                    GL.Vertex2(bottomLeft);
                    GL.Vertex2(topLeft);
                    GL.Vertex2(topRight);

                    GL.Vertex2(topRight);
                    GL.Vertex2(bottomRight);
                    GL.Vertex2(bottomLeft);
                }
            }

            GL.End();
            game.SwapBuffers();
            game.Title =
                string.Format(
                    "FPS: {0} | Cells: {1} | Render Time: {2} | Update Time: {3}",
                    game.RenderFrequency.ToString("0.0000"), (XSize * YSize).ToString("N0"),
                    game.RenderTime.ToString("0.0000"), game.UpdateTime.ToString("0.0000"));
        }
        #endregion

        #region Update
        private static void OnUpdate(object sender, FrameEventArgs e)
        {
            // add game logic, input handling
            var game = ((GameWindow)sender);
            if (game.Keyboard[Key.Escape])
            {
                game.Exit();
            }
            if (game.Keyboard[Key.Enter])
            {
                InitColors(map);
            }
            int maxParallelism = Math.Max(1, Environment.ProcessorCount - 1);
            int cellsPerTask = XSize * YSize / maxParallelism;

            /*Map map2 = map;
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

            //for (int i = 0; i < mapBuffer.Cells.Length; i++)
            //{
                //var startParallelTime = DateTime.Now;
                //mapBuffer.Cells[i].Color = Neighbourhood(map, map.Cells[i].X, map.Cells[i].Y);
                //Task.Factory.StartNew(() =>
            Parallel.For(
                (long)0,
                maxParallelism,
                new ParallelOptions { MaxDegreeOfParallelism = maxParallelism },
                (taskIndex) =>
                    {
                        var endRange = taskIndex == maxParallelism ? XSize * YSize : (taskIndex + 1) * cellsPerTask;
                        for (long i = taskIndex * cellsPerTask; i < endRange; i++)
                        {
                            mapBuffer.Cells[i].Color = Neighbourhood(map, map.Cells[i].X, map.Cells[i].Y);
                        }
                        //Console.WriteLine("Task {0} has finished...", taskIndex);
                    });
            //);
            //Console.WriteLine("Finished parallelising, took {0}ms", DateTime.Now.Subtract(startParallelTime).Milliseconds);
        //}
            var mapPH = map;
            map = mapBuffer;
            mapBuffer = mapPH;
        }
        private static byte[] Neighbourhood(Map map, int x, int y)
        {
            float[] values = new float[3];

            int startPosX = (x - 1 < 0) ? x : x - 1;
            int startPosY = (y - 1 < 0) ? y : y - 1;
            int endPosX = (x >= XSize - 1) ? x : x + 1;
            int endPosY = (y >= YSize - 1) ? y : y + 1;


            // See how many are alive
            int arrayIndex = 0;
            for (int rowNum = startPosX; rowNum <= endPosX; rowNum++)
            {
                for (int colNum = startPosY; colNum <= endPosY; colNum++)
                {
                    values[0] += map.Cells[(rowNum * YSize) + colNum].Color[0];
                    values[1] += map.Cells[(rowNum * YSize) + colNum].Color[1];
                    values[2] += map.Cells[(rowNum * YSize) + colNum].Color[2];
                    arrayIndex++;
                }
            }
            //var mostCommon = values.GroupBy(v => v).OrderByDescending(g => g.Count()).FirstOrDefault();
            //return mostCommon != null ? (int)Math.Round(mostCommon.First() + (((double)Rnd.Next(-1, 2)) * 0.6)) : cells[x, y].C;
            //int average = 0;
            //for (int i = 0; i < arrayIndex; i++)
            //{
            //    average += values[i].ToArgb();
            //}
            //average = (int)Math.Round(average / ((double)arrayIndex + 1));

            //return new[]
            //{
            //    (byte)(values[0] > (255f / 2f * arrayIndex) ? 255 : 0),
            //    (byte)(values[1] > (255f / 2f * arrayIndex) ? 255 : 0),
            //    (byte)(values[2] > (255f / 2f * arrayIndex) ? 255 : 0),
            //};
            //var neighbourhood = new[]
            //                        {
            //                            (byte)Math.Round((decimal)values[0]/(decimal)arrayIndex),
            //                            (byte)Math.Round((decimal)values[1]/(decimal)arrayIndex),
            //                            (byte)Math.Round((decimal)values[2]/(decimal)arrayIndex),
            //                        };
            float maxValue = values[0] > values[1] && values[0] > values[2]
                               ? values[0]
                               : values[1] > values[0] && values[1] > values[2] ? values[1] : values[2];
            float scale = values.Sum() / 255;
            var neighbourhood = new byte[]
                                    {
                                        (byte)Math.Round(values[0] / scale),
                                        (byte)Math.Round(values[1] / scale),
                                        (byte)Math.Round(values[2] / scale),
                                    };
            return neighbourhood;
        }
        #endregion

        private static void InitColors(Map map)
        {
            var colorConverter = new ColorConverter();
            for (int i = 0; i < XSize * YSize; i++)
            {
                //map.Cells[i].Color = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                //map.Cells[i].Color = new[] { (byte)0, (byte)0, (byte)Rnd.Next(0, 255) };
                var r = (byte)Rnd.Next(0, 256);
                var g = (byte)(Rnd.Next(0, 256 - r));
                var b = (byte)(255 - g - r);
                map.Cells[i].Color = new[] { r, g, b };
            }
        }
    }
}