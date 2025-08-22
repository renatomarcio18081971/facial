namespace teste_facial
{
    public partial class frmPrincipal : Form
    {
        public frmPrincipal()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmCapturaFoto captura = new frmCapturaFoto();
            captura.ShowDialog();
        }
    }
}
