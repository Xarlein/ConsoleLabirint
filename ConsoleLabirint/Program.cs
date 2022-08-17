using System;
using System.Collections.Generic;

namespace ConsoleLabirint
{
    public class Program
    {

        private static Engine _engine;

        private static void Main()
        {
            while (true)
            {
                _engine = new Engine();
                _engine.StartGame();
                Console.Clear();
                Console.WriteLine("WE ARE THE CHAPIONS!\n\nRestart Game? \n\tAny key - restart\n\tESCAPE - exit");

                if (Console.ReadKey().Key == ConsoleKey.Escape)
                    break;

                Console.Clear();
            }
        }
    }

    public struct MapData : IInit
    {
        private const int mapSize = 13;
        private byte[,] _map;
        private Dictionary<byte, string> _mapElements;

        public static int MapSize => mapSize;
        public byte[,] Map { get => _map; set => _map = value; }
        public Dictionary<byte, string> MapElements { get => _mapElements; private set => _mapElements = value; }

        public void Init()
        {
            _map = new byte[mapSize, mapSize]
            {
                { 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 1, 0, 1, 0, 1, 1, 1, 1, 1 },
                { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1 },
                { 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1 },
                { 1, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 0, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 0, 1, 0, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 0, 0, 0, 1, 1, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
            };

            _mapElements = new Dictionary<byte, string>()
            {
                { 0, "   " },  // void code
                { 1, "▐▐▐" },  // wall code
                { 2, " ☻ " },  // player code
                { 3, " @ " }   // end game
            };
        }

        public string Compare(byte index)
        {
            return _mapElements[index];
        }

    }

    #region INTERFACES

    public interface IDraw
    {
        public void Draw();
    }

    public interface IInit
    {
        public void Init();
    }

    #endregion

    public class Map : IDraw
    {
        private MapData _mapData;
        private Vector2d _endGame;

        public Map(MapData mapData)
        {
            _mapData = mapData;
            _endGame = new Vector2d(7, 11);
            _mapData.Map[_endGame.X, _endGame.Y] = 3;
        }

        public void Draw()
        {
            for (int x = 0; x < MapData.MapSize; x++)
            {
                for (int y = 0; y < MapData.MapSize; y++)
                {
                    Console.Write(_mapData.Compare(_mapData.Map[x, y]));
                }
                Console.WriteLine();
            }
        }

        public bool IsGameEnd(Vector2d player)
        {
            return (player.X == _endGame.X && player.Y == _endGame.Y);
        }

    }

    public class Vector2d
    {
        private int _x;
        private int _y;

        public int X { get => _x; set => _x = value; }
        public int Y { get => _y; set => _y = value; }

        public Vector2d()
        {
            _x = 0;
            _y = 0;
        }

        public Vector2d(int x, int y)
        {
            _x = x;
            _y = y;
        }
    }

    public class Player : IDraw
    {

        private Vector2d _position;
        private Vector2d _prevPosition;
        private Vector2d _direction;
        private MapData _mapData;

        public Vector2d Posiiton { get => _position; private set => _position = value; }

        public Player(MapData mapData)
        {
            _mapData = mapData;
            _position = new Vector2d(0, 1);
            _direction = new Vector2d();
            _prevPosition = new Vector2d();
        }

        public void SetPosition(Vector2d position)
        {
            _position = position;
        }

        public void Spawn()
        {
            _position = new Vector2d(0, 1);
        }

        public void Draw()
        {
            _mapData.Map[_prevPosition.X, _prevPosition.Y] = (byte)(_mapData.Map[_prevPosition.X, _prevPosition.Y] == 1 ? 1 : 0);
            _mapData.Map[_position.X, _position.Y] = 2;
        }

        public void HandleInput()
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.W:
                    _direction = new Vector2d(1, 0);
                    break;
                case ConsoleKey.S:
                    _direction = new Vector2d(-1, 0);
                    break;
                case ConsoleKey.A:
                    _direction = new Vector2d(0, -1);
                    break;
                case ConsoleKey.D:
                    _direction = new Vector2d(0, 1);
                    break;
            }

            _prevPosition = _position;

            _position = new Vector2d(
                ((_position.X + _direction.X) < MapData.MapSize && (_position.X + _direction.X) > -1) ?
                    (_position.X + _direction.X) : _position.X,
                ((_position.Y + _direction.Y) < MapData.MapSize && (_position.Y + _direction.Y) > -1) ?
                    (_position.Y + _direction.Y) : _position.Y);

            _position = (_mapData.Map[_position.X, _position.Y] != 1) ? _position : _prevPosition;


        }

        public void Debug()
        {
            Console.WriteLine($"DEBUG:\ndirection: X: {_direction.X} Y: {_direction.Y}\nposition: X: {_position.X} Y: {_position.Y}");
        }
    }

    public class Engine
    {
        private MapData _mapData;
        private Map _map;
        private Player _player;

        public Engine()
        {

            _mapData = new MapData();
            _mapData.Init();

            _map = new Map(_mapData);
            _player = new Player(_mapData);
        }

        public void HowToUsage()
        {
            Console.WriteLine("\nUsage:\n\tW - Forward\n\tS - Back\n\tA - Left\n\tD - Right\n");
        }

        public void StartGame()
        {
            Console.Title = "SUPER DUPER GAME by Xarlein v_0.1";
            _player.Spawn();


            while (true)
            {
#if DEBUG
                _player.Debug();
#endif
                HowToUsage();
                _map.Draw();
                _player.HandleInput();

                if (_map.IsGameEnd(_player.Posiiton))
                    break;

                _player.Draw();
                Console.Clear();
            }
        }
    }
}
