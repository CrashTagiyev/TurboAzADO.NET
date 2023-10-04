using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TurboAzADO.NET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>



    public class CarElani
    {

        public CarElani(string? marka, string? model, string? color, int? year, int? price, string? description)
        {
            Marka = marka;
            Model = model;
            Color = color;
            Year = year;
            Price = price;
            Description = description;
        }

        public string? Marka { get; set; }
        public string? Model { get; set; }
        public string? Color { get; set; }
        public int? Year { get; set; }
        public int? Price { get; set; }
        public string? Description { get; set; }


    }



    public partial class MainWindow : Window
    {
        public ObservableCollection<CarMarkaClass> CarMarkas { get; set; }
        public ObservableCollection<string> CarModelsCollection { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<CarElani> Elanlar { get; set; } = new ObservableCollection<CarElani>();

  
        SqlConnection? conn = null;
        public MainWindow()
        {

            try
            {
         
                CarMarkas = new ObservableCollection<CarMarkaClass>();
                FillCardMarkaModels();
                for (int i = 0; i < CarMarkas[1].models.Count; i++)
                {
                    CarModelsCollection.Add(CarMarkas[1].models[i].ToString());
                }
                string connectionString = @"Data Source=DESKTOP-HFMQLBA\MSSQLSERVER01;Initial Catalog=TurboAz;Integrated Security=True;Connect Timeout=30;Encrypt=False;";

                conn = new SqlConnection(connectionString);
                InitializeComponent();

                DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
          insertNewCar(Marka_TBox.SelectedItem.ToString(), Model_TBox.SelectedItem.ToString(), Color_TBox.Text, Convert.ToInt32(Year_TBox.Text), Description_TBox.Text, Convert.ToInt32(Price_TBox.Text));
        }

        private void AddButton_Click_1(object sender, RoutedEventArgs e)
        {

        }

        public void FillCardMarkaModels()
        {
            string jsonData = ReadEmbeddedJsonFile("cars.json");
            List<CarMarkaClass> carsList = JsonSerializer.Deserialize<List<CarMarkaClass>>(jsonData);

            foreach (var car in carsList)
            {
                CarMarkas.Add(car);
            }


        }
        public string ReadEmbeddedJsonFile(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(str => str.EndsWith(fileName));

            if (resourceName == null)
            {
                return null; // Handle the case where the resource is not found
            }

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        public void insertNewCar(string Marka, string Model, string color, int year, string description, int price)
        {
            try
            {
                conn.Open();
                string insertData =
                    $"EXEC AddElan @Marka = '{Marka}', @Model = '{Model}', @Color = '{color}', @Year = {year}, @Description = '{description}', @Price = {price}";
                using SqlCommand cmd = new SqlCommand(insertData, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        public void selectDatabase()
        {
            SqlDataReader reader = null;
            try
            {
                conn.Open();
                using SqlCommand command = new SqlCommand("SELECT * FROM Elanlar", conn);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    //Console.WriteLine(reader[0] + " " + reader[1] + " " + reader[2]);
                    Elanlar.Add(new CarElani(reader["Marka"].ToString(), reader["Model"].ToString(), reader["Color"].ToString(), Convert.ToInt32(reader["Year"].ToString()),Convert.ToInt32( reader["Price"].ToString()),reader["Description"].ToString()));
                    ////Console.WriteLine(reader["Id"] + " " + reader["FirstName"] + " " + reader["LastName"]);

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                reader.Close();
                conn.Close();
            }
        }



        public class CarMarkaClass
        {
            public string? name { get; set; }
            public ObservableCollection<CarModels>? models { get; set; }

            public override string ToString()
            {
                return name;
            }
        }
        public class CarModels
        {
            public string? name { get; set; }
            public override string ToString()
            {
                return name;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            selectDatabase();
        }
    }
}
