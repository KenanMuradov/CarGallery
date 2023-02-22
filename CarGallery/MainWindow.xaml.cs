using CarGallery.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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

namespace CarGallery;

public partial class MainWindow : Window
{
    public ObservableCollection<Car>? Cars { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        Cars = new();
    }
}
