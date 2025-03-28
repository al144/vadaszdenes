using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace vadaszdenes
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            citygrid.MouseDown += CityGrid_MouseDown;

            for (int i = 0; i < 20; i++)
            {
                citygrid.ColumnDefinitions.Add(new ColumnDefinition());
                citygrid.RowDefinitions.Add(new RowDefinition());
            }

            ReadFromAccessDB();
            FillGridWithImage("/pictures/grass.png");

            // Adding buildings by concatenating all lists into one
            buildings.AddRange(houses.Cast<Building>().Concat(factories.Cast<Building>())
                                      .Concat(hospitals.Cast<Building>())
                                      .Concat(schools.Cast<Building>())
                                      .Concat(policeOffices.Cast<Building>()));

            // Adding residents by combining both adults and children
            residents.AddRange(adults.Cast<Resident>().Concat(children.Cast<Resident>()));


            AssignResidentsToHouses();

            PlaceBuildingsOnGrid();
        }

        // Collections for buildings and residents
        public static List<House> houses = new List<House>();
        public static List<Factory> factories = new List<Factory>();
        public static List<Hospital> hospitals = new List<Hospital>();
        public static List<School> schools = new List<School>();
        public static List<Police> policeOffices = new List<Police>();
        public static List<Building> buildings = new List<Building>();
        public static List<Resident> residents = new List<Resident>();
        public static List<Adult> adults = new List<Adult>();
        public static List<Child> children = new List<Child>();


        private int playedMonths = 0;
        private int column;
        private int row;
        private bool build_window_visibility = false;

        // Event handling for building placement
        private void CityGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPosition = e.GetPosition(citygrid);
            double cellWidth = citygrid.ActualWidth / citygrid.ColumnDefinitions.Count;
            double cellHeight = citygrid.ActualHeight / citygrid.RowDefinitions.Count;

            column = (int)(clickPosition.X / cellWidth);
            row = (int)(clickPosition.Y / cellHeight);
        }

        private void New_Project(object sender, RoutedEventArgs e)
        {
            FillGridWithImage("/pictures/add.png");
        }

        public string selectedBuildingPath;
        public Image selectedTile;

        private void SelectBuilding(string imgPath)
        {
            selectedBuildingPath = imgPath;
        }

        private void PlaceBuilding()
        {
            if (selectedTile != null && !string.IsNullOrEmpty(selectedBuildingPath))
            {
                selectedTile.Source = new BitmapImage(new Uri(selectedBuildingPath, UriKind.RelativeOrAbsolute));
                selectedTile.MouseLeftButtonDown += (s, e) => ChangeBuildWindow();
            }
        }

        private void Tile_Click(object sender, MouseButtonEventArgs e)
        {
            selectedTile = sender as Image;
            if (selectedTile != null && e.ClickCount == 2) // Double-click to place a building
            {
                PlaceBuilding();
            }
        }

        // Building window visibility control
        private void ChangeBuildWindow()
        {
            build_window_visibility = !build_window_visibility;
            New_Project_Button.Content = build_window_visibility ? "Close" : "New Project";
            Build_Window.Visibility = build_window_visibility ? Visibility.Visible : Visibility.Collapsed;
            if (build_window_visibility)
            {
                FillGridWithImage("/pictures/close.png");
            }
            else
            {
                FillGridWithImage("/pictures/add.png");
            }
        }

        private void FillGridWithImage(string imgPath)
        {
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    var existingImage = citygrid.Children.OfType<Image>().FirstOrDefault(img => Grid.GetColumn(img) == i && Grid.GetRow(img) == j);

                    if (existingImage != null)
                    {
                        UpdateImage(existingImage, imgPath);
                    }
                    else
                    {
                        AddNewImage(i, j, imgPath);
                    }
                }
            }
        }

        private void UpdateImage(Image existingImage, string imgPath)
        {
            if (existingImage.Source.ToString().Contains("grass.png") && !imgPath.Contains("grass.png"))
            {
                existingImage.Source = new BitmapImage(new Uri(imgPath, UriKind.RelativeOrAbsolute));
                existingImage.MouseLeftButtonDown += (s, e) => ChangeBuildWindow();
            }
            else if (!existingImage.Source.ToString().Contains("grass.png") && imgPath.Contains("grass.png"))
            {
                existingImage.Source = new BitmapImage(new Uri(imgPath, UriKind.RelativeOrAbsolute));
                existingImage.MouseLeftButtonDown -= (s, e) => ChangeBuildWindow();
            }
        }

        private void AddNewImage(int i, int j, string imgPath)
        {
            Image newImage = new Image
            {
                Source = new BitmapImage(new Uri(imgPath, UriKind.RelativeOrAbsolute)),
                Visibility = Visibility.Visible,
                Stretch = Stretch.Fill
            };

            if (!imgPath.Contains("grass.png"))
            {
                newImage.MouseLeftButtonDown += (s, e) => ChangeBuildWindow();
            }

            Grid.SetColumn(newImage, i);
            Grid.SetRow(newImage, j);
            citygrid.Children.Add(newImage);
        }

        // Assign residents to their corresponding houses
        private void AssignResidentsToHouses()
        {
            foreach (House house in houses)
            {
                foreach (Resident resident in residents)
                {
                    if (resident.getHouse_id() == house.getHouse_id())
                    {
                        house.Residentadd(resident);
                    }
                }
            }
        }

        private void PlaceBuildingsOnGrid()
        {
            foreach (Building building in buildings)
            {
                Image buildingImage = new Image
                {
                    Source = new BitmapImage(new Uri(building.getPicturePos(), UriKind.RelativeOrAbsolute)),
                    Visibility = Visibility.Visible,
                    Tag = building
                };

                buildingImage.MouseLeftButtonDown += Update_Stats;

                Grid.SetColumn(buildingImage, building.getPos_X());
                Grid.SetRow(buildingImage, building.getPos_Y());
                citygrid.Children.Add(buildingImage);
            }
        }

        // Display building stats on click
        private void Update_Stats(object sender, MouseButtonEventArgs e)
        {
            Stats.Visibility = Visibility.Visible;
            Stats.Content = "";

            if (sender is Image clickedImage && clickedImage.Tag is Building clickedBuilding)
            {
                if (clickedBuilding.getType() == "Factory")
                {
                    Stats.Content += $"Type: {clickedBuilding.getType()}\n" +
                                      $"Name: {clickedBuilding.getName()}\n" +
                                      $"Workers: {clickedBuilding.getWorkers()}\n" +
                                      $"Income: {clickedBuilding.getIncome()}\n" +
                                      $"Cost: {clickedBuilding.getCost()}\n";
                }
                else if (clickedBuilding.getType() == "House")
                {
                    Stats.Content += $"Name: {clickedBuilding.getName()}\nResidents:\n";
                    foreach (Resident resident in clickedBuilding.getResidents())
                    {
                        Stats.Content += $"Name: {resident.getName()}\nAge: {resident.getAge()}\n";
                    }
                }
            }
        }

        // Database interaction and loading data
        public void ReadFromAccessDB()
        {
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + System.IO.Path.GetFullPath("alomvaros.accdb") + ";Persist Security Info=False;";

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    LoadResidentsFromDatabase(conn);
                    LoadBuildingsFromDatabase(conn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void LoadResidentsFromDatabase(OleDbConnection conn)
        {
            string query = "SELECT * FROM Lakosok";
            using (OleDbDataReader reader = new OleDbCommand(query, conn).ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader["Foglalkozas"].Equals("tanuló") || reader["Foglalkozas"].Equals("nincs"))
                    {
                        children.Add(new Child((int)reader["LakosAzon"], (2025 - (int)reader["SzuletesiEv"]), (string)reader["Név"], (int)reader["LakhelyAzon"]));
                    }
                    else
                    {
                        adults.Add(new Adult((int)reader["LakosAzon"], (2025 - (int)reader["SzuletesiEv"]), (string)reader["Név"], (int)reader["LakhelyAzon"]));
                    }
                }
            }
        }

        private void LoadBuildingsFromDatabase(OleDbConnection conn)
        {
            string query = "SELECT * FROM Épületek";
            using (OleDbDataReader reader = new OleDbCommand(query, conn).ExecuteReader())
            {
                while (reader.Read())
                {
                    switch (reader["Típus"])
                    {
                        case "Kórház":
                            hospitals.Add(new Hospital((int)reader["ÉpületAzon"], (string)reader["Név"], (int)reader["Sor"], (int)reader["Oszlop"], (string)reader["EpitesEve"]));
                            break;
                        case "Iskola":
                            schools.Add(new School((int)reader["ÉpületAzon"], (string)reader["Név"], (int)reader["Sor"], (int)reader["Oszlop"], (string)reader["EpitesEve"]));
                            break;
                        case "Rendőrség":
                            policeOffices.Add(new Police((int)reader["ÉpületAzon"], (string)reader["Név"], (int)reader["Sor"], (int)reader["Oszlop"], (string)reader["EpitesEve"]));
                            break;
                        case "Gyár":
                            factories.Add(new Factory((int)reader["ÉpületAzon"], (string)reader["Név"], (int)reader["Sor"], (int)reader["Oszlop"], (string)reader["EpitesEve"], false));
                            break;
                        case "Erőmű":
                            factories.Add(new Factory((int)reader["ÉpületAzon"], (string)reader["Név"], (int)reader["Sor"], (int)reader["Oszlop"], (string)reader["EpitesEve"], true));
                            break;
                        case "Lakóház":
                            houses.Add(new House((int)reader["ÉpületAzon"], (string)reader["Név"], (int)reader["Sor"], (int)reader["Oszlop"], (int)reader["HasznosTerulet"], reader["EpitesEve"].ToString(), new List<Resident>())); break;
                    }
                }
            }
        }




        public class Resident
        {
            protected int resident_id;
            protected int age;
            protected string name;
            protected int house_id;
            protected string job;
            public Resident(int resident_id, int age, string name, int house_id, string job)
            {
                this.resident_id = resident_id;
                this.age = age;
                this.name = name;
                this.house_id = house_id;
                this.job = job;
            }

            public int getResident_id()
            {
                return resident_id;
            }

            public int getHouse_id()
            {
                return house_id;
            }

            public int getAge()
            {
                return age;
            }

            public string getName()
            {
                return name;
            }
        }


        public class Adult : Resident
        {
            public Adult(int resident_id, int age, string name, int house_id)
                : base(resident_id, age, name, house_id, " ")
            {

            }
        }

        public class Child : Resident
        {
            public Child(int resident_id, int age, string name, int house_id)
                : base(resident_id, age, name, house_id, (age < 6 ? "nincs" : "tanuló"))
            {

            }
        }

        public abstract class Building
        {
            protected string name;
            protected int pos_x;
            protected int pos_y;
            protected string built;
            protected string type;
            protected string picturePos;
            protected string ststus;
            protected int workers;
            protected int cost;
            protected int income;
            protected List<Resident> residents;

            public Building(string picturePos) { this.picturePos = picturePos; }
            public Building(string name, string picturePos, int pos_x, int pos_y, string type, string built)
            {
                this.picturePos = picturePos;
                this.pos_x = pos_x;
                this.pos_y = pos_y;
                this.type = type;
                this.name = name;
                this.built = built;
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

            public string getType()
            {
                return type;
            }

            public string getName()
            {
                return name;
            }

            public string getStatus()
            {
                return ststus;
            }

            public int getIncome()
            {
                return income;
            }

            public int getCost()
            {
                return cost;
            }

            public int getWorkers()
            {
                return workers;
            }
            public List<Resident> getResidents()
            {
                return residents;
            }

        }

        public class House : Building
        {
            protected int hapiness; // ????????????????????????????????????????????????
            protected int size;
            protected int house_id;

            public House() :base("/pictures/house.png")  { }
            public House(int house_id, string name, int pos_x, int pos_y, int size, string built, List<Resident> residents)
                : base(name, ( size < 2000 ? "/pictures/house.png" : "/pictures/apartmantblock.png"), pos_x, pos_y, "House", built)
            {
                this.size = size;
                this.house_id = house_id;
                this.residents = residents;
            }

            public void Residentadd(Resident r)
            {
                residents.Add(r);
            }

            public int getHouse_id()
            {
                return house_id;
            }
        }

        public abstract class Service : Building
        {
            protected int service_id;
            protected string name;

            public Service(string picturePos) : base(picturePos) { }
            public Service(int service_id,  string name, string picturePos, int pos_x, int pos_y, string type, string built)
                : base(name, picturePos, pos_x, pos_y, type, built)
            {
                this.workers = (int)Math.Floor((double)adults.Count() / (int)(hospitals.Count() + factories.Count() + schools.Count() + policeOffices.Count()));
                this.cost = workers * 50;
                this.income = workers * 100;
                this.service_id = service_id;
                this.name = name;
            }


        }

        public class Factory : Service
        {

            public Factory() :base("/pictures/factory.png") { }
            public Factory(int service_id, string name, int pos_x, int pos_y, string built, bool power)
                : base(service_id, name,(power ? "/pictures/powerplant.png" : "/pictures/factory.png"), pos_x, pos_y, "Factory", built) { }
        }

        public class Hospital : Service
        {
            protected int ambulance;

            public Hospital() :base("/pictures/hospital.png") { }
            public Hospital(int service_id, string name, int pos_x, int pos_y, string built)
                : base(service_id, name, "/pictures/hospital.png", pos_x, pos_y, "Hospital", built)
            {
                this.ambulance = (int)Math.Floor(workers / 15.0);

            }
        }

        public class School : Service
        {
            protected int students;

            public School() :base("/pictures/school.png") { }
            public School(int service_id, string name, int pos_x, int pos_y, string built)
                : base(service_id, name, "/pictures/school.png", pos_x, pos_y, "School", built)
            {
                this.students = (int)Math.Floor((double)children.Count() / schools.Count());
                income = 0;
            }
        }

        public class Police : Service
        {
            
            public Police() :base("/pictures/police.png") { }
            public Police(int service_id, string name, int pos_x, int pos_y, string built)
                : base(service_id, name, "/pictures/police.png", pos_x, pos_y, "Police", built)
            {
                income = 0;
            }
        }

    }
}