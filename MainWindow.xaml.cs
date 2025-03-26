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
            protected int poz_x;
            protected int poz_y;
            protected string type;
            protected int size;

            public Building(int poz_x, int poz_y, string type, int size)
            {
                this.poz_x = poz_x;
                this.poz_y = poz_y;
                this.type = type;
                this.size = size;
            }

            public string GetBuildingType() // Metódus neve más, hogy ne ütközzön Object.GetType()-tal
            {
                return this.type;
            }
        }


        abstract class WorkPlace : Building
        {
            protected int workers;
            protected int cost;
            protected int income;
            protected string picturePos;

            public WorkPlace(int poz_x, int poz_y, string type, int size, int workers, int cost, int income)
                : base(poz_x, poz_y, type, size)
            {
                this.workers = workers;
                this.cost = cost;
                this.income = income;
            }
        }

        class Factory : WorkPlace
        {
            public Factory(int poz_x, int poz_y, int size, string type, int workers, int cost, int income)
                : base(poz_x, poz_y, "Factory", size, workers, cost, income) { }
        }

        class House : Building
        {
            protected int hapiness;

            public House(int poz_x, int poz_y, int size, int hapiness)
                : base(poz_x, poz_y, "House", size)
            {
                this.hapiness = hapiness;
            }
        }


        House jaj = new House(1, 1, 1, 1);
        jaj.almaSama();

    }
}