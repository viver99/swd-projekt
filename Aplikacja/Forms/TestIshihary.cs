﻿using Aplikacja.Forms;
using Aplikacja.Helpers;
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

namespace Aplikacja
{
    public partial class TestIshihary : Form
    {
        private List<Images> images;
        private int indexOfImage;
        private Images image;

        public TestIshihary()
        {
            InitializeComponent();
            images = new List<Images>();
            indexOfImage = 0;



            String sql = "SELECT * FROM \"" + Images.IMAGE_TABLE_NAME + "\" LIMIT 40";
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            MyDataBase myDataBase = new MyDataBase();
            myDataBase.open();
            myDataBase.clearWyniki();
            myDataBase.clearDiagnozy();

            System.Data.SQLite.SQLiteDataReader sqlReader = myDataBase.query(sql);
            while (sqlReader.Read())
            {
                Images image = new Images();
                image.Id = Convert.ToInt32(sqlReader[Images.IMAGE_ID]);
                image.Name = (String)sqlReader[Images.IMAGE_NAME].ToString();
                image.Type = (String)sqlReader[Images.IMAGE_TYPE].ToString();
                image.Value = (String)sqlReader[Images.IMAGE_VALUE].ToString();
                image.WrongValue = (String)sqlReader[Images.IMAGE_WRONG_VALUE].ToString();

                images.Add(image);
            }
            myDataBase.close();
            Shuffle.shuffle(images);
            setImage(images.First());
        }


        private void setImage(Images image) {

            try
            {
                pictureBox1.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\Images\" + image.Name);
                this.image = image;
                this.indexOfImage++;
            }
            catch
            {
                MessageBox.Show("Nie znaleziono obrazu!", "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            zapisz_wynik(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            zapisz_wynik(null);
        }

        private void zapisz_wynik(String wartosc) {
            Wynik wynik = new Wynik();
            // wynik.Id = 0; // jest AUTOINCREMENT
            wynik.IdTestu = image.Id;
            wynik.Type = image.Type;
            try
            {
               
                if (image.Id < 18)
                {
                    if (wartosc == null) {
                        wynik.WynikTestu = 1;
                    }
                    else if (wartosc == image.Value) {
                        wynik.WynikTestu = 2;
                    }
                    else {
                        wynik.WynikTestu = 3;
                    }
                }
                if (image.Id > 18 && image.Id < 22) {
                    if (wartosc == null) {
                        wynik.WynikTestu = 1;
                    }
                    else if(wartosc == image.WrongValue) {
                        wynik.WynikTestu = 2;
                    }
                    else {
                        wynik.WynikTestu = 0;
                    }
                }
               if(image.Id > 22) {
                    if (wartosc == null) {
                        wynik.WynikTestu = 1.0d;
                    }
                    else
                    {
                        wynik.WynikTestu = Convert.ToDouble(wartosc);
                    }
                    
                }
             
            }
            catch
            {
                MessageBox.Show("Proszę wprowadzić liczbę!", "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            wynik.Data = DateTime.Today.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();

            MyDataBase myDataBase = new MyDataBase();
            myDataBase.open();
            myDataBase.addWynik(wynik);
            myDataBase.close();

            if (indexOfImage < images.Count - 1)
            {
                setImage(images.ElementAtOrDefault(indexOfImage));
            }
            else
            {
                this.Close();

                TestFM testFM = new TestFM();
                testFM.Show();

                
            }

            if (this.Contains(textBox1))
            {
                textBox1.Text = null;
                ActiveControl = textBox1;
            }


        }
    }
}
