using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace Etut_Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SqlConnection baglanti = new SqlConnection(@"Data Source=MACHINEX\MSSQLSERVER01;Initial Catalog=EtutTest;Integrated Security=True");

        void DersListesi()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM TBLDersler", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            cmbDers.ValueMember = "DersID";
            cmbDersAdi.ValueMember = "DersID";
            cmbDers.DisplayMember = "DersAd";
            cmbDersAdi.DisplayMember = "DersAd";
            cmbDers.DataSource = dt;
            cmbDersAdi.DataSource = dt;
        }

        void EtutListesi()
        {
            SqlDataAdapter da = new SqlDataAdapter("EXECUTE sp_Etut", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            int etutSayisi = dt.Rows.Count;
            for (int i = 0; i < etutSayisi; i++)
            {
                if (Convert.ToBoolean(dataGridView1.Rows[i].Cells[5].Value) == true)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Green;
                }
            }
        }

        void OgretmenListesi()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT OgrtID,(Ad+ ' ' +Soyad) AS Ogretmen FROM TBLOgretmen WHERE BransID = @BransID", baglanti);
            da.SelectCommand.Parameters.AddWithValue("@BransID", cmbDers.SelectedValue);
            DataTable dt = new DataTable();
            da.Fill(dt);
            cmbOgretmen.ValueMember = "OgrtID";
            cmbOgretmen.DisplayMember = "Ogretmen";
            cmbOgretmen.DataSource = dt;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DersListesi();
            EtutListesi();
        }

        private void cmbDers_SelectedIndexChanged(object sender, EventArgs e)
        {
            OgretmenListesi();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("INSERT INTO TBLEtut (DersID,OgrtID,Tarih,Saat) VALUES(@p1,@p2,@p3,@p4)", baglanti);
            komut.Parameters.AddWithValue("@p1", cmbDers.SelectedValue);
            komut.Parameters.AddWithValue("@p2", cmbOgretmen.SelectedValue);
            komut.Parameters.AddWithValue("@p3", mskTarih.Text);
            komut.Parameters.AddWithValue("@p4", mskSaat.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Etüt Oluşturuldu", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int secilen = dataGridView1.SelectedCells[0].RowIndex;
            txtEtutID.Text = dataGridView1.Rows[secilen].Cells[0].Value.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("UPDATE TBLEtut SET OgrID=@p1, Durum=@p2 WHERE ID=@p3", baglanti);
            komut.Parameters.AddWithValue("@p1", txtOgrenciID.Text);
            komut.Parameters.AddWithValue("@p2", true);
            komut.Parameters.AddWithValue("@p3", txtEtutID.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Etüt Öğrenciye Verildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            EtutListesi();
        }

        private void btnFotografEkle_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            pictureBox1.ImageLocation = openFileDialog1.FileName;
        }

        private void btnOgrenciEkle_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("INSERT INTO TBLOgrenci (Ad,Soyad,Fotograf,Sinif,Telefon,Mail) VALUES(@p1,@p2,@p3,@p4,@p5,@p6)", baglanti);
            komut.Parameters.AddWithValue("@p1", txtAd.Text);
            komut.Parameters.AddWithValue("@p2", txtSoyad.Text);
            komut.Parameters.AddWithValue("@p3", pictureBox1.ImageLocation);
            komut.Parameters.AddWithValue("@p4", txtSinif.Text);
            komut.Parameters.AddWithValue("@p5", mskTelefon.Text);
            komut.Parameters.AddWithValue("@p6", txtMail.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Öğrenci Sisteme Kaydedildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        bool DersKayiliMi(string dersAd)
        {
            foreach (DataRowView drv in cmbDers.Items)
            {
                if (drv.Row["DersAd"].ToString() == dersAd)
                {
                    return true;
                }
            }
            return false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!DersKayiliMi(txtDers.Text))
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("INSERT INTO TBLDersler (DersAd) VALUES(@p1)", baglanti);
                komut.Parameters.AddWithValue("@p1", txtDers.Text);
                komut.ExecuteNonQuery();
                baglanti.Close();
                MessageBox.Show("Ders Sisteme Kaydedildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DersListesi();
            }
            else
            {
                MessageBox.Show("Bu Ders Sistemde Bulunuyor", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnOgretmenEkle_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("INSERT INTO TBLOgretmen (Ad,Soyad,BransID) VALUES(@p1,@p2,@p3)", baglanti);
            komut.Parameters.AddWithValue("@p1", txtOgrtAd.Text);
            komut.Parameters.AddWithValue("@p2", txtOgrtSoyad.Text);
            komut.Parameters.AddWithValue("@p3", cmbDersAdi.SelectedValue);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Ders Sisteme Kaydedildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            OgretmenListesi();
        }
    }
}
