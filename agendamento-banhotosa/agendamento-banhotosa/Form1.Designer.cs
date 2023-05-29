namespace agendamento_banhotosa
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            txtTelefone = new MaskedTextBox();
            label4 = new Label();
            label3 = new Label();
            DateTimePicker = new DateTimePicker();
            label2 = new Label();
            label1 = new Label();
            titulo = new Label();
            pictureBox1 = new PictureBox();
            comboBox1 = new ComboBox();
            label5 = new Label();
            label6 = new Label();
            checkBox1 = new CheckBox();
            checkBox2 = new CheckBox();
            consultar = new Button();
            checkedListBox1 = new CheckedListBox();
            agendar = new Button();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // txtTelefone
            // 
            txtTelefone.Location = new Point(194, 179);
            txtTelefone.Mask = "(99) 00000-0000";
            txtTelefone.Name = "txtTelefone";
            txtTelefone.Size = new Size(140, 26);
            txtTelefone.TabIndex = 26;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Times New Roman", 12F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point);
            label4.Location = new Point(576, 168);
            label4.Name = "label4";
            label4.Size = new Size(180, 19);
            label4.TabIndex = 25;
            label4.Text = "Seg. a Sab. das 9h às 17h.";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Times New Roman", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(145, 86);
            label3.Name = "label3";
            label3.Size = new Size(364, 15);
            label3.TabIndex = 24;
            label3.Text = "Efetuar o cadastro caso não tenha antes de realizar o agendamento.";
            // 
            // DateTimePicker
            // 
            DateTimePicker.ImeMode = ImeMode.NoControl;
            DateTimePicker.Location = new Point(194, 359);
            DateTimePicker.MinDate = new DateTime(2023, 5, 26, 0, 0, 0, 0);
            DateTimePicker.Name = "DateTimePicker";
            DateTimePicker.Size = new Size(140, 26);
            DateTimePicker.TabIndex = 23;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(100, 299);
            label2.Name = "label2";
            label2.Size = new Size(76, 22);
            label2.TabIndex = 22;
            label2.Text = "Serviço:";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(92, 179);
            label1.Name = "label1";
            label1.Size = new Size(84, 22);
            label1.TabIndex = 21;
            label1.Text = "Telefone:";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // titulo
            // 
            titulo.AutoSize = true;
            titulo.Font = new Font("Cascadia Code", 27.75F, FontStyle.Bold, GraphicsUnit.Point);
            titulo.Location = new Point(194, 22);
            titulo.Name = "titulo";
            titulo.Size = new Size(264, 49);
            titulo.TabIndex = 20;
            titulo.Text = "AGENDAMENTO";
            titulo.TextAlign = ContentAlignment.TopCenter;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(561, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(195, 149);
            pictureBox1.TabIndex = 27;
            pictureBox1.TabStop = false;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(194, 239);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(140, 27);
            comboBox1.TabIndex = 29;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label5.Location = new Point(124, 239);
            label5.Name = "label5";
            label5.Size = new Size(52, 22);
            label5.TabIndex = 30;
            label5.Text = "PET:";
            label5.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label6.Location = new Point(96, 359);
            label6.Name = "label6";
            label6.Size = new Size(80, 22);
            label6.TabIndex = 31;
            label6.Text = "Horario:";
            label6.TextAlign = ContentAlignment.MiddleRight;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(194, 299);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(71, 23);
            checkBox1.TabIndex = 32;
            checkBox1.Text = "Banho";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(271, 299);
            checkBox2.Name = "checkBox2";
            checkBox2.RightToLeft = RightToLeft.Yes;
            checkBox2.Size = new Size(60, 23);
            checkBox2.TabIndex = 33;
            checkBox2.Text = "Tosa";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // consultar
            // 
            consultar.Font = new Font("Times New Roman", 9F, FontStyle.Bold, GraphicsUnit.Point);
            consultar.Location = new Point(357, 359);
            consultar.Name = "consultar";
            consultar.Size = new Size(80, 26);
            consultar.TabIndex = 34;
            consultar.Text = "Consultar";
            consultar.UseVisualStyleBackColor = true;
            consultar.Click += button1_Click;
            // 
            // checkedListBox1
            // 
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Location = new Point(561, 211);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(195, 151);
            checkedListBox1.TabIndex = 35;
            // 
            // agendar
            // 
            agendar.Location = new Point(600, 390);
            agendar.Name = "agendar";
            agendar.Size = new Size(120, 37);
            agendar.TabIndex = 36;
            agendar.Text = "AGENDAR";
            agendar.UseVisualStyleBackColor = true;
            agendar.Click += agendar_Click;
            // 
            // button1
            // 
            button1.Location = new Point(12, 16);
            button1.Name = "button1";
            button1.Size = new Size(141, 55);
            button1.TabIndex = 37;
            button1.Text = "Fazer Cadastro";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            button2.Font = new Font("Times New Roman", 9F, FontStyle.Bold, GraphicsUnit.Point);
            button2.Location = new Point(12, 462);
            button2.Name = "button2";
            button2.RightToLeft = RightToLeft.Yes;
            button2.Size = new Size(140, 40);
            button2.TabIndex = 38;
            button2.Text = "Atualiza Cadastro";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            button3.Font = new Font("Times New Roman", 9F, FontStyle.Bold, GraphicsUnit.Point);
            button3.Location = new Point(632, 459);
            button3.Name = "button3";
            button3.RightToLeft = RightToLeft.Yes;
            button3.Size = new Size(140, 40);
            button3.TabIndex = 39;
            button3.Text = "Cancelar Agendamento";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(153, 199, 183);
            ClientSize = new Size(784, 511);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(agendar);
            Controls.Add(checkedListBox1);
            Controls.Add(consultar);
            Controls.Add(checkBox2);
            Controls.Add(checkBox1);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(comboBox1);
            Controls.Add(pictureBox1);
            Controls.Add(txtTelefone);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(DateTimePicker);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(titulo);
            Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point);
            Margin = new Padding(4);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MaskedTextBox txtTelefone;
        private Label label4;
        private Label label3;
        private DateTimePicker DateTimePicker;
        private Label label2;
        private Label label1;
        private Label titulo;
        private PictureBox pictureBox1;
        private ComboBox comboBox1;
        private Label label5;
        private Label label6;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private Button consultar;
        private CheckedListBox checkedListBox1;
        private Button agendar;
        private Button button1;
        private Button button2;
        private Button button3;
    }
}