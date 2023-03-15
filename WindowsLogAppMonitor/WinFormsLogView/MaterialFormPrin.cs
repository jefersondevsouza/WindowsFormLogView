using MaterialSkin;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsLogView.Properties;

namespace WinFormsLogView
{
    //https://www.macoratti.net/17/07/cshp_matlog1.htm
    //https://www.youtube.com/watch?v=lvL4bDZccJU
    //https://www.nuget.org/packages/MaterialSkin.2/
    public partial class MaterialFormPrin : MaterialSkin.Controls.MaterialForm
    {
        private long position = 0;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private Panel panel1;
        private MaterialSkin.Controls.MaterialTextBox textBoxFile;
        private MaterialSkin.Controls.MaterialButton btnProcurar;
        private MaterialSkin.Controls.MaterialButton btnIniciar;

        private MaterialSkin.Controls.MaterialMultiLineTextBox richTextBox1;
        string prov = "prov.txt";

        public MaterialFormPrin()
        {
            InitializeComponent();
            //this.textBox1.Text = @"C:\Users\jsilveira\Desktop\UaisoftAgendamentoService\Uaisoft.Agendamento.Service\bin\Debug\Log\UaisoftAgendamentoService_Log.txt";

            timer.Interval = 2000;
            timer.Enabled = false;
            timer.Tick += Timer_Tick;

            // Create a material theme manager and add the form to manage (this)
            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;

            //// Configure color schema
            //materialSkinManager.ColorScheme = new ColorScheme(
            //    Primary.BlueGrey800, Primary.BlueGrey900,
            //    Primary.BlueGrey500,  Accent.LightBlue200,
            //    TextShade.WHITE
            //);

            materialSkinManager.ColorScheme = new ColorScheme(
                Color.Black, //titulo e botoes
                Color.FromArgb(51,51,51),//barra do windows
                Color.Black, //não fez diferenca nessa tela
                Color.Orange,// //não fez diferenca nessa tela
                TextShade.WHITE // lucidez das letras no botão e no título
);

            this.richTextBox1.BackColor = Color.Black;
        }

        private void MaterialFormPrin_Load(object sender, EventArgs e)
        {
            Settings.Default.Reload();
            if (!string.IsNullOrWhiteSpace(Settings.Default.FileLocal))
            {
                this.textBoxFile.Text = Settings.Default.FileLocal;
            }
        }

        private void MaterialFormPrin_Shown(object sender, EventArgs e)
        {
            //this.richTextBox1.BackColor = textBoxFile.BackColor;
            //this.richTextBox1.ForeColor = textBoxFile.ForeColor;

        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBoxFile.Text = ofd.FileName;
                Settings.Default.FileLocal = textBoxFile.Text;
                Settings.Default.Save();
                IniciarLeitura();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IniciarLeitura();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //C:\Users\jsilveira\Desktop\UaisoftAgendamentoService\Uaisoft.Agendamento.Service\bin\Debug\Log\UaisoftAgendamentoService_Log.txt
            if (EstaModificado())
                LerArquivo();
        }

        private bool EstaModificado()
        {
            if (File.Exists(prov))
            {
                FileInfo finew = new FileInfo(textBoxFile.Text);
                FileInfo fi = new FileInfo(prov);

                if (finew.Length == fi.Length)
                {
                    return false;
                }
            }

            return true;
        }

        private void IniciarLeitura()
        {
            this.richTextBox1.Text = string.Empty;
            this.position = 0;
            if (File.Exists(prov))
                File.Delete(prov);
            LerArquivo();
            timer.Start();
        }

