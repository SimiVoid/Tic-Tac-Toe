using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool?[][] BoardData;
        public bool? GameMode = null;
        public bool WhoMove;
        public MainWindow()
        {
            InitializeComponent();

            BoardData = new bool?[3][];

            for (var x = 0; x < 3; ++x)
            {
                BoardData[x] = new bool?[3];
                for (var y = 0; y < 3; ++y) BoardData[x][y] = null;
            }

            DrawBoard();
        }

        public void DrawBoard()
        {
            Board.Background = Brushes.Black;

            var title = new TextBlock()
            {
                Text = "Tic Tac Toe",
                FontSize = 26,
                Foreground = Brushes.White
            };

            Canvas.SetTop(title, 10);
            Canvas.SetLeft(title, 90);
            Board.Children.Add(title);

            var line1 = new Line()
            {
                X1 = 30,
                Y1 = 260,
                X2 = 270,
                Y2 = 260,
                Stroke = Brushes.White,
                StrokeThickness = 2
            };

            Board.Children.Add(line1);

            var line2 = new Line()
            {
                X1 = 30,
                Y1 = 340,
                X2 = 270,
                Y2 = 340,
                Stroke = Brushes.White,
                StrokeThickness = 2
            };

            Board.Children.Add(line2);

            var line3 = new Line()
            {
                X1 = 110,
                Y1 = 180,
                X2 = 110,
                Y2 = 420,
                Stroke = Brushes.White,
                StrokeThickness = 2
            };

            Board.Children.Add(line3);

            var line4 = new Line()
            {
                X1 = 190,
                Y1 = 180,
                X2 = 190,
                Y2 = 420,
                Stroke = Brushes.White,
                StrokeThickness = 2
            };

            Board.Children.Add(line4);
        }

        private void DrawCross(int x, int y)
        {
            var line1 = new Line()
            {
                X1 = 30 + x * 80 + 40 - 30,
                Y1 = 180 + y * 80 + 40 - 30,
                X2 = 30 + x * 80 + 40 + 30,
                Y2 = 180 + y * 80 + 40 + 30,
                Stroke = Brushes.White,
                StrokeThickness = 2
            };

            Board.Children.Add(line1);

            var line2 = new Line()
            {
                X1 = 30 + x * 80 + 40 + 30,
                Y1 = 180 + y * 80 + 40 - 30,
                X2 = 30 + x * 80 + 40 - 30,
                Y2 = 180 + y * 80 + 40 + 30,
                Stroke = Brushes.White,
                StrokeThickness = 2
            };

            Board.Children.Add(line2);
        }

        private void DrawCircle(int x, int y)
        {
            var circle = new Ellipse()
            {
                Height = 60,
                Width = 60,
                Stroke = Brushes.White,
                StrokeThickness = 2,
            };

            Canvas.SetTop(circle, 180 + 10 + y * 80);
            Canvas.SetLeft(circle, 30 + 10 + x * 80);

            Board.Children.Add(circle);
        }

        private Tuple<bool?, List<Point>> CheckBoard()
        {
            for (var i = 0; i < 3; ++i)
            {
                var points = new List<Point>();
                var c1 = 0;
                var c2 = 0;

                for (var j = 0; j < 3; ++j)
                    if (BoardData[i][j] is true)
                    {
                        c1++;
                        points.Add(new Point(i, j));
                    }
                    else if (BoardData[i][j] is false)
                    {
                        c2++;
                        points.Add(new Point(i, j));
                    }

                if (c1 == 3) return Tuple.Create<bool?, List<Point>>(true, points);
                else if (c2 == 3) return Tuple.Create<bool?, List<Point>>(false, points);
            }

            for (var i = 0; i < 3; ++i)
            {
                var points = new List<Point>();
                var c1 = 0;
                var c2 = 0;

                for (var j = 0; j < 3; ++j)
                    if (BoardData[j][i] is true)
                    {
                        c1++;
                        points.Add(new Point(j, i));
                    }
                    else if (BoardData[j][i] is false)
                    {
                        c2++;
                        points.Add(new Point(j, i));
                    }

                if (c1 == 3) return Tuple.Create<bool?, List<Point>>(true, points);
                else if (c2 == 3) return Tuple.Create<bool?, List<Point>>(false, points);
            }

            if(BoardData[0][0] == BoardData[1][1] && BoardData[2][2] == BoardData[1][1])
            {
                var points = new List<Point> { new Point(0, 0), new Point(1, 1), new Point(2, 2) };

                if (BoardData[0][0] == true)
                    return Tuple.Create<bool?, List<Point>>(true, points);
                else if (BoardData[0][0] == false)
                    return Tuple.Create<bool?, List<Point>>(false, points);

            }

            if (BoardData[2][0] == BoardData[1][1] && BoardData[0][2] == BoardData[1][1])
            {
                var points = new List<Point> {new Point(2, 0), new Point(1, 1), new Point(0, 2)};

                if (BoardData[2][0] == true)
                    return Tuple.Create<bool?, List<Point>>(true, points);
                else if (BoardData[2][0] == false)
                    return Tuple.Create<bool?, List<Point>>(false, points);
            }

            return null;
        }

        private void ComputerMove()
        {
            int posX = -1, posY = -1;

            var freePoints = new List<Point>();

            for(var i = 0; i < 3; ++i)
                for(var j = 0; j < 3; ++j)
                    if (BoardData[i][j] == null)
                        freePoints.Add(new Point(i, j));

            foreach (var point in freePoints)
            {
                if (BoardData[Convert.ToInt32(point.X)][1] ==
                    BoardData[Convert.ToInt32(point.X)][0])
                {
                    posX = Convert.ToInt32(point.X);
                    posY = Convert.ToInt32(point.Y);
                }
                else if (BoardData[Convert.ToInt32(point.X)][2] ==
                        BoardData[Convert.ToInt32(point.X)][0])
                {
                    posX = Convert.ToInt32(point.X);
                    posY = Convert.ToInt32(point.Y);
                }
                else if (BoardData[Convert.ToInt32(point.X)][0] ==
                         BoardData[Convert.ToInt32(point.X)][1])
                {
                    posX = Convert.ToInt32(point.X);
                    posY = Convert.ToInt32(point.Y);
                }
                else if (BoardData[Convert.ToInt32(point.X)][2] ==
                        BoardData[Convert.ToInt32(point.X)][1])
                {
                    posX = Convert.ToInt32(point.X);
                    posY = Convert.ToInt32(point.Y);
                }
                else if (BoardData[Convert.ToInt32(point.X)][0] ==
                         BoardData[Convert.ToInt32(point.X)][2])
                {
                    posX = Convert.ToInt32(point.X);
                    posY = Convert.ToInt32(point.Y);
                }
                else if (BoardData[Convert.ToInt32(point.X)][1] == 
                         BoardData[Convert.ToInt32(point.X)][2])
                {
                    posX = Convert.ToInt32(point.X);
                    posY = Convert.ToInt32(point.Y);
                }
                else if (BoardData[1][Convert.ToInt32(point.Y)] ==
                         BoardData[0][Convert.ToInt32(point.Y)])
                {
                    posX = Convert.ToInt32(point.X);
                    posY = Convert.ToInt32(point.Y);
                }
                else if (BoardData[2][Convert.ToInt32(point.Y)] ==
                        BoardData[0][Convert.ToInt32(point.Y)])
                    {
                        posX = Convert.ToInt32(point.X);
                        posY = Convert.ToInt32(point.Y);
                    }
                else if (BoardData[2][Convert.ToInt32(point.Y)] ==
                         BoardData[1][Convert.ToInt32(point.Y)])
                {
                    posX = Convert.ToInt32(point.X);
                    posY = Convert.ToInt32(point.Y);
                }
                else if (BoardData[1][Convert.ToInt32(point.Y)] ==
                        BoardData[0][Convert.ToInt32(point.Y)])
                {
                    posX = Convert.ToInt32(point.X);
                    posY = Convert.ToInt32(point.Y);
                }
                else if (BoardData[0][Convert.ToInt32(point.Y)] ==
                         BoardData[2][Convert.ToInt32(point.Y)])
                {
                    posX = Convert.ToInt32(point.X);
                    posY = Convert.ToInt32(point.Y);
                }
                else if (BoardData[1][Convert.ToInt32(point.Y)] ==
                        BoardData[2][Convert.ToInt32(point.Y)])
                {
                    posX = Convert.ToInt32(point.X);
                    posY = Convert.ToInt32(point.Y);
                }
                else if (BoardData[0][0] == BoardData[1][1] && Convert.ToInt32(point.Y) == 2 &&
                         Convert.ToInt32(point.X) == 2)
                {
                    posX = Convert.ToInt32(point.X);
                    posY = Convert.ToInt32(point.Y);
                }
                else if (BoardData[0][0] == BoardData[2][2] && Convert.ToInt32(point.Y) == 1 &&
                         Convert.ToInt32(point.X) == 1)
                {
                    posX = Convert.ToInt32(point.X);
                    posY = Convert.ToInt32(point.Y);
                }
                else if (BoardData[2][2] == BoardData[1][1] && Convert.ToInt32(point.Y) == 0 &&
                         Convert.ToInt32(point.X) == 0)
                {
                    posX = Convert.ToInt32(point.X);
                    posY = Convert.ToInt32(point.Y);
                }
                else if (BoardData[2][0] == BoardData[1][1] && Convert.ToInt32(point.Y) == 2 &&
                         Convert.ToInt32(point.X) == 0)
                {
                    posX = Convert.ToInt32(point.X);
                    posY = Convert.ToInt32(point.Y);
                }
                else if (BoardData[2][0] == BoardData[0][2] && Convert.ToInt32(point.Y) == 1 &&
                         Convert.ToInt32(point.X) == 1)
                {
                    posX = Convert.ToInt32(point.X);
                    posY = Convert.ToInt32(point.Y);
                }
                else if (BoardData[0][2] == BoardData[1][1] && Convert.ToInt32(point.Y) == 0 &&
                         Convert.ToInt32(point.X) == 2)
                {
                    posX = Convert.ToInt32(point.X);
                    posY = Convert.ToInt32(point.Y);
                }
                else
                {
                    posX = Convert.ToInt32(point.X);
                    posY = Convert.ToInt32(point.Y);
                }

                if (posX != -1 && posY != -1) break;
            }

            BoardData[posX][posY] = false;
            DrawCircle(posX, posY);
            WhoMove = true;
        }       

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Application.Current.Shutdown();
            else if (e.Key == Key.S)
            {
                GameMode = false;
                WhoMove = true;
            }
            else if (e.Key == Key.M)
            {
                GameMode = true;
                WhoMove = true;
            }
            else if (e.Key == Key.R)
            {
                GameMode = null;
                
                Board.Children.Clear();
                DrawBoard();

                for (var x = 0; x < 3; ++x)
                    for (var y = 0; y < 3; ++y) 
                        BoardData[x][y] = null;
            }
        }

        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (GameMode == null) return;

            var pos = Mouse.GetPosition(Board);

            for(var i = 0; i < 3; ++i)
            for(var j = 0; j < 3; ++j)
                if(pos.X >= 30 + i * 80 && pos.X <= 110 + i * 80 && pos.Y >= 180 + j * 80 && pos.Y <= 260 + j * 80)
                {
                    if (WhoMove && BoardData[i][j] == null)
                    {
                        BoardData[i][j] = true;
                        DrawCross(i, j);

                        WhoMove = false;
                        if (GameMode == false) ComputerMove();
                    }
                    else if (!WhoMove && GameMode == true && BoardData[i][j] == null)
                    {
                        BoardData[i][j] = false;
                        DrawCircle(i, j);

                        WhoMove = true;
                    }
                }

            var board = CheckBoard();

            if (board == null) return;
            
            GameMode = null;

            Debug.Print(board.Item2[0].X.ToString(CultureInfo.InvariantCulture) + " " + board.Item2[0].Y.ToString(CultureInfo.InvariantCulture));

            var line = new Line()
            {
                X1 = 30 + board.Item2[0].X * 80 + 40,
                Y1 = 180 + board.Item2[0].Y * 80 + 40,
                X2 = 30 + board.Item2[2].X  * 80 + 40,
                Y2 = 180 + board.Item2[2].Y * 80 + 40,
                Stroke = Brushes.White,
                StrokeThickness = 2
            };

            if (board.Item2[0].X == board.Item2[2].X)
            {
                line.Y1 -= 40;
                line.Y2 += 40;
            }
            else if (board.Item2[0].Y == board.Item2[2].Y)
            {
                line.X1 -= 40;
                line.X2 += 40;
            }
            else if (board.Item2[0].X == 0 && board.Item2[0].Y == 0)
            {
                line.X1 -= 40;
                line.Y1 -= 40;
                line.X2 += 40;
                line.Y2 += 40;
            }
            else
            {
                line.X1 += 40;
                line.Y1 -= 40;
                line.X2 -= 40;
                line.Y2 += 40;
            }

            Board.Children.Add(line);
        }
    }
}
