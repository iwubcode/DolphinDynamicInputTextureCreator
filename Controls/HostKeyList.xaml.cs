using System;
using System.Collections.Generic;
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

namespace DolphinDynamicInputTextureCreator.Controls
{
    /// <summary>
    /// Interaction logic for HostKeyList.xaml
    /// </summary>
    public partial class HostKeyList : UserControl
    {
        private Data.HostDevice HostDevice
        {
            get
            {
                return (Data.HostDevice)DataContext;
            }
        }

        public HostKeyList()
        {
            InitializeComponent();
        }

        private void AddHostKey_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".png";
            dlg.Filter = "PNG Files (*.png)|*.png";
            dlg.Multiselect = true;

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                foreach (var filename in dlg.FileNames)
                {
                    HostDevice.HostKeys.Add(new Data.HostKey
                    {
                        Name = "",
                        TexturePath = filename
                    });
                }
            }
        }

        /// <summary>
        /// catches dropped files and adds them to the HostKey list.
        /// </summary>
        private void AddHostKey_DragEnter(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string file in files)
            {
                if (System.IO.Path.GetExtension(file).ToLower() == ".png")
                {
                    HostDevice.HostKeys.Add(new Data.HostKey
                    {
                        Name = "",
                        TexturePath = file
                    });
                }
            }
        }
    }
}
