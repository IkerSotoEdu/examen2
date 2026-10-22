using System.Drawing;
using System.Windows.Forms;

namespace examen2;

public sealed class Interfaz : Form
{
    private static readonly Color BackgroundColor = Color.FromArgb(15, 18, 27);
    private static readonly Color SurfaceColor = Color.FromArgb(25, 30, 42);
    private static readonly Color InputColor = Color.FromArgb(18, 23, 34);
    private static readonly Color BorderColor = Color.FromArgb(49, 58, 76);
    private static readonly Color TextColor = Color.FromArgb(232, 237, 246);
    private static readonly Color MutedTextColor = Color.FromArgb(151, 163, 183);
    private static readonly Color AccentColor = Color.FromArgb(75, 145, 255);

    public ComboBox cmbTaller = null!;
    public TextBox txtResponsable = null!;
    public NumericUpDown nudLugares = null!;
    public RadioButton rdoMatutino = null!;
    public RadioButton rdoVespertino = null!;
    public CheckBox chkRequiereMaterial = null!;
    public Button btnRegistrar = null!;
    public Button btnCancelar = null!;
    public Button btnLimpiar = null!;
    public DataGridView dgvReservas = null!;
    public Label lblLugaresDisponibles = null!;
    public Label lblReservasRegistradas = null!;
    public Label lblLugaresOcupados = null!;

    public Interfaz()
    {
        InitializeInterface();

        var controller = new InterfazController();
        controller.registrarTaller(this);
        controller.limpiarCampos(this);
        controller.actualizarDisponibilidad(this);
        controller.cancelarReserva(this);
    }


