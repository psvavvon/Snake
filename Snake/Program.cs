﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SnakeClassLibrary;

namespace SnakeGame
{
    static class Program
    {
        static Program()
        {
            Console.CursorVisible = false;
        }

        static Game _game { get; set; }
        static int CurrentScore { get; set; } = 2;

        static void Main(string[] args)
        {
            while (true)
            {
                bool shouldQuit = StartGame();
                if (shouldQuit)
                    break;
            }
        }

        private static bool StartGame()
        {
            _game = new Game(10, 20);
            var direction = Snake.Direction.Up;
            ConsoleKey? lastKeyPressed = null;
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    lastKeyPressed = Console.ReadKey(true).Key;
                }
                switch (lastKeyPressed)
                {
                    case null:
                        break;
                    case ConsoleKey.W:
                        if (direction != Snake.Direction.Down)
                            direction = Snake.Direction.Up;
                        break;
                    case ConsoleKey.A:
                        if (direction != Snake.Direction.Right)
                            direction = Snake.Direction.Left;
                        break;
                    case ConsoleKey.S:
                        if (direction != Snake.Direction.Up)
                            direction = Snake.Direction.Down;
                        break;
                    case ConsoleKey.D:
                        if (direction != Snake.Direction.Left)
                            direction = Snake.Direction.Right;
                        break;
                    case ConsoleKey.Escape:
                        return true;
                }
                switch (_game.Tick(direction))
                {
                    case Game.GameEvent.Lose:
                        Console.SetCursorPosition(_game.Field.Width / 2, _game.Field.Height / 2);
                        Console.Write($"Game Over! Your score:{CurrentScore}");
                        Console.ReadKey();
                        Console.Clear();
                        return false;
                    case Game.GameEvent.Upscore:
                        CurrentScore++;
                        if (CurrentScore > Properties.Settings.Default.HighScore)
                        {
                            Properties.Settings.Default.HighScore = CurrentScore;
                            Properties.Settings.Default.Save();
                        }
                        break;
                    default:
                        break;
                    
                }
                Update(_game.Field);
                Thread.Sleep(TimeSpan.FromSeconds(0.25));
            }
        }

        private static void Update(Field field)
        {
            DrawBorders(field.Height, field.Width);
            Console.Write($"\nHigh Score: {Properties.Settings.Default.HighScore}");
            Console.Write($"\nYour Score: {CurrentScore}");
            DrawSnake(field.Snake);
            DrawFood(field);
            Console.SetCursorPosition(0, 0);
        }

        private static void DrawFood(Field field)
        {
            var food = field.Blocks.Cast<Block>().Single(block => block is FoodBlock);
            Console.SetCursorPosition(food.Coords.Column + 1, food.Coords.Row + 1);
            Console.Write(Constants.Chars.Extra.Food);
        }

        private static void DrawSnake(Snake snake)
        {
            foreach (SnakeBlock block in snake.Blocks)
            {
                Console.SetCursorPosition(block.Coords.Column + 1, block.Coords.Row + 1);
                Console.Write(Constants.Chars.Snake);
            }
        }

        private static void DrawBorders(int height, int width)
        {
            string borders = GetBorders(height, width);
            Console.Write(borders);
        }

        static string GetBorders(int height, int width)
        {
            var lines = new StringBuilder();
            var line = new StringBuilder();

            line.Append(Constants.Chars.Walls.Angles.LeftUpAngle);
            line.Append(Constants.Chars.Walls.HorizontalWall, width);
            line.Append(Constants.Chars.Walls.Angles.RightUpAngle);

            lines.AppendLine(line.ToString());

            line.Clear();

            line.Append(Constants.Chars.Walls.VerticalWall);
            line.Append(Constants.Chars.Empty, width);
            line.Append(Constants.Chars.Walls.VerticalWall);

            for (int i = 0; i < height; ++i)
            {
                lines.AppendLine(line.ToString());
            }

            line.Clear();
            line.Append(Constants.Chars.Walls.Angles.LeftBottomAngle);
            line.Append(Constants.Chars.Walls.HorizontalWall, width);
            line.Append(Constants.Chars.Walls.Angles.RightBottomAngle);

            lines.AppendLine(line.ToString());

            return lines.ToString();
        }
    }
}