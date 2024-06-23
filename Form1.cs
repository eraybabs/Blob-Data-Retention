/* Blob: Blob, "Binary Large Object" kelimelerinin baþ harflerinden oluþan bir kýsaltmadýr ve genellikle
 
veritabanlarýnda büyük miktarda veri saklamak için kullanýlan bir veri türünü ifade eder. Blob, metin dýþýndaki

verileri temsil eder ve genellikle resimler, ses dosyalarý, videolar gibi ikili veri türlerini kapsar.

Blob, veri tabanlarýnda özel bir veri türü olarak kullanýlýr. Bu, veritabanlarýna ikili veri akýþlarýný

(binary data streams) doðrudan saklama yeteneði saðlar. Blob, bir dosyanýn içeriðini veya dosyayý temsil eden bir 

referansý (örneðin, dosyanýn diskteki yolunu) içerebilir.

Blob verileri, metinsel olmayan verileri saklamak için kullanýlýrken, veritabaný sistemlerinde tipik olarak 

CLOB (Character Large Object) veya NCLOB (National Character Large Object) gibi metinsel verileri saklamak için 

kullanýlan özel veri türlerinden farklýdýr. Blob, özellikle dosya veya medya türündeki büyük verileri veritabaný 

içinde saklamak için yaygýn olarak kullanýlýr.

Özetle, Blob, veritabanlarýnda büyük ikili veri kümelerini saklamak için kullanýlan bir veri türüdür. */

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
using System.Data.SqlClient;

namespace Blob
{
    public partial class Form1 : Form
    {
        // Veritabaný baðlantýsý için SqlConnection nesnesi oluþturma

        SqlConnection con = new SqlConnection("Data Source=DESKTOP-G7KG1E8;Initial catalog=BLOB;Integrated Security=True");

        public Form1()
        {
            InitializeComponent();

            // Form baþlatýldýðýnda aðaç görünümünü doldurmak için PopulateTree() metodu çaðýrma

            PopulateTree();
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            // Kullanýcý dosya seçmek için dosya açma iletiþim kutusunu açmak için SelectAndSaveBlob() metodu çaðýrma

            SelectAndSaveBlob();
        }

        private void SelectAndSaveBlob()
        {
            // Dosya seçme iletiþim kutusu oluþturuluyor ve kullanýcýya dosya seçtirme

            OpenFileDialog fd = new OpenFileDialog();

            fd.ShowDialog();

            // Seçilen dosyanýn yolunu alma

            string filePath = fd.FileName;

            // Dosya adýný ve uzantýsýný alma

            string fileName = Path.GetFileName(filePath.Remove(filePath.Length - 4));

            string fileExtension = Path.GetExtension(filePath);

            // Dosyanýn içeriðini byte dizisine okuma

            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            byte[] dataToBlob = new byte[fs.Length];

            fs.Read(dataToBlob, 0, Convert.ToInt32(fs.Length));

            fs.Close();

            // Veritabanýna dosya bilgilerini ve içeriðini kaydetmek için SqlCommand nesnesi oluþturma

            SqlCommand cmd = new SqlCommand("Insert Into BlobStore (FileName, FileExtension, BlobData) Values (@name, @ext, @data)", con);

            cmd.Parameters.AddWithValue("@name", fileName);

            cmd.Parameters.AddWithValue("@ext", fileExtension);

            cmd.Parameters.AddWithValue("@data", dataToBlob);

            // Veritabaný baðlantýsý açýlýyor ve komut çalýþtýrma

            con.Open();

            cmd.ExecuteNonQuery();

            con.Close();

        }

        private void PopulateTree()

        {

            // Aðaç görünümündeki düðümleri temizleyerek baþlangýç yapma

            treeView1.Nodes.Clear();

            // Veritabanýndan tüm kayýtlarý seçmek için SqlDataAdapter ve DataSet kullanma

            SqlDataAdapter da = new SqlDataAdapter("Select * From BlobStore", con);

            SqlCommandBuilder cb = new SqlCommandBuilder(da);

            DataSet ds = new DataSet();

            da.Fill(ds);

            // DataSet içindeki her satýr için aðaç görünümüne düðüm ekleme

            foreach (DataRow row in ds.Tables[0].Rows)

            {

                treeView1.Nodes.Add(row["FileName"].ToString() + row["FileExtension"].ToString());

            }

        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)

        {

            // Çift týklanan düðümün verilerini almak için SqlCommand oluþturma

            con.Open();

            SqlCommand cmd = new SqlCommand("Select * From BlobStore Where FileName = @FileName", con);

            SqlParameter param = new SqlParameter();

            param.ParameterName = "@FileName";

            // Seçilen dosyanýn adýný almak için düðümün metnini bölme

            param.Value = treeView1.SelectedNode.Text.ToString().Split('.').First();

            cmd.Parameters.Add(param);

            // SqlCommand ile veri seti doldurma

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();

            da.Fill(ds, "BlobStore");

            // Eðer veri setinde kayýt varsa, blob verisini dosyaya yazma

            if (ds.Tables["BlobStore"].Rows.Count > 0)

            {

                Byte[] byteBlobData = new Byte[0];

                byteBlobData = (Byte[])(ds.Tables["BlobStore"].Rows[0]["BlobData"]);

                MemoryStream stmBlobData = new MemoryStream(byteBlobData);

                using (FileStream file =

                        new FileStream(

                            ds.Tables["BlobStore"].Rows[0]["FileName"].ToString() +

                            ds.Tables["BlobStore"].Rows[0]["FileExtension"].ToString(), FileMode.Create,

                            FileAccess.Write))
                {

                    file.Write(byteBlobData, 0, byteBlobData.Length);

                }

            }

            // Veritabaný baðlantýsý kapatma

            con.Close();

        }

    }

}