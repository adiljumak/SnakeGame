using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Serialization;



//TODO: Добавить проверку чтобы змейка за край не выходила
//TODO: Добавить проверку чтобы еда не появлялась в змейке или в стене
//TODO: Добавить проверку чтобы змейка в себя не врезалась
//TODO: Чтобы еда в стене не появилась
//TODO: Столкновение змейки со стеной
//TODO: Логика перехода между уровнями(если набрали какое-то кол-во очков)
//TODO: Изменять уровень через какое-время
//TODO: Добавить таймер чтобы еда появлялась
//TODO: Remove draw method from snake wtf


namespace SnakeGame
{
    public abstract class GameObject
    {
        public char sign;
        public List<Point> body;
        public ConsoleColor color;

        public GameObject(char sign, ConsoleColor color)
        {
            this.sign = sign;
            this.color = color;
            this.body = new List<Point>();
        }


        public void draw()
        {
            Console.ForegroundColor = color;
            for (int i = 0; i < body.Count; i++)
            {
                Console.SetCursorPosition(body[i].X, body[i].Y);
                Console.Write(sign);
            }
        }


        public void clear()
        {
            for (int i = 0; i < body.Count; i++)
            {
                Console.SetCursorPosition(body[i].X, body[i].Y);
                Console.Write(' ');
            }
        }


        


    }

    


    public class Point
    {

        int x;
        int y;

