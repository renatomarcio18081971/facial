using AForge.Video;
using AForge.Video.DirectShow;
using DlibDotNet;
using System.Globalization;

namespace teste_facial
{
    public partial class frmCapturaFoto : Form
    {
        private FrontalFaceDetector detector;
        private FilterInfoCollection dispositivosVideo;
        private VideoCaptureDevice fonteVideo;
        private readonly FotosRepository _foto;
        private Dictionary<string, Matrix<float>> fotoSalva = [];
        private ShapePredictor sp;
        private DlibDotNet.Dnn.LossMetric facerec;
        private Guid idAssinatura;
        private Dictionary<string, Matrix<float>> listaDeFotos = [];
        private Guid idArquivo;

        public frmCapturaFoto()
        {
            InitializeComponent();
            _foto = new FotosRepository();
        }

        private void Iniciar()
        {
            detector = Dlib.GetFrontalFaceDetector();
            sp = ShapePredictor.Deserialize("shape_predictor_5_face_landmarks.dat");
            facerec = DlibDotNet.Dnn.LossMetric.Deserialize("dlib_face_recognition_resnet_model_v1.dat");
        }

        private void FinalizarCamera()
        {
            if (File.Exists(string.Concat(idAssinatura.ToString(), ".jpg")))
            {
                File.Delete(string.Concat(idAssinatura.ToString(), ".jpg"));
            }
            detector.Dispose();
            facerec.Dispose();
            sp.Dispose();
            IniciarCamera();
        }

        private float CompareDescriptors(Matrix<float> d1, Matrix<float> d2)
        {
            float sum = 0;
            for (int i = 0; i < d1.Size; i++)
                sum += (d1[i] - d2[i]) * (d1[i] - d2[i]);
            return (float)Math.Sqrt(sum);
        }

        private void IniciarCamera()
        {
            Iniciar();
            fotoSalva = [];
            dispositivosVideo = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (dispositivosVideo.Count == 0)
            {
                MessageBox.Show("Nenhuma câmera encontrada.");
                return;
            }
            fonteVideo = new VideoCaptureDevice(dispositivosVideo[0].MonikerString);
            fonteVideo.NewFrame += new NewFrameEventHandler(Video_NewFrame);
            fonteVideo.Start();
        }

        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap imagem = (Bitmap)eventArgs.Frame.Clone();

            using (Graphics g = Graphics.FromImage(imagem))
            {
                int larguraGuia = 300; // mais largo
                int alturaGuia = 390;  // mais baixo
                int x = (imagem.Width - larguraGuia) / 2;
                int y = (imagem.Height - alturaGuia) / 2;

                Pen guia = new Pen(Color.LimeGreen, 3);
                g.DrawEllipse(guia, x, y, larguraGuia, alturaGuia);
            }
            pictureBox1.Image?.Dispose();
            pictureBox1.Image = imagem;
        }