    public class Taller
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public int CupoMaximo { get; set; }
        public int LugaresOcupados { get; set; }
        public int LugaresDisponibles => CupoMaximo - LugaresOcupados;
    }

    private readonly Taller[] talleres = new Taller[]
    {
        new Taller { Id = 1, Nombre = "Programación C#", CupoMaximo = 20, LugaresOcupados = 0 },
        new Taller { Id = 2, Nombre = "Diseño digital", CupoMaximo = 15, LugaresOcupados = 0 },
        new Taller { Id = 3, Nombre = "Robótica", CupoMaximo = 10, LugaresOcupados = 0 }
    };

    public IReadOnlyList<Taller> Talleres => talleres;

    private void InitializeInterface()
    {
        Text = "Sistema de reservas de talleres";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(900, 650);
        Size = new Size(1180, 780);
        BackColor = BackgroundColor;
        ForeColor = TextColor;
        Font = new Font("Segoe UI", 10F);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = BackgroundColor,
            ColumnCount = 1,
            RowCount = 5,
            Padding = new Padding(22, 18, 22, 14)
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 184));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        Controls.Add(layout);

        layout.Controls.Add(CreateHeader(), 0, 0);
        layout.Controls.Add(CreateReservationCards(), 0, 1);
        layout.Controls.Add(CreateActionBar(), 0, 2);
        layout.Controls.Add(CreateReservationsSection(), 0, 3);
        layout.Controls.Add(CreateSummaryBar(), 0, 4);
    }

    private Control CreateHeader()
    {
        var header = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = BackgroundColor,
            Padding = new Padding(2, 0, 0, 8)
        };

        var title = new Label
        {
            Text = "Reservas de talleres",
            AutoSize = true,
            Font = new Font("Segoe UI Semibold", 19F, FontStyle.Bold),
            ForeColor = TextColor,
            Location = new Point(0, 0)
        };
        var subtitle = new Label
        {
            Text = "Administra las inscripciones y disponibilidad de cada sesión.",
            AutoSize = true,
            Font = new Font("Segoe UI", 9.5F),
            ForeColor = MutedTextColor,
            Location = new Point(2, 42)
        };

        header.Controls.Add(title);
        header.Controls.Add(subtitle);
        return header;
    }

    private Control CreateReservationCards()
    {
        var cards = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            BackColor = BackgroundColor,
            Padding = new Padding(0, 4, 0, 4)
        };
        cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32));

        cards.Controls.Add(CreateWorkshopCard(), 0, 0);
        cards.Controls.Add(CreateResponsibleCard(), 1, 0);
        cards.Controls.Add(CreatePreferencesCard(), 2, 0);
        return cards;
    }

    private Control CreateWorkshopCard()
    {
        var card = CreateCard("01  Taller");
        var content = CreateCardContent();

        cmbTaller = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Dock = DockStyle.Top,
            BackColor = InputColor,
            ForeColor = TextColor,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F)
        };
        cmbTaller.Items.AddRange(new object[] { talleres[0].Nombre, talleres[1].Nombre, talleres[2].Nombre });
        cmbTaller.SelectedIndex = 0;

        lblLugaresDisponibles = new Label
        {
            Text = "Lugares disponibles: —",
            AutoSize = true,
            ForeColor = Color.FromArgb(94, 207, 177),
            Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
            Margin = new Padding(0, 18, 0, 0)
        };

        content.Controls.Add(CreateFieldLabel("Taller"));
        content.Controls.Add(cmbTaller);
        content.Controls.Add(lblLugaresDisponibles);
        card.Controls.Add(content, 0, 1);
        return card;
    }

    private Control CreateResponsibleCard()
    {
        var card = CreateCard("02  Datos de la reserva");
        var content = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = SurfaceColor,
            Padding = new Padding(2, 4, 2, 0)
        };
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        content.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        txtResponsable = new TextBox
        {
            Dock = DockStyle.Fill,
            BackColor = InputColor,
            ForeColor = TextColor,
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Segoe UI", 10F)
        };

        var quantityRow = new TableLayoutPanel
        {
            ColumnCount = 2,
            RowCount = 1,
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 10, 0, 0),
            BackColor = SurfaceColor
        };
        quantityRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
        quantityRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
        quantityRow.Controls.Add(new Label
        {
            Text = "Cantidad de lugares",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = MutedTextColor,
            BackColor = SurfaceColor,
            Font = new Font("Segoe UI", 9F)
        }, 0, 0);

        nudLugares = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 20,
            Value = 1,
            Dock = DockStyle.Right,
            Width = 90,
            BackColor = InputColor,
            ForeColor = TextColor,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(0, 7, 0, 0)
        };
        quantityRow.Controls.Add(nudLugares, 1, 0);

        content.Controls.Add(CreateFieldLabel("Responsable"), 0, 0);
        content.Controls.Add(txtResponsable, 0, 1);
        content.Controls.Add(quantityRow, 0, 2);
        card.Controls.Add(content, 0, 1);
        return card;
    }

    private Control CreatePreferencesCard()
    {
        var card = CreateCard("03  Preferencias");
        var content = CreateCardContent();

        var shiftLabel = CreateFieldLabel("Turno");
        var shiftRow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 34,
            WrapContents = false,
            BackColor = SurfaceColor,
            Margin = new Padding(0, 0, 0, 7)
        };
        rdoMatutino = CreateRadioButton("Matutino", true);
        rdoVespertino = CreateRadioButton("Vespertino", false);
        shiftRow.Controls.Add(rdoMatutino);
        shiftRow.Controls.Add(rdoVespertino);

        chkRequiereMaterial = new CheckBox
        {
            Text = "Requiere material",
            AutoSize = true,
            ForeColor = TextColor,
            BackColor = SurfaceColor,
            Margin = new Padding(0, 10, 0, 0)
        };

        content.Controls.Add(shiftLabel);
        content.Controls.Add(shiftRow);
        content.Controls.Add(chkRequiereMaterial);
        card.Controls.Add(content, 0, 1);
        return card;
    }

    private Control CreateActionBar()
    {
        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 10, 0, 4),
            BackColor = BackgroundColor
        };

        btnRegistrar = CreateButton("Registrar reserva", true, 164);
        btnCancelar = CreateButton("Cancelar seleccionada", false, 184);
        btnLimpiar = CreateButton("Limpiar captura", false, 160);
        actions.Controls.Add(btnRegistrar);
        actions.Controls.Add(btnCancelar);
        actions.Controls.Add(btnLimpiar);
        return actions;
    }

    private Control CreateReservationsSection()
    {
        var section = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = BackgroundColor,
            Padding = new Padding(0, 4, 0, 4)
        };
        section.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        section.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var sectionHeading = new Label
        {
            Text = "Reservas registradas",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold),
            ForeColor = TextColor
        };

        dgvReservas = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = SurfaceColor,
            BorderStyle = BorderStyle.FixedSingle,
            GridColor = BorderColor,
            EnableHeadersVisualStyles = false,
            ColumnHeadersHeight = 38,
            RowTemplate = { Height = 34 },
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ReadOnly = true,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false
        };
        dgvReservas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(34, 41, 56);
        dgvReservas.ColumnHeadersDefaultCellStyle.ForeColor = TextColor;
        dgvReservas.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        dgvReservas.DefaultCellStyle.BackColor = SurfaceColor;
        dgvReservas.DefaultCellStyle.ForeColor = TextColor;
        dgvReservas.DefaultCellStyle.SelectionBackColor = Color.FromArgb(43, 75, 119);
        dgvReservas.DefaultCellStyle.SelectionForeColor = Color.White;
        dgvReservas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(29, 35, 48);
        dgvReservas.Columns.Add("Responsable", "Responsable");
        dgvReservas.Columns.Add("Taller", "Taller");
        dgvReservas.Columns.Add("Lugares", "Lugares");
        dgvReservas.Columns.Add("Turno", "Turno");
        dgvReservas.Columns.Add("Material", "Material");

        section.Controls.Add(sectionHeading, 0, 0);
        section.Controls.Add(dgvReservas, 0, 1);
        return section;
    }

    private Control CreateSummaryBar()
    {
        var summary = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            BackColor = BackgroundColor,
            Padding = new Padding(0, 8, 0, 0)
        };
        summary.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        summary.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        summary.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));

        lblReservasRegistradas = CreateSummaryLabel("Reservas: —");
        lblLugaresOcupados = CreateSummaryLabel("Lugares ocupados: ");
        summary.Controls.Add(lblReservasRegistradas, 0, 0);
        summary.Controls.Add(lblLugaresOcupados, 1, 0);
        summary.Controls.Add(CreateSummaryLabel("Sistema de talleres"), 2, 0);
        return summary;
    }

    private static TableLayoutPanel CreateCard(string title)
    {
        var card = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = SurfaceColor,
            CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
            Margin = new Padding(0, 0, 12, 0),
            Padding = new Padding(12, 8, 12, 10)
        };
        card.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        card.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        card.Controls.Add(new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
            ForeColor = AccentColor
        }, 0, 0);
        return card;
    }

    private static FlowLayoutPanel CreateCardContent()
    {
        return new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = SurfaceColor,
            Padding = new Padding(2, 4, 2, 0)
        };
    }

    private static Label CreateFieldLabel(string text)
    {
        return new Label
        {
            Text = text,
            AutoSize = true,
            ForeColor = MutedTextColor,
            BackColor = SurfaceColor,
            Font = new Font("Segoe UI", 9F),
            Margin = new Padding(0, 0, 0, 5)
        };
    }

    private static RadioButton CreateRadioButton(string text, bool isChecked)
    {
        return new RadioButton
        {
            Text = text,
            AutoSize = true,
            Checked = isChecked,
            ForeColor = TextColor,
            BackColor = SurfaceColor,
            Margin = new Padding(0, 2, 22, 0)
        };
    }

    private static Button CreateButton(string text, bool isPrimary, int width)
    {
        return new Button
        {
            Text = text,
            Width = width,
            Height = 36,
            FlatStyle = FlatStyle.Flat,
            BackColor = isPrimary ? AccentColor : SurfaceColor,
            ForeColor = TextColor,
            Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
            Margin = new Padding(0, 0, 12, 0),
            Cursor = Cursors.Hand
        };
    }

    private static Label CreateSummaryLabel(string text)
    {
        return new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = MutedTextColor,
            Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold)
        };
    }
}