        public int X
        {
            get
            {
                return x;
            }
            set
            {
                if(value < 0)
                {
                    x = Program.Width - 1;
                }
                else if(value > Program.Width + 1)
                {
                    x = 0;
                }
                else
                {
                    x = value;
                }
            }
        }
        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                if (value < 0)
                {
                    y = Program.Height - 1;
                }
                else if(value > Program.Height + 1)
                {
                    y = 0;
                }
                else
                {
                    y = value;
                }
            }
        }

        /*
         public Point(int X, int Y)
        { 
            this.X = X > 0 ? X : Program.Width;
            this.Y = Y > 0 ? Y : Program.Height;
            this.X = X;
            this.Y = Y;
            
        } 
           */


    }



    class Snake:GameObject
    {
        
       


        public int dx = 0, dy = 0;

        public Snake(int headX, int headY, char sign, ConsoleColor color):base(sign,color)
        {
            Point head = new Point { X = headX, Y = headY };
           
            body.Add(head);
         
            draw();
        } 

        


        public void move(Food food)
        {


            for (int i = body.Count - 1; i > 0; --i)
            {
                body[i].X = body[i - 1].X;
                body[i].Y = body[i - 1].Y;
            }

            body[0].X += dx;
            body[0].Y += dy;




            /*if(body[0].X < 0)
            {
                body[0].X = Program.Width - 1;
            }
            if(body[0].Y < 0)
            {
                body[0].Y = Program.Height - 1;
            }*/





            food.isCollision(body[0]);


        }

        public void changeDirection(int dx, int dy)
        {
            this.dx = dx;
            this.dy = dy;
        }

        public void growth(Point point)
        {
            body.Add(new Point { X = point.X, Y = point.Y});
        }
    }


    
    class Food : GameObject
    {
        Random rnd = new Random();
        public Food(int x, int y, char sign, ConsoleColor color) : base (sign, color)
        {
            Point position = new Point { X = x, Y = y };
            
            body.Add(position);
            draw();
        }


        public bool isCollision(Point head)
        {
            return head.X == body[0].X && head.Y == body[0].Y;
        }

        public void generate(Snake snake, Wall wall)
        {


            while (true)
            {
                bool ok = true;


                int randomX = rnd.Next(2, Program.Width - 1);
                int randomY = rnd.Next(2, Program.Height - 1);

                for(int i = 0; i < snake.body.Count; i++)
                {
                    if(snake.body[i].X == randomX && snake.body[i].Y == randomY)
                    {
                        ok = false;
                    }
                }
                for (int i = 0; i < wall.body.Count; i++)
                {
                    if (wall.body[i].X == randomX && wall.body[i].Y == randomY)
                    {
                        ok = false;
                    }
                }
                if (ok)
                {
                    this.body.Add(new Point { X = randomX, Y = randomY });
                    break;
                }
                

            }

            
            //body[0].X = rnd.Next(1, 20);
            //body[0].Y = rnd.Next(1, 20);
            //draw();
        }

        public void draw()
        {
            Console.ForegroundColor = color;
            Console.SetCursorPosition(body[body.Count - 1].X, body[body.Count - 1].Y);
            Console.Write(sign);
        }
    }



    class Wall: GameObject
    {

        public Wall(char sign, ConsoleColor color, string path):base(sign, color)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using(StreamReader reader = new StreamReader(fs))
                {
                    int rowNubmer = 0;
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        for(int columnNumber = 0; columnNumber < line.Length; columnNumber++)
                        {
                            if (line[columnNumber] == '#')
                            {
                                body.Add(new Point { X = columnNumber, Y = rowNubmer });
                            }
                        }

                        rowNubmer++;
                    }
                    



                }
            }


            draw();
        }
    }




    public class Program
    {
        public static bool superPowerActive = false;
        public static int speed = 100;

        public static int num = 0;
        public static TimerCallback tm = new TimerCallback(moving);
        public static Timer snakeTimer = new Timer(tm, num, 0, speed);
        


        public static TimerCallback tm2 = new TimerCallback(spawnNewFood);
        public static Timer spawnNewFoodTimer = new Timer(tm2, num, 2000, 2000);

        public static TimerCallback tm3 = new TimerCallback(superPower);
        public static Timer superPowerTimer = new Timer(tm3, 0, 10000, 10000);

        public static bool running = true;

        public static Random rnd = new Random();

        public static int Width = 40;
        public static int Height = 40;

        static Snake snake = new Snake(5, 4, '$', ConsoleColor.DarkGreen);

        static Food food = new Food(5, 5, '@', ConsoleColor.Yellow);

        static Wall wall = new Wall('#', ConsoleColor.Red, @"Level1.txt");

        //static int level = 1;


        static public bool nextLevel = true;
        static void moving(object obj)
        {
            
            snake.clear();
            snake.move(food);
            snake.draw();

            for(int i = 0; i < food.body.Count; i++)
            {
                if (snake.body[0].X == food.body[i].X && snake.body[0].Y == food.body[i].Y)
                {

                    snake.growth(snake.body[0]);
                    //food.generate(rnd.Next(1, Width), rnd.Next(1, Height));
                }
            }
            
            for (int i = 0; i < wall.body.Count; i++)
            {
                if (snake.body[0].X == wall.body[i].X && snake.body[0].Y == wall.body[i].Y)
                {

                    Console.Clear();
                    Console.WriteLine("END GAME");
                    Program.running = false;
                    Console.Write("Enter esc to close the app");
                    snakeTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    spawnNewFoodTimer.Change(Timeout.Infinite, Timeout.Infinite);

                    
                }
            }
            



            if (snake.body.Count > 3 && Program.nextLevel)
            {
                Program.nextLevel = false;
                Console.Clear();
                wall = new Wall('#', ConsoleColor.Red, @"Level2.txt");
                for(int i = 1; i < snake.body.Count; i++)
                {
                    snake.body.RemoveAt(i);
                }

            }


            Console.SetCursorPosition(0, Program.Height + 2);
            Console.Write("Size: " + snake.body.Count + " " + DateTime.Now.ToLongTimeString());
            
            

        }


        static void spawnNewFood(object obj)
        {
           
           food.generate(snake, wall);
           food.draw();
        }

        /*
        static void changeMap(object obj)
        {
            level++;
            snake.body.RemoveAll(x => x == null);
            food.body.RemoveAll(x => x == null);

            Console.Clear();

            string levelPath = "Level" + level.ToString();

            wall = new Wall('#', ConsoleColor.Red, @levelPath);
        }*/

        static void superPower(object obj)
        {
            if (!superPowerActive)
            {
                int x = rnd.Next(1, 100);
                if (x > 30)
                {
                    Program.speed /= 2;
                    snakeTimer.Change(0, speed);
                    superPowerActive = true;
                }
            }
            else
            {
                Program.speed *= 2;
                superPowerActive = false;
                snakeTimer.Change(0, speed);
            }
        }

        public static void Main(string[] args)
        {

            

            bool pause = false;



            




            
            //TimerCallback tm3 = new TimerCallback(changeMap);
            //Timer changeMapTimer = new Timer(tm3, level, 0, 2000000);


            Random rnd = new Random();



            

            Console.CursorVisible = false;
            Console.SetCursorPosition(20, 20);



           

            while (running)
            {
                


                //snake.clear();
                //snake.move(food);

                
                



                //snake.draw();
                
               

                

                //Thread.Sleep(1000);

                ConsoleKeyInfo pressedKey = Console.ReadKey();
                switch (pressedKey.Key)
                {
                    case ConsoleKey.UpArrow:
                        snake.changeDirection(0, -1);
                        break;
                    case ConsoleKey.DownArrow:
                        snake.changeDirection(0, 1);
                        break;
                    case ConsoleKey.LeftArrow:
                        snake.changeDirection(-1, 0);
                        break;
                    case ConsoleKey.RightArrow:
                        snake.changeDirection(1, 0);
                        break;
                   
                    case ConsoleKey.Tab:
                        pause = !pause;
                        if (pause)
                        {
                            snakeTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        }
                        else
                        {
                            snakeTimer.Change(0, speed);
                        }

                        break;
                    case ConsoleKey.Escape:
                        running = false;                      
                        break;
                }





                





            }





        }
    }
}
