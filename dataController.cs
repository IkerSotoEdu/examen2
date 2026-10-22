using System;
using System.Windows.Forms;

namespace examen2;

public class InterfazController
{

    public void registrarTaller(Interfaz interfaz)
    {
        interfaz.btnRegistrar.Click += (sender, e) =>
        {


            if (string.IsNullOrWhiteSpace(interfaz.txtResponsable.Text))
            {
                MessageBox.Show("Por favor ingresa un responsable.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string responsable = interfaz.txtResponsable.Text;
                string nombreTaller = interfaz.cmbTaller.Text;
                string turno = interfaz.rdoMatutino.Checked ? "Matutino" : "Vespertino";
                bool requiereMaterial = interfaz.chkRequiereMaterial.Checked;
                var taller = interfaz.Talleres.FirstOrDefault(t => t.Nombre == nombreTaller);
                int lugaresSolicitados = (int)interfaz.nudLugares.Value;

                if (taller is null)
                {
                    MessageBox.Show("Selecciona un taller válido.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int lugaresOcupados = ObtenerLugaresOcupados(interfaz, nombreTaller);
                int lugaresDisponibles = taller.CupoMaximo - lugaresOcupados;
                if (lugaresSolicitados > lugaresDisponibles)
                {
                    MessageBox.Show($"Solo quedan {lugaresDisponibles} lugares disponibles para este taller.", "Cupo insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                interfaz.dgvReservas.Rows.Add(responsable, nombreTaller, lugaresSolicitados, turno, requiereMaterial ? "Sí" : "No");

                ActualizarContadores(interfaz);

                MessageBox.Show($"Taller registrado:\nResponsable: {responsable}\nNombre del Taller: {nombreTaller}\nTurno: {turno}\nRequiere Material: {(requiereMaterial ? "Sí" : "No")}", "Registro Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        };
    }

    public void ActualizarContadores(Interfaz interfaz)
    {
        int totalReservas = interfaz.dgvReservas.Rows.Count;
        interfaz.lblReservasRegistradas.Text = $"Reservas registradas: {totalReservas}";
        int lugaresOcupadosTotales = ObtenerLugaresOcupados(interfaz);
        interfaz.lblLugaresOcupados.Text = $"Lugares ocupados: {lugaresOcupadosTotales}";

        var tallerSeleccionado = interfaz.Talleres.FirstOrDefault(t => t.Nombre == interfaz.cmbTaller.SelectedItem?.ToString());
        if (tallerSeleccionado is null)
        {
            interfaz.lblLugaresDisponibles.Text = "Lugares disponibles: —";
            return;
        }

        int lugaresOcupadosTaller = ObtenerLugaresOcupados(interfaz, tallerSeleccionado.Nombre);
        interfaz.lblLugaresDisponibles.Text = $"Lugares disponibles: {tallerSeleccionado.CupoMaximo - lugaresOcupadosTaller}";
    }

    public void actualizarDisponibilidad(Interfaz interfaz)
    {
        interfaz.cmbTaller.SelectedIndexChanged += (sender, e) => ActualizarContadores(interfaz);
        ActualizarContadores(interfaz);
    }

    private static int ObtenerLugaresOcupados(Interfaz interfaz, string? nombreTaller = null)
    {
        return interfaz.dgvReservas.Rows
            .Cast<DataGridViewRow>()
            .Where(row => nombreTaller is null || string.Equals(
                Convert.ToString(row.Cells["Taller"].Value),
                nombreTaller,
                StringComparison.Ordinal))
            .Sum(row => Convert.ToInt32(row.Cells["Lugares"].Value));
    }


    public void limpiarCampos(Interfaz interfaz)
    {
        interfaz.btnLimpiar.Click += (sender, e) =>
        {
            interfaz.txtResponsable.Clear();
            interfaz.cmbTaller.SelectedIndex = 1;
            interfaz.rdoMatutino.Checked = true;
            interfaz.rdoVespertino.Checked = false;
            interfaz.chkRequiereMaterial.Checked = false;
        };
    }


    public void cancelarReserva(Interfaz interfaz)
    {
        interfaz.btnCancelar.Click += (sender, e) =>
        {
            MessageBox.Show("Estas seguro que quieres cancelar la reserva?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (MessageBox.Show("Estas seguro que quieres cancelar la reserva?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (interfaz.dgvReservas.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in interfaz.dgvReservas.SelectedRows)
                    {
                        interfaz.dgvReservas.Rows.Remove(row);
                    }
                    ActualizarContadores(interfaz);
                }
                else
                {
                    MessageBox.Show("Por favor selecciona una reserva para cancelar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        };
    }
}