        private void btnCapturarImagem_Click(object sender, EventArgs e)
        {
            try
            {
                if (pictureBox1.Image != null)
                {
                    lblmensaem.Text = "Validando foto";
                    idAssinatura = Guid.NewGuid();
                    Bitmap imagemCapturada;
                    try
                    {
                        imagemCapturada = new Bitmap(pictureBox1.Image);
                    }
                    catch (Exception)
                    {
                        lblmensaem.Text = string.Empty;
                        FinalizarCamera();
                        MessageBox.Show("Centralize o rosto dentro da figura oval.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        throw;
                    }
                    imagemCapturada.Save(string.Concat(idAssinatura.ToString(), ".jpg"), System.Drawing.Imaging.ImageFormat.Jpeg);
                    if (fonteVideo != null && fonteVideo.IsRunning)
                    {
                        fonteVideo.SignalToStop();
                        fonteVideo.WaitForStop();
                    }
                    string imagePath = string.Concat(idAssinatura.ToString(), ".jpg");
                    using var img = Dlib.LoadImage<RgbPixel>(imagePath);
                    var faces = detector.Operator(img);
                    if (faces.Length == 0)
                    {
                        lblmensaem.Text = string.Empty;
                        FinalizarCamera();
                        MessageBox.Show("Nenhum rosto detectado.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    lblmensaem.Text = "Salvando foto";
                    var shape = sp.Detect(img, faces[0]);
                    var chipDetail = Dlib.GetFaceChipDetails(shape, 150, 0.25);
                    using var faceChip = Dlib.ExtractImageChip<RgbPixel>(img, chipDetail);
                    var matrix = new Matrix<RgbPixel>(faceChip);
                    var descriptors = facerec.Operator(matrix);
                    var faceDescriptor = descriptors[0];
                    var descriptorText = string.Join(",", faceDescriptor.ToArray().Select(f => f.ToString(CultureInfo.InvariantCulture)));
                    _foto.Salvar(imagePath, descriptorText, string.Concat(idAssinatura.ToString()));


                    //Carregar vetor de foto salva no banco
                    lblmensaem.Text = "Validando estrutura foto";
                    var pessoa = _foto.Obter(string.Concat(idAssinatura.ToString()));
                    var image = pessoa?.ImagePath;
                    var valores = pessoa?.Descriptor.Split(',').Select(s => float.Parse(s, CultureInfo.InvariantCulture)).ToArray();
                    var matriz = new Matrix<float>(128, 1);
                    if (valores == null) throw new Exception("Valores não foram localizados !");
                    for (int i = 0; i < valores.Length; i++)
                    {
                        matriz[i] = valores[i];
                    }
                    if (pessoa == null)
                    {
                        lblmensaem.Text = string.Empty;
                        FinalizarCamera();
                        MessageBox.Show("Erro ao carregar foto salva, verifique", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    fotoSalva.Add(pessoa.ImagePath, matriz);

                    //Compara foto salva no banco se nao comparar apaga o registro no banco                
                    using var imgComparacao = Dlib.LoadImage<RgbPixel>(string.Concat(idAssinatura.ToString(), ".jpg"));
                    var facesComparacao = detector.Operator(imgComparacao);
                    if (facesComparacao.Length == 0)
                    {
                        lblmensaem.Text = string.Empty;
                        FinalizarCamera();
                        MessageBox.Show("Nenhum rosto detectado.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _foto.ApagarFoto(idAssinatura.ToString());
                        return;
                    }
                    var shapeComparacao = sp.Detect(imgComparacao, facesComparacao[0]);
                    var chipDetailComparacao = Dlib.GetFaceChipDetails(shapeComparacao, 150, 0.25);
                    using var faceChipComparacao = Dlib.ExtractImageChip<RgbPixel>(imgComparacao, chipDetailComparacao);
                    var matrixComparacao = new Matrix<RgbPixel>(faceChipComparacao);
                    var descriptorsComparacao = facerec.Operator(matrixComparacao);
                    var novoVetor = descriptorsComparacao[0];
                    var vetoresSalvos = fotoSalva;
                    string melhorCorrespondencia = string.Empty;
                    float menorDistancia = float.MaxValue;
                    foreach (var (nome, vetorSalvo) in vetoresSalvos)
                    {
                        float distancia = CompareDescriptors(novoVetor, vetorSalvo);
                        if (distancia < menorDistancia)
                        {
                            menorDistancia = distancia;
                            melhorCorrespondencia = nome;
                        }
                    }
                    if (menorDistancia > 0.65f)
                    {
                        lblmensaem.Text = string.Empty;
                        FinalizarCamera();
                        MessageBox.Show("Falha na validação durante o processo, favor repetir", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _foto.ApagarFoto(idAssinatura.ToString());
                    }
                    FinalizarCamera();
                    lblmensaem.Text = "Processo finalizado";
                    MessageBox.Show("Processo finalizado !", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                FinalizarCamera();
            }
        }

        private void frmCapturaFoto_Load(object sender, EventArgs e)
        {
            IniciarCamera();
            lblmensaem.Text = string.Empty;
            lblmensaem.Width = this.ClientSize.Width;
        }

        private void frmCapturaFoto_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (fonteVideo != null && fonteVideo.IsRunning)
            {
                fonteVideo.SignalToStop();
                fonteVideo.WaitForStop();
            }
        }

        private void btnReconhecer_Click(object sender, EventArgs e)
        {
            try
            {
                if (pictureBox1.Image != null)
                {
                    idArquivo = Guid.NewGuid();
                    Bitmap imagemCapturada;
                    try
                    {
                        imagemCapturada = new Bitmap(pictureBox1.Image);
                    }
                    catch (Exception)
                    {
                        FinalizarCamera();
                        MessageBox.Show("Centralize o rosto dentro da figura oval.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        throw;
                    }
                    imagemCapturada.Save(string.Concat(idArquivo.ToString(), ".jpg"), System.Drawing.Imaging.ImageFormat.Jpeg);

                    var listaDePessoas = _foto.ObterTodos();
                    foreach (var pessoa in listaDePessoas)
                    {
                        var image = pessoa?.ImagePath;
                        var valores = pessoa?.Descriptor.Split(',').Select(s => float.Parse(s, CultureInfo.InvariantCulture)).ToArray();
                        var matriz = new Matrix<float>(128, 1);
                        if (valores == null) throw new Exception("Valores não foram localizados !");
                        for (int i = 0; i < valores.Length; i++)
                        {
                            matriz[i] = valores[i];
                        }
                        if (pessoa == null)
                        {
                            FinalizarCamera();
                            MessageBox.Show("Erro ao carregar foto salva, verifique", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        listaDeFotos.Add(pessoa.ImagePath, matriz);
                    }


                    using var img = Dlib.LoadImage<RgbPixel>(string.Concat(idArquivo.ToString(), ".jpg"));
                    var faces = detector.Operator(img);
                    if (faces.Length == 0)
                    {
                        MessageBox.Show("Nenhum rosto detectado.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    var shape = sp.Detect(img, faces[0]);
                    var chipDetail = Dlib.GetFaceChipDetails(shape, 150, 0.25);
                    using var faceChip = Dlib.ExtractImageChip<RgbPixel>(img, chipDetail);
                    var matrix = new Matrix<RgbPixel>(faceChip);
                    var descriptors = facerec.Operator(matrix);
                    var novoVetor = descriptors[0];
                    var vetoresSalvos = listaDeFotos;
                    string melhorCorrespondencia = string.Empty;
                    float menorDistancia = float.MaxValue;
                    foreach (var (nome, vetorSalvo) in vetoresSalvos)
                    {
                        float distancia = CompareDescriptors(novoVetor, vetorSalvo);
                        if (distancia < menorDistancia)
                        {
                            menorDistancia = distancia;
                            melhorCorrespondencia = nome;
                        }
                    }
                    if (menorDistancia < 0.65f)
                        MessageBox.Show($"Rosto reconhecido: {melhorCorrespondencia} (distância: {menorDistancia:F4})");
                    else
                        MessageBox.Show("Rosto não reconhecido.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    listaDeFotos = [];
                    FinalizarCamera();
                }
            }
            finally
            {
                listaDeFotos = [];
                FinalizarCamera();
            }
        }
    }
}