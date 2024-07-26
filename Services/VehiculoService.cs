using Microsoft.Data.SqlClient;
using System;

public class VehiculoService
{
    private readonly string _connectionString = "Server=DESKTOP-BHO1B15;Database=ParqueaderoDB;Integrated Security=True;TrustServerCertificate=True;";

    public void RegistrarIngreso(string placa)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Vehiculos (Placa, FechaIngreso) VALUES (@Placa, @FechaIngreso)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Placa", placa);
                command.Parameters.AddWithValue("@FechaIngreso", DateTime.Now);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al registrar el ingreso: {ex.Message}");
        }
    }

    public string RegistrarSalida(string placa)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Actualizar la fecha de salida
                string updateQuery = "UPDATE Vehiculos SET FechaSalida = @FechaSalida WHERE Placa = @Placa AND FechaSalida IS NULL";
                SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                updateCommand.Parameters.AddWithValue("@FechaSalida", DateTime.Now);
                updateCommand.Parameters.AddWithValue("@Placa", placa);

                connection.Open();
                int rowsAffected = updateCommand.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    // Obtener la información del vehículo
                    string selectQuery = "SELECT FechaIngreso, FechaSalida FROM Vehiculos WHERE Placa = @Placa AND FechaSalida IS NOT NULL";
                    SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                    selectCommand.Parameters.AddWithValue("@Placa", placa);

                    SqlDataReader reader = selectCommand.ExecuteReader();
                    if (reader.Read())
                    {
                        DateTime fechaIngreso = reader.GetDateTime(0);
                        DateTime fechaSalida = reader.GetDateTime(1);
                        reader.Close();

                        // Calcular el cobro por minutos
                        double minutos = (fechaSalida - fechaIngreso).TotalMinutes;
                        double tarifaPorHora = 2500; // Tarifa por hora en pesos
                        double totalCobro;

                        if (minutos <= 30)
                        {
                            totalCobro = tarifaPorHora / 2;
                        }
                        else if (minutos <= 60)
                        {
                            totalCobro = tarifaPorHora;
                        }
                        else if (minutos <= 90)
                        {
                            totalCobro = tarifaPorHora * 1.5;
                        }
                        else
                        {
                            int horasCompletas = (int)(minutos / 60);
                            double minutosRestantes = minutos % 60;

                            totalCobro = tarifaPorHora * horasCompletas;
                            if (minutosRestantes <= 30)
                            {
                                totalCobro += tarifaPorHora / 2;
                            }
                            else
                            {
                                totalCobro += tarifaPorHora;
                            }
                        }

                        return $"Vehículo con placa {placa} retirado. Minutos: {minutos:F2}. Total a pagar: {totalCobro:N0} COP";
                    }
                    else
                    {
                        reader.Close();
                        return "No se encontraron datos para el vehículo con esa placa.";
                    }
                }
                else
                {
                    return "No se encontró un vehículo con esa placa o ya fue retirado.";
                }
            }
        }
        catch (Exception ex)
        {
            return $"Error al registrar la salida: {ex.Message}";
        }
    }
}