        private void LerArquivo()
        {
            try
            {
                if (File.Exists(textBoxFile.Text))
                {

                    File.Copy(textBoxFile.Text, prov, true);

                    using (FileStream fs = new FileStream(prov, FileMode.Open, FileAccess.Read))
                    {
                        if (position > 0)
                        {
                            fs.Position = position;
                        }

                        using (StreamReader sr = new StreamReader(fs))
                        {
                            string s = sr.ReadToEnd();
                            if (s.Length > 0)
                            {
                                position = fs.Position;
                                SetarTexto(s);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show(this, "Arquivo não encontrado!");
                    timer.Stop();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Erro ao Ler Arquivo!", ex.Message);
            }
        }

        private void SetarTexto(string texto)
        {
            if (this.richTextBox1.InvokeRequired)
            {
                Action safeWrite = delegate { SetarTexto(texto); };
                richTextBox1.Invoke(safeWrite);
            }
            else
            {
                richTextBox1.Text += texto;
                richTextBox1.Focus();
                this.richTextBox1.SelectionStart = this.richTextBox1.Text.Length - 1;
            }

        }

        //private void btnMinimizar_Click(object sender, EventArgs e)
        //{
        //    this.WindowState = FormWindowState.Minimized;
        //}

        //Size? FormWindowStateBeforeMaximized = null;
        //private void btnMaximizar_Click(object sender, EventArgs e)
        //{
        //    if (this.WindowState == FormWindowState.Maximized)
        //    {
        //        this.WindowState = FormWindowState.Normal;
        //        if (FormWindowStateBeforeMaximized.HasValue)
        //        {
        //            this.Size = this.FormWindowStateBeforeMaximized.Value;
        //        }
        //    }
        //    else
        //    {
        //        this.FormWindowStateBeforeMaximized = this.Size;
        //        this.WindowState = FormWindowState.Maximized;
        //    }
        //}

        //private void btnFechar_Click(object sender, EventArgs e)
        //{
        //    this.Close();
        //}

        //private void panel2_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (MouseButtons.Left == e.Button)
        //    {
        //        if (this.Tag != null)
        //        {
        //            Point p = (Point)this.Tag;

        //            int x = e.Location.X - p.X;
        //            int y = e.Location.Y - p.Y;

        //           // this.Tag = new Point(x, y);

        //            x = this.Location.X + x;
        //            y = this.Location.Y + y;

        //            //this.label1.Text = $"x={x}, y={y}";
        //            this.Location = new Point(x, y);
        //        }
        //        else
        //        {
        //            this.Tag = e.Location;
        //        }
        //    }
        //}

        //private void panel2_MouseUp(object sender, MouseEventArgs e)
        //{
        //    this.Tag = null;
        //}

        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBoxFile = new MaterialSkin.Controls.MaterialTextBox();
            this.btnProcurar = new MaterialSkin.Controls.MaterialButton();
            this.btnIniciar = new MaterialSkin.Controls.MaterialButton();
            this.richTextBox1 = new MaterialSkin.Controls.MaterialMultiLineTextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.textBoxFile);
            this.panel1.Controls.Add(this.btnProcurar);
            this.panel1.Controls.Add(this.btnIniciar);
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1162, 48);
            this.panel1.TabIndex = 5;
            // 
            // textBoxFile
            // 
            this.textBoxFile.AnimateReadOnly = false;
            this.textBoxFile.BackColor = System.Drawing.Color.Black;
            this.textBoxFile.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxFile.Depth = 0;
            this.textBoxFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxFile.Enabled = false;
            this.textBoxFile.Font = new System.Drawing.Font("Roboto", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.textBoxFile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.textBoxFile.LeadingIcon = null;
            this.textBoxFile.Location = new System.Drawing.Point(0, 0);
            this.textBoxFile.MaxLength = 50;
            this.textBoxFile.MouseState = MaterialSkin.MouseState.OUT;
            this.textBoxFile.Multiline = false;
            this.textBoxFile.Name = "textBoxFile";
            this.textBoxFile.Size = new System.Drawing.Size(1025, 50);
            this.textBoxFile.TabIndex = 0;
            this.textBoxFile.TabStop = false;
            this.textBoxFile.Text = "";
            this.textBoxFile.TrailingIcon = null;
            // 
            // btnProcurar
            // 
            this.btnProcurar.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnProcurar.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnProcurar.Depth = 0;
            this.btnProcurar.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnProcurar.HighEmphasis = true;
            this.btnProcurar.Icon = null;
            this.btnProcurar.Location = new System.Drawing.Point(1025, 0);
            this.btnProcurar.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnProcurar.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnProcurar.Name = "btnProcurar";
            this.btnProcurar.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnProcurar.Size = new System.Drawing.Size(64, 48);
            this.btnProcurar.TabIndex = 6;
            this.btnProcurar.Text = "...";
            this.btnProcurar.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnProcurar.UseAccentColor = false;
            this.btnProcurar.UseVisualStyleBackColor = true;
            this.btnProcurar.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // btnIniciar
            // 
            this.btnIniciar.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnIniciar.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnIniciar.Depth = 0;
            this.btnIniciar.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnIniciar.HighEmphasis = true;
            this.btnIniciar.Icon = null;
            this.btnIniciar.Location = new System.Drawing.Point(1089, 0);
            this.btnIniciar.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnIniciar.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnIniciar.Name = "btnIniciar";
            this.btnIniciar.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnIniciar.Size = new System.Drawing.Size(73, 48);
            this.btnIniciar.TabIndex = 8;
            this.btnIniciar.Text = "Iniciar";
            this.btnIniciar.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnIniciar.UseAccentColor = false;
            this.btnIniciar.UseVisualStyleBackColor = true;
            this.btnIniciar.Click += new System.EventHandler(this.button2_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Depth = 0;
            this.richTextBox1.Font = new System.Drawing.Font("Cascadia Mono", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.richTextBox1.Location = new System.Drawing.Point(6, 118);
            this.richTextBox1.MouseState = MaterialSkin.MouseState.HOVER;
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(1151, 704);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            // 
            // MaterialFormPrin
            // 
            this.ClientSize = new System.Drawing.Size(1163, 828);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.richTextBox1);
            this.Name = "MaterialFormPrin";
            this.Text = "LOG VIEW";
            this.Load += new System.EventHandler(this.MaterialFormPrin_Load);
            this.Shown += new System.EventHandler(this.MaterialFormPrin_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

    }
}
