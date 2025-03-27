using Microsoft.Win32.SafeHandles;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace vadaszdenes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            for (int i = 0; i < 20; i++)
            {
                citygrid.ColumnDefinitions.Add(new ColumnDefinition());
                citygrid.RowDefinitions.Add(new RowDefinition());
            }
            List<House> houses = new List<House>();

            houses.Add(new House(1, 2, 212, 50));

            foreach (House a in houses)
            {

                Image h = new Image
                {
                    Source = new BitmapImage(new Uri(a.getPicturePos())),
                    Height = 150,
                    Width = 150,
                    Visibility = Visibility.Visible,
                };

                Grid.SetColumn(h, 10);
                Grid.SetRow(h, 10);
                citygrid.Children.Add(h);

                
                
            }
            

        }

        Point m_start;
        Vector m_startOffset;

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as Grid;
            TranslateTransform translate = element.RenderTransform as TranslateTransform;

            m_start = e.GetPosition(gridHost);
            m_startOffset = new Vector(translate.X, translate.Y);
            element.CaptureMouse();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            FrameworkElement element = sender as Grid;
            TranslateTransform translate = element.RenderTransform as TranslateTransform;

            if (element.IsMouseCaptured)
            {
                Vector offset = Point.Subtract(e.GetPosition(gridHost), m_start);

                double newX = m_startOffset.X + offset.X;
                double newY = m_startOffset.Y + offset.Y;

                translate.X = Math.Max(-500, Math.Min(500, newX));
                translate.Y = Math.Max(-920, Math.Min(920, newY));
            }
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as Grid;
            element.ReleaseMouseCapture();
        }


        public void Show_Stats_Window(object s, MouseButtonEventArgs e)
        {
            Stats.Visibility = Visibility.Visible;



        }

        public void Update_Stats(object s, MouseButtonEventArgs e)
        {

        }


        abstract class Building
        {
            protected int pos_x;
            protected int pos_y;
            protected string type;
            protected int size;
            protected string picturePos;

            public Building(string picturePos ,int pos_x, int pos_y, string type, int size)
            {
                this.picturePos = picturePos;
                this.pos_x = pos_x;
                this.pos_y = pos_y;
                this.type = type;
                this.size = size;
            }

            public int getPos_X()
            {
                return pos_x;
            }

            public int getPos_Y()
            {
                return pos_y;
            }

            public string getPicturePos()
            {
                return picturePos;
            }


        }


        abstract class WorkPlace : Building
        {
            protected int workers;
            protected int cost;
            protected int income;

            public WorkPlace(string picturePos, int pos_x, int pos_y, string type, int size, int workers, int cost, int income)
                : base(picturePos ,pos_x, pos_y, type, size)
            {
                this.workers = workers;
                this.cost = cost;
                this.income = income;
            }
        }

        class Factory : WorkPlace
        {
            protected int income;
            protected int cost;
            public Factory(int pos_x, int pos_y, int size, int workers, int cost, int income)
                : base("", pos_x, pos_y, "Factory", size, workers, cost, income) { 
                
                this.income = income;
                this.cost = cost; 
            }
        }

        class House : Building
        {
            protected int hapiness;

            public House(int pos_x, int pos_y, int size, int hapiness)
                : base("pictures/House.jpeg", pos_x, pos_y, "House", size)
            {
                this.hapiness = hapiness;
            }
        }
    }
}