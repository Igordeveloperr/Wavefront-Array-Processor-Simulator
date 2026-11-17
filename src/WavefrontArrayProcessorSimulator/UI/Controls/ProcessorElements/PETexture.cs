using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaveVS.PE;

namespace WaveVS.VSGraphics
{
    // отрисовка текстуры ПЭ
    // Пользовательский контрол для отрисовки ПЭ
    internal class PETexture : UserControl
    {
        private const int PeSize = 70;
        private readonly Font font = new Font("Arial", 10);

        // Элементы для буферов
        private Panel leftBufferPanel;
        private Label leftBufferLabel;
        private Panel topBufferPanel;
        private Label topBufferLabel;
        private Panel mainPanel;
        private Label mainLabel;

        // Свойства для доступа к элементам (опционально)
        public Panel MainPanel => mainPanel;
        public Panel LeftBufferPanel => leftBufferPanel;
        public Panel TopBufferPanel => topBufferPanel;
        public int PESize => PeSize;

        // Ссылка на элемент процессора
        private ProcessorElement _processorElement;

        public PETexture()
        {
            InitializeComponents();
            Size = new Size(PeSize + 30, PeSize + 30); // Учитываем буферы
            this.Click += OnProcessorElementClick;
            // Подписываемся на клики дочерних панелей
            mainPanel.Click += OnProcessorElementClick;
            leftBufferPanel.Click += OnProcessorElementClick;
            topBufferPanel.Click += OnProcessorElementClick;

            // Также подписываем лейблы (если они могут перехватывать клики)
            mainLabel.Click += OnProcessorElementClick;
            leftBufferLabel.Click += OnProcessorElementClick;
            topBufferLabel.Click += OnProcessorElementClick;
        }

        private void InitializeComponents()
        {
            // Основная панель ПЭ
            mainPanel = new Panel
            {
                Size = new Size(PeSize, PeSize),
                Location = new Point(30, 30),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            mainLabel = new Label
            {
                Text = "ПЭ",
                Font = font,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            mainPanel.Controls.Add(mainLabel);

            // Левый буфер
            leftBufferPanel = new Panel
            {
                Size = new Size(30, PeSize),
                Location = new Point(0, 30),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            leftBufferLabel = new Label
            {
                Text = "",
                Font = font,
                ForeColor = Color.Red,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            leftBufferPanel.Controls.Add(leftBufferLabel);

            // Верхний буфер
            topBufferPanel = new Panel
            {
                Size = new Size(PeSize, 30),
                Location = new Point(30, 0),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            topBufferLabel = new Label
            {
                Text = "",
                Font = font,
                ForeColor = Color.Red,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            topBufferPanel.Controls.Add(topBufferLabel);

            // Добавляем все элементы на контрол
            Controls.Add(mainPanel);
            Controls.Add(leftBufferPanel);
            Controls.Add(topBufferPanel);

            // Настройки самого контрола
            BackColor = Color.Transparent;
            DoubleBuffered = true;
        }

        public void UpdatePE(ProcessorElement processorElement)
        {
            _processorElement = processorElement;

            // Обновляем основной текст
            mainLabel.Text = $"ПЭ{processorElement.Id.I}{processorElement.Id.J}\nACC: {processorElement.ResultRegister.Value}\n{processorElement.State}";

            // Обновляем левый буфер
            leftBufferLabel.Text = processorElement.LeftInputBuffer.Value == null ?
                "" : $"{processorElement.LeftInputBuffer.Value}";

            // Обновляем верхний буфер
            topBufferLabel.Text = processorElement.TopInputBuffer.Value == null ?
                "" : $"{processorElement.TopInputBuffer.Value}";
        }

        // обновление буферов ПЭ
        public void UpdateBufferPE(float? leftValue, float? topValue)
        {
            // Обновляем левый буфер
            leftBufferLabel.Text = leftValue == null ?
                "" : $"{leftValue}";

            // Обновляем верхний буфер
            topBufferLabel.Text = topValue == null ?
                "" : $"{topValue}";
        }

        private void OnProcessorElementClick(object sender, EventArgs e)
        {
            Console.WriteLine($"{_processorElement.Id.I}{_processorElement.Id.J}");
        }
    }
}
