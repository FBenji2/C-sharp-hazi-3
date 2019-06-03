using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public partial class Form1 : Form
    {

        private string root;
        private FileInfo loadedFile = null;
        private int countdown = 100;


        public Form1()
        {
            InitializeComponent();
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            Close(); //kilépés
        }

        private void miOpen_Click(object sender, EventArgs e) //az "open gombra:"
        {
            InputDialog dlg = new InputDialog(); if (dlg.ShowDialog() == DialogResult.OK) //megnyit egy inputdialogot
            {
                string result = dlg.Path; //elkéri az elérési út eredményét
                MessageBox.Show("Választott mappa: " + result); //kiírja az elérési utat
                root = dlg.Path; //és egyenlővé is teszi
                if(root[root.Length-1] != '\\') //ezt hozzáraktam, mert mindig lehagytam a \-t az elejéről :D
                {
                    root = root + '\\';
                }
                DirectoryInfo dir = new DirectoryInfo(root);
                listView1.Items.Clear();
                foreach (FileInfo fi in dir.GetFiles())
                {
                    listView1.Items.Add(new ListViewItem(new string[] { fi.Name, fi.Length.ToString() }));
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e) //ha a bal oldali listviewen kattintunk másik fájlra akkor mi történik
        {
            if (listView1.SelectedItems.Count == 1) //ha egy elemet válaszottunk ki, akkor megszerezzük, hogy melyik az és kiírjuk jobb oldalon a kért adatokat
            {
                loadedFile = new FileInfo(root + listView1.SelectedItems[0].SubItems[0].Text);
                iText.Text = "Name: " +  loadedFile.Name + Environment.NewLine + "Created: " + loadedFile.CreationTime;
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e) //dublaklikknél
        {
            fileContent.Clear(); //tisztítjuk a jobb listviewot ahova a tartalmat írjuk ki
            if (listView1.SelectedItems.Count == 1)
            {
                fileContent.Text = File.ReadAllText(root + loadedFile.Name); //kiírjuk a kiválaszott fájl tartalmát
                reloadTimer.Start(); //elindítjuk az időzítőt
                countdown = 100; //reseteljük az időzítőt
                loadedFile = new FileInfo(root + listView1.SelectedItems[0].SubItems[0].Text); //ezt a feladat követelte meg, valójában fölösleges, mert a sima kattintásnál már alkalmaztam
            }
        }

        private void miAutoRefresh_Click(object sender, EventArgs e)
        {
            listView1_DoubleClick(sender,e); //itt meghívjuk a double click függvényét, mert gyakorlatilag azt kell csinálni ami benne van
        }

        private void reloadTimer_Tick(object sender, EventArgs e) //időzítőt figyeli és csökkenti a változót, amit visszarak 100-ra ha lejár és kiírja megint a fájlt
        {
            countdown--; detailsPanel.Invalidate();
            if (countdown <= 0)
            {
                countdown = 100;
                fileContent.Text = File.ReadAllText(loadedFile.FullName);
            }
        }

        private void detailsPanel_Paint(object sender, PaintEventArgs e) //beszínezi a panel tetején pirossal idő függvényében
        {
            if (loadedFile != null)
                e.Graphics.FillRectangle(Brushes.Red, 0, 0, countdown, 2);
        }
    }
}
