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
    /// Interaction logic for TexturePicker.xaml
    /// </summary>
    public partial class TexturePicker : UserControl
    {
        private Data.DynamicInputPack InputPack
        {
            get
            {
                return (Data.DynamicInputPack)DataContext;
            }
        }

        public TexturePicker()
        {
            InitializeComponent();
        }

        private void AddTexture_Click(object sender, RoutedEventArgs e)
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
                    InputPack.Textures.Add(new Data.DynamicInputTexture
                    {
                        TextureHash = InputPack.ShouldGetHashFromTextureFilename ? System.IO.Path.GetFileName(filename) : "hash value",
                        TexturePath = filename
                    });
                }
            }
        }

        /// <summary>
        /// catches dropped files and adds them to the texture list.
        /// </summary>
        private void AddTexture_DragEnter(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string file in files)
            {
                if (System.IO.Path.GetExtension(file).ToLower() == ".png")
                {
                    InputPack.Textures.Add(new Data.DynamicInputTexture
                    {
                        TextureHash = InputPack.ShouldGetHashFromTextureFilename ? System.IO.Path.GetFileName(file) : "hash value",
                        TexturePath = file
                    });
                }
            }
        }
    }
}
