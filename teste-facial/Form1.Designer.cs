namespace teste_facial
{
    partial class frmCapturaFoto
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
            pictureBox1 = new PictureBox();
            btnCapturarImagem = new Button();
            lblmensaem = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(31, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(703, 481);
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // btnCapturarImagem
            // 
            btnCapturarImagem.Location = new Point(31, 499);
            btnCapturarImagem.Name = "btnCapturarImagem";
            btnCapturarImagem.Size = new Size(703, 29);
            btnCapturarImagem.TabIndex = 4;
            btnCapturarImagem.Text = "Tirar Foto";
            btnCapturarImagem.UseVisualStyleBackColor = true;
            btnCapturarImagem.Click += btnCapturarImagem_Click;
            // 
            // lblmensaem
            // 
            lblmensaem.AutoSize = true;
            lblmensaem.Location = new Point(31, 564);
            lblmensaem.Name = "lblmensaem";
            lblmensaem.Size = new Size(90, 20);
            lblmensaem.TabIndex = 5;
            lblmensaem.Text = "lblmensaem";
            // 
            // frmCapturaFoto
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(764, 618);
            Controls.Add(lblmensaem);
            Controls.Add(btnCapturarImagem);
            Controls.Add(pictureBox1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "frmCapturaFoto";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Salvar foto";
            FormClosing += frmCapturaFoto_FormClosing;
            Load += frmCapturaFoto_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private PictureBox pictureBox1;
        private Button btnIniciarCamera;
        private Button btnCapturarImagem;
        private Label lblmensaem;
    }
}
