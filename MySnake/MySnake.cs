using PluginInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MySnake
{
    public class MySnake : ISmartSnake
    {
        public Move Direction { get; set; }
        public bool Reverse { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }

        private List<Point> stones;



        public void Startup(Size size, List<Point> stones)
        {
            Name = "MySnake";
            Color = Color.Violet;
            this.stones = stones;
        }


        private Point prevPosition = Point.Empty;

        public void Update(Snake snake, List<Snake> enemies, List<Point> food, List<Point> dead)
        {

            // Находим ближайшую еду
            Point closestFood = GetClosestFood(snake.Position, food);


            // Определяем направление к ближайшей еде
            if (closestFood != Point.Empty)
            {
                if (snake.Position.X < closestFood.X)
                {
                    if (snake.Position != prevPosition)
                    {
                        Direction = Move.Right;
                    }
                    else
                    {
                        Random rnd = new Random();
                        switch (rnd.Next(1, 3))
                        {
                            case 1: Direction = Move.Up; break;
                            case 2: Direction = Move.Down; break;
                            case 3: Direction = Move.Left; break;

                        }

                    }
                }
                else if (snake.Position.X > closestFood.X)
                {
                    if (snake.Position != prevPosition)
                    {
                        Direction = Move.Left;
                    }
                    else
                    {
                        Random rnd = new Random();
                        switch (rnd.Next(1, 3))
                        {
                            case 1: Direction = Move.Down; break;
                            case 2: Direction = Move.Up; break;
                            case 3: Direction = Move.Right; break;

                        }

                    }
                }
                else if (snake.Position.Y < closestFood.Y)
                {
                    if (snake.Position != prevPosition)
                    {
                        Direction = Move.Down;
                    }
                    else
                    {
                        Random rnd = new Random();
                        switch (rnd.Next(1, 3))
                        {
                            case 1: Direction = Move.Left; break;
                            case 2: Direction = Move.Right; break;

                            case 3: Direction = Move.Up; break;
                        }
                    }
                }
                else if (snake.Position.Y > closestFood.Y)
                {
                    if (snake.Position != prevPosition)
                    {
                        Direction = Move.Up;
                    }
                    else
                    {
                        Random rnd = new Random();
                        switch (rnd.Next(1, 3))
                        {
                            case 1: Direction = Move.Left; break;
                            case 2: Direction = Move.Right; break;
                            case 3: Direction = Move.Down; break;
                        }

                    }
                }
            }
            prevPosition = snake.Position;


            // Проверяем, является ли следующая точка движения точкой из списка stones
            Point nextPosition = GetNextPosition(snake.Position, Direction);
            if (stones.Contains(nextPosition) || nextPosition == prevPosition)
            {
                // Выбираем другое направление
                Direction = GetSafeDirection(snake.Position, stones);
                nextPosition = GetNextPosition(snake.Position, Direction);

                // Если новая позиция также является точкой из списка stones, двигаемся в текущем направлении
                if (stones.Contains(nextPosition) || nextPosition == prevPosition)
                {
                    nextPosition = GetNextPosition(snake.Position, Direction);
                }
            }

            prevPosition = snake.Position;

        }

        // Метод для вычисления следующей позиции на основе текущей позиции и направления
        private Point GetNextPosition(Point currentPosition, Move direction)
        {
            Point nextPosition = currentPosition;

            switch (direction)
            {
                case Move.Up:
                    nextPosition.Y--;
                    break;
                case Move.Down:
                    nextPosition.Y++;
                    break;
                case Move.Left:
                    nextPosition.X--;
                    break;
                case Move.Right:
                    nextPosition.X++;
                    break;
            }

            return nextPosition;
        }
        // Метод для выбора безопасного направления, исключая точки из списка stones и избегая тупиков
        private Move GetSafeDirection(Point currentPosition, List<Point> stones)
        {
            Random rnd = new Random();
            List<Move> safeDirections = new List<Move>();

            foreach (Move direction in Enum.GetValues(typeof(Move)))
            {
                Point nextPosition = GetNextPosition(currentPosition, direction);
                if (!stones.Contains(nextPosition) && !IsDeadEnd(nextPosition, stones))
                {
                    safeDirections.Add(direction);
                }
            }

            if (safeDirections.Count > 0)
            {
                // Если текущее направление безопасно, оставляем его
                if (safeDirections.Contains(Direction))
                {
                    return Direction;
                }

                return safeDirections[rnd.Next(safeDirections.Count)];
            }
            else if (!stones.Contains(GetNextPosition(currentPosition, Direction)))
            {
                // Если текущее направление безопасно, оставляем его
                return Direction;
            }
            else
            {
                // Если нет безопасного направления, выбираем случайное безопасное направление
                return GetRandomSafeDirection(currentPosition, stones);
            }
        }

        // Метод для выбора случайного безопасного направления
        private Move GetRandomSafeDirection(Point currentPosition, List<Point> stones)
        {
            Random rnd = new Random();
            List<Move> safeDirections = new List<Move>();

            foreach (Move direction in Enum.GetValues(typeof(Move)))
            {
                Point nextPosition = GetNextPosition(currentPosition, direction);
                if (!stones.Contains(nextPosition))
                {
                    safeDirections.Add(direction);
                }
            }

            if (safeDirections.Count > 0)
            {
                return safeDirections[rnd.Next(safeDirections.Count)];
            }
            else
            {
                // Если нет безопасного направления, возвращаем случайное направление
                return (Move)rnd.Next(1, 5);
            }
        }

        // Метод для проверки, является ли позиция тупиком (закрытым местом)
        private bool IsDeadEnd(Point currentPosition, List<Point> stones)
        {
            int count = 0;
            foreach (Move direction in Enum.GetValues(typeof(Move)))
            {
                Point nextPosition = GetNextPosition(currentPosition, direction);
                if (!stones.Contains(nextPosition))
                {
                    count++;
                }
            }
            return count == 0;
        }

        // Метод для нахождения ближайшей еды
        private Point GetClosestFood(Point position, List<Point> food)
        {
            if (food.Count == 0)
            {
                return Point.Empty;
            }

            Point closestFood = food[0];
            double closestDistance = GetDistance(position, closestFood);

            foreach (Point f in food)
            {
                double distance = GetDistance(position, f);
                if (distance < closestDistance)
                {
                    closestFood = f;
                    closestDistance = distance;
                }
            }
            return closestFood;
        }

        //вычисление расстояния между двумя точками
        private double GetDistance(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
    }

}