using ProyectoTest.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ProyectoTest.Logica
{
    public class CompraLogica
    {
        private static CompraLogica _instancia = null;

        public CompraLogica()
        {

        }

        public static CompraLogica Instancia
        {
            get
            {
                if (_instancia == null)
                {
                    _instancia = new CompraLogica();
                }

                return _instancia;
            }
        }

        public bool Registrar(Compra oCompra)
        {

            bool respuesta = false;
            using (SqlConnection oConexion = new SqlConnection(Conexion.CN))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    foreach (DetalleCompra dc in oCompra.oDetalleCompra)
                    {
                        query.AppendLine("insert into detalle_compra(IdCompra,IdProducto,Cantidad,Total) values (¡idcompra!," + dc.IdProducto + "," + dc.Cantidad + "," + dc.Total + ")");
                    }

                    SqlCommand cmd = new SqlCommand("sp_registrarCompra", oConexion);
                    cmd.Parameters.AddWithValue("IdUsuario", oCompra.IdUsuario);
                    cmd.Parameters.AddWithValue("TotalProducto", oCompra.TotalProducto);
                    cmd.Parameters.AddWithValue("Total", oCompra.Total);
                    cmd.Parameters.AddWithValue("Dni", oCompra.Dni);
                    cmd.Parameters.AddWithValue("Nombres", oCompra.Nombres);
                    cmd.Parameters.AddWithValue("ApellidoP", oCompra.ApellidoP);
                    cmd.Parameters.AddWithValue("ApellidoM", oCompra.ApellidoM);
                    cmd.Parameters.AddWithValue("Telefono", oCompra.Telefono);
                    cmd.Parameters.AddWithValue("Direccion", oCompra.Direccion);
                    cmd.Parameters.AddWithValue("IdDistrito", oCompra.IdDistrito);
                    cmd.Parameters.AddWithValue("QueryDetalleCompra", query.ToString());
                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oConexion.Open();
                    cmd.ExecuteNonQuery();
                    respuesta = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);

                }
                catch (Exception ex)
                {
                    respuesta = false;
                }
            }
            return respuesta;
        }
        public List<Compra> ListarCompras()
        {
            List<Compra> lista = new List<Compra>();
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                using (SqlCommand cmd = new SqlCommand("sp_listarCompras", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Compra compra = new Compra()
                            {
                                IdCompra = reader.GetInt32(reader.GetOrdinal("IdCompra")),
                                Dni = reader.GetString(reader.GetOrdinal("Dni")),
                                Nombres = reader.GetString(reader.GetOrdinal("Nombres")),
                                ApellidoP = reader.GetString(reader.GetOrdinal("ApellidoP")),
                                ApellidoM = reader.GetString(reader.GetOrdinal("ApellidoM")),
                                Telefono = reader.GetString(reader.GetOrdinal("Telefono")),
                                Direccion = reader.GetString(reader.GetOrdinal("Direccion")),
                                Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                                // Asignar FechaTexto desde FechaCompra
                                FechaTexto = reader.IsDBNull(reader.GetOrdinal("FechaCompra"))
                                    ? null
                                    : reader.GetDateTime(reader.GetOrdinal("FechaCompra")).ToString("dd/MM/yyyy")
                            };
                            lista.Add(compra);
                        }
                    }
                }
            }
            return lista;
        }

    }
}
