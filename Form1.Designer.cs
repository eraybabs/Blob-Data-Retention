namespace Blob
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btn_Add = new Button();
            treeView1 = new TreeView();
            SuspendLayout();
            // 
            // btn_Add
            // 
            btn_Add.Location = new Point(316, 65);
            btn_Add.Name = "btn_Add";
            btn_Add.Size = new Size(94, 29);
            btn_Add.TabIndex = 0;
            btn_Add.Text = "Blob Ekle";
            btn_Add.UseVisualStyleBackColor = true;
            btn_Add.Click += btn_Add_Click;
            // 
            // treeView1
            // 
            treeView1.Location = new Point(287, 199);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(151, 121);
            treeView1.TabIndex = 1;
            treeView1.NodeMouseDoubleClick += treeView1_NodeMouseDoubleClick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(treeView1);
            Controls.Add(btn_Add);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private Button btn_Add;
        private TreeView treeView1;
    }
}
