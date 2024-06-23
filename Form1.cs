/* Blob: Blob, "Binary Large Object" kelimelerinin ba� harflerinden olu�an bir k�saltmad�r ve genellikle
 
veritabanlar�nda b�y�k miktarda veri saklamak i�in kullan�lan bir veri t�r�n� ifade eder. Blob, metin d���ndaki

verileri temsil eder ve genellikle resimler, ses dosyalar�, videolar gibi ikili veri t�rlerini kapsar.

Blob, veri tabanlar�nda �zel bir veri t�r� olarak kullan�l�r. Bu, veritabanlar�na ikili veri ak��lar�n�

(binary data streams) do�rudan saklama yetene�i sa�lar. Blob, bir dosyan�n i�eri�ini veya dosyay� temsil eden bir 

referans� (�rne�in, dosyan�n diskteki yolunu) i�erebilir.

Blob verileri, metinsel olmayan verileri saklamak i�in kullan�l�rken, veritaban� sistemlerinde tipik olarak 

CLOB (Character Large Object) veya NCLOB (National Character Large Object) gibi metinsel verileri saklamak i�in 

kullan�lan �zel veri t�rlerinden farkl�d�r. Blob, �zellikle dosya veya medya t�r�ndeki b�y�k verileri veritaban� 

i�inde saklamak i�in yayg�n olarak kullan�l�r.

�zetle, Blob, veritabanlar�nda b�y�k ikili veri k�melerini saklamak i�in kullan�lan bir veri t�r�d�r. */

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
        // Veritaban� ba�lant�s� i�in SqlConnection nesnesi olu�turma

        SqlConnection con = new SqlConnection("Data Source=DESKTOP-G7KG1E8;Initial catalog=BLOB;Integrated Security=True");

        public Form1()
        {
            InitializeComponent();

            // Form ba�lat�ld���nda a�a� g�r�n�m�n� doldurmak i�in PopulateTree() metodu �a��rma

            PopulateTree();
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            // Kullan�c� dosya se�mek i�in dosya a�ma ileti�im kutusunu a�mak i�in SelectAndSaveBlob() metodu �a��rma

            SelectAndSaveBlob();
        }

        private void SelectAndSaveBlob()
        {
            // Dosya se�me ileti�im kutusu olu�turuluyor ve kullan�c�ya dosya se�tirme

            OpenFileDialog fd = new OpenFileDialog();

            fd.ShowDialog();

            // Se�ilen dosyan�n yolunu alma

            string filePath = fd.FileName;

            // Dosya ad�n� ve uzant�s�n� alma

            string fileName = Path.GetFileName(filePath.Remove(filePath.Length - 4));

            string fileExtension = Path.GetExtension(filePath);

            // Dosyan�n i�eri�ini byte dizisine okuma

            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            byte[] dataToBlob = new byte[fs.Length];

            fs.Read(dataToBlob, 0, Convert.ToInt32(fs.Length));

            fs.Close();

            // Veritaban�na dosya bilgilerini ve i�eri�ini kaydetmek i�in SqlCommand nesnesi olu�turma

            SqlCommand cmd = new SqlCommand("Insert Into BlobStore (FileName, FileExtension, BlobData) Values (@name, @ext, @data)", con);

            cmd.Parameters.AddWithValue("@name", fileName);

            cmd.Parameters.AddWithValue("@ext", fileExtension);

            cmd.Parameters.AddWithValue("@data", dataToBlob);

            // Veritaban� ba�lant�s� a��l�yor ve komut �al��t�rma

            con.Open();

            cmd.ExecuteNonQuery();

            con.Close();

        }

        private void PopulateTree()

        {

            // A�a� g�r�n�m�ndeki d���mleri temizleyerek ba�lang�� yapma

            treeView1.Nodes.Clear();

            // Veritaban�ndan t�m kay�tlar� se�mek i�in SqlDataAdapter ve DataSet kullanma

            SqlDataAdapter da = new SqlDataAdapter("Select * From BlobStore", con);

            SqlCommandBuilder cb = new SqlCommandBuilder(da);

            DataSet ds = new DataSet();

            da.Fill(ds);

            // DataSet i�indeki her sat�r i�in a�a� g�r�n�m�ne d���m ekleme

            foreach (DataRow row in ds.Tables[0].Rows)

            {

                treeView1.Nodes.Add(row["FileName"].ToString() + row["FileExtension"].ToString());

            }

        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)

        {

            // �ift t�klanan d���m�n verilerini almak i�in SqlCommand olu�turma

            con.Open();

            SqlCommand cmd = new SqlCommand("Select * From BlobStore Where FileName = @FileName", con);

            SqlParameter param = new SqlParameter();

            param.ParameterName = "@FileName";

            // Se�ilen dosyan�n ad�n� almak i�in d���m�n metnini b�lme

            param.Value = treeView1.SelectedNode.Text.ToString().Split('.').First();

            cmd.Parameters.Add(param);

            // SqlCommand ile veri seti doldurma

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();

            da.Fill(ds, "BlobStore");

            // E�er veri setinde kay�t varsa, blob verisini dosyaya yazma

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

            // Veritaban� ba�lant�s� kapatma

            con.Close();

        }

    }

}