using PluginInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Images
{
    public partial class PictureForm : Form
    {
        public PictureForm()
        {
            InitializeComponent();
            FindPlugins();
            CreatePluginsMenu();
        }

        void FindPlugins()
        {
            // папка с плагинами
            string folder = System.AppDomain.CurrentDomain.BaseDirectory;

            // dll-файлы в этой папке
            string[] files = Directory.GetFiles(folder, "*.dll");

            FileStream file1 = new FileStream("config.ini", FileMode.OpenOrCreate);
            file1.Seek(0, SeekOrigin.Begin);
            StreamReader streamReader = new StreamReader(file1);
            String config_info = streamReader.ReadToEnd();
            file1.Seek(0, SeekOrigin.Begin);
            StreamWriter stream = new StreamWriter(file1);


            foreach (string file in files)
                try
                {
                    Assembly assembly = Assembly.LoadFile(file);

                    foreach (Type type in assembly.GetTypes())
                    {
                        Type iface = type.GetInterface("PluginInterface.IPlugin");
                        if (iface != null)
                        {
                            VersionAttribute versionAttribute = (VersionAttribute)type.GetCustomAttribute(typeof(VersionAttribute));
                            IPlugin plugin = (IPlugin)Activator.CreateInstance(type);
                            Console.WriteLine();
                            if (config_info.Contains(plugin.Name) || config_info == "")
                            {
                                versions[plugin] = versionAttribute.Major.ToString() + "." + versionAttribute.Minor.ToString();
                                plugins.Add(plugin.Name, plugin);
                                stream.Write("Author:" + plugin.Author + ';');
                                stream.WriteLine(" Name:" + plugin.Name);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки плагина\n" + ex.Message);
                }
            stream.Close();
            streamReader.Close();
            file1.Close();
        }

        private void OnPluginClick(object sender, EventArgs args)
        {
            object plugin = plugins[((ToolStripMenuItem)sender).Text];

            var t = plugin.GetType();
            var method = t.GetMethod("Transform");
            pictureBox.Image = (Bitmap)method?.Invoke(plugin, new object[] { (Bitmap)pictureBox.Image });
            pictureBox.Refresh();
        }
        Dictionary<string, IPlugin> plugins = new Dictionary<string, IPlugin>();
        Dictionary<IPlugin, string> versions = new Dictionary<IPlugin, string>();

        void CreatePluginsMenu()
        {
            foreach (IPlugin p in plugins.Values)
            {
                var menuItem = new ToolStripMenuItem(p.Name);
                menuItem.Click += OnPluginClick;

                плагиныToolStripMenuItem.DropDownItems.Add(menuItem);
            }

        }

        private void pluginsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = "";
            foreach (IPlugin p in plugins.Values)
            {
                message += "Author:" + p.Author + ' ' + " Name:" + p.Name;
                message += " Version: " + versions[p] + '\n';
            }
            MessageBox.Show(message);
        }

        private void PictureForm_Load(object sender, EventArgs e)
        {

        }
    }
}
