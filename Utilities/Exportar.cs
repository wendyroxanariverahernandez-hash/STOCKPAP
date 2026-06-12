using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace STOCKPAP.Utilities
{
    public static class Exportar
    {
        public static void GuardarArchivo(string nombre, string filtro, string extension, string contenido)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Guardar archivo";
                sfd.Filter = filtro;
                sfd.FileName = $"{nombre}_{DateTime.Now:yyyyMMdd_HHmm}{extension}";
                
                string ruta = ConfigHelper.Obtener("RutaDescargas", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                if (Directory.Exists(ruta)) sfd.InitialDirectory = ruta;

                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    File.WriteAllText(sfd.FileName, contenido, Encoding.UTF8);
                    MessageBox.Show("Archivo generado correctamente.", "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static void GuardarArchivoPdf(string nombre, string contenido)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Guardar reporte PDF";
                sfd.Filter = "Archivo PDF|*.pdf";
                sfd.FileName = $"{nombre}_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
                
                string ruta = ConfigHelper.Obtener("RutaDescargas", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                if (Directory.Exists(ruta)) sfd.InitialDirectory = ruta;

                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create))
                    {
                        Document doc = new Document(PageSize.A4, 40, 40, 40, 40);
                        PdfWriter.GetInstance(doc, fs);
                        doc.Open();

                        iTextSharp.text.Font fontTitle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
                        iTextSharp.text.Font fontBody = FontFactory.GetFont(FontFactory.COURIER, 9);

                        string[] lineas = contenido.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                        foreach (var linea in lineas)
                        {
                            if (linea.Contains("REPORTE") || linea.Contains("TICKET GLOBAL"))
                            {
                                Paragraph p = new Paragraph(linea, fontTitle);
                                p.Alignment = Element.ALIGN_CENTER;
                                p.SpacingAfter = 10;
                                doc.Add(p);
                            }
                            else
                            {
                                doc.Add(new Paragraph(linea, fontBody));
                            }
                        }

                        doc.Close();
                    } // Aqui se cierra y libera el FileStream

                    MessageBox.Show("Archivo PDF generado correctamente.", "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}