using ClosedXML.Excel;
using Gestion.DAL;
using Indexai.Models;
using QRCoder;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Barcode;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Z.EntityFramework.Plus;

namespace Indexai.Services
{
    public class PdfExportService
    {
        private int conteo;
        private List<Tuple<int, int>> exluirList = new List<Tuple<int, int>>();

        private DateTime esFecha(string txtFecha)
        {
            CultureInfo esES = new CultureInfo("es-ES");
            DateTime dateValue = DateTime.MinValue;
            DateTime.TryParseExact(txtFecha, "g", esES, DateTimeStyles.None, out dateValue);
            return dateValue;
        }

        public PdfPage Imagen2Pdf(string rutaSalida, string rutaImg, Image imgTMP, SizeF tamanio)
        {
            PdfDocument docpdf = new PdfDocument();
            float WidthPdf = imgTMP.Width;      //Imagen
            float HeightPdf = imgTMP.Height;    //Imagen
            SizeF tamanioPagina = tamanio;      //Página
            if (WidthPdf > HeightPdf)
            {
                docpdf.PageSettings.Orientation = PdfPageOrientation.Landscape;
                if (WidthPdf > tamanio.Height) WidthPdf = tamanio.Height - 1;
                if (HeightPdf > tamanio.Width) HeightPdf = tamanio.Width - 1;
                tamanioPagina.Width = tamanio.Height;
                tamanioPagina.Height = tamanio.Width;
            }
            else
            {
                docpdf.PageSettings.Orientation = PdfPageOrientation.Portrait;
                if (WidthPdf > tamanio.Width) WidthPdf = tamanio.Width-1;
                if (HeightPdf > tamanio.Height - 1) HeightPdf = tamanio.Height - 1;
            }

            docpdf.PageSettings.Size = PdfPageSize.A5;
            docpdf.PageSettings.Size = new SizeF(tamanioPagina.Width, tamanio.Height);
            //docpdf.PageSettings.Size = tamanioPagina;
            docpdf.PageSettings.Margins.Top = 0;
            docpdf.PageSettings.Margins.Right = 0;
            docpdf.PageSettings.Margins.Bottom = 0;
            docpdf.PageSettings.Margins.Left = 0;
            PdfPage page = docpdf.Pages.Add();//Add a page to the document.
            PdfGraphics graphics = page.Graphics;
            if (File.Exists(rutaImg))
            {
                PdfBitmap image = new PdfBitmap(rutaImg);
                SizeF pageSize = page.GetClientSize();  //Width = 595 Height = 421  Console.WriteLine(pageSize.Width + " - " + pageSize.Height);
                RectangleF imageBounds = new RectangleF(0,0, WidthPdf, HeightPdf);//Setting image bounds
                graphics.DrawImage(image, imageBounds);//Draw the image
                                                       
                docpdf.Save(rutaSalida);   //Save the document.
            }
            docpdf.Close(true);    //Close the document.
            return page;
            
        }

        public void ExportPdfRotuloCarpeta1(int idCarpeta, string exportPath)
        {
            string rc_titulo1 = "", rc_titulo2 = "", rc_titulo3 = "", rc_cal_codigo = "", rc_cal_version = "", rc_cal_fecha = "", txtQR = "", nomDependencia = "", codDependencia = "", nomSubdependencia = "", codSubdependencia = "", nomSerie = "", codSerie = "", codSubserie = "", nomSubserie = "", marco = "", nombres = "", nroExp = "", nomExp = "", nroCaja = ""; ;
            int idProyecto = 0, existeMarcoL = 0, folioFinal = 0;
            DateTime fechaInicial = DateTime.MaxValue, fechaFinal = DateTime.MinValue;

            //Create a new PDF document.
            PdfDocument doc = new PdfDocument();
            float factor = 3;
            doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
            doc.PageSettings.Size = PdfPageSize.A5;
            doc.PageSettings.Size = new SizeF(100 * factor, 100 * factor);
            doc.PageSettings.Margins.Top = 1;
            doc.PageSettings.Margins.Right = 1;
            doc.PageSettings.Margins.Bottom = 1;
            doc.PageSettings.Margins.Left = 1;

            PdfPage page = doc.Pages.Add();//Add a page to the document.
            PdfGraphics graphics = page.Graphics;
            PdfPen borde = new PdfPen(Color.Black, 1);
            PdfPen bordeDebug = new PdfPen(Color.Red, 1);
            RectangleF recMarco = new RectangleF(1, 1, (float)93 * factor, (float)92 * factor); //Borde X-6 Y-7
            page.Graphics.DrawRectangle(borde, recMarco);
            RectangleF recparte1 = new RectangleF(1, 1, recMarco.Width, (float)factor * 10); //Borde
            page.Graphics.DrawRectangle(borde, recparte1);

            //Consulta base de datos
            var datHC = EntitiesRepository.Entities.t_carpeta.Include("t_tercero").Include("t_lote").Include("p_proyecto").Include("p_formato").AsNoTracking()
                    .Where(f => f.id == idCarpeta)
                    .Select(p => new { p.t_lote.p_proyecto.p_formato, p.t_lote.t_carpeta, p.t_tercero, p.t_lote.p_subserie, p.t_lote.p_subdependencia, p.t_lote.p_proyecto, p.fecha_expediente_ini, p.fecha_expediente_fin, p.total_folios });

            var dataFormato = datHC.FirstOrDefault();
            if (dataFormato != null)
            {
                if (dataFormato.p_formato.Count > 0)
                {
                    if (dataFormato.p_formato.FirstOrDefault().rc_titulo1 != null) rc_titulo1 = dataFormato.p_formato.FirstOrDefault().rc_titulo1;
                    if (dataFormato.p_formato.FirstOrDefault().rc_titulo2 != null) rc_titulo2 = dataFormato.p_formato.FirstOrDefault().rc_titulo2;
                    if (dataFormato.p_formato.FirstOrDefault().rc_titulo3 != null) rc_titulo3 = dataFormato.p_formato.FirstOrDefault().rc_titulo3;
                    if (dataFormato.p_formato.FirstOrDefault().rc_cal_codigo != null) rc_cal_codigo = dataFormato.p_formato.FirstOrDefault().rc_cal_codigo;
                    if (dataFormato.p_formato.FirstOrDefault().rc_cal_version != null) rc_cal_version = dataFormato.p_formato.FirstOrDefault().rc_cal_version;
                    if (dataFormato.p_formato.FirstOrDefault().rc_cal_fecha != null) rc_cal_fecha = dataFormato.p_formato.FirstOrDefault().rc_cal_fecha;
                    int.TryParse(dataFormato.p_formato.FirstOrDefault().rc_marco_legal.ToString(), out existeMarcoL);
                    DateTime.TryParse(dataFormato.p_formato.FirstOrDefault().fecha_inicial_defecto.ToString(), out fechaInicial);//FECHAS EXTREMAS
                }

                string archivadoPor = dataFormato.p_proyecto.nom_proyecto;
                idProyecto = dataFormato.p_proyecto.id;
                nomDependencia = dataFormato.p_subdependencia.p_dependencia.nombre;
                codDependencia = dataFormato.p_subdependencia.p_dependencia.codigo;
                nomSubdependencia = dataFormato.p_subdependencia.nombre;
                if (dataFormato.p_subdependencia.cod != null) codSubdependencia = dataFormato.p_subdependencia.cod;
                nomSerie = dataFormato.p_subserie.p_serie.nombre;
                codSerie = dataFormato.p_subserie.p_serie.codigo;
                if (dataFormato.p_subserie.codigo != null) codSubserie = dataFormato.p_subserie.codigo;
                nomSubserie = dataFormato.p_subserie.nombre;
                if (dataFormato.t_carpeta.FirstOrDefault().t_lote.marco != null) marco = dataFormato.t_carpeta.FirstOrDefault().t_lote.marco;
                nombres = dataFormato.t_tercero.nombres + " " + dataFormato.t_tercero.apellidos;
                nroExp = dataFormato.t_carpeta.FirstOrDefault().nom_expediente;
                DateTime.TryParse(dataFormato.fecha_expediente_ini.ToString(), out fechaInicial);
                DateTime.TryParse(dataFormato.fecha_expediente_fin.ToString(), out fechaFinal);
                nomExp = dataFormato.t_carpeta.FirstOrDefault().nro_expediente;
                nroCaja = dataFormato.t_carpeta.FirstOrDefault().nro_caja;
            }

            ////////LOGO
            if (File.Exists("logo_" + idProyecto + ".png"))
            {
                PdfBitmap image = new PdfBitmap("logo_" + idProyecto + ".png");
                SizeF pageSize = page.GetClientSize();  //Width = 712 Height = 532  Console.WriteLine(pageSize.Width + " - " + pageSize.Height);
                RectangleF imageBounds = new RectangleF((float)factor * 2, (float)factor * 2, (float)19 * factor, (float)5 * factor);//Setting image bounds
                graphics.DrawImage(image, imageBounds);//Draw the image
            }
            ////////TITULO
            PdfStringFormat formatoTxtCentrado = new PdfStringFormat(); formatoTxtCentrado.Alignment = PdfTextAlignment.Center;
            PdfStringFormat formatoTxtIzquierda = new PdfStringFormat(); formatoTxtIzquierda.Alignment = PdfTextAlignment.Left;
            PdfFont fontNegrita = new PdfStandardFont(PdfFontFamily.Helvetica, (float)factor * 2, PdfFontStyle.Bold);//Set the standard font.
            PdfFont fontNegritaX1_5 = new PdfStandardFont(PdfFontFamily.Helvetica, factor * (float)2.5, PdfFontStyle.Bold);//Set the standard font.
            graphics.DrawString(rc_titulo1, fontNegrita, PdfBrushes.Black, new PointF((float)factor * 49, (float)factor * 1), formatoTxtCentrado);//Draw the text.
            graphics.DrawString(rc_titulo2, fontNegrita, PdfBrushes.Black, new PointF((float)factor * 49, (float)factor * 4), formatoTxtCentrado);//Draw the text.
            graphics.DrawString(rc_titulo3, fontNegrita, PdfBrushes.Black, new PointF((float)factor * 49, (float)factor * 7), formatoTxtCentrado);//Draw the text.
            RectangleF recTitulo = new RectangleF(factor * (float)23.23, 1, (float)factor * 51, (float)factor * 10); //Borde
            page.Graphics.DrawRectangle(borde, recTitulo);
            ////////CALIDAD
            PdfFont fontFormatoNegrita = new PdfStandardFont(PdfFontFamily.Helvetica, factor * (float)1.7, PdfFontStyle.Bold);
            graphics.DrawString(rc_cal_codigo, fontFormatoNegrita, PdfBrushes.Black, new PointF(recTitulo.X + recTitulo.Width + 4, (float)factor * 1), formatoTxtIzquierda);
            graphics.DrawString(rc_cal_version, fontFormatoNegrita, PdfBrushes.Black, new PointF(recTitulo.X + recTitulo.Width + 4, (float)factor * 4), formatoTxtIzquierda);
            graphics.DrawString(rc_cal_fecha, fontFormatoNegrita, PdfBrushes.Black, new PointF(recTitulo.X + recTitulo.Width + 4, (float)factor * 7), formatoTxtIzquierda);

            //Dependencia
            PdfFont fontlabel = new PdfStandardFont(PdfFontFamily.Helvetica, (float)factor * 2);//Set the standard font.
            PdfStringFormat stringCentrado = new PdfStringFormat();   //Initialize PdfStringFormat and set the properties
            stringCentrado.Alignment = PdfTextAlignment.Center;
            stringCentrado.LineAlignment = PdfVerticalAlignment.Middle;
            PdfStringFormat stringIzquierda = new PdfStringFormat();   //Initialize PdfStringFormat and set the properties
            stringIzquierda.Alignment = PdfTextAlignment.Left;
            stringIzquierda.LineAlignment = PdfVerticalAlignment.Middle;

            PdfTextElement element = new PdfTextElement("DEPENDENCIA SECCIÓN");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            PdfLayoutFormat layoutFormat = new PdfLayoutFormat();
            layoutFormat.Break = PdfLayoutBreakType.FitPage;
            RectangleF bounds = new RectangleF(new PointF((float)factor * 1, (float)factor * 11), new SizeF((float)factor * 18, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            PointF point1 = new PointF((float)factor * 20, (float)factor * 17);
            PointF point2 = new PointF((float)factor * 67, (float)factor * 17);
            PdfPen pen = new PdfPen(PdfBrushes.Black, 0.8f);//Initialize pen to draw the line
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement(nomDependencia);  //Create a text element
            element.Font = fontNegrita;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, (float)factor * 11), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            element = new PdfTextElement("CÓDIGO");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point2.X + (float)1, (float)factor * 11), new SizeF((float)factor * 19, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Código
            point1 = new PointF((float)factor * 76, point1.Y);
            point2 = new PointF((float)factor * 93, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement(codDependencia);  //Create a text element
            element.Font = fontNegrita;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, (float)factor * 11), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //Ofc Productora
            element = new PdfTextElement("OF. PRODUCTORA SUBSECCIÓN");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 1, (float)factor * 18), new SizeF((float)factor * 19, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 20, (float)factor * 24);
            point2 = new PointF((float)factor * 67, (float)factor * 24);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement(nomSubdependencia);  //Create a text element
            element.Font = fontNegrita;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, (float)factor * 18), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            element = new PdfTextElement("CÓDIGO");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point2.X + (float)1, (float)factor * 18), new SizeF((float)factor * 19, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 76, point1.Y);
            point2 = new PointF((float)factor * 93, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement(codSubdependencia);  //Create a text element
            element.Font = fontNegrita;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X + 1, (float)factor * 18), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //Serie
            element = new PdfTextElement("SERIE");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 1, (float)factor * 25), new SizeF((float)factor * 19, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 20, (float)factor * 31);
            point2 = new PointF((float)factor * 67, (float)factor * 31);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement(nomSerie);  //Create a text element
            element.Font = fontNegrita;
            element.StringFormat = stringCentrado;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, (float)factor * 25), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            element = new PdfTextElement("CÓDIGO");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point2.X + (float)1, (float)factor * 25), new SizeF((float)factor * 19, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 76, point1.Y);
            point2 = new PointF((float)factor * 93, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement(codSerie);  //Create a text element
            element.Font = fontNegrita;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X + 1, (float)factor * 25), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //SubSerie
            element = new PdfTextElement("SUBSERIE");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 1, (float)factor * 32), new SizeF((float)factor * 19, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 20, (float)factor * 38);
            point2 = new PointF((float)factor * 67, (float)factor * 38);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement(nomSubserie);  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, (float)factor * 32), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            element = new PdfTextElement("CÓDIGO");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point2.X + (float)1, (float)factor * 32), new SizeF((float)factor * 19, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 76, point1.Y);
            point2 = new PointF((float)factor * 93, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement(codSubserie);  //Create a text element
            element.Font = fontNegrita;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X + (float)1, (float)factor * 32), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Marco Legal
            element = new PdfTextElement("MARCO LEGAL");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 1, (float)factor * 38), new SizeF((float)factor * 19, (float)factor * 3));//Set bounds to draw multi-line text
            if (existeMarcoL == 1) element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 20, (float)factor * 41);
            point2 = new PointF((float)factor * 67, (float)factor * 41);
            if (existeMarcoL == 1) page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement(marco);  //Create a text element
            element.Font = fontNegrita;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, (float)factor * 38), new SizeF(point2.X - point1.X, (float)factor * 3));//Set bounds to draw multi-line text
            if (existeMarcoL == 1) element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            element = new PdfTextElement("DIGITALIZADO");  //Create a text element
            element.Font = fontNegrita;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point2.X, (float)factor * 41), new SizeF((float)factor * 30, (float)factor * 3));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //Nombre del Expediente
            element = new PdfTextElement("NOMBRE DEL EXPEDIENTE");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 1, point1.Y + 3), new SizeF(recMarco.Width - 2, (float)factor * 3));//Set bounds to draw multi-line text
            if (existeMarcoL != 1) bounds.Y = bounds.Y - 4;
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            if (existeMarcoL != 1) bounds.Y = bounds.Y + 4;
            element = new PdfTextElement(nombres);  //Create a text element
            element.Font = fontNegritaX1_5;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 2, bounds.Y + 7), new SizeF(recMarco.Width, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //CUADRO SECCIÓN UNO
            RectangleF recSeccion1 = new RectangleF(1, 1, recMarco.Width, bounds.Y + 12);
            page.Graphics.DrawRectangle(borde, recSeccion1);

            //SECCIÓN DOS
            //No Expediente
            element = new PdfTextElement("No. EXPEDIENTE");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 1, recSeccion1.Height + 2), new SizeF((float)factor * 16, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            element = new PdfTextElement(nroExp);  //Create a text element
            element.Font = fontNegritaX1_5;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 17, bounds.Y + 3), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 16, bounds.Y + 12);
            point2 = new PointF(recMarco.Width, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //Fechas EXTREMAS
            element = new PdfTextElement("FECHA INICIAL");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 1, bounds.Y + 15), new SizeF((float)factor * 16, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 16, point1.Y + 15);
            point2 = new PointF((float)factor * 32, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            string rxtFecha = "S.F.";
            var fechaMaxSistema = new DateTime(DateTime.MaxValue.Year, DateTime.MaxValue.Month, DateTime.MaxValue.Day, DateTime.MaxValue.Hour, DateTime.MaxValue.Minute, DateTime.MaxValue.Second);
            if (fechaInicial < fechaMaxSistema) rxtFecha = fechaInicial.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            element = new PdfTextElement(rxtFecha);  //Create a text element
            element.Font = fontNegritaX1_5;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 16, bounds.Y), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            element = new PdfTextElement("FECHA FINAL");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 33, bounds.Y), new SizeF((float)factor * 16, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 48, point1.Y);
            point2 = new PointF((float)factor * 64, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            rxtFecha = "S.F.";
            if (fechaFinal > DateTime.MinValue) rxtFecha = fechaFinal.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            element = new PdfTextElement(rxtFecha);  //Create a text element
            element.Font = fontNegritaX1_5;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 48, bounds.Y), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //Fechas Carpeta - Correlativo
            element = new PdfTextElement("CARPETA No.");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 1, bounds.Y + 15), new SizeF((float)factor * 16, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 16, point1.Y + 15);
            point2 = new PointF((float)factor * 32, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement(nomExp);  //Create a text element
            element.Font = fontNegritaX1_5;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 16, bounds.Y), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            element = new PdfTextElement("CORRELATIVO");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 33, bounds.Y), new SizeF((float)factor * 16, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 48, point1.Y);
            point2 = new PointF((float)factor * 64, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement("1 de 1");  //Create a text element
            element.Font = fontNegritaX1_5;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 48, bounds.Y), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //Folios
            element = new PdfTextElement("FOLIOS");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 1, bounds.Y + 15), new SizeF((float)factor * 16, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 16, point1.Y + 15);
            point2 = new PointF((float)factor * 32, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            string txtFolios = "1 AL " + folioFinal;
            if (folioFinal == 0) txtFolios = "0 AL 0";
            element = new PdfTextElement(txtFolios);  //Create a text element
            element.Font = fontNegritaX1_5;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 16, bounds.Y), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            graphics.DrawString("CAJA No.", fontlabel, PdfBrushes.Black, new PointF((float)factor * 30, bounds.Y + 14), formatoTxtCentrado);
            //Codigo de Barras CAJA
            PdfCode39Barcode barcode = new PdfCode39Barcode();//Drawing Code39 barcode
            barcode.BarHeight = 45;//Setting height of the barcode
            PdfFont fontBarcode = new PdfStandardFont(PdfFontFamily.Helvetica, (float)factor * 4, PdfFontStyle.Bold);//Set the standard font.
            barcode.Font = fontBarcode;
            barcode.Text = nroCaja;
            barcode.Size = new SizeF((float)factor * 60, (float)factor * 18);
            barcode.Draw(page, new PointF((float)factor * 2, bounds.Y + 23));//Printing barcode on to the Pdf.

            if (existeMarcoL == 1 && marco != "") txtQR = "MARCO LEGAL: " + marco + " / "; if (nroExp != "") txtQR = txtQR + nroExp + " - ";
            if (!string.IsNullOrEmpty(nombres)) txtQR = txtQR + nombres + " / ";
            if (!string.IsNullOrEmpty(nomExp)) txtQR = txtQR + "CARPETA: " + nomExp + " / "; if (nomExp != "") txtQR = txtQR + "CAJA: " + nroCaja;
            PdfQRBarcode barcodeQr = new PdfQRBarcode();//Drawing QR Barcode
            barcodeQr.ErrorCorrectionLevel = PdfErrorCorrectionLevel.Medium;//Set Error Correction Level
            barcodeQr.XDimension = 3;//Set XDimension
            barcodeQr.Size = new SizeF(factor * (float)30.5, (float)factor * 30);
            barcodeQr.Text = Regex.Replace(txtQR, @"[^0-9a-zA-Z:,|._-Ññ]+", " ");
            barcodeQr.Draw(page, new PointF((float)63 * factor, recSeccion1.Height + 29));//Printing barcode on to the Pdf.

            //Save the document.
            doc.Save($"{exportPath}" + "/RKP_" + idCarpeta + ".pdf");

            //Close the document.
            doc.Close(true);
        }

        public void ExportPdfRotuloCarpeta2(int idCarpeta, string exportPath, ref List<string> exportedList)
        {
            string rc_titulo1 = "", rc_titulo2 = "", rc_titulo3 = "", rc_cal_codigo = "", rc_cal_version = "", rc_cal_fecha = "", txtQR = "", nomDependencia = "", codDependencia = "", nomSubdependencia = "", codSubdependencia = "", nomSerie = "", codSerie = "", codSubserie = "", nomSubserie = "", marco = "", nombres = "", nroExp = "", nomExp = "", nroCaja = "", nroCarpeta = "";
            int idProyecto = 0, existeMarcoL = 0, folioInicial = 0, folioFinal = 0;
            DateTime fechaInicial = DateTime.MaxValue, fechaFinal = DateTime.MinValue;

            //Create a new PDF document.
            PdfDocument doc = new PdfDocument();
            float factor = 3;
            doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
            doc.PageSettings.Size = PdfPageSize.A5;
            doc.PageSettings.Size = new SizeF(100 * factor, 100 * factor);
            doc.PageSettings.Margins.Top = 1;
            doc.PageSettings.Margins.Right = 1;
            doc.PageSettings.Margins.Bottom = 1;
            doc.PageSettings.Margins.Left = 1;

            PdfPage page = doc.Pages.Add();//Add a page to the document.
            PdfGraphics graphics = page.Graphics;
            PdfPen borde = new PdfPen(Color.Black, 1);
            PdfPen bordeDebug = new PdfPen(Color.Red, 1);
            RectangleF recMarco = new RectangleF(1, 1, (float)93 * factor, (float)92 * factor); //Borde X-6 Y-7
            page.Graphics.DrawRectangle(borde, recMarco);
            RectangleF recparte1 = new RectangleF(1, 1, recMarco.Width, (float)factor * 10); //Borde
            page.Graphics.DrawRectangle(borde, recparte1);

            //Consulta base de datos
            var datHC = EntitiesRepository.Entities.t_carpeta.Include("t_lote").Include("p_proyecto").Include("t_tercero").AsNoTracking()
                    .Where(f => f.id == idCarpeta)
                    .Select(p => new { p.t_tercero, p.t_lote.p_subserie, p.t_lote.marco, p.t_lote.id_proyecto, p.t_lote.p_subdependencia, p.nom_expediente, p.nro_expediente, p.nro_caja, p.nro_carpeta, p.t_lote.p_proyecto, p.fecha_expediente_ini, p.fecha_expediente_fin, p.kp_folioini, p.kp_foliofin });

            var dataCarpeta = datHC.FirstOrDefault();
            if (dataCarpeta != null)
            {
                var dataFormato = EntitiesRepository.Entities.p_formato.AsNoTracking().Where(f => f.id_proyecto == dataCarpeta.id_proyecto).FirstOrDefault();
                if (dataFormato.rc_titulo1 != null) rc_titulo1 = dataFormato.rc_titulo1;
                if (dataFormato.rc_titulo2 != null) rc_titulo2 = dataFormato.rc_titulo2;
                if (dataFormato.rc_titulo3 != null) rc_titulo3 = dataFormato.rc_titulo3;
                if (dataFormato.rc_cal_codigo != null) rc_cal_codigo = dataFormato.rc_cal_codigo;
                if (dataFormato.rc_cal_version != null) rc_cal_version = dataFormato.rc_cal_version;
                if (dataFormato.rc_cal_fecha != null) rc_cal_fecha = dataFormato.rc_cal_fecha;
                int.TryParse(dataFormato.rc_marco_legal.ToString(), out existeMarcoL);
                DateTime.TryParse(dataFormato.fecha_inicial_defecto.ToString(), out fechaInicial);//FECHAS EXTREMAS


                string archivadoPor = dataCarpeta.p_proyecto.nom_proyecto;
                idProyecto = dataCarpeta.p_proyecto.id;
                nomDependencia = dataCarpeta.p_subdependencia.p_dependencia.nombre;
                codDependencia = dataCarpeta.p_subdependencia.p_dependencia.codigo;
                nomSubdependencia = dataCarpeta.p_subdependencia.nombre;
                if (dataCarpeta.p_subdependencia.cod != null) codSubdependencia = dataCarpeta.p_subdependencia.cod;
                nomSerie = dataCarpeta.p_subserie.p_serie.nombre;
                codSerie = dataCarpeta.p_subserie.p_serie.codigo;
                if (dataCarpeta.p_subserie.codigo != null) codSubserie = dataCarpeta.p_subserie.codigo;
                nomSubserie = dataCarpeta.p_subserie.nombre;
                if (dataCarpeta.marco != null) marco = dataCarpeta.marco;
                nombres = dataCarpeta.t_tercero.nombres + " " + dataCarpeta.t_tercero.apellidos;
                nroExp = dataCarpeta.nom_expediente;
                DateTime.TryParse(dataCarpeta.fecha_expediente_ini.ToString(), out fechaInicial);
                DateTime.TryParse(dataCarpeta.fecha_expediente_fin.ToString(), out fechaFinal);
                nomExp = dataCarpeta.nro_expediente;
                nroCaja = dataCarpeta.nro_caja;
                if (dataCarpeta.nro_carpeta != null) nroCarpeta = dataCarpeta.nro_carpeta.ToString();
                folioInicial = GlobalClass.GetNumber(dataCarpeta.kp_folioini.ToString(), 1);
                folioFinal = GlobalClass.GetNumber(dataCarpeta.kp_foliofin.ToString());
            }

            ////////LOGO
            if (File.Exists("logo_" + idProyecto + ".png"))
            {
                PdfBitmap image = new PdfBitmap("logo_" + idProyecto + ".png");
                SizeF pageSize = page.GetClientSize();  //Width = 712 Height = 532  Console.WriteLine(pageSize.Width + " - " + pageSize.Height);
                RectangleF imageBounds = new RectangleF((float)factor * 2, (float)factor * 2, (float)19 * factor, (float)5 * factor);//Setting image bounds
                graphics.DrawImage(image, imageBounds);//Draw the image
            }
            ////////TITULO
            PdfStringFormat formatoTxtCentrado = new PdfStringFormat(); formatoTxtCentrado.Alignment = PdfTextAlignment.Center;
            PdfStringFormat formatoTxtIzquierda = new PdfStringFormat(); formatoTxtIzquierda.Alignment = PdfTextAlignment.Left;
            PdfFont fontNegrita = new PdfStandardFont(PdfFontFamily.Helvetica, (float)factor * 2, PdfFontStyle.Bold);//Set the standard font.
            PdfFont fontNegritaX1_5 = new PdfStandardFont(PdfFontFamily.Helvetica, factor * (float)2.5, PdfFontStyle.Bold);//Set the standard font.
            graphics.DrawString(rc_titulo1, fontNegrita, PdfBrushes.Black, new PointF((float)factor * 49, (float)factor * 1), formatoTxtCentrado);//Draw the text.
            graphics.DrawString(rc_titulo2, fontNegrita, PdfBrushes.Black, new PointF((float)factor * 49, (float)factor * 4), formatoTxtCentrado);//Draw the text.
            graphics.DrawString(rc_titulo3, fontNegrita, PdfBrushes.Black, new PointF((float)factor * 49, (float)factor * 7), formatoTxtCentrado);//Draw the text.
            RectangleF recTitulo = new RectangleF(factor * (float)23.23, 1, (float)factor * 51, (float)factor * 10); //Borde
            page.Graphics.DrawRectangle(borde, recTitulo);
            ////////CALIDAD
            PdfFont fontFormatoNegrita = new PdfStandardFont(PdfFontFamily.Helvetica, factor * (float)1.7, PdfFontStyle.Bold);
            graphics.DrawString(rc_cal_codigo, fontFormatoNegrita, PdfBrushes.Black, new PointF(recTitulo.X + recTitulo.Width + 4, (float)factor * 1), formatoTxtIzquierda);
            graphics.DrawString(rc_cal_version, fontFormatoNegrita, PdfBrushes.Black, new PointF(recTitulo.X + recTitulo.Width + 4, (float)factor * 4), formatoTxtIzquierda);
            graphics.DrawString(rc_cal_fecha, fontFormatoNegrita, PdfBrushes.Black, new PointF(recTitulo.X + recTitulo.Width + 4, (float)factor * 7), formatoTxtIzquierda);

            //Dependencia
            PdfFont fontlabel = new PdfStandardFont(PdfFontFamily.Helvetica, (float)factor * 2);//Set the standard font.
            PdfStringFormat stringCentrado = new PdfStringFormat();   //Initialize PdfStringFormat and set the properties
            stringCentrado.Alignment = PdfTextAlignment.Center;
            stringCentrado.LineAlignment = PdfVerticalAlignment.Middle;
            PdfStringFormat stringIzquierda = new PdfStringFormat();   //Initialize PdfStringFormat and set the properties
            stringIzquierda.Alignment = PdfTextAlignment.Left;
            stringIzquierda.LineAlignment = PdfVerticalAlignment.Middle;

            PdfTextElement element = new PdfTextElement("DEPENDENCIA SECCIÓN");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            PdfLayoutFormat layoutFormat = new PdfLayoutFormat();
            layoutFormat.Break = PdfLayoutBreakType.FitPage;
            RectangleF bounds = new RectangleF(new PointF((float)factor * 1, (float)factor * 11), new SizeF((float)factor * 18, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            PointF point1 = new PointF((float)factor * 20, (float)factor * 17);
            PointF point2 = new PointF((float)factor * 67, (float)factor * 17);
            PdfPen pen = new PdfPen(PdfBrushes.Black, 0.8f);//Initialize pen to draw the line
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement(nomDependencia);  //Create a text element
            element.Font = fontNegrita;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, (float)factor * 11), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            element = new PdfTextElement("CÓDIGO");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point2.X + (float)1, (float)factor * 11), new SizeF((float)factor * 19, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Código
            point1 = new PointF((float)factor * 76, point1.Y);
            point2 = new PointF((float)factor * 93, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement(codDependencia);  //Create a text element
            element.Font = fontNegrita;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, (float)factor * 11), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //Ofc Productora
            element = new PdfTextElement("OF. PRODUCTORA SUBSECCIÓN");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 1, (float)factor * 18), new SizeF((float)factor * 19, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 20, (float)factor * 24);
            point2 = new PointF((float)factor * 67, (float)factor * 24);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement(nomSubdependencia);  //Create a text element
            element.Font = fontNegrita;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, (float)factor * 18), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            element = new PdfTextElement("CÓDIGO");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point2.X + (float)1, (float)factor * 18), new SizeF((float)factor * 19, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 76, point1.Y);
            point2 = new PointF((float)factor * 93, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement(codSubdependencia);  //Create a text element
            element.Font = fontNegrita;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X + 1, (float)factor * 18), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //Serie
            element = new PdfTextElement("SERIE");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 1, (float)factor * 25), new SizeF((float)factor * 19, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 20, (float)factor * 31);
            point2 = new PointF((float)factor * 67, (float)factor * 31);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement(nomSerie);  //Create a text element
            element.Font = fontNegrita;
            element.StringFormat = stringCentrado;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, (float)factor * 25), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            element = new PdfTextElement("CÓDIGO");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point2.X + (float)1, (float)factor * 25), new SizeF((float)factor * 19, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 76, point1.Y);
            point2 = new PointF((float)factor * 93, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement(codSerie);  //Create a text element
            element.Font = fontNegrita;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X + 1, (float)factor * 25), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //SubSerie
            element = new PdfTextElement("SUBSERIE");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 1, (float)factor * 32), new SizeF((float)factor * 19, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 20, (float)factor * 38);
            point2 = new PointF((float)factor * 67, (float)factor * 38);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement(nomSubserie);  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, (float)factor * 32), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            element = new PdfTextElement("CÓDIGO");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point2.X + (float)1, (float)factor * 32), new SizeF((float)factor * 19, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 76, point1.Y);
            point2 = new PointF((float)factor * 93, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement(codSubserie);  //Create a text element
            element.Font = fontNegrita;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X + (float)1, (float)factor * 32), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Marco Legal
            element = new PdfTextElement("MARCO LEGAL");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 1, (float)factor * 38), new SizeF((float)factor * 19, (float)factor * 3));//Set bounds to draw multi-line text
            //if (existeMarcoL == 1) element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 20, (float)factor * 41);
            point2 = new PointF((float)factor * 67, (float)factor * 41);
            //if (existeMarcoL == 1) page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement(marco);  //Create a text element
            element.Font = fontNegrita;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, (float)factor * 38), new SizeF(point2.X - point1.X, (float)factor * 3));//Set bounds to draw multi-line text
            if (existeMarcoL == 1) element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            element = new PdfTextElement("DIGITALIZADO");  //Create a text element
            element.Font = fontNegrita;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point2.X, (float)factor * 41), new SizeF((float)factor * 30, (float)factor * 3));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //Nombre del Expediente
            element = new PdfTextElement("NOMBRE DEL EXPEDIENTE");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 1, point1.Y + 3), new SizeF(recMarco.Width - 2, (float)factor * 3));//Set bounds to draw multi-line text
            if (existeMarcoL != 1) bounds.Y = bounds.Y - 4;
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            if (existeMarcoL != 1) bounds.Y = bounds.Y + 4;
            element = new PdfTextElement(nroExp);  //Create a text element //'nombres' si se requiere poner nombres y apellidos
            element.Font = fontNegritaX1_5;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 1, bounds.Y + 7), new SizeF(recMarco.Width - ((float)factor * 2), (float)factor * 12));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //CUADRO SECCIÓN UNO
            RectangleF recSeccion1 = new RectangleF(1, 1, recMarco.Width, bounds.Y + 12);
            RectangleF recSeccion1_v2 = new RectangleF(1, 1, recMarco.Width, bounds.Y + 29);
            page.Graphics.DrawRectangle(borde, recSeccion1_v2);

            //SECCIÓN DOS
            //No Expediente
            element = new PdfTextElement("No. EXPEDIENTE");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 1, recSeccion1.Height + 2), new SizeF((float)factor * 16, (float)factor * 5));//Set bounds to draw multi-line text
            //element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            element = new PdfTextElement(nroExp);  //Create a text element
            element.Font = fontNegritaX1_5;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 17, bounds.Y + 3), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            //element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 1, bounds.Y + 12);
            point2 = new PointF(recMarco.Width, point1.Y);
            //page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //Fechas EXTREMAS
            element = new PdfTextElement("FECHA INICIAL");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 1, bounds.Y + 15), new SizeF((float)factor * 16, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 16, point1.Y + 15);
            point2 = new PointF((float)factor * 32, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            string rxtFecha = "S.F.";
            var fechaMaxSistema = new DateTime(DateTime.MaxValue.Year, DateTime.MaxValue.Month, DateTime.MaxValue.Day, DateTime.MaxValue.Hour, DateTime.MaxValue.Minute, DateTime.MaxValue.Second);
            if (fechaInicial < fechaMaxSistema) rxtFecha = fechaInicial.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            element = new PdfTextElement(rxtFecha);  //Create a text element
            element.Font = fontNegritaX1_5;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 16, bounds.Y), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            element = new PdfTextElement("FECHA FINAL");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 33, bounds.Y), new SizeF((float)factor * 16, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 48, point1.Y);
            point2 = new PointF((float)factor * 64, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            rxtFecha = "S.F.";
            if (fechaFinal > DateTime.MinValue) rxtFecha = fechaFinal.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            element = new PdfTextElement(rxtFecha);  //Create a text element
            element.Font = fontNegritaX1_5;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 48, bounds.Y), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //Fechas Carpeta - Correlativo
            element = new PdfTextElement("CARPETA No.");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 1, bounds.Y + 15), new SizeF((float)factor * 16, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 16, point1.Y + 15);
            point2 = new PointF((float)factor * 32, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement(nroCarpeta);  //Create a text element
            element.Font = fontNegritaX1_5;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 16, bounds.Y), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            element = new PdfTextElement("CORRELATIVO");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 33, bounds.Y), new SizeF((float)factor * 16, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 48, point1.Y);
            point2 = new PointF((float)factor * 64, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement("1 de 1");  //Create a text element
            element.Font = fontNegritaX1_5;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 48, bounds.Y), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //Folios
            element = new PdfTextElement("FOLIOS");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 1, bounds.Y + 15), new SizeF((float)factor * 16, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF((float)factor * 16, point1.Y + 15);
            point2 = new PointF((float)factor * 32, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            string txtFolios = folioInicial + " A " + folioFinal;
            if (folioFinal == 0) txtFolios = "0 A 0";
            element = new PdfTextElement(txtFolios);  //Create a text element
            element.Font = fontNegritaX1_5;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF((float)factor * 16, bounds.Y), new SizeF(point2.X - point1.X, (float)factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            graphics.DrawString("CAJA No.", fontlabel, PdfBrushes.Black, new PointF((float)factor * 30, bounds.Y + 14), formatoTxtCentrado);
            //Codigo de Barras CAJA
            PdfCode39Barcode barcode = new PdfCode39Barcode();//Drawing Code39 barcode
            barcode.BarHeight = 45;//Setting height of the barcode
            PdfFont fontBarcode = new PdfStandardFont(PdfFontFamily.Helvetica, (float)factor * 4, PdfFontStyle.Bold);//Set the standard font.
            barcode.Font = fontBarcode;
            barcode.Text = nroCaja;
            barcode.Size = new SizeF((float)factor * 60, (float)factor * 18);
            barcode.Draw(page, new PointF((float)factor * 2, bounds.Y + 23));//Printing barcode on to the Pdf.

            if (existeMarcoL == 1 && marco != "") txtQR = "MARCO LEGAL: " + marco + " / "; if (nroExp != "") txtQR = txtQR + nroExp + " - ";
            if (!string.IsNullOrEmpty(nombres)) txtQR = txtQR + nombres + " / ";
            if (!string.IsNullOrEmpty(nomExp)) txtQR = txtQR + "CARPETA: " + nomExp + " / "; if (nomExp != "") txtQR = txtQR + "CAJA: " + nroCaja;
            PdfQRBarcode barcodeQr = new PdfQRBarcode();//Drawing QR Barcode
            barcodeQr.ErrorCorrectionLevel = PdfErrorCorrectionLevel.Medium;//Set Error Correction Level
            barcodeQr.XDimension = 3;//Set XDimension
            barcodeQr.Size = new SizeF(factor * (float)30.5, (float)factor * 30);
            barcodeQr.Text = Regex.Replace(txtQR, @"[^0-9a-zA-Z:,|._-Ññ]+", " ");
            barcodeQr.Draw(page, new PointF((float)63 * factor, recSeccion1.Height + 29));//Printing barcode on to the Pdf.

            //Save the document.
            string filename = $"{exportPath}" + "/RKP_" + idCarpeta + ".pdf";
            doc.Save(filename);
            exportedList.Add(filename);
            //Close the document.
            doc.Close(true);
        }

        public void ExportPdfRotuloCarpeta3(int idCarpeta, string exportPath, ref List<string> exportedList, p_formato mipFormato, int consecutivoKP = 1)
        {
            string rc_titulo1 = "", rc_titulo2 = "", rc_titulo3 = "", rc_cal_codigo = "", rc_cal_version = "", rc_cal_fecha = "", txtQR = "", nomDependencia = "", codDependencia = "", nomSubdependencia = "", codSubdependencia = "", nomSerie = "", codSerie = "", codSubserie = "", nomSubserie = "", marco = "", nombres = "", nroExp = "", nomExp = "", nroCaja = "", nroCarpeta = "", tomo = "";
            int idProyecto = 0, existeMarcoL = 0, folioInicial = 0, folioFinal = 0, lonTxtExp = 0;
            DateTime fechaInicial = DateTime.MaxValue, fechaFinal = DateTime.MinValue;

            //Create a new PDF document.
            PdfDocument doc = new PdfDocument();
            float factor = 3;
            doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
            doc.PageSettings.Size = PdfPageSize.A5;
            doc.PageSettings.Size = new SizeF(100 * factor, 100 * factor);
            doc.PageSettings.Margins.Top = 2;
            doc.PageSettings.Margins.Left = 4;
            doc.PageSettings.Margins.Right = 0;
            doc.PageSettings.Margins.Bottom = 1;


            PdfPage page = doc.Pages.Add();//Add a page to the document.
            PdfGraphics graphics = page.Graphics;
            PdfPen borde = new PdfPen(Color.Black, 1);
            PdfPen bordeDebug = new PdfPen(Color.Red, 1);
            RectangleF recMarco = new RectangleF(1, 1, (float)97 * factor, (float)97 * factor); //Borde X-6 Y-7
            page.Graphics.DrawRectangle(borde, recMarco);
            RectangleF recparte1 = new RectangleF(1, 1, recMarco.Width, factor * 10); //Borde
            page.Graphics.DrawRectangle(borde, recparte1);

            //Consulta base de datos
            var datHC = EntitiesRepository.Entities.t_carpeta.Include("t_lote").Include("p_proyecto").Include("t_tercero").AsNoTracking()
                    .Where(f => f.id == idCarpeta)
                    .Select(p => new { p.t_tercero, p.t_lote.p_subserie, p.t_lote.marco, p.t_lote.id_proyecto, p.t_lote.p_subdependencia, p.nom_expediente, p.nro_expediente, p.nro_caja, p.nro_carpeta, p.t_lote.p_proyecto, p.fecha_expediente_ini, p.fecha_expediente_fin, p.kp_folioini, p.kp_foliofin, p.tomo, p.tomo_fin });

            var dataCarpeta = datHC.FirstOrDefault();
            if (dataCarpeta != null)
            {
                if (mipFormato.rc_titulo1 != null) rc_titulo1 = mipFormato.rc_titulo1;
                if (mipFormato.rc_titulo2 != null) rc_titulo2 = mipFormato.rc_titulo2;
                if (mipFormato.rc_titulo3 != null) rc_titulo3 = mipFormato.rc_titulo3;
                if (mipFormato.rc_cal_codigo != null) rc_cal_codigo = mipFormato.rc_cal_codigo;
                if (mipFormato.rc_cal_version != null) rc_cal_version = mipFormato.rc_cal_version;
                if (mipFormato.rc_cal_fecha != null) rc_cal_fecha = mipFormato.rc_cal_fecha;
                int.TryParse(mipFormato.rc_marco_legal.ToString(), out existeMarcoL);
                DateTime.TryParse(mipFormato.fecha_inicial_defecto.ToString(), out fechaInicial);//FECHAS EXTREMAS

                string archivadoPor = dataCarpeta.p_proyecto.nom_proyecto;
                idProyecto = dataCarpeta.p_proyecto.id;
                nomDependencia = dataCarpeta.p_subdependencia.p_dependencia.nombre;
                codDependencia = dataCarpeta.p_subdependencia.p_dependencia.codigo;
                nomSubdependencia = dataCarpeta.p_subdependencia.nombre;
                if (dataCarpeta.p_subdependencia.cod != null) codSubdependencia = dataCarpeta.p_subdependencia.cod;
                nomSerie = dataCarpeta.p_subserie.p_serie.nombre;
                codSerie = dataCarpeta.p_subserie.p_serie.codigo;
                if (dataCarpeta.p_subserie.codigo != null) codSubserie = dataCarpeta.p_subserie.codigo;
                nomSubserie = dataCarpeta.p_subserie.nombre;
                if (dataCarpeta.marco != null) marco = dataCarpeta.marco;
                nombres = dataCarpeta.t_tercero?.nombres + " " + dataCarpeta.t_tercero?.apellidos;
                nomExp = dataCarpeta.nom_expediente.Replace(System.Environment.NewLine, "").Trim();
                lonTxtExp = nomExp.Length;
                DateTime.TryParse(dataCarpeta.fecha_expediente_ini.ToString(), out fechaInicial);
                DateTime.TryParse(dataCarpeta.fecha_expediente_fin.ToString(), out fechaFinal);
                nroExp = dataCarpeta.nro_expediente;
                nroCaja = dataCarpeta.nro_caja;
                //if (dataCarpeta.nro_carpeta != null) nroCarpeta = dataCarpeta.nro_carpeta.ToString();
                //else 
                nroCarpeta = consecutivoKP.ToString();
                if (GlobalClass.GetNumber(dataCarpeta.nro_carpeta.ToString()) > 0) nroCarpeta = dataCarpeta.nro_carpeta.ToString();
                if (dataCarpeta.tomo != null) tomo = dataCarpeta.tomo.ToString();
                if (dataCarpeta.tomo_fin != null) tomo += " DE " + dataCarpeta.tomo_fin.ToString();
                else tomo += " DE " + dataCarpeta.tomo.ToString();

                folioInicial = GlobalClass.GetNumber(dataCarpeta.kp_folioini.ToString(), 1); if (folioInicial == 0) folioInicial = 1;
                folioFinal = GlobalClass.GetNumber(dataCarpeta.kp_foliofin.ToString());
            }

            ////////LOGO
            if (File.Exists("logo_" + idProyecto + ".png"))
            {
                PdfBitmap image = new PdfBitmap("logo_" + idProyecto + ".png");
                SizeF pageSize = page.GetClientSize();  //Width = 712 Height = 532  Console.WriteLine(pageSize.Width + " - " + pageSize.Height);
                RectangleF imageBounds = new RectangleF(factor * 2, factor * 2, (float)19 * factor, (float)5 * factor);//Setting image bounds
                graphics.DrawImage(image, imageBounds);//Draw the image
            }
            PdfLinearGradientBrush brush = new PdfLinearGradientBrush(new PointF(0, 0), new PointF(1, 1), Color.Red, Color.Blue);
            brush.Background = Color.FromArgb(51, 102, 204);
            ////////TITULO
            PdfStringFormat formatoTxtCentrado = new PdfStringFormat(); formatoTxtCentrado.Alignment = PdfTextAlignment.Center;
            PdfStringFormat formatoTxtIzquierda = new PdfStringFormat(); formatoTxtIzquierda.Alignment = PdfTextAlignment.Left;
            PdfFont fontNegrita = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 2, PdfFontStyle.Bold);//Set the standard font.
            PdfFont fontMin = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.9f);//Set the standard font.
            PdfFont fontMinMin = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.4f);
            PdfFont fontNegritaMin = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.8f, PdfFontStyle.Bold);//Set the standard font.
            PdfFont fontX1_5 = new PdfStandardFont(PdfFontFamily.Helvetica, factor * (float)2.5);//Set the standard font.
            PdfFont fontNegritaX1_5 = new PdfStandardFont(PdfFontFamily.Helvetica, factor * (float)2.5, PdfFontStyle.Bold);//Set the standard font.
            //Rectangulos titulo central
            RectangleF recTitulo = new RectangleF(factor * 23.23f, 1, factor * 51, factor * 10); //Borde
            page.Graphics.DrawRectangle(borde, recTitulo);
            RectangleF recTitulo1 = new RectangleF(factor * 23.23f, 1, factor * 51, factor * 3.4f); //Borde
            graphics.DrawRectangle(borde, recTitulo1);
            graphics.DrawRectangle(brush, recTitulo1);
            RectangleF recTitulo2 = new RectangleF(factor * 23.23f, 1, factor * 51, factor * 6.6f); //Borde
            graphics.DrawRectangle(borde, recTitulo2);

            graphics.DrawString(rc_titulo1, fontNegrita, PdfBrushes.White, new PointF(factor * 49, factor * 1), formatoTxtCentrado);//Draw the text.
            graphics.DrawString(rc_titulo2, fontNegrita, PdfBrushes.Black, new PointF(factor * 49, factor * 4), formatoTxtCentrado);//Draw the text.
            graphics.DrawString(rc_titulo3, fontNegrita, PdfBrushes.Black, new PointF(factor * 49, factor * 7.5f), formatoTxtCentrado);//Draw the text.

            ////////CALIDAD
            PdfFont fontFormatoNegrita = new PdfStandardFont(PdfFontFamily.Helvetica, factor * (float)1.7, PdfFontStyle.Bold);
            graphics.DrawString(rc_cal_codigo, fontFormatoNegrita, PdfBrushes.Black, new PointF(recTitulo.X + recTitulo.Width + 4, factor * 0.8f), formatoTxtIzquierda);
            graphics.DrawString(rc_cal_version, fontFormatoNegrita, PdfBrushes.Black, new PointF(recTitulo.X + recTitulo.Width + 4, factor * 3.2f), formatoTxtIzquierda);
            graphics.DrawString(rc_cal_fecha, fontFormatoNegrita, PdfBrushes.Black, new PointF(recTitulo.X + recTitulo.Width + 4, factor * 5.6f), formatoTxtIzquierda);
            graphics.DrawString("Página 1 de 1", fontFormatoNegrita, PdfBrushes.Black, new PointF(recTitulo.X + recTitulo.Width + 4, factor * 8f), formatoTxtIzquierda);

            //Dependencia
            PdfFont fontlabel = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 2);//Set the standard font.
            PdfFont fontlabelMin = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.9f);//Set the standard font.
            PdfFont fontlabelMinMin = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.6f);//Set the standard font.
            PdfFont fontlabelMinMinMin = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.3f);//Set the standard font.
            PdfStringFormat stringCentrado = new PdfStringFormat();   //Initialize PdfStringFormat and set the properties
            stringCentrado.Alignment = PdfTextAlignment.Center;
            stringCentrado.LineAlignment = PdfVerticalAlignment.Middle;
            PdfStringFormat stringIzquierda = new PdfStringFormat();   //Initialize PdfStringFormat and set the properties
            stringIzquierda.Alignment = PdfTextAlignment.Left;
            stringIzquierda.LineAlignment = PdfVerticalAlignment.Middle;

            PdfTextElement element = new PdfTextElement("SECCIÓN - UNIDAD ADMINISTRATIVA");  //Create a text element
            element.Font = fontlabelMin;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            PdfLayoutFormat layoutFormat = new PdfLayoutFormat();
            layoutFormat.Break = PdfLayoutBreakType.FitPage;
            RectangleF bounds = new RectangleF(new PointF(factor * 1, factor * 11), new SizeF(factor * 18, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //LINEA
            PointF point1 = new PointF(factor * 20, factor * 17);
            PointF point2 = new PointF(factor * 71, factor * 17);
            PdfPen pen = new PdfPen(PdfBrushes.Black, 0.8f);//Initialize pen to draw the line
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //DATO
            element = new PdfTextElement(nomDependencia);  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, factor * 11), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            element = new PdfTextElement("CÓDIGO");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point2.X + (float)1, factor * 11), new SizeF(factor * 19, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Código
            point1 = new PointF(factor * 80, point1.Y);
            point2 = new PointF(factor * 97, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement(codDependencia);  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, factor * 11), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //Ofc Productora
            element = new PdfTextElement("SUBSECCIÓN - OFICINA PRODUCTORA");  //Create a text element
            element.Font = fontlabelMinMin;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 1, factor * 18), new SizeF(factor * 19, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //LINEA
            point1 = new PointF(factor * 20, factor * 24);
            point2 = new PointF(factor * 71, factor * 24);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //DATO
            element = new PdfTextElement(nomSubdependencia);  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, factor * 18), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            element = new PdfTextElement("CÓDIGO");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point2.X + (float)1, factor * 18), new SizeF(factor * 19, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Linea
            point1 = new PointF(factor * 80, point1.Y);
            point2 = new PointF(factor * 97, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //Dato
            element = new PdfTextElement(codSubdependencia);  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X + 1, factor * 18), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //Serie
            element = new PdfTextElement("SERIE");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 1, factor * 25), new SizeF(factor * 19, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Línea
            point1 = new PointF(factor * 20, factor * 31);
            point2 = new PointF(factor * 71, factor * 31);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //Dato
            element = new PdfTextElement(nomSerie);  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringCentrado;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, factor * 25), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            element = new PdfTextElement("CÓDIGO");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point2.X + (float)1, factor * 25), new SizeF(factor * 19, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Línea
            point1 = new PointF(factor * 80, point1.Y);
            point2 = new PointF(factor * 97, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //Dato
            element = new PdfTextElement(codSerie);  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X + 1, factor * 25), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //SubSerie
            element = new PdfTextElement("SUBSERIE");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 1, factor * 32), new SizeF(factor * 19, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Línea
            point1 = new PointF(factor * 20, factor * 38);
            point2 = new PointF(factor * 71, factor * 38);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //Dato
            element = new PdfTextElement(nomSubserie);  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, factor * 32), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            element = new PdfTextElement("CÓDIGO");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point2.X + (float)1, factor * 32), new SizeF(factor * 19, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Línea
            point1 = new PointF(factor * 80, point1.Y);
            point2 = new PointF(factor * 97, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //Dato
            element = new PdfTextElement(codSubserie);  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X + (float)1, factor * 32), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //Marco Legal
            element = new PdfTextElement("MARCO LEGAL");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 1, factor * 40f), new SizeF(factor * 19, factor * 3));//Set bounds to draw multi-line text
            //if (existeMarcoL == 1) 
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Línea
            point1 = new PointF(factor * 20, factor * 44);
            point2 = new PointF(factor * 97, factor * 44);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //Dato
            element = new PdfTextElement(marco);  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, factor * 40f), new SizeF(point2.X - point1.X, factor * 3));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //Nombre del Expediente
            element = new PdfTextElement("NOMBRE DEL EXPEDIENTE");  //Create a text element
            element.Font = fontlabelMin;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 1, point1.Y + 3), new SizeF(factor * 19, factor * 5));//Set bounds to draw multi-line text
            //if (existeMarcoL != 1) bounds.Y = bounds.Y - 4;
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //if (existeMarcoL != 1) bounds.Y = bounds.Y + 4;
            //Dato
            string nomExpRev = nomExp;
            if (lonTxtExp > 173) nomExpRev = nomExp.Substring(0, 173);
            element = new PdfTextElement(nomExpRev);  //Create a text element //'nombres' si se requiere poner nombres y apellidos
            element.Font = fontMin;
            if (lonTxtExp > 127) element.Font = fontMinMin;
            //if (lonTxtExp > 127) nomExp = nomExp.Substring(0, 127);
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 20, bounds.Y), new SizeF(recMarco.Width - (factor * 21), factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Linea
            point1 = new PointF(factor * 20, bounds.Y + 18);
            point2 = new PointF(factor * 97, bounds.Y + 18);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //CUADRO SECCIÓN UNO
            RectangleF recSeccion1 = new RectangleF(1, 1, recMarco.Width, bounds.Y + 12);
            /*RectangleF recSeccion1_v2 = new RectangleF(1, 1, recMarco.Width, bounds.Y + 29);
            page.Graphics.DrawRectangle(borde, recSeccion1_v2);*/

            //SECCIÓN DOS
            //No Expediente
            element = new PdfTextElement("No. EXPEDIENTE");  //Create a text element
            element.Font = fontlabelMin;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 1, point1.Y + 5), new SizeF(factor * 19, factor * 3));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Dato
            element = new PdfTextElement(nroExp);  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 20, bounds.Y), new SizeF(recMarco.Width - (factor * 21), factor * 3));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Linea
            point1 = new PointF(factor * 20, bounds.Y + 9);
            point2 = new PointF(recMarco.Width, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document

            //Fechas EXTREMAS
            element = new PdfTextElement("FECHA INICIAL");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 1, bounds.Y + 10), new SizeF(factor * 16, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Linea
            point1 = new PointF(factor * 16, point1.Y + 13);
            point2 = new PointF(factor * 32, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //Dato
            DateTime fMin = DateTime.MinValue;
            string rxtFecha = "S.F.";
            var fechaMaxSistema = new DateTime(DateTime.MaxValue.Year, DateTime.MaxValue.Month, DateTime.MaxValue.Day, DateTime.MaxValue.Hour, DateTime.MaxValue.Minute, DateTime.MaxValue.Second);
            if (fechaInicial < fechaMaxSistema) rxtFecha = fechaInicial.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            else
            {
                var fMinBD = EntitiesRepository.Entities.t_documento.AsNoTracking().Where(c => c.id_carpeta == idCarpeta && c.folio_ini == 1).Select(p => p.fecha).FirstOrDefault();
                fMin = fMinBD ?? fechaMaxSistema;
                if (fMin < fechaMaxSistema) rxtFecha = fMin.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            //Calcula fechas correctas
            /*if (fMin == DateTime.MinValue && fechaFinal > DateTime.MinValue)
            {
                fMin = fechaFinal.AddDays((fechaFinal.Day * -1) + 1);
                rxtFecha = fMin.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            if (fMin.Ticks == 3155378975990000000 && fechaFinal == DateTime.MinValue)
            {
                var fMaxBD = EntitiesRepository.Entities.t_carpeta.AsNoTracking().Where(c => c.nom_expediente == dataCarpeta.nom_expediente && c.id < idCarpeta).OrderBy(p => p.fecha_expediente_fin).Select(p => p.fecha_expediente_fin).FirstOrDefault();
                fechaFinal = fMaxBD ?? fechaMaxSistema;
            } */
            element = new PdfTextElement(rxtFecha);  //Create a text element
            element.Font = fontX1_5;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 16, bounds.Y), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            element = new PdfTextElement("FECHA FINAL");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 33, bounds.Y), new SizeF(factor * 16, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF(factor * 48, point1.Y);
            point2 = new PointF(factor * 64, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            rxtFecha = "S.F.";
            if (fechaFinal.Ticks < 3155378975990000000 && fechaFinal > DateTime.MinValue) rxtFecha = fechaFinal.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            element = new PdfTextElement(rxtFecha);  //Create a text element
            element.Font = fontX1_5;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 48, bounds.Y), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //Fechas Carpeta - Correlativo
            element = new PdfTextElement("CARPETA No.");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 1, bounds.Y + 12), new SizeF(factor * 16, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Linea
            point1 = new PointF(factor * 16, point1.Y + 12);
            point2 = new PointF(factor * 32, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //Dato
            element = new PdfTextElement(nroCarpeta);  //Create a text element
            element.Font = fontX1_5;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 16, bounds.Y), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            element = new PdfTextElement("CORRELATIVO");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 33, bounds.Y), new SizeF(factor * 16, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Linea
            point1 = new PointF(factor * 48, point1.Y);
            point2 = new PointF(factor * 64, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //Dato
            element = new PdfTextElement(tomo);  //Create a text element
            element.Font = fontX1_5;
            int lenTomo = tomo.Length;
            if (lenTomo > 10) element.Font = fontMin;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 48, bounds.Y), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //Folios
            element = new PdfTextElement("FOLIOS");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 1, bounds.Y + 12), new SizeF(factor * 16, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Linea
            point1 = new PointF(factor * 16, point1.Y + 12);
            point2 = new PointF(factor * 32, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //Dato
            string txtFolios = folioInicial + " A " + folioFinal;
            int total_folio = 0;
            if (folioFinal == 0) txtFolios = "0 A 0";
            else total_folio = folioFinal - folioInicial + 1;
            element = new PdfTextElement(txtFolios);  //Create a text element //fontNegritaX1_5 //fontNegrita
            if (txtFolios.Length > 15)
            {
                element.Font = fontNegritaMin;
            }
            else if (txtFolios.Length > 8)
            {
                element.Font = fontNegritaMin;

            }
            else
            {
                element.Font = fontX1_5;
            }
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 16, bounds.Y), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //Titulo total folios
            element = new PdfTextElement("TOTAL FOLIOS");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point2.X + 1, bounds.Y), new SizeF(factor * 16, factor * 4));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Linea Total folios
            point1 = new PointF(factor * 48, point1.Y);
            point2 = new PointF(factor * 64, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //Dato total Folios
            element = new PdfTextElement(total_folio.ToString());  //Create a text element
            element.Font = fontX1_5;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 49, bounds.Y), new SizeF(factor * 13.5f, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //SIGNATURA TOPOGRAFICA
            RectangleF recSigna = new RectangleF(factor * 1f, point1.Y + 7, factor * 61.5f, factor * 9); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //Cuadro ubicación
            recSigna = new RectangleF(factor * 1f, point1.Y + 7, factor * 10f, factor * 9); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //TXT Cuerpo
            element = new PdfTextElement("UBICACIÓN TOPOGRÁFICA");  //Create a text element
            element.Font = fontlabelMinMinMin;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, recSigna, layoutFormat);
            //Ajuste para el Y del cuadro 
            //point1.Y += + 
            //Cuadro Bodega
            recSigna = new RectangleF(factor * 1f, point1.Y + 7, factor * 17f, factor * 9); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //recTitulo Bodega
            recSigna = new RectangleF(factor * 11f, point1.Y + 7, factor * 7f, factor * 3); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //Fondo titulo bodega
            brush = new PdfLinearGradientBrush(new PointF(0, 0), new PointF(1, 1), Color.Red, Color.Blue);
            brush.Background = Color.FromArgb(226, 226, 226);
            recSigna.Width = recSigna.Width - 1;
            recSigna.X = recSigna.X + 0.5f;
            recSigna.Y = recSigna.Y - 0.5f;
            graphics.DrawRectangle(brush, recSigna);
            //TXT Bodega
            PdfFont fontTituloCuadro = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.4F, PdfFontStyle.Bold);//Set the standard font.
            element = new PdfTextElement("BODEGA");  //Create a text element
            element.Font = fontTituloCuadro;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, recSigna, layoutFormat);

            //rec CUERPO
            recSigna = new RectangleF(factor * 1f, point1.Y + 7, factor * 24f, factor * 9); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //recTitulo TORRE
            recSigna = new RectangleF(factor * 18f, point1.Y + 7, factor * 7f, factor * 3); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //FOndo titulo torre
            recSigna.Width = recSigna.Width - 1;
            recSigna.X = recSigna.X + 0.5f;
            recSigna.Y = recSigna.Y - 0.5f;
            graphics.DrawRectangle(brush, recSigna);
            //TXT Torre
            fontTituloCuadro = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.4F, PdfFontStyle.Bold);//Set the standard font.
            element = new PdfTextElement("TORRE");  //Create a text element
            element.Font = fontTituloCuadro;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, recSigna, layoutFormat);

            recSigna = new RectangleF(factor * 1f, point1.Y + 7, factor * 31f, factor * 9); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //recTitulo CUERPO
            recSigna = new RectangleF(factor * 25f, point1.Y + 7, factor * 7f, factor * 3); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //FOndo titulo Cuerpo
            recSigna.Width = recSigna.Width - 1;
            recSigna.X = recSigna.X + 0.5f;
            recSigna.Y = recSigna.Y - 0.5f;
            graphics.DrawRectangle(brush, recSigna);
            //TXT Cuerpo
            fontTituloCuadro = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.4F, PdfFontStyle.Bold);//Set the standard font.
            element = new PdfTextElement("CUERPO");  //Create a text element
            element.Font = fontTituloCuadro;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, recSigna, layoutFormat);


            recSigna = new RectangleF(factor * 1f, point1.Y + 7, factor * 38f, factor * 9); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //recTitulo PISO
            recSigna = new RectangleF(factor * 32f, point1.Y + 7, factor * 7f, factor * 3); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //FOndo titulo Piso
            recSigna.Width = recSigna.Width - 1;
            recSigna.X = recSigna.X + 0.5f;
            recSigna.Y = recSigna.Y - 0.5f;
            graphics.DrawRectangle(brush, recSigna);
            //TXT Cuerpo
            fontTituloCuadro = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.4F, PdfFontStyle.Bold);//Set the standard font.
            element = new PdfTextElement("PISO");  //Create a text element
            element.Font = fontTituloCuadro;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, recSigna, layoutFormat);

            recSigna = new RectangleF(factor * 1f, point1.Y + 7, factor * 45f, factor * 9); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //recTitulo NIVEL
            recSigna = new RectangleF(factor * 39f, point1.Y + 7, factor * 7f, factor * 3); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //FOndo titulo Nivel
            recSigna.Width = recSigna.Width - 1;
            recSigna.X = recSigna.X + 0.5f;
            recSigna.Y = recSigna.Y - 0.5f;
            graphics.DrawRectangle(brush, recSigna);
            //TXT Cuerpo
            fontTituloCuadro = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.4F, PdfFontStyle.Bold);//Set the standard font.
            element = new PdfTextElement("NIVEL");  //Create a text element
            element.Font = fontTituloCuadro;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, recSigna, layoutFormat);

            recSigna = new RectangleF(factor * 1f, point1.Y + 7, factor * 52f, factor * 9); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //recTitulo PASILLO
            recSigna = new RectangleF(factor * 46f, point1.Y + 7, factor * 7f, factor * 3); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //FOndo titulo Pasillo
            recSigna.Width = recSigna.Width - 1;
            recSigna.Height = recSigna.Height - 1;
            recSigna.X = recSigna.X + 0.5f;
            recSigna.Y = recSigna.Y + 0.5f;
            graphics.DrawRectangle(brush, recSigna);
            //TXT Cuerpo
            fontTituloCuadro = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.4F, PdfFontStyle.Bold);//Set the standard font.
            element = new PdfTextElement("PASILLO");  //Create a text element
            element.Font = fontTituloCuadro;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, recSigna, layoutFormat);

            //recTitulo POSICION CJ
            recSigna = new RectangleF(factor * 53f, point1.Y + 7, factor * 9.5f, factor * 3); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //FOndo titulo POSICION CJ
            recSigna.Width = recSigna.Width - 1;
            recSigna.Height = recSigna.Height - 1;
            recSigna.X = recSigna.X + 0.5f;
            recSigna.Y = recSigna.Y + 0.5f;
            graphics.DrawRectangle(brush, recSigna);
            //TXT Cuerpo
            fontTituloCuadro = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.4F, PdfFontStyle.Bold);//Set the standard font.
            element = new PdfTextElement("POSICION CJ");  //Create a text element
            element.Font = fontTituloCuadro;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, recSigna, layoutFormat);

            graphics.DrawString("CAJA No.", fontlabel, PdfBrushes.Black, new PointF(factor * 5, recSigna.Y + 41), formatoTxtCentrado);
            //Codigo de Barras CAJA
            PdfCode39Barcode barcode = new PdfCode39Barcode();//Drawing Code39 barcode
            barcode.BarHeight = 30;//Setting height of the barcode
            PdfFont fontBarcode = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 2.6f, PdfFontStyle.Bold);//Set the standard font.
            barcode.Font = fontBarcode;
            barcode.Text = nroCaja.PadLeft(5, '0');
            barcode.Size = new SizeF(factor * 50, factor * 14);
            barcode.Draw(page, new PointF(factor * 12, recSigna.Y + 32));//Printing barcode on to the Pdf.

            ///////////QR/////////////
            //if (existeMarcoL == 1 && marco != "") txtQR = "MARCO LEGAL: " + marco + " / "; if (nroExp != "") txtQR = txtQR + nroExp + " - ";
            txtQR = "NA";
            if (!string.IsNullOrEmpty(nroExp)) txtQR = nroExp;
            if (!string.IsNullOrEmpty(nomExp)) txtQR += " - " + nomExp;
            if (!string.IsNullOrEmpty(tomo)) txtQR += " TOMO: " + tomo;
            txtQR = txtQR.Replace('Á', 'A');
            txtQR = txtQR.Replace('É', 'E');
            txtQR = txtQR.Replace('Í', 'I');
            txtQR = txtQR.Replace('Ó', 'O');
            txtQR = txtQR.Replace('Ú', 'U');
            txtQR = txtQR.Replace('Ñ', 'N');
            txtQR = Regex.Replace(txtQR, @"[^0-9a-zA-Z|]+", " ");    //[^0-9a-zA-Z:\u00C0-\u00FF,|._-Ññ]+
            lonTxtExp = txtQR.Length; Console.WriteLine(lonTxtExp);

            if (lonTxtExp > 200) txtQR = txtQR.Substring(0, Math.Min(lonTxtExp, 200));

            ////PdfQRBarcode barcodeQr = new PdfQRBarcode();//Drawing QR Barcode
            //barcodeQr.ErrorCorrectionLevel = PdfErrorCorrectionLevel.Medium;
            //if (lonTxtExp > 127) barcodeQr.ErrorCorrectionLevel = PdfErrorCorrectionLevel.Low;
            //barcodeQr.XDimension = 3;//Set XDimension
            //barcodeQr.Size = new SizeF(factor * 34f, factor * 34f);
            //barcodeQr.Text = txtQR;
            //barcodeQr.Draw(page, new PointF(63f * factor, recSeccion1.Height + 29));//Printing barcode on to the Pdf.


            //QR NUEVA VERSIÓN
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeGenerator.ECCLevel nivelQR = QRCodeGenerator.ECCLevel.M;
            if (lonTxtExp > 120) nivelQR = QRCodeGenerator.ECCLevel.L;
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(txtQR, nivelQR);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(21);
            PdfBitmap image2 = new PdfBitmap(qrCodeImage);
            RectangleF imageBounds2 = new RectangleF(63f * factor, recSeccion1.Height + 29, factor * 34f, factor * 34f);//Setting image bounds
            graphics.DrawImage(image2, imageBounds2);//Draw the image

            //Save the document.
            string filename = $"{exportPath}" + "/RKP_" + idCarpeta + ".pdf";
            doc.Save(filename);
            //Close the document.
            doc.Close(true);

            var match = exportedList.FirstOrDefault(stringToCheck => stringToCheck.Contains(filename));

            if (match == null)
            {
                exportedList.Add(filename);
            }

        }

        /**
         * Metodo que construye un rectangulo con titulo y data variable
         */
        private void buildRectangleLabel(double x, double y, double width, double height,
            String titulo, PdfFont font, PdfStringFormat stringFormat, PdfPage page, PdfPen borde, String data,
            PdfStringFormat stringFormatData, PdfFont fontLabelData)
        {

            //Limite del texto
            RectangleF rectangulo = new RectangleF((float)x, (float)y, (float)width, (float)height);
            page.Graphics.DrawRectangle(borde, rectangulo);

            //Titulo Celda
            PdfTextElement sbTitulo = new PdfTextElement(titulo);
            sbTitulo.Font = font;
            sbTitulo.StringFormat = stringFormat;
            sbTitulo.Brush = new PdfSolidBrush(Color.Black);
            PdfLayoutFormat layoutFormat = new PdfLayoutFormat();
            layoutFormat.Break = PdfLayoutBreakType.FitPage;
            RectangleF bounds1 = new RectangleF((float)x + 3, (float)y, (float)width, (float)height);
            sbTitulo.Draw(page, bounds1, layoutFormat);

            //Label de datos
            PdfTextElement labelData = new PdfTextElement(data);
            labelData.Font = fontLabelData;
            labelData.StringFormat = stringFormatData;
            labelData.Brush = new PdfSolidBrush(Color.Black);
            PdfLayoutFormat layoutFormat2 = new PdfLayoutFormat();
            layoutFormat.Break = PdfLayoutBreakType.FitPage;
            RectangleF bounds2 = new RectangleF((float)x + 3, (float)y + 2, (float)width, (float)height);
            labelData.Draw(page, bounds2, layoutFormat2);

        }

        public void ExportPdfRotuloCarpeta4(int idCarpeta, string exportPath, ref List<string> exportedList, p_formato mipFormato)
        {
            //Create a new PDF document.
            PdfDocument doc = new PdfDocument();
            float factor = 1;
            doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
            doc.PageSettings.Size = new SizeF(340 * factor, 283 * factor); //1417 X 1181
            doc.PageSettings.Margins.Top = 1;
            doc.PageSettings.Margins.Right = 1;
            doc.PageSettings.Margins.Bottom = 1;
            doc.PageSettings.Margins.Left = 1;

            PdfPage page = doc.Pages.Add();//Add a page to the document.
            PdfGraphics graphics = page.Graphics;
            PdfPen borde = new PdfPen(Color.Black, 1);
            PdfPen sinBorde = new PdfPen(Color.Black, 0);
            PdfPen bordeDebug = new PdfPen(Color.Red, 1);

            //Dependencia
            PdfFont fontlabelMax = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 8f, PdfFontStyle.Bold);//Set the standard font.
            PdfFont fontlabelSub = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 6f, PdfFontStyle.Bold);//Set the standard font.
            PdfFont fontLabelData = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 6.5f);

            PdfStringFormat stringCentrado = new PdfStringFormat();   //Initialize PdfStringFormat and set the properties
            stringCentrado.Alignment = PdfTextAlignment.Center;
            stringCentrado.LineAlignment = PdfVerticalAlignment.Middle;

            PdfStringFormat stringIzquierda = new PdfStringFormat();   //Initialize PdfStringFormat and set the properties
            stringIzquierda.Alignment = PdfTextAlignment.Left;
            stringIzquierda.LineAlignment = PdfVerticalAlignment.Top;

            //Borde
            RectangleF recMarco = new RectangleF(12, 26, (float)315 * factor, (float)231 * factor); //Borde X-6 Y-7
            page.Graphics.DrawRectangle(borde, recMarco);

            string nomORG = "", nomDependecia = "", nomSubDependencia = "", rc_titulo1 = "", rc_titulo2 = "", rc_titulo3 = "", rc_cal_codigo = "", rc_cal_version = "", rc_cal_fecha = "", codDependencia = "", nomSubdependencia = "", codSubdependencia = "", aliasSubserie = "", nomSerie = "", codSerie = "", codSubserie = "", nomSubserie = "", marco = "", nombres = "", nroExp = "", nomExp = "", nroCaja = "", tomo = "";
            DateTime fechaInicial = DateTime.MaxValue, fechaFinal = DateTime.MinValue;
            int idProyecto = 0, existeMarcoL = 0, folioInicial = 0, folioFinal = 0, num_caja = 0, nro_carpeta = 1;

            //Consulta base de datos
            var datHC = EntitiesRepository.Entities.t_carpeta.Include("t_lote").Include("p_proyecto").Include("p_organizacion").Include("t_tercero").AsNoTracking()
                    .Where(f => f.id == idCarpeta)
                    .Select(p => new { p.t_lote.p_proyecto.p_organizacion, p.t_lote.p_subdependencia.p_dependencia, p.t_lote.p_subdependencia, p.t_tercero, p.t_lote.p_subserie.p_serie, p.t_lote.p_subserie, p.t_lote.marco, p.t_lote.id_proyecto, p.nom_expediente, p.nro_expediente, p.nro_caja, p.nro_carpeta, p.t_lote.p_proyecto, p.fecha_expediente_ini, p.fecha_expediente_fin, p.kp_folioini, p.kp_foliofin, p.tomo, p.int_caja });

            var dataFormato = datHC.FirstOrDefault();
            if (dataFormato != null)
            {
                string archivadoPor = dataFormato.p_proyecto.nom_proyecto;
                idProyecto = dataFormato.p_proyecto.id;
                nomORG = dataFormato.p_organizacion.nombre ?? "";
                nomDependecia = dataFormato.p_dependencia.nombre ?? "";
                nomSubDependencia = dataFormato.p_subdependencia.nombre ?? "";
                nomSerie = dataFormato.p_serie.nombre ?? "";
                nomSubserie = dataFormato.p_subserie.nombre ?? "";
                aliasSubserie = dataFormato.p_subserie.alias_subserie ?? "";
                nroExp = dataFormato.nro_expediente ?? "";
                nomExp = dataFormato.nom_expediente ?? "";
                nro_carpeta = GlobalClass.GetNumber(dataFormato.nro_carpeta.ToString(), 1);
                DateTime.TryParse(dataFormato.fecha_expediente_ini.ToString(), out fechaInicial);
                DateTime.TryParse(dataFormato.fecha_expediente_fin.ToString(), out fechaFinal);
                folioInicial = GlobalClass.GetNumber(dataFormato.kp_folioini.ToString(), 1); if (folioInicial == 0) folioInicial = 1;
                folioFinal = GlobalClass.GetNumber(dataFormato.kp_foliofin.ToString());
                tomo = dataFormato.tomo ?? "";
                num_caja = dataFormato.int_caja ?? 0;
            }
            string pathRoot;
            pathRoot = Path.GetPathRoot("logo_" + idProyecto + ".png");

            ////////LOGO
            if (File.Exists("logo_" + idProyecto + ".png"))
            {
                PdfBitmap image = new PdfBitmap("logo_" + idProyecto + ".png");
                SizeF pageSize = page.GetClientSize();  //Width = 712 Height = 532  Console.WriteLine(pageSize.Width + " - " + pageSize.Height);
                RectangleF imageBounds = new RectangleF(15, 38, 80, factor * 16);//Setting image bounds
                graphics.DrawImage(image, imageBounds);//Draw the image
            }

            buildRectangleLabel(12, 26, recMarco.Width, factor * 40, nomORG, fontlabelMax, stringCentrado, page, sinBorde, "", stringCentrado, fontLabelData);
            buildRectangleLabel(12, factor * 66, recMarco.Width, factor * 26, "DEPENDENCIA / FONDO: ", fontlabelSub, stringIzquierda, page, borde, $@"{nomSubDependencia} / {nomORG}", stringCentrado, fontLabelData);
            buildRectangleLabel(12, factor * 92, recMarco.Width, factor * 26, "SERIE / SUBSERIE:", fontlabelSub, stringIzquierda, page, borde, $@"{nomSerie}/{nomSubserie}", stringCentrado, fontLabelData);
            buildRectangleLabel(12, factor * 118, recMarco.Width, factor * 26, "NOMBRE EXPEDIENTE:", fontlabelSub, stringIzquierda, page, borde, nomExp, stringCentrado, fontLabelData);
            buildRectangleLabel(12, factor * 144, recMarco.Width, factor * 29, "ASUNTO:", fontlabelSub, stringIzquierda, page, borde, $@"{aliasSubserie} / {nomExp}", stringCentrado, fontLabelData);
            buildRectangleLabel(12, factor * 173, recMarco.Width / 3, factor * 28, "FECHA INICIAL:", fontlabelSub, stringIzquierda, page, borde, fechaInicial.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), stringCentrado, fontLabelData);
            buildRectangleLabel(12, factor * 201, recMarco.Width / 3, factor * 28, "FOLIO INICIAL:", fontlabelSub, stringIzquierda, page, borde, folioInicial.ToString(), stringCentrado, fontLabelData);
            buildRectangleLabel(12, factor * 229, recMarco.Width / 3, factor * 28, "NO. CAJA:", fontlabelSub, stringCentrado, page, borde, "", stringCentrado, fontLabelData);

            buildRectangleLabel(117, factor * 173, recMarco.Width / 3, factor * 28, "FECHA FINAL:", fontlabelSub, stringIzquierda, page, borde, fechaFinal.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), stringCentrado, fontLabelData);
            buildRectangleLabel(117, factor * 201, recMarco.Width / 3, factor * 28, "FOLIO FINAL:", fontlabelSub, stringIzquierda, page, borde, folioFinal.ToString(), stringCentrado, fontLabelData);
            buildRectangleLabel(117, factor * 229, recMarco.Width / 3, factor * 28, "", fontlabelSub, stringIzquierda, page, borde, num_caja.ToString(), stringCentrado, fontLabelData);

            buildRectangleLabel(222, factor * 173, recMarco.Width / 3, factor * 28, "NO. CARPETA:", fontlabelSub, stringIzquierda, page, borde, nro_carpeta.ToString(), stringCentrado, fontLabelData);
            buildRectangleLabel(222, factor * 201, recMarco.Width / 3, factor * 28, "TOMO:", fontlabelSub, stringIzquierda, page, borde, tomo, stringCentrado, fontLabelData);
            buildRectangleLabel(222, factor * 229, recMarco.Width / 3, factor * 28, "RESERVA O CLASIFICACIÓN", fontlabelSub, stringIzquierda, page, borde, "SI / NO", stringCentrado, fontLabelData);

            //Save the document.
            string filename = $"{exportPath}" + "/RKP_" + idCarpeta + ".pdf";
            doc.Save(filename);
            exportedList.Add(filename);
            //Close the document.
            doc.Close(true);
        }

        public void ExportPdfRotuloCarpeta5(int idCarpeta, string exportPath, ref List<string> exportedList, p_formato mipFormato, int consecutivoKP = 1)
        {
            string rc_titulo1 = "", rc_titulo2 = "", rc_titulo3 = "", rc_cal_codigo = "", rc_cal_version = "", rc_cal_fecha = "", txtQR = "", nomDependencia = "", codDependencia = "", nomSubdependencia = "", codSubdependencia = "", nomSerie = "", codSerie = "", codSubserie = "", nomSubserie = "", marco = "", nombres = "", nroExp = "", nomExp = "", nroCaja = "", tomo = "";
            int idProyecto = 0, existeMarcoL = 0, folioInicial = 0, folioFinal = 0; int? nroCarpeta = 0;
            DateTime fechaInicial = DateTime.MaxValue, fechaFinal = DateTime.MinValue;

            //Create a new PDF document.
            PdfDocument doc = new PdfDocument();
            float factor = 3;
            doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
            doc.PageSettings.Size = PdfPageSize.A5;
            doc.PageSettings.Size = new SizeF(100 * factor, 100 * factor);
            doc.PageSettings.Margins.Top = 2;
            doc.PageSettings.Margins.Left = 4;
            doc.PageSettings.Margins.Right = 0;
            doc.PageSettings.Margins.Bottom = 1;


            PdfPage page = doc.Pages.Add();//Add a page to the document.
            PdfGraphics graphics = page.Graphics;
            PdfPen borde = new PdfPen(Color.Black, 1);
            PdfPen bordeDebug = new PdfPen(Color.Red, 1);
            RectangleF recMarco = new RectangleF(1, 1, (float)97 * factor, (float)97 * factor); //Borde X-6 Y-7
            page.Graphics.DrawRectangle(borde, recMarco);
            RectangleF recparte1 = new RectangleF(1, 1, recMarco.Width, factor * 10); //Borde
            page.Graphics.DrawRectangle(borde, recparte1);

            //Consulta base de datos
            var datHC = EntitiesRepository.Entities.t_carpeta.Include("t_lote").Include("p_proyecto").Include("t_tercero").AsNoTracking()
                    .Where(f => f.id == idCarpeta)
                    .Select(p => new { p.t_tercero, p.t_lote.p_subserie, p.t_lote.marco, p.t_lote.id_proyecto, p.t_lote.p_subdependencia, p.nom_expediente, p.nro_expediente, p.nro_caja, p.nro_carpeta, p.t_lote.p_proyecto, p.fecha_expediente_ini, p.fecha_expediente_fin, p.kp_folioini, p.kp_foliofin, p.tomo, p.tomo_fin });

            var dataCarpeta = datHC.FirstOrDefault();
            if (dataCarpeta != null)
            {
                if (mipFormato.rc_titulo1 != null) rc_titulo1 = mipFormato.rc_titulo1;
                if (mipFormato.rc_titulo2 != null) rc_titulo2 = mipFormato.rc_titulo2;
                if (mipFormato.rc_titulo3 != null) rc_titulo3 = mipFormato.rc_titulo3;
                if (mipFormato.rc_cal_codigo != null) rc_cal_codigo = mipFormato.rc_cal_codigo;
                if (mipFormato.rc_cal_version != null) rc_cal_version = mipFormato.rc_cal_version;
                if (mipFormato.rc_cal_fecha != null) rc_cal_fecha = mipFormato.rc_cal_fecha;
                int.TryParse(mipFormato.rc_marco_legal.ToString(), out existeMarcoL);
                DateTime.TryParse(mipFormato.fecha_inicial_defecto.ToString(), out fechaInicial);//FECHAS EXTREMAS

                string archivadoPor = dataCarpeta.p_proyecto.nom_proyecto;
                idProyecto = dataCarpeta.p_proyecto.id;
                nomDependencia = dataCarpeta.p_subdependencia.p_dependencia.nombre;
                codDependencia = dataCarpeta.p_subdependencia.p_dependencia.codigo;
                nomSubdependencia = dataCarpeta.p_subdependencia.nombre;
                if (dataCarpeta.p_subdependencia.cod != null) codSubdependencia = dataCarpeta.p_subdependencia.cod;
                nomSerie = dataCarpeta.p_subserie.p_serie.nombre;
                codSerie = dataCarpeta.p_subserie.p_serie.codigo;
                if (dataCarpeta.p_subserie.codigo != null) codSubserie = dataCarpeta.p_subserie.codigo;
                nomSubserie = dataCarpeta.p_subserie.nombre;
                if (dataCarpeta.marco != null) marco = dataCarpeta.marco;
                nombres = dataCarpeta.t_tercero?.nombres + " " + dataCarpeta.t_tercero?.apellidos;
                //nomExp = dataCarpeta.nom_expediente.Replace(System.Environment.NewLine, "");

                DateTime.TryParse(dataCarpeta.fecha_expediente_ini.ToString(), out fechaInicial);
                DateTime.TryParse(dataCarpeta.fecha_expediente_fin.ToString(), out fechaFinal);
                //nroExp = dataCarpeta.nro_expediente;
                nroCaja = dataCarpeta.nro_caja;
                //if (dataCarpeta.nro_carpeta != null) nroCarpeta = dataCarpeta.nro_carpeta.ToString();
                //else 
                nroCarpeta = dataCarpeta.nro_carpeta;
                if (dataCarpeta.tomo != null) tomo = dataCarpeta.tomo.ToString();
                if (dataCarpeta.tomo_fin != null) tomo += " DE " + dataCarpeta.tomo_fin.ToString();
                else tomo += " DE " + dataCarpeta.tomo?.ToString();
                if (tomo == " DE ") tomo = "1 DE 1";

                folioInicial = GlobalClass.GetNumber(dataCarpeta.kp_folioini.ToString(), 1); if (folioInicial == 0) folioInicial = 1;
                folioFinal = GlobalClass.GetNumber(dataCarpeta.kp_foliofin.ToString());
            }

            IQueryable<t_documento> datListDocumento = EntitiesRepository.Entities.t_documento.IncludeOptimized(x => x.t_carpeta).IncludeOptimized(x => x.t_carpeta.t_lote).Where(p => p.t_carpeta.t_lote.id_proyecto == GlobalClass.id_proyecto && p.t_carpeta.nro_caja == nroCaja && p.t_carpeta.nro_carpeta == nroCarpeta);
            int idTercero = -1, id_doc_actual = -1, folioIni = -1, folioFin = -1, folioTotal = 0, folioIniActual = -1, folioFinActual = -1; DateTime? fecIni = DateTime.MinValue; DateTime? fecFin = DateTime.MinValue; DateTime? fecApertura = DateTime.MaxValue;
            string NomTercero = string.Empty, nomFUD = string.Empty, nomactualFUD = string.Empty, nom_exp = string.Empty, nomDocumento = string.Empty, rxtFechaIni = "S.F.", rxtFechaFin = "S.F.", numFUDone = string.Empty;
            foreach (var item in datListDocumento.Select(p => new { p.id, p.id_carpeta, p.p_tipodoc.nombre, p.p_tipodoc.principal, p.t_carpeta.id_tercero, p.nro_doc, p.observacion, p.t_carpeta.nro_caja, p.t_carpeta.nro_carpeta, p.t_carpeta.nro_expediente, p.folio_ini, p.folio_fin, p.pag_ini, p.fecha }).OrderBy(x => x.id_carpeta).ThenBy(x => x.pag_ini).ToList())
            {
                nomDocumento = item.nombre.ToUpper();
                folioIniActual = GlobalClass.GetNumber(item.folio_ini?.ToString(), -1);
                folioFinActual = GlobalClass.GetNumber(item.folio_fin?.ToString(), -1);
                //SI ES DOCUMENTO PRINCIPAL DE FUD
                if (nomDocumento.Contains("FUD") || nomDocumento.Contains("NOVEDAD") || nomDocumento.Contains("APELACI"))
                {
                    if (id_doc_actual != -1)
                    {
                        //nomFUD = item.nro_doc?.ToString();
                        folioTotal += folioFin - folioIni + 1;

                        folioIni = -1;
                        folioFin = -1;
                    }
                    numFUDone = "NO ENCONTRADO";
                    //Numeros de FUD
                    if (string.IsNullOrEmpty(item.nro_doc) && !item.nro_expediente.ToString().Contains("KP")) numFUDone = item.nro_expediente.ToString().Trim();
                    else numFUDone = item.nro_doc?.ToString().Trim();

                    if (!nomExp.Contains(numFUDone))
                    {
                        nomExp += " - " + numFUDone;
                        //Cédulas de Terceros
                        idTercero = GlobalClass.GetNumber(item.id_tercero?.ToString());
                        nroExp += " - " + getTercero(item.id, idTercero, true);
                    }

                    //Calcula FECHAS Extremas
                    if (item.fecha != DateTime.MinValue && item.principal != 0)
                    {
                        if (fecIni == DateTime.MinValue) fecIni = item.fecha;
                        fecFin = item.fecha;
                    }
                }


                //Calcula Fecha de apertura
                //Calcula FOLIOS
                if (folioIniActual != -1 && folioIni == -1) folioIni = folioIniActual;
                if (folioFinActual != -1) folioFin = folioFinActual;

                id_doc_actual = item.id;
            }

            if (folioIni != -1)
            {
                folioTotal += folioFin - folioIni + 1;
            }
            //table.Rows.Add(new string[] { serial.ToString(), codSubdepen, codSerie, codSubserie, nom_exp, rxtFechaIni, rxtFechaFin, "X", "", "", folioIni.ToString(), folioFin.ToString(), nro_caja, nro_carpeta, "FISICO", "MEDIA", observacion });


            ////////LOGO
            if (File.Exists("logo_" + idProyecto + ".png"))
            {
                PdfBitmap image = new PdfBitmap("logo_" + idProyecto + ".png");
                SizeF pageSize = page.GetClientSize();  //Width = 712 Height = 532  Console.WriteLine(pageSize.Width + " - " + pageSize.Height);
                RectangleF imageBounds = new RectangleF(factor * 2, factor * 2, (float)19 * factor, (float)5 * factor);//Setting image bounds
                graphics.DrawImage(image, imageBounds);//Draw the image
            }
            PdfLinearGradientBrush brush = new PdfLinearGradientBrush(new PointF(0, 0), new PointF(1, 1), Color.Red, Color.Blue);
            brush.Background = Color.FromArgb(51, 102, 204);
            ////////TITULO
            PdfStringFormat formatoTxtCentrado = new PdfStringFormat(); formatoTxtCentrado.Alignment = PdfTextAlignment.Center;
            PdfStringFormat formatoTxtIzquierda = new PdfStringFormat(); formatoTxtIzquierda.Alignment = PdfTextAlignment.Left;
            PdfFont fontNegrita = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 2, PdfFontStyle.Bold);//Set the standard font.
            PdfFont fontMin = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.9f);//Set the standard font.
            PdfFont fontNegritaMin = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.8f, PdfFontStyle.Bold);//Set the standard font.
            PdfFont fontX1_5 = new PdfStandardFont(PdfFontFamily.Helvetica, factor * (float)2.2);//Set the standard font.
            PdfFont fontNegritaX1_5 = new PdfStandardFont(PdfFontFamily.Helvetica, factor * (float)2.2, PdfFontStyle.Bold);//Set the standard font.
            //Rectangulos titulo central
            RectangleF recTitulo = new RectangleF(factor * 23.23f, 1, factor * 51, factor * 10); //Borde
            page.Graphics.DrawRectangle(borde, recTitulo);
            RectangleF recTitulo1 = new RectangleF(factor * 23.23f, 1, factor * 51, factor * 3.4f); //Borde
            graphics.DrawRectangle(borde, recTitulo1);
            graphics.DrawRectangle(brush, recTitulo1);
            RectangleF recTitulo2 = new RectangleF(factor * 23.23f, 1, factor * 51, factor * 6.6f); //Borde
            graphics.DrawRectangle(borde, recTitulo2);

            graphics.DrawString(rc_titulo1, fontNegrita, PdfBrushes.White, new PointF(factor * 49, factor * 1), formatoTxtCentrado);//Draw the text.
            graphics.DrawString(rc_titulo2, fontNegrita, PdfBrushes.Black, new PointF(factor * 49, factor * 4), formatoTxtCentrado);//Draw the text.
            graphics.DrawString(rc_titulo3, fontNegrita, PdfBrushes.Black, new PointF(factor * 49, factor * 7.5f), formatoTxtCentrado);//Draw the text.

            ////////CALIDAD
            PdfFont fontFormatoNegrita = new PdfStandardFont(PdfFontFamily.Helvetica, factor * (float)1.7, PdfFontStyle.Bold);
            graphics.DrawString(rc_cal_codigo, fontFormatoNegrita, PdfBrushes.Black, new PointF(recTitulo.X + recTitulo.Width + 4, factor * 0.8f), formatoTxtIzquierda);
            graphics.DrawString(rc_cal_version, fontFormatoNegrita, PdfBrushes.Black, new PointF(recTitulo.X + recTitulo.Width + 4, factor * 3.2f), formatoTxtIzquierda);
            graphics.DrawString(rc_cal_fecha, fontFormatoNegrita, PdfBrushes.Black, new PointF(recTitulo.X + recTitulo.Width + 4, factor * 5.6f), formatoTxtIzquierda);
            graphics.DrawString("Página 1 de 1", fontFormatoNegrita, PdfBrushes.Black, new PointF(recTitulo.X + recTitulo.Width + 4, factor * 8f), formatoTxtIzquierda);

            //Dependencia
            PdfFont fontlabel = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 2);//Set the standard font.
            PdfFont fontlabelMin = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.9f);//Set the standard font.
            PdfFont fontlabelMin2 = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.8f);//Set the standard font.
            PdfFont fontlabelMinMin = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.6f);//Set the standard font.
            PdfFont fontlabelMinMinMin = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.3f);//Set the standard font.
            PdfStringFormat stringCentrado = new PdfStringFormat();   //Initialize PdfStringFormat and set the properties
            stringCentrado.Alignment = PdfTextAlignment.Center;
            stringCentrado.LineAlignment = PdfVerticalAlignment.Middle;
            PdfStringFormat stringIzquierda = new PdfStringFormat();   //Initialize PdfStringFormat and set the properties
            stringIzquierda.Alignment = PdfTextAlignment.Left;
            stringIzquierda.LineAlignment = PdfVerticalAlignment.Middle;

            PdfTextElement element = new PdfTextElement("SECCIÓN - UNIDAD ADMINISTRATIVA");  //Create a text element
            element.Font = fontlabelMinMin;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            PdfLayoutFormat layoutFormat = new PdfLayoutFormat();
            layoutFormat.Break = PdfLayoutBreakType.FitPage;
            RectangleF bounds = new RectangleF(new PointF(factor * 1, factor * 11), new SizeF(factor * 18, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //LINEA
            PointF point1 = new PointF(factor * 20, bounds.Y + bounds.Height + 1);
            PointF point2 = new PointF(factor * 71, point1.Y);
            PdfPen pen = new PdfPen(PdfBrushes.Black, 0.8f);//Initialize pen to draw the line
            PdfPen penRed = new PdfPen(PdfBrushes.Red, 0.8f);//Initialize pen to draw the line
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //DATO
            element = new PdfTextElement(nomDependencia);  //Create a text element
            element.Font = fontlabelMin2;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, factor * 11), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            element = new PdfTextElement("CÓDIGO");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point2.X + (float)1, factor * 11), new SizeF(factor * 19, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Código
            point1 = new PointF(factor * 80, point1.Y);
            point2 = new PointF(factor * 97, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            element = new PdfTextElement(codDependencia);  //Create a text element
            element.Font = fontlabelMin2;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, factor * 11), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //Ofc Productora
            element = new PdfTextElement("SUBSECCIÓN - OFICINA PRODUCTORA");  //Create a text element
            element.Font = fontlabelMinMin;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 1, bounds.Y + bounds.Height + 1), new SizeF(factor * 19, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //LINEA
            point1 = new PointF(factor * 20, bounds.Y + bounds.Height + 1);
            point2 = new PointF(factor * 71, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //DATO
            element = new PdfTextElement(nomSubdependencia);  //Create a text element
            element.Font = fontlabelMin2;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, bounds.Y), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            element = new PdfTextElement("CÓDIGO");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point2.X + (float)1, bounds.Y), new SizeF(factor * 19, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Linea
            point1 = new PointF(factor * 80, point1.Y);
            point2 = new PointF(factor * 97, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //Dato
            element = new PdfTextElement(codSubdependencia);  //Create a text element
            element.Font = fontlabelMin2;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X + 1, bounds.Y), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //Serie
            element = new PdfTextElement("SERIE");  //Create a text element
            element.Font = fontlabelMinMin;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 1, bounds.Y + bounds.Height + 1), new SizeF(factor * 19, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Línea
            point1 = new PointF(factor * 20, bounds.Y + bounds.Height + 1);
            point2 = new PointF(factor * 71, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //Dato
            element = new PdfTextElement(nomSerie);  //Create a text element
            element.Font = fontlabelMin2;
            element.StringFormat = stringCentrado;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, bounds.Y + 1), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            element = new PdfTextElement("CÓDIGO");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point2.X + (float)1, bounds.Y), new SizeF(factor * 19, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Línea
            point1 = new PointF(factor * 80, point1.Y);
            point2 = new PointF(factor * 97, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //Dato
            element = new PdfTextElement(codSerie);  //Create a text element
            element.Font = fontlabelMin2;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X + 1, bounds.Y), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //SubSerie
            element = new PdfTextElement("SUBSERIE");  //Create a text element
            element.Font = fontlabelMinMin;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 1, bounds.Y + bounds.Height + 2), new SizeF(factor * 19, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Línea
            point1 = new PointF(factor * 20, bounds.Y + bounds.Height + 1);
            point2 = new PointF(factor * 71, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //Dato
            element = new PdfTextElement(nomSubserie);  //Create a text element
            element.Font = fontlabelMin2;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, bounds.Y), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            element = new PdfTextElement("CÓDIGO");  //Create a text element
            element.Font = fontlabelMin2;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point2.X + (float)1, bounds.Y), new SizeF(factor * 19, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Línea
            point1 = new PointF(factor * 80, point1.Y);
            point2 = new PointF(factor * 97, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //Dato
            element = new PdfTextElement(codSubserie);  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X + (float)1, bounds.Y), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //Marco Legal
            element = new PdfTextElement("MARCO LEGAL");  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 1, bounds.Y + bounds.Height + 2), new SizeF(factor * 19, factor * 3));//Set bounds to draw multi-line text
            //if (existeMarcoL == 1) 
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Línea
            point1 = new PointF(factor * 20, bounds.Y + bounds.Height + 1);
            point2 = new PointF(factor * 97, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //Dato
            element = new PdfTextElement(marco);  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point1.X, bounds.Y), new SizeF(point2.X - point1.X, factor * 3));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //Nombre del Expediente
            element = new PdfTextElement("NOMBRE DEL EXPEDIENTE");  //Create a text element
            element.Font = fontlabelMin;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 1, bounds.Y + bounds.Height + 2), new SizeF(factor * 19, factor * 11));//Set bounds to draw multi-line text
            //if (existeMarcoL != 1) bounds.Y = bounds.Y - 4;
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //if (existeMarcoL != 1) bounds.Y = bounds.Y + 4;
            //Dato
            element = new PdfTextElement(nomExp);  //Create a text element //'nombres' si se requiere poner nombres y apellidos
            element.Font = fontMin;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 20, bounds.Y), new SizeF(recMarco.Width - (factor * 21), factor * 11));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Linea
            point1 = new PointF(factor * 20, bounds.Y + bounds.Height + 1);
            point2 = new PointF(factor * 97, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //CUADRO SECCIÓN UNO
            RectangleF recSeccion1 = new RectangleF(1, 1, recMarco.Width, bounds.Y + 12);
            /*RectangleF recSeccion1_v2 = new RectangleF(1, 1, recMarco.Width, bounds.Y + 29);
            page.Graphics.DrawRectangle(borde, recSeccion1_v2);*/

            //SECCIÓN DOS
            //No Expediente
            element = new PdfTextElement("No. EXPEDIENTE");  //Create a text element
            element.Font = fontlabelMin;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 1, bounds.Y + bounds.Height + 2), new SizeF(factor * 19, factor * 11));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Dato
            element = new PdfTextElement(nroExp);  //Create a text element
            element.Font = fontlabel;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 20, bounds.Y), new SizeF(recMarco.Width - (factor * 21), factor * 11));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Linea
            point1 = new PointF(factor * 20, bounds.Y + bounds.Height + 1);
            point2 = new PointF(recMarco.Width, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document

            //Punto de control { X = 60 Y = 158 Width = 228 Height = 9}

            bounds = new RectangleF(new PointF(60, 172), new SizeF(228, 9));

            //
            //Fechas EXTREMAS
            element = new PdfTextElement("FECHA INICIAL");  //Create a text element
            element.Font = fontlabelMin;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 1, bounds.Y + 10), new SizeF(factor * 16, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Linea
            point1 = new PointF(factor * 16, point1.Y + 13);
            point2 = new PointF(factor * 32, point1.Y);
            //page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //Dato
            DateTime fMin = DateTime.MinValue;
            string rxtFecha = "S.F.";
            var fechaMaxSistema = new DateTime(DateTime.MaxValue.Year, DateTime.MaxValue.Month, DateTime.MaxValue.Day, DateTime.MaxValue.Hour, DateTime.MaxValue.Minute, DateTime.MaxValue.Second);
            rxtFecha = fecIni?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            if (rxtFecha == null) rxtFecha = "S.F.";
            element = new PdfTextElement(rxtFecha);  //Create a text element
            element.Font = fontX1_5;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 16, bounds.Y), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Linea
            point1 = new PointF(factor * 16, bounds.Y + bounds.Height - (1 * factor));
            point2 = new PointF(factor * 32, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document

            element = new PdfTextElement("FECHA FINAL");  //Create a text element
            element.Font = fontlabelMin;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 33, bounds.Y), new SizeF(factor * 16, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            point1 = new PointF(factor * 48, point1.Y);
            point2 = new PointF(factor * 64, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            rxtFecha = "S.F.";
            rxtFecha = fecFin?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            if (rxtFecha == null) rxtFecha = "S.F.";
            element = new PdfTextElement(rxtFecha);  //Create a text element
            element.Font = fontX1_5;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 48, bounds.Y), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //Fechas Carpeta - Correlativo
            element = new PdfTextElement("CARPETA No.");  //Create a text element
            element.Font = fontlabelMin;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 1, bounds.Y + 10), new SizeF(factor * 16, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Dato
            element = new PdfTextElement($@"KP{nroCarpeta}");  //Create a text element
            element.Font = fontX1_5;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 16, bounds.Y), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Linea
            point1 = new PointF(factor * 16, bounds.Y + bounds.Height - (1 * factor));
            point2 = new PointF(factor * 32, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document

            element = new PdfTextElement("CORRELATIVO");  //Create a text element
            element.Font = fontlabelMin;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 33, bounds.Y), new SizeF(factor * 16, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Linea
            point1 = new PointF(factor * 48, point1.Y);
            point2 = new PointF(factor * 64, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //Dato
            element = new PdfTextElement(tomo);  //Create a text element
            element.Font = fontX1_5;
            int lenTomo = tomo.Length;
            if (lenTomo > 10) element.Font = fontMin;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 48, bounds.Y), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //Folios
            element = new PdfTextElement("FOLIOS");  //Create a text element
            element.Font = fontlabelMin;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 1, bounds.Y + 10), new SizeF(factor * 16, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            folioInicial = 1;
            folioFinal = folioTotal;
            //Dato
            string txtFolios = folioInicial + " A " + folioFinal;
            int total_folio = 0;
            if (folioFinal == 0) txtFolios = "0 A 0";
            else total_folio = folioFinal - folioInicial + 1;
            element = new PdfTextElement(txtFolios);  //Create a text element //fontNegritaX1_5 //fontNegrita
            if (txtFolios.Length > 15)
            {
                element.Font = fontNegritaMin;
            }
            else if (txtFolios.Length > 8)
            {
                element.Font = fontNegritaMin;

            }
            else
            {
                element.Font = fontX1_5;
            }
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 16, bounds.Y), new SizeF(point2.X - point1.X, factor * 5));//Set bounds to draw multi-line text
            //element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Linea
            point1 = new PointF(factor * 16, bounds.Y + bounds.Height - (1 * factor));
            point2 = new PointF(factor * 32, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document

            //Titulo total folios
            element = new PdfTextElement("TOTAL FOLIOS");  //Create a text element
            element.Font = fontlabelMin;
            element.StringFormat = stringIzquierda;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(point2.X + 1, bounds.Y), new SizeF(factor * 16, factor * 4));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set
            //Linea Correlativo
            point1 = new PointF(factor * 48, point1.Y);
            point2 = new PointF(factor * 64, point1.Y);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //Dato total Folios
            element = new PdfTextElement(total_folio.ToString());  //Create a text element
            element.Font = fontX1_5;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            bounds = new RectangleF(new PointF(factor * 49, bounds.Y), new SizeF(factor * 13.5f, factor * 5));//Set bounds to draw multi-line text
            element.Draw(page, bounds, layoutFormat);//Draw the text element with the properties and formats set

            //SIGNATURA TOPOGRAFICA {X = 3 Y = 211 Width = 184.5 Height = 27}

            var recSeccion3 = new PointF(factor * 64, point1.Y + 5);

            RectangleF recSigna = new RectangleF(factor * 1f, recSeccion3.Y, factor * 61.5f, factor * 9); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //Cuadro ubicación
            recSigna = new RectangleF(factor * 1f, recSeccion3.Y, factor * 10f, factor * 9); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //TXT Cuerpo
            element = new PdfTextElement("UBICACIÓN TOPOGRÁFICA");  //Create a text element
            element.Font = fontlabelMinMinMin;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, recSigna, layoutFormat);
            //Ajuste para el Y del cuadro 
            //point1.Y += + 
            //Cuadro Bodega
            recSigna = new RectangleF(factor * 1f, recSeccion3.Y, factor * 17f, factor * 9); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //recTitulo Bodega
            recSigna = new RectangleF(factor * 11f, recSeccion3.Y, factor * 7f, factor * 3); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //Fondo titulo bodega
            brush = new PdfLinearGradientBrush(new PointF(0, 0), new PointF(1, 1), Color.Red, Color.Blue);
            brush.Background = Color.FromArgb(226, 226, 226);
            recSigna.Width = recSigna.Width - 1;
            recSigna.X = recSigna.X + 0.5f;
            recSigna.Y = recSigna.Y - 0.5f;
            graphics.DrawRectangle(brush, recSigna);
            //TXT Bodega
            PdfFont fontTituloCuadro = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.4F, PdfFontStyle.Bold);//Set the standard font.
            element = new PdfTextElement("BODEGA");  //Create a text element
            element.Font = fontTituloCuadro;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, recSigna, layoutFormat);

            //rec CUERPO
            recSigna = new RectangleF(factor * 1f, recSeccion3.Y, factor * 24f, factor * 9); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //recTitulo TORRE
            recSigna = new RectangleF(factor * 18f, recSeccion3.Y, factor * 7f, factor * 3); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //FOndo titulo torre
            recSigna.Width = recSigna.Width - 1;
            recSigna.X = recSigna.X + 0.5f;
            recSigna.Y = recSigna.Y - 0.5f;
            graphics.DrawRectangle(brush, recSigna);
            //TXT Torre
            fontTituloCuadro = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.4F, PdfFontStyle.Bold);//Set the standard font.
            element = new PdfTextElement("TORRE");  //Create a text element
            element.Font = fontTituloCuadro;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, recSigna, layoutFormat);

            recSigna = new RectangleF(factor * 1f, recSeccion3.Y, factor * 31f, factor * 9); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //recTitulo CUERPO
            recSigna = new RectangleF(factor * 25f, recSeccion3.Y, factor * 7f, factor * 3); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //FOndo titulo Cuerpo
            recSigna.Width = recSigna.Width - 1;
            recSigna.X = recSigna.X + 0.5f;
            recSigna.Y = recSigna.Y - 0.5f;
            graphics.DrawRectangle(brush, recSigna);
            //TXT Cuerpo
            fontTituloCuadro = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.4F, PdfFontStyle.Bold);//Set the standard font.
            element = new PdfTextElement("CUERPO");  //Create a text element
            element.Font = fontTituloCuadro;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, recSigna, layoutFormat);

            recSigna = new RectangleF(factor * 1f, recSeccion3.Y, factor * 38f, factor * 9); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //recTitulo PISO
            recSigna = new RectangleF(factor * 32f, recSeccion3.Y, factor * 7f, factor * 3); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //FOndo titulo Piso
            recSigna.Width = recSigna.Width - 1;
            recSigna.X = recSigna.X + 0.5f;
            recSigna.Y = recSigna.Y - 0.5f;
            graphics.DrawRectangle(brush, recSigna);
            //TXT Cuerpo
            fontTituloCuadro = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.4F, PdfFontStyle.Bold);//Set the standard font.
            element = new PdfTextElement("PISO");  //Create a text element
            element.Font = fontTituloCuadro;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, recSigna, layoutFormat);

            recSigna = new RectangleF(factor * 1f, recSeccion3.Y, factor * 45f, factor * 9); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //recTitulo NIVEL
            recSigna = new RectangleF(factor * 39f, recSeccion3.Y, factor * 7f, factor * 3); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //FOndo titulo Nivel
            recSigna.Width = recSigna.Width - 1;
            recSigna.X = recSigna.X + 0.5f;
            recSigna.Y = recSigna.Y - 0.5f;
            graphics.DrawRectangle(brush, recSigna);
            //TXT Cuerpo
            fontTituloCuadro = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.4F, PdfFontStyle.Bold);//Set the standard font.
            element = new PdfTextElement("NIVEL");  //Create a text element
            element.Font = fontTituloCuadro;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, recSigna, layoutFormat);

            recSigna = new RectangleF(factor * 1f, recSeccion3.Y, factor * 52f, factor * 9); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //recTitulo PASILLO
            recSigna = new RectangleF(factor * 46f, recSeccion3.Y, factor * 7f, factor * 3); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //FOndo titulo Pasillo
            recSigna.Width = recSigna.Width - 1;
            recSigna.Height = recSigna.Height - 1;
            recSigna.X = recSigna.X + 0.5f;
            recSigna.Y = recSigna.Y + 0.5f;
            graphics.DrawRectangle(brush, recSigna);
            //TXT Cuerpo
            fontTituloCuadro = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.4F, PdfFontStyle.Bold);//Set the standard font.
            element = new PdfTextElement("PASILLO");  //Create a text element
            element.Font = fontTituloCuadro;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, recSigna, layoutFormat);

            //recTitulo POSICION CJ
            recSigna = new RectangleF(factor * 53f, recSeccion3.Y, factor * 9.5f, factor * 3); //Borde
            page.Graphics.DrawRectangle(borde, recSigna);
            //FOndo titulo POSICION CJ
            recSigna.Width = recSigna.Width - 1;
            recSigna.Height = recSigna.Height - 1;
            recSigna.X = recSigna.X + 0.5f;
            recSigna.Y = recSigna.Y + 0.5f;
            graphics.DrawRectangle(brush, recSigna);
            //TXT Cuerpo
            fontTituloCuadro = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 1.4F, PdfFontStyle.Bold);//Set the standard font.
            element = new PdfTextElement("POSICION CJ");  //Create a text element
            element.Font = fontTituloCuadro;
            element.StringFormat = stringCentrado;
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, recSigna, layoutFormat);

            graphics.DrawString("CAJA No.", fontlabel, PdfBrushes.Black, new PointF(factor * 5, recSigna.Y + 41), formatoTxtCentrado);
            //Codigo de Barras CAJA
            PdfCode39Barcode barcode = new PdfCode39Barcode();//Drawing Code39 barcode
            barcode.BarHeight = 30;//Setting height of the barcode
            PdfFont fontBarcode = new PdfStandardFont(PdfFontFamily.Helvetica, factor * 2.6f, PdfFontStyle.Bold);//Set the standard font.
            barcode.Font = fontBarcode;
            barcode.Text = nroCaja.PadLeft(5, '0');
            barcode.Size = new SizeF(factor * 50, factor * 14);
            barcode.Draw(page, new PointF(factor * 10, recSigna.Y + 30));//Printing barcode on to the Pdf.

            /////////////QR/////////////
            //string[] wordsNumExp = nroExp.Split('-');
            //string[] wordsNomExp = nomExp.Split('-');

            //txtQR = "NA";
            //for (int cp = 0; cp < wordsNumExp.Length; cp++)
            //{
            //    if (!string.IsNullOrEmpty(wordsNumExp[cp].Trim()))
            //    {
            //        if (txtQR == "NA") txtQR = $@"{wordsNomExp[cp]?.Trim()} {wordsNumExp[cp]?.Trim()} - ";
            //        else txtQR += $@"{wordsNomExp[cp]?.Trim()} {wordsNumExp[cp]?.Trim()} - ";
            //    }
            //} System.Environment.NewLine

            txtQR = nomExp?.Trim();

            //if (!string.IsNullOrEmpty(tomo)) txtQR += " TOMO: " + tomo;
            txtQR = txtQR.Replace('Á', 'A');
            txtQR = txtQR.Replace('É', 'E');
            txtQR = txtQR.Replace('Í', 'I');
            txtQR = txtQR.Replace('Ó', 'O');
            txtQR = txtQR.Replace('Ú', 'U');
            txtQR = txtQR.Replace('Ñ', 'N');
            txtQR = Regex.Replace(txtQR, @"[^0-9a-zA-Z-]+", " ");    //[^0-9a-zA-Z:\u00C0-\u00FF,|._-Ññ]+
            int lonTxtExp = txtQR.Length; Console.WriteLine(txtQR); Console.WriteLine(lonTxtExp);
            if (lonTxtExp > 200) txtQR = txtQR.Substring(0, Math.Min(lonTxtExp, 200)); Console.WriteLine(lonTxtExp);

            //PdfQRBarcode barcodeQr = new PdfQRBarcode();//Drawing QR Barcode
            //barcodeQr.ErrorCorrectionLevel = PdfErrorCorrectionLevel.Low;//Set Error Correction Level
            ////if (lonTxtExp > 120) barcodeQr.ErrorCorrectionLevel = PdfErrorCorrectionLevel.Low;
            //barcodeQr.XDimension = 3;//Set XDimension
            //barcodeQr.Size = new SizeF(factor * 36f, factor * 36f);
            //barcodeQr.Text = txtQR; //barcodeQr.Text = Regex.Replace(txtQR, @"[^0-9a-zA-Z:,|._-Ññ]+", " ");
            //barcodeQr.Draw(page, new PointF(62f * factor, recSeccion3.Y - (factor * 12f)));//Printing barcode on to the Pdf.

            //QR NUEVA VERSIÓN
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeGenerator.ECCLevel nivelQR = QRCodeGenerator.ECCLevel.M;
            if (lonTxtExp > 120) nivelQR = QRCodeGenerator.ECCLevel.L;
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(txtQR, nivelQR);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(21);
            PdfBitmap image2 = new PdfBitmap(qrCodeImage);
            RectangleF imageBounds2 = new RectangleF(63f * factor, recSeccion3.Y - (factor * 12f), factor * 34f, factor * 34f);//Setting image bounds
            graphics.DrawImage(image2, imageBounds2);//Draw the image

            //Save the document.
            string filename = $"{exportPath}" + "/RKP_" + idCarpeta + ".pdf";
            doc.Save(filename);
            //Close the document.
            doc.Close(true);
            var match = exportedList.FirstOrDefault(stringToCheck => stringToCheck.Contains(filename));

            // in this case out testItem.Id (1) is equal to an item in the list
            if (match == null)
            {
                exportedList.Add(filename);
            }
        }

        public void ExportPdfRotuloCarpeta6(int idCarpeta, string exportPath, ref List<string> exportedList, p_formato mipFormato, int consecutivoKP = 1)
        {
        
            PdfDocument doc = new PdfDocument();
            doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
            doc.PageSettings.Size = PdfPageSize.A5;
            doc.PageSettings.Size = new SizeF(80, 60); //MEDIDAS EN MILIMETROS DEL LIENSO
            doc.PageSettings.Margins.Top = 1;
            doc.PageSettings.Margins.Left = 1;
            doc.PageSettings.Margins.Right = 1;
            doc.PageSettings.Margins.Bottom = 1;


            PdfPage page = doc.Pages.Add();//Add a page to the document.
            PdfGraphics graphics = page.Graphics;
            PdfPen borde = new PdfPen(Color.Black, 0.5f);
            PdfPen bordeDebug = new PdfPen(Color.Red, 1);
            RectangleF recMarco = new RectangleF(0, 0, 78 , 58 ); //Borde X-6 Y-7
            page.Graphics.DrawRectangle(borde, recMarco);
            RectangleF recparte1 = new RectangleF(0, 0, recMarco.Width, 10); //Borde
            page.Graphics.DrawRectangle(borde, recparte1);



            ////////LOGO FONDO ADAPTACION
           
            if (File.Exists("logo_" + GlobalClass.id_proyecto + ".png"))
            {
                PdfBitmap image = new PdfBitmap("logo_" + GlobalClass.id_proyecto + ".png");
                SizeF pageSize = page.GetClientSize();  //Width = 712 Height = 532  Console.WriteLine(pageSize.Width + " - " + pageSize.Height);
                RectangleF imageBounds = new RectangleF(2, 2, (float)19, (float)5);//Setting image bounds
                graphics.DrawImage(image, imageBounds);//Draw the image
            }
            PdfLinearGradientBrush brush = new PdfLinearGradientBrush(new PointF(0, 0), new PointF(1, 1), Color.Red, Color.Blue);
            brush.Background = Color.FromArgb(51, 102, 204);


            ////////LOGO MINISTARIO
            if (File.Exists("logo2_" + GlobalClass.id_proyecto + ".png"))
            {
                PdfBitmap image = new PdfBitmap("logo2_" + GlobalClass.id_proyecto + ".png");
                SizeF pageSize = page.GetClientSize();  //Width = 712 Height = 532  Console.WriteLine(pageSize.Width + " - " + pageSize.Height);
                RectangleF imageBounds = new RectangleF(58, 2, (float)19, (float)5);//Setting image bounds
                graphics.DrawImage(image, imageBounds);//Draw the image
            }
            PdfLinearGradientBrush brushE = new PdfLinearGradientBrush(new PointF(0, 0), new PointF(1, 1), Color.Red, Color.Blue);
            brushE.Background = Color.FromArgb(51, 102, 204);
            ////////TITULO
            PdfStringFormat formatoTxtCentrado = new PdfStringFormat(); formatoTxtCentrado.Alignment = PdfTextAlignment.Center;
            PdfStringFormat formatoTxtIzquierda = new PdfStringFormat(); formatoTxtIzquierda.Alignment = PdfTextAlignment.Left;
            PdfFont fontNegrita = new PdfStandardFont(PdfFontFamily.Helvetica, 2, PdfFontStyle.Bold);//Set the standard font.
            PdfFont fontMin = new PdfStandardFont(PdfFontFamily.Helvetica, 1.9f);//Set the standard font.
            PdfFont fontNegritaMin = new PdfStandardFont(PdfFontFamily.Helvetica, 1.8f, PdfFontStyle.Bold);//Set the standard font.
            PdfFont fontX1_5 = new PdfStandardFont(PdfFontFamily.Helvetica, (float)2.2);//Set the standard font.
            PdfFont fontNegritaX1_5 = new PdfStandardFont(PdfFontFamily.Helvetica, (float)2.2, PdfFontStyle.Bold);//Set the standard font.



           /// graphics.DrawString("Hello George!!!", fontNegrita, PdfBrushes.Black, new PointF(3, 3));
            /// graphics.DrawString("Hello World!!!", fontNegrita, PdfBrushes.Blue, new PointF(50, 50));

            string filename = $"{exportPath}" + "/RKP_" + idCarpeta + ".pdf";
            doc.Save(filename);
            //Close the document.
            doc.Close(true);
        }
        public void pdfHojaControl1(int idCarpeta, string selectedPath)
        {
            string hc_titulo1 = "", hc_titulo2 = "", hc_titulo3 = "", hc_cal_codigo = "", hc_cal_version = "", hc_cal_fecha = "", nombres = "", nroExp = "", archivadoPor = "", nomExpediente = "", numCaja = "", nomSubdependencia = "", nota1 = "", nota2 = "";
            int idProyecto = 0;
            RectangleF imageBounds = new RectangleF(4, 9, 84, 22);
            //Create a new PDF document.
            PdfDocument doc = new PdfDocument();

            doc.PageSettings.Orientation = PdfPageOrientation.Portrait;
            doc.PageSettings.Size = PdfPageSize.Letter; //Width = 612 Height = 792
            doc.PageSettings.Margins.Bottom = 30;
            PdfPage page = doc.Pages.Add();//Add a page to the document.

            RectangleF recHeader = new RectangleF(0, 0, doc.Pages[0].GetClientSize().Width, 50);
            RectangleF recFooter = new RectangleF(0, 0, doc.Pages[0].GetClientSize().Width, 10);
            PdfPageTemplateElement header = new PdfPageTemplateElement(recHeader);
            PdfPageTemplateElement footer = new PdfPageTemplateElement(recFooter);
            PdfPen borde = new PdfPen(Color.Black, 1);
            PdfGraphics graphicsPag = page.Graphics;
            PdfGraphics graphicsHeader = header.Graphics;
            PdfGraphics graphicsFooter = footer.Graphics;
            //HEADER
            RectangleF recImagen = new RectangleF(0, 0, 92, 42); //Borde
            graphicsHeader.DrawRectangle(borde, recImagen);

            //Consulta base de datos
            var datHC = EntitiesRepository.Entities.t_carpeta.Include("t_tercero").Include("t_lote").Include("p_proyecto").Include("p_formato").AsNoTracking()
                    .Where(f => f.id == idCarpeta)
                    .Select(p => new { p.t_lote.p_proyecto.p_formato, p.t_lote.t_carpeta, p.t_tercero, p.t_lote.p_subdependencia, p.t_lote.p_proyecto });
            var dataFormato = datHC.FirstOrDefault();
            if (dataFormato != null)
            {
                if (dataFormato.p_formato.Count > 0)
                {
                    if (dataFormato.t_tercero != null) nombres = dataFormato.t_tercero.nombres + " " + dataFormato.t_tercero.apellidos;
                    if (dataFormato.p_formato.FirstOrDefault().hc_titulo1 != null) hc_titulo1 = dataFormato.p_formato.FirstOrDefault().hc_titulo1;
                    if (dataFormato.p_formato.FirstOrDefault().hc_titulo2 != null) hc_titulo2 = dataFormato.p_formato.FirstOrDefault().hc_titulo2;
                    if (dataFormato.p_formato.FirstOrDefault().hc_titulo3 != null) hc_titulo3 = dataFormato.p_formato.FirstOrDefault().hc_titulo3;
                    if (dataFormato.p_formato.FirstOrDefault().hc_cal_codigo != null) hc_cal_codigo = dataFormato.p_formato.FirstOrDefault().hc_cal_codigo;
                    if (dataFormato.p_formato.FirstOrDefault().hc_cal_version != null) hc_cal_version = dataFormato.p_formato.FirstOrDefault().hc_cal_version;
                    if (dataFormato.p_formato.FirstOrDefault().hc_cal_fecha != null) hc_cal_fecha = dataFormato.p_formato.FirstOrDefault().hc_cal_fecha;
                    nota1 = dataFormato.p_formato.FirstOrDefault().hc_nota1;
                    nota2 = dataFormato.p_formato.FirstOrDefault().hc_nota2;
                }
                nroExp = dataFormato.t_carpeta.FirstOrDefault().nom_expediente;
                archivadoPor = dataFormato.p_proyecto.nom_proyecto;
                idProyecto = dataFormato.p_proyecto.id;
                nomExpediente = dataFormato.t_carpeta.FirstOrDefault().nro_expediente;
                numCaja = dataFormato.t_carpeta.FirstOrDefault().nro_caja;
                nomSubdependencia = dataFormato.p_subdependencia.nombre;
            }

            ////////LOGO
            if (File.Exists("logo_" + idProyecto + ".png"))
            {
                PdfBitmap image = new PdfBitmap("logo_" + idProyecto + ".png");
                graphicsHeader.DrawImage(image, 4, 9, 84, 22);//Draw the image
            }

            ////////TITULO
            PdfStringFormat formatoTxtCentrado = new PdfStringFormat(); formatoTxtCentrado.Alignment = PdfTextAlignment.Center;
            PdfStringFormat formatoTxtIzquierda = new PdfStringFormat(); formatoTxtIzquierda.Alignment = PdfTextAlignment.Left;
            PdfFont fontTitulo = new PdfStandardFont(PdfFontFamily.Helvetica, 7, PdfFontStyle.Bold);//Set the standard font.
            graphicsHeader.DrawString(hc_titulo1, fontTitulo, PdfBrushes.Black, new PointF(266, 6), formatoTxtCentrado);//Draw the text.
            graphicsHeader.DrawString(hc_titulo2, fontTitulo, PdfBrushes.Black, new PointF(266, 17), formatoTxtCentrado);//Draw the text.
            graphicsHeader.DrawString(hc_titulo3, fontTitulo, PdfBrushes.Black, new PointF(266, 28), formatoTxtCentrado);//Draw the text.
            RectangleF recTitulo = new RectangleF(92, 0, 358, recImagen.Height); //Borde
            graphicsHeader.DrawRectangle(borde, recTitulo);
            ////////CALIDAD
            PdfFont fontFormatoNegrita = new PdfStandardFont(PdfFontFamily.Helvetica, 5, PdfFontStyle.Bold);
            PdfFont fontFormatoNegritaV2 = new PdfStandardFont(PdfFontFamily.Helvetica, 7, PdfFontStyle.Bold);
            graphicsHeader.DrawString(hc_cal_codigo, fontTitulo, PdfBrushes.Black, new PointF(460, 4), formatoTxtIzquierda);
            graphicsHeader.DrawString(hc_cal_version, fontTitulo, PdfBrushes.Black, new PointF(460, 13), formatoTxtIzquierda);
            graphicsHeader.DrawString(hc_cal_fecha, fontTitulo, PdfBrushes.Black, new PointF(460, 22), formatoTxtIzquierda);

            PdfPageNumberField pageNumber = new PdfPageNumberField();
            PdfPageCountField count = new PdfPageCountField();
            PdfCompositeField compositeField = new PdfCompositeField(fontFormatoNegritaV2, PdfBrushes.Black, "Páginas {0} de {1}", pageNumber, count);
            compositeField.StringFormat = formatoTxtIzquierda;
            compositeField.Draw(graphicsHeader, new PointF(460, 31));
            //graphics.DrawString("Páginas 1 de 1", fontTitulo, PdfBrushes.Black, new PointF(460, 31), formatoTxtIzquierda);

            RectangleF recCalidad = new RectangleF(450, 0, 82, recImagen.Height); //Borde
            graphicsHeader.DrawRectangle(borde, recCalidad);
            doc.Template.Top = header;

            //FOOTER
            PdfFont fontPie = new PdfStandardFont(PdfFontFamily.Helvetica, 6);//Set the standard font.
            graphicsFooter.DrawString("Desarrollado por Alpha Intelligence AI", fontPie, PdfBrushes.Black, new PointF(50, 0), formatoTxtCentrado);//Draw the text.
            doc.Template.Bottom = footer;

            ///////ENCABEZADO
            ///LÍNEA 1
            ///TXT HOja de control
            var AlturaEncabezadoLinea1 = recImagen.Height + 20;
            PdfFont fontComun = new PdfStandardFont(PdfFontFamily.Helvetica, 6);
            graphicsPag.DrawString("Hoja de Control No.", fontComun, PdfBrushes.Black, new PointF(imageBounds.X, AlturaEncabezadoLinea1), formatoTxtIzquierda);
            graphicsPag.DrawString("1", fontComun, PdfBrushes.Black, new PointF(recTitulo.X + 10, AlturaEncabezadoLinea1), formatoTxtCentrado);
            RectangleF recEncHC = new RectangleF(recTitulo.X, AlturaEncabezadoLinea1 - 2, 22, 12);
            page.Graphics.DrawRectangle(borde, recEncHC);
            ///TXT Carpeta
            graphicsPag.DrawString("Carpeta", fontComun, PdfBrushes.Black, new PointF(imageBounds.X + 130, AlturaEncabezadoLinea1), formatoTxtIzquierda);
            graphicsPag.DrawString(dataFormato.t_carpeta.FirstOrDefault().nro_expediente, fontComun, PdfBrushes.Black, new PointF(imageBounds.X + 190 + 82, AlturaEncabezadoLinea1), formatoTxtCentrado);
            RectangleF recEncarpeta = new RectangleF(recTitulo.X + 75, AlturaEncabezadoLinea1 - 2, 220, 12);
            page.Graphics.DrawRectangle(borde, recEncarpeta);
            ///TXT Caja
            graphicsPag.DrawString("Caja", fontComun, PdfBrushes.Black, new PointF(imageBounds.X + 190 + 205, AlturaEncabezadoLinea1), formatoTxtIzquierda);
            graphicsPag.DrawString(dataFormato.t_carpeta.FirstOrDefault().nro_caja, fontComun, PdfBrushes.Black, new PointF(imageBounds.X + 190 + 190 + 82, AlturaEncabezadoLinea1), formatoTxtCentrado);
            RectangleF recEncaja = new RectangleF(recTitulo.X + 145 + 190, AlturaEncabezadoLinea1 - 2, 80, 12);
            page.Graphics.DrawRectangle(borde, recEncaja);
            ///LÍNEA 2
            var AlturaEncabezadoLinea2 = recEncaja.Y + 20;
            graphicsPag.DrawString("Carpeta o Expediente No.", fontComun, PdfBrushes.Black, new PointF(imageBounds.X, AlturaEncabezadoLinea2), formatoTxtIzquierda);
            graphicsPag.DrawString(nroExp, fontComun, PdfBrushes.Black, new PointF(recTitulo.X + 37, AlturaEncabezadoLinea2), formatoTxtCentrado);
            RectangleF recEncExp = new RectangleF(recTitulo.X, AlturaEncabezadoLinea2 - 2, 75, 12);
            page.Graphics.DrawRectangle(borde, recEncExp);
            ///LÍNEA 3
            var AlturaEncabezadoLinea3 = recEncExp.Y + 20;
            graphicsPag.DrawString("Nombre del Expediente o del declarante", fontComun, PdfBrushes.Black, new PointF(imageBounds.X, AlturaEncabezadoLinea3), formatoTxtIzquierda);
            graphicsPag.DrawString(nombres, fontComun, PdfBrushes.Black, new PointF(recTitulo.X + 222, AlturaEncabezadoLinea3), formatoTxtCentrado);
            RectangleF recEncNomExp = new RectangleF(recTitulo.X + 30, AlturaEncabezadoLinea3 - 2, 384, 12);
            page.Graphics.DrawRectangle(borde, recEncNomExp);
            ///LÍNEA 4
            ///TXT Fecha apertura hoja de control
            var AlturaEncabezadoLinea4 = recEncNomExp.Y + 20;
            graphicsPag.DrawString("Fecha de apertura hoja de control", fontComun, PdfBrushes.Black, new PointF(imageBounds.X, AlturaEncabezadoLinea4), formatoTxtIzquierda);
            graphicsPag.DrawString("2019/10/15", fontComun, PdfBrushes.Black, new PointF(recTitulo.X + 60, AlturaEncabezadoLinea4), formatoTxtCentrado);
            RectangleF recEncFechaIni = new RectangleF(recTitulo.X + 30, AlturaEncabezadoLinea4 - 2, 65, 12);
            page.Graphics.DrawRectangle(borde, recEncFechaIni);
            ///TXT Dependencia
            graphicsPag.DrawString("Dependencia", fontComun, PdfBrushes.Black, new PointF(imageBounds.X + 190 + 5, AlturaEncabezadoLinea4), formatoTxtIzquierda);
            graphicsPag.DrawString(nomSubdependencia, fontComun, PdfBrushes.Black, new PointF(imageBounds.X + 190 + 187, AlturaEncabezadoLinea4), formatoTxtCentrado);
            RectangleF recDependencia = new RectangleF(recTitulo.X + 150, AlturaEncabezadoLinea4 - 2, 264, 12);
            page.Graphics.DrawRectangle(borde, recDependencia);
            ///LÍNEA 4
            var AlturaEncabezadoLinea5 = recDependencia.Y + 20;
            PdfTextElement textNota = new PdfTextElement(nota1, fontComun);
            RectangleF recNota = new RectangleF(imageBounds.X, AlturaEncabezadoLinea5, 525, page.GetClientSize().Height);
            textNota.Draw(page, recNota);

            ////////////////CREAR TABLA////////////////
            PdfGrid pdfGrid = new PdfGrid();// Create a PdfLightTable.
            DataTable table = new DataTable();// Initialize DataTable to assign as DateSource to the light table.
            table.Columns.Add("No folios que contiene");//Include columns to the DataTable.
            table.Columns.Add("Documento a archivar");
            table.Columns.Add("No folio en el que queda");
            table.Columns.Add("Fecha ingreso");
            table.Columns.Add("Archivado por");
            table.Columns.Add("Observaciones");
            PdfStringFormat stringCentrado = new PdfStringFormat();   //Initialize PdfStringFormat and set the properties
            stringCentrado.Alignment = PdfTextAlignment.Center;
            stringCentrado.LineAlignment = PdfVerticalAlignment.Middle;

            var datDocs = EntitiesRepository.Entities.t_documento.Include("t_documento_resp").Include("p_tipoitem")
                .Where(f => f.id_carpeta == idCarpeta)
                .Select(p => new { p.t_documento_resp, p.pag_ini, p.p_tipodoc }).OrderBy(x => x.pag_ini).ToList();
            int folio_anterior = 0; int folio_final = 0;
            DateTime fechaPrincipal = DateTime.MinValue;
            //Estilo de fila en Table
            PdfGridCellStyle gridCellStyle = new PdfGridCellStyle();     //Initialize PdfGridCellStyle. Set background color and string format
            gridCellStyle.StringFormat = stringCentrado;
            gridCellStyle.Font = fontComun;
            foreach (var item in datDocs)
            {
                int folios = 0; int pagActual = 0; int nIni = 0; int nFin = 0; string folInicial = ""; string folFinal = ""; string fecha = ""; DateTime fec = DateTime.MinValue;
                int docPrincipal = item.p_tipodoc.principal;
                foreach (var itemResp in item.t_documento_resp)
                {
                    string descr = itemResp.p_tipoitem.descripcion.Trim().ToUpper();
                    string tipo = itemResp.p_tipoitem.type.Trim().ToUpper();
                    if (folInicial == "" && tipo == "NUMERICO" && (descr == "FOLIO INICIAL" || descr == "FOLIO INICIA" || descr == "FOLIO INICIO" || descr == "FOLIO INI")) folInicial = itemResp.valor;
                    if (folFinal == "" && tipo == "NUMERICO" && (descr == "FOLIO FINAL" || descr == "FOLIO FINALIZA" || descr == "FOLIO TERMINA" || descr == "FOLIO FIN")) folFinal = itemResp.valor;
                    if (fecha == "" && tipo == "FECHA") fecha = itemResp.valor;
                }
                int.TryParse(item.pag_ini.ToString(), out pagActual);
                bool isNumericIni = int.TryParse(folInicial, out nIni);
                bool isNumericFin = int.TryParse(folFinal, out nFin);
                bool isDateFecha = DateTime.TryParse(fecha, out fec);
                if (isNumericIni && isNumericFin) folios = nFin - nIni;
                if (folios == 0) folios = pagActual - folio_anterior;
                if (docPrincipal == 1 && isDateFecha) fechaPrincipal = fec;
                table.Rows.Add(new string[] { folios.ToString(), item.p_tipodoc.nombre + " " + nombres + ", " + nroExp, item.pag_ini.ToString(), fechaPrincipal.ToString("dd/M/yyyy", CultureInfo.InvariantCulture), archivadoPor, "" });//Include rows to the DataTable.
                folio_anterior = pagActual;
                if (nFin > 0) folio_final = nFin;
            }

            pdfGrid.DataSource = table;//Assign data source.
            pdfGrid.Columns[0].Width = 30;
            pdfGrid.Columns[1].Width = 180;
            pdfGrid.Columns[2].Width = 30;
            pdfGrid.Columns[3].Width = 50;
            pdfGrid.Columns[4].Width = 110;
            pdfGrid.Columns[5].Width = 124;
            for (int c = 0; c < table.Rows.Count; c++)
            {
                PdfGridRow gridRow = pdfGrid.Rows[c];
                gridRow.ApplyStyle(gridCellStyle);
            }

            //Estilo fuente de header en Table
            PdfFont fuenteHeader = new PdfStandardFont(PdfFontFamily.Helvetica, 6, PdfFontStyle.Bold);
            PdfGridCellStyle HeaderCellStyle = new PdfGridCellStyle();     //Initialize PdfGridCellStyle. Set background color and string format
            HeaderCellStyle.StringFormat = stringCentrado;
            HeaderCellStyle.Font = fuenteHeader;
            pdfGrid.Headers.ApplyStyle(HeaderCellStyle);

            PdfGridLayoutResult pdfGridLayoutResult = pdfGrid.Draw(page, imageBounds.X, recNota.Y + 20);//Draw PdfLightTable.

            int pageCount = doc.Pages.Count;
            var lastPage = doc.Pages[pageCount - 1];//Identifica última Hoja
            float totalBorde = recNota.Y;
            if (pageCount == 2) totalBorde += 570;
            if (pageCount == 3) totalBorde += 1300;

            var finalTabla = pdfGridLayoutResult.Bounds.Bottom;
            textNota = new PdfTextElement(nota2, fontComun);
            RectangleF recNota2 = new RectangleF(imageBounds.X + 30, finalTabla + 3, 495, page.GetClientSize().Height);
            textNota.Draw(lastPage, recNota2);
            //Console.WriteLine(pdfGridLayoutResult.Bounds.Height + ((640) * (pageCount - 1)) + recNota.Y + 20);

            //CUADRO BORDE DE TABLA
            PdfLayoutFormat format = new PdfLayoutFormat();
            format.Break = PdfLayoutBreakType.FitPage;
            format.Layout = PdfLayoutType.Paginate; //Width = 612 Height = 792
            format.PaginateBounds = new RectangleF(0, 0, 612, 792 - 60);
            RectangleF rect = new RectangleF(0, header.Y, 532, pdfGridLayoutResult.Bounds.Height + totalBorde);
            PdfRectangle recBordeTabla = new PdfRectangle(rect);
            recBordeTabla.Draw(page, 0, AlturaEncabezadoLinea1 - 10, format);

            //RectangleF recBorde = new RectangleF(0, AlturaEncabezadoLinea1 - 10, 532, finalTabla - AlturaEncabezadoLinea1 + 23);
            //page.Graphics.DrawRectangle(borde, recBorde);

            ///TXT Total Folios
            textNota = new PdfTextElement("Folios Carpeta", fontFormatoNegritaV2);
            RectangleF recTotal = new RectangleF(recNota2.X, finalTabla + 40, 100, page.GetClientSize().Height);
            textNota.Draw(lastPage, recTotal);

            RectangleF rectTotalFolios = new RectangleF(recTotal.X + 60, recTotal.Y, 30, 10);
            lastPage.Graphics.DrawRectangle(borde, rectTotalFolios);
            lastPage.Graphics.DrawString(folio_final.ToString(), fontFormatoNegritaV2, PdfBrushes.Black, new PointF(rectTotalFolios.X + (rectTotalFolios.Width / 2), recTotal.Y + 1), formatoTxtCentrado);

            lastPage.Graphics.DrawString("Hoja " + pageCount + " de " + pageCount, fontFormatoNegritaV2, PdfBrushes.Black, new PointF(recEncaja.X, recTotal.Y + 1), formatoTxtCentrado);

            //Save the document.
            doc.Save($"{selectedPath}/" + "HC_" + idCarpeta + ".pdf");

            //Close the document.
            doc.Close(true);
        }

        private void HC2_AddPage(int idCarpeta, PdfDocument doc, List<regHC2> lista, string fechApertura, int totalPag, p_formato mipFormato, string tomo)
        {
            string hc_titulo1 = "", hc_titulo2 = "", hc_titulo3 = "", hc_cal_codigo = "", hc_cal_version = "", hc_cal_fecha = "", nombres = "", nroExp = "", archivadoPor = "", nomExpediente = "", numCaja = "", nomSubdependencia = "", nota1 = "", nota2 = "", nroCarpeta = "", hc_ini = "", hc_fin = "";
            int idProyecto = 0, numHCini = 0, numHCfin = 0;
            conteo = 0;
            RectangleF imageBounds = new RectangleF(4, 9, 84, 22);
            PdfPage page = doc.Pages.Add();//Add a page to the document.
            int pageCount = doc.Pages.Count;
            var lastPage = doc.Pages[pageCount - 1];

            RectangleF recHeader = new RectangleF(0, 0, doc.Pages[0].GetClientSize().Width, 50);
            RectangleF recFooter = new RectangleF(0, 0, doc.Pages[0].GetClientSize().Width, 10);
            PdfPageTemplateElement header = new PdfPageTemplateElement(recHeader);
            PdfPageTemplateElement footer = new PdfPageTemplateElement(recFooter);
            PdfPen borde = new PdfPen(Color.Black, 1);
            PdfGraphics graphicsPag = page.Graphics;
            //PdfGraphics graphicsHeader = header.Graphics;
            PdfGraphics graphicsFooter = footer.Graphics;

            //Consulta base de datos
            var datHC = EntitiesRepository.Entities.t_carpeta.Include("t_tercero").Include("t_lote").Include("p_proyecto").AsNoTracking()
                    .Where(f => f.id == idCarpeta)
                    .Select(p => new { p.nro_caja, p.t_lote.t_carpeta, p.t_tercero, p.t_lote.p_subdependencia, p.t_lote.p_proyecto, p.nro_carpeta, p.nro_expediente, p.nom_expediente, p.hc_inicio, p.hc_fin });
            var dataFormato = datHC.FirstOrDefault();
            if (dataFormato != null)
            {
                if (dataFormato.t_tercero != null) nombres = dataFormato.t_tercero.nombres + " " + dataFormato.t_tercero.apellidos;
                if (mipFormato != null)
                {
                    if (mipFormato.hc_titulo1 != null) hc_titulo1 = mipFormato.hc_titulo1;
                    if (mipFormato.hc_titulo2 != null) hc_titulo2 = mipFormato.hc_titulo2;
                    if (mipFormato.hc_titulo3 != null) hc_titulo3 = mipFormato.hc_titulo3;
                    if (mipFormato.hc_cal_codigo != null) hc_cal_codigo = mipFormato.hc_cal_codigo;
                    if (mipFormato.hc_cal_version != null) hc_cal_version = mipFormato.hc_cal_version;
                    if (mipFormato.hc_cal_fecha != null) hc_cal_fecha = mipFormato.hc_cal_fecha;
                    nota1 = mipFormato.hc_nota1;
                    nota2 = mipFormato.hc_nota2;
                }
                if (dataFormato.nro_caja != null) numCaja = dataFormato.nro_caja;
                if (dataFormato.nro_expediente != null) nroExp = dataFormato.nro_expediente;
                if (dataFormato.nom_expediente != null) nomExpediente = dataFormato.nom_expediente;
                //if (dataFormato.tomo != null) tomo = dataFormato.tomo;
                if (dataFormato.hc_inicio != null)
                {
                    hc_ini = dataFormato.hc_inicio;
                    numHCini = GlobalClass.GetNumber(hc_ini) + pageCount - 1;
                    //numHCfin = GlobalClass.GetNumber(hc_ini) + totalPag - 1;
                }
                if (dataFormato.hc_fin != null)
                {
                    hc_fin = dataFormato.hc_fin;
                    numHCfin = GlobalClass.GetNumber(hc_fin);
                    if (numHCini > numHCfin) numHCfin = numHCini;
                }
                if (dataFormato.nro_carpeta != null) nroCarpeta = dataFormato.nro_carpeta.ToString();
                archivadoPor = dataFormato.p_proyecto.nom_proyecto;
                idProyecto = dataFormato.p_proyecto.id;
                nomSubdependencia = dataFormato.p_subdependencia.nombre;
            }

            //HEADER
            RectangleF recImagen = new RectangleF(0.5f, 0.5f, 170, 42); //Borde
            graphicsPag.DrawRectangle(borde, recImagen);
            ////////LOGO
            if (File.Exists("logo_" + idProyecto + ".png"))
            {
                PdfBitmap image = new PdfBitmap("logo_" + idProyecto + ".png");
                graphicsPag.DrawImage(image, 4, 4, 160, 34);//Draw the image
            }

            ////////TITULO
            /////Create new PDF gradient brush.
            PdfLinearGradientBrush brush = new PdfLinearGradientBrush(new PointF(0, 0), new PointF(1, 1), Color.Red, Color.Blue);
            ///brush.Background = Color.FromArgb(0, 0, 204);
            RectangleF recTitulo = new RectangleF(170.5f, 0.5f, 280, 14); //Borde
           /// graphicsPag.DrawRectangle(brush, recTitulo);
            ///graphicsPag.DrawRectangle(borde, recTitulo);
           /// RectangleF recTitulo2 = new RectangleF(170.5f, 0.5f, 280, 28); //Borde
           /// graphicsPag.DrawRectangle(borde, recTitulo2);
            RectangleF recTitulo3 = new RectangleF(170.5f, 0.5f, 280, recImagen.Height); //Borde
            graphicsPag.DrawRectangle(borde, recTitulo3);
            PdfStringFormat formatoTxtCentrado = new PdfStringFormat(); formatoTxtCentrado.Alignment = PdfTextAlignment.Center;
            PdfStringFormat formatoTxtCentro = new PdfStringFormat(); formatoTxtCentro.Alignment = PdfTextAlignment.Center; formatoTxtCentro.LineAlignment = PdfVerticalAlignment.Middle;
            PdfStringFormat formatoTxtIzquierda = new PdfStringFormat(); formatoTxtIzquierda.Alignment = PdfTextAlignment.Left;
            PdfFont fontTitulo = new PdfStandardFont(PdfFontFamily.Helvetica, 8.5f);//cambia fuente encabezado general.
           /// graphicsPag.DrawString(hc_titulo1, fontTitulo, PdfBrushes.White, new PointF(310, 4), formatoTxtCentrado);//Draw the text.
            graphicsPag.DrawString(hc_titulo1, fontTitulo, PdfBrushes.Black, new PointF(310, 17), formatoTxtCentrado);//Draw the text.
            ///graphicsPag.DrawString(hc_titulo3, fontTitulo, PdfBrushes.Black, new PointF(310, 32), formatoTxtCentrado);//Draw the text.
            ///
            string codigo1 = "Codigo:9-GDM-F-09" ;


            ////////CALIDAD
            PdfFont fontFormatoNegrita = new PdfStandardFont(PdfFontFamily.Helvetica, 3, PdfFontStyle.Bold);
            PdfFont fontFormatoNegritaV2 = new PdfStandardFont(PdfFontFamily.Helvetica, 4, PdfFontStyle.Bold);
            RectangleF recCalidad = new RectangleF(450.5f, 0.1f, 81, 42); //Borde
            graphicsPag.DrawRectangle(borde, recCalidad);
            recCalidad = new RectangleF(450.5f, 3f, 81, 10.5f); //Borde
            graphicsPag.DrawRectangle(borde, recCalidad);
            recCalidad = new RectangleF(450.5f, 5f, 81, 21); //Borde
            graphicsPag.DrawRectangle(borde, recCalidad);
            ///recCalidad = new RectangleF(450.5f, 0.5f, 81, 31.5f); //Borde
           ///graphicsPag.DrawRectangle(borde, recCalidad);
            graphicsPag.DrawString(codigo1, fontTitulo, PdfBrushes.Black, new PointF(452, 2.5f), formatoTxtIzquierda);
            graphicsPag.DrawString(hc_cal_version, fontTitulo, PdfBrushes.Black, new PointF(452, 16), formatoTxtIzquierda);
            graphicsPag.DrawString(hc_cal_fecha, fontTitulo, PdfBrushes.Black, new PointF(452, 30), formatoTxtIzquierda);
            
            
            ///CAMPO PAGINAS 1 DE BAJO CAMPO CODIGO Y FECHA
            
           
           /// PdfPageNumberField pageNumber = new PdfPageNumberField();
          ///  PdfPageCountField count = new PdfPageCountField();
           /// PdfCompositeField compositeField = new PdfCompositeField(fontFormatoNegritaV2, PdfBrushes.Black, "Páginas {0} de ", pageNumber);
           /// compositeField.StringFormat = formatoTxtIzquierda;
           // compositeField.Draw(graphicsPag, new PointF(452, 32.5f));
            //graphics.DrawString("Páginas 1 de 1", fontTitulo, PdfBrushes.Black, new PointF(460, 31), formatoTxtIzquierda);

            //RectangleF recCalidad = new RectangleF(0.5f, 0, 612, recImagen.Height); //Borde
            //graphicsHeader.DrawRectangle(borde, recCalidad);
            //doc.Template.Top = header;

            //doc.Template.Bottom = footer;

            ///////ENCABEZADO
            ///LÍNEA 1
            ///TXT HOja de control
            var AlturaEncabezadoLinea1 = recImagen.Height + 9;
            PdfFont fontComun = new PdfStandardFont(PdfFontFamily.Helvetica, 6);
            PdfFont fontComun63 = new PdfStandardFont(PdfFontFamily.Helvetica, 6.3f);
            graphicsPag.DrawString("Hoja de Control No.", fontComun, PdfBrushes.Black, new PointF(imageBounds.X, AlturaEncabezadoLinea1), formatoTxtIzquierda);
            graphicsPag.DrawString(pageCount.ToString(), fontComun, PdfBrushes.Black, new PointF(recTitulo.X - 12, AlturaEncabezadoLinea1), formatoTxtCentrado);
            RectangleF recEncHC1 = new RectangleF(recTitulo.X - 22, AlturaEncabezadoLinea1 - 2, 22, 12);
            page.Graphics.DrawRectangle(borde, recEncHC1);
            graphicsPag.DrawString(" de ", fontComun, PdfBrushes.Black, new PointF(recEncHC1.X + 26, AlturaEncabezadoLinea1), formatoTxtIzquierda);
            RectangleF recEncHC2 = new RectangleF(recEncHC1.X + 40, AlturaEncabezadoLinea1 - 2, 22, 12);
            page.Graphics.DrawRectangle(borde, recEncHC2);
            float xHCfinal = recEncHC2.X + (recEncHC2.Width / 2);
            float yHCfinal = AlturaEncabezadoLinea1;
           /// PdfCompositeField compositeField2 = new PdfCompositeField(fontComun, PdfBrushes.Black, "{0}", count);
           /// compositeField2.StringFormat = formatoTxtIzquierda;
            //compositeField2.Draw(graphicsPag, new PointF(xHCfinal-2, yHCfinal));
            //graphicsPag.DrawString(count.ToString(), fontComun, PdfBrushes.Black, new PointF(xHCfinal, yHCfinal), formatoTxtCentrado);

            ///LÍNEA 2
            var AlturaEncabezadoLinea2 = AlturaEncabezadoLinea1 + 16;
            graphicsPag.DrawString("Carpeta o Expediente No.", fontComun, PdfBrushes.Black, new PointF(imageBounds.X, AlturaEncabezadoLinea2), formatoTxtIzquierda);
            graphicsPag.DrawString(tomo, fontComun, PdfBrushes.Black, new PointF(recEncHC1.X + 30, AlturaEncabezadoLinea2), formatoTxtCentrado);
            RectangleF recEncExp = new RectangleF(recEncHC1.X, AlturaEncabezadoLinea2 - 2, recEncHC1.Width + recEncHC2.Width + 19, 12);
            page.Graphics.DrawRectangle(borde, recEncExp);
            ///TXT Dependencia
            graphicsPag.DrawString("Caja No.", fontComun, PdfBrushes.Black, new PointF(recEncExp.X + recEncExp.Width + 10, AlturaEncabezadoLinea2), formatoTxtIzquierda);
            graphicsPag.DrawString(numCaja, fontComun, PdfBrushes.Black, new PointF(392, AlturaEncabezadoLinea2), formatoTxtCentrado);
            RectangleF recCaja = new RectangleF(recEncHC2.X + 70, AlturaEncabezadoLinea2 - 2, 268, 12);
            page.Graphics.DrawRectangle(borde, recCaja);

            ///LÍNEA 3
            var AlturaEncabezadoLinea3 = recEncExp.Y + 19;
            graphicsPag.DrawString("Nombre ", fontComun, PdfBrushes.Black, new PointF(imageBounds.X, AlturaEncabezadoLinea3), formatoTxtIzquierda);
            //graphicsPag.DrawString(nomExpediente, fontComun, PdfBrushes.Black, new PointF(recEncHC1.X + 189, AlturaEncabezadoLinea3), formatoTxtCentrado);
            RectangleF recEncNomExp = new RectangleF(recEncHC1.X, AlturaEncabezadoLinea3 - 2, 378, 18);
            page.Graphics.DrawRectangle(borde, recEncNomExp);
            PdfTextElement txtpdfExpediente = new PdfTextElement(nomExpediente, fontComun);
            recEncNomExp.X = recEncNomExp.X; recEncNomExp.Y = recEncNomExp.Y + 1; recEncNomExp.Width = recEncNomExp.Width - 5; recEncNomExp.Height = recEncNomExp.Height - 2;
            txtpdfExpediente.StringFormat = formatoTxtCentro;
            txtpdfExpediente.Draw(page, recEncNomExp);
            ///LÍNEA 4
            ///TXT Fecha apertura hoja de control
            var AlturaEncabezadoLinea4 = recEncNomExp.Y + 24;
            graphicsPag.DrawString("Fecha de apertura hoja de control", fontComun, PdfBrushes.Black, new PointF(imageBounds.X, AlturaEncabezadoLinea4), formatoTxtIzquierda);
            RectangleF recEncFechaIni = new RectangleF(recEncHC1.X, AlturaEncabezadoLinea4 - 2, recEncHC1.Width + recEncHC2.Width + 18, 12);
            page.Graphics.DrawRectangle(borde, recEncFechaIni);
            graphicsPag.DrawString(fechApertura, fontComun, PdfBrushes.Black, new PointF(recEncHC1.X + 30, AlturaEncabezadoLinea4), formatoTxtCentrado);
            ///TXT Dependencia
            graphicsPag.DrawString("Dependencia", fontComun, PdfBrushes.Black, new PointF(recEncHC2.X + 30, AlturaEncabezadoLinea4), formatoTxtIzquierda);
            graphicsPag.DrawString(nomSubdependencia, fontComun, PdfBrushes.Black, new PointF(392, AlturaEncabezadoLinea4), formatoTxtCentrado);
            RectangleF recDependencia = new RectangleF(recEncHC2.X + 70, AlturaEncabezadoLinea4 - 2, 268, 12);
            page.Graphics.DrawRectangle(borde, recDependencia);
            ///LÍNEA 4
            var AlturaEncabezadoLinea5 = recDependencia.Y + 18;
            PdfTextElement textNota = new PdfTextElement(nota1, fontComun);
            RectangleF recNota = new RectangleF(imageBounds.X, AlturaEncabezadoLinea5, 525, page.GetClientSize().Height);
            textNota.Draw(page, recNota);

            ////////////////CREAR ENCABEZADO TABLA////////////////
            //  ANCHO DE COLUMNAS
            var anchoHead = new List<int>();
            anchoHead.Add(20);  //0 ítem
            anchoHead.Add(30);
            anchoHead.Add(145);//2  Tipo documental
            anchoHead.Add(27);
            anchoHead.Add(27);//4   Folio final
            anchoHead.Add(40);
            anchoHead.Add(60);//6 usuario
            anchoHead.Add(40);
            anchoHead.Add(133);//8 Observaciones

            ////ENCABEZADO TABLA////
            float AlturaEncabezadoTabla = AlturaEncabezadoLinea1 + 95;
            RectangleF recEnc1 = new RectangleF(imageBounds.X, AlturaEncabezadoTabla, anchoHead[0], 40); //ítem
            page.Graphics.DrawRectangle(borde, recEnc1);
            graphicsPag.DrawString("Ítem", fontComun, PdfBrushes.Black, new PointF(imageBounds.X + anchoHead[0] / 2, AlturaEncabezadoTabla + 20), formatoTxtCentrado);

            RectangleF recEnc2 = new RectangleF(recEnc1.X + recEnc1.Width, AlturaEncabezadoTabla, anchoHead[1], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc2);    //Nro Folios
            PdfTextElement textTitulo = new PdfTextElement("No de folios que contiene", fontComun); textTitulo.StringFormat = formatoTxtCentro;
            textTitulo.Draw(page, recEnc2);

            RectangleF recEnc3 = new RectangleF(recEnc2.X + recEnc2.Width, AlturaEncabezadoTabla, anchoHead[2], 40); //Tipo Documental
            page.Graphics.DrawRectangle(borde, recEnc3);
            textTitulo = new PdfTextElement("Tipo Documental", fontComun); textTitulo.StringFormat = formatoTxtCentro;
            textTitulo.Draw(page, recEnc3);

            RectangleF recEnc4 = new RectangleF(recEnc3.X + recEnc3.Width, AlturaEncabezadoTabla, anchoHead[3] + anchoHead[4], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc4);
            textTitulo = new PdfTextElement("Rango de Folios", fontComun); textTitulo.StringFormat = formatoTxtCentro;
            textTitulo.Draw(page, recEnc4);

            RectangleF recEnc41 = new RectangleF(recEnc3.X + recEnc3.Width, AlturaEncabezadoTabla + 20, anchoHead[3], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc41);
            textTitulo = new PdfTextElement("Desde", fontComun); textTitulo.StringFormat = formatoTxtCentro;
            textTitulo.Draw(page, recEnc41);

            RectangleF recEnc42 = new RectangleF(recEnc41.X + recEnc41.Width, AlturaEncabezadoTabla + 20, anchoHead[4], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc42);
            textTitulo = new PdfTextElement("Hasta", fontComun); textTitulo.StringFormat = formatoTxtCentro;
            textTitulo.Draw(page, recEnc42);

            RectangleF recEnc5 = new RectangleF(recEnc4.X + recEnc4.Width, AlturaEncabezadoTabla, anchoHead[5], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc5);
            textTitulo = new PdfTextElement("Fecha del documento (dd/mm/aaaa)", fontComun); textTitulo.StringFormat = formatoTxtCentro;
            textTitulo.Draw(page, recEnc5);

            RectangleF recEnc6 = new RectangleF(recEnc5.X + recEnc5.Width, AlturaEncabezadoTabla, anchoHead[6], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc6);
            textTitulo = new PdfTextElement("Archivado por", fontComun); textTitulo.StringFormat = formatoTxtCentro;
            textTitulo.Draw(page, recEnc6);

            RectangleF recEnc7 = new RectangleF(recEnc6.X + recEnc6.Width, AlturaEncabezadoTabla, anchoHead[7], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc7);
            textTitulo = new PdfTextElement("Fecha de Ingreso del documento", fontComun); textTitulo.StringFormat = formatoTxtCentro;
            textTitulo.Draw(page, recEnc7);

            RectangleF recEnc8 = new RectangleF(recEnc7.X + recEnc7.Width, AlturaEncabezadoTabla, anchoHead[8], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc8);
            textTitulo = new PdfTextElement("Observaciones", fontComun); textTitulo.StringFormat = formatoTxtCentro;
            textTitulo.Draw(page, recEnc8);

            ////////////////CREAR TABLA////////////////
            PdfGrid pdfGrid = new PdfGrid();// Create a PdfLightTable.
            PdfGrid pdfGridTMP = new PdfGrid();// Create a PdfLightTable.
            DataTable table = new DataTable();// Initialize DataTable to assign as DateSource to the light table.
            table.Columns.Add("Ítem");//Include columns to the DataTable.
            table.Columns.Add("No folios que contiene");//Include columns to the DataTable.
            table.Columns.Add("Tipo Documental");
            table.Columns.Add("Desde");
            table.Columns.Add("Hasta");
            table.Columns.Add("Fecha Documento");
            table.Columns.Add("Archivado por");
            table.Columns.Add("Fecha Ingreso");
            table.Columns.Add("Observaciones");
            PdfStringFormat stringCentrado = new PdfStringFormat();   //Initialize PdfStringFormat and set the properties
            stringCentrado.Alignment = PdfTextAlignment.Center;
            stringCentrado.LineAlignment = PdfVerticalAlignment.Middle;

            DateTime fechaPrincipal = DateTime.MinValue;
            //Estilo de fila en Table
            PdfGridCellStyle gridCellStyle = new PdfGridCellStyle();     //Initialize PdfGridCellStyle. Set background color and string format
            gridCellStyle.StringFormat = stringCentrado;
            gridCellStyle.Font = fontComun63;

            PdfGridRowStyle pdfGridRowStyle = new PdfGridRowStyle();
            pdfGridRowStyle.Font = fontComun63;
            //pdfGrid

            string usuario = "";
            if (String.IsNullOrEmpty(usuario)) usuario = archivadoPor;
            for (int c = 0; c < lista.Count; c++)
            {
                table.Rows.Add(new string[] { lista[c].item, lista[c].folios, lista[c].tipoDocumental, lista[c].desde, lista[c].hasta, lista[c].fecha, lista[c].archivado, lista[c].fechaIngreso, lista[c].observaciones });//Include rows to the DataTable.
            }

            /*int ttalReg = datDocs.Count();
            int k = (int)Math.Ceiling(ttalReg / 27f);*/
            float KAltura = AlturaEncabezadoTabla + 90;
            if (doc.Pages.Count > 1) KAltura = AlturaEncabezadoTabla - 50;
            float floatDefecto = 25.1f;
            //Calcula el alto x defecto
            pdfGridTMP.DataSource = table;//Assign data source.
            if (table.Rows.Count > 0) floatDefecto = pdfGridTMP.Rows[0].Height + 2;
            float maxAlturaTabla = doc.Pages[doc.Pages.Count - 1].GetClientSize().Height - KAltura;
            int promPagina = 27; int estimaPagina = 1;
            estimaPagina = table.Rows.Count / promPagina; if (estimaPagina == 0) estimaPagina = 1;
            maxAlturaTabla = maxAlturaTabla * estimaPagina;

            pdfGrid.DataSource = table;//Tiene que reasignar de nuevo e source

            pdfGrid.BeginCellLayout += PdfGrid_BeginCellLayoutHC; //quita encabezados
            for (int i = 0; i < anchoHead.Count; i++)   //pone ancho a las columnas
            {
                pdfGrid.Columns[i].Width = anchoHead[i];
            }
            //pone el alto a las Filas
            for (int c = 0; c < table.Rows.Count; c++)
            {
                PdfGridRow gridRow = pdfGrid.Rows[c];
                gridRow.ApplyStyle(gridCellStyle);
                //Console.WriteLine(gridRow.Height);
                gridRow.Height = 26.5f;
                /*if (gridRow.Height < floatDefecto)
                {
                    gridRow.Height = floatDefecto;
                }*/
            }
            //IMPRIME TABAL EN EL PDF
            PdfGridLayoutResult pdfGridLayoutResult = pdfGrid.Draw(page, imageBounds.X, AlturaEncabezadoTabla - 11.5f);//Draw PdfLightTable.

            float totalBorde = recNota.Y;
            pageCount = doc.Pages.Count;

            lastPage = doc.Pages[pageCount - 1];
            var finalTabla = pdfGridLayoutResult.Bounds.Bottom;
            textNota = new PdfTextElement(nota2, fontComun);
            RectangleF recNota2 = new RectangleF(imageBounds.X + 30, finalTabla + 3, 495, page.GetClientSize().Height);
            textNota.Draw(lastPage, recNota2);
            //Console.WriteLine(pdfGridLayoutResult.Bounds.Height + ((640) * (pageCount - 1)) + recNota.Y + 20);

            //FOOTER
            PdfFont fontPie = new PdfStandardFont(PdfFontFamily.Helvetica, 6);//Set the standard font.
            graphicsPag.DrawString("Desarrollado por Alpha Intelligence AI", fontPie, PdfBrushes.Black, new PointF(470, recNota2.Y + 20), formatoTxtCentrado);//Draw the text.

            //CUADRO BORDE DE TABLA
            PdfLayoutFormat format = new PdfLayoutFormat();
            format.Break = PdfLayoutBreakType.FitPage;
            format.Layout = PdfLayoutType.Paginate; //Width = 612 Height = 792
            format.PaginateBounds = new RectangleF(0, 0, 612, 792 - 60);
            RectangleF rect = new RectangleF(0, header.Y, 532, 860 + 20);
            PdfRectangle recBordeTabla = new PdfRectangle(rect);
            recBordeTabla.Draw(page, 0, AlturaEncabezadoLinea1 - 8.5f, format);

            //RectangleF recBorde = new RectangleF(0, AlturaEncabezadoLinea1 - 10, 532, finalTabla - AlturaEncabezadoLinea1 + 23);
            //page.Graphics.DrawRectangle(borde, recBorde);
            /*
            ///TXT Total Folios
            textNota = new PdfTextElement("Folios Carpeta", fontFormatoNegritaV2);
            RectangleF recTotal = new RectangleF(recNota2.X, finalTabla + 30, 60, 10);
            textNota.Draw(lastPage, recTotal);

            RectangleF rectTotalFolios = new RectangleF(recTotal.X + 60, recTotal.Y, 30, 10);
            lastPage.Graphics.DrawRectangle(borde, rectTotalFolios);
            lastPage.Graphics.DrawString(foliosTotal.ToString(), fontFormatoNegritaV2, PdfBrushes.Black, new PointF(rectTotalFolios.X + (rectTotalFolios.Width / 2), recTotal.Y + 1), formatoTxtCentrado);

            lastPage.Graphics.DrawString("Hoja " + pageCount + " de " + pageCount, fontFormatoNegritaV2, PdfBrushes.Black, new PointF(recEncHC1.X, recTotal.Y + 1), formatoTxtCentrado);
            */
        }

        private void HC4_AddPage(int idCarpeta, PdfDocument doc, List<regHC2> lista, string fechApertura, p_formato mipFormato, string nro_carpeta, string nom_expediente)
        {
            string hc_titulo1 = "", hc_titulo2 = "", hc_titulo3 = "", hc_cal_codigo = "", hc_cal_version = "", hc_cal_fecha = "", nombres = "", nroExp = "", archivadoPor = "", nomExpediente = "", numCaja = "", nomSubdependencia = "", nota1 = "", nota2 = "", nroCarpeta = "", hc_ini = "", hc_fin = "";
            int idProyecto = 0, numHCini = 0, numHCfin = 0;
            conteo = 0;
            RectangleF imageBounds = new RectangleF(4, 9, 84, 22);
            PdfPage page = doc.Pages.Add();//Add a page to the document.
            int pageCount = doc.Pages.Count;
            var lastPage = doc.Pages[pageCount - 1];

            RectangleF recHeader = new RectangleF(0, 0, doc.Pages[0].GetClientSize().Width, 50);
            RectangleF recFooter = new RectangleF(0, 0, doc.Pages[0].GetClientSize().Width, 10);
            PdfPageTemplateElement header = new PdfPageTemplateElement(recHeader);
            PdfPageTemplateElement footer = new PdfPageTemplateElement(recFooter);
            PdfPen borde = new PdfPen(Color.Black, 1);
            PdfGraphics graphicsPag = page.Graphics;
            //PdfGraphics graphicsHeader = header.Graphics;
            PdfGraphics graphicsFooter = footer.Graphics;

            //Consulta base de datos
            var datHC = EntitiesRepository.Entities.t_carpeta.Include("t_tercero").Include("t_lote").Include("p_proyecto").AsNoTracking()
                    .Where(f => f.id == idCarpeta)
                    .Select(p => new { p.nro_caja, p.t_lote.t_carpeta, p.t_tercero, p.t_lote.p_subdependencia, p.t_lote.p_proyecto, p.nro_carpeta, p.nro_expediente, p.nom_expediente, p.hc_inicio, p.hc_fin });
            var dataFormato = datHC.FirstOrDefault();
            if (dataFormato != null)
            {
                if (dataFormato.t_tercero != null) nombres = dataFormato.t_tercero.nombres + " " + dataFormato.t_tercero.apellidos;
                if (mipFormato != null)
                {
                    if (mipFormato.hc_titulo1 != null) hc_titulo1 = mipFormato.hc_titulo1;
                    if (mipFormato.hc_titulo2 != null) hc_titulo2 = mipFormato.hc_titulo2;
                    if (mipFormato.hc_titulo3 != null) hc_titulo3 = mipFormato.hc_titulo3;
                    if (mipFormato.hc_cal_codigo != null) hc_cal_codigo = mipFormato.hc_cal_codigo;
                    if (mipFormato.hc_cal_version != null) hc_cal_version = mipFormato.hc_cal_version;
                    if (mipFormato.hc_cal_fecha != null) hc_cal_fecha = mipFormato.hc_cal_fecha;
                    nota1 = mipFormato.hc_nota1;
                    nota2 = mipFormato.hc_nota2;
                }
                if (dataFormato.nro_caja != null) numCaja = dataFormato.nro_caja;
                if (dataFormato.nro_expediente != null) nroExp = dataFormato.nro_expediente;
                if (dataFormato.nom_expediente != null) nomExpediente = dataFormato.nom_expediente;
                //if (dataFormato.tomo != null) tomo = dataFormato.tomo;
                if (dataFormato.hc_inicio != null)
                {
                    hc_ini = dataFormato.hc_inicio;
                    numHCini = GlobalClass.GetNumber(hc_ini) + pageCount - 1;
                    //numHCfin = GlobalClass.GetNumber(hc_ini) + totalPag - 1;
                }
                if (dataFormato.hc_fin != null)
                {
                    hc_fin = dataFormato.hc_fin;
                    numHCfin = GlobalClass.GetNumber(hc_fin);
                    if (numHCini > numHCfin) numHCfin = numHCini;
                }
                if (dataFormato.nro_carpeta != null) nroCarpeta = dataFormato.nro_carpeta.ToString();
                archivadoPor = dataFormato.p_proyecto.nom_proyecto;
                idProyecto = dataFormato.p_proyecto.id;
                nomSubdependencia = dataFormato.p_subdependencia.nombre;
            }

            //HEADER
            RectangleF recImagen = new RectangleF(0.5f, 0.5f, 170, 42); //Borde
            graphicsPag.DrawRectangle(borde, recImagen);
            ////////LOGO
            if (File.Exists("logo_" + idProyecto + ".png"))
            {
                PdfBitmap image = new PdfBitmap("logo_" + idProyecto + ".png");
                graphicsPag.DrawImage(image, 4, 4, 160, 34);//Draw the image
            }

            ////////TITULO
            /////Create new PDF gradient brush.
            PdfLinearGradientBrush brush = new PdfLinearGradientBrush(new PointF(0, 0), new PointF(1, 1), Color.Red, Color.Blue);
            brush.Background = Color.FromArgb(51, 102, 204);
            RectangleF recTitulo = new RectangleF(170.5f, 0.5f, 280, 14); //Borde
            graphicsPag.DrawRectangle(brush, recTitulo);
            graphicsPag.DrawRectangle(borde, recTitulo);
            RectangleF recTitulo2 = new RectangleF(170.5f, 0.5f, 280, 28); //Borde
            graphicsPag.DrawRectangle(borde, recTitulo2);
            RectangleF recTitulo3 = new RectangleF(170.5f, 0.5f, 280, recImagen.Height); //Borde
            graphicsPag.DrawRectangle(borde, recTitulo3);
            PdfStringFormat formatoTxtCentrado = new PdfStringFormat(); formatoTxtCentrado.Alignment = PdfTextAlignment.Center;
            PdfStringFormat formatoTxtCentro = new PdfStringFormat(); formatoTxtCentro.Alignment = PdfTextAlignment.Center; formatoTxtCentro.LineAlignment = PdfVerticalAlignment.Middle;
            PdfStringFormat formatoTxtIzquierda = new PdfStringFormat(); formatoTxtIzquierda.Alignment = PdfTextAlignment.Left;
            PdfFont fontTitulo = new PdfStandardFont(PdfFontFamily.Helvetica, 7, PdfFontStyle.Bold);//Set the standard font.
            graphicsPag.DrawString(hc_titulo1, fontTitulo, PdfBrushes.White, new PointF(310, 4), formatoTxtCentrado);//Draw the text.
            graphicsPag.DrawString(hc_titulo2, fontTitulo, PdfBrushes.Black, new PointF(310, 17), formatoTxtCentrado);//Draw the text.
            graphicsPag.DrawString(hc_titulo3, fontTitulo, PdfBrushes.Black, new PointF(310, 32), formatoTxtCentrado);//Draw the text.

            ////////CALIDAD
            PdfFont fontFormatoNegrita = new PdfStandardFont(PdfFontFamily.Helvetica, 5, PdfFontStyle.Bold);
            PdfFont fontFormatoNegritaV2 = new PdfStandardFont(PdfFontFamily.Helvetica, 7, PdfFontStyle.Bold);
            RectangleF recCalidad = new RectangleF(450.5f, 0.5f, 81, 42); //Borde
            graphicsPag.DrawRectangle(borde, recCalidad);
            recCalidad = new RectangleF(450.5f, 0.5f, 81, 10.5f); //Borde
            graphicsPag.DrawRectangle(borde, recCalidad);
            recCalidad = new RectangleF(450.5f, 0.5f, 81, 21); //Borde
            graphicsPag.DrawRectangle(borde, recCalidad);
            recCalidad = new RectangleF(450.5f, 0.5f, 81, 31.5f); //Borde
            graphicsPag.DrawRectangle(borde, recCalidad);
            graphicsPag.DrawString(hc_cal_codigo, fontTitulo, PdfBrushes.Black, new PointF(452, 1.5f), formatoTxtIzquierda);
            graphicsPag.DrawString(hc_cal_version, fontTitulo, PdfBrushes.Black, new PointF(452, 11), formatoTxtIzquierda);
            graphicsPag.DrawString(hc_cal_fecha, fontTitulo, PdfBrushes.Black, new PointF(452, 21.5f), formatoTxtIzquierda);

            PdfPageNumberField pageNumber = new PdfPageNumberField();
            PdfPageCountField count = new PdfPageCountField();
            PdfCompositeField compositeField = new PdfCompositeField(fontFormatoNegritaV2, PdfBrushes.Black, "Páginas {0} de ", pageNumber);
            compositeField.StringFormat = formatoTxtIzquierda;
            compositeField.Draw(graphicsPag, new PointF(452, 32.5f));
            //graphics.DrawString("Páginas 1 de 1", fontTitulo, PdfBrushes.Black, new PointF(460, 31), formatoTxtIzquierda);

            //RectangleF recCalidad = new RectangleF(0.5f, 0, 612, recImagen.Height); //Borde
            //graphicsHeader.DrawRectangle(borde, recCalidad);
            //doc.Template.Top = header;

            //doc.Template.Bottom = footer;

            ///////ENCABEZADO
            ///LÍNEA 1
            ///TXT HOja de control
            var AlturaEncabezadoLinea1 = recImagen.Height + 9;
            PdfFont fontComun = new PdfStandardFont(PdfFontFamily.Helvetica, 6);
            PdfFont fontComun63 = new PdfStandardFont(PdfFontFamily.Helvetica, 6.3f);
            graphicsPag.DrawString("Hoja de Control No.", fontComun, PdfBrushes.Black, new PointF(imageBounds.X, AlturaEncabezadoLinea1), formatoTxtIzquierda);
            graphicsPag.DrawString(pageCount.ToString(), fontComun, PdfBrushes.Black, new PointF(recTitulo.X - 12, AlturaEncabezadoLinea1), formatoTxtCentrado);
            RectangleF recEncHC1 = new RectangleF(recTitulo.X - 22, AlturaEncabezadoLinea1 - 2, 22, 12);
            page.Graphics.DrawRectangle(borde, recEncHC1);
            graphicsPag.DrawString(" de ", fontComun, PdfBrushes.Black, new PointF(recEncHC1.X + 26, AlturaEncabezadoLinea1), formatoTxtIzquierda);
            RectangleF recEncHC2 = new RectangleF(recEncHC1.X + 40, AlturaEncabezadoLinea1 - 2, 22, 12);
            page.Graphics.DrawRectangle(borde, recEncHC2);
            float xHCfinal = recEncHC2.X + (recEncHC2.Width / 2);
            float yHCfinal = AlturaEncabezadoLinea1;
            PdfCompositeField compositeField2 = new PdfCompositeField(fontComun, PdfBrushes.Black, "{0}", count);
            compositeField2.StringFormat = formatoTxtIzquierda;
            //compositeField2.Draw(graphicsPag, new PointF(xHCfinal-2, yHCfinal));
            //graphicsPag.DrawString(count.ToString(), fontComun, PdfBrushes.Black, new PointF(xHCfinal, yHCfinal), formatoTxtCentrado);

            ///LÍNEA 2
            var AlturaEncabezadoLinea2 = AlturaEncabezadoLinea1 + 16;
            graphicsPag.DrawString("Carpeta o Expediente No.", fontComun, PdfBrushes.Black, new PointF(imageBounds.X, AlturaEncabezadoLinea2), formatoTxtIzquierda);
            graphicsPag.DrawString(nro_carpeta, fontComun, PdfBrushes.Black, new PointF(recEncHC1.X + 30, AlturaEncabezadoLinea2), formatoTxtCentrado);
            RectangleF recEncExp = new RectangleF(recEncHC1.X, AlturaEncabezadoLinea2 - 2, recEncHC1.Width + recEncHC2.Width + 19, 12);
            page.Graphics.DrawRectangle(borde, recEncExp);
            ///TXT Dependencia
            graphicsPag.DrawString("Caja No.", fontComun, PdfBrushes.Black, new PointF(recEncExp.X + recEncExp.Width + 10, AlturaEncabezadoLinea2), formatoTxtIzquierda);
            graphicsPag.DrawString(numCaja, fontComun, PdfBrushes.Black, new PointF(392, AlturaEncabezadoLinea2), formatoTxtCentrado);
            RectangleF recCaja = new RectangleF(recEncHC2.X + 70, AlturaEncabezadoLinea2 - 2, 268, 12);
            page.Graphics.DrawRectangle(borde, recCaja);

            ///LÍNEA 3
            var AlturaEncabezadoLinea3 = recEncExp.Y + 19;
            graphicsPag.DrawString("Nombre del Expediente o del declarante", fontComun, PdfBrushes.Black, new PointF(imageBounds.X, AlturaEncabezadoLinea3), formatoTxtIzquierda);
            //graphicsPag.DrawString(nomExpediente, fontComun, PdfBrushes.Black, new PointF(recEncHC1.X + 189, AlturaEncabezadoLinea3), formatoTxtCentrado);
            RectangleF recEncNomExp = new RectangleF(recEncHC1.X, AlturaEncabezadoLinea3 - 2, 378, 18);
            page.Graphics.DrawRectangle(borde, recEncNomExp);
            PdfTextElement txtpdfExpediente = new PdfTextElement(nom_expediente, fontComun);
            recEncNomExp.X = recEncNomExp.X; recEncNomExp.Y = recEncNomExp.Y + 1; recEncNomExp.Width = recEncNomExp.Width - 5; recEncNomExp.Height = recEncNomExp.Height - 2;
            txtpdfExpediente.StringFormat = formatoTxtCentro;
            txtpdfExpediente.Draw(page, recEncNomExp);
            ///LÍNEA 4
            ///TXT Fecha apertura hoja de control
            var AlturaEncabezadoLinea4 = recEncNomExp.Y + 24;
            graphicsPag.DrawString("Fecha de apertura hoja de control", fontComun, PdfBrushes.Black, new PointF(imageBounds.X, AlturaEncabezadoLinea4), formatoTxtIzquierda);
            RectangleF recEncFechaIni = new RectangleF(recEncHC1.X, AlturaEncabezadoLinea4 - 2, recEncHC1.Width + recEncHC2.Width + 18, 12);
            page.Graphics.DrawRectangle(borde, recEncFechaIni);
            graphicsPag.DrawString(fechApertura, fontComun, PdfBrushes.Black, new PointF(recEncHC1.X + 30, AlturaEncabezadoLinea4), formatoTxtCentrado);
            ///TXT Dependencia
            graphicsPag.DrawString("Dependencia", fontComun, PdfBrushes.Black, new PointF(recEncHC2.X + 30, AlturaEncabezadoLinea4), formatoTxtIzquierda);
            graphicsPag.DrawString(nomSubdependencia, fontComun, PdfBrushes.Black, new PointF(392, AlturaEncabezadoLinea4), formatoTxtCentrado);
            RectangleF recDependencia = new RectangleF(recEncHC2.X + 70, AlturaEncabezadoLinea4 - 2, 268, 12);
            page.Graphics.DrawRectangle(borde, recDependencia);
            ///LÍNEA 4
            var AlturaEncabezadoLinea5 = recDependencia.Y + 18;
            PdfTextElement textNota = new PdfTextElement(nota1, fontComun);
            RectangleF recNota = new RectangleF(imageBounds.X, AlturaEncabezadoLinea5, 525, page.GetClientSize().Height);
            textNota.Draw(page, recNota);

            ////////////////CREAR ENCABEZADO TABLA////////////////
            //  ANCHO DE COLUMNAS
            var anchoHead = new List<int>();
            anchoHead.Add(20);  //0 ítem
            anchoHead.Add(30);
            anchoHead.Add(145);//2  Tipo documental
            anchoHead.Add(27);
            anchoHead.Add(27);//4   Folio final
            anchoHead.Add(40);
            anchoHead.Add(60);//6 usuario
            anchoHead.Add(40);
            anchoHead.Add(133);//8 Observaciones

            ////ENCABEZADO TABLA////
            float AlturaEncabezadoTabla = AlturaEncabezadoLinea1 + 95;
            RectangleF recEnc1 = new RectangleF(imageBounds.X, AlturaEncabezadoTabla, anchoHead[0], 40); //ítem
            page.Graphics.DrawRectangle(borde, recEnc1);
            graphicsPag.DrawString("Ítem", fontComun, PdfBrushes.Black, new PointF(imageBounds.X + anchoHead[0] / 2, AlturaEncabezadoTabla + 20), formatoTxtCentrado);

            RectangleF recEnc2 = new RectangleF(recEnc1.X + recEnc1.Width, AlturaEncabezadoTabla, anchoHead[1], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc2);    //Nro Folios
            PdfTextElement textTitulo = new PdfTextElement("No de folios que contiene", fontComun); textTitulo.StringFormat = formatoTxtCentro;
            textTitulo.Draw(page, recEnc2);

            RectangleF recEnc3 = new RectangleF(recEnc2.X + recEnc2.Width, AlturaEncabezadoTabla, anchoHead[2], 40); //Tipo Documental
            page.Graphics.DrawRectangle(borde, recEnc3);
            textTitulo = new PdfTextElement("Tipo Documental", fontComun); textTitulo.StringFormat = formatoTxtCentro;
            textTitulo.Draw(page, recEnc3);

            RectangleF recEnc4 = new RectangleF(recEnc3.X + recEnc3.Width, AlturaEncabezadoTabla, anchoHead[3] + anchoHead[4], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc4);
            textTitulo = new PdfTextElement("Rango de Folios", fontComun); textTitulo.StringFormat = formatoTxtCentro;
            textTitulo.Draw(page, recEnc4);

            RectangleF recEnc41 = new RectangleF(recEnc3.X + recEnc3.Width, AlturaEncabezadoTabla + 20, anchoHead[3], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc41);
            textTitulo = new PdfTextElement("Desde", fontComun); textTitulo.StringFormat = formatoTxtCentro;
            textTitulo.Draw(page, recEnc41);

            RectangleF recEnc42 = new RectangleF(recEnc41.X + recEnc41.Width, AlturaEncabezadoTabla + 20, anchoHead[4], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc42);
            textTitulo = new PdfTextElement("Hasta", fontComun); textTitulo.StringFormat = formatoTxtCentro;
            textTitulo.Draw(page, recEnc42);

            RectangleF recEnc5 = new RectangleF(recEnc4.X + recEnc4.Width, AlturaEncabezadoTabla, anchoHead[5], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc5);
            textTitulo = new PdfTextElement("Fecha del documento (dd/mm/aaaa)", fontComun); textTitulo.StringFormat = formatoTxtCentro;
            textTitulo.Draw(page, recEnc5);

            RectangleF recEnc6 = new RectangleF(recEnc5.X + recEnc5.Width, AlturaEncabezadoTabla, anchoHead[6], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc6);
            textTitulo = new PdfTextElement("Archivado por", fontComun); textTitulo.StringFormat = formatoTxtCentro;
            textTitulo.Draw(page, recEnc6);

            RectangleF recEnc7 = new RectangleF(recEnc6.X + recEnc6.Width, AlturaEncabezadoTabla, anchoHead[7], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc7);
            textTitulo = new PdfTextElement("Fecha de Ingreso del documento", fontComun); textTitulo.StringFormat = formatoTxtCentro;
            textTitulo.Draw(page, recEnc7);

            RectangleF recEnc8 = new RectangleF(recEnc7.X + recEnc7.Width, AlturaEncabezadoTabla, anchoHead[8], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc8);
            textTitulo = new PdfTextElement("Observaciones", fontComun); textTitulo.StringFormat = formatoTxtCentro;
            textTitulo.Draw(page, recEnc8);

            ////////////////CREAR TABLA////////////////
            PdfGrid pdfGrid = new PdfGrid();// Create a PdfLightTable.
            PdfGrid pdfGridTMP = new PdfGrid();// Create a PdfLightTable.
            DataTable table = new DataTable();// Initialize DataTable to assign as DateSource to the light table.
            table.Columns.Add("Ítem");//Include columns to the DataTable.
            table.Columns.Add("No folios que contiene");//Include columns to the DataTable.
            table.Columns.Add("Tipo Documental");
            table.Columns.Add("Desde");
            table.Columns.Add("Hasta");
            table.Columns.Add("Fecha Documento");
            table.Columns.Add("Archivado por");
            table.Columns.Add("Fecha Ingreso");
            table.Columns.Add("Observaciones");
            PdfStringFormat stringCentrado = new PdfStringFormat();   //Initialize PdfStringFormat and set the properties
            stringCentrado.Alignment = PdfTextAlignment.Center;
            stringCentrado.LineAlignment = PdfVerticalAlignment.Middle;

            DateTime fechaPrincipal = DateTime.MinValue;
            //Estilo de fila en Table
            PdfGridCellStyle gridCellStyle = new PdfGridCellStyle();     //Initialize PdfGridCellStyle. Set background color and string format
            gridCellStyle.StringFormat = stringCentrado;
            gridCellStyle.Font = fontComun63;

            PdfGridRowStyle pdfGridRowStyle = new PdfGridRowStyle();
            pdfGridRowStyle.Font = fontComun63;
            //pdfGrid

            string usuario = "";
            if (String.IsNullOrEmpty(usuario)) usuario = archivadoPor;
            for (int c = 0; c < lista.Count; c++)
            {
                table.Rows.Add(new string[] { lista[c].item, lista[c].folios, lista[c].tipoDocumental, lista[c].desde, lista[c].hasta, lista[c].fecha, lista[c].archivado, lista[c].fechaIngreso, lista[c].observaciones });//Include rows to the DataTable.
            }

            /*int ttalReg = datDocs.Count();
            int k = (int)Math.Ceiling(ttalReg / 27f);*/
            float KAltura = AlturaEncabezadoTabla + 90;
            if (doc.Pages.Count > 1) KAltura = AlturaEncabezadoTabla - 50;
            float floatDefecto = 25.1f;
            //Calcula el alto x defecto
            pdfGridTMP.DataSource = table;//Assign data source.
            if (table.Rows.Count > 0) floatDefecto = pdfGridTMP.Rows[0].Height + 2;
            float maxAlturaTabla = doc.Pages[doc.Pages.Count - 1].GetClientSize().Height - KAltura;
            int promPagina = 27; int estimaPagina = 1;
            estimaPagina = table.Rows.Count / promPagina; if (estimaPagina == 0) estimaPagina = 1;
            maxAlturaTabla = maxAlturaTabla * estimaPagina;

            pdfGrid.DataSource = table;//Tiene que reasignar de nuevo e source

            pdfGrid.BeginCellLayout += PdfGrid_BeginCellLayoutHC; //quita encabezados
            for (int i = 0; i < anchoHead.Count; i++)   //pone ancho a las columnas
            {
                pdfGrid.Columns[i].Width = anchoHead[i];
            }
            //pone el alto a las Filas
            for (int c = 0; c < table.Rows.Count; c++)
            {
                PdfGridRow gridRow = pdfGrid.Rows[c];
                gridRow.ApplyStyle(gridCellStyle);
                //Console.WriteLine(gridRow.Height);
                gridRow.Height = 26.5f;
                /*if (gridRow.Height < floatDefecto)
                {
                    gridRow.Height = floatDefecto;
                }*/
            }
            //IMPRIME TABAL EN EL PDF
            PdfGridLayoutResult pdfGridLayoutResult = pdfGrid.Draw(page, imageBounds.X, AlturaEncabezadoTabla - 11.5f);//Draw PdfLightTable.

            float totalBorde = recNota.Y;
            pageCount = doc.Pages.Count;

            lastPage = doc.Pages[pageCount - 1];
            var finalTabla = pdfGridLayoutResult.Bounds.Bottom;
            textNota = new PdfTextElement(nota2, fontComun);
            RectangleF recNota2 = new RectangleF(imageBounds.X + 30, finalTabla + 3, 495, page.GetClientSize().Height);
            textNota.Draw(lastPage, recNota2);
            //Console.WriteLine(pdfGridLayoutResult.Bounds.Height + ((640) * (pageCount - 1)) + recNota.Y + 20);

            //FOOTER
            PdfFont fontPie = new PdfStandardFont(PdfFontFamily.Helvetica, 6);//Set the standard font.
            graphicsPag.DrawString("Desarrollado por Alpha Intelligence AI", fontPie, PdfBrushes.Black, new PointF(470, recNota2.Y + 20), formatoTxtCentrado);//Draw the text.

            //CUADRO BORDE DE TABLA
            PdfLayoutFormat format = new PdfLayoutFormat();
            format.Break = PdfLayoutBreakType.FitPage;
            format.Layout = PdfLayoutType.Paginate; //Width = 612 Height = 792
            format.PaginateBounds = new RectangleF(0, 0, 612, 792 - 60);
            RectangleF rect = new RectangleF(0, header.Y, 532, 860 + 20);
            PdfRectangle recBordeTabla = new PdfRectangle(rect);
            recBordeTabla.Draw(page, 0, AlturaEncabezadoLinea1 - 8.5f, format);

            //RectangleF recBorde = new RectangleF(0, AlturaEncabezadoLinea1 - 10, 532, finalTabla - AlturaEncabezadoLinea1 + 23);
            //page.Graphics.DrawRectangle(borde, recBorde);
            /*
            ///TXT Total Folios
            textNota = new PdfTextElement("Folios Carpeta", fontFormatoNegritaV2);
            RectangleF recTotal = new RectangleF(recNota2.X, finalTabla + 30, 60, 10);
            textNota.Draw(lastPage, recTotal);

            RectangleF rectTotalFolios = new RectangleF(recTotal.X + 60, recTotal.Y, 30, 10);
            lastPage.Graphics.DrawRectangle(borde, rectTotalFolios);
            lastPage.Graphics.DrawString(foliosTotal.ToString(), fontFormatoNegritaV2, PdfBrushes.Black, new PointF(rectTotalFolios.X + (rectTotalFolios.Width / 2), recTotal.Y + 1), formatoTxtCentrado);

            lastPage.Graphics.DrawString("Hoja " + pageCount + " de " + pageCount, fontFormatoNegritaV2, PdfBrushes.Black, new PointF(recEncHC1.X, recTotal.Y + 1), formatoTxtCentrado);
            */
        }


        private string getPrimerPalabra(string txt)
        {
            string t = string.Empty;
            if (!string.IsNullOrEmpty(txt))
            {
                string[] datoscortados = txt.Split(' ');
                t = datoscortados[0];
            }
            return t;
        }

        private string getTercero(int id_documento, int idTercero, bool onlyCedula = false)
        {
           string principal = string.Empty, cc = string.Empty, nombres = string.Empty, apellidos = string.Empty;

            var datDocs = EntitiesRepository.Entities.t_documento_tercero.Include("t_tercero").AsNoTracking().Where(x => x.id_documento == id_documento || x.t_tercero.id == idTercero).OrderBy(x => x.t_tercero.id).ToList();
            //Si no esta den datos básicos busca el principal
            foreach (var item in datDocs)
            {
                if (string.IsNullOrEmpty(principal) && item.sol_principal)
                {
                    cc = item.t_tercero.identificacion;
                    nombres = item.t_tercero.nombres.Trim();
                    apellidos = item.t_tercero.apellidos.Trim();
                    if (!onlyCedula) principal = $@"{cc} {nombres} {apellidos}";
                    if (onlyCedula) principal = $@"{cc}";
                }
            }

            //if (string.IsNullOrEmpty(cc))
            //{
            //    //Si no hay principal busca el primero que ingresaron
            //    foreach (var item in datDocs)
            //    {
            //        if (string.IsNullOrEmpty(principal))
            //        {
            //            cc = item.t_tercero.identificacion;
            //            nombres = item.t_tercero.nombres;
            //            apellidos = item.t_tercero.apellidos;
            //            if (!onlyCedula) principal = $@"{cc} {nombres} {apellidos}";
            //            if (onlyCedula) principal = $@"{cc}";
            //        }
            //    }
            //}

            if (string.IsNullOrEmpty(cc) && idTercero != null && idTercero != 0)
            {
                // Busca terceros en datos básicos
                var datDocs2 = EntitiesRepository.Entities.t_tercero.AsNoTracking().Where(x => x.id == idTercero).ToList();
                foreach (var item in datDocs2)
                {
                    cc = item.identificacion;
                    nombres = item.nombres.Trim();
                    apellidos = item.apellidos.Trim();
                    if (!onlyCedula) principal = $@"{cc} {nombres} {apellidos}";
                    if (onlyCedula) principal = $@"{cc}";
                }
            }
            return principal;
        }


        private string txtPrimeraMayus(string v)
        {
            if (string.IsNullOrEmpty(v)) return string.Empty;
            v = v.ToLowerInvariant();
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(v);
        }

        public async void pdfHojaControl2_ok(int idCarpeta, string selectedPath, bool appendPDF, p_formato mipFormato,bool txtcaja = true)
        {   //p_usuario = id_usuario(Indexó),p_usuario1 = idusr_asignado("Asignado"), p_usuario2 = idusr_control(Control calidad)
            RectangleF imageBounds = new RectangleF(4, 9, 84, 22);
            //Create a new PDF document.
            PdfDocument doc = new PdfDocument();

            doc.PageSettings.Orientation = PdfPageOrientation.Portrait;
            doc.PageSettings.Size = PdfPageSize.Legal; //Width = 532 Height = 712
            doc.PageSettings.Margins.Bottom = 30;

            var datDocs = EntitiesRepository.Entities.t_documento.Include("t_carpeta").Include("t_carpeta_estado").Include("p_usuario") // fecha_registro //.Include("t_documento_resp")
            .Where(f => f.id_carpeta == idCarpeta)
            .Select(p => new { p.p_tipodoc, p.t_carpeta.p_usuario, p.t_carpeta.t_carpeta_estado, p.t_carpeta.tomo, p.t_carpeta.tomo_fin, p.pag_ini, p.pag_fin, p.folio_ini, p.folio_fin, p.fecha, p.observacion, p.item, p.t_carpeta.nro_caja, p.t_carpeta.fecha_indexa }).OrderBy(x => x.folio_ini).ToList();
            int folio_anterior = 0, folio_final = 0, foliosTotal = 0, itemSerial = -1;
            DateTime fechaPrincipal = DateTime.MinValue;
            List<regHC2> listRegistros = new List<regHC2>();
            bool setFechaTitulo = false;
            string fechApertura = "";
            string folderCaja = "";
            int tomo_ini = -1, tomo_fin = -1;
            foreach (var item in datDocs)
            {
                if (tomo_ini == -1) tomo_ini = GlobalClass.GetNumber(item.tomo, 1);
                if (tomo_fin == -1) tomo_fin = GlobalClass.GetNumber(item.tomo_fin, 1);
                regHC2 regHC2 = new regHC2();
                int nc = GlobalClass.GetNumber(item.nro_caja, 1);
                folderCaja = "Caja " + nc.ToString();
                int folios = 0; int pagActual = 0; int nIni = 0; int nFin = 0; string usuario = ""; DateTime fechaReg = DateTime.MinValue; DateTime fechaApertura = DateTime.MaxValue; DateTime fec = DateTime.MinValue;
                int docPrincipal = item.p_tipodoc.principal;
                bool excluir = item.p_tipodoc.excluir; // Si es verdadero debe eliminar la página inicio y fin
                if (!excluir)
                {
                    if (itemSerial == -1) itemSerial = GlobalClass.GetNumber(item.item?.ToString(), 1);
                    if (itemSerial == 0) itemSerial = 1;
                    foreach (var itemResp in item.t_carpeta_estado)
                    {
                        if (itemResp.fase == "I")
                        {
                            if (String.IsNullOrEmpty(usuario)) usuario = getPrimerPalabra(itemResp.p_usuario.nombres) + " " + getPrimerPalabra(itemResp.p_usuario.apellidos);
                            if (itemResp.fecha_estado > fechaReg) fechaReg = itemResp.fecha_estado;
                            if (itemResp.fecha_estado < fechaApertura) fechaApertura = itemResp.fecha_estado;
                        }
                    }
                    if (fechaApertura == DateTime.MaxValue)
                    {
                        if (item.fecha_indexa != null)
                        {
                            fechaApertura = (DateTime)item.fecha_indexa;
                            fechaReg = (DateTime)item.fecha_indexa;
                        }
                        if (String.IsNullOrEmpty(usuario)) usuario = getPrimerPalabra(item.p_usuario.nombres) + " " + getPrimerPalabra(item.p_usuario.apellidos);
                    }
                    //if (String.IsNullOrEmpty(usuario)) usuario = archivadoPor;
                    //Fecha de apertura de hoja de control
                    if (!setFechaTitulo)
                    {
                        fechApertura = fechaApertura.ToString("dd / MM / yyyy", CultureInfo.InvariantCulture);
                        //graphicsPag.DrawString(, fontComun, PdfBrushes.Black, new PointF(recEncHC1.X + 30, AlturaEncabezadoLinea4), formatoTxtCentrado);
                        setFechaTitulo = true;
                    }
                    //int.TryParse(item.pag_ini.ToString(), out pagActual);
                    bool isNumericIni = int.TryParse(item.folio_ini.ToString(), out nIni);
                    bool isNumericFin = int.TryParse(item.folio_fin.ToString(), out nFin);
                    if (isNumericIni && isNumericFin) folios = nFin - nIni + 1;
                    bool isDateFecha = DateTime.TryParse(item.fecha.ToString(), out fec);

                    if (folios == 0) folios = pagActual - folio_anterior;
                    foliosTotal += folios;
                    if (docPrincipal == 1 && isDateFecha) fechaPrincipal = fec;
                    //table.Rows.Add(new string[] { folios.ToString(), item.p_tipodoc.nombre + " " + nombres + ", " + nroExp, item.pag_ini.ToString(), fechaPrincipal.ToString("dd/M/yyyy", CultureInfo.InvariantCulture), archivadoPor, "" });//Include rows to the DataTable.
                    string fechaDoc = fec.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    if (fechaDoc == "01/01/0001" || fechaDoc == "01/01/2001") fechaDoc = "S.F.";

                    regHC2.item = itemSerial.ToString();
                    regHC2.folios = folios.ToString();
                    regHC2.tipoDocumental = item.p_tipodoc.nombre;
                    regHC2.desde = nIni.ToString();
                    regHC2.hasta = nFin.ToString();
                    regHC2.fecha = fechaDoc.ToString();
                    regHC2.archivado = usuario.ToString();
                    regHC2.fechaIngreso = fechaReg.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).Trim();
                    regHC2.observaciones = txtPrimeraMayus(item.observacion);
                    listRegistros.Add(regHC2);

                    //table.Rows.Add(new string[] { itemSerial.ToString(), folios.ToString(), item.p_tipodoc.nombre, nIni.ToString(), nFin.ToString(), fechaDoc, usuario, fechaReg.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), item.observacion });//Include rows to the DataTable.
                    folio_anterior = pagActual;
                    if (nFin > 0) folio_final = nFin;
                    itemSerial++;
                }
                else
                {
                    exluirList.Add(Tuple.Create(item.pag_ini.Value, item.pag_fin.Value));
                }
            }

            if (tomo_ini > tomo_fin) tomo_fin = tomo_ini;
            exluirList.Clear();

            /*//Si es el último TOMO Adicionar 6 filas vacias de comodín
            if( tomo_fin == tomo_ini)
            {
                regHC2 regHC2V = new regHC2(); regHC2V.item = "";
                listRegistros.Add(regHC2V);
                listRegistros.Add(regHC2V);
                listRegistros.Add(regHC2V);
                listRegistros.Add(regHC2V);
                listRegistros.Add(regHC2V);
                listRegistros.Add(regHC2V);
            } */

            List<regHC2> listRegistrosEnviados = new List<regHC2>();

            int paginasReg = 1;
            decimal Division = (decimal)listRegistros.Count / (decimal)27;
            decimal TotalHojas = Math.Ceiling(Division); if (TotalHojas < 1) TotalHojas = 1;
            string txTomo = $@"{tomo_ini} DE {tomo_fin}";
            int regXpagina = 0;
            for (int c = 0; c < listRegistros.Count; c++)
            {
                listRegistrosEnviados.Add(listRegistros[c]);
                if (c + 1 == (paginasReg * 27))
                {
                    HC2_AddPage(idCarpeta, doc, listRegistrosEnviados, fechApertura, (int)TotalHojas, mipFormato, txTomo);
                    regXpagina = listRegistrosEnviados.Count;
                    listRegistrosEnviados.Clear();
                    paginasReg++;
                }
            }
            regXpagina = listRegistrosEnviados.Count;

            if (regXpagina > 0)
            {
                for (int f = regXpagina; f < 27; f++)
                {
                    regHC2 regHC2 = new regHC2();
                    listRegistrosEnviados.Add(regHC2);
                }
                if (listRegistrosEnviados.Count > 0)
                {
                    HC2_AddPage(idCarpeta, doc, listRegistrosEnviados, fechApertura, (int)TotalHojas, mipFormato, txTomo);
                    listRegistrosEnviados.Clear();
                    paginasReg++;
                }
            }

            //HC2
            //PdfDocument doc

            var carpeta = EntitiesRepository.Entities.t_carpeta.Include("t_lote").AsNoTracking().Where(x => x.id == idCarpeta).FirstOrDefault();

            string folderPDF = GlobalClass.ruta_proyecto + $@"/{carpeta.t_lote.nom_lote}/{carpeta.nro_caja}/{carpeta.nro_expediente}/";
            string ruta = folderPDF + $@"{carpeta.nro_expediente}.pdf";
            if (!File.Exists(ruta)) folderPDF = GlobalClass.ruta_proyecto + $@"/{carpeta.t_lote.nom_lote}/{carpeta.nro_caja}/";
            ruta = folderPDF + $@"{carpeta.nro_expediente}.pdf";
            string rutaFinal = $"{selectedPath}/" + folderCaja + "/";
            if (!txtcaja) rutaFinal = $"{selectedPath}/";
            if (!Directory.Exists(rutaFinal)) Directory.CreateDirectory(rutaFinal);

            if (appendPDF && File.Exists(ruta))
            {
                string hcPDF = rutaFinal + idCarpeta + ".pdf";
                doc.Save(hcPDF);
                System.IO.File.Copy(ruta, folderPDF + $@"{carpeta.nro_expediente}_sinHC.pdf", true);
                ruta = folderPDF + $@"{carpeta.nro_expediente}_sinHC.pdf";
                //Close the document.
                //doc.Close(true);

                PdfDocument finalDoc = new PdfDocument();

                // Creates a string array of source files to be merged.
                using (Stream stream1 = File.OpenRead(hcPDF))
                {
                    var document = new Syncfusion.Pdf.Parsing.PdfLoadedDocument(ruta);
                    if (exluirList.Count != 0)
                    {
                        exluirList.Reverse();
                        foreach (var removeRange in exluirList)
                        {
                            for (int i = removeRange.Item2 - 1; i >= removeRange.Item1 - 1; i--)
                            {
                                document.Pages.RemoveAt(i);
                            }
                        }
                        document.Save(ruta);
                        document.Close(true);
                    }
                    using (Stream stream2 = File.OpenRead(ruta))
                    {
                        Stream[] streams = { stream1, stream2 };

                        // Merges PDFDocument.

                        PdfDocument.Merge(finalDoc, streams);

                        //Saves the final document

                        finalDoc.Save($"{selectedPath}/{folderCaja}/" + new FileInfo(ruta).Name.TrimEnd("_sinHC.pdf".ToCharArray()) + ".pdf");

                        //Closes the document

                        finalDoc.Close(true);
                    }
                }

                // Creates a PDF stream for merging.

                doc.Close(true);
                File.Delete(hcPDF);
            }
            else
            {
                //Save the document.
                if(txtcaja) doc.Save($"{selectedPath}/{folderCaja}/00_Hoja_Control_" + carpeta.nro_expediente + ".pdf");
                else doc.Save($"{selectedPath}/00_Hoja_Control_" + carpeta.nro_expediente + ".pdf");

                //Close the document.
                doc.Close(true);
            }
        }


        public void pdfHojaControl3(int idCarpeta, string dirAPP, bool append, p_formato miformato)
        {
            conteo = 0;
            int idProyecto = GlobalClass.id_proyecto;

            RectangleF imageBounds = new RectangleF(4, 9, 84, 22);

            //Create a new PDF document.
            PdfDocument doc = new PdfDocument();

            doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
            doc.PageSettings.Size = PdfPageSize.Legal; //Width = 612 Height = 1008
            doc.PageSettings.Margins.Bottom = 10;
            PdfPage page = doc.Pages.Add();//Add a page to the document.

            RectangleF recHeader = new RectangleF(50, 54, doc.Pages[0].GetClientSize().Width, 122);
            RectangleF recFooter = new RectangleF(50, 492, doc.Pages[0].GetClientSize().Width, 40);
            //PdfPageTemplateElement footer = new PdfPageTemplateElement(recFooter);
            PdfPageTemplateElement header = new PdfPageTemplateElement(recHeader);
            PdfPen borde = new PdfPen(Color.Black, 0.5f);
            PdfGraphics graphicsPag = page.Graphics;
            PdfGraphics graphicsHeader = header.Graphics;
            //PdfGraphics graphicsFooter = footer.Graphics;

            PdfStringFormat formatoTxtCentrado = new PdfStringFormat(); formatoTxtCentrado.Alignment = PdfTextAlignment.Center; formatoTxtCentrado.LineAlignment = PdfVerticalAlignment.Middle;
            PdfStringFormat formatoTxtIzquierda = new PdfStringFormat(); formatoTxtIzquierda.Alignment = PdfTextAlignment.Left;
            PdfFont fontTitulo = new PdfStandardFont(PdfFontFamily.Helvetica, 7, PdfFontStyle.Bold);//Set the standard font
            PdfFont fontData = new PdfStandardFont(PdfFontFamily.Helvetica, 7);//Set the standard font.

            //HEADER
            RectangleF recLogo = new RectangleF(0, 0, 370, 34); //Rectangulo Logo
            RectangleF recTitle = new RectangleF(370, 0, 335, 34); //Rectangolo Titulo
            RectangleF recSpace = new RectangleF(705, 0, 195, 34); //Rectangulo Espacio
            //Variables 
            string codProductor = "", productor = "", marco = "", codSubserie = "", subserie = "", nroExp = "", nomExp = "", nroCaja = "", nroCarpeta = "";
            //Consulta base de datos
            var datHC = EntitiesRepository.Entities.t_carpeta.Include("t_lote").Include("p_subserie").Include("p_subdependencia").Include("p_dependencia").Include("p_trd").AsNoTracking()
                    .Where(f => f.id == idCarpeta)
                    .Select(p => new { p.t_lote.marco, p.nom_expediente, p.nro_expediente, p.nro_caja, p.nro_carpeta, p.t_lote.p_subserie, p.t_lote.p_subdependencia.cod, p.t_lote.p_subdependencia.nombre, p.t_lote.p_subdependencia.p_dependencia.p_trd.nombre_trd });
            var dataFormato = datHC.FirstOrDefault();
            if (dataFormato != null)
            {
                codProductor = dataFormato.cod ?? string.Empty;
                productor = dataFormato.nombre ?? string.Empty;
                marco = dataFormato.nombre_trd ?? string.Empty;
                codSubserie = dataFormato.p_subserie.codigo ?? string.Empty;
                subserie = dataFormato.p_subserie.nombre ?? string.Empty;
                nroExp = dataFormato.nro_expediente ?? string.Empty;
                nomExp = dataFormato.nom_expediente ?? string.Empty;
                nroCaja = dataFormato.nro_caja ?? string.Empty;
                nroCarpeta = dataFormato.nro_carpeta?.ToString() ?? string.Empty;
            }
            if (string.IsNullOrEmpty(nroExp)) return;

            if (!string.IsNullOrEmpty(nroExp))
            {
                EntitiesRepository.Context.Database.ExecuteSqlCommand("exec [dbo].[asignaItemExpediente] @nro_expediente", new SqlParameter("@nro_expediente", nroExp));
            }
            //identifica nombre a Exportar
            string nomExportar = nroExp;
            nomExportar = Regex.Replace(nroExp, @"\s+([^\s]+)", "");
            if (File.Exists($"{dirAPP}/" + "HC_" + nomExportar + ".pdf")) return;

            ////////LOGO
            if (File.Exists("logo_" + idProyecto + ".png"))
            {
                PdfBitmap image = new PdfBitmap("logo_" + idProyecto + ".png");
                graphicsHeader.DrawImage(image, 30, 4, 104, 24);//Draw the image
            }

            graphicsHeader.DrawRectangle(borde, recLogo);
            graphicsHeader.DrawRectangle(borde, recTitle);
            graphicsHeader.DrawString("HOJA DE CONTROL EXPEDIENTES", fontTitulo, PdfBrushes.Black, recTitle, formatoTxtCentrado);//Draw the text.
            graphicsHeader.DrawRectangle(borde, recSpace);

            PdfPageNumberField pageNumber = new PdfPageNumberField();
            PdfPageCountField count = new PdfPageCountField();
            PdfCompositeField compositeField = new PdfCompositeField(fontTitulo, PdfBrushes.Black, "Página {0} de {1}", pageNumber, count);
            compositeField.StringFormat = formatoTxtIzquierda;
            compositeField.Draw(graphicsHeader, new PointF(recSpace.X + (recSpace.Width / 2), 17));

            //Rectangulo Cod. Productor
            RectangleF recCod = new RectangleF(0, 34, 63, 20);
            graphicsHeader.DrawRectangle(borde, recCod);
            graphicsHeader.DrawString("Cod. Productor", fontTitulo, PdfBrushes.Black, recCod, formatoTxtCentrado);//Draw the text.
            RectangleF recCodData = new RectangleF(63, 34, 307, 20); //Rectangulo Cod. Productor - Data
            graphicsHeader.DrawRectangle(borde, recCodData);
            graphicsHeader.DrawString(codProductor, fontData, PdfBrushes.Black, recCodData, formatoTxtCentrado);//Draw the text.

            //Rectangulo Productor
            RectangleF recProd = new RectangleF(370, 34, 167, 20);
            graphicsHeader.DrawRectangle(borde, recProd);
            graphicsHeader.DrawString("Productor", fontTitulo, PdfBrushes.Black, recProd, formatoTxtCentrado);//Draw the text.
            RectangleF recProdData = new RectangleF(537, 34, 363, 20); //Rectangulo Productor - Data
            graphicsHeader.DrawRectangle(borde, recProdData);
            graphicsHeader.DrawString(productor, fontData, PdfBrushes.Black, recProdData, formatoTxtCentrado);//Draw the text.

            //Rectangulo Versión TRD/ TVD
            RectangleF recVersion = new RectangleF(0, 54, 63, 20);
            graphicsHeader.DrawRectangle(borde, recVersion);
            graphicsHeader.DrawString("Versión TRD/ TVD", fontTitulo, PdfBrushes.Black, recVersion, formatoTxtCentrado);//Draw the text.
            RectangleF recVersionData = new RectangleF(63, 54, 63, 20); //Rectangulo Versión TRD/ TVD - Data
            graphicsHeader.DrawRectangle(borde, recVersionData);
            graphicsHeader.DrawString(marco.Trim(), fontData, PdfBrushes.Black, recVersionData, formatoTxtCentrado);//Draw the text.

            //Rectangulo Cod.
            RectangleF recCodSub = new RectangleF(126, 54, 244, 20);
            graphicsHeader.DrawRectangle(borde, recCodSub);
            graphicsHeader.DrawString("Cod. Subserie", fontTitulo, PdfBrushes.Black, recCodSub, formatoTxtCentrado);//Draw the text.
            RectangleF recCodSubData = new RectangleF(370, 54, 167, 20);//Rectangulo Cod. Subserie- Data
            graphicsHeader.DrawRectangle(borde, recCodSubData);
            graphicsHeader.DrawString(codSubserie, fontData, PdfBrushes.Black, recCodSubData, formatoTxtCentrado);//Draw the text.

            //Rectangulo Subserie Documental
            RectangleF recCodSubDoc = new RectangleF(537, 54, 91, 20);
            graphicsHeader.DrawRectangle(borde, recCodSubDoc);
            graphicsHeader.DrawString("Subserie Documental", fontTitulo, PdfBrushes.Black, recCodSubDoc, formatoTxtCentrado);//Draw the text.
            RectangleF recCodSubDocData = new RectangleF(628, 54, 272, 20);//Rectangulo Subserie Documental - Data
            graphicsHeader.DrawRectangle(borde, recCodSubDocData);
            graphicsHeader.DrawString(subserie, fontData, PdfBrushes.Black, recCodSubDocData, formatoTxtCentrado);//Draw the text.

            //Nombre Expediente
            RectangleF recExped = new RectangleF(0, 74, 63, 20);
            graphicsHeader.DrawRectangle(borde, recExped);
            graphicsHeader.DrawString("Nombre Expediente", fontTitulo, PdfBrushes.Black, recExped, formatoTxtCentrado);//Draw the text.
            RectangleF recExpedData = new RectangleF(63, 74, 474, 20); //Nombre Expediente - Data
            graphicsHeader.DrawRectangle(borde, recExpedData);
            graphicsHeader.DrawString(nomExp, fontData, PdfBrushes.Black, recExpedData, formatoTxtCentrado);//Draw the text.

            //Caja (s)
            RectangleF recVacio = new RectangleF(537, 74, 45, 20);
            graphicsHeader.DrawRectangle(borde, recVacio);
            RectangleF recExpedCaja = new RectangleF(582, 74, 46, 20); //Caja
            graphicsHeader.DrawRectangle(borde, recExpedCaja);
            graphicsHeader.DrawString("Caja (s)", fontTitulo, PdfBrushes.Black, recExpedCaja, formatoTxtCentrado);//Draw the text.
            RectangleF recExpedCajaData = new RectangleF(628, 74, 79, 20); //Caja-Data
            graphicsHeader.DrawRectangle(borde, recExpedCajaData);
            graphicsHeader.DrawString(nroCaja, fontData, PdfBrushes.Black, recExpedCajaData, formatoTxtCentrado);//Draw the text.

            //Carpeta (s)
            RectangleF recCarpeta = new RectangleF(707, 74, 60, 20);
            graphicsHeader.DrawRectangle(borde, recCarpeta);
            graphicsHeader.DrawString("Carpeta (s)", fontTitulo, PdfBrushes.Black, recCarpeta, formatoTxtCentrado);//Draw the text.
            RectangleF recCarpetaData = new RectangleF(767, 74, 133, 20); //Carpeta - Data
            graphicsHeader.DrawRectangle(borde, recCarpetaData);
            graphicsHeader.DrawString(nroCarpeta, fontData, PdfBrushes.Black, recCarpetaData, formatoTxtCentrado);//Draw the text.

            //Espacio Vacio
            RectangleF recVacio2 = new RectangleF(0, 95, 900, 8);
            graphicsHeader.DrawRectangle(new PdfPen(Color.White, 1), recVacio2);

            //TABLA
            DataTable table = new DataTable();// Initialize DataTable to assign as DateSource to the light table.
            table.Columns.Add("FECHA");
            table.Columns.Add("RADICADO");
            table.Columns.Add("TIPO DOCUMENTAL");
            table.Columns.Add("ASUNTO");
            table.Columns.Add("FOLIO INICIAL");
            table.Columns.Add("FOLIO FINAL");
            table.Columns.Add("Total Folios");
            table.Columns.Add("Tomo");
            table.Columns.Add("MAGNETICO");
            table.Columns.Add("OBSERVACIONES");

            //DATA TABLA
            var datDocs = EntitiesRepository.Entities.t_documento.Include("t_documento_resp").Include("p_tipoitem")
                            .Where(f => f.t_carpeta.nom_expediente == nomExp && f.t_carpeta.nro_caja == nroCaja && f.p_tipodoc.excluir == false)
                            .Select(p => new { p.fecha, p.pag_ini, p.folio_ini, p.folio_fin, p.t_carpeta.tomo, p.observacion, p.nom_doc, p.p_tipodoc.nombre, p.t_documento_resp, p.item }).OrderBy(x => x.item).ToList();
            int folio_anterior = 0; int folio_final = 0;
            DateTime fechaPrincipal = DateTime.MinValue;
            DateTime fechaMin = DateTime.MinValue;

            foreach (var item in datDocs)
            {
                string fecha = "S/F", radicado = "SIN RADICADO", tipoDocumental = "", asunto = "", tomo = "", magnetico = "", observacion = "";
                int folios = 0; int pagActual = 0; int nIni = 0; int nFin = 0;
                DateTime fec = DateTime.MinValue;
                if (item.fecha != null && item.fecha > fechaMin) fecha = item.fecha?.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
                foreach (var itemResp in item.t_documento_resp)
                {
                    string descr = itemResp.p_tipoitem.descripcion.Trim().ToUpper();
                    string tipo = itemResp.p_tipoitem.type.Trim().ToUpper();
                    string vr = itemResp.valor.Trim().ToUpper();
                    if (descr == "NRO DOCUMENTO" && vr != "0" && !string.IsNullOrEmpty(vr)) radicado = vr;
                    if (magnetico == "" && tipo == "NOTA" && descr == "MAGNETICO") magnetico = vr;
                }
                tipoDocumental = item.nombre;
                nIni = GlobalClass.GetNumber(item.folio_ini.ToString());
                nFin = GlobalClass.GetNumber(item.folio_fin.ToString());
                folios = nFin - nIni + 1;
                asunto = item.nom_doc;
                tomo = item.tomo;
                observacion = item.observacion;
                table.Rows.Add(new string[] { fecha, radicado, tipoDocumental, asunto, nIni.ToString(), nFin.ToString(), folios.ToString(), tomo, magnetico, observacion });//Include rows to the DataTable.
                folio_anterior = pagActual;
                if (nFin > 0) folio_final = nFin;
            }

            //ANCHO DE TABLA
            var anchoHead = new List<int>();
            anchoHead.Add(63);  //0) FECHA DD/MM/AAAA
            anchoHead.Add(63);  //1) RADICADO
            anchoHead.Add(244); //2) TIPO DOCUMENTAL
            anchoHead.Add(167); //3)ASUNTO
            anchoHead.Add(45);  //4)FOLIO INICIAL
            anchoHead.Add(46);  //5)FOLIO FINAL
            anchoHead.Add(39);  //6)Total Folios
            anchoHead.Add(39);  //7)TOMO
            anchoHead.Add(61);  //8)MAGNETICO
            anchoHead.Add(133);  //9)OBSERVACIONES

            //SET TABLE TO PDFGRID
            PdfGrid pdfGrid = new PdfGrid();
            pdfGrid.DataSource = table;
            for (int i = 0; i < anchoHead.Count; i++)
            {
                pdfGrid.Columns[i].Width = anchoHead[i];
            }

            //Texto centrado
            PdfStringFormat stringCentrado = new PdfStringFormat();   //Initialize PdfStringFormat and set the properties
            stringCentrado.Alignment = PdfTextAlignment.Center;
            stringCentrado.LineAlignment = PdfVerticalAlignment.Middle;
            PdfFont fontComun = new PdfStandardFont(PdfFontFamily.Helvetica, 6);    //Fuente
            //ESTILO FILAS EN TABLA
            PdfGridCellStyle gridCellStyle = new PdfGridCellStyle();
            gridCellStyle.StringFormat = stringCentrado;
            gridCellStyle.Font = fontComun;
            for (int c = 0; c < table.Rows.Count; c++)
            {
                PdfGridRow gridRow = pdfGrid.Rows[c];
                gridRow.ApplyStyle(gridCellStyle);
            }

            //Rectangulo FECHA DD/MM/AAAA
            RectangleF refecha = new RectangleF(0, 102, anchoHead[0], 20);
            graphicsHeader.DrawRectangle(borde, refecha);
            graphicsHeader.DrawString("FECHA DD/MM/AAAA", fontTitulo, PdfBrushes.Black, refecha, formatoTxtCentrado);

            //RADICADO
            RectangleF reRadi = new RectangleF(63, 102, anchoHead[1], 20);
            graphicsHeader.DrawRectangle(borde, reRadi);
            graphicsHeader.DrawString("RADICADO", fontTitulo, PdfBrushes.Black, reRadi, formatoTxtCentrado);

            //TIPO DOCUMENTAL
            RectangleF reTipoDoc = new RectangleF(126, 102, anchoHead[2], 20);
            graphicsHeader.DrawRectangle(borde, reTipoDoc);
            graphicsHeader.DrawString("TIPO DOCUMENTAL", fontTitulo, PdfBrushes.Black, reTipoDoc, formatoTxtCentrado);

            //ASUNTO
            RectangleF reAsun = new RectangleF(370, 102, anchoHead[3], 20);
            graphicsHeader.DrawRectangle(borde, reAsun);
            graphicsHeader.DrawString("ASUNTO", fontTitulo, PdfBrushes.Black, reAsun, formatoTxtCentrado);

            //FOLIO INICIAL
            RectangleF reFolioIni = new RectangleF(537, 102, anchoHead[4], 20);
            graphicsHeader.DrawRectangle(borde, reFolioIni);
            graphicsHeader.DrawString("FOLIO INICIAL", fontTitulo, PdfBrushes.Black, reFolioIni, formatoTxtCentrado);

            //FOLIO FINAL
            RectangleF reFolioFin = new RectangleF(582, 102, anchoHead[5], 20);
            graphicsHeader.DrawRectangle(borde, reFolioFin);
            graphicsHeader.DrawString("FOLIO FINAL", fontTitulo, PdfBrushes.Black, reFolioFin, formatoTxtCentrado);

            //TOTAL FOLIOS
            RectangleF reTotalFolio = new RectangleF(628, 102, anchoHead[6], 20);
            graphicsHeader.DrawRectangle(borde, reTotalFolio);
            graphicsHeader.DrawString("TOTAL FOLIOS", fontTitulo, PdfBrushes.Black, reTotalFolio, formatoTxtCentrado);

            //TOMO
            RectangleF reTomo = new RectangleF(667, 102, anchoHead[7], 20);
            graphicsHeader.DrawRectangle(borde, reTomo);
            graphicsHeader.DrawString("TOMO", fontTitulo, PdfBrushes.Black, reTomo, formatoTxtCentrado);

            //MAGNÉTICO
            RectangleF reMagnetico = new RectangleF(706, 102, anchoHead[8], 20);
            graphicsHeader.DrawRectangle(borde, reMagnetico);
            graphicsHeader.DrawString("MAGNÉTICO", fontTitulo, PdfBrushes.Black, reMagnetico, formatoTxtCentrado);

            //OBSERVACIONES
            RectangleF reObservaciones = new RectangleF(767, 102, anchoHead[9], 20);
            graphicsHeader.DrawRectangle(borde, reObservaciones);
            graphicsHeader.DrawString("OBSERVACIONES", fontTitulo, PdfBrushes.Black, reObservaciones, formatoTxtCentrado);

            doc.Template.Top = header;

            //Grilla
            pdfGrid.BeginCellLayout += PdfGrid_BeginCellLayout; // Quita encabezado
            PdfGridLayoutResult pdfGridLayoutResult = pdfGrid.Draw(page, refecha.X, refecha.Y + refecha.Height - 23.4f);

            int pageCount = doc.Pages.Count;
            var lastPage = doc.Pages[pageCount - 1];//Identifica última Hoja
            var finalTabla = pdfGridLayoutResult.Bounds.Bottom;

            if (finalTabla < 400f)
            {
                PdfGraphics graphicsLastPag = lastPage.Graphics;
                //ELABORADO POR
                RectangleF recElab = new RectangleF(0, 400, 66, 20);
                graphicsLastPag.DrawRectangle(borde, recElab);
                graphicsLastPag.DrawString("ELABORADO POR", fontTitulo, PdfBrushes.Black, recElab, formatoTxtCentrado);//Draw the text.
                RectangleF recElabData = new RectangleF(66, 400, 306, 20); //Elaborado - Data
                graphicsLastPag.DrawRectangle(borde, recElabData);
                graphicsLastPag.DrawString("Skaphe Tecnología", fontData, PdfBrushes.Black, recElabData, formatoTxtCentrado);//Draw the text.

                //LUGAR Y FECHA
                RectangleF recLug = new RectangleF(0, 420, 66, 20);
                graphicsLastPag.DrawRectangle(borde, recLug);
                graphicsLastPag.DrawString("LUGAR Y FECHA", fontTitulo, PdfBrushes.Black, recLug, formatoTxtCentrado);//Draw the text.
                RectangleF recLugData = new RectangleF(66, 420, 306, 20); //LUGAR Y FECHA - Data
                graphicsLastPag.DrawRectangle(borde, recLugData);
                graphicsLastPag.DrawString("23/09/2020", fontData, PdfBrushes.Black, recLugData, formatoTxtCentrado);//Draw the text.

                //ELABORADO POR
                RectangleF recResp = new RectangleF(600, 400, 66, 20);
                graphicsLastPag.DrawRectangle(borde, recResp);
                graphicsLastPag.DrawString("RESPONSABLE EXPEDIENTE", fontTitulo, PdfBrushes.Black, recResp, formatoTxtCentrado);//Draw the text.
                RectangleF recRespData = new RectangleF(666, 400, 250, 20); //Elaborado - Data
                graphicsLastPag.DrawRectangle(borde, recRespData);
                graphicsLastPag.DrawString("", fontData, PdfBrushes.Black, recRespData, formatoTxtCentrado);//Draw the text.
            }
            if (string.IsNullOrEmpty(nroCaja)) nroCaja = "SIN_CAJA";
            doc.Save($"{dirAPP}/HC_{nroCaja}_{nomExportar}.pdf");
            //doc.Save($"{dirAPP}/" + "HC_" + nomExportar + ".pdf");
            doc.Close(true);

        }

        public void HC4_newPDF(int idCarpeta, string nro_carpeta, string nro_expediente, List<regHC2> listRegistros, string fechApertura, p_formato mipFormato, string nom_exp, string selectedPath, bool appendPDF, string folderCaja, int consecutivo)
        {
            PdfDocument doc = new PdfDocument();            //Create a new PDF document.

            doc.PageSettings.Orientation = PdfPageOrientation.Portrait;
            doc.PageSettings.Size = PdfPageSize.Legal; //Width = 532 Height = 712
            doc.PageSettings.Margins.Bottom = 30;

            List<regHC2> listRegistrosEnviados = new List<regHC2>();
            int regXexp = 0;
            string txKP = $@"KP{nro_carpeta}";
            for (int c = 0; c < listRegistros.Count; c++)
            {
                regXexp = listRegistrosEnviados.Count;
                if (listRegistros[c].nuevo && (regXexp > 0 || regXexp == 27))
                {
                    completarItems(ref listRegistrosEnviados, 27);
                    HC4_AddPage(idCarpeta, doc, listRegistrosEnviados, fechApertura, mipFormato, txKP, nom_exp);
                    listRegistrosEnviados.Clear();
                }

                listRegistrosEnviados.Add(listRegistros[c]);

            }

            List<regHC2> listRegistrosEnviados2 = new List<regHC2>();
            //listRegistrosEnviados2.Clear();

            if (listRegistrosEnviados.Count > 27)
            {
                for (int f2 = 0; f2 < listRegistrosEnviados.Count; f2++)
                {
                    listRegistrosEnviados2.Add(listRegistrosEnviados[f2]);
                    if (esMultiplo((f2 + 1), 27))
                    {
                        HC4_AddPage(idCarpeta, doc, listRegistrosEnviados2, fechApertura, mipFormato, txKP, nom_exp);
                        listRegistrosEnviados2.Clear();
                    }
                }
            }
            else
            {
                completarItems(ref listRegistrosEnviados, 27);
                HC4_AddPage(idCarpeta, doc, listRegistrosEnviados, fechApertura, mipFormato, txKP, nom_exp);
                listRegistrosEnviados.Clear();
            }
            if (listRegistrosEnviados2.Count > 0)
            {
                completarItems(ref listRegistrosEnviados2, 27);
                HC4_AddPage(idCarpeta, doc, listRegistrosEnviados2, fechApertura, mipFormato, txKP, nom_exp);
                listRegistrosEnviados2.Clear();
            }

            //HC4 GUARDA pdf
            if (doc.Pages.Count > 0)
            {


                var carpeta = EntitiesRepository.Entities.t_carpeta.Include("t_lote").AsNoTracking().Where(x => x.id == idCarpeta).FirstOrDefault();

                string folderPDF = GlobalClass.ruta_proyecto + $@"/{carpeta.t_lote.nom_lote}/{carpeta.nro_caja}/{carpeta.nro_expediente}/";
                string ruta = folderPDF + $@"{carpeta.nro_expediente}.pdf";
                if (!File.Exists(ruta)) folderPDF = GlobalClass.ruta_proyecto + $@"/{carpeta.t_lote.nom_lote}/{carpeta.nro_caja}/";
                ruta = folderPDF + $@"{carpeta.nro_expediente}.pdf";
                string rutaFinal = $"{selectedPath}/" + folderCaja + "/";
                if (!Directory.Exists(rutaFinal)) Directory.CreateDirectory(rutaFinal);

                if (appendPDF && File.Exists(ruta))
                {
                    string hcPDF = rutaFinal + idCarpeta + " " + nom_exp + ".pdf";
                    doc.Save(hcPDF);
                    System.IO.File.Copy(ruta, folderPDF + $@"{carpeta.nro_expediente}_sinHC.pdf", true);
                    ruta = folderPDF + $@"{carpeta.nro_expediente}_{nom_exp}_sinHC.pdf";
                    //Close the document.
                    //doc.Close(true);

                    PdfDocument finalDoc = new PdfDocument();

                    // Creates a string array of source files to be merged.
                    using (Stream stream1 = File.OpenRead(hcPDF))
                    {
                        var document = new Syncfusion.Pdf.Parsing.PdfLoadedDocument(ruta);
                        if (exluirList.Count != 0)
                        {
                            exluirList.Reverse();
                            foreach (var removeRange in exluirList)
                            {
                                for (int i = removeRange.Item2 - 1; i >= removeRange.Item1 - 1; i--)
                                {
                                    document.Pages.RemoveAt(i);
                                }
                            }
                            document.Save(ruta);
                            document.Close(true);
                        }
                        using (Stream stream2 = File.OpenRead(ruta))
                        {
                            Stream[] streams = { stream1, stream2 };

                            // Merges PDFDocument.

                            PdfDocument.Merge(finalDoc, streams);

                            //Saves the final document

                            finalDoc.Save($"{selectedPath}/{folderCaja}/" + new FileInfo(ruta).Name.TrimEnd($"- {consecutivo}_sinHC.pdf".ToCharArray()) + ".pdf");

                            //Closes the document

                            finalDoc.Close(true);
                        }
                    }

                    // Creates a PDF stream for merging.

                    doc.Close(true);
                    File.Delete(hcPDF);
                }
                else
                {
                    //Save the document.
                    doc.Save($"{selectedPath}/{folderCaja}/{nro_expediente} - {idCarpeta} - {consecutivo} - {nom_exp}.pdf");

                    //Close the document.
                    doc.Close(true);
                }
            }
        }


        bool esMultiplo(int numero, int multiplo)
        {
            if (numero % multiplo == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
            // También se podría hacer un:
            // return numero % multiplo == 0
        }

        public void completarItems(ref List<regHC2> listRegistrosEnviados, int total)
        {
            int regXexp = listRegistrosEnviados.Count;
            //completa los 27 ítems
            for (int f = regXexp; f < 27; f++)
            {
                regHC2 regHC2 = new regHC2();
                listRegistrosEnviados.Add(regHC2);
            }
        }

        public async void pdfHojaControl4(int idCarpeta, string selectedPath, bool appendPDF, p_formato mipFormato)
        {   //p_usuario = id_usuario(Indexó),p_usuario1 = idusr_asignado("Asignado"), p_usuario2 = idusr_control(Control calidad)
            int consecutivo = 1;
            RectangleF imageBounds = new RectangleF(4, 9, 84, 22);
            var datDocs = EntitiesRepository.Entities.t_documento.AsNoTracking().Include("t_carpeta").Include("t_carpeta_estado").Include("p_usuario")
            .Where(f => f.id_carpeta == idCarpeta)
            .Select(p => new { p.p_tipodoc, p.t_carpeta.t_carpeta_estado, p.t_carpeta.tomo, p.t_carpeta.tomo_fin, p.pag_ini, p.pag_fin, p.folio_ini, p.folio_fin, p.fecha, p.observacion, p.item, p.t_carpeta.nro_caja, p.t_carpeta.nro_expediente, p.t_carpeta.nro_carpeta, p.t_carpeta.id_tercero, p.nro_doc, p.id }).OrderBy(x => x.pag_ini).ToList();
            int folio_anterior = 0, folio_final = 0, foliosTotal = 0, itemSerial = -1;
            DateTime fechaPrincipal = DateTime.MinValue;
            List<regHC2> listRegistros = new List<regHC2>();
            bool setFechaTitulo = false;
            string fechApertura = "", folderCaja = "", nro_carpeta = "", nom_exp = string.Empty, numeroExpediente = string.Empty, NumFUD = string.Empty, NomPrincipal = string.Empty;
            int tomo_ini = -1, tomo_fin = -1, idTercero = 0;
            foreach (var item in datDocs)
            {
                int folios = 0; int pagActual = 0; int nIni = 0; int nFin = 0; string usuario = "", nomDocumento = string.Empty, nomFUD = string.Empty, observacion = string.Empty; DateTime fechaReg = DateTime.MinValue; DateTime fechaApertura = DateTime.MaxValue; DateTime fec = DateTime.MinValue;
                regHC2 regHC2 = new regHC2();
                nro_carpeta = item.nro_carpeta?.ToString();
                numeroExpediente = item.nro_expediente;
                idTercero = GlobalClass.GetNumber(item.id_tercero?.ToString());
                string[] words = item.p_tipodoc.nombre.ToUpper().Split('(');
                nomDocumento = words[0];
                observacion = txtPrimeraMayus(item.observacion);
                //SI ES DOCUMENTO PRINCIPAL DE FUD
                if (((nomDocumento.Contains("FUD") || nomDocumento.Contains("NOVEDAD") || nomDocumento.Contains("SUBSIDIO")) && item.folio_ini == 1) && !string.IsNullOrEmpty(NumFUD))
                {
                    regHC2.nuevo = true;
                    nom_exp = $@"{NumFUD?.Trim()} - {NomPrincipal}";
                    HC4_newPDF(idCarpeta, nro_carpeta, numeroExpediente, listRegistros, fechApertura, mipFormato, nom_exp, selectedPath, appendPDF, folderCaja, consecutivo);
                    listRegistros.Clear();
                    NomPrincipal = string.Empty;
                    NumFUD = string.Empty;
                    setFechaTitulo = false;
                    consecutivo++;
                }
                //Identifica el Número de FUD
                if (string.IsNullOrEmpty(NumFUD) && (nomDocumento.Contains("FUD") || nomDocumento.Contains("NOVEDAD") || nomDocumento.Contains("SUBSIDIO")))
                {
                    if (string.IsNullOrEmpty(item.nro_doc) && !item.nro_expediente.ToString().Contains("KP")) NumFUD = item.nro_expediente.ToString();
                    else NumFUD = item.nro_doc?.ToString().Trim();
                }
                //Identifica el tercero Principal
                if (string.IsNullOrEmpty(NomPrincipal) && (nomDocumento.Contains("FUD") || nomDocumento.Contains("NOVEDAD") || nomDocumento.Contains("SUBSIDIO")))
                {
                    NomPrincipal = getTercero(item.id, idTercero);
                }


                if (tomo_ini == -1) tomo_ini = GlobalClass.GetNumber(item.tomo, 1);
                if (tomo_fin == -1) tomo_fin = GlobalClass.GetNumber(item.tomo_fin, 1);

                int nc = GlobalClass.GetNumber(item.nro_caja, 1);
                folderCaja = "Caja " + nc.ToString();
                int docPrincipal = item.p_tipodoc.principal;
                bool excluir = item.p_tipodoc.excluir; // Si es verdadero debe eliminar la página inicio y fin
                if (!excluir)
                {
                    if (itemSerial == -1) itemSerial = GlobalClass.GetNumber(item.item?.ToString(), 1);
                    if (itemSerial == 0) itemSerial = 1;
                    foreach (var itemResp in item.t_carpeta_estado)
                    {
                        if (itemResp.fase == "I")
                        {
                            if (String.IsNullOrEmpty(usuario)) usuario = getPrimerPalabra(itemResp.p_usuario.nombres) + " " + getPrimerPalabra(itemResp.p_usuario.apellidos);
                            if (itemResp.fecha_estado > fechaReg) fechaReg = itemResp.fecha_estado;
                            if (itemResp.fecha_estado < fechaApertura) fechaApertura = itemResp.fecha_estado;
                        }
                    }
                    //if (String.IsNullOrEmpty(usuario)) usuario = archivadoPor;
                    //Fecha de apertura de hoja de control
                    if (!setFechaTitulo)
                    {
                        fechApertura = fechaApertura.ToString("dd / MM / yyyy", CultureInfo.InvariantCulture);
                        //graphicsPag.DrawString(, fontComun, PdfBrushes.Black, new PointF(recEncHC1.X + 30, AlturaEncabezadoLinea4), formatoTxtCentrado);
                        setFechaTitulo = true;
                    }
                    //int.TryParse(item.pag_ini.ToString(), out pagActual);
                    bool isNumericIni = int.TryParse(item.folio_ini.ToString(), out nIni);
                    bool isNumericFin = int.TryParse(item.folio_fin.ToString(), out nFin);
                    if (isNumericIni && isNumericFin) folios = nFin - nIni + 1;
                    bool isDateFecha = DateTime.TryParse(item.fecha.ToString(), out fec);

                    if (folios == 0) folios = pagActual - folio_anterior;
                    foliosTotal += folios;
                    if (docPrincipal == 1 && isDateFecha) fechaPrincipal = fec;
                    //table.Rows.Add(new string[] { folios.ToString(), item.p_tipodoc.nombre + " " + nombres + ", " + nroExp, item.pag_ini.ToString(), fechaPrincipal.ToString("dd/M/yyyy", CultureInfo.InvariantCulture), archivadoPor, "" });//Include rows to the DataTable.
                    string fechaDoc = fec.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    if (fechaDoc == "01/01/0001" || fechaDoc == "01/01/2001") fechaDoc = "S.F.";
                    if (string.IsNullOrEmpty(observacion) && fechaDoc == "S.F.") observacion = "La declaración no evidencia fecha";

                    regHC2.item = itemSerial.ToString();
                    regHC2.folios = folios.ToString();
                    regHC2.tipoDocumental = nomDocumento;
                    regHC2.desde = nIni.ToString();
                    regHC2.hasta = nFin.ToString();
                    regHC2.fecha = fechaDoc.ToString();
                    regHC2.archivado = $@"{usuario} IMPRETICS";
                    regHC2.fechaIngreso = fechaReg.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    regHC2.observaciones = observacion;
                    listRegistros.Add(regHC2);

                    //table.Rows.Add(new string[] { itemSerial.ToString(), folios.ToString(), item.p_tipodoc.nombre, nIni.ToString(), nFin.ToString(), fechaDoc, usuario, fechaReg.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), item.observacion });//Include rows to the DataTable.
                    folio_anterior = pagActual;
                    if (nFin > 0) folio_final = nFin;
                    itemSerial++;
                }
                else
                {
                    exluirList.Add(Tuple.Create(item.pag_ini.Value, item.pag_fin.Value));
                }
            }

            if (listRegistros.Count > 0)
            {
                nom_exp = $@"{NumFUD?.Trim()} - {NomPrincipal}";
                HC4_newPDF(idCarpeta, nro_carpeta, numeroExpediente, listRegistros, fechApertura, mipFormato, nom_exp, selectedPath, appendPDF, folderCaja, consecutivo);
                listRegistros.Clear();
                NumFUD = string.Empty;
                NomPrincipal = string.Empty;
            }
            if (tomo_ini > tomo_fin) tomo_fin = tomo_ini;
            exluirList.Clear();


        }


        private void PdfGrid_BeginCellLayout(object sender, PdfGridBeginCellLayoutEventArgs args)
        {
            conteo++;
            PdfGrid grid = (sender as PdfGrid);
            if (conteo <= grid.Headers.Count * grid.Columns.Count)
            {
                args.Skip = true;
            }
        }

        private void PdfGrid_BeginCellLayoutFUID(object sender, PdfGridBeginCellLayoutEventArgs args)
        {
            float cx = args.Bounds.X;
            float cy = args.Bounds.Y;
            float ancho = args.Bounds.Right;
            float alto = args.Bounds.Bottom;
            conteo++;
            PdfGrid grid = (sender as PdfGrid);
            if (conteo <= grid.Headers.Count * grid.Columns.Count)
            {
                args.Skip = true;
            }
            else if (args.CellIndex == 4)
            {
                PdfFont fontComunMin = new PdfStandardFont(PdfFontFamily.Helvetica, 5.5f);
                args.Style.Font = fontComunMin;
            }
            else if (args.CellIndex == 16)
            {
                PdfFont fontComunMin2 = new PdfStandardFont(PdfFontFamily.Helvetica, 4.8f);
                args.Style.Font = fontComunMin2;
                args.Skip = true;
                string valObservaciones = args.Value;
                PdfFont fontComunMin = new PdfStandardFont(PdfFontFamily.Helvetica, 5.5f);
                //Html element 
                PdfHTMLTextElement htmlelement = new PdfHTMLTextElement("<html><body>" + valObservaciones.Trim() + "</body></html>", fontComunMin, PdfBrushes.Black);
                htmlelement.TextAlign = TextAlign.Justify;
                var cuadro = args.Bounds;
                cuadro.X += 1;
                cuadro.Y += 1;
                cuadro.Width = cuadro.Width - 4;
                cuadro.Height = cuadro.Height - 4;
                //if (args.RowIndex > 0)
                //{
                if (cuadro.Height > 0) htmlelement.Draw(args.Graphics, cuadro);


                var cuadroBorde = args.Bounds;
                PdfPen borde = new PdfPen(Color.Black, 1);
                RectangleF recMarco = new RectangleF(cx, cy, ancho, alto); //Borde X-6 Y-7
                args.Graphics.DrawRectangle(borde, cuadroBorde);
                //}

            }
            else
            {
                PdfFont fontComun = new PdfStandardFont(PdfFontFamily.Helvetica, 6);
                args.Style.Font = fontComun;
            }
        }


        private void PdfGrid_BeginCellLayoutHC(object sender, PdfGridBeginCellLayoutEventArgs args)
        {
            conteo++;
            PdfGrid grid = (sender as PdfGrid);
            if (conteo <= grid.Headers.Count * grid.Columns.Count)
            {
                args.Skip = true;
            }
            if (args.CellIndex == 2)
            {
                PdfFont fontComun71 = new PdfStandardFont(PdfFontFamily.Helvetica, 7.1f);
                args.Style.Font = fontComun71;
            }
            else if (args.CellIndex == 8)
            {
                PdfFont fontComun5 = new PdfStandardFont(PdfFontFamily.Helvetica, 5.6f);
                args.Style.Font = fontComun5;
            }
            else
            {
                PdfFont fontComun63 = new PdfStandardFont(PdfFontFamily.Helvetica, 6.3f);
                args.Style.Font = fontComun63;
            }
        }

        public void ExportPdfFuid1(string exportPath, int idLote = 0, string nro_caja = null)
        {
            conteo = 0;
            if (idLote == 0 && string.IsNullOrEmpty(nro_caja)) return;
            IQueryable<t_carpeta> datFUID = EntitiesRepository.Entities.t_carpeta.Include("t_tercero").Include("t_lote").Include("p_proyecto").Include("p_formato");
            if (idLote != 0) datFUID = datFUID.Where(c => c.id_lote == idLote);
            if (!string.IsNullOrEmpty(nro_caja)) datFUID = datFUID.Where(c => c.nro_caja == nro_caja);
            var datFUID2 = datFUID.Select(p => new { p.t_lote.p_proyecto.p_formato, p.t_lote.t_carpeta, p.t_tercero, p.t_lote.p_subdependencia, p.t_lote.p_proyecto, p.t_lote.p_subserie });
            var cuentaReg = datFUID2.Count();
            if (cuentaReg == 0)
            {
                MessageBox.Show("No hay datos para el FUID con esos parámetros.");
                return;
            }

            var dataFormato = datFUID2.FirstOrDefault().p_formato.FirstOrDefault();
            string fi_titulo1 = "", fi_titulo2 = "", fi_titulo3 = "", fi_cal_codigo = "", fi_cal_version = "", fi_cal_fecha = "", entProductora = "", undAdmin = "", ofcProductora = "", fi_objeto = "", codSerie = "", codSubSerie = "", fi_elaboradox = "", fi_elaboradox_cargo = "", fi_entregadox = "", fi_entregadox_cargo = "", fi_recibidox = "", fi_recibidox_cargo = "", fi_lugar = "", fi_fecha = "";
            p_organizacion dataOrg;
            p_dependencia dataDependencia;
            p_subdependencia dataSUBDependencia;
            p_subserie dataSUBSerie = datFUID2.FirstOrDefault().p_subserie;
            int idProyecto = 0;
            if (dataFormato != null)
            {
                dataOrg = dataFormato.p_proyecto.p_organizacion;
                dataDependencia = dataOrg.p_dependencia.FirstOrDefault();
                dataSUBDependencia = dataDependencia.p_subdependencia.FirstOrDefault();
                if (!string.IsNullOrEmpty(dataFormato.fi_titulo1)) fi_titulo1 = dataFormato.fi_titulo1;
                if (!string.IsNullOrEmpty(dataFormato.fi_titulo2)) fi_titulo2 = dataFormato.fi_titulo2;
                if (!string.IsNullOrEmpty(dataFormato.fi_titulo3)) fi_titulo3 = dataFormato.hc_titulo3;
                if (!string.IsNullOrEmpty(dataFormato.fi_cal_codigo)) fi_cal_codigo = dataFormato.fi_cal_codigo;
                if (!string.IsNullOrEmpty(dataFormato.fi_cal_version)) fi_cal_version = dataFormato.fi_cal_version;
                if (!string.IsNullOrEmpty(dataFormato.fi_cal_fecha)) fi_cal_fecha = dataFormato.fi_cal_fecha;
                if (!string.IsNullOrEmpty(dataFormato.fi_elaboradox)) fi_elaboradox = dataFormato.fi_elaboradox;
                if (!string.IsNullOrEmpty(dataFormato.fi_elaboradox_cargo)) fi_elaboradox_cargo = dataFormato.fi_elaboradox_cargo;
                if (!string.IsNullOrEmpty(dataFormato.fi_entregadox)) fi_entregadox = dataFormato.fi_entregadox;
                if (!string.IsNullOrEmpty(dataFormato.fi_entregadox_cargo)) fi_entregadox_cargo = dataFormato.fi_entregadox_cargo;
                if (!string.IsNullOrEmpty(dataFormato.fi_recibidox)) fi_recibidox = dataFormato.fi_recibidox;
                if (!string.IsNullOrEmpty(dataFormato.fi_recibidox_cargo)) fi_recibidox_cargo = dataFormato.fi_recibidox_cargo;
                if (!string.IsNullOrEmpty(dataFormato.fi_lugar)) fi_lugar = dataFormato.fi_lugar;
                if (!string.IsNullOrEmpty(dataFormato.fi_fecha)) fi_fecha = dataFormato.fi_fecha;
                if (string.IsNullOrEmpty(dataFormato.fi_fecha)) fi_fecha = DateTime.Now.ToString("dd/MM/yyyy H:mm", CultureInfo.InvariantCulture);
                if (!string.IsNullOrEmpty(dataOrg.nombre)) entProductora = dataOrg.nombre;
                if (!string.IsNullOrEmpty(dataDependencia.und_administrativa)) undAdmin = dataDependencia.und_administrativa;
                if (!string.IsNullOrEmpty(dataSUBDependencia.nombre)) ofcProductora = dataSUBDependencia.nombre;
                if (!string.IsNullOrEmpty(dataFormato.fi_objeto)) fi_objeto = dataFormato.fi_objeto;
                if (!string.IsNullOrEmpty(dataSUBSerie.nombre)) codSerie = dataSUBSerie.codigo;
                if (!string.IsNullOrEmpty(dataSUBSerie.p_serie.nombre)) codSubSerie = dataSUBSerie.p_serie.codigo;
                idProyecto = dataFormato.p_proyecto.id;
            }

            //Create a new PDF document.
            PdfDocument doc = new PdfDocument();

            doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
            doc.PageSettings.Size = PdfPageSize.Letter; //Width = 612 Height = 792
            PdfPage page = doc.Pages.Add();//Add a page to the document.
            RectangleF recHeader = new RectangleF(0, 0, doc.Pages[0].GetClientSize().Width, 50);
            RectangleF recFooter = new RectangleF(0, 0, doc.Pages[0].GetClientSize().Width, 10);
            PdfPageTemplateElement header = new PdfPageTemplateElement(recHeader);
            PdfPageTemplateElement footer = new PdfPageTemplateElement(recFooter);
            PdfPen borde = new PdfPen(Color.Black, 1);
            PdfGraphics graphicsPag = page.Graphics;
            PdfGraphics graphicsHeader = header.Graphics;
            PdfGraphics graphicsFooter = footer.Graphics;
            SizeF pageSize = page.GetClientSize();  //Width = 712 Height = 532  Console.WriteLine(pageSize.Width + " - " + pageSize.Height);
            RectangleF imageBounds = new RectangleF(24, 9, 84, 22);//Setting image bounds
            PdfBitmap image;
            if (File.Exists("logo_" + idProyecto + ".png")) //Logo
            {
                image = new PdfBitmap("logo_" + idProyecto + ".png");
                graphicsHeader.DrawImage(image, imageBounds);//Draw the image
            }
            RectangleF recImagen = new RectangleF(0, 0, 132, 42); //Borde
            graphicsHeader.DrawRectangle(borde, recImagen);
            ////////TITULO
            PdfStringFormat formatoTxtCentrado = new PdfStringFormat(); formatoTxtCentrado.Alignment = PdfTextAlignment.Center;
            PdfStringFormat formatoTxtIzquierda = new PdfStringFormat(); formatoTxtIzquierda.Alignment = PdfTextAlignment.Left;
            PdfFont fontTitulo = new PdfStandardFont(PdfFontFamily.Helvetica, 7, PdfFontStyle.Bold);//Set the standard font.
            graphicsHeader.DrawString(fi_titulo1, fontTitulo, PdfBrushes.Black, new PointF(376, 6), formatoTxtCentrado);//Draw the text.
            graphicsHeader.DrawString(fi_titulo2, fontTitulo, PdfBrushes.Black, new PointF(376, 17), formatoTxtCentrado);//Draw the text.
            graphicsHeader.DrawString(fi_titulo3, fontTitulo, PdfBrushes.Black, new PointF(376, 28), formatoTxtCentrado);//Draw the text.
            RectangleF recTitulo = new RectangleF(132, 0, 458, recImagen.Height); //Borde
            graphicsHeader.DrawRectangle(borde, recTitulo);
            ////////CALIDAD
            PdfFont fontFormatoNegrita = new PdfStandardFont(PdfFontFamily.Helvetica, 5, PdfFontStyle.Bold);
            graphicsHeader.DrawString(fi_cal_codigo, fontTitulo, PdfBrushes.Black, new PointF(600, 4), formatoTxtIzquierda);
            graphicsHeader.DrawString(fi_cal_version, fontTitulo, PdfBrushes.Black, new PointF(600, 13), formatoTxtIzquierda);
            graphicsHeader.DrawString(fi_cal_fecha, fontTitulo, PdfBrushes.Black, new PointF(600, 22), formatoTxtIzquierda);
            //Conteo de PÁGINAS
            PdfPageNumberField pageNumber = new PdfPageNumberField();
            PdfPageCountField count = new PdfPageCountField();
            PdfCompositeField compositeField = new PdfCompositeField(fontTitulo, PdfBrushes.Black, "Páginas {0} de {1}", pageNumber, count);
            compositeField.StringFormat = formatoTxtIzquierda;
            compositeField.Draw(graphicsHeader, new PointF(600, 31));

            RectangleF recCalidad = new RectangleF(590, 0, 122, recImagen.Height); //Borde
            graphicsHeader.DrawRectangle(borde, recCalidad);
            doc.Template.Top = header;
            //FOOTER
            PdfFont fontPie = new PdfStandardFont(PdfFontFamily.Helvetica, 6);//Set the standard font.
            graphicsFooter.DrawString("Desarrollado por Alpha Intelligence AI", fontPie, PdfBrushes.Black, new PointF(50, 0), formatoTxtCentrado);//Draw the text.
            doc.Template.Bottom = footer;

            ///////ENCABEZADO
            ///LÍNEA 1
            PdfPen pen = new PdfPen(PdfBrushes.Black, 0.8f);//Initialize pen to draw the line
            PdfFont fontComun = new PdfStandardFont(PdfFontFamily.Helvetica, 6);
            ///TXT Entidad Remitente
            var AlturaEncabezadoLinea1 = recImagen.Height + 10;
            graphicsPag.DrawString("ENTIDAD REMITENTE", fontComun, PdfBrushes.Black, new PointF(0, AlturaEncabezadoLinea1), formatoTxtIzquierda);
            PointF point1 = new PointF(90, AlturaEncabezadoLinea1 + 7);
            PointF point2 = new PointF(390, AlturaEncabezadoLinea1 + 7);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT Entidad Productora
            var AlturaEncabezadoLinea2 = point2.Y + 7;
            graphicsPag.DrawString("ENTIDAD PRODUCTORA", fontComun, PdfBrushes.Black, new PointF(0, AlturaEncabezadoLinea2), formatoTxtIzquierda);
            graphicsPag.DrawString(entProductora, fontComun, PdfBrushes.Black, new PointF(240, AlturaEncabezadoLinea2), formatoTxtCentrado);
            point1 = new PointF(90, AlturaEncabezadoLinea2 + 7);
            point2 = new PointF(390, AlturaEncabezadoLinea2 + 7);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT Entidad Productora
            var AlturaEncabezadoLinea3 = point2.Y + 7;
            graphicsPag.DrawString("UNIDAD ADMINISTRATIVA", fontComun, PdfBrushes.Black, new PointF(0, AlturaEncabezadoLinea3), formatoTxtIzquierda);
            graphicsPag.DrawString(undAdmin, fontComun, PdfBrushes.Black, new PointF(240, AlturaEncabezadoLinea3), formatoTxtCentrado);
            point1 = new PointF(90, AlturaEncabezadoLinea3 + 7);
            point2 = new PointF(390, AlturaEncabezadoLinea3 + 7);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT OFICINA Productora
            var AlturaEncabezadoLinea4 = point2.Y + 7;
            graphicsPag.DrawString("OFICINA PRODUCTORA", fontComun, PdfBrushes.Black, new PointF(0, AlturaEncabezadoLinea4), formatoTxtIzquierda);
            graphicsPag.DrawString(ofcProductora, fontComun, PdfBrushes.Black, new PointF(240, AlturaEncabezadoLinea4), formatoTxtCentrado);
            point1 = new PointF(90, AlturaEncabezadoLinea4 + 7);
            point2 = new PointF(390, AlturaEncabezadoLinea4 + 7);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            /// OBJETO
            var AlturaEncabezadoLinea5 = point2.Y + 7;
            graphicsPag.DrawString("OBJETO", fontComun, PdfBrushes.Black, new PointF(0, AlturaEncabezadoLinea5), formatoTxtIzquierda);
            graphicsPag.DrawString(fi_objeto, fontComun, PdfBrushes.Black, new PointF(240, AlturaEncabezadoLinea5), formatoTxtCentrado);
            point1 = new PointF(90, AlturaEncabezadoLinea5 + 7);
            point2 = new PointF(390, AlturaEncabezadoLinea5 + 7);
            page.Graphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            //CUADRO REGISTRO DE ENTRADA
            RectangleF recRegentra = new RectangleF(440, AlturaEncabezadoLinea1 + 7, 272, 60); //Borde
            page.Graphics.DrawRectangle(borde, recRegentra);
            //FIla 1
            RectangleF rectxtRegentra = new RectangleF(440, AlturaEncabezadoLinea1 + 7, 272, 15); //Borde
            page.Graphics.DrawRectangle(borde, rectxtRegentra);
            graphicsPag.DrawString("REGISTRO DE ENTRADA", fontComun, PdfBrushes.Black, new PointF(576, AlturaEncabezadoLinea1 + 11), formatoTxtCentrado);
            //Fila 2 COl1
            RectangleF recF2C1 = new RectangleF(440, AlturaEncabezadoLinea1 + 7 + 15, 31, 15); //Borde
            page.Graphics.DrawRectangle(borde, recF2C1);
            graphicsPag.DrawString("AÑO", fontComun, PdfBrushes.Black, new PointF(456, AlturaEncabezadoLinea1 + 11 + 15), formatoTxtCentrado);
            //Fila 2 Col 2
            RectangleF recF2C2 = new RectangleF(recF2C1.X + recF2C1.Width, recF2C1.Y, 65, 15); //Borde
            page.Graphics.DrawRectangle(borde, recF2C2);
            //Fila 2 Col 3
            RectangleF recF2C3 = new RectangleF(recF2C2.X + recF2C2.Width, recF2C1.Y, 45, 15); //Borde
            page.Graphics.DrawRectangle(borde, recF2C3);
            graphicsPag.DrawString("MES", fontComun, PdfBrushes.Black, new PointF(recF2C2.X + recF2C2.Width + 22, AlturaEncabezadoLinea1 + 11 + 15), formatoTxtCentrado);
            //Fila 2 Col 4
            RectangleF recF2C4 = new RectangleF(recF2C3.X + recF2C3.Width, recF2C1.Y, 45, 15); //Borde
            page.Graphics.DrawRectangle(borde, recF2C4);
            graphicsPag.DrawString("DÍA", fontComun, PdfBrushes.Black, new PointF(recF2C3.X + recF2C3.Width + 22, AlturaEncabezadoLinea1 + 11 + 15), formatoTxtCentrado);
            //Fila 2 Col 5
            RectangleF recF2C5 = new RectangleF(recF2C4.X + recF2C4.Width, recF2C1.Y, 91, 15); //Borde
            page.Graphics.DrawRectangle(borde, recF2C5);
            graphicsPag.DrawString("N.T.", fontComun, PdfBrushes.Black, new PointF(recF2C4.X + recF2C4.Width + 45, AlturaEncabezadoLinea1 + 11 + 15), formatoTxtCentrado);
            //FIla 3
            //Fila 3 COl1
            RectangleF recF3C1 = new RectangleF(440, AlturaEncabezadoLinea1 + 7 + 30, 31, 15); //Borde
            page.Graphics.DrawRectangle(borde, recF3C1);
            //Fila 3 Col 2
            RectangleF recF3C2 = new RectangleF(recF2C1.X + recF3C1.Width, recF3C1.Y, 65, 15); //Borde
            page.Graphics.DrawRectangle(borde, recF3C2);
            //Fila 3 Col 3
            RectangleF recF3C3 = new RectangleF(recF3C2.X + recF3C2.Width, recF3C1.Y, 45, 15); //Borde
            page.Graphics.DrawRectangle(borde, recF3C3);
            //Fila 3 Col 4
            RectangleF recF3C4 = new RectangleF(recF3C3.X + recF3C3.Width, recF3C1.Y, 45, 15); //Borde
            page.Graphics.DrawRectangle(borde, recF3C4);
            //Fila 3 Col 5
            RectangleF recF3C5 = new RectangleF(recF3C4.X + recF3C4.Width, recF3C1.Y, 91, 15); //Borde
            page.Graphics.DrawRectangle(borde, recF3C5);
            //FILA 4
            graphicsPag.DrawString("N.T. = Número de Transferencia", fontComun, PdfBrushes.Black, new PointF(recF3C4.X + recF3C4.Width, AlturaEncabezadoLinea1 + 11 + 45), formatoTxtCentrado);
            //  ANCHO DE COLUMNAS
            var anchoHead = new List<int>();
            anchoHead.Add(40);  //0
            anchoHead.Add(50);
            anchoHead.Add(170);//2
            anchoHead.Add(40);
            anchoHead.Add(40);//4
            anchoHead.Add(26);
            anchoHead.Add(26);//6
            anchoHead.Add(26);
            anchoHead.Add(26);//8
            anchoHead.Add(26);
            anchoHead.Add(60);//10
            anchoHead.Add(50);
            anchoHead.Add(50);//12
            anchoHead.Add(82);
            ////ENCABEZADO TABLA////
            graphicsPag.DrawString("N° ORDEN", fontComun, PdfBrushes.Black, new PointF(anchoHead[0] / 2, AlturaEncabezadoLinea1 + 92), formatoTxtCentrado);
            RectangleF recEnc1 = new RectangleF((float)0.5, AlturaEncabezadoLinea1 + 75, anchoHead[0], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc1);
            graphicsPag.DrawString("CÓDIGO", fontComun, PdfBrushes.Black, new PointF(recEnc1.X + recEnc1.Width + (anchoHead[1] / 2), AlturaEncabezadoLinea1 + 92), formatoTxtCentrado);
            RectangleF recEnc2 = new RectangleF(recEnc1.X + recEnc1.Width, AlturaEncabezadoLinea1 + 75, anchoHead[1], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc2);
            graphicsPag.DrawString("NOMBRE DE LAS SERIES, SUBSERIES DOCUMENTALES", fontComun, PdfBrushes.Black, new PointF(recEnc2.X + recEnc2.Width + (anchoHead[2] / 2), AlturaEncabezadoLinea1 + 92), formatoTxtCentrado);
            RectangleF recEnc3 = new RectangleF(recEnc2.X + recEnc2.Width, AlturaEncabezadoLinea1 + 75, anchoHead[2], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc3);
            graphicsPag.DrawString("FECHAS EXTREMAS", fontComun, PdfBrushes.Black, new PointF(recEnc3.X + recEnc3.Width + (anchoHead[3]), AlturaEncabezadoLinea1 + 78), formatoTxtCentrado);
            graphicsPag.DrawString("(dd/mm/aaaa)", fontComun, PdfBrushes.Black, new PointF(recEnc3.X + recEnc3.Width + (anchoHead[3]), AlturaEncabezadoLinea1 + 86), formatoTxtCentrado);
            RectangleF recEnc4 = new RectangleF(recEnc3.X + recEnc3.Width, AlturaEncabezadoLinea1 + 75, anchoHead[3] + anchoHead[4], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc4);
            graphicsPag.DrawString("Inicial", fontComun, PdfBrushes.Black, new PointF(recEnc3.X + recEnc3.Width + (anchoHead[3] / 2), AlturaEncabezadoLinea1 + 102), formatoTxtCentrado);
            RectangleF recEnc41 = new RectangleF(recEnc3.X + recEnc3.Width, AlturaEncabezadoLinea1 + 75 + 20, anchoHead[3], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc41);
            graphicsPag.DrawString("Final", fontComun, PdfBrushes.Black, new PointF(recEnc41.X + recEnc41.Width + (anchoHead[4] / 2), AlturaEncabezadoLinea1 + 102), formatoTxtCentrado);
            RectangleF recEnc42 = new RectangleF(recEnc41.X + recEnc41.Width, AlturaEncabezadoLinea1 + 75 + 20, anchoHead[4], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc42);
            graphicsPag.DrawString("UNIDAD DE CONSERVACIÓN", fontComun, PdfBrushes.Black, new PointF(recEnc42.X + recEnc42.Width + (anchoHead[5] + anchoHead[6]), AlturaEncabezadoLinea1 + 82), formatoTxtCentrado);
            RectangleF recEnc5 = new RectangleF(recEnc4.X + recEnc4.Width, AlturaEncabezadoLinea1 + 75, anchoHead[5] + anchoHead[6] + anchoHead[7] + anchoHead[8], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc5);
            graphicsPag.DrawString("Caja", fontComun, PdfBrushes.Black, new PointF(recEnc4.X + recEnc4.Width + (anchoHead[5] / 2), AlturaEncabezadoLinea1 + 102), formatoTxtCentrado);
            RectangleF recEnc51 = new RectangleF(recEnc4.X + recEnc4.Width, AlturaEncabezadoLinea1 + 75 + 20, anchoHead[5], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc51);
            graphicsPag.DrawString("Carpeta", fontComun, PdfBrushes.Black, new PointF(recEnc51.X + recEnc51.Width + (anchoHead[6] / 2), AlturaEncabezadoLinea1 + 102), formatoTxtCentrado);
            RectangleF recEnc52 = new RectangleF(recEnc51.X + recEnc51.Width, AlturaEncabezadoLinea1 + 75 + 20, anchoHead[6], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc52);
            graphicsPag.DrawString("Tomo", fontComun, PdfBrushes.Black, new PointF(recEnc52.X + recEnc52.Width + (anchoHead[7] / 2), AlturaEncabezadoLinea1 + 102), formatoTxtCentrado);
            RectangleF recEnc53 = new RectangleF(recEnc52.X + recEnc52.Width, AlturaEncabezadoLinea1 + 75 + 20, anchoHead[7], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc53);
            graphicsPag.DrawString("Otros", fontComun, PdfBrushes.Black, new PointF(recEnc53.X + recEnc53.Width + (anchoHead[8] / 2), AlturaEncabezadoLinea1 + 102), formatoTxtCentrado);
            RectangleF recEnc54 = new RectangleF(recEnc53.X + recEnc53.Width, AlturaEncabezadoLinea1 + 75 + 20, anchoHead[8], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc54);
            graphicsPag.DrawString("N°", fontComun, PdfBrushes.Black, new PointF(recEnc5.X + recEnc5.Width + (anchoHead[9] / 2), AlturaEncabezadoLinea1 + 86), formatoTxtCentrado);
            graphicsPag.DrawString("FOLIOS", fontComun, PdfBrushes.Black, new PointF(recEnc5.X + recEnc5.Width + (anchoHead[9] / 2), AlturaEncabezadoLinea1 + 94), formatoTxtCentrado);
            RectangleF recEnc6 = new RectangleF(recEnc5.X + recEnc5.Width, AlturaEncabezadoLinea1 + 75, anchoHead[9], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc6);
            graphicsPag.DrawString("N° DE CAJA", fontComun, PdfBrushes.Black, new PointF(recEnc6.X + recEnc6.Width + (anchoHead[10] / 2), AlturaEncabezadoLinea1 + 92), formatoTxtCentrado);
            RectangleF recEnc7 = new RectangleF(recEnc6.X + recEnc6.Width, AlturaEncabezadoLinea1 + 75, anchoHead[10], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc7);
            graphicsPag.DrawString("SOPORTE", fontComun, PdfBrushes.Black, new PointF(recEnc7.X + recEnc7.Width + (anchoHead[11] / 2), AlturaEncabezadoLinea1 + 92), formatoTxtCentrado);
            RectangleF recEnc8 = new RectangleF(recEnc7.X + recEnc7.Width, AlturaEncabezadoLinea1 + 75, anchoHead[11], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc8);
            graphicsPag.DrawString("FRECUENCIA", fontComun, PdfBrushes.Black, new PointF(recEnc8.X + recEnc8.Width + (anchoHead[12] / 2), AlturaEncabezadoLinea1 + 86), formatoTxtCentrado);
            graphicsPag.DrawString("DE CONSULTA", fontComun, PdfBrushes.Black, new PointF(recEnc8.X + recEnc8.Width + (anchoHead[12] / 2), AlturaEncabezadoLinea1 + 94), formatoTxtCentrado);
            RectangleF recEnc9 = new RectangleF(recEnc8.X + recEnc8.Width, AlturaEncabezadoLinea1 + 75, anchoHead[12], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc9);
            graphicsPag.DrawString("NOTAS", fontComun, PdfBrushes.Black, new PointF(recEnc9.X + recEnc9.Width + (anchoHead[13] / 2), AlturaEncabezadoLinea1 + 92), formatoTxtCentrado);
            RectangleF recEnc10 = new RectangleF(recEnc9.X + recEnc9.Width, AlturaEncabezadoLinea1 + 75, anchoHead[13] - 1, 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc10);
            DataTable table = new DataTable();// Initialize DataTable to assign as DateSource to the light table.
            table.Columns.Add("Orden");//Include columns to the DataTable.
            table.Columns.Add("Codigo");
            table.Columns.Add("Series");
            table.Columns.Add("Inicial");
            table.Columns.Add("Final");
            table.Columns.Add("Caja");
            table.Columns.Add("Carpeta");
            table.Columns.Add("Tomo");
            table.Columns.Add("Otro");
            table.Columns.Add("Folios");
            table.Columns.Add("NumCaja");
            table.Columns.Add("Soporte");
            table.Columns.Add("Crecuencia");
            table.Columns.Add("Nota");
            //datFUID2
            IQueryable<t_carpeta> datListCarpeta = EntitiesRepository.Entities.t_carpeta;
            if (idLote != 0) datListCarpeta = datListCarpeta.Where(c => c.id_lote == idLote);
            if (!string.IsNullOrEmpty(nro_caja)) datListCarpeta = datListCarpeta.Where(c => c.nro_caja == nro_caja);
            int serial = 1; DateTime fecIni; DateTime fecFin;
            foreach (var item in datListCarpeta.OrderBy(x => x.fecha_expediente_ini).ToList())
            {
                DateTime.TryParse(item.fecha_expediente_ini.ToString(), out fecIni);
                DateTime.TryParse(item.fecha_expediente_fin.ToString(), out fecFin);
                string NomTercero = "";
                if (item.t_tercero != null) NomTercero = $@"{item.t_tercero.nombres} {item.t_tercero.apellidos}";
                table.Rows.Add(new string[] { serial.ToString(), codSubSerie, NomTercero, fecIni.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), fecFin.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), "", "X", "", "", item.total_folios.ToString(), item.nro_caja, "FISICO", "MEDIA", item.nro_expediente });//Include rows to the DataTable.
                serial++;
            }
            PdfGrid pdfGrid = new PdfGrid();
            pdfGrid.DataSource = table;
            for (int i = 0; i < anchoHead.Count; i++)
            {
                pdfGrid.Columns[i].Width = anchoHead[i];
            }
            PdfStringFormat stringCentrado = new PdfStringFormat();   //Initialize PdfStringFormat and set the properties
            stringCentrado.Alignment = PdfTextAlignment.Center;
            stringCentrado.LineAlignment = PdfVerticalAlignment.Middle;
            //Estilo de fila en Table
            PdfGridCellStyle gridCellStyle = new PdfGridCellStyle();     //Initialize PdfGridCellStyle. Set background color and string format
            gridCellStyle.StringFormat = stringCentrado;
            gridCellStyle.Font = fontComun;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                PdfGridRow gridRow = pdfGrid.Rows[i];
                gridRow.ApplyStyle(gridCellStyle);
            }
            pdfGrid.BeginCellLayout += PdfGrid_BeginCellLayout;
            PdfGridLayoutResult pdfGridLayoutResult = pdfGrid.Draw(page, new PointF(0, recEnc1.Y + (float)16.5));    //Draw grid to the page of PDF document

            int pageCount = doc.Pages.Count;
            var lastPage = doc.Pages[pageCount - 1];//Identifica última Hoja
            /*float totalBorde = pdfGrid.;
            if (pageCount == 2) totalBorde += 570;
            if (pageCount == 3) totalBorde += 1300;*/

            /*var finalTabla = pdfGridLayoutResult.Bounds.Bottom;
            nota = dataFormato.p_formato.FirstOrDefault().hc_nota2;
            textNota = new PdfTextElement(nota, fontComun);
            RectangleF recNota2 = new RectangleF(imageBounds.X + 30, finalTabla + 3, 495, page.GetClientSize().Height);
            textNota.Draw(lastPage, recNota2);*/
            var lastpagGraphics = lastPage.Graphics;
            var altoFinal = pdfGridLayoutResult.Bounds.Bottom + 15;
            lastpagGraphics.DrawString("Elaborado por: ", fontComun, PdfBrushes.Black, new PointF(4, altoFinal), formatoTxtIzquierda);
            point1 = new PointF(50, altoFinal + 7);
            point2 = new PointF(255, altoFinal + 7);
            lastpagGraphics.DrawString(fi_elaboradox, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), altoFinal), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT Entidad Productora
            AlturaEncabezadoLinea2 = point2.Y + 7;
            lastpagGraphics.DrawString("Cargo:", fontComun, PdfBrushes.Black, new PointF(4, AlturaEncabezadoLinea2), formatoTxtIzquierda);
            point1 = new PointF(50, AlturaEncabezadoLinea2 + 7);
            point2 = new PointF(255, AlturaEncabezadoLinea2 + 7);
            lastpagGraphics.DrawString(fi_elaboradox_cargo, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea2), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT Entidad Productora
            AlturaEncabezadoLinea3 = point2.Y + 7;
            lastpagGraphics.DrawString("Firma", fontComun, PdfBrushes.Black, new PointF(4, AlturaEncabezadoLinea3), formatoTxtIzquierda);
            point1 = new PointF(50, AlturaEncabezadoLinea3 + 7);
            point2 = new PointF(200, AlturaEncabezadoLinea3 + 7);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT OFICINA Productora
            AlturaEncabezadoLinea4 = point2.Y + 7;
            lastpagGraphics.DrawString("Lugar", fontComun, PdfBrushes.Black, new PointF(4, AlturaEncabezadoLinea4), formatoTxtIzquierda);
            point1 = new PointF(50, AlturaEncabezadoLinea4 + 7);
            point2 = new PointF(200, AlturaEncabezadoLinea4 + 7);
            lastpagGraphics.DrawString(fi_lugar, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea4), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            /// OBJETO
            AlturaEncabezadoLinea5 = point2.Y + 7;
            lastpagGraphics.DrawString("Fecha", fontComun, PdfBrushes.Black, new PointF(4, AlturaEncabezadoLinea5), formatoTxtIzquierda);
            point1 = new PointF(50, AlturaEncabezadoLinea5 + 7);
            point2 = new PointF(200, AlturaEncabezadoLinea5 + 7);
            lastpagGraphics.DrawString(fi_fecha, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea5), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document

            if (File.Exists("firmaElabora_" + idProyecto + ".png")) //Logo
            {
                image = new PdfBitmap("firmaElabora_" + idProyecto + ".png");
                imageBounds = new RectangleF(205, AlturaEncabezadoLinea3, 50, 30);//Setting image bounds
                lastpagGraphics.DrawImage(image, imageBounds);//Draw the image
            }
            //RECTANGULO
            RectangleF recFin1 = new RectangleF((float)0.5, altoFinal - 5, 260, 75); //Borde
            lastpagGraphics.DrawRectangle(borde, recFin1);

            lastpagGraphics.DrawString("Entregado por: ", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + 4, altoFinal), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + 50, altoFinal + 7);
            point2 = new PointF(recFin1.Width + 205, altoFinal + 7);
            lastpagGraphics.DrawString(fi_entregadox, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), altoFinal), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT Entidad Productora
            AlturaEncabezadoLinea2 = point2.Y + 7;
            lastpagGraphics.DrawString("Cargo:", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + 4, AlturaEncabezadoLinea2), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + 50, AlturaEncabezadoLinea2 + 7);
            point2 = new PointF(recFin1.Width + 205, AlturaEncabezadoLinea2 + 7);
            lastpagGraphics.DrawString(fi_entregadox_cargo, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea2), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT Entidad Productora
            AlturaEncabezadoLinea3 = point2.Y + 7;
            lastpagGraphics.DrawString("Firma", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + 4, AlturaEncabezadoLinea3), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + 50, AlturaEncabezadoLinea3 + 7);
            point2 = new PointF(recFin1.Width + 150, AlturaEncabezadoLinea3 + 7);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT OFICINA Productora
            AlturaEncabezadoLinea4 = point2.Y + 7;
            lastpagGraphics.DrawString("Lugar", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + 4, AlturaEncabezadoLinea4), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + 50, AlturaEncabezadoLinea4 + 7);
            point2 = new PointF(recFin1.Width + 150, AlturaEncabezadoLinea4 + 7);
            PdfHTMLTextElement element = new PdfHTMLTextElement();  //Create a text element  //element.Brush = new PdfSolidBrush(Color.Black);
            element.HTMLText = fi_lugar;
            element.Font = fontComun;
            element.TextAlign = TextAlign.Center;
            PdfMetafileLayoutFormat layoutFormat = new PdfMetafileLayoutFormat();
            layoutFormat.Layout = PdfLayoutType.Paginate;
            //PdfLayoutFormat layoutFormat = new PdfLayoutFormat(); //Set the properties to paginate the text
            layoutFormat.Break = PdfLayoutBreakType.FitPage;
            RectangleF bounds = new RectangleF(new PointF(recFin1.Width + 50, AlturaEncabezadoLinea4 - 7), new SizeF(point2.X - point1.X, 15));   //Set bounds to draw multiline text
            element.Draw(lastPage, bounds, layoutFormat);   //Draw the text element with the properties and formats set
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            /// OBJETO
            AlturaEncabezadoLinea5 = point2.Y + 7;
            lastpagGraphics.DrawString("Fecha", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + 4, AlturaEncabezadoLinea5), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + 50, AlturaEncabezadoLinea5 + 7);
            point2 = new PointF(recFin1.Width + 150, AlturaEncabezadoLinea5 + 7);
            lastpagGraphics.DrawString(fi_fecha, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea5), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
                                                              //FIRMA
            if (File.Exists("firmaEntrega_" + idProyecto + ".png")) //Logo
            {
                image = new PdfBitmap("firmaEntrega_" + idProyecto + ".png");
                imageBounds = new RectangleF(recFin1.Width + 155, AlturaEncabezadoLinea3, 50, 30);//Setting image bounds
                lastpagGraphics.DrawImage(image, imageBounds);//Draw the image
            }

            //RECTANGULO
            RectangleF recFin2 = new RectangleF(recFin1.Width + (float)0.5, altoFinal - 5, 210, 75); //Borde
            lastpagGraphics.DrawRectangle(borde, recFin2);

            //CUADRO TRES 3
            lastpagGraphics.DrawString("Recibido por: ", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + recFin2.Width + 4, altoFinal), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + recFin2.Width + 50, altoFinal + 7);
            point2 = new PointF(recFin1.Width + recFin2.Width + 235, altoFinal + 7);
            lastpagGraphics.DrawString(fi_recibidox, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), altoFinal), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT Entidad Productora
            AlturaEncabezadoLinea2 = point2.Y + 7;
            lastpagGraphics.DrawString("Cargo:", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + recFin2.Width + 4, AlturaEncabezadoLinea2), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + recFin2.Width + 50, AlturaEncabezadoLinea2 + 7);
            point2 = new PointF(recFin1.Width + recFin2.Width + 235, AlturaEncabezadoLinea2 + 7);
            lastpagGraphics.DrawString(fi_recibidox_cargo, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea2), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT Entidad Productora
            AlturaEncabezadoLinea3 = point2.Y + 7;
            lastpagGraphics.DrawString("Firma", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + recFin2.Width + 4, AlturaEncabezadoLinea3), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + recFin2.Width + 50, AlturaEncabezadoLinea3 + 7);
            point2 = new PointF(recFin1.Width + recFin2.Width + 235, AlturaEncabezadoLinea3 + 7);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT OFICINA Productora
            AlturaEncabezadoLinea4 = point2.Y + 7;
            lastpagGraphics.DrawString("Lugar", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + recFin2.Width + 4, AlturaEncabezadoLinea4), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + recFin2.Width + 50, AlturaEncabezadoLinea4 + 7);
            point2 = new PointF(recFin1.Width + recFin2.Width + 235, AlturaEncabezadoLinea4 + 7);
            lastpagGraphics.DrawString(fi_lugar, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea4), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            /// OBJETO
            AlturaEncabezadoLinea5 = point2.Y + 7;
            lastpagGraphics.DrawString("Fecha", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + recFin2.Width + 4, AlturaEncabezadoLinea5), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + recFin2.Width + 50, AlturaEncabezadoLinea5 + 7);
            point2 = new PointF(recFin1.Width + recFin2.Width + 235, AlturaEncabezadoLinea5 + 7);
            lastpagGraphics.DrawString(fi_fecha, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea5), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
                                                              //RECTANGULO
            RectangleF recFin3 = new RectangleF(recFin1.Width + recFin2.Width + (float)0.5, altoFinal - 5, (float)241.5, 75); //Borde
            lastpagGraphics.DrawRectangle(borde, recFin3);

            //Save the document.
            doc.Save($"{exportPath}/FUID_" + idProyecto + ".pdf");

            //Close the document.
            doc.Close(true);
        }

        ////public void ExportPfdCaja2(int codigo, string nomLote, string nro_caja, string exportFolderPath, ref List<string> exportedList, p_formato dataFormato)
        public void ExportPdfFuid2(string exportPath, p_formato dataFormato, string nomLote, string nro_caja)
        {
            conteo = 0;
            if (string.IsNullOrEmpty(nomLote) && string.IsNullOrEmpty(nro_caja)) return;
            string fi_titulo1 = "", fi_titulo2 = "", fi_titulo3 = "", fi_cal_codigo = "", fi_cal_version = "", fi_cal_fecha = "", entProductora = "", fi_objeto = "", nomOrg = string.Empty, nomDependencia = string.Empty, codDependencia = string.Empty, nomSubdepen = string.Empty, codSubdepen = string.Empty, codSerie = string.Empty, nomSerie = string.Empty, codSubserie = string.Empty, nomSubserie = string.Empty, fi_elaboradox = "", fi_elaboradox_cargo = "", fi_entregadox = "", fi_entregadox_cargo = "", fi_recibidox = "", fi_recibidox_cargo = "", fi_lugar = "", fi_fecha = "";
            DateTime dateDefecto = DateTime.MinValue;

            var qEncabezado = EntitiesRepository.Entities.t_carpeta.Include("t_lote").Include("t_lote").Include("p_subserie").Include("p_serie").Include("p_subdependencia").Include("p_dependencia").Include("p_organizacion").Where(c => c.nro_caja == nro_caja && c.t_lote.nom_lote == nomLote && c.t_lote.id_proyecto == GlobalClass.id_proyecto);
            //Texto organización
            //nomOrg = qEncabezado.FirstOrDefault().t_lote.p_proyecto.p_organizacion?.nombre;
            //Texto Dependencia
            foreach (var item in qEncabezado.Select(m => m.t_lote.p_subdependencia.p_dependencia).Distinct().ToList())
            {
                if (string.IsNullOrEmpty(codDependencia)) codDependencia = item.codigo;
                else codDependencia += " - " + item.codigo;
                if (string.IsNullOrEmpty(nomDependencia)) nomDependencia = item.nombre;
                else nomDependencia += " - " + item.nombre;
            }
            //Texto SubDependencia
            foreach (var item in qEncabezado.Select(m => m.t_lote.p_subdependencia).Distinct().ToList())
            {
                if (string.IsNullOrEmpty(codSubdepen)) codSubdepen = item.cod;
                else codSubdepen += " - " + item.cod;
                if (string.IsNullOrEmpty(nomSubdepen)) nomSubdepen = item.nombre;
                else nomSubdepen += " - " + item.nombre;
            }

            IQueryable<t_carpeta> datFUID = EntitiesRepository.Entities.t_carpeta.Include("t_lote").Include("p_subserie").Include("p_proyecto").AsNoTracking().Where(p => p.t_lote.id_proyecto == GlobalClass.id_proyecto && p.nro_caja == nro_caja && p.t_lote.nom_lote == nomLote);
            var datFUID2 = datFUID.Select(p => new { p.fecha_expediente_ini, p.fecha_expediente_fin, p.nro_expediente, p.nom_expediente, p.t_lote.p_subserie.nombre });
            var cuentaReg = datFUID2.Count();
            if (cuentaReg == 0)
            {
                //MessageBox.Show("No hay datos para el FUID con esos parámetros.");
                return;
            }

            //var dataFormato = datFUID2.FirstOrDefault().p_formato.FirstOrDefault();
            int idProyecto = 0;
            if (dataFormato != null)
            {
                p_organizacion dataOrg;
                dateDefecto = dataFormato.fecha_inicial_defecto ?? DateTime.MinValue;
                idProyecto = dataFormato.p_proyecto.id;
                //Nombre de la organización
                dataOrg = dataFormato.p_proyecto.p_organizacion;
                nomOrg = dataOrg.nombre;
                if (!string.IsNullOrEmpty(dataFormato.fi_titulo1)) fi_titulo1 = dataFormato.fi_titulo1;
                if (!string.IsNullOrEmpty(dataFormato.fi_titulo2)) fi_titulo2 = dataFormato.fi_titulo2;
                if (!string.IsNullOrEmpty(dataFormato.fi_titulo3)) fi_titulo3 = dataFormato.hc_titulo3;
                if (!string.IsNullOrEmpty(dataFormato.fi_cal_codigo)) fi_cal_codigo = dataFormato.fi_cal_codigo;
                if (!string.IsNullOrEmpty(dataFormato.fi_cal_version)) fi_cal_version = dataFormato.fi_cal_version;
                if (!string.IsNullOrEmpty(dataFormato.fi_cal_fecha)) fi_cal_fecha = dataFormato.fi_cal_fecha;
                if (!string.IsNullOrEmpty(dataFormato.fi_elaboradox)) fi_elaboradox = dataFormato.fi_elaboradox;
                if (!string.IsNullOrEmpty(dataFormato.fi_elaboradox_cargo)) fi_elaboradox_cargo = dataFormato.fi_elaboradox_cargo;
                if (!string.IsNullOrEmpty(dataFormato.fi_entregadox)) fi_entregadox = dataFormato.fi_entregadox;
                if (!string.IsNullOrEmpty(dataFormato.fi_entregadox_cargo)) fi_entregadox_cargo = dataFormato.fi_entregadox_cargo;
                if (!string.IsNullOrEmpty(dataFormato.fi_recibidox)) fi_recibidox = dataFormato.fi_recibidox;
                if (!string.IsNullOrEmpty(dataFormato.fi_recibidox_cargo)) fi_recibidox_cargo = dataFormato.fi_recibidox_cargo;
                if (!string.IsNullOrEmpty(dataFormato.fi_lugar)) fi_lugar = dataFormato.fi_lugar;
                if (!string.IsNullOrEmpty(dataFormato.fi_fecha)) fi_fecha = dataFormato.fi_fecha;
                if (string.IsNullOrEmpty(dataFormato.fi_fecha)) fi_fecha = DateTime.Now.ToString("dd/MM/yyyy H:mm", CultureInfo.InvariantCulture);
                if (!string.IsNullOrEmpty(dataFormato.fi_objeto)) fi_objeto = dataFormato.fi_objeto;

                idProyecto = dataFormato.p_proyecto.id;
            }

            //Create a new PDF document.
            PdfDocument doc = new PdfDocument();

            doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
            doc.PageSettings.Size = PdfPageSize.Letter; //Width = 612 Height = 792
            doc.PageSettings.Margins.Bottom = 22;
            PdfPage page = doc.Pages.Add();//Add a page to the document.

            RectangleF recHeader = new RectangleF(0, 0, doc.Pages[0].GetClientSize().Width, 50);
            RectangleF recFooter = new RectangleF(0, 0, doc.Pages[0].GetClientSize().Width, 10);
            PdfPageTemplateElement header = new PdfPageTemplateElement(recHeader);
            PdfPageTemplateElement footer = new PdfPageTemplateElement(recFooter);
            PdfPen borde = new PdfPen(Color.Black, 1);
            PdfGraphics graphicsPag = page.Graphics;
            PdfGraphics graphicsHeader = header.Graphics;
            PdfGraphics graphicsFooter = footer.Graphics;
            SizeF pageSize = page.GetClientSize();  //Width = 712 Height = 532  Console.WriteLine(pageSize.Width + " - " + pageSize.Height);
            RectangleF imageBounds = new RectangleF(24, 9, 84, 22);//Setting image bounds
            PdfBitmap image;
            if (File.Exists("logo_" + idProyecto + ".png")) //Logo
            {
                image = new PdfBitmap("logo_" + idProyecto + ".png");
                graphicsHeader.DrawImage(image, imageBounds);//Draw the image
            }
            RectangleF recImagen = new RectangleF(0, 0, 132, 42); //Borde
            graphicsHeader.DrawRectangle(borde, recImagen);
            ////////TITULO
            /////Fondo Azul
            PdfLinearGradientBrush brush = new PdfLinearGradientBrush(new PointF(0, 0), new PointF(1, 1), Color.Red, Color.Blue);
            brush.Background = Color.FromArgb(51, 102, 204);

            PdfStringFormat formatoTxtCentrado = new PdfStringFormat(); formatoTxtCentrado.Alignment = PdfTextAlignment.Center;
            PdfStringFormat formatoTxtIzquierda = new PdfStringFormat(); formatoTxtIzquierda.Alignment = PdfTextAlignment.Left;
            PdfFont fontTitulo = new PdfStandardFont(PdfFontFamily.Helvetica, 7, PdfFontStyle.Bold);//Set the standard font.
            RectangleF recTitulo1 = new RectangleF(132, 0, 458, 17); //Borde
            graphicsHeader.DrawRectangle(brush, recTitulo1);
            graphicsHeader.DrawString(fi_titulo1, fontTitulo, PdfBrushes.White, new PointF(376, 6), formatoTxtCentrado);
            graphicsHeader.DrawString(fi_titulo2, fontTitulo, PdfBrushes.Black, new PointF(376, 18), formatoTxtCentrado);
            graphicsHeader.DrawString(fi_titulo3, fontTitulo, PdfBrushes.Black, new PointF(376, 29), formatoTxtCentrado);
            RectangleF recTitulo = new RectangleF(132, 0, 458, recImagen.Height); //Borde
            graphicsHeader.DrawRectangle(borde, recTitulo);
            ////////CALIDAD
            PdfFont fontFormatoNegrita = new PdfStandardFont(PdfFontFamily.Helvetica, 5, PdfFontStyle.Bold);
            graphicsHeader.DrawString(fi_cal_codigo, fontTitulo, PdfBrushes.Black, new PointF(600, 4), formatoTxtIzquierda);
            graphicsHeader.DrawString(fi_cal_version, fontTitulo, PdfBrushes.Black, new PointF(600, 13), formatoTxtIzquierda);
            graphicsHeader.DrawString(fi_cal_fecha, fontTitulo, PdfBrushes.Black, new PointF(600, 22), formatoTxtIzquierda);
            //Conteo de PÁGINAS
            PdfPageNumberField pageNumber = new PdfPageNumberField();
            PdfPageCountField count = new PdfPageCountField();
            PdfCompositeField compositeField = new PdfCompositeField(fontTitulo, PdfBrushes.Black, "Páginas {0} de {1}", pageNumber, count);
            compositeField.StringFormat = formatoTxtIzquierda;
            compositeField.Draw(graphicsHeader, new PointF(600, 31));

            RectangleF recCalidad = new RectangleF(590, 0, 122, recImagen.Height); //Borde
            graphicsHeader.DrawRectangle(borde, recCalidad);
            doc.Template.Top = header;
            //FOOTER
            PdfFont fontPie = new PdfStandardFont(PdfFontFamily.Helvetica, 6);//Set the standard font.
            /*graphicsFooter.DrawString("Desarrollado por Alpha Intelligence AI", fontPie, PdfBrushes.Black, new PointF(50, 0), formatoTxtCentrado);//Draw the text.
            doc.Template.OddBottom = footer; */

            ///////ENCABEZADO
            ///LÍNEA 1
            PdfPen pen = new PdfPen(PdfBrushes.Black, 0.8f);//Initialize pen to draw the line
            PdfFont fontComun = new PdfStandardFont(PdfFontFamily.Helvetica, 6);
            PdfFont fontComunNegrita = new PdfStandardFont(PdfFontFamily.Helvetica, 6, PdfFontStyle.Bold);
            PdfFont fontComunMin = new PdfStandardFont(PdfFontFamily.Helvetica, 4);
            ///TXT lABEL
            var AlturaEncabezadoLinea1 = recImagen.Height + 10;
            RectangleF labelCod = new RectangleF(90, AlturaEncabezadoLinea1, 50, 11); //Borde   graphicsPag.DrawRectangle(borde, labelCod);
            graphicsPag.DrawString("CÓDIGO", fontComunNegrita, PdfBrushes.Black, new PointF(labelCod.X + (labelCod.Width / 2), AlturaEncabezadoLinea1), formatoTxtCentrado);
            RectangleF labelNomb = new RectangleF(140, AlturaEncabezadoLinea1, 290, 11); //Borde    graphicsPag.DrawRectangle(borde, labelNomb);
            graphicsPag.DrawString("NOMBRE", fontComunNegrita, PdfBrushes.Black, new PointF(labelNomb.X + (labelNomb.Width / 2), AlturaEncabezadoLinea1), formatoTxtCentrado);

            ///TXT Entidad Productora
            var AlturaEncabezadoLinea2 = AlturaEncabezadoLinea1 + 11;
            graphicsPag.DrawString("ENTIDAD PRODUCTORA", fontComunNegrita, PdfBrushes.Black, new PointF(0, AlturaEncabezadoLinea2), formatoTxtIzquierda);
            RectangleF recEntidadProdCod = new RectangleF(90, AlturaEncabezadoLinea2 - 2, 50, 11); //Borde
            graphicsPag.DrawRectangle(borde, recEntidadProdCod);
            RectangleF recEntidadProdNomb = new RectangleF(140, AlturaEncabezadoLinea2 - 2, 290, 11); //Borde
            graphicsPag.DrawRectangle(borde, recEntidadProdNomb);
            graphicsPag.DrawString(nomOrg, fontComun, PdfBrushes.Black, new PointF(recEntidadProdNomb.X + (recEntidadProdNomb.Width / 2), AlturaEncabezadoLinea2), formatoTxtCentrado);

            ///TXT UNIDAD ADMINISTRATIVA
            var AlturaEncabezadoLinea3 = recEntidadProdCod.Y + recEntidadProdCod.Height;
            RectangleF undAdminCod = new RectangleF(90, AlturaEncabezadoLinea3, 50, 11); //Borde
            graphicsPag.DrawRectangle(borde, undAdminCod);
            RectangleF undAdminNomb = new RectangleF(140, AlturaEncabezadoLinea3, 290, 11); //Borde
            graphicsPag.DrawRectangle(borde, undAdminNomb);
            graphicsPag.DrawString("UNIDAD ADMINISTRATIVA", fontComunNegrita, PdfBrushes.Black, new PointF(0, AlturaEncabezadoLinea3 + 2), formatoTxtIzquierda);
            graphicsPag.DrawString(nomDependencia, fontComun, PdfBrushes.Black, new PointF(undAdminNomb.X + (undAdminNomb.Width / 2), AlturaEncabezadoLinea3 + 2), formatoTxtCentrado);

            ///TXT OFICINA Productora
            var AlturaEncabezadoLinea4 = undAdminCod.Y + undAdminCod.Height;
            RectangleF ofProdCod = new RectangleF(90, AlturaEncabezadoLinea4, 50, 11); //Borde
            graphicsPag.DrawRectangle(borde, ofProdCod);
            RectangleF ofProdNomb = new RectangleF(140, AlturaEncabezadoLinea4, 290, 11); //Borde
            graphicsPag.DrawRectangle(borde, ofProdNomb);
            graphicsPag.DrawString("OFICINA PRODUCTORA", fontComunNegrita, PdfBrushes.Black, new PointF(0, AlturaEncabezadoLinea4 + 2), formatoTxtIzquierda);
            graphicsPag.DrawString(nomSubdepen, fontComun, PdfBrushes.Black, new PointF(ofProdNomb.X + (ofProdNomb.Width / 2), AlturaEncabezadoLinea4 + 2), formatoTxtCentrado);

            /// OBJETO
            var AlturaEncabezadoLinea5 = ofProdNomb.Y + ofProdNomb.Height;
            RectangleF objCod = new RectangleF(90, ofProdCod.Y + ofProdCod.Height, 50, 11); //Borde
            graphicsPag.DrawRectangle(borde, objCod);
            RectangleF objNomb = new RectangleF(140, ofProdCod.Y + ofProdCod.Height, 290, 11); //Borde
            graphicsPag.DrawRectangle(borde, objNomb);
            graphicsPag.DrawString("OBJETO", fontComunNegrita, PdfBrushes.Black, new PointF(0, AlturaEncabezadoLinea5 + 2), formatoTxtIzquierda);
            graphicsPag.DrawString(fi_objeto, fontComun, PdfBrushes.Black, new PointF(objNomb.X + (objNomb.Width / 2), AlturaEncabezadoLinea5 + 2), formatoTxtCentrado);

            //Cuadro PRIMARIA/SECUNDARIA
            graphicsPag.DrawString("PRIMARIA", fontComunNegrita, PdfBrushes.Black, new PointF(575, 45), formatoTxtCentrado);
            RectangleF recPrimaria = new RectangleF(550, 53, 50, 11); //Borde
            graphicsPag.DrawRectangle(borde, recPrimaria);
            graphicsPag.DrawString("SECUNDARIA", fontComunNegrita, PdfBrushes.Black, new PointF(625, 45), formatoTxtCentrado);
            RectangleF recSecundaria = new RectangleF(600, 53, 50, 11); //Borde
            graphicsPag.DrawRectangle(borde, recSecundaria);
            //CUADRO TRANSFERENCIA
            graphicsPag.DrawString("TRANSFERENCIA", fontComunNegrita, PdfBrushes.Black, new PointF(490, recPrimaria.Y + 2), formatoTxtIzquierda);

            //FIla 1
            RectangleF rectxtRegentra = new RectangleF(440, AlturaEncabezadoLinea1 + 19, 272, 11); //Borde
            page.Graphics.DrawRectangle(borde, rectxtRegentra);
            graphicsPag.DrawString("REGISTRO DE ENTRADA", fontComun, PdfBrushes.Black, new PointF(rectxtRegentra.X + (rectxtRegentra.Width / 2), rectxtRegentra.Y + 2), formatoTxtCentrado);
            //Fila 2 COl1
            RectangleF recF2C1 = new RectangleF(440, rectxtRegentra.Y + rectxtRegentra.Height, 31, 11); //Borde
            page.Graphics.DrawRectangle(borde, recF2C1);
            graphicsPag.DrawString("AÑO", fontComun, PdfBrushes.Black, new PointF(456, recF2C1.Y + 2), formatoTxtCentrado);
            //Fila 2 Col 2
            RectangleF recF2C2 = new RectangleF(recF2C1.X + recF2C1.Width, recF2C1.Y, 65, 11); //Borde
            page.Graphics.DrawRectangle(borde, recF2C2);
            //Divide Columna
            RectangleF recF2F3 = new RectangleF(recF2C1.X + recF2C1.Width, recF2C1.Y, 25, 22); //Borde
            page.Graphics.DrawRectangle(borde, recF2F3);
            //Fila 2 Col 3
            RectangleF recF2C3 = new RectangleF(recF2C2.X + recF2C2.Width, recF2C1.Y, 45, 11); //Borde
            page.Graphics.DrawRectangle(borde, recF2C3);
            graphicsPag.DrawString("MES", fontComun, PdfBrushes.Black, new PointF(recF2C2.X + recF2C2.Width + 22, recF2C3.Y + 2), formatoTxtCentrado);
            //Fila 2 Col 4
            RectangleF recF2C4 = new RectangleF(recF2C3.X + recF2C3.Width, recF2C1.Y, 45, 11); //Borde
            page.Graphics.DrawRectangle(borde, recF2C4);
            graphicsPag.DrawString("DÍA", fontComun, PdfBrushes.Black, new PointF(recF2C3.X + recF2C3.Width + 22, recF2C4.Y + 2), formatoTxtCentrado);
            //Fila 2 Col 5
            RectangleF recF2C5 = new RectangleF(recF2C4.X + recF2C4.Width, recF2C1.Y, 86, 11); //Borde
            page.Graphics.DrawRectangle(borde, recF2C5);
            graphicsPag.DrawString("N.T.", fontComun, PdfBrushes.Black, new PointF(recF2C4.X + recF2C4.Width + 45, recF2C5.Y + 2), formatoTxtCentrado);
            //FIla 3
            //Fila 3 COl1
            RectangleF recF3C1 = new RectangleF(440, recF2C5.Y + recF2C5.Height, 31, 11); //Borde
            page.Graphics.DrawRectangle(borde, recF3C1);
            //Fila 3 Col 2
            RectangleF recF3C2 = new RectangleF(recF2C1.X + recF3C1.Width, recF3C1.Y, 65, 11); //Borde
            page.Graphics.DrawRectangle(borde, recF3C2);
            //Fila 3 Col 3
            RectangleF recF3C3 = new RectangleF(recF3C2.X + recF3C2.Width, recF3C1.Y, 45, 11); //Borde
            page.Graphics.DrawRectangle(borde, recF3C3);
            //Fila 3 Col 4
            RectangleF recF3C4 = new RectangleF(recF3C3.X + recF3C3.Width, recF3C1.Y, 45, 11); //Borde
            page.Graphics.DrawRectangle(borde, recF3C4);
            //Fila 3 Col 5
            RectangleF recF3C5 = new RectangleF(recF3C4.X + recF3C4.Width, recF3C1.Y, 86, 11); //Borde
            page.Graphics.DrawRectangle(borde, recF3C5);
            //FILA 4
            RectangleF recF4C1 = new RectangleF(recF3C1.X, recF3C1.Y + recF3C1.Height, 272, 11); //Borde
            page.Graphics.DrawRectangle(borde, recF4C1);
            graphicsPag.DrawString("N.T. = Número de Transferencia", fontComun, PdfBrushes.Black, new PointF(recF3C4.X + recF3C4.Width, recF4C1.Y + 2), formatoTxtCentrado);
            //  ANCHO DE COLUMNAS
            var anchoHead = new List<int>();
            anchoHead.Add(20);  //0
            anchoHead.Add(30);
            anchoHead.Add(30);
            anchoHead.Add(30);
            anchoHead.Add(170);//4 NOM EXP
            anchoHead.Add(40);
            anchoHead.Add(40);//6
            anchoHead.Add(26);
            anchoHead.Add(37);//8
            anchoHead.Add(20);
            anchoHead.Add(26);//10
            anchoHead.Add(26);
            anchoHead.Add(30);//12 Nro Caja
            anchoHead.Add(35);  // Nro Carpeta
            anchoHead.Add(36);//14 SOPORTE
            anchoHead.Add(50);
            anchoHead.Add(64);//16 OBSERVACIONES
            /*anchoHead.Add(20);  //0
            anchoHead.Add(32);
            anchoHead.Add(25);
            anchoHead.Add(25);
            anchoHead.Add(190);//4 NOM EXP
            anchoHead.Add(40);
            anchoHead.Add(40);//6
            anchoHead.Add(26); //7 UND DE CONSERVACIÓN  - Carpeta
            anchoHead.Add(24);//8                       - Tomo
            anchoHead.Add(20);//                        - Otro
            anchoHead.Add(26);//10  Folio Ini
            anchoHead.Add(26);//    Folio FIn
            anchoHead.Add(36);//12 Nro Caja
            anchoHead.Add(36);  // Nro Carpeta
            anchoHead.Add(36);//14 SOPORTE
            anchoHead.Add(50);
            anchoHead.Add(60);//16 OBSERVACIONES*/

            ////ENCABEZADO TABLA////
            float AlturaEncabezadoTabla = AlturaEncabezadoLinea1 + 67;

            graphicsPag.DrawString("N° \n ORDEN", fontComunMin, PdfBrushes.Black, new PointF(anchoHead[0] / 2, AlturaEncabezadoTabla + 17), formatoTxtCentrado);
            RectangleF recEnc1 = new RectangleF((float)0.5, AlturaEncabezadoTabla, anchoHead[0], 40);
            page.Graphics.DrawRectangle(borde, recEnc1);

            RectangleF recEnc2 = new RectangleF(recEnc1.X + recEnc1.Width, AlturaEncabezadoTabla, (anchoHead[1] + anchoHead[2] + anchoHead[3]), 20);
            page.Graphics.DrawRectangle(borde, recEnc2);
            graphicsPag.DrawString("CÓDIGO", fontComun, PdfBrushes.Black, new PointF(recEnc1.X + recEnc1.Width + ((anchoHead[1] + anchoHead[2] + anchoHead[3]) / 2), recEnc2.Y + 7), formatoTxtCentrado);

            RectangleF recEnc21 = new RectangleF(recEnc1.X + recEnc1.Width, AlturaEncabezadoTabla + 20, anchoHead[1], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc21);
            graphicsPag.DrawString("DEPENDENCIA", fontComunMin, PdfBrushes.Black, new PointF(recEnc1.X + recEnc1.Width + (anchoHead[1] / 2), recEnc2.Y + recEnc2.Height + 7), formatoTxtCentrado);
            RectangleF recEnc22 = new RectangleF(recEnc21.X + recEnc21.Width, AlturaEncabezadoTabla + 20, anchoHead[2], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc22);
            graphicsPag.DrawString("SERIE", fontComunMin, PdfBrushes.Black, new PointF(recEnc21.X + recEnc21.Width + (anchoHead[2] / 2), recEnc2.Y + recEnc2.Height + 7), formatoTxtCentrado);
            RectangleF recEnc23 = new RectangleF(recEnc22.X + recEnc22.Width, AlturaEncabezadoTabla + 20, anchoHead[3], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc23);
            graphicsPag.DrawString("SUBSERIE", fontComunMin, PdfBrushes.Black, new PointF(recEnc22.X + recEnc22.Width + (anchoHead[3] / 2), recEnc2.Y + recEnc2.Height + 7), formatoTxtCentrado);
            graphicsPag.DrawString("NOMBRE DE LAS SERIES, SUBSERIES DOCUMENTALES", fontComun, PdfBrushes.Black, new PointF(recEnc23.X + recEnc23.Width + (anchoHead[4] / 2), AlturaEncabezadoTabla + 17), formatoTxtCentrado);
            RectangleF recEnc3 = new RectangleF(recEnc23.X + recEnc23.Width, AlturaEncabezadoTabla, anchoHead[4], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc3);
            graphicsPag.DrawString("FECHAS EXTREMAS", fontComun, PdfBrushes.Black, new PointF(recEnc3.X + recEnc3.Width + (anchoHead[5]), AlturaEncabezadoTabla + 3), formatoTxtCentrado);
            graphicsPag.DrawString("(dd/mm/aaaa)", fontComun, PdfBrushes.Black, new PointF(recEnc3.X + recEnc3.Width + (anchoHead[5]), AlturaEncabezadoTabla + 11), formatoTxtCentrado);
            RectangleF recEnc4 = new RectangleF(recEnc3.X + recEnc3.Width, AlturaEncabezadoTabla, anchoHead[5] + anchoHead[6], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc4);
            graphicsPag.DrawString("Inicial", fontComun, PdfBrushes.Black, new PointF(recEnc3.X + recEnc3.Width + (anchoHead[5] / 2), AlturaEncabezadoTabla + 27), formatoTxtCentrado);
            RectangleF recEnc41 = new RectangleF(recEnc3.X + recEnc3.Width, AlturaEncabezadoTabla + 20, anchoHead[5], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc41);
            graphicsPag.DrawString("Final", fontComun, PdfBrushes.Black, new PointF(recEnc41.X + recEnc41.Width + (anchoHead[6] / 2), AlturaEncabezadoTabla + 27), formatoTxtCentrado);
            RectangleF recEnc42 = new RectangleF(recEnc41.X + recEnc41.Width, AlturaEncabezadoTabla + 20, anchoHead[6], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc42);
            graphicsPag.DrawString("UNIDAD CONSERVACIÓN", fontComun, PdfBrushes.Black, new PointF(recEnc42.X + recEnc42.Width + (anchoHead[7] + anchoHead[8] + anchoHead[9]) / 2, AlturaEncabezadoTabla + 7), formatoTxtCentrado);
            RectangleF recEnc5 = new RectangleF(recEnc4.X + recEnc4.Width, AlturaEncabezadoTabla, anchoHead[7] + anchoHead[8] + anchoHead[9], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc5);
            /*graphicsPag.DrawString("Caja", fontComun, PdfBrushes.Black, new PointF(recEnc4.X + recEnc4.Width + (anchoHead[7] / 2), AlturaEncabezadoTabla + 27), formatoTxtCentrado);
            RectangleF recEnc51 = new RectangleF(recEnc4.X + recEnc4.Width, AlturaEncabezadoTabla + 20, anchoHead[7], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc51);*/
            graphicsPag.DrawString("Carpeta", fontComun, PdfBrushes.Black, new PointF(recEnc4.X + recEnc4.Width + (anchoHead[7] / 2), AlturaEncabezadoTabla + 27), formatoTxtCentrado);
            RectangleF recEnc52 = new RectangleF(recEnc4.X + recEnc4.Width, AlturaEncabezadoTabla + 20, anchoHead[7], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc52);
            graphicsPag.DrawString("Tomo", fontComun, PdfBrushes.Black, new PointF(recEnc52.X + recEnc52.Width + (anchoHead[8] / 2), AlturaEncabezadoTabla + 27), formatoTxtCentrado);
            RectangleF recEnc53 = new RectangleF(recEnc52.X + recEnc52.Width, AlturaEncabezadoTabla + 20, anchoHead[8], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc53);
            graphicsPag.DrawString("Otros", fontComun, PdfBrushes.Black, new PointF(recEnc53.X + recEnc53.Width + (anchoHead[9] / 2), AlturaEncabezadoTabla + 27), formatoTxtCentrado);
            RectangleF recEnc54 = new RectangleF(recEnc53.X + recEnc53.Width, AlturaEncabezadoTabla + 20, anchoHead[9], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc54);
            //graphicsPag.DrawString("N°", fontComun, PdfBrushes.Black, new PointF(recEnc5.X + recEnc5.Width + (anchoHead[11] / 2), AlturaEncabezadoTabla + 11), formatoTxtCentrado);
            RectangleF recEnc6 = new RectangleF(recEnc5.X + recEnc5.Width, AlturaEncabezadoTabla, (anchoHead[10] + anchoHead[11]), 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc6);
            graphicsPag.DrawString("N° FOLIOS", fontComun, PdfBrushes.Black, new PointF(recEnc5.X + recEnc5.Width + (anchoHead[10] + anchoHead[11]) / 2, recEnc6.Y + 7), formatoTxtCentrado);

            RectangleF recEnc61 = new RectangleF(recEnc5.X + recEnc5.Width, AlturaEncabezadoTabla + 20, anchoHead[10], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc61);
            graphicsPag.DrawString("Desde", fontComun, PdfBrushes.Black, new PointF(recEnc5.X + recEnc5.Width + (anchoHead[10]) / 2, recEnc6.Y + 27), formatoTxtCentrado);
            RectangleF recEnc62 = new RectangleF(recEnc5.X + recEnc5.Width, AlturaEncabezadoTabla + 20, anchoHead[11], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc62);
            graphicsPag.DrawString("Hasta", fontComun, PdfBrushes.Black, new PointF(recEnc5.X + recEnc5.Width + anchoHead[10] + (anchoHead[11] / 2), recEnc6.Y + 27), formatoTxtCentrado);

            graphicsPag.DrawString("N° CAJA", fontComun, PdfBrushes.Black, new PointF(recEnc6.X + recEnc6.Width + (anchoHead[12] / 2), AlturaEncabezadoTabla + 17), formatoTxtCentrado);
            RectangleF recEnc65 = new RectangleF(recEnc6.X + recEnc6.Width, AlturaEncabezadoTabla, anchoHead[12], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc65);

            graphicsPag.DrawString("N° DE", fontComun, PdfBrushes.Black, new PointF(recEnc65.X + recEnc65.Width + (anchoHead[13] / 2), AlturaEncabezadoTabla + 11), formatoTxtCentrado);
            graphicsPag.DrawString("CARPETA", fontComun, PdfBrushes.Black, new PointF(recEnc65.X + recEnc65.Width + (anchoHead[13] / 2), AlturaEncabezadoTabla + 19), formatoTxtCentrado);
            RectangleF recEnc7 = new RectangleF(recEnc65.X + recEnc65.Width, AlturaEncabezadoTabla, anchoHead[13], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc7);
            graphicsPag.DrawString("SOPORTE", fontComun, PdfBrushes.Black, new PointF(recEnc7.X + recEnc7.Width + (anchoHead[14] / 2), AlturaEncabezadoTabla + 17), formatoTxtCentrado);
            RectangleF recEnc8 = new RectangleF(recEnc7.X + recEnc7.Width, AlturaEncabezadoTabla, anchoHead[14], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc8);
            graphicsPag.DrawString("FRECUENCIA", fontComun, PdfBrushes.Black, new PointF(recEnc8.X + recEnc8.Width + (anchoHead[15] / 2), AlturaEncabezadoTabla + 11), formatoTxtCentrado);
            graphicsPag.DrawString("DE CONSULTA", fontComun, PdfBrushes.Black, new PointF(recEnc8.X + recEnc8.Width + (anchoHead[15] / 2), AlturaEncabezadoTabla + 19), formatoTxtCentrado);
            RectangleF recEnc9 = new RectangleF(recEnc8.X + recEnc8.Width, AlturaEncabezadoTabla, anchoHead[15], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc9);
            graphicsPag.DrawString("OBSERVACIONES", fontComunMin, PdfBrushes.Black, new PointF(recEnc9.X + recEnc9.Width + (anchoHead[16] / 2), AlturaEncabezadoTabla + 17), formatoTxtCentrado);
            RectangleF recEnc10 = new RectangleF(recEnc9.X + recEnc9.Width, AlturaEncabezadoTabla, anchoHead[16], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc10);

            DataTable table = new DataTable();// Initialize DataTable to assign as DateSource to the light table.
            table.Columns.Add("Orden");//Include columns to the DataTable.
            table.Columns.Add("CodigoDep");
            table.Columns.Add("CodigoSer");
            table.Columns.Add("CodigoSub");
            table.Columns.Add("Series");
            table.Columns.Add("Inicial");
            table.Columns.Add("Final");
            table.Columns.Add("Carpeta");
            table.Columns.Add("Tomo");
            table.Columns.Add("Otro");
            table.Columns.Add("Folio_ini");
            table.Columns.Add("Folio_fin");
            table.Columns.Add("Caja");
            table.Columns.Add("NumCaja");
            table.Columns.Add("Soporte");
            table.Columns.Add("Frecuencia");
            table.Columns.Add("Observaciones");
            //datFUID2
            IQueryable<t_carpeta> datListCarpeta = EntitiesRepository.Entities.t_carpeta.Include("t_lote").AsNoTracking().Where(p => p.t_lote.id_proyecto == GlobalClass.id_proyecto && p.nro_caja == nro_caja && p.t_lote.nom_lote == nomLote);
            int serial = 1; DateTime fecIni; DateTime fecFin;
            foreach (var item in datListCarpeta.OrderBy(x => x.nro_carpeta).ThenBy(x => x.nro_expediente).ToList())
            {
                string NomTercero = ""; codSubserie = string.Empty;
                if (!string.IsNullOrEmpty(item.t_lote.p_subserie.codigo)) codSubserie = item.t_lote.p_subserie.codigo;
                if (item.t_tercero != null) NomTercero = $@"{item.t_tercero.nombres} {item.t_tercero.apellidos}";
                DateTime.TryParse(item.fecha_expediente_ini.ToString(), out fecIni);
                DateTime.TryParse(item.fecha_expediente_fin.ToString(), out fecFin);
                //Dato
                DateTime fMin = DateTime.MinValue;
                string rxtFecha = "S.F.";
                var fechaMaxSistema = new DateTime(DateTime.MaxValue.Year, DateTime.MaxValue.Month, DateTime.MaxValue.Day, DateTime.MaxValue.Hour, DateTime.MaxValue.Minute, DateTime.MaxValue.Second);
                if (fecIni < fechaMaxSistema) rxtFecha = fecIni.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                else
                {
                    var fMinBD = EntitiesRepository.Entities.t_documento.AsNoTracking().Where(c => c.id_carpeta == item.id && c.folio_ini == 1).Select(p => p.fecha).FirstOrDefault();
                    fMin = fMinBD ?? fechaMaxSistema;
                    if (fMin < fechaMaxSistema) rxtFecha = fMin.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                //Calcula fechas correctas
                /*if (fMin == DateTime.MinValue && fecFin > DateTime.MinValue)
                {
                    fMin = fecFin.AddDays((fecFin.Day * -1) + 1);
                    rxtFecha = fMin.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                if (fMin.Ticks == 3155378975990000000 && fecFin == DateTime.MinValue)
                {
                    var fMaxBD = EntitiesRepository.Entities.t_carpeta.AsNoTracking().Where(c => c.nom_expediente == item.nom_expediente && c.id < item.id).OrderBy(p => p.fecha_expediente_fin).Select(p => p.fecha_expediente_fin).FirstOrDefault();
                    fecFin = fMaxBD ?? fechaMaxSistema;
                } */
                string tomo = string.Empty, observaciones = string.Empty;
                if (item.tomo != null) tomo = item.tomo.ToString();
                if (item.tomo_fin != null) tomo += " DE " + item.tomo_fin.ToString();
                else tomo += " DE " + item.tomo.ToString();

                observaciones = item.kp_observacion?.ToString();

                table.Rows.Add(new string[] { serial.ToString(), codDependencia, codSerie, codSubserie, item.nom_expediente, rxtFecha, fecFin.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), "X", tomo, "", item.kp_folioini.ToString(), item.kp_foliofin.ToString(), item.nro_caja, item.nro_carpeta.ToString(), "FISICO", "MEDIA", observaciones });
                serial++;
            }
            PdfGrid pdfGrid = new PdfGrid();
            pdfGrid.DataSource = table;
            for (int i = 0; i < anchoHead.Count; i++)
            {
                pdfGrid.Columns[i].Width = anchoHead[i];
            }
            PdfStringFormat stringCentrado = new PdfStringFormat();   //Initialize PdfStringFormat and set the properties
            stringCentrado.Alignment = PdfTextAlignment.Center;
            stringCentrado.LineAlignment = PdfVerticalAlignment.Middle;
            //Estilo de fila en Table
            PdfGridCellStyle gridCellStyle = new PdfGridCellStyle();     //Initialize PdfGridCellStyle. Set background color and string format
            gridCellStyle.StringFormat = stringCentrado;
            gridCellStyle.Font = fontComun;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                PdfGridRow gridRow = pdfGrid.Rows[i];
                gridRow.ApplyStyle(gridCellStyle);
                //Console.WriteLine(gridRow.Height);
                if (gridRow.Height > 55) gridRow.Height = 55;
            }
            pdfGrid.BeginCellLayout += PdfGrid_BeginCellLayoutFUID;
            PdfGridLayoutResult pdfGridLayoutResult = pdfGrid.Draw(page, new PointF(0, recEnc1.Y + (float)16.5));    //Draw grid to the page of PDF document

            int pageCount = doc.Pages.Count;
            var lastPage = doc.Pages[pageCount - 1];//Identifica última Hoja
            /*float totalBorde = pdfGrid.;
            if (pageCount == 2) totalBorde += 570;
            if (pageCount == 3) totalBorde += 1300;*/

            var altoFinal = pdfGridLayoutResult.Bounds.Bottom + 8;
            if (altoFinal > 360)
            {
                doc.Pages.Add();
                pageCount++;
                lastPage = doc.Pages[pageCount - 1];
                altoFinal = 10;
            }


            /*var finalTabla = pdfGridLayoutResult.Bounds.Bottom;
            nota = dataFormato.p_formato.FirstOrDefault().hc_nota2;
            textNota = new PdfTextElement(nota, fontComun);
            RectangleF recNota2 = new RectangleF(imageBounds.X + 30, finalTabla + 3, 495, page.GetClientSize().Height);
            textNota.Draw(lastPage, recNota2);*/
            var lastpagGraphics = lastPage.Graphics;
            lastpagGraphics.DrawString("Elaborado por: ", fontComun, PdfBrushes.Black, new PointF(4, altoFinal), formatoTxtIzquierda);
            PointF point1 = new PointF(50, altoFinal + 7);
            PointF point2 = new PointF(255, altoFinal + 7);
            lastpagGraphics.DrawString(fi_elaboradox, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), altoFinal), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT Entidad Productora
            AlturaEncabezadoLinea2 = point2.Y + 7;
            lastpagGraphics.DrawString("Cargo:", fontComun, PdfBrushes.Black, new PointF(4, AlturaEncabezadoLinea2), formatoTxtIzquierda);
            point1 = new PointF(50, AlturaEncabezadoLinea2 + 7);
            point2 = new PointF(255, AlturaEncabezadoLinea2 + 7);
            lastpagGraphics.DrawString(fi_elaboradox_cargo, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea2), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT Entidad Productora
            AlturaEncabezadoLinea3 = point2.Y + 7;
            lastpagGraphics.DrawString("Firma", fontComun, PdfBrushes.Black, new PointF(4, AlturaEncabezadoLinea3), formatoTxtIzquierda);
            point1 = new PointF(50, AlturaEncabezadoLinea3 + 7);
            point2 = new PointF(255, AlturaEncabezadoLinea3 + 7);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT OFICINA Productora
            AlturaEncabezadoLinea4 = point2.Y + 7;
            lastpagGraphics.DrawString("Lugar", fontComun, PdfBrushes.Black, new PointF(4, AlturaEncabezadoLinea4), formatoTxtIzquierda);
            point1 = new PointF(50, AlturaEncabezadoLinea4 + 7);
            point2 = new PointF(255, AlturaEncabezadoLinea4 + 7);
            lastpagGraphics.DrawString(fi_lugar, fontComunMin, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea4), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            /// OBJETO
            AlturaEncabezadoLinea5 = point2.Y + 7;
            lastpagGraphics.DrawString("Fecha", fontComun, PdfBrushes.Black, new PointF(4, AlturaEncabezadoLinea5), formatoTxtIzquierda);
            point1 = new PointF(50, AlturaEncabezadoLinea5 + 7);
            point2 = new PointF(255, AlturaEncabezadoLinea5 + 7);
            lastpagGraphics.DrawString(fi_fecha, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea5), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document

            /*if (File.Exists("firmaElabora_" + idProyecto + ".png")) //Logo
            {
                image = new PdfBitmap("firmaElabora_" + idProyecto + ".png");
                imageBounds = new RectangleF(205, AlturaEncabezadoLinea3, 50, 30);//Setting image bounds
                lastpagGraphics.DrawImage(image, imageBounds);//Draw the image
            }*/
            //RECTANGULO
            RectangleF recFin1 = new RectangleF((float)0.5, altoFinal - 5, 260, 75); //Borde
            lastpagGraphics.DrawRectangle(borde, recFin1);

            lastpagGraphics.DrawString("Entregado por: ", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + 4, altoFinal), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + 50, altoFinal + 7);
            point2 = new PointF(recFin1.Width + 205, altoFinal + 7);
            lastpagGraphics.DrawString(fi_entregadox, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), altoFinal), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT Entidad Productora
            AlturaEncabezadoLinea2 = point2.Y + 7;
            lastpagGraphics.DrawString("Cargo:", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + 4, AlturaEncabezadoLinea2), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + 50, AlturaEncabezadoLinea2 + 7);
            point2 = new PointF(recFin1.Width + 205, AlturaEncabezadoLinea2 + 7);
            lastpagGraphics.DrawString(fi_entregadox_cargo, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea2), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT Entidad Productora
            AlturaEncabezadoLinea3 = point2.Y + 7;
            lastpagGraphics.DrawString("Firma", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + 4, AlturaEncabezadoLinea3), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + 50, AlturaEncabezadoLinea3 + 7);
            point2 = new PointF(recFin1.Width + 205, AlturaEncabezadoLinea3 + 7);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT OFICINA Productora
            AlturaEncabezadoLinea4 = point2.Y + 7;
            lastpagGraphics.DrawString("Lugar", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + 4, AlturaEncabezadoLinea4), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + 50, AlturaEncabezadoLinea4 + 7);
            point2 = new PointF(recFin1.Width + 205, AlturaEncabezadoLinea4 + 7);
            PdfHTMLTextElement element = new PdfHTMLTextElement();  //Create a text element  //element.Brush = new PdfSolidBrush(Color.Black);
            element.HTMLText = fi_lugar;
            element.Font = fontComunMin;
            element.TextAlign = TextAlign.Center;
            PdfMetafileLayoutFormat layoutFormat = new PdfMetafileLayoutFormat();
            layoutFormat.Layout = PdfLayoutType.Paginate;
            //PdfLayoutFormat layoutFormat = new PdfLayoutFormat(); //Set the properties to paginate the text
            layoutFormat.Break = PdfLayoutBreakType.FitPage;
            RectangleF bounds = new RectangleF(new PointF(recFin1.Width + 50, AlturaEncabezadoLinea4 - 1), new SizeF(point2.X - point1.X, 15));   //Set bounds to draw multiline text
            element.Draw(lastPage, bounds, layoutFormat);   //Draw the text element with the properties and formats set
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            /// OBJETO
            AlturaEncabezadoLinea5 = point2.Y + 7;
            lastpagGraphics.DrawString("Fecha", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + 4, AlturaEncabezadoLinea5), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + 50, AlturaEncabezadoLinea5 + 7);
            point2 = new PointF(recFin1.Width + 205, AlturaEncabezadoLinea5 + 7);
            lastpagGraphics.DrawString(fi_fecha, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea5), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
                                                              //FIRMA
            /*if (File.Exists("firmaEntrega_" + idProyecto + ".png")) //Logo
            {
                image = new PdfBitmap("firmaEntrega_" + idProyecto + ".png");
                imageBounds = new RectangleF(recFin1.Width + 155, AlturaEncabezadoLinea3, 50, 30);//Setting image bounds
                lastpagGraphics.DrawImage(image, imageBounds);//Draw the image
            }*/

            //RECTANGULO
            RectangleF recFin2 = new RectangleF(recFin1.Width + (float)0.5, altoFinal - 5, 210, 75); //Borde
            lastpagGraphics.DrawRectangle(borde, recFin2);

            //CUADRO TRES 3
            lastpagGraphics.DrawString("Recibido por: ", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + recFin2.Width + 4, altoFinal), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + recFin2.Width + 50, altoFinal + 7);
            point2 = new PointF(recFin1.Width + recFin2.Width + 235, altoFinal + 7);
            lastpagGraphics.DrawString(fi_recibidox, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), altoFinal), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT Entidad Productora
            AlturaEncabezadoLinea2 = point2.Y + 7;
            lastpagGraphics.DrawString("Cargo:", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + recFin2.Width + 4, AlturaEncabezadoLinea2), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + recFin2.Width + 50, AlturaEncabezadoLinea2 + 7);
            point2 = new PointF(recFin1.Width + recFin2.Width + 235, AlturaEncabezadoLinea2 + 7);
            lastpagGraphics.DrawString(fi_recibidox_cargo, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea2), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT Entidad Productora
            AlturaEncabezadoLinea3 = point2.Y + 7;
            lastpagGraphics.DrawString("Firma", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + recFin2.Width + 4, AlturaEncabezadoLinea3), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + recFin2.Width + 50, AlturaEncabezadoLinea3 + 7);
            point2 = new PointF(recFin1.Width + recFin2.Width + 235, AlturaEncabezadoLinea3 + 7);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT OFICINA Productora
            AlturaEncabezadoLinea4 = point2.Y + 7;
            lastpagGraphics.DrawString("Lugar", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + recFin2.Width + 4, AlturaEncabezadoLinea4), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + recFin2.Width + 50, AlturaEncabezadoLinea4 + 7);
            point2 = new PointF(recFin1.Width + recFin2.Width + 235, AlturaEncabezadoLinea4 + 7);
            lastpagGraphics.DrawString(fi_lugar, fontComunMin, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea4), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            /// OBJETO
            AlturaEncabezadoLinea5 = point2.Y + 7;
            lastpagGraphics.DrawString("Fecha", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + recFin2.Width + 4, AlturaEncabezadoLinea5), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + recFin2.Width + 50, AlturaEncabezadoLinea5 + 7);
            point2 = new PointF(recFin1.Width + recFin2.Width + 235, AlturaEncabezadoLinea5 + 7);
            lastpagGraphics.DrawString(fi_fecha, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea5), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
                                                              //RECTANGULO
            RectangleF recFin3 = new RectangleF(recFin1.Width + recFin2.Width + (float)0.5, altoFinal - 5, (float)241.5, 75); //Borde
            lastpagGraphics.DrawRectangle(borde, recFin3);

            lastpagGraphics.DrawString("Desarrollado por Alpha Intelligence AI", fontPie, PdfBrushes.Black, new PointF(50, recFin3.Y + recFin3.Height + 5), formatoTxtCentrado);//Draw the text.


            //Save the document.
            doc.Save($"{exportPath}/FUID_" + nomLote + "_" + nro_caja + ".pdf");

            //Close the document.
            doc.Close(true);
        }

        public void ExportPdfFuid4(string exportPath, p_formato dataFormato, string nomLote, int int_caja)
        {
            conteo = 0;
            if (string.IsNullOrEmpty(nomLote) && int_caja > 0) return;
            string fi_titulo1 = "", fi_titulo2 = "", fi_titulo3 = "", fi_cal_codigo = "", fi_cal_version = "", fi_cal_fecha = "", entProductora = "", fi_objeto = "", nomOrg = string.Empty, nomDependencia = string.Empty, codDependencia = string.Empty, nomSubdepen = string.Empty, codSubdepen = string.Empty, codSerie = string.Empty, nomSerie = string.Empty, codSubserie = string.Empty, nomSubserie = string.Empty, fi_elaboradox = "", fi_elaboradox_cargo = "", fi_entregadox = "", fi_entregadox_cargo = "", fi_recibidox = "", fi_recibidox_cargo = "", fi_lugar = "", fi_fecha = "";
            DateTime dateDefecto = DateTime.MinValue;

            var qEncabezado = EntitiesRepository.Entities.t_carpeta.Include("t_lote").Include("t_lote").Include("p_subserie").Include("p_serie").Include("p_subdependencia").Include("p_dependencia").Include("p_organizacion").Where(c => c.int_caja == int_caja && c.t_lote.id_proyecto == GlobalClass.id_proyecto);
            //Texto organización
            //nomOrg = qEncabezado.FirstOrDefault().t_lote.p_proyecto.p_organizacion?.nombre;
            //Texto Dependencia
            foreach (var item in qEncabezado.Select(m => m.t_lote.p_subdependencia.p_dependencia).Distinct().ToList())
            {
                if (string.IsNullOrEmpty(codDependencia)) codDependencia = item.codigo;
                else codDependencia += " - " + item.codigo;
                if (string.IsNullOrEmpty(nomDependencia)) nomDependencia = item.nombre;
                else nomDependencia += " - " + item.nombre;
            }
            //Texto SubDependencia
            foreach (var item in qEncabezado.Select(m => m.t_lote.p_subdependencia).Distinct().ToList())
            {
                if (string.IsNullOrEmpty(codSubdepen)) codSubdepen = item.cod;
                else codSubdepen += " - " + item.cod;
                if (string.IsNullOrEmpty(nomSubdepen)) nomSubdepen = item.nombre;
                else nomSubdepen += " - " + item.nombre;
            }

            /*IQueryable<t_carpeta> datFUID = EntitiesRepository.Entities.t_carpeta.Include("t_lote").Include("p_subserie").Include("p_proyecto").AsNoTracking().Where(p => p.t_lote.id_proyecto == GlobalClass.id_proyecto && p.int_caja == int_caja);
            var datFUID2 = datFUID.Select(p => new { p.fecha_expediente_ini, p.fecha_expediente_fin, p.nro_expediente, p.nom_expediente, p.t_lote.p_subserie.nombre });
            var cuentaReg = datFUID2.Count();
            if (cuentaReg == 0)
            {
                //MessageBox.Show("No hay datos para el FUID con esos parámetros.");
                return;
            }*/

            //var dataFormato = datFUID2.FirstOrDefault().p_formato.FirstOrDefault();
            int idProyecto = 0;
            if (dataFormato != null)
            {
                p_organizacion dataOrg;
                dateDefecto = dataFormato.fecha_inicial_defecto ?? DateTime.MinValue;
                idProyecto = dataFormato.p_proyecto.id;
                //Nombre de la organización
                dataOrg = dataFormato.p_proyecto.p_organizacion;
                nomOrg = dataOrg.nombre;
                if (!string.IsNullOrEmpty(dataFormato.fi_titulo1)) fi_titulo1 = dataFormato.fi_titulo1;
                if (!string.IsNullOrEmpty(dataFormato.fi_titulo2)) fi_titulo2 = dataFormato.fi_titulo2;
                if (!string.IsNullOrEmpty(dataFormato.fi_titulo3)) fi_titulo3 = dataFormato.hc_titulo3;
                if (!string.IsNullOrEmpty(dataFormato.fi_cal_codigo)) fi_cal_codigo = dataFormato.fi_cal_codigo;
                if (!string.IsNullOrEmpty(dataFormato.fi_cal_version)) fi_cal_version = dataFormato.fi_cal_version;
                if (!string.IsNullOrEmpty(dataFormato.fi_cal_fecha)) fi_cal_fecha = dataFormato.fi_cal_fecha;
                if (!string.IsNullOrEmpty(dataFormato.fi_elaboradox)) fi_elaboradox = dataFormato.fi_elaboradox;
                if (!string.IsNullOrEmpty(dataFormato.fi_elaboradox_cargo)) fi_elaboradox_cargo = dataFormato.fi_elaboradox_cargo;
                if (!string.IsNullOrEmpty(dataFormato.fi_entregadox)) fi_entregadox = dataFormato.fi_entregadox;
                if (!string.IsNullOrEmpty(dataFormato.fi_entregadox_cargo)) fi_entregadox_cargo = dataFormato.fi_entregadox_cargo;
                if (!string.IsNullOrEmpty(dataFormato.fi_recibidox)) fi_recibidox = dataFormato.fi_recibidox;
                if (!string.IsNullOrEmpty(dataFormato.fi_recibidox_cargo)) fi_recibidox_cargo = dataFormato.fi_recibidox_cargo;
                if (!string.IsNullOrEmpty(dataFormato.fi_lugar)) fi_lugar = dataFormato.fi_lugar;
                if (!string.IsNullOrEmpty(dataFormato.fi_fecha)) fi_fecha = dataFormato.fi_fecha;
                if (string.IsNullOrEmpty(dataFormato.fi_fecha)) fi_fecha = DateTime.Now.ToString("dd/MM/yyyy H:mm", CultureInfo.InvariantCulture);
                if (!string.IsNullOrEmpty(dataFormato.fi_objeto)) fi_objeto = dataFormato.fi_objeto;

                idProyecto = dataFormato.p_proyecto.id;
            }

            //Create a new PDF document.
            PdfDocument doc = new PdfDocument();

            doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
            doc.PageSettings.Size = PdfPageSize.Letter; //Width = 612 Height = 792
            //doc.PageSettings.Margins.Bottom = 25;
            PdfPage page = doc.Pages.Add();//Add a page to the document.
            RectangleF recHeader = new RectangleF(0, 0, doc.Pages[0].GetClientSize().Width, 50);
            RectangleF recFooter = new RectangleF(0, 0, doc.Pages[0].GetClientSize().Width, 10);
            PdfPageTemplateElement header = new PdfPageTemplateElement(recHeader);
            PdfPageTemplateElement footer = new PdfPageTemplateElement(recFooter);
            PdfPen borde = new PdfPen(Color.Black, 1);
            PdfGraphics graphicsPag = page.Graphics;
            PdfGraphics graphicsHeader = header.Graphics;
            //PdfGraphics graphicsFooter = footer.Graphics;
            SizeF pageSize = page.GetClientSize();  //Width = 712 Height = 532  Console.WriteLine(pageSize.Width + " - " + pageSize.Height);
            RectangleF imageBounds = new RectangleF(24, 9, 84, 22);//Setting image bounds
            PdfBitmap image;
            if (File.Exists("logo_" + idProyecto + ".png")) //Logo
            {
                image = new PdfBitmap("logo_" + idProyecto + ".png");
                graphicsHeader.DrawImage(image, imageBounds);//Draw the image
            }
            RectangleF recImagen = new RectangleF(0, 0, 132, 42); //Borde
            graphicsHeader.DrawRectangle(borde, recImagen);
            ////////TITULO
            /////Fondo Azul
            PdfLinearGradientBrush brush = new PdfLinearGradientBrush(new PointF(0, 0), new PointF(1, 1), Color.Red, Color.Blue);
            brush.Background = Color.FromArgb(51, 102, 204);

            PdfStringFormat formatoTxtCentrado = new PdfStringFormat(); formatoTxtCentrado.Alignment = PdfTextAlignment.Center;
            PdfStringFormat formatoTxtIzquierda = new PdfStringFormat(); formatoTxtIzquierda.Alignment = PdfTextAlignment.Left;
            PdfFont fontTitulo = new PdfStandardFont(PdfFontFamily.Helvetica, 7, PdfFontStyle.Bold);//Set the standard font.
            RectangleF recTitulo1 = new RectangleF(132, 0, 458, 17); //Borde
            graphicsHeader.DrawRectangle(brush, recTitulo1);
            graphicsHeader.DrawString(fi_titulo1, fontTitulo, PdfBrushes.White, new PointF(376, 6), formatoTxtCentrado);
            graphicsHeader.DrawString(fi_titulo2, fontTitulo, PdfBrushes.Black, new PointF(376, 18), formatoTxtCentrado);
            graphicsHeader.DrawString(fi_titulo3, fontTitulo, PdfBrushes.Black, new PointF(376, 29), formatoTxtCentrado);
            RectangleF recTitulo = new RectangleF(132, 0, 458, recImagen.Height); //Borde
            graphicsHeader.DrawRectangle(borde, recTitulo);
            ////////CALIDAD
            PdfFont fontFormatoNegrita = new PdfStandardFont(PdfFontFamily.Helvetica, 5, PdfFontStyle.Bold);
            graphicsHeader.DrawString(fi_cal_codigo, fontTitulo, PdfBrushes.Black, new PointF(600, 4), formatoTxtIzquierda);
            graphicsHeader.DrawString(fi_cal_version, fontTitulo, PdfBrushes.Black, new PointF(600, 13), formatoTxtIzquierda);
            graphicsHeader.DrawString(fi_cal_fecha, fontTitulo, PdfBrushes.Black, new PointF(600, 22), formatoTxtIzquierda);
            //Conteo de PÁGINAS
            PdfPageNumberField pageNumber = new PdfPageNumberField();
            PdfPageCountField count = new PdfPageCountField();
            PdfCompositeField compositeField = new PdfCompositeField(fontTitulo, PdfBrushes.Black, "Páginas {0} de {1}", pageNumber, count);
            compositeField.StringFormat = formatoTxtIzquierda;
            compositeField.Draw(graphicsHeader, new PointF(600, 31));

            RectangleF recCalidad = new RectangleF(590, 0, 122, recImagen.Height); //Borde
            graphicsHeader.DrawRectangle(borde, recCalidad);
            doc.Template.Top = header;
            //FOOTER
            PdfFont fontPie = new PdfStandardFont(PdfFontFamily.Helvetica, 6);//Set the standard font.
            /*graphicsFooter.DrawString("Desarrollado por Alpha Intelligence AI", fontPie, PdfBrushes.Black, new PointF(50, 0), formatoTxtCentrado);//Draw the text.
            doc.Template.OddBottom = footer; */

            ///////ENCABEZADO
            ///LÍNEA 1
            PdfPen pen = new PdfPen(PdfBrushes.Black, 0.8f);//Initialize pen to draw the line
            PdfFont fontComun = new PdfStandardFont(PdfFontFamily.Helvetica, 6);
            PdfFont fontComunNegrita = new PdfStandardFont(PdfFontFamily.Helvetica, 6, PdfFontStyle.Bold);
            PdfFont fontComunMin5 = new PdfStandardFont(PdfFontFamily.Helvetica, 5);
            PdfFont fontComunMin = new PdfStandardFont(PdfFontFamily.Helvetica, 4);
            ///TXT lABEL
            var AlturaEncabezadoLinea1 = recImagen.Height + 10;
            RectangleF labelCod = new RectangleF(90, AlturaEncabezadoLinea1, 50, 11); //Borde   graphicsPag.DrawRectangle(borde, labelCod);
            graphicsPag.DrawString("CÓDIGO", fontComunNegrita, PdfBrushes.Black, new PointF(labelCod.X + (labelCod.Width / 2), AlturaEncabezadoLinea1), formatoTxtCentrado);
            RectangleF labelNomb = new RectangleF(140, AlturaEncabezadoLinea1, 290, 11); //Borde    graphicsPag.DrawRectangle(borde, labelNomb);
            graphicsPag.DrawString("NOMBRE", fontComunNegrita, PdfBrushes.Black, new PointF(labelNomb.X + (labelNomb.Width / 2), AlturaEncabezadoLinea1), formatoTxtCentrado);

            ///TXT Entidad Productora
            var AlturaEncabezadoLinea2 = AlturaEncabezadoLinea1 + 11;
            graphicsPag.DrawString("ENTIDAD PRODUCTORA", fontComunNegrita, PdfBrushes.Black, new PointF(0, AlturaEncabezadoLinea2), formatoTxtIzquierda);
            RectangleF recEntidadProdCod = new RectangleF(90, AlturaEncabezadoLinea2 - 2, 50, 11); //Borde
            graphicsPag.DrawRectangle(borde, recEntidadProdCod);
            graphicsPag.DrawString("UARIV", fontComun, PdfBrushes.Black, new PointF(recEntidadProdCod.X + (recEntidadProdCod.Width / 2), AlturaEncabezadoLinea2), formatoTxtCentrado);
            RectangleF recEntidadProdNomb = new RectangleF(140, AlturaEncabezadoLinea2 - 2, 290, 11); //Borde
            graphicsPag.DrawRectangle(borde, recEntidadProdNomb);
            graphicsPag.DrawString(nomOrg, fontComun, PdfBrushes.Black, new PointF(recEntidadProdNomb.X + (recEntidadProdNomb.Width / 2), AlturaEncabezadoLinea2), formatoTxtCentrado);

            ///TXT UNIDAD ADMINISTRATIVA
            var AlturaEncabezadoLinea3 = recEntidadProdCod.Y + recEntidadProdCod.Height;
            RectangleF undAdminCod = new RectangleF(90, AlturaEncabezadoLinea3, 50, 11); //Borde
            graphicsPag.DrawRectangle(borde, undAdminCod);
            graphicsPag.DrawString(codDependencia, fontComun, PdfBrushes.Black, new PointF(undAdminCod.X + (undAdminCod.Width / 2), AlturaEncabezadoLinea3 + 2), formatoTxtCentrado);
            RectangleF undAdminNomb = new RectangleF(140, AlturaEncabezadoLinea3, 290, 11); //Borde
            graphicsPag.DrawRectangle(borde, undAdminNomb);
            graphicsPag.DrawString("UNIDAD ADMINISTRATIVA", fontComunNegrita, PdfBrushes.Black, new PointF(0, AlturaEncabezadoLinea3 + 2), formatoTxtIzquierda);
            graphicsPag.DrawString(nomDependencia, fontComun, PdfBrushes.Black, new PointF(undAdminNomb.X + (undAdminNomb.Width / 2), AlturaEncabezadoLinea3 + 2), formatoTxtCentrado);

            ///TXT OFICINA Productora
            var AlturaEncabezadoLinea4 = undAdminCod.Y + undAdminCod.Height;
            RectangleF ofProdCod = new RectangleF(90, AlturaEncabezadoLinea4, 50, 11); //Borde
            graphicsPag.DrawRectangle(borde, ofProdCod);
            graphicsPag.DrawString(codSubdepen, fontComun, PdfBrushes.Black, new PointF(ofProdCod.X + (ofProdCod.Width / 2), AlturaEncabezadoLinea4 + 2), formatoTxtCentrado);
            RectangleF ofProdNomb = new RectangleF(140, AlturaEncabezadoLinea4, 290, 11); //Borde
            graphicsPag.DrawRectangle(borde, ofProdNomb);
            graphicsPag.DrawString("OFICINA PRODUCTORA", fontComunNegrita, PdfBrushes.Black, new PointF(0, AlturaEncabezadoLinea4 + 2), formatoTxtIzquierda);
            graphicsPag.DrawString(nomSubdepen, fontComun, PdfBrushes.Black, new PointF(ofProdNomb.X + (ofProdNomb.Width / 2), AlturaEncabezadoLinea4 + 2), formatoTxtCentrado);

            /// OBJETO
            var AlturaEncabezadoLinea5 = ofProdNomb.Y + ofProdNomb.Height;
            RectangleF objCod = new RectangleF(90, ofProdCod.Y + ofProdCod.Height, 50, 11); //Borde
            graphicsPag.DrawRectangle(borde, objCod);
            RectangleF objNomb = new RectangleF(140, ofProdCod.Y + ofProdCod.Height, 290, 11); //Borde
            graphicsPag.DrawRectangle(borde, objNomb);
            graphicsPag.DrawString("OBJETO", fontComunNegrita, PdfBrushes.Black, new PointF(0, AlturaEncabezadoLinea5 + 2), formatoTxtIzquierda);
            graphicsPag.DrawString(fi_objeto, fontComun, PdfBrushes.Black, new PointF(objNomb.X + (objNomb.Width / 2), AlturaEncabezadoLinea5 + 2), formatoTxtCentrado);

            //Cuadro PRIMARIA/SECUNDARIA
            graphicsPag.DrawString("PRIMARIA", fontComunNegrita, PdfBrushes.Black, new PointF(575, 45), formatoTxtCentrado);
            RectangleF recPrimaria = new RectangleF(550, 53, 50, 11); //Borde
            graphicsPag.DrawRectangle(borde, recPrimaria);
            graphicsPag.DrawString("SECUNDARIA", fontComunNegrita, PdfBrushes.Black, new PointF(625, 45), formatoTxtCentrado);
            RectangleF recSecundaria = new RectangleF(600, 53, 50, 11); //Borde
            graphicsPag.DrawRectangle(borde, recSecundaria);
            //CUADRO TRANSFERENCIA
            graphicsPag.DrawString("TRANSFERENCIA", fontComunNegrita, PdfBrushes.Black, new PointF(490, recPrimaria.Y + 2), formatoTxtIzquierda);

            //FIla 1
            RectangleF rectxtRegentra = new RectangleF(440, AlturaEncabezadoLinea1 + 19, 271, 11); //Borde
            page.Graphics.DrawRectangle(borde, rectxtRegentra);
            graphicsPag.DrawString("REGISTRO DE ENTRADA", fontComun, PdfBrushes.Black, new PointF(rectxtRegentra.X + (rectxtRegentra.Width / 2), rectxtRegentra.Y + 2), formatoTxtCentrado);
            //Fila 2 COl1
            RectangleF recF2C1 = new RectangleF(440, rectxtRegentra.Y + rectxtRegentra.Height, 98, 11); //Borde
            page.Graphics.DrawRectangle(borde, recF2C1);
            graphicsPag.DrawString("AÑO", fontComun, PdfBrushes.Black, new PointF(recF2C1.X + (recF2C1.Width / 2), recF2C1.Y + 2), formatoTxtCentrado);
            //Fila 2 Col 3
            RectangleF recF2C3 = new RectangleF(recF2C1.X + recF2C1.Width, recF2C1.Y, 45, 11); //Borde
            page.Graphics.DrawRectangle(borde, recF2C3);
            graphicsPag.DrawString("MES", fontComun, PdfBrushes.Black, new PointF(recF2C1.X + recF2C1.Width + 22, recF2C3.Y + 2), formatoTxtCentrado);
            //Fila 2 Col 4
            RectangleF recF2C4 = new RectangleF(recF2C3.X + recF2C3.Width, recF2C1.Y, 45, 11); //Borde
            page.Graphics.DrawRectangle(borde, recF2C4);
            graphicsPag.DrawString("DÍA", fontComun, PdfBrushes.Black, new PointF(recF2C3.X + recF2C3.Width + 22, recF2C4.Y + 2), formatoTxtCentrado);
            //Fila 2 Col 5
            RectangleF recF2C5 = new RectangleF(recF2C4.X + recF2C4.Width, recF2C1.Y, 83, 11); //Borde
            page.Graphics.DrawRectangle(borde, recF2C5);
            graphicsPag.DrawString("N.T.", fontComun, PdfBrushes.Black, new PointF(recF2C4.X + recF2C4.Width + 45, recF2C5.Y + 2), formatoTxtCentrado);
            //FIla 3
            //Fila 3 COl1
            RectangleF recF3C1 = new RectangleF(440, recF2C5.Y + recF2C5.Height, recF2C1.Width, 11); //Borde
            page.Graphics.DrawRectangle(borde, recF3C1);
            //Fila 3 Col 2
            /*RectangleF recF3C2 = new RectangleF(recF2C1.X + recF3C1.Width, recF3C1.Y, 65, 11); //Borde
            page.Graphics.DrawRectangle(borde, recF3C2);    */
            //Fila 3 Col 3
            RectangleF recF3C3 = new RectangleF(recF3C1.X + recF3C1.Width, recF3C1.Y, 45, 11); //Borde
            page.Graphics.DrawRectangle(borde, recF3C3);

            //Fila 3 Col 4
            RectangleF recF3C4 = new RectangleF(recF3C3.X + recF3C3.Width, recF3C1.Y, 45, 11); //Borde
            page.Graphics.DrawRectangle(borde, recF3C4);

            //Fila 3 Col 5
            RectangleF recF3C5 = new RectangleF(recF3C4.X + recF3C4.Width, recF3C1.Y, 83, 11); //Borde
            page.Graphics.DrawRectangle(borde, recF3C5);
            //FILA 4
            RectangleF recF4C1 = new RectangleF(recF3C1.X, recF3C1.Y + recF3C1.Height, 271, 11); //Borde
            page.Graphics.DrawRectangle(borde, recF4C1);
            graphicsPag.DrawString("N.T. = Número de Transferencia", fontComun, PdfBrushes.Black, new PointF(recF3C4.X + recF3C4.Width, recF4C1.Y + 2), formatoTxtCentrado);
            //  ANCHO DE COLUMNAS
            var anchoHead = new List<int>();
            anchoHead.Add(20);  //0
            anchoHead.Add(32);
            anchoHead.Add(25);
            anchoHead.Add(25);
            anchoHead.Add(190);//4 NOM EXP
            anchoHead.Add(40);
            anchoHead.Add(40);//6
            anchoHead.Add(26); //7 UND DE CONSERVACIÓN  - Carpeta
            anchoHead.Add(24);//8                       - Tomo
            anchoHead.Add(20);//                        - Otro
            anchoHead.Add(26);//10  Folio Ini
            anchoHead.Add(26);//    Folio FIn
            anchoHead.Add(36);//12 Nro Caja
            anchoHead.Add(36);  // Nro Carpeta
            anchoHead.Add(36);//14 SOPORTE
            anchoHead.Add(50);
            anchoHead.Add(60);//16 OBSERVACIONES

            ////ENCABEZADO TABLA////
            float AlturaEncabezadoTabla = AlturaEncabezadoLinea1 + 67;

            graphicsPag.DrawString("N° \n ORDEN", fontComunMin, PdfBrushes.Black, new PointF(anchoHead[0] / 2, AlturaEncabezadoTabla + 17), formatoTxtCentrado);
            RectangleF recEnc1 = new RectangleF((float)0.5, AlturaEncabezadoTabla, anchoHead[0], 40);
            page.Graphics.DrawRectangle(borde, recEnc1);

            RectangleF recEnc2 = new RectangleF(recEnc1.X + recEnc1.Width, AlturaEncabezadoTabla, (anchoHead[1] + anchoHead[2] + anchoHead[3]), 20);
            page.Graphics.DrawRectangle(borde, recEnc2);
            graphicsPag.DrawString("CÓDIGO", fontComun, PdfBrushes.Black, new PointF(recEnc1.X + recEnc1.Width + ((anchoHead[1] + anchoHead[2] + anchoHead[3]) / 2), recEnc2.Y + 7), formatoTxtCentrado);

            RectangleF recEnc21 = new RectangleF(recEnc1.X + recEnc1.Width, AlturaEncabezadoTabla + 20, anchoHead[1], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc21);
            graphicsPag.DrawString("DEPENDENCIA", fontComunMin, PdfBrushes.Black, new PointF(recEnc1.X + recEnc1.Width + (anchoHead[1] / 2), recEnc2.Y + recEnc2.Height + 7), formatoTxtCentrado);
            RectangleF recEnc22 = new RectangleF(recEnc21.X + recEnc21.Width, AlturaEncabezadoTabla + 20, anchoHead[2], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc22);
            graphicsPag.DrawString("SERIE", fontComunMin, PdfBrushes.Black, new PointF(recEnc21.X + recEnc21.Width + (anchoHead[2] / 2), recEnc2.Y + recEnc2.Height + 7), formatoTxtCentrado);
            RectangleF recEnc23 = new RectangleF(recEnc22.X + recEnc22.Width, AlturaEncabezadoTabla + 20, anchoHead[3], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc23);
            graphicsPag.DrawString("SUBSERIE", fontComunMin, PdfBrushes.Black, new PointF(recEnc22.X + recEnc22.Width + (anchoHead[3] / 2), recEnc2.Y + recEnc2.Height + 7), formatoTxtCentrado);
            graphicsPag.DrawString("NOMBRE DE LAS SERIES, SUBSERIES DOCUMENTALES", fontComun, PdfBrushes.Black, new PointF(recEnc23.X + recEnc23.Width + (anchoHead[4] / 2), AlturaEncabezadoTabla + 17), formatoTxtCentrado);
            RectangleF recEnc3 = new RectangleF(recEnc23.X + recEnc23.Width, AlturaEncabezadoTabla, anchoHead[4], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc3);
            graphicsPag.DrawString("FECHAS EXTREMAS", fontComun, PdfBrushes.Black, new PointF(recEnc3.X + recEnc3.Width + (anchoHead[5]), AlturaEncabezadoTabla + 3), formatoTxtCentrado);
            graphicsPag.DrawString("(dd/mm/aaaa)", fontComun, PdfBrushes.Black, new PointF(recEnc3.X + recEnc3.Width + (anchoHead[5]), AlturaEncabezadoTabla + 11), formatoTxtCentrado);
            RectangleF recEnc4 = new RectangleF(recEnc3.X + recEnc3.Width, AlturaEncabezadoTabla, anchoHead[5] + anchoHead[6], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc4);
            graphicsPag.DrawString("Inicial", fontComun, PdfBrushes.Black, new PointF(recEnc3.X + recEnc3.Width + (anchoHead[5] / 2), AlturaEncabezadoTabla + 27), formatoTxtCentrado);
            RectangleF recEnc41 = new RectangleF(recEnc3.X + recEnc3.Width, AlturaEncabezadoTabla + 20, anchoHead[5], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc41);
            graphicsPag.DrawString("Final", fontComun, PdfBrushes.Black, new PointF(recEnc41.X + recEnc41.Width + (anchoHead[6] / 2), AlturaEncabezadoTabla + 27), formatoTxtCentrado);
            RectangleF recEnc42 = new RectangleF(recEnc41.X + recEnc41.Width, AlturaEncabezadoTabla + 20, anchoHead[6], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc42);
            graphicsPag.DrawString("UNIDAD DE \n CONSERVACIÓN", fontComun, PdfBrushes.Black, new PointF(recEnc42.X + recEnc42.Width + (anchoHead[7] + anchoHead[8] + anchoHead[9]) / 2, AlturaEncabezadoTabla + 3), formatoTxtCentrado);
            RectangleF recEnc5 = new RectangleF(recEnc4.X + recEnc4.Width, AlturaEncabezadoTabla, anchoHead[7] + anchoHead[8] + anchoHead[9], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc5);
            /*graphicsPag.DrawString("Caja", fontComun, PdfBrushes.Black, new PointF(recEnc4.X + recEnc4.Width + (anchoHead[7] / 2), AlturaEncabezadoTabla + 27), formatoTxtCentrado);
            RectangleF recEnc51 = new RectangleF(recEnc4.X + recEnc4.Width, AlturaEncabezadoTabla + 20, anchoHead[7], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc51);*/
            graphicsPag.DrawString("Carpeta", fontComun, PdfBrushes.Black, new PointF(recEnc4.X + recEnc4.Width + (anchoHead[7] / 2), AlturaEncabezadoTabla + 27), formatoTxtCentrado);
            RectangleF recEnc52 = new RectangleF(recEnc4.X + recEnc4.Width, AlturaEncabezadoTabla + 20, anchoHead[7], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc52);
            graphicsPag.DrawString("Tomo", fontComun, PdfBrushes.Black, new PointF(recEnc52.X + recEnc52.Width + (anchoHead[8] / 2), AlturaEncabezadoTabla + 27), formatoTxtCentrado);
            RectangleF recEnc53 = new RectangleF(recEnc52.X + recEnc52.Width, AlturaEncabezadoTabla + 20, anchoHead[8], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc53);
            graphicsPag.DrawString("Otros", fontComun, PdfBrushes.Black, new PointF(recEnc53.X + recEnc53.Width + (anchoHead[9] / 2), AlturaEncabezadoTabla + 27), formatoTxtCentrado);
            RectangleF recEnc54 = new RectangleF(recEnc53.X + recEnc53.Width, AlturaEncabezadoTabla + 20, anchoHead[9], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc54);
            //graphicsPag.DrawString("N°", fontComun, PdfBrushes.Black, new PointF(recEnc5.X + recEnc5.Width + (anchoHead[11] / 2), AlturaEncabezadoTabla + 11), formatoTxtCentrado);
            RectangleF recEnc6 = new RectangleF(recEnc5.X + recEnc5.Width, AlturaEncabezadoTabla, (anchoHead[10] + anchoHead[11]), 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc6);
            graphicsPag.DrawString("N° FOLIOS", fontComun, PdfBrushes.Black, new PointF(recEnc5.X + recEnc5.Width + (anchoHead[10] + anchoHead[11]) / 2, recEnc6.Y + 7), formatoTxtCentrado);

            RectangleF recEnc61 = new RectangleF(recEnc5.X + recEnc5.Width, AlturaEncabezadoTabla + 20, anchoHead[10], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc61);
            graphicsPag.DrawString("Desde", fontComun, PdfBrushes.Black, new PointF(recEnc5.X + recEnc5.Width + (anchoHead[10]) / 2, recEnc6.Y + 27), formatoTxtCentrado);
            RectangleF recEnc62 = new RectangleF(recEnc5.X + recEnc5.Width, AlturaEncabezadoTabla + 20, anchoHead[11], 20); //Borde
            page.Graphics.DrawRectangle(borde, recEnc62);
            graphicsPag.DrawString("Hasta", fontComun, PdfBrushes.Black, new PointF(recEnc5.X + recEnc5.Width + anchoHead[10] + (anchoHead[11] / 2), recEnc6.Y + 27), formatoTxtCentrado);

            graphicsPag.DrawString("N° CAJA", fontComun, PdfBrushes.Black, new PointF(recEnc6.X + recEnc6.Width + (anchoHead[12] / 2), AlturaEncabezadoTabla + 17), formatoTxtCentrado);
            RectangleF recEnc65 = new RectangleF(recEnc6.X + recEnc6.Width, AlturaEncabezadoTabla, anchoHead[12], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc65);

            graphicsPag.DrawString("N° DE", fontComun, PdfBrushes.Black, new PointF(recEnc65.X + recEnc65.Width + (anchoHead[13] / 2), AlturaEncabezadoTabla + 11), formatoTxtCentrado);
            graphicsPag.DrawString("CARPETA", fontComun, PdfBrushes.Black, new PointF(recEnc65.X + recEnc65.Width + (anchoHead[13] / 2), AlturaEncabezadoTabla + 19), formatoTxtCentrado);
            RectangleF recEnc7 = new RectangleF(recEnc65.X + recEnc65.Width, AlturaEncabezadoTabla, anchoHead[13], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc7);
            graphicsPag.DrawString("SOPORTE", fontComun, PdfBrushes.Black, new PointF(recEnc7.X + recEnc7.Width + (anchoHead[14] / 2), AlturaEncabezadoTabla + 17), formatoTxtCentrado);
            RectangleF recEnc8 = new RectangleF(recEnc7.X + recEnc7.Width, AlturaEncabezadoTabla, anchoHead[14], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc8);
            graphicsPag.DrawString("FRECUENCIA", fontComun, PdfBrushes.Black, new PointF(recEnc8.X + recEnc8.Width + (anchoHead[15] / 2), AlturaEncabezadoTabla + 11), formatoTxtCentrado);
            graphicsPag.DrawString("DE CONSULTA", fontComun, PdfBrushes.Black, new PointF(recEnc8.X + recEnc8.Width + (anchoHead[15] / 2), AlturaEncabezadoTabla + 19), formatoTxtCentrado);
            RectangleF recEnc9 = new RectangleF(recEnc8.X + recEnc8.Width, AlturaEncabezadoTabla, anchoHead[15], 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc9);
            graphicsPag.DrawString("OBSERVACIONES", fontComunMin, PdfBrushes.Black, new PointF(recEnc9.X + recEnc9.Width + (anchoHead[16] / 2), AlturaEncabezadoTabla + 17), formatoTxtCentrado);
            RectangleF recEnc10 = new RectangleF(recEnc9.X + recEnc9.Width, AlturaEncabezadoTabla, anchoHead[16] - 1, 40); //Borde
            page.Graphics.DrawRectangle(borde, recEnc10);

            DataTable table = new DataTable();// Initialize DataTable to assign as DateSource to the light table.
            table.Columns.Add("Orden");//Include columns to the DataTable.
            table.Columns.Add("CodigoDep");
            table.Columns.Add("CodigoSer");
            table.Columns.Add("CodigoSub");
            table.Columns.Add("Series");
            table.Columns.Add("Inicial");
            table.Columns.Add("Final");
            table.Columns.Add("Carpeta");
            table.Columns.Add("Tomo");
            table.Columns.Add("Otro");
            table.Columns.Add("Folio_ini");
            table.Columns.Add("Folio_fin");
            table.Columns.Add("Caja");
            table.Columns.Add("NumCaja");
            table.Columns.Add("Soporte");
            table.Columns.Add("Frecuencia");
            table.Columns.Add("Observaciones");
            //datFUID2  DateTime fechaPrincipal = DateTime.MinValue;
            IQueryable<t_documento> datListDocumento = EntitiesRepository.Entities.t_documento.IncludeOptimized(x => x.t_carpeta).IncludeOptimized(x => x.t_carpeta.t_lote).Where(p => p.t_carpeta.t_lote.id_proyecto == GlobalClass.id_proyecto && p.t_carpeta.int_caja == int_caja);
            int serial = 1, idTercero = -1, id_doc_actual = -1, id_carpeta_actual = -1, folioIni = -1, folioFin = -1, folioIniActual = -1, folioFinActual = -1; DateTime? fecIni = DateTime.MinValue; DateTime? fecFin = DateTime.MinValue; DateTime? fecApertura = DateTime.MaxValue;
            string NomTercero = string.Empty, nomFUD = string.Empty, nomactualFUD = string.Empty, nom_exp = string.Empty, nomDocumento = string.Empty, nro_caja = string.Empty, nro_carpeta = string.Empty, txt_carpeta = string.Empty, observacion = string.Empty, rxtFechaIni = "S.F.", rxtFechaFin = "S.F."; codSerie = string.Empty; codSubserie = string.Empty;
            foreach (var item in datListDocumento.Select(p => new { p.id, p.t_carpeta.t_lote.p_subserie.p_serie.codigo, p.t_carpeta.t_lote.p_subserie, p.p_tipodoc.nombre, p.t_carpeta.id_tercero, p.nro_doc, p.observacion, p.t_carpeta.nro_caja, p.t_carpeta.nro_carpeta, p.folio_ini, p.folio_fin, p.pag_ini, p.fecha, p.fecha_regdoc, p.p_tipodoc.principal, p.id_carpeta, p.t_carpeta.hc_fin }).AsEnumerable().OrderBy(x => x.nro_carpeta).ThenBy(x => GlobalClass.GetNumber(x.hc_fin)).ThenBy(x => x.id_carpeta).ThenBy(x => x.pag_ini).ToList())
            {
                codSerie = item.codigo;
                codSubserie = item.p_subserie?.codigo;
                nomDocumento = item.nombre.ToUpper();
                nro_caja = "CJ" + int_caja.ToString().PadLeft(7, '0');
                folioIniActual = GlobalClass.GetNumber(item.folio_ini?.ToString(), -1);
                folioFinActual = GlobalClass.GetNumber(item.folio_fin?.ToString(), -1);
                observacion = txtPrimeraMayus(item.observacion?.Trim());
                //SI ES DOCUMENTO PRINCIPAL DE FUD
                //                if (((nomDocumento.Contains("FUD") || nomDocumento.Contains("NOVEDAD") || nomDocumento.Contains("SUBSIDIO")) && item.folio_ini == 1) && !string.IsNullOrEmpty(NumFUD))

                if (((nomDocumento.Contains("FUD") || nomDocumento.Contains("NOVEDAD") || nomDocumento.Contains("SUBSIDIO")) && item.folio_ini == 1))
                {
                    if (!string.IsNullOrEmpty(nomFUD) && nomFUD != item.nro_doc?.ToString().Trim())
                    {
                        rxtFechaIni = "S.F."; rxtFechaFin = "S.F.";
                        if (fecIni != DateTime.MinValue) rxtFechaIni = fecIni?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        if (fecFin != DateTime.MinValue) rxtFechaFin = fecFin?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        if (string.IsNullOrEmpty(rxtFechaIni) && !string.IsNullOrEmpty(rxtFechaFin)) rxtFechaIni = rxtFechaFin;
                        if (string.IsNullOrEmpty(rxtFechaFin) && !string.IsNullOrEmpty(rxtFechaIni)) rxtFechaFin = rxtFechaIni;
                        table.Rows.Add(new string[] { serial.ToString(), codSubdepen, codSerie, codSubserie, nom_exp, rxtFechaIni, rxtFechaFin, "X", "", "", folioIni.ToString(), folioFin.ToString(), nro_caja, nro_carpeta, "FISICO", "MEDIA", string.Empty });
                        serial++;

                        fecIni = DateTime.MinValue;
                        fecFin = DateTime.MinValue;
                        folioIni = -1;
                        folioFin = -1;
                    }
                    idTercero = GlobalClass.GetNumber(item.id_tercero?.ToString());
                    nomFUD = item.nro_doc?.ToString().Trim();
                    nom_exp = $@"{nomFUD} - " + getTercero(item.id, idTercero);

                }
                //Calcula FECHAS Extremas
                if (item.fecha != DateTime.MinValue && item.principal != 0)
                {
                    if (fecIni == DateTime.MinValue) fecIni = item.fecha;
                    fecFin = item.fecha;
                }
                if (item.fecha_regdoc != DateTime.MinValue)
                {
                    fecApertura = item.fecha_regdoc;
                }
                //Calcula Fecha de apertura
                //Calcula FOLIOS
                if (folioIniActual != -1 && folioIni == -1) folioIni = folioIniActual;
                if (folioFinActual != -1) folioFin = folioFinActual;

                nro_carpeta = "KP" + item.nro_carpeta?.ToString();
                //id_carpeta_actual = item.id_carpeta;
            }
            if (folioIni != -1)
            {
                if (string.IsNullOrEmpty(rxtFechaIni) && !string.IsNullOrEmpty(rxtFechaFin)) rxtFechaIni = rxtFechaFin;
                if (string.IsNullOrEmpty(rxtFechaFin) && !string.IsNullOrEmpty(rxtFechaIni)) rxtFechaFin = rxtFechaIni;
                table.Rows.Add(new string[] { serial.ToString(), codSubdepen, codSerie, codSubserie, nom_exp, rxtFechaIni, rxtFechaFin, "X", "", "", folioIni.ToString(), folioFin.ToString(), nro_caja, nro_carpeta, "FISICO", "MEDIA", string.Empty });
            }
            //Fecha de apertura de hoja de control
            //.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            graphicsPag.DrawString(fecApertura?.ToString("yyyy", CultureInfo.InvariantCulture), fontComun, PdfBrushes.Black, new PointF(recF3C1.X + (recF3C1.Width / 2), recF3C1.Y + 2), formatoTxtCentrado);
            graphicsPag.DrawString(fecApertura?.ToString("MM", CultureInfo.InvariantCulture), fontComun, PdfBrushes.Black, new PointF(recF3C3.X + (recF3C3.Width / 2), recF3C3.Y + 2), formatoTxtCentrado);
            graphicsPag.DrawString(fecApertura?.ToString("dd", CultureInfo.InvariantCulture), fontComun, PdfBrushes.Black, new PointF(recF3C4.X + (recF3C4.Width / 2), recF3C4.Y + 2), formatoTxtCentrado);

            PdfGrid pdfGrid = new PdfGrid();
            pdfGrid.DataSource = table;
            for (int i = 0; i < anchoHead.Count; i++)
            {
                pdfGrid.Columns[i].Width = anchoHead[i];
            }
            PdfStringFormat stringCentrado = new PdfStringFormat();   //Initialize PdfStringFormat and set the properties
            stringCentrado.Alignment = PdfTextAlignment.Center;
            stringCentrado.LineAlignment = PdfVerticalAlignment.Middle;
            //Estilo de fila en Table
            PdfGridCellStyle gridCellStyle = new PdfGridCellStyle();     //Initialize PdfGridCellStyle. Set background color and string format
            gridCellStyle.StringFormat = stringCentrado;
            gridCellStyle.Font = fontComun;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                PdfGridRow gridRow = pdfGrid.Rows[i];
                gridRow.ApplyStyle(gridCellStyle);
            }
            pdfGrid.BeginCellLayout += PdfGrid_BeginCellLayoutFUID;

            for (int c = 0; c < table.Rows.Count; c++)
            {
                PdfGridRow gridRow = pdfGrid.Rows[c];
                //gridRow.ApplyStyle(gridCellStyle);
                //Console.WriteLine(gridRow.Height);
                gridRow.Height = 11.94f;

                /*if (gridRow.Height < floatDefecto)
                {
                    gridRow.Height = floatDefecto;
                }*/
            }

            PdfGridLayoutResult pdfGridLayoutResult = pdfGrid.Draw(page, new PointF(0, recEnc1.Y + (float)16.5));    //Draw grid to the page of PDF document

            int pageCount = doc.Pages.Count;
            var lastPage = doc.Pages[pageCount - 1];//Identifica última Hoja
            /*float totalBorde = pdfGrid.;
            if (pageCount == 2) totalBorde += 570;
            if (pageCount == 3) totalBorde += 1300;*/

            /*var finalTabla = pdfGridLayoutResult.Bounds.Bottom;
            nota = dataFormato.p_formato.FirstOrDefault().hc_nota2;
            textNota = new PdfTextElement(nota, fontComun);
            RectangleF recNota2 = new RectangleF(imageBounds.X + 30, finalTabla + 3, 495, page.GetClientSize().Height);
            textNota.Draw(lastPage, recNota2);*/
            var lastpagGraphics = lastPage.Graphics;
            var altoFinal = pdfGridLayoutResult.Bounds.Bottom + 10;
            if (altoFinal > 400f)
            {
                altoFinal = 7f;
                doc.Pages.Add();
                pageCount = doc.Pages.Count;
                lastPage = doc.Pages[pageCount - 1];
                lastpagGraphics = lastPage.Graphics;
            }
            lastpagGraphics.DrawString("Elaborado por: ", fontComun, PdfBrushes.Black, new PointF(4, altoFinal), formatoTxtIzquierda);
            PointF point1 = new PointF(50, altoFinal + 7);
            PointF point2 = new PointF(255, altoFinal + 7);
            lastpagGraphics.DrawString(fi_elaboradox, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), altoFinal), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT Entidad Productora
            AlturaEncabezadoLinea2 = point2.Y + 7;
            lastpagGraphics.DrawString("Cargo:", fontComun, PdfBrushes.Black, new PointF(4, AlturaEncabezadoLinea2), formatoTxtIzquierda);
            point1 = new PointF(50, AlturaEncabezadoLinea2 + 7);
            point2 = new PointF(255, AlturaEncabezadoLinea2 + 7);
            lastpagGraphics.DrawString(fi_elaboradox_cargo, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea2), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT Entidad Productora
            AlturaEncabezadoLinea3 = point2.Y + 7;
            lastpagGraphics.DrawString("Firma", fontComun, PdfBrushes.Black, new PointF(4, AlturaEncabezadoLinea3), formatoTxtIzquierda);
            point1 = new PointF(50, AlturaEncabezadoLinea3 + 7);
            point2 = new PointF(255, AlturaEncabezadoLinea3 + 7);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT OFICINA Productora
            AlturaEncabezadoLinea4 = point2.Y + 7;
            lastpagGraphics.DrawString("Lugar", fontComun, PdfBrushes.Black, new PointF(4, AlturaEncabezadoLinea4), formatoTxtIzquierda);
            point1 = new PointF(50, AlturaEncabezadoLinea4 + 7);
            point2 = new PointF(255, AlturaEncabezadoLinea4 + 7);
            lastpagGraphics.DrawString(fi_lugar, fontComunMin, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea4), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            /// OBJETO
            AlturaEncabezadoLinea5 = point2.Y + 7;
            lastpagGraphics.DrawString("Fecha", fontComun, PdfBrushes.Black, new PointF(4, AlturaEncabezadoLinea5), formatoTxtIzquierda);
            point1 = new PointF(50, AlturaEncabezadoLinea5 + 7);
            point2 = new PointF(255, AlturaEncabezadoLinea5 + 7);
            lastpagGraphics.DrawString(fi_fecha, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea5), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document

            /*if (File.Exists("firmaElabora_" + idProyecto + ".png")) //Logo
            {
                image = new PdfBitmap("firmaElabora_" + idProyecto + ".png");
                imageBounds = new RectangleF(205, AlturaEncabezadoLinea3, 50, 30);//Setting image bounds
                lastpagGraphics.DrawImage(image, imageBounds);//Draw the image
            }*/
            //RECTANGULO
            RectangleF recFin1 = new RectangleF((float)0.5, altoFinal - 5, 260, 75); //Borde
            lastpagGraphics.DrawRectangle(borde, recFin1);

            lastpagGraphics.DrawString("Entregado por: ", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + 4, altoFinal), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + 50, altoFinal + 7);
            point2 = new PointF(recFin1.Width + 205, altoFinal + 7);
            lastpagGraphics.DrawString(fi_entregadox, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), altoFinal), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT Entidad Productora
            AlturaEncabezadoLinea2 = point2.Y + 7;
            lastpagGraphics.DrawString("Cargo:", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + 4, AlturaEncabezadoLinea2), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + 50, AlturaEncabezadoLinea2 + 7);
            point2 = new PointF(recFin1.Width + 205, AlturaEncabezadoLinea2 + 7);
            lastpagGraphics.DrawString(fi_entregadox_cargo, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea2), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT Entidad Productora
            AlturaEncabezadoLinea3 = point2.Y + 7;
            lastpagGraphics.DrawString("Firma", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + 4, AlturaEncabezadoLinea3), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + 50, AlturaEncabezadoLinea3 + 7);
            point2 = new PointF(recFin1.Width + 205, AlturaEncabezadoLinea3 + 7);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT OFICINA Productora
            AlturaEncabezadoLinea4 = point2.Y + 7;
            lastpagGraphics.DrawString("Lugar", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + 4, AlturaEncabezadoLinea4), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + 50, AlturaEncabezadoLinea4 + 7);
            point2 = new PointF(recFin1.Width + 205, AlturaEncabezadoLinea4 + 7);
            PdfHTMLTextElement element = new PdfHTMLTextElement();  //Create a text element  //element.Brush = new PdfSolidBrush(Color.Black);
            element.HTMLText = fi_lugar;
            element.Font = fontComunMin;
            element.TextAlign = TextAlign.Center;
            PdfMetafileLayoutFormat layoutFormat = new PdfMetafileLayoutFormat();
            layoutFormat.Layout = PdfLayoutType.Paginate;
            //PdfLayoutFormat layoutFormat = new PdfLayoutFormat(); //Set the properties to paginate the text
            layoutFormat.Break = PdfLayoutBreakType.FitPage;
            RectangleF bounds = new RectangleF(new PointF(recFin1.Width + 50, AlturaEncabezadoLinea4 - 1), new SizeF(point2.X - point1.X, 15));   //Set bounds to draw multiline text
            element.Draw(lastPage, bounds, layoutFormat);   //Draw the text element with the properties and formats set
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            /// OBJETO
            AlturaEncabezadoLinea5 = point2.Y + 7;
            lastpagGraphics.DrawString("Fecha", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + 4, AlturaEncabezadoLinea5), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + 50, AlturaEncabezadoLinea5 + 7);
            point2 = new PointF(recFin1.Width + 205, AlturaEncabezadoLinea5 + 7);
            lastpagGraphics.DrawString(fi_fecha, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea5), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
                                                              //FIRMA
            /*if (File.Exists("firmaEntrega_" + idProyecto + ".png")) //Logo
            {
                image = new PdfBitmap("firmaEntrega_" + idProyecto + ".png");
                imageBounds = new RectangleF(recFin1.Width + 155, AlturaEncabezadoLinea3, 50, 30);//Setting image bounds
                lastpagGraphics.DrawImage(image, imageBounds);//Draw the image
            }*/

            //RECTANGULO
            RectangleF recFin2 = new RectangleF(recFin1.Width + (float)0.5, altoFinal - 5, 210, 75); //Borde
            lastpagGraphics.DrawRectangle(borde, recFin2);

            //CUADRO TRES 3
            lastpagGraphics.DrawString("Recibido por: ", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + recFin2.Width + 4, altoFinal), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + recFin2.Width + 50, altoFinal + 7);
            point2 = new PointF(recFin1.Width + recFin2.Width + 235, altoFinal + 7);
            lastpagGraphics.DrawString(fi_recibidox, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), altoFinal), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT Entidad Productora
            AlturaEncabezadoLinea2 = point2.Y + 7;
            lastpagGraphics.DrawString("Cargo:", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + recFin2.Width + 4, AlturaEncabezadoLinea2), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + recFin2.Width + 50, AlturaEncabezadoLinea2 + 7);
            point2 = new PointF(recFin1.Width + recFin2.Width + 235, AlturaEncabezadoLinea2 + 7);
            lastpagGraphics.DrawString(fi_recibidox_cargo, fontComunMin5, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea2), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT Entidad Productora
            AlturaEncabezadoLinea3 = point2.Y + 7;
            lastpagGraphics.DrawString("Firma", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + recFin2.Width + 4, AlturaEncabezadoLinea3), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + recFin2.Width + 50, AlturaEncabezadoLinea3 + 7);
            point2 = new PointF(recFin1.Width + recFin2.Width + 235, AlturaEncabezadoLinea3 + 7);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            ///TXT OFICINA Productora
            AlturaEncabezadoLinea4 = point2.Y + 7;
            lastpagGraphics.DrawString("Lugar", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + recFin2.Width + 4, AlturaEncabezadoLinea4), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + recFin2.Width + 50, AlturaEncabezadoLinea4 + 7);
            point2 = new PointF(recFin1.Width + recFin2.Width + 235, AlturaEncabezadoLinea4 + 7);
            lastpagGraphics.DrawString(fi_lugar, fontComunMin, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea4), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
            /// OBJETO
            AlturaEncabezadoLinea5 = point2.Y + 7;
            lastpagGraphics.DrawString("Fecha", fontComun, PdfBrushes.Black, new PointF(recFin1.Width + recFin2.Width + 4, AlturaEncabezadoLinea5), formatoTxtIzquierda);
            point1 = new PointF(recFin1.Width + recFin2.Width + 50, AlturaEncabezadoLinea5 + 7);
            point2 = new PointF(recFin1.Width + recFin2.Width + 235, AlturaEncabezadoLinea5 + 7);
            lastpagGraphics.DrawString(fi_fecha, fontComun, PdfBrushes.Black, new PointF((point1.X + (point2.X - point1.X) / 2), AlturaEncabezadoLinea5), formatoTxtCentrado);
            lastpagGraphics.DrawLine(pen, point1, point2);    //Draw the line on PDF document
                                                              //RECTANGULO
            RectangleF recFin3 = new RectangleF(recFin1.Width + recFin2.Width + (float)0.5, altoFinal - 5, (float)241.5, 75); //Borde
            lastpagGraphics.DrawRectangle(borde, recFin3);

            lastpagGraphics.DrawString("Desarrollado por Alpha Intelligence AI", fontPie, PdfBrushes.Black, new PointF(50, recFin3.Y + recFin3.Height + 5), formatoTxtCentrado);//Draw the text.



            //Save the document.
            doc.Save($"{exportPath}/FUID_" + nro_caja + ".pdf");

            //Close the document.
            doc.Close(true);
        }


        public void ExportPfdCaja(int codigo, int idLote, string nro_caja, string exportFolderPath, ref List<string> exportedList) //"codigo" puede ser cualquier número, se le pregunta al usuario en el momento de generar el formato
        {
            if (codigo == 0 && string.IsNullOrEmpty(nro_caja)) return;
            IQueryable<t_carpeta> datFUID = EntitiesRepository.Entities.t_carpeta.Include("t_tercero").Include("t_lote").Include("p_proyecto").Include("p_formato");
            var datFUID2 = datFUID.Where(c => c.nro_caja == nro_caja && c.id_lote == idLote).Select(p => new { p.t_lote.p_proyecto.p_formato, p.t_lote.t_carpeta, p.t_tercero, p.t_lote.p_subdependencia, p.t_lote.p_proyecto, p.t_lote.p_subserie, p.fecha_expediente_ini, p.fecha_expediente_fin, p.nro_expediente, p.nom_expediente });
            var cuentaReg = datFUID2.Count();
            if (cuentaReg == 0)
            {
                MessageBox.Show("No hay datos para el FUID con esos parámetros.");
                return;
            }

            var dataFormato = datFUID2.FirstOrDefault().p_formato.FirstOrDefault();
            string cj_titulo1 = "", cj_titulo2 = "", cj_titulo3 = "", cj_cal_codigo = "", cj_cal_version = "", cj_cal_fecha = "", nomOrg = "", nomDependencia = "", codDependencia = "";
            DateTime dateDefecto = DateTime.MinValue;
            p_organizacion dataOrg;
            p_dependencia dataDependencia;
            p_subdependencia dataSUBDependencia;
            int idProyecto = 0;
            if (dataFormato != null)
            {
                dateDefecto = dataFormato.fecha_inicial_defecto ?? DateTime.MinValue;
                dataOrg = dataFormato.p_proyecto.p_organizacion;
                dataDependencia = dataOrg.p_dependencia.FirstOrDefault();
                nomDependencia = dataDependencia.nombre;
                codDependencia = dataDependencia.codigo;
                dataSUBDependencia = dataDependencia.p_subdependencia.FirstOrDefault();
                nomOrg = dataOrg.nombre;
                idProyecto = dataFormato.p_proyecto.id;
                if (!string.IsNullOrEmpty(dataFormato.cj_titulo1)) cj_titulo1 = dataFormato.cj_titulo1;
                if (!string.IsNullOrEmpty(dataFormato.cj_titulo2)) cj_titulo2 = dataFormato.cj_titulo2;
                if (!string.IsNullOrEmpty(dataFormato.cj_titulo3)) cj_titulo3 = dataFormato.hc_titulo3;
                if (!string.IsNullOrEmpty(dataFormato.cj_cal_codigo)) cj_cal_codigo = dataFormato.cj_cal_codigo;
                if (!string.IsNullOrEmpty(dataFormato.cj_cal_version)) cj_cal_version = dataFormato.cj_cal_version;
                if (!string.IsNullOrEmpty(dataFormato.cj_cal_fecha)) cj_cal_fecha = dataFormato.cj_cal_fecha;
            }

            var dataSUBSerie = datFUID2.FirstOrDefault().p_subserie;

            //Create a new PDF document.
            PdfDocument doc = new PdfDocument();
            doc.PageSettings.Orientation = PdfPageOrientation.Portrait;
            doc.PageSettings.Size = PdfPageSize.Letter; //Width = 612 Height = 792
            doc.PageSettings.Margins.All = 5;
            PdfPage page = doc.Pages.Add();//Add a page to the document.
            PdfGraphics graphicsPag = page.Graphics;

            //FOOTER
            PdfStringFormat formatoTxtPie = new PdfStringFormat(); formatoTxtPie.Alignment = PdfTextAlignment.Center;
            PdfFont fontpie = new PdfStandardFont(PdfFontFamily.Helvetica, 6);//Set the standard font.
            /*RectangleF recFooter = new RectangleF(0, 0, doc.Pages[0].GetClientSize().Width, 10);
            PdfPageTemplateElement footer = new PdfPageTemplateElement(recFooter);
            PdfGraphics graphicsFooter = footer.Graphics;
            PdfFont fontPie = new PdfStandardFont(PdfFontFamily.Helvetica, 6);//Set the standard font.
            graphicsFooter.DrawString("Desarrollado por Alpha Intelligence AI", fontPie, PdfBrushes.Black, new PointF(50, 0), formatoTxtPie);//Draw the text.
            doc.Template.Bottom = footer; */

            ///RECUADROS SUPERIORES
            RectangleF recBorde = new RectangleF(62, (float)0.5, 114, 44);
            PdfPen borde = new PdfPen(Color.Black, 1);
            graphicsPag.DrawRectangle(borde, recBorde);
            recBorde = new RectangleF(62, (float)0.5, 283, 44);
            graphicsPag.DrawRectangle(borde, recBorde);
            //Número Consecutivo - codigo
            PdfStringFormat formatoTxtCentrado = new PdfStringFormat(); formatoTxtCentrado.Alignment = PdfTextAlignment.Center;
            PdfStringFormat formatoTxtIzquierda = new PdfStringFormat(); formatoTxtIzquierda.Alignment = PdfTextAlignment.Left;
            PdfFont fontCodigo = new PdfStandardFont(PdfFontFamily.Helvetica, 24, PdfFontStyle.Bold);//Set the standard font.
            graphicsPag.DrawString(codigo.ToString(), fontCodigo, PdfBrushes.Black, new PointF(119, 7), formatoTxtCentrado);//Draw the text.
            //CODIGO DE BARRAS CAJA
            PdfCode39Barcode barcode = new PdfCode39Barcode();//Drawing Code39 barcode
            barcode.BarHeight = 40;//Setting height of the barcode
            PdfFont fontBarcode = new PdfStandardFont(PdfFontFamily.Helvetica, 0, PdfFontStyle.Bold);//Set the standard font.
            barcode.Font = fontBarcode;
            barcode.Text = codigo.ToString();
            barcode.Size = new SizeF(150, 40);
            barcode.Draw(page, new PointF(186, 3));//Printing barcode on to the Pdf.
            //ENCABEZADO
            ///RECUADROS SUPERIORES
            recBorde = new RectangleF((float)0.5, recBorde.Height + 6, 97, 42);
            graphicsPag.DrawRectangle(borde, recBorde);
            recBorde = new RectangleF(recBorde.X, recBorde.Y, 329, 42);
            graphicsPag.DrawRectangle(borde, recBorde);
            recBorde = new RectangleF(recBorde.X, recBorde.Y, 408, 42);
            graphicsPag.DrawRectangle(borde, recBorde);
            PdfBitmap image;
            RectangleF imageBounds = new RectangleF(6, recBorde.Y + 10, 84, 22);//Setting image bounds
            if (File.Exists("logo_" + idProyecto + ".png")) //Logo
            {
                image = new PdfBitmap("logo_" + idProyecto + ".png");
                graphicsPag.DrawImage(image, imageBounds);//Draw the image
            }
            ////////TITULO
            PdfFont fontTitulo = new PdfStandardFont(PdfFontFamily.Helvetica, 7, PdfFontStyle.Bold);//Set the standard font.
            graphicsPag.DrawString(cj_titulo1, fontTitulo, PdfBrushes.Black, new PointF(204, recBorde.Y + 6), formatoTxtCentrado);//Draw the text.
            graphicsPag.DrawString(cj_titulo2, fontTitulo, PdfBrushes.Black, new PointF(204, recBorde.Y + 17), formatoTxtCentrado);//Draw the text.
            graphicsPag.DrawString(cj_titulo3, fontTitulo, PdfBrushes.Black, new PointF(204, recBorde.Y + 28), formatoTxtCentrado);//Draw the text.
            ////////CALIDAD
            //PdfFont fontFormatoNegrita = new PdfStandardFont(PdfFontFamily.Helvetica, 5, PdfFontStyle.Bold);
            graphicsPag.DrawString(cj_cal_codigo, fontTitulo, PdfBrushes.Black, new PointF(333, recBorde.Y + 4), formatoTxtIzquierda);
            graphicsPag.DrawString(cj_cal_version, fontTitulo, PdfBrushes.Black, new PointF(333, recBorde.Y + 13), formatoTxtIzquierda);
            graphicsPag.DrawString(cj_cal_fecha, fontTitulo, PdfBrushes.Black, new PointF(333, recBorde.Y + 22), formatoTxtIzquierda);
            //Conteo de PÁGINAS
            PdfPageNumberField pageNumber = new PdfPageNumberField();
            PdfPageCountField count = new PdfPageCountField();
            PdfCompositeField compositeField = new PdfCompositeField(fontTitulo, PdfBrushes.Black, "Páginas {0} de {1}", pageNumber, count);
            compositeField.StringFormat = formatoTxtIzquierda;
            compositeField.Draw(graphicsPag, new PointF(333, recBorde.Y + 31));
            ///RECUADRO ORGANIZACIÓN
            recBorde = new RectangleF((float)0.5, recBorde.Y + 42, 408, 42);
            graphicsPag.DrawRectangle(borde, recBorde);

            PdfFont fontOrg = new PdfStandardFont(PdfFontFamily.Helvetica, 14);
            PdfHTMLTextElement element = new PdfHTMLTextElement();  //Create a text element  //element.Brush = new PdfSolidBrush(Color.Black);
            element.HTMLText = nomOrg;
            element.Font = fontOrg;
            element.TextAlign = TextAlign.Center;
            PdfMetafileLayoutFormat layoutFormat = new PdfMetafileLayoutFormat();
            layoutFormat.Layout = PdfLayoutType.Paginate;
            //PdfLayoutFormat layoutFormat = new PdfLayoutFormat(); //Set the properties to paginate the text
            layoutFormat.Break = PdfLayoutBreakType.FitPage;
            RectangleF bounds = new RectangleF(new PointF(4, recBorde.Y + 5), new SizeF(400, 36));   //Set bounds to draw multiline text
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set

            ///RECUADRO DEPENDENCIA
            recBorde = new RectangleF((float)0.5, bounds.Y + bounds.Height + (float)1, 408, 40);
            graphicsPag.DrawRectangle(borde, recBorde);
            recBorde = new RectangleF((float)0.5, recBorde.Y, 85, 40);
            graphicsPag.DrawRectangle(borde, recBorde);

            PdfFont fontDEPserie = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
            graphicsPag.DrawString("DEPENDENCIA:", fontDEPserie, PdfBrushes.Black, new PointF(4, recBorde.Y + 13), formatoTxtIzquierda);//Draw the text.
            element.HTMLText = "<b>" + nomDependencia + "</b>";
            element.Font = fontDEPserie;
            bounds = new RectangleF(new PointF(recBorde.X + 89, recBorde.Y + 12), new SizeF(315, 26));   //Set bounds to draw multiline text
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set

            ///RECUADRO CODIGO DEPENDENCIA
            recBorde = new RectangleF((float)0.5, recBorde.Y + recBorde.Height + (float)0, 408, 40);
            graphicsPag.DrawRectangle(borde, recBorde);
            recBorde = new RectangleF((float)0.5, recBorde.Y, 85, 40);
            graphicsPag.DrawRectangle(borde, recBorde);
            element.HTMLText = "CÓDIGO DE LA DEPENDENCIA:";
            element.TextAlign = TextAlign.Left;
            bounds = new RectangleF(new PointF(4, recBorde.Y + 8), new SizeF(77, 26));   //Set bounds to draw multiline text
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set
            element.HTMLText = "<b>" + codDependencia + "</b>";
            element.TextAlign = TextAlign.Center;
            bounds = new RectangleF(new PointF(recBorde.X + 89, recBorde.Y + 13), new SizeF(315, 26));   //Set bounds to draw multiline text
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set

            ///RECUADRO CODIGO SERIE Y CODIGO
            recBorde = new RectangleF((float)0.5, recBorde.Y + recBorde.Height + (float)0, 408, 40);
            graphicsPag.DrawRectangle(borde, recBorde);
            recBorde = new RectangleF((float)0.5, recBorde.Y, 85, 40);
            graphicsPag.DrawRectangle(borde, recBorde);
            element.HTMLText = "SERIE Y CÓDIGO";
            element.TextAlign = TextAlign.Left;
            bounds = new RectangleF(new PointF(4, recBorde.Y + 13), new SizeF(77, 26));   //Set bounds to draw multiline text
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set
            element.HTMLText = "<b>" + dataSUBSerie.p_serie.nombre + "\n" + dataSUBSerie.p_serie.codigo + "</b>";
            element.TextAlign = TextAlign.Center;
            bounds = new RectangleF(new PointF(recBorde.X + 89, recBorde.Y + 6), new SizeF(315, 26));   //Set bounds to draw multiline text
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set

            ///RECUADRO CODIGO SUBSERIE Y CODIGO
            recBorde = new RectangleF((float)0.5, recBorde.Y + recBorde.Height + (float)0, 408, 40);
            graphicsPag.DrawRectangle(borde, recBorde);
            recBorde = new RectangleF((float)0.5, recBorde.Y, 85, 40);
            graphicsPag.DrawRectangle(borde, recBorde);
            element.HTMLText = "SUBSERIE Y CÓDIGO";
            element.TextAlign = TextAlign.Left;
            bounds = new RectangleF(new PointF(4, recBorde.Y + 8), new SizeF(77, 26));   //Set bounds to draw multiline text
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set
            element.HTMLText = "<b>" + dataSUBSerie.nombre + "\n" + dataSUBSerie.codigo + "</b>";
            element.TextAlign = TextAlign.Center;
            bounds = new RectangleF(new PointF(recBorde.X + 89, recBorde.Y + 4), new SizeF(315, 32));   //Set bounds to draw multiline text
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set

            ///RECUADRO No CAJA
            recBorde = new RectangleF((float)0.5, recBorde.Y + recBorde.Height + (float)0, 408, 14);
            graphicsPag.DrawRectangle(borde, recBorde);
            element.HTMLText = "No. DE CAJA";
            element.TextAlign = TextAlign.Center;
            bounds = new RectangleF(new PointF(recBorde.X + 4, recBorde.Y + 2), new SizeF(400, 10));   //Set bounds to draw multiline text
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set
            recBorde = new RectangleF((float)0.5, recBorde.Y + recBorde.Height + (float)0, 408, 56);
            graphicsPag.DrawRectangle(borde, recBorde);
            fontBarcode = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);//Set the standard font.
            barcode.BarHeight = 52;//Barcode
            barcode.Font = fontBarcode;
            barcode.Text = nro_caja.PadLeft(5, '0');
            barcode.Size = new SizeF(220, barcode.BarHeight);
            barcode.Draw(page, new PointF(94, recBorde.Y + 3));//Printing barcode on to the Pdf.

            //BUCLE PARA CALCULAR DATOS SIGUIENTES
            DateTime fechaInicial = DateTime.MaxValue;
            DateTime fechaFinal = DateTime.MinValue;
            DateTime vrfechaIni, vrfechaFin;
            string txtqrCaja = "CAJA No.: " + nro_caja + ".";
            foreach (var item in datFUID2.ToList())
            {
                //Console.WriteLine(item.fecha_expediente_ini.ToString());
                vrfechaIni = item.fecha_expediente_ini ?? DateTime.MinValue;
                vrfechaFin = item.fecha_expediente_fin ?? DateTime.MinValue;
                if (vrfechaIni != DateTime.MinValue && vrfechaIni < fechaInicial) fechaInicial = vrfechaIni;
                if (vrfechaFin > fechaFinal) fechaFinal = vrfechaFin;
                txtqrCaja = txtqrCaja + " CARPETA: " + item.nro_expediente + "/" + item.nom_expediente;
            }
            vrfechaIni = dateDefecto; //VALIDA FECHA POR DEFECTO
            if (vrfechaIni > fechaInicial) fechaInicial = vrfechaIni;
            ///RECUADRO FECHAS EXTREMAS
            recBorde = new RectangleF((float)0.5, recBorde.Y + recBorde.Height + (float)0, 408, 14);
            graphicsPag.DrawRectangle(borde, recBorde);
            element.HTMLText = "FECHAS EXTREMAS (dd/mm/aaaa):     " + fechaInicial.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + " - " + fechaFinal.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            element.TextAlign = TextAlign.Left;
            bounds = new RectangleF(new PointF(recBorde.X + 4, recBorde.Y + 2), new SizeF(400, 10));   //Set bounds to draw multiline text
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set
            ///AÑOS
            recBorde = new RectangleF((float)0.5, recBorde.Y + recBorde.Height + (float)0, 408, 14);
            graphicsPag.DrawRectangle(borde, recBorde);
            element.HTMLText = "AÑOS (aaaa):                                        " + fechaInicial.ToString("yyyy", CultureInfo.InvariantCulture) + " - " + fechaFinal.ToString("yyyy", CultureInfo.InvariantCulture);
            bounds = new RectangleF(new PointF(recBorde.X + 4, recBorde.Y + 2), new SizeF(400, 10));   //Set bounds to draw multiline text
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set
            ///RECUADRO QR
            recBorde = new RectangleF((float)0.5, recBorde.Y + recBorde.Height + (float)0, 408, 255);
            graphicsPag.DrawRectangle(borde, recBorde);
            PdfQRBarcode barcodeQr = new PdfQRBarcode();//Drawing QR Barcode
            barcodeQr.ErrorCorrectionLevel = PdfErrorCorrectionLevel.Medium;//Set Error Correction Level
            barcodeQr.XDimension = 3;//Set XDimension
            barcodeQr.Size = new SizeF(250, 250);
            int lonTxtQR = txtqrCaja.Length; if (lonTxtQR > 1024) txtqrCaja = txtqrCaja.Substring(0, 1024);
            barcodeQr.Text = Regex.Replace(txtqrCaja, @"[^0-9a-zA-Z:,|._-Ññ]+", " ");
            barcodeQr.Draw(page, new PointF(79, recBorde.Y + 2));//Printing barcode on to the Pdf.

            graphicsPag.DrawString("Desarrollado por Alpha Intelligence AI", fontpie, PdfBrushes.Black, new PointF(50, recBorde.Y + recBorde.Height - 8), formatoTxtPie);

            //Save the document.
            string filename = $"{exportFolderPath}" + "/Caja-" + nro_caja + ".pdf";
            doc.Save(filename);
            //Close the document.
            doc.Close(true);
            exportedList.Add(filename);
        }
        public void ExportPfdCaja2(int codigo, string nomLote, string nro_caja, string exportFolderPath, ref List<string> exportedList, p_formato dataFormato) //"codigo" puede ser cualquier número, se le pregunta al usuario en el momento de generar el formato
        {
            if (codigo == 0) codigo++;
            if (string.IsNullOrEmpty(nomLote) && string.IsNullOrEmpty(nro_caja)) return;
            string cj_titulo1 = "", cj_titulo2 = "", cj_titulo3 = "", cj_cal_codigo = "", cj_cal_version = "", cj_cal_fecha = "", nomOrg = string.Empty, nomDependencia = string.Empty, codDependencia = string.Empty, nomSubdepen = string.Empty, codSubdepen = string.Empty, nomSerie = string.Empty, codSerie = string.Empty, nomSubserie = string.Empty, codSubserie = string.Empty;
            DateTime dateDefecto = DateTime.MinValue;

            var qEncabezado = EntitiesRepository.Entities.t_carpeta.Include("t_lote").Include("t_lote").Include("p_subserie").Include("p_serie").Include("p_subdependencia").Include("p_dependencia").Include("p_organizacion").Where(c => c.nro_caja == nro_caja && c.t_lote.nom_lote == nomLote);
            //Texto organización
            //nomOrg = qEncabezado.FirstOrDefault().t_lote.p_proyecto.p_organizacion?.nombre;
            //Texto Dependencia
            foreach (var item in qEncabezado.Select(m => m.t_lote.p_subdependencia.p_dependencia).Distinct().ToList())
            {
                if (string.IsNullOrEmpty(codDependencia)) codDependencia = item.codigo;
                else codDependencia += " - " + item.codigo;
                if (string.IsNullOrEmpty(nomDependencia)) nomDependencia = item.nombre;
                else nomDependencia += " - " + item.nombre;
            }
            //Texto SubDependencia
            foreach (var item in qEncabezado.Select(m => m.t_lote.p_subdependencia).Distinct().ToList())
            {
                if (string.IsNullOrEmpty(codSubdepen)) codSubdepen = item.cod;
                else codSubdepen += " - " + item.cod;
                if (string.IsNullOrEmpty(nomSubdepen)) nomSubdepen = item.nombre;
                else nomSubdepen += " - " + item.nombre;
            }
            //Texto Serie
            foreach (var item in qEncabezado.Select(m => new { m.t_lote.p_subserie.p_serie.codigo, m.t_lote.p_subserie.p_serie.nombre }).Distinct().ToList())
            {
                if (string.IsNullOrEmpty(codSerie)) codSerie = item.codigo;
                else codSerie += " - " + item.codigo;
                if (string.IsNullOrEmpty(nomSerie)) nomSerie = item.nombre;
                else nomSerie += " - " + item.nombre;
            }
            //Texto Sub Serie
            foreach (var item in qEncabezado.Select(m => m.t_lote.p_subserie).Distinct().ToList())
            {
                if (string.IsNullOrEmpty(codSubserie)) codSubserie = item.codigo;
                else codSubserie += " - " + item.codigo;
                if (string.IsNullOrEmpty(nomSubserie)) nomSubserie = item.nombre;
                else nomSubserie += " - " + item.nombre;
            }

            IQueryable<t_carpeta> datFUID = EntitiesRepository.Entities.t_carpeta.Include("t_lote").Include("p_subserie").Include("p_proyecto").AsNoTracking().Where(p => p.t_lote.id_proyecto == GlobalClass.id_proyecto && p.nro_caja == nro_caja && p.t_lote.nom_lote == nomLote);
            var datFUID2 = datFUID.Select(p => new { p.fecha_expediente_ini, p.fecha_expediente_fin, p.nro_expediente, p.nom_expediente, p.t_lote.p_subserie.nombre, p.nro_carpeta, p.tomo, p.tomo_fin });
            var cuentaReg = datFUID2.Count();
            if (cuentaReg == 0)
            {
                //MessageBox.Show("No hay datos para el FUID con esos parámetros.");
                return;
            }

            //var dataFormato = datFUID2.FirstOrDefault().p_formato.FirstOrDefault();

            int idProyecto = 0;

            if (dataFormato != null)
            {
                p_organizacion dataOrg;
                dateDefecto = dataFormato.fecha_inicial_defecto ?? DateTime.MinValue;
                idProyecto = dataFormato.p_proyecto.id;
                //Nombre de la organización
                dataOrg = dataFormato.p_proyecto.p_organizacion;
                nomOrg = dataOrg.nombre;
                if (!string.IsNullOrEmpty(dataFormato.cj_titulo1)) cj_titulo1 = dataFormato.cj_titulo1;
                if (!string.IsNullOrEmpty(dataFormato.cj_titulo2)) cj_titulo2 = dataFormato.cj_titulo2;
                if (!string.IsNullOrEmpty(dataFormato.cj_titulo3)) cj_titulo3 = dataFormato.hc_titulo3;
                if (!string.IsNullOrEmpty(dataFormato.cj_cal_codigo)) cj_cal_codigo = dataFormato.cj_cal_codigo;
                if (!string.IsNullOrEmpty(dataFormato.cj_cal_version)) cj_cal_version = dataFormato.cj_cal_version;
                if (!string.IsNullOrEmpty(dataFormato.cj_cal_fecha)) cj_cal_fecha = dataFormato.cj_cal_fecha;
            }

            //Create a new PDF document.
            PdfDocument doc = new PdfDocument();
            doc.PageSettings.Orientation = PdfPageOrientation.Portrait;
            doc.PageSettings.Size = PdfPageSize.Letter; //Width = 612 Height = 792
            doc.PageSettings.Margins.Top = 1;
            doc.PageSettings.Margins.Left = 1;
            PdfPage page = doc.Pages.Add();//Add a page to the document.
            PdfGraphics graphicsPag = page.Graphics;

            //Rectangulo para recortar
            PdfPen bordePunteado = new PdfPen(Color.Black, 1);
            bordePunteado.DashStyle = PdfDashStyle.DashDot;
            RectangleF recBordeTest = new RectangleF(0.5f, 0.5f, 510, 708);
            graphicsPag.DrawRectangle(bordePunteado, recBordeTest);
            //Factor de escala
            float fx = 1.18f;
            float fy = 1.09f;
            float fl = (float)Math.Round(fy, 1, MidpointRounding.ToEven);
            //FOOTER
            PdfStringFormat formatoTxtPie = new PdfStringFormat(); formatoTxtPie.Alignment = PdfTextAlignment.Center;
            PdfFont fontpie = new PdfStandardFont(PdfFontFamily.Helvetica, 5.5f);//Set the standard font.

            //ENCABEZADO
            PdfStringFormat formatoTxtCentrado = new PdfStringFormat(); formatoTxtCentrado.Alignment = PdfTextAlignment.Center;
            PdfStringFormat formatoTxtIzquierda = new PdfStringFormat(); formatoTxtIzquierda.Alignment = PdfTextAlignment.Left;
            PdfFont fontBarcode = new PdfStandardFont(PdfFontFamily.Helvetica, 0, PdfFontStyle.Bold);//Set the standard font.
            PdfCode39Barcode barcode = new PdfCode39Barcode();//Drawing Code39 barcode
            barcode.Font = fontBarcode;
            ///RECUADROS SUPERIORES
            //RectangleF recBorde = new RectangleF(14f, 14f, 114*fx, 42*fy);
            PdfPen borde = new PdfPen(Color.Black, 1);  //ecBorde = new RectangleF((float)0.5, recBorde.Height + 6, 97, 42);
            PdfPen bordeRojo = new PdfPen(Color.Red, 1);
            PdfPen bordeAzul = new PdfPen(Color.Blue, 1);
            PdfPen bordeVerde = new PdfPen(Color.Green, 1);

            RectangleF recBorde = new RectangleF(14f, 14f, 408 * fx, 42 * fy);
            graphicsPag.DrawRectangle(borde, recBorde);
            PdfBitmap image;
            RectangleF imageBounds = new RectangleF(recBorde.X + 6, recBorde.Y + 10, 95 * fx, 22 * fy);//Setting image bounds
            if (File.Exists("logo_" + idProyecto + ".png")) //Logo
            {
                image = new PdfBitmap("logo_" + idProyecto + ".png");
                graphicsPag.DrawImage(image, imageBounds);//Draw the image
            }
            ////////TITULO
            //Rectangulos titulos
            PdfFont fontTitulo = new PdfStandardFont(PdfFontFamily.Helvetica, 7 * fl, PdfFontStyle.Bold);//Set the standard font.
            RectangleF recBordeT = new RectangleF(118f * fx, recBorde.Y, 215f * fx, 15 * fy);
            graphicsPag.DrawRectangle(borde, recBordeT);
            graphicsPag.DrawString(cj_titulo1, fontTitulo, PdfBrushes.Black, new PointF((recBordeT.X + recBordeT.Width / 2), recBorde.Y + 4 * fy), formatoTxtCentrado);//Draw the text.
            recBordeT = new RectangleF(recBordeT.X, recBorde.Y + recBordeT.Height, recBordeT.Width, 13 * fy);
            graphicsPag.DrawRectangle(borde, recBordeT);
            graphicsPag.DrawString(cj_titulo2, fontTitulo, PdfBrushes.Black, new PointF((recBordeT.X + recBordeT.Width / 2), recBorde.Y + 17 * fy), formatoTxtCentrado);//Draw the text.
            recBordeT = new RectangleF(recBordeT.X, recBordeT.Y + recBordeT.Height, recBordeT.Width, 14 * fy);
            graphicsPag.DrawRectangle(borde, recBordeT);
            graphicsPag.DrawString(cj_titulo3, fontTitulo, PdfBrushes.Black, new PointF((recBordeT.X + recBordeT.Width / 2), recBorde.Y + 31 * fy), formatoTxtCentrado);//Draw the text.
            ////////CALIDAD
            //PdfFont fontFormatoNegrita = new PdfStandardFont(PdfFontFamily.Helvetica, 5, PdfFontStyle.Bold);
            graphicsPag.DrawString(cj_cal_codigo, fontTitulo, PdfBrushes.Black, new PointF(recBordeT.X + recBordeT.Width + 5, recBorde.Y + 4 * fy), formatoTxtIzquierda);
            graphicsPag.DrawString(cj_cal_version, fontTitulo, PdfBrushes.Black, new PointF(recBordeT.X + recBordeT.Width + 5, recBorde.Y + 13 * fy), formatoTxtIzquierda);
            graphicsPag.DrawString(cj_cal_fecha, fontTitulo, PdfBrushes.Black, new PointF(recBordeT.X + recBordeT.Width + 5, recBorde.Y + 22 * fy), formatoTxtIzquierda);
            PdfPageNumberField pageNumber = new PdfPageNumberField();   //Conteo de PÁGINAS
            PdfPageCountField count = new PdfPageCountField();
            PdfCompositeField compositeField = new PdfCompositeField(fontTitulo, PdfBrushes.Black, "Páginas {0} de {1}", pageNumber, count);
            compositeField.StringFormat = formatoTxtIzquierda;
            compositeField.Draw(graphicsPag, new PointF(recBordeT.X + recBordeT.Width + 5, recBorde.Y + 31 * fy));

            ///RECUADRO ORGANIZACIÓN
            RectangleF recBordeOrg = new RectangleF(recBorde.X, recBorde.Y + recBorde.Height, 408 * fx, 42 * fy);
            graphicsPag.DrawRectangle(borde, recBordeOrg);

            PdfFont fontOrg = new PdfStandardFont(PdfFontFamily.Helvetica, 14 * fl);
            PdfHTMLTextElement element = new PdfHTMLTextElement();  //Create a text element  //element.Brush = new PdfSolidBrush(Color.Black);
            element.HTMLText = nomOrg?.Trim();
            element.Font = fontOrg;
            element.TextAlign = TextAlign.Center;
            PdfMetafileLayoutFormat layoutFormat = new PdfMetafileLayoutFormat();
            layoutFormat.Layout = PdfLayoutType.Paginate;
            //PdfLayoutFormat layoutFormat = new PdfLayoutFormat(); //Set the properties to paginate the text
            layoutFormat.Break = PdfLayoutBreakType.FitPage;
            RectangleF bounds = new RectangleF(new PointF(recBordeOrg.X + 2, recBordeOrg.Y + 5), new SizeF(recBordeOrg.Width - 4, recBordeOrg.Height - 7));
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set

            ///RECUADRO DEPENDENCIA
            RectangleF recBordeDepen = new RectangleF(recBordeOrg.X, recBordeOrg.Y + recBordeOrg.Height, 85 * fx, 40 * fy);
            graphicsPag.DrawRectangle(borde, recBordeDepen);
            RectangleF recBordeDepenDato = new RectangleF(recBordeDepen.X + recBordeDepen.Width, recBordeDepen.Y, 323 * fx, recBordeDepen.Height);
            graphicsPag.DrawRectangle(borde, recBordeDepenDato);
            //Titulo
            PdfFont fontTitulos = new PdfStandardFont(PdfFontFamily.Helvetica, 9 * fl);
            element.HTMLText = "SECCIÓN - UNIDAD ADMINISTRATIVA Y CÓDIGO:";
            element.Font = fontTitulos; element.TextAlign = TextAlign.Left;
            bounds = new RectangleF(new PointF(recBordeDepen.X + 2, recBordeDepen.Y + 5), new SizeF(recBordeDepen.Width - 4, recBordeDepen.Height - 7));
            element.Draw(page, bounds, layoutFormat);
            //Dato
            element.HTMLText = "<b>" + nomDependencia + "</b>";
            element.Font = fontTitulos; element.TextAlign = TextAlign.Center;
            bounds = new RectangleF(new PointF(recBordeDepenDato.X + 2, recBordeDepenDato.Y + 12), new SizeF(recBordeDepenDato.Width - 4, recBordeDepenDato.Height - 14));
            element.Draw(page, bounds, layoutFormat);


            ///RECUADRO SUBSECCIÓN
            RectangleF recBordeSeccion = new RectangleF(recBordeDepen.X, recBordeDepen.Y + recBordeDepen.Height, 85 * fx, 40 * fy);
            graphicsPag.DrawRectangle(borde, recBordeSeccion);
            RectangleF recBordeSeccionDato = new RectangleF(recBordeSeccion.X + recBordeSeccion.Width, recBordeSeccion.Y, 323 * fx, recBordeSeccion.Height);
            graphicsPag.DrawRectangle(borde, recBordeSeccionDato);
            //Titulo
            PdfFont fontTitle = new PdfStandardFont(PdfFontFamily.Helvetica, 9 * fl);
            element.HTMLText = "SUBSECCIÓN - OFICINA PRODUCTORA Y CÓDIGO:";
            element.Font = fontTitle; element.TextAlign = TextAlign.Left;
            bounds = new RectangleF(new PointF(recBordeSeccion.X + 2, recBordeSeccion.Y + 3), new SizeF(recBordeSeccion.Width - 4, recBordeSeccion.Height - 5));
            //bounds = new RectangleF(new PointF(4, recBorde.Y + 3), new SizeF(77, 36));   //Set bounds to draw multiline text
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set
            //DATO
            element.HTMLText = "<b>" + nomSubdepen + "</b>"; element.TextAlign = TextAlign.Center;
            //bounds = new RectangleF(new PointF(recBorde.X + 89, recBorde.Y + 13), new SizeF(315, 26));   //Set bounds to draw multiline text
            bounds = new RectangleF(new PointF(recBordeSeccionDato.X + 2, recBordeSeccionDato.Y + 13), new SizeF(recBordeSeccionDato.Width - 4, recBordeSeccionDato.Height - 15));
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set


            ///RECUADRO CODIGO SERIE Y CODIGO
            RectangleF recBordeSerie = new RectangleF(recBordeSeccion.X, recBordeSeccion.Y + recBordeSeccion.Height, 85 * fx, 40 * fy);
            graphicsPag.DrawRectangle(borde, recBordeSerie);
            RectangleF recBordeSerieDato = new RectangleF(recBordeSerie.X + recBordeSerie.Width, recBordeSerie.Y, 323 * fx, recBordeSerie.Height);
            graphicsPag.DrawRectangle(borde, recBordeSerieDato);
            element.HTMLText = "SERIE Y CÓDIGO";
            element.Font = fontTitle;
            element.TextAlign = TextAlign.Left;
            bounds = new RectangleF(new PointF(recBordeSerie.X + 2, recBordeSerie.Y + 13), new SizeF(recBordeSerie.Width - 4, recBordeSerie.Height - 15));
            //bounds = new RectangleF(new PointF(4, recBorde.Y + 13), new SizeF(77, 26));   //Set bounds to draw multiline text
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set
            element.HTMLText = "<b>" + nomSerie + "</b>";
            element.TextAlign = TextAlign.Center;
            //bounds = new RectangleF(new PointF(recBorde.X + 89, recBorde.Y + 6), new SizeF(315, 26));   //Set bounds to draw multiline text
            bounds = new RectangleF(new PointF(recBordeSerieDato.X + 2, recBordeSerieDato.Y + 6), new SizeF(recBordeSerieDato.Width - 4, recBordeSerieDato.Height - 8));
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set

            ///RECUADRO CODIGO SUBSERIE Y CODIGO
            RectangleF recBordeSubserie = new RectangleF(recBordeSerie.X, recBordeSerie.Y + recBordeSerie.Height, 85 * fx, 40 * fy);
            graphicsPag.DrawRectangle(borde, recBordeSubserie);
            RectangleF recBordeSubserieDato = new RectangleF(recBordeSubserie.X + recBordeSubserie.Width, recBordeSubserie.Y, 323 * fx, recBordeSubserie.Height);
            graphicsPag.DrawRectangle(borde, recBordeSubserieDato);
            element.HTMLText = "SUBSERIE Y CÓDIGO";
            element.TextAlign = TextAlign.Left;
            bounds = new RectangleF(new PointF(recBordeSubserie.X + 2, recBordeSubserie.Y + 8), new SizeF(recBordeSubserie.Width - 4, recBordeSubserie.Height - 10));
            //bounds = new RectangleF(new PointF(4, recBorde.Y + 8), new SizeF(77, 26));   //Set bounds to draw multiline text
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set
            element.HTMLText = "<b>" + nomSubserie + "</b>";
            element.TextAlign = TextAlign.Center;
            bounds = new RectangleF(new PointF(recBordeSubserieDato.X + 2, recBordeSubserieDato.Y + 4), new SizeF(recBordeSubserieDato.Width - 4, recBordeSubserieDato.Height - 6));
            //bounds = new RectangleF(new PointF(recBorde.X + 89, recBorde.Y + 4), new SizeF(315, 32));   //Set bounds to draw multiline text
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set

            ///RECUADRO No CAJA
            RectangleF recBordeNocaja = new RectangleF(recBordeSubserie.X, recBordeSubserie.Y + recBordeSubserie.Height, 408 * fx, 14 * fy);
            graphicsPag.DrawRectangle(borde, recBordeNocaja);
            element.HTMLText = "No. DE CAJA";
            element.TextAlign = TextAlign.Center;
            bounds = new RectangleF(new PointF(recBordeNocaja.X + 2, recBordeNocaja.Y + 2), new SizeF(recBordeNocaja.Width - 4, recBordeNocaja.Height - 4));
            //bounds = new RectangleF(new PointF(recBorde.X + 4, recBorde.Y + 2), new SizeF(400, 10));   //Set bounds to draw multiline text
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set

            //CODIGO de BARRAS
            RectangleF recBordeBarras = new RectangleF(recBordeNocaja.X, recBordeNocaja.Y + recBordeNocaja.Height, 408 * fx, 56 * fy);
            graphicsPag.DrawRectangle(borde, recBordeBarras);
            fontBarcode = new PdfStandardFont(PdfFontFamily.Helvetica, 11 * fl, PdfFontStyle.Bold);//Set the standard font.
            barcode.BarHeight = 52 * fy;//Barcode
            barcode.Font = fontBarcode;
            barcode.Text = nro_caja.PadLeft(5, '0');
            barcode.Size = new SizeF(250 * fx, barcode.BarHeight);
            barcode.Draw(page, new PointF(96 * fx, recBordeBarras.Y + 3));//Printing barcode on to the Pdf.

            //BUCLE PARA CALCULAR DATOS SIGUIENTES
            DateTime fechaInicial = DateTime.MaxValue;
            DateTime fechaFinal = DateTime.MinValue;
            DateTime vrfechaIni, vrfechaFin;
            string txtqrCaja = "CJ: " + nro_caja + ".";
            foreach (var item in datFUID2.OrderBy(x => x.nro_carpeta).ToList())
            {
                //Console.WriteLine(item.fecha_expediente_ini.ToString());
                vrfechaIni = item.fecha_expediente_ini ?? DateTime.MinValue;
                vrfechaFin = item.fecha_expediente_fin ?? DateTime.MinValue;
                if (vrfechaIni != DateTime.MinValue && vrfechaIni < fechaInicial) fechaInicial = vrfechaIni;
                if (vrfechaFin > fechaFinal) fechaFinal = vrfechaFin;
                txtqrCaja = txtqrCaja + "- KP: " + item.nro_carpeta + " TOMO: " + item.tomo + " DE " + item.tomo + " " + item.nom_expediente;
            }
            vrfechaIni = dateDefecto; //VALIDA FECHA POR DEFECTO
            if (vrfechaIni > fechaInicial) fechaInicial = vrfechaIni;


            //RECUADRO SIGNATURA
            RectangleF recBordeSignatura = new RectangleF(recBordeBarras.X, recBordeBarras.Y + recBordeBarras.Height, 408 * fx, 28 * fy);
            graphicsPag.DrawRectangle(borde, recBordeSignatura);
            //Fondo GRIS
            PdfLinearGradientBrush brush = new PdfLinearGradientBrush(new PointF(0, 0), new PointF(1, 1), Color.Red, Color.Blue);
            brush.Background = Color.FromArgb(226, 226, 226);
            bounds = new RectangleF(new PointF(recBorde.X + 64.5f * fx, recBordeSignatura.Y + 0.5f / fy), new SizeF(343f * fx, 10f * fy));
            graphicsPag.DrawRectangle(brush, bounds);

            PdfFont fontTitleMin = new PdfStandardFont(PdfFontFamily.Helvetica, 7.5f * fl);
            //Rectangulo
            RectangleF recUbicacion = new RectangleF(new PointF(recBordeSignatura.X, recBordeSignatura.Y), new SizeF(64f * fx, recBordeSignatura.Height));
            graphicsPag.DrawRectangle(borde, recUbicacion);
            element.HTMLText = "UBICACIÓN TOPOGRÁFICA";
            element.Font = fontTitleMin;
            element.TextAlign = TextAlign.Center;
            bounds = new RectangleF(new PointF(recUbicacion.X + 2, recUbicacion.Y + 4), new SizeF(recUbicacion.Width - 4, recUbicacion.Height - 6));
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set

            //Rectangulo BODEGA
            RectangleF recBodega = new RectangleF(new PointF(recUbicacion.X + recUbicacion.Width, recBordeSignatura.Y), new SizeF(46.4f * fx, recBordeSignatura.Height));
            graphicsPag.DrawRectangle(borde, recBodega);
            //Rect Titulo
            RectangleF recBodegaTitulo = new RectangleF(new PointF(recBodega.X, recBordeSignatura.Y), new SizeF(recBodega.Width, 10 * fy));
            graphicsPag.DrawRectangle(borde, recBodegaTitulo);
            //titulo Bodega
            bounds = new RectangleF(new PointF(recBodegaTitulo.X + 0.5f, recBodegaTitulo.Y + 0.5f), new SizeF(recBodegaTitulo.Width - 1, recBodegaTitulo.Height - 1));
            PdfTextElement eleText = new PdfTextElement("BODEGA");
            //PdfFont fontlabel = new PdfStandardFont(PdfFontFamily.Helvetica,  2);//Set the standard font.
            eleText.Font = fontTitleMin;
            eleText.StringFormat = formatoTxtCentrado;
            eleText.Brush = new PdfSolidBrush(Color.Black);
            eleText.Draw(page, bounds);

            //Rect Cuerpo
            RectangleF recCuerpotitulo = new RectangleF(new PointF(recBodega.X + recBodega.Width, recBordeSignatura.Y), new SizeF(46.4f * fx, 10 * fy));
            graphicsPag.DrawRectangle(borde, recCuerpotitulo);
            bounds = new RectangleF(new PointF(recCuerpotitulo.X + 0.5f, recCuerpotitulo.Y + 0.5f), new SizeF(recCuerpotitulo.Width - 1, recCuerpotitulo.Height - 1));
            //titulo Cuerpo
            eleText = new PdfTextElement("CUERPO");
            //PdfFont fontlabel = new PdfStandardFont(PdfFontFamily.Helvetica,  2);//Set the standard font.
            eleText.Font = fontTitleMin;
            eleText.StringFormat = formatoTxtCentrado;
            eleText.Brush = new PdfSolidBrush(Color.Black);
            eleText.Draw(page, bounds);

            //Rectangulo TORRE
            RectangleF recTorre = new RectangleF(new PointF(recCuerpotitulo.X + recCuerpotitulo.Width, recBordeSignatura.Y), new SizeF(46.4f * fx, recBordeSignatura.Height));
            graphicsPag.DrawRectangle(borde, recTorre);
            //Rect Titulo
            RectangleF recTorreTitulo = new RectangleF(new PointF(recTorre.X, recBordeSignatura.Y), new SizeF(recTorre.Width, 10 * fy));
            graphicsPag.DrawRectangle(borde, recTorreTitulo);
            bounds = new RectangleF(new PointF(recTorreTitulo.X + 0.5f, recTorreTitulo.Y + 0.5f), new SizeF(recTorreTitulo.Width - 1, recTorreTitulo.Height - 1));
            //titulo Bodega
            eleText = new PdfTextElement("TORRE");
            //PdfFont fontlabel = new PdfStandardFont(PdfFontFamily.Helvetica,  2);//Set the standard font.
            eleText.Font = fontTitleMin;
            eleText.StringFormat = formatoTxtCentrado;
            eleText.Brush = new PdfSolidBrush(Color.Black);
            eleText.Draw(page, bounds);

            //Rect Cuerpo PISO
            RectangleF recPisoTitulo = new RectangleF(new PointF(recTorre.X + recTorre.Width, recBordeSignatura.Y), new SizeF(46.4f * fx, 10 * fy));
            graphicsPag.DrawRectangle(borde, recPisoTitulo);
            bounds = new RectangleF(new PointF(recPisoTitulo.X + 0.5f, recPisoTitulo.Y + 0.5f), new SizeF(recPisoTitulo.Width - 1, recPisoTitulo.Height - 1));
            //titulo Cuerpo
            eleText = new PdfTextElement("PISO");
            //PdfFont fontlabel = new PdfStandardFont(PdfFontFamily.Helvetica,  2);//Set the standard font.
            eleText.Font = fontTitleMin;
            eleText.StringFormat = formatoTxtCentrado;
            eleText.Brush = new PdfSolidBrush(Color.Black);
            eleText.Draw(page, bounds);

            //Rectangulo NIVEL
            RectangleF recNivel = new RectangleF(new PointF(recPisoTitulo.X + recPisoTitulo.Width, recBordeSignatura.Y), new SizeF(46.4f * fx, recBordeSignatura.Height));
            graphicsPag.DrawRectangle(borde, recNivel);
            //Rect Titulo
            RectangleF recNivelTitulo = new RectangleF(new PointF(recNivel.X, recBordeSignatura.Y), new SizeF(recNivel.Width, 10 * fy));
            graphicsPag.DrawRectangle(borde, recNivelTitulo);
            bounds = new RectangleF(new PointF(recNivelTitulo.X + 0.5f, recNivelTitulo.Y + 0.5f), new SizeF(recNivelTitulo.Width - 1, recNivelTitulo.Height - 1));
            //titulo Nivel
            eleText = new PdfTextElement("NIVEL");
            //PdfFont fontlabel = new PdfStandardFont(PdfFontFamily.Helvetica,  2);//Set the standard font.
            eleText.Font = fontTitleMin;
            eleText.StringFormat = formatoTxtCentrado;
            eleText.Brush = new PdfSolidBrush(Color.Black);
            eleText.Draw(page, bounds);

            //Rect Cuerpo PASILLO
            RectangleF recPasilloTitulo = new RectangleF(new PointF(recNivel.X + recNivel.Width, recBordeSignatura.Y), new SizeF(46.4f * fx, 10 * fy));
            graphicsPag.DrawRectangle(borde, recPasilloTitulo);
            bounds = new RectangleF(new PointF(recPasilloTitulo.X + 0.5f, recPasilloTitulo.Y + 0.5f), new SizeF(recPasilloTitulo.Width - 1, recPasilloTitulo.Height - 1));
            //titulo Cuerpo
            eleText = new PdfTextElement("PASILLO");
            //PdfFont fontlabel = new PdfStandardFont(PdfFontFamily.Helvetica,  2);//Set the standard font.
            eleText.Font = fontTitleMin;
            eleText.StringFormat = formatoTxtCentrado;
            eleText.Brush = new PdfSolidBrush(Color.Black);
            eleText.Draw(page, bounds);

            //Rectangulo POSICION CJ
            RectangleF rcPosicioncj = new RectangleF(new PointF(recPasilloTitulo.X + recPasilloTitulo.Width, recBordeSignatura.Y), new SizeF(65.5f * fx, recBordeSignatura.Height));
            graphicsPag.DrawRectangle(borde, rcPosicioncj);
            //Rect Titulo
            RectangleF rcPosicioncjTitulo = new RectangleF(new PointF(rcPosicioncj.X, recBordeSignatura.Y), new SizeF(rcPosicioncj.Width, 10 * fy));
            graphicsPag.DrawRectangle(borde, rcPosicioncjTitulo);
            bounds = new RectangleF(new PointF(rcPosicioncjTitulo.X + 0.5f, rcPosicioncjTitulo.Y + 0.5f), new SizeF(rcPosicioncjTitulo.Width - 1, rcPosicioncjTitulo.Height - 1));
            //titulo Bodega
            eleText = new PdfTextElement("POSICION CJ");
            //PdfFont fontlabel = new PdfStandardFont(PdfFontFamily.Helvetica,  2);//Set the standard font.
            eleText.Font = fontTitleMin;
            eleText.StringFormat = formatoTxtCentrado;
            eleText.Brush = new PdfSolidBrush(Color.Black);
            eleText.Draw(page, bounds);


            //TEXTO QR
            txtqrCaja = txtqrCaja.Replace('Á', 'A');
            txtqrCaja = txtqrCaja.Replace('É', 'E');
            txtqrCaja = txtqrCaja.Replace('Í', 'I');
            txtqrCaja = txtqrCaja.Replace('Ó', 'O');
            txtqrCaja = txtqrCaja.Replace('Ú', 'U');
            txtqrCaja = txtqrCaja.Replace('Ñ', 'N');
            txtqrCaja = Regex.Replace(txtqrCaja, @"[^0-9a-zA-Z,-]+", " ");    //[^0-9a-zA-Z:\u00C0-\u00FF,|._-Ññ]+
            //Console.WriteLine(barcodeQr.Text);

            ///RECUADRO QR
            RectangleF recBordeQR = new RectangleF(recBordeSignatura.X, recBordeSignatura.Y + recBordeSignatura.Height, 408 * fx, 255 * fy);
            graphicsPag.DrawRectangle(borde, recBordeQR);
            ////QR NUEVA VERSIÓN
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeGenerator.ECCLevel nivelQR = QRCodeGenerator.ECCLevel.M;
            int lonTxtQR = txtqrCaja.Length;
            if (lonTxtQR < 512) nivelQR = QRCodeGenerator.ECCLevel.H;
            if (lonTxtQR > 1024) nivelQR = QRCodeGenerator.ECCLevel.L;
            if (lonTxtQR > 2816)
            {
                txtqrCaja = txtqrCaja.Substring(0, Math.Min(txtqrCaja.Length, 2816));
            }
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(txtqrCaja, nivelQR);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(21);
            PdfBitmap image2 = new PdfBitmap(qrCodeImage);
            RectangleF imageBounds2 = new RectangleF(106 * fx, recBordeQR.Y + 1, 240 * fx, 240 * fx);//Setting image bounds
            graphicsPag.DrawImage(image2, imageBounds2);//Draw the image

            graphicsPag.DrawString("Developed by Alpha Intelligence AI", fontpie, PdfBrushes.Black, new PointF(50 * fx, recBordeQR.Y + recBordeQR.Height - 7 * fy), formatoTxtPie);

            ///RECUADRO FECHAS EXTREMAS
            RectangleF recBordeFechas = new RectangleF(recBordeQR.X, recBordeQR.Y + recBordeQR.Height, 408 * fx, 14 * fy);
            graphicsPag.DrawRectangle(borde, recBordeFechas);
            //DATO
            element.HTMLText = "FECHAS EXTREMAS (dd/mm/aaaa):     " + fechaInicial.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + " - " + fechaFinal.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            element.TextAlign = TextAlign.Center;
            bounds = new RectangleF(new PointF(recBordeFechas.X + 4, recBordeFechas.Y + 2), new SizeF(400 * fx, 10 * fy));
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set

            ///AÑOS
            RectangleF recBordeAnios = new RectangleF(recBordeFechas.X, recBordeFechas.Y + recBordeFechas.Height, 408 * fx, 14 * fy);
            graphicsPag.DrawRectangle(borde, recBordeAnios);
            element.HTMLText = "AÑOS (aaaa):                                        " + fechaInicial.ToString("yyyy", CultureInfo.InvariantCulture) + " - " + fechaFinal.ToString("yyyy", CultureInfo.InvariantCulture);
            bounds = new RectangleF(new PointF(recBordeAnios.X + 4, recBordeAnios.Y + 2), new SizeF(400 * fx, 10 * fy));   //Set bounds to draw multiline text
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set

            var filemane = $"{exportFolderPath}" + "/Caja-" + nro_caja + ".pdf";
            //Save the document.
            doc.Save(filemane);
            //Close the document.
            doc.Close(true);
            exportedList.Add(filemane);
        }

        public void ExportPfdCaja3(int codigo, string nomLote, int int_caja, string exportFolderPath, ref List<string> exportedList, p_formato dataFormato) //"codigo" puede ser cualquier número, se le pregunta al usuario en el momento de generar el formato
        {
            if (codigo == 0) codigo++;
            if (int_caja == 0) return;
            string cj_titulo1 = "", cj_titulo2 = "", cj_titulo3 = "", cj_cal_codigo = "", cj_cal_version = "", cj_cal_fecha = "", nomOrg = string.Empty, nomDependencia = string.Empty, codDependencia = string.Empty, nomSubdepen = string.Empty, codSubdepen = string.Empty, nomSerie = string.Empty, codSerie = string.Empty, nomSubserie = string.Empty, codSubserie = string.Empty;
            DateTime dateDefecto = DateTime.MinValue;

            var qEncabezado = EntitiesRepository.Entities.t_carpeta.Include("t_lote").Include("t_lote").Include("p_subserie").Include("p_serie").Include("p_subdependencia").Include("p_dependencia").Include("p_organizacion").Where(c => c.int_caja == int_caja);
            //Texto organización
            //nomOrg = qEncabezado.FirstOrDefault().t_lote.p_proyecto.p_organizacion?.nombre;
            //Texto Dependencia
            foreach (var item in qEncabezado.Select(m => m.t_lote.p_subdependencia.p_dependencia).Distinct().ToList())
            {
                if (string.IsNullOrEmpty(codDependencia)) codDependencia = item.codigo;
                else codDependencia += " - " + item.codigo;
                if (string.IsNullOrEmpty(nomDependencia)) nomDependencia = item.nombre;
                else nomDependencia += " - " + item.nombre;
            }
            //Texto SubDependencia
            foreach (var item in qEncabezado.Select(m => m.t_lote.p_subdependencia).Distinct().ToList())
            {
                if (string.IsNullOrEmpty(codSubdepen)) codSubdepen = item.cod;
                else codSubdepen += " - " + item.cod;
                if (string.IsNullOrEmpty(nomSubdepen)) nomSubdepen = item.nombre;
                else nomSubdepen += " - " + item.nombre;
            }
            //Texto Serie
            foreach (var item in qEncabezado.Select(m => new { m.t_lote.p_subserie.p_serie.codigo, m.t_lote.p_subserie.p_serie.nombre }).Distinct().ToList())
            {
                if (string.IsNullOrEmpty(codSerie)) codSerie = item.codigo;
                else codSerie += " - " + item.codigo;
                if (string.IsNullOrEmpty(nomSerie)) nomSerie = item.nombre;
                else nomSerie += " - " + item.nombre;
            }
            //Texto Sub Serie
            foreach (var item in qEncabezado.Select(m => m.t_lote.p_subserie).Distinct().ToList())
            {
                if (string.IsNullOrEmpty(codSubserie)) codSubserie = item.codigo;
                else codSubserie += " - " + item.codigo;
                if (string.IsNullOrEmpty(nomSubserie)) nomSubserie = item.nombre;
                else nomSubserie += " - " + item.nombre;
            }

            IQueryable<t_documento> datFUID = EntitiesRepository.Entities.t_documento.Include("t_carpeta").Include("t_lote").AsNoTracking().Where(p => p.t_carpeta.t_lote.id_proyecto == GlobalClass.id_proyecto && p.t_carpeta.int_caja == int_caja && p.nro_doc != null).OrderBy(p => p.t_carpeta.id);
            var datFUID2 = datFUID.Select(p => new { p.fecha, p.nro_doc, p.t_carpeta.nro_carpeta });

            //var dataFormato = datFUID2.FirstOrDefault().p_formato.FirstOrDefault();

            int idProyecto = 0;

            if (dataFormato != null)
            {
                p_organizacion dataOrg;
                dateDefecto = dataFormato.fecha_inicial_defecto ?? DateTime.MinValue;
                idProyecto = dataFormato.p_proyecto.id;
                //Nombre de la organización
                dataOrg = dataFormato.p_proyecto.p_organizacion;
                nomOrg = dataOrg.nombre;
                if (!string.IsNullOrEmpty(dataFormato.cj_titulo1)) cj_titulo1 = dataFormato.cj_titulo1;
                if (!string.IsNullOrEmpty(dataFormato.cj_titulo2)) cj_titulo2 = dataFormato.cj_titulo2;
                if (!string.IsNullOrEmpty(dataFormato.cj_titulo3)) cj_titulo3 = dataFormato.hc_titulo3;
                if (!string.IsNullOrEmpty(dataFormato.cj_cal_codigo)) cj_cal_codigo = dataFormato.cj_cal_codigo;
                if (!string.IsNullOrEmpty(dataFormato.cj_cal_version)) cj_cal_version = dataFormato.cj_cal_version;
                if (!string.IsNullOrEmpty(dataFormato.cj_cal_fecha)) cj_cal_fecha = dataFormato.cj_cal_fecha;
            }

            //Create a new PDF document.
            PdfDocument doc = new PdfDocument();
            doc.PageSettings.Orientation = PdfPageOrientation.Portrait;
            doc.PageSettings.Size = PdfPageSize.Letter; //Width = 612 Height = 792
            doc.PageSettings.Margins.Top = 1;
            doc.PageSettings.Margins.Left = 1;
            PdfPage page = doc.Pages.Add();//Add a page to the document.
            PdfGraphics graphicsPag = page.Graphics;

            //Rectangulo para recortar
            PdfPen bordePunteado = new PdfPen(Color.Black, 1);
            bordePunteado.DashStyle = PdfDashStyle.DashDot;
            RectangleF recBordeTest = new RectangleF(0.5f, 0.5f, 510, 708);
            graphicsPag.DrawRectangle(bordePunteado, recBordeTest);
            //Factor de escala
            float fx = 1.18f;
            float fy = 1.09f;
            float fl = (float)Math.Round(fy, 1, MidpointRounding.ToEven);
            //FOOTER
            PdfStringFormat formatoTxtPie = new PdfStringFormat(); formatoTxtPie.Alignment = PdfTextAlignment.Center;
            PdfFont fontpie = new PdfStandardFont(PdfFontFamily.Helvetica, 5.5f);//Set the standard font.

            //ENCABEZADO
            PdfStringFormat formatoTxtCentrado = new PdfStringFormat(); formatoTxtCentrado.Alignment = PdfTextAlignment.Center;
            PdfStringFormat formatoTxtIzquierda = new PdfStringFormat(); formatoTxtIzquierda.Alignment = PdfTextAlignment.Left;
            PdfFont fontBarcode = new PdfStandardFont(PdfFontFamily.Helvetica, 0, PdfFontStyle.Bold);//Set the standard font.
            PdfCode39Barcode barcode = new PdfCode39Barcode();//Drawing Code39 barcode
            barcode.Font = fontBarcode;
            ///RECUADROS SUPERIORES
            //RectangleF recBorde = new RectangleF(14f, 14f, 114*fx, 42*fy);
            PdfPen borde = new PdfPen(Color.Black, 1);  //ecBorde = new RectangleF((float)0.5, recBorde.Height + 6, 97, 42);
            PdfPen bordeRojo = new PdfPen(Color.Red, 1);
            PdfPen bordeAzul = new PdfPen(Color.Blue, 1);
            PdfPen bordeVerde = new PdfPen(Color.Green, 1);

            RectangleF recBorde = new RectangleF(14f, 14f, 408 * fx, 42 * fy);
            graphicsPag.DrawRectangle(borde, recBorde);
            PdfBitmap image;
            RectangleF imageBounds = new RectangleF(recBorde.X + 6, recBorde.Y + 10, 95 * fx, 22 * fy);//Setting image bounds
            if (File.Exists("logo_" + idProyecto + ".png")) //Logo
            {
                image = new PdfBitmap("logo_" + idProyecto + ".png");
                graphicsPag.DrawImage(image, imageBounds);//Draw the image
            }
            ////////TITULO

            PdfFont fontTitulo = new PdfStandardFont(PdfFontFamily.Helvetica, 7 * fl, PdfFontStyle.Bold);//Set the standard font.
            RectangleF recBordeT = new RectangleF(118f * fx, recBorde.Y, 215f * fx, 15 * fy);

            PdfLinearGradientBrush brush = new PdfLinearGradientBrush(new PointF(0, 0), new PointF(1, 1), Color.Red, Color.Blue);
            brush.Background = Color.FromArgb(51, 102, 204);
            RectangleF recTitulo = new RectangleF(170.5f, 0.5f, 280, 14); //Borde
            graphicsPag.DrawRectangle(brush, recBordeT);
            graphicsPag.DrawRectangle(borde, recBordeT);

            //Rectangulos titulos
            //graphicsPag.DrawRectangle(borde, recBordeT);

            graphicsPag.DrawString(cj_titulo1, fontTitulo, PdfBrushes.White, new PointF((recBordeT.X + recBordeT.Width / 2), recBorde.Y + 4 * fy), formatoTxtCentrado);//Draw the text.
            recBordeT = new RectangleF(recBordeT.X, recBorde.Y + recBordeT.Height, recBordeT.Width, 13 * fy);
            graphicsPag.DrawRectangle(borde, recBordeT);
            graphicsPag.DrawString(cj_titulo2, fontTitulo, PdfBrushes.Black, new PointF((recBordeT.X + recBordeT.Width / 2), recBorde.Y + 17 * fy), formatoTxtCentrado);//Draw the text.
            recBordeT = new RectangleF(recBordeT.X, recBordeT.Y + recBordeT.Height, recBordeT.Width, 14 * fy);
            graphicsPag.DrawRectangle(borde, recBordeT);
            graphicsPag.DrawString(cj_titulo3, fontTitulo, PdfBrushes.Black, new PointF((recBordeT.X + recBordeT.Width / 2), recBorde.Y + 31 * fy), formatoTxtCentrado);//Draw the text.
            ////////CALIDAD
            //PdfFont fontFormatoNegrita = new PdfStandardFont(PdfFontFamily.Helvetica, 5, PdfFontStyle.Bold);
            graphicsPag.DrawString(cj_cal_codigo, fontTitulo, PdfBrushes.Black, new PointF(recBordeT.X + recBordeT.Width + 5, recBorde.Y + 4 * fy), formatoTxtIzquierda);
            graphicsPag.DrawString(cj_cal_version, fontTitulo, PdfBrushes.Black, new PointF(recBordeT.X + recBordeT.Width + 5, recBorde.Y + 13 * fy), formatoTxtIzquierda);
            graphicsPag.DrawString(cj_cal_fecha, fontTitulo, PdfBrushes.Black, new PointF(recBordeT.X + recBordeT.Width + 5, recBorde.Y + 22 * fy), formatoTxtIzquierda);
            PdfPageNumberField pageNumber = new PdfPageNumberField();   //Conteo de PÁGINAS
            PdfPageCountField count = new PdfPageCountField();
            PdfCompositeField compositeField = new PdfCompositeField(fontTitulo, PdfBrushes.Black, "Páginas {0} de {1}", pageNumber, count);
            compositeField.StringFormat = formatoTxtIzquierda;
            compositeField.Draw(graphicsPag, new PointF(recBordeT.X + recBordeT.Width + 5, recBorde.Y + 31 * fy));

            ///RECUADRO ORGANIZACIÓN
            RectangleF recBordeOrg = new RectangleF(recBorde.X, recBorde.Y + recBorde.Height, 408 * fx, 42 * fy);
            graphicsPag.DrawRectangle(borde, recBordeOrg);

            PdfFont fontOrg = new PdfStandardFont(PdfFontFamily.Helvetica, 14 * fl);
            PdfHTMLTextElement element = new PdfHTMLTextElement();  //Create a text element  //element.Brush = new PdfSolidBrush(Color.Black);
            element.HTMLText = nomOrg?.Trim();
            element.Font = fontOrg;
            element.TextAlign = TextAlign.Center;
            PdfMetafileLayoutFormat layoutFormat = new PdfMetafileLayoutFormat();
            layoutFormat.Layout = PdfLayoutType.Paginate;
            //PdfLayoutFormat layoutFormat = new PdfLayoutFormat(); //Set the properties to paginate the text
            layoutFormat.Break = PdfLayoutBreakType.FitPage;
            RectangleF bounds = new RectangleF(new PointF(recBordeOrg.X + 2, recBordeOrg.Y + 5), new SizeF(recBordeOrg.Width - 4, recBordeOrg.Height - 7));
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set

            ///RECUADRO DEPENDENCIA
            RectangleF recBordeDepen = new RectangleF(recBordeOrg.X, recBordeOrg.Y + recBordeOrg.Height, 85 * fx, 40 * fy);
            graphicsPag.DrawRectangle(borde, recBordeDepen);
            RectangleF recBordeDepenDato = new RectangleF(recBordeDepen.X + recBordeDepen.Width, recBordeDepen.Y, 323 * fx, recBordeDepen.Height);
            graphicsPag.DrawRectangle(borde, recBordeDepenDato);
            //Titulo
            PdfFont fontTitulos = new PdfStandardFont(PdfFontFamily.Helvetica, 9 * fl);
            element.HTMLText = "SECCIÓN - UNIDAD ADMINISTRATIVA Y CÓDIGO:";
            element.Font = fontTitulos; element.TextAlign = TextAlign.Left;
            bounds = new RectangleF(new PointF(recBordeDepen.X + 2, recBordeDepen.Y + 5), new SizeF(recBordeDepen.Width - 4, recBordeDepen.Height - 7));
            element.Draw(page, bounds, layoutFormat);
            //Dato
            element.HTMLText = "<b>" + nomDependencia + " - " + codDependencia + "</b>";
            element.Font = fontTitulos; element.TextAlign = TextAlign.Center;
            bounds = new RectangleF(new PointF(recBordeDepenDato.X + 2, recBordeDepenDato.Y + 12), new SizeF(recBordeDepenDato.Width - 4, recBordeDepenDato.Height - 14));
            element.Draw(page, bounds, layoutFormat);


            ///RECUADRO SUBSECCIÓN
            RectangleF recBordeSeccion = new RectangleF(recBordeDepen.X, recBordeDepen.Y + recBordeDepen.Height, 85 * fx, 40 * fy);
            graphicsPag.DrawRectangle(borde, recBordeSeccion);
            RectangleF recBordeSeccionDato = new RectangleF(recBordeSeccion.X + recBordeSeccion.Width, recBordeSeccion.Y, 323 * fx, recBordeSeccion.Height);
            graphicsPag.DrawRectangle(borde, recBordeSeccionDato);
            //Titulo
            PdfFont fontTitle = new PdfStandardFont(PdfFontFamily.Helvetica, 9 * fl);
            element.HTMLText = "SUBSECCIÓN - OFICINA PRODUCTORA Y CÓDIGO:";
            element.Font = fontTitle; element.TextAlign = TextAlign.Left;
            bounds = new RectangleF(new PointF(recBordeSeccion.X + 2, recBordeSeccion.Y + 3), new SizeF(recBordeSeccion.Width - 4, recBordeSeccion.Height - 5));
            //bounds = new RectangleF(new PointF(4, recBorde.Y + 3), new SizeF(77, 36));   //Set bounds to draw multiline text
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set
            //DATO
            element.HTMLText = "<b>" + nomSubdepen + " - " + codSubdepen + "</b>"; element.TextAlign = TextAlign.Center;
            //bounds = new RectangleF(new PointF(recBorde.X + 89, recBorde.Y + 13), new SizeF(315, 26));   //Set bounds to draw multiline text
            bounds = new RectangleF(new PointF(recBordeSeccionDato.X + 2, recBordeSeccionDato.Y + 13), new SizeF(recBordeSeccionDato.Width - 4, recBordeSeccionDato.Height - 15));
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set


            ///RECUADRO CODIGO SERIE Y CODIGO
            RectangleF recBordeSerie = new RectangleF(recBordeSeccion.X, recBordeSeccion.Y + recBordeSeccion.Height, 85 * fx, 40 * fy);
            graphicsPag.DrawRectangle(borde, recBordeSerie);
            RectangleF recBordeSerieDato = new RectangleF(recBordeSerie.X + recBordeSerie.Width, recBordeSerie.Y, 323 * fx, recBordeSerie.Height);
            graphicsPag.DrawRectangle(borde, recBordeSerieDato);
            element.HTMLText = "SERIE Y CÓDIGO";
            element.Font = fontTitle;
            element.TextAlign = TextAlign.Left;
            bounds = new RectangleF(new PointF(recBordeSerie.X + 2, recBordeSerie.Y + 13), new SizeF(recBordeSerie.Width - 4, recBordeSerie.Height - 15));
            //bounds = new RectangleF(new PointF(4, recBorde.Y + 13), new SizeF(77, 26));   //Set bounds to draw multiline text
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set
            element.HTMLText = "<b>" + nomSerie + " - " + codSerie + "</b>";
            element.TextAlign = TextAlign.Center;
            //bounds = new RectangleF(new PointF(recBorde.X + 89, recBorde.Y + 6), new SizeF(315, 26));   //Set bounds to draw multiline text
            bounds = new RectangleF(new PointF(recBordeSerieDato.X + 2, recBordeSerieDato.Y + 6), new SizeF(recBordeSerieDato.Width - 4, recBordeSerieDato.Height - 8));
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set

            ///RECUADRO CODIGO SUBSERIE Y CODIGO
            RectangleF recBordeSubserie = new RectangleF(recBordeSerie.X, recBordeSerie.Y + recBordeSerie.Height, 85 * fx, 40 * fy);
            graphicsPag.DrawRectangle(borde, recBordeSubserie);
            RectangleF recBordeSubserieDato = new RectangleF(recBordeSubserie.X + recBordeSubserie.Width, recBordeSubserie.Y, 323 * fx, recBordeSubserie.Height);
            graphicsPag.DrawRectangle(borde, recBordeSubserieDato);
            element.HTMLText = "SUBSERIE Y CÓDIGO";
            element.TextAlign = TextAlign.Left;
            bounds = new RectangleF(new PointF(recBordeSubserie.X + 2, recBordeSubserie.Y + 8), new SizeF(recBordeSubserie.Width - 4, recBordeSubserie.Height - 10));
            //bounds = new RectangleF(new PointF(4, recBorde.Y + 8), new SizeF(77, 26));   //Set bounds to draw multiline text
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set
            element.HTMLText = "<b>" + nomSubserie + " - " + codSubserie + "</b>";
            element.TextAlign = TextAlign.Center;
            bounds = new RectangleF(new PointF(recBordeSubserieDato.X + 2, recBordeSubserieDato.Y + 4), new SizeF(recBordeSubserieDato.Width - 4, recBordeSubserieDato.Height - 6));
            //bounds = new RectangleF(new PointF(recBorde.X + 89, recBorde.Y + 4), new SizeF(315, 32));   //Set bounds to draw multiline text
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set

            ///RECUADRO No CAJA
            RectangleF recBordeNocaja = new RectangleF(recBordeSubserie.X, recBordeSubserie.Y + recBordeSubserie.Height, 408 * fx, 14 * fy);
            graphicsPag.DrawRectangle(borde, recBordeNocaja);
            element.HTMLText = "No. DE CAJA";
            element.TextAlign = TextAlign.Center;
            bounds = new RectangleF(new PointF(recBordeNocaja.X + 2, recBordeNocaja.Y + 2), new SizeF(recBordeNocaja.Width - 4, recBordeNocaja.Height - 4));
            //bounds = new RectangleF(new PointF(recBorde.X + 4, recBorde.Y + 2), new SizeF(400, 10));   //Set bounds to draw multiline text
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set

            //CODIGO de BARRAS
            string nro_caja = "CJ" + int_caja.ToString().PadLeft(7, '0');
            RectangleF recBordeBarras = new RectangleF(recBordeNocaja.X, recBordeNocaja.Y + recBordeNocaja.Height, 408 * fx, 56 * fy);
            graphicsPag.DrawRectangle(borde, recBordeBarras);
            fontBarcode = new PdfStandardFont(PdfFontFamily.Helvetica, 11 * fl, PdfFontStyle.Bold);//Set the standard font.
            barcode.BarHeight = 52 * fy;//Barcode
            barcode.Font = fontBarcode;
            barcode.Text = nro_caja;
            barcode.Size = new SizeF(250 * fx, barcode.BarHeight);
            barcode.Draw(page, new PointF(96 * fx, recBordeBarras.Y + 3));//Printing barcode on to the Pdf.

            //BUCLE PARA CALCULAR DATOS SIGUIENTES
            int? nroKPactual = -1; string nroFUDini = string.Empty, nroFUDfin = string.Empty;
            DateTime fechaInicial = DateTime.MaxValue;
            DateTime fechaFinal = DateTime.MinValue;
            DateTime vrfecha = DateTime.MinValue;
            string txtqrCaja = nro_caja + ": ";
            foreach (var item in datFUID2.ToList())
            {
                vrfecha = item.fecha ?? DateTime.MinValue;
                if (vrfecha != DateTime.MinValue && fechaInicial == DateTime.MaxValue) fechaInicial = vrfecha;
                if (vrfecha != DateTime.MinValue) fechaFinal = vrfecha;

                if (nroKPactual != -1 && nroKPactual != item.nro_carpeta)
                {
                    txtqrCaja += "| KP" + nroKPactual + "  " + nroFUDini + " - " + nroFUDfin;
                    nroFUDini = string.Empty;
                    nroFUDfin = string.Empty;
                    nroKPactual = item.nro_carpeta;
                }
                else
                {
                    if (!string.IsNullOrEmpty(item.nro_doc))
                    {
                        if (string.IsNullOrEmpty(nroFUDini)) nroFUDini = item.nro_doc;
                        nroFUDfin = item.nro_doc;
                    }
                    nroKPactual = item.nro_carpeta;
                }
            }
            if (!string.IsNullOrEmpty(nroFUDini)) txtqrCaja += "| KP" + nroKPactual + "  " + nroFUDini + " - " + nroFUDfin;

            //RECUADRO SIGNATURA
            RectangleF recBordeSignatura = new RectangleF(recBordeBarras.X, recBordeBarras.Y + recBordeBarras.Height, 408 * fx, 28 * fy);
            graphicsPag.DrawRectangle(borde, recBordeSignatura);
            //Fondo GRIS
            PdfLinearGradientBrush brush2 = new PdfLinearGradientBrush(new PointF(0, 0), new PointF(1, 1), Color.Red, Color.Blue);
            brush.Background = Color.FromArgb(226, 226, 226);
            bounds = new RectangleF(new PointF(recBorde.X + 64.5f * fx, recBordeSignatura.Y + 0.5f / fy), new SizeF(343f * fx, 10f * fy));
            graphicsPag.DrawRectangle(brush2, bounds);

            PdfFont fontTitleMin = new PdfStandardFont(PdfFontFamily.Helvetica, 7.5f * fl);
            //Rectangulo
            RectangleF recUbicacion = new RectangleF(new PointF(recBordeSignatura.X, recBordeSignatura.Y), new SizeF(64f * fx, recBordeSignatura.Height));
            graphicsPag.DrawRectangle(borde, recUbicacion);
            element.HTMLText = "UBICACIÓN TOPOGRÁFICA";
            element.Font = fontTitleMin;
            element.TextAlign = TextAlign.Center;
            bounds = new RectangleF(new PointF(recUbicacion.X + 2, recUbicacion.Y + 4), new SizeF(recUbicacion.Width - 4, recUbicacion.Height - 6));
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set

            //Rectangulo BODEGA
            RectangleF recBodega = new RectangleF(new PointF(recUbicacion.X + recUbicacion.Width, recBordeSignatura.Y), new SizeF(46.4f * fx, recBordeSignatura.Height));
            graphicsPag.DrawRectangle(borde, recBodega);
            //Rect Titulo
            RectangleF recBodegaTitulo = new RectangleF(new PointF(recBodega.X, recBordeSignatura.Y), new SizeF(recBodega.Width, 10 * fy));
            graphicsPag.DrawRectangle(borde, recBodegaTitulo);
            //titulo Bodega
            bounds = new RectangleF(new PointF(recBodegaTitulo.X + 0.5f, recBodegaTitulo.Y + 0.5f), new SizeF(recBodegaTitulo.Width - 1, recBodegaTitulo.Height - 1));
            PdfTextElement eleText = new PdfTextElement("BODEGA");
            //PdfFont fontlabel = new PdfStandardFont(PdfFontFamily.Helvetica,  2);//Set the standard font.
            eleText.Font = fontTitleMin;
            eleText.StringFormat = formatoTxtCentrado;
            eleText.Brush = new PdfSolidBrush(Color.Black);
            eleText.Draw(page, bounds);

            //Rect Cuerpo
            RectangleF recCuerpotitulo = new RectangleF(new PointF(recBodega.X + recBodega.Width, recBordeSignatura.Y), new SizeF(46.4f * fx, 10 * fy));
            graphicsPag.DrawRectangle(borde, recCuerpotitulo);
            bounds = new RectangleF(new PointF(recCuerpotitulo.X + 0.5f, recCuerpotitulo.Y + 0.5f), new SizeF(recCuerpotitulo.Width - 1, recCuerpotitulo.Height - 1));
            //titulo Cuerpo
            eleText = new PdfTextElement("CUERPO");
            //PdfFont fontlabel = new PdfStandardFont(PdfFontFamily.Helvetica,  2);//Set the standard font.
            eleText.Font = fontTitleMin;
            eleText.StringFormat = formatoTxtCentrado;
            eleText.Brush = new PdfSolidBrush(Color.Black);
            eleText.Draw(page, bounds);

            //Rectangulo TORRE
            RectangleF recTorre = new RectangleF(new PointF(recCuerpotitulo.X + recCuerpotitulo.Width, recBordeSignatura.Y), new SizeF(46.4f * fx, recBordeSignatura.Height));
            graphicsPag.DrawRectangle(borde, recTorre);
            //Rect Titulo
            RectangleF recTorreTitulo = new RectangleF(new PointF(recTorre.X, recBordeSignatura.Y), new SizeF(recTorre.Width, 10 * fy));
            graphicsPag.DrawRectangle(borde, recTorreTitulo);
            bounds = new RectangleF(new PointF(recTorreTitulo.X + 0.5f, recTorreTitulo.Y + 0.5f), new SizeF(recTorreTitulo.Width - 1, recTorreTitulo.Height - 1));
            //titulo Bodega
            eleText = new PdfTextElement("TORRE");
            //PdfFont fontlabel = new PdfStandardFont(PdfFontFamily.Helvetica,  2);//Set the standard font.
            eleText.Font = fontTitleMin;
            eleText.StringFormat = formatoTxtCentrado;
            eleText.Brush = new PdfSolidBrush(Color.Black);
            eleText.Draw(page, bounds);

            //Rect Cuerpo PISO
            RectangleF recPisoTitulo = new RectangleF(new PointF(recTorre.X + recTorre.Width, recBordeSignatura.Y), new SizeF(46.4f * fx, 10 * fy));
            graphicsPag.DrawRectangle(borde, recPisoTitulo);
            bounds = new RectangleF(new PointF(recPisoTitulo.X + 0.5f, recPisoTitulo.Y + 0.5f), new SizeF(recPisoTitulo.Width - 1, recPisoTitulo.Height - 1));
            //titulo Cuerpo
            eleText = new PdfTextElement("PISO");
            //PdfFont fontlabel = new PdfStandardFont(PdfFontFamily.Helvetica,  2);//Set the standard font.
            eleText.Font = fontTitleMin;
            eleText.StringFormat = formatoTxtCentrado;
            eleText.Brush = new PdfSolidBrush(Color.Black);
            eleText.Draw(page, bounds);

            //Rectangulo NIVEL
            RectangleF recNivel = new RectangleF(new PointF(recPisoTitulo.X + recPisoTitulo.Width, recBordeSignatura.Y), new SizeF(46.4f * fx, recBordeSignatura.Height));
            graphicsPag.DrawRectangle(borde, recNivel);
            //Rect Titulo
            RectangleF recNivelTitulo = new RectangleF(new PointF(recNivel.X, recBordeSignatura.Y), new SizeF(recNivel.Width, 10 * fy));
            graphicsPag.DrawRectangle(borde, recNivelTitulo);
            bounds = new RectangleF(new PointF(recNivelTitulo.X + 0.5f, recNivelTitulo.Y + 0.5f), new SizeF(recNivelTitulo.Width - 1, recNivelTitulo.Height - 1));
            //titulo Nivel
            eleText = new PdfTextElement("NIVEL");
            //PdfFont fontlabel = new PdfStandardFont(PdfFontFamily.Helvetica,  2);//Set the standard font.
            eleText.Font = fontTitleMin;
            eleText.StringFormat = formatoTxtCentrado;
            eleText.Brush = new PdfSolidBrush(Color.Black);
            eleText.Draw(page, bounds);

            //Rect Cuerpo PASILLO
            RectangleF recPasilloTitulo = new RectangleF(new PointF(recNivel.X + recNivel.Width, recBordeSignatura.Y), new SizeF(46.4f * fx, 10 * fy));
            graphicsPag.DrawRectangle(borde, recPasilloTitulo);
            bounds = new RectangleF(new PointF(recPasilloTitulo.X + 0.5f, recPasilloTitulo.Y + 0.5f), new SizeF(recPasilloTitulo.Width - 1, recPasilloTitulo.Height - 1));
            //titulo Cuerpo
            eleText = new PdfTextElement("PASILLO");
            //PdfFont fontlabel = new PdfStandardFont(PdfFontFamily.Helvetica,  2);//Set the standard font.
            eleText.Font = fontTitleMin;
            eleText.StringFormat = formatoTxtCentrado;
            eleText.Brush = new PdfSolidBrush(Color.Black);
            eleText.Draw(page, bounds);

            //Rectangulo POSICION CJ
            RectangleF rcPosicioncj = new RectangleF(new PointF(recPasilloTitulo.X + recPasilloTitulo.Width, recBordeSignatura.Y), new SizeF(65.5f * fx, recBordeSignatura.Height));
            graphicsPag.DrawRectangle(borde, rcPosicioncj);
            //Rect Titulo
            RectangleF rcPosicioncjTitulo = new RectangleF(new PointF(rcPosicioncj.X, recBordeSignatura.Y), new SizeF(rcPosicioncj.Width, 10 * fy));
            graphicsPag.DrawRectangle(borde, rcPosicioncjTitulo);
            bounds = new RectangleF(new PointF(rcPosicioncjTitulo.X + 0.5f, rcPosicioncjTitulo.Y + 0.5f), new SizeF(rcPosicioncjTitulo.Width - 1, rcPosicioncjTitulo.Height - 1));
            //titulo Bodega
            eleText = new PdfTextElement("POSICION CJ");
            //PdfFont fontlabel = new PdfStandardFont(PdfFontFamily.Helvetica,  2);//Set the standard font.
            eleText.Font = fontTitleMin;
            eleText.StringFormat = formatoTxtCentrado;
            eleText.Brush = new PdfSolidBrush(Color.Black);
            eleText.Draw(page, bounds);

            ///RECUADRO QR
            RectangleF recBordeQR = new RectangleF(recBordeSignatura.X, recBordeSignatura.Y + recBordeSignatura.Height, 408 * fx, 255 * fy);
            graphicsPag.DrawRectangle(borde, recBordeQR);

            string txtQRFinal = Regex.Replace(txtqrCaja, @"[^0-9a-zA-Z,Ó|.-_-Ññ]+", " ");
            int lonTxtQR = txtQRFinal.Length;
            //////QR VERSIÓN ANERIOR
            //PdfQRBarcode barcodeQr = new PdfQRBarcode();//Drawing QR Barcode
            //barcodeQr.ErrorCorrectionLevel = PdfErrorCorrectionLevel.Medium;//Set Error Correction Level
            //barcodeQr.XDimension = 3;//Set XDimension
            //barcodeQr.Size = new SizeF(250 * fx, 250 * fy);
            ////if (lonTxtQR < 512) barcodeQr.ErrorCorrectionLevel = PdfErrorCorrectionLevel.High;
            //if (lonTxtQR > 1024) barcodeQr.ErrorCorrectionLevel = PdfErrorCorrectionLevel.Low;
            //barcodeQr.Text = Regex.Replace(txtqrCaja, @"[^0-9a-zA-Z,Ó|.-_-Ññ]+", " ");
            //Console.WriteLine(barcodeQr.Text);
            //barcodeQr.Draw(page, new PointF(101 * fx, recBordeQR.Y + 2));//Printing barcode on to the Pdf.

            ////QR NUEVA VERSIÓN
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeGenerator.ECCLevel nivelQR = QRCodeGenerator.ECCLevel.M;
            if (lonTxtQR > 1024) nivelQR = QRCodeGenerator.ECCLevel.L;
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(txtQRFinal, nivelQR);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(21);
            PdfBitmap image2 = new PdfBitmap(qrCodeImage);
            RectangleF imageBounds2 = new RectangleF(106 * fx, recBordeQR.Y + 1, 240 * fx, 240 * fx);//Setting image bounds
            graphicsPag.DrawImage(image2, imageBounds2);//Draw the image

            graphicsPag.DrawString("Developed by Alpha Intelligence AI", fontpie, PdfBrushes.Black, new PointF(50 * fx, recBordeQR.Y + recBordeQR.Height - 7 * fy), formatoTxtPie);

            ///RECUADRO FECHAS EXTREMAS
            RectangleF recBordeFechas = new RectangleF(recBordeQR.X, recBordeQR.Y + recBordeQR.Height, 408 * fx, 14 * fy);
            graphicsPag.DrawRectangle(borde, recBordeFechas);
            //DATO
            element.HTMLText = "FECHAS EXTREMAS (dd/mm/aaaa):     " + fechaInicial.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + " - " + fechaFinal.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            element.TextAlign = TextAlign.Center;
            bounds = new RectangleF(new PointF(recBordeFechas.X + 4, recBordeFechas.Y + 2), new SizeF(400 * fx, 10 * fy));
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set

            ///AÑOS
            RectangleF recBordeAnios = new RectangleF(recBordeFechas.X, recBordeFechas.Y + recBordeFechas.Height, 408 * fx, 14 * fy);
            graphicsPag.DrawRectangle(borde, recBordeAnios);
            element.HTMLText = "AÑOS (aaaa):                                        " + fechaInicial.ToString("yyyy", CultureInfo.InvariantCulture) + " - " + fechaFinal.ToString("yyyy", CultureInfo.InvariantCulture);
            bounds = new RectangleF(new PointF(recBordeAnios.X + 4, recBordeAnios.Y + 2), new SizeF(400 * fx, 10 * fy));   //Set bounds to draw multiline text
            element.Draw(page, bounds, layoutFormat);   //Draw the text element with the properties and formats set

            var filemane = $"{exportFolderPath}" + "/Caja-" + nro_caja + ".pdf";
            //Save the document.
            doc.Save(filemane);
            //Close the document.
            doc.Close(true);

            var match = exportedList.FirstOrDefault(stringToCheck => stringToCheck.Contains(filemane));

            // in this case out testItem.Id (1) is equal to an item in the list
            if (match == null)
            {
                exportedList.Add(filemane);
            }
            
        }

        private void buildRectangle(double x, double y, double width, double height,
    String titulo, PdfFont font, PdfStringFormat stringFormat, PdfPage page, PdfPen borde)
        {
            //Limite del texto
            RectangleF rectangulo = new RectangleF((float)x, (float)y, (float)width, (float)height);
            page.Graphics.DrawRectangle(borde, rectangulo);

            //Titulo Celda
            PdfTextElement sbTitulo = new PdfTextElement(titulo);
            sbTitulo.Font = font;
            sbTitulo.StringFormat = stringFormat;
            sbTitulo.Brush = new PdfSolidBrush(Color.Black);
            PdfLayoutFormat layoutFormat = new PdfLayoutFormat();
            layoutFormat.Break = PdfLayoutBreakType.FitPage;
            RectangleF bounds1 = new RectangleF((float)x + 3, (float)y, (float)width - 6, (float)height);
            sbTitulo.Draw(page, bounds1, layoutFormat);
        }
        //_exportService.Indice1(IndiceArchivoMaestro iaMaestro, List<IndiceArchivoDetalle> ListDetalle,string Ruta);
        public void Indice1(IndiceArchivoMaestro iaMaestro, List<IndiceArchivoDetalle> ListDetalle, string Ruta)
        {
            string ciudad = string.Empty, despacho = string.Empty, subserie = string.Empty, nroRadicacion = string.Empty, parteA = string.Empty, parteB = string.Empty, Otro = string.Empty, cuaderno = string.Empty, totalCuardernos = string.Empty;
            float factor = 3;
            var wb = new XLWorkbook(@"resources\FINDICE_1.xlsx");   //EXCEL
            var ws = wb.Worksheet("Indice Electrónico");            //ESCEL
            PdfDocument doc = new PdfDocument();
            doc.PageSettings.Orientation = PdfPageOrientation.Portrait;
            doc.PageSettings.Size = PdfPageSize.Legal; //{ Width = 612 Height = 1008}  216 x 356
            doc.PageSettings.Margins.Top = 10;
            doc.PageSettings.Margins.Right = 11;
            doc.PageSettings.Margins.Bottom = 1;
            doc.PageSettings.Margins.Left = 11;
            PdfPage page = doc.Pages.Add();//Add a page to the document.
            PdfGraphics graphics = page.Graphics;
            PdfPen borde = new PdfPen(Color.Black, 1);

            PdfStringFormat formatoTxtIzquierda = new PdfStringFormat(); formatoTxtIzquierda.Alignment = PdfTextAlignment.Left; formatoTxtIzquierda.LineAlignment = PdfVerticalAlignment.Middle;
            PdfStringFormat formatoTxtCentrado = new PdfStringFormat(); formatoTxtCentrado.Alignment = PdfTextAlignment.Center; formatoTxtCentrado.LineAlignment = PdfVerticalAlignment.Middle;
            PdfFont fontNegrita = new PdfStandardFont(PdfFontFamily.Helvetica, 5, PdfFontStyle.Bold);//Set the standard font.
            PdfFont fontNTitulo = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);//Set the standard font.
            ////////LOGO
            if (File.Exists($@"logo_{GlobalClass.id_proyecto}.png"))
            {
                PdfBitmap image = new PdfBitmap($@"logo_{GlobalClass.id_proyecto}.png");
                RectangleF imageBounds = new RectangleF(17, 2, 116, 36);//Setting image bounds
                graphics.DrawImage(image, imageBounds);//Draw the image
            }
            ciudad = iaMaestro.ciudad; despacho = iaMaestro.despacho; subserie = iaMaestro.SerieSubserie;  
            parteA = iaMaestro.ParteA; parteB = iaMaestro.ParteB;
            nroRadicacion = iaMaestro.NroRadicacion;
            cuaderno = iaMaestro.Cuaderno;
            totalCuardernos = iaMaestro.NroCarpeta;

            string[] wordsNumExp = iaMaestro.NroRadicacion?.Trim().Split('_');
            if (wordsNumExp.Length > 0)
            {
                nroRadicacion = wordsNumExp[0];
                if(wordsNumExp.Length > 1)cuaderno = GlobalClass.GetNumber(wordsNumExp[1], 1).ToString();
                
                int totalTomos = EntitiesRepository.Entities.t_carpeta.Where(x => x.nro_expediente.StartsWith(nroRadicacion)).Count();
                totalCuardernos = totalTomos.ToString();
            }

            //Encabezado
            float ancho = doc.PageSettings.Size.Width - doc.PageSettings.Margins.Right - doc.PageSettings.Margins.Left;
            buildRectangle(0, 0, 150, 40, "", fontNegrita, formatoTxtIzquierda, page, borde);
            buildRectangle(150, 0, ancho - 150, 40, "ÍNDICE DEL EXPEDIENTE JUDICIAL ELECTRÓNICO", fontNTitulo, formatoTxtCentrado, page, borde);

            float anchoEncabezadoC3 = 100f; float anchoEncabezadoC4 = 90f;
            float anchoEncabezadoC1 = 110f; float anchoEncabezadoC2 = ancho - anchoEncabezadoC1 - 20 - anchoEncabezadoC3 - anchoEncabezadoC4;
            float altoFilaEncabezado = 12f;
            PdfFont font_6 = new PdfStandardFont(PdfFontFamily.Helvetica, 6);
            PdfFont font_6N = new PdfStandardFont(PdfFontFamily.Helvetica, 6, PdfFontStyle.Bold);
            PdfFont font_7 = new PdfStandardFont(PdfFontFamily.Helvetica, 7);
            PdfFont font_7N = new PdfStandardFont(PdfFontFamily.Helvetica, 7, PdfFontStyle.Bold);
            buildRectangle(0, 50, anchoEncabezadoC1, altoFilaEncabezado, "Ciudad", font_6N, formatoTxtIzquierda, page, borde);
            buildRectangle(anchoEncabezadoC1, 50, anchoEncabezadoC2, altoFilaEncabezado, ciudad, font_7, formatoTxtIzquierda, page, borde);
            buildRectangle(0, 50 + (altoFilaEncabezado * 1), anchoEncabezadoC1, altoFilaEncabezado, "Despacho Judicial", font_6N, formatoTxtIzquierda, page, borde);
            buildRectangle(anchoEncabezadoC1, 50 + (altoFilaEncabezado * 1), anchoEncabezadoC2, altoFilaEncabezado, despacho, font_7, formatoTxtIzquierda, page, borde);
            buildRectangle(0, 50 + (altoFilaEncabezado * 2), anchoEncabezadoC1, altoFilaEncabezado, "Serie o Subserie Documental", font_6N, formatoTxtIzquierda, page, borde);
            buildRectangle(anchoEncabezadoC1, 50 + (altoFilaEncabezado * 2), anchoEncabezadoC2, altoFilaEncabezado, subserie, font_7, formatoTxtIzquierda, page, borde);
            buildRectangle(0, 50 + (altoFilaEncabezado * 3), anchoEncabezadoC1, altoFilaEncabezado, "No. Radicación del Proceso", font_6N, formatoTxtIzquierda, page, borde);
            buildRectangle(anchoEncabezadoC1, 50 + (altoFilaEncabezado * 3), anchoEncabezadoC2, altoFilaEncabezado, $@"{nroRadicacion} " , font_7, formatoTxtIzquierda, page, borde);
            buildRectangle(0, 50 + (altoFilaEncabezado * 4), anchoEncabezadoC1, altoFilaEncabezado * 2, "Partes Procesales (Parte A) (demandado, procesado, accionado)", font_6N, formatoTxtIzquierda, page, borde);
            buildRectangle(anchoEncabezadoC1, 50 + (altoFilaEncabezado * 4), anchoEncabezadoC2, altoFilaEncabezado * 2, parteA, font_7, formatoTxtIzquierda, page, borde);
            buildRectangle(0, 50 + (altoFilaEncabezado * 6), anchoEncabezadoC1, altoFilaEncabezado * 2, "Partes Procesales (Parte B) (demandante, denunciante, accionante)", font_6N, formatoTxtIzquierda, page, borde);
            buildRectangle(anchoEncabezadoC1, 50 + (altoFilaEncabezado * 6), anchoEncabezadoC2, altoFilaEncabezado * 2, parteB, font_7, formatoTxtIzquierda, page, borde);
            buildRectangle(0, 50 + (altoFilaEncabezado * 8), anchoEncabezadoC1, altoFilaEncabezado, "Cuaderno", font_6N, formatoTxtIzquierda, page, borde);
            buildRectangle(anchoEncabezadoC1, 50 + (altoFilaEncabezado * 8), anchoEncabezadoC2, altoFilaEncabezado, cuaderno, font_7, formatoTxtIzquierda, page, borde);
            //buildRectangle(0, 50 + (altoFilaEncabezado * 9), anchoEncabezadoC1, altoFilaEncabezado, "Cuaderno", font_6N, formatoTxtIzquierda, page, borde);
            //buildRectangle(anchoEncabezadoC1, 50 + (altoFilaEncabezado * 9), anchoEncabezadoC2, altoFilaEncabezado, cuaderno, font_7, formatoTxtIzquierda, page, borde);

            //Encabezado DOS
            buildRectangle(ancho - anchoEncabezadoC4 - anchoEncabezadoC3, 50, anchoEncabezadoC3 + anchoEncabezadoC4, altoFilaEncabezado, "EXPEDIENTE FÍSICO", font_7N, formatoTxtCentrado, page, borde);
            buildRectangle(ancho - anchoEncabezadoC4 - anchoEncabezadoC3, 50 + (altoFilaEncabezado * 1), anchoEncabezadoC3, altoFilaEncabezado * 2, "El expediente judicial posee documentos físicos:", font_6, formatoTxtIzquierda, page, borde);
            buildRectangle(ancho - anchoEncabezadoC4, 50 + (altoFilaEncabezado * 1), anchoEncabezadoC4, altoFilaEncabezado * 2, "SI: __X__     NO: ____", font_6, formatoTxtCentrado, page, borde);
            buildRectangle(ancho - anchoEncabezadoC4 - anchoEncabezadoC3, 50 + (altoFilaEncabezado * 3), anchoEncabezadoC3, altoFilaEncabezado * 2, "No. de carpetas, legajos o tomos:", font_6, formatoTxtIzquierda, page, borde);
            buildRectangle(ancho - anchoEncabezadoC4, 50 + (altoFilaEncabezado * 3), anchoEncabezadoC4, altoFilaEncabezado * 2, totalCuardernos, font_6, formatoTxtCentrado, page, borde);
            //TABLA
            var anchoHead = new List<int>();
            anchoHead.Add(140);  //0 Nombre Documento
            anchoHead.Add(49);  //1 Fecha Creación Documento
            anchoHead.Add(49);  //2 Fecha Incorporación Expediente
            anchoHead.Add(39);  //3 Orden Documento
            anchoHead.Add(33);  //4 Número Páginas
            anchoHead.Add(33);
            anchoHead.Add(33);  //6 Página Fin
            anchoHead.Add(45);  //7 Formato
            anchoHead.Add(51);  //8 Tamaño
            anchoHead.Add(51);  //9 Origen
            anchoHead.Add(65);  //10 Observaciones

            DataTable table = new DataTable();// Initialize DataTable to assign as DateSource to the light table.
            table.Columns.Add("Nombre Documento");//Include columns to the DataTable.
            table.Columns.Add("Fecha Creación Documento");
            table.Columns.Add("Fecha Incorporación Expediente");
            table.Columns.Add("Orden Documento");
            table.Columns.Add("Número Páginas");
            table.Columns.Add("Página Inicio");
            table.Columns.Add("Página Final");
            table.Columns.Add("Formato");
            table.Columns.Add("Tamaño KB");
            table.Columns.Add("Origen");
            table.Columns.Add("Observaciones");
            for (int c = 0; c < ListDetalle.Count; c++)
            {
                float tamanio = ListDetalle[c].tamanio / 1024;
                table.Rows.Add(new string[] { ListDetalle[c].NombreDocumento, ListDetalle[c].fechaCreacion.ToString("dd/M/yyyy", CultureInfo.InvariantCulture), ListDetalle[c].fechaIncorporacion.ToString("dd/M/yyyy", CultureInfo.InvariantCulture), ListDetalle[c].orden.ToString(), ListDetalle[c].TotalPaginas.ToString(), ListDetalle[c].PaginaInicio.ToString(), ListDetalle[c].PaginaFin.ToString(), ListDetalle[c].Formato, tamanio.ToString(), ListDetalle[c].Origen, ListDetalle[c].Observaciones });//Include rows to the DataTable.
            }

            PdfGrid pdfGrid = new PdfGrid();
            pdfGrid.DataSource = table;
            for (int i = 0; i < anchoHead.Count; i++)
            {
                pdfGrid.Columns[i].Width = anchoHead[i];
            }

            //Estilo Header Tabla
            PdfGridCellStyle headerstyle = new PdfGridCellStyle();
            headerstyle.Font = font_6N;
            headerstyle.StringFormat = formatoTxtCentrado;
            headerstyle.BackgroundBrush = PdfBrushes.AliceBlue;
            pdfGrid.Headers.ApplyStyle(headerstyle);    //Apply style 

            //Estilo de fila en Table
            PdfGridCellStyle gridCellStyle = new PdfGridCellStyle();     //Initialize PdfGridCellStyle. Set background color and string format
            gridCellStyle.StringFormat = formatoTxtCentrado;
            gridCellStyle.Font = font_6;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                PdfGridRow gridRow = pdfGrid.Rows[i];
                gridRow.ApplyStyle(gridCellStyle);
            }
            //pdfGrid.BeginCellLayout += PdfGrid_BeginCellLayoutFUID;

            PdfGridLayoutResult pdfGridLayoutResult = pdfGrid.Draw(page, new PointF(0, 50 + (altoFilaEncabezado * 11)));    //Draw grid to the page of PDF document

            int pageCount = doc.Pages.Count;
            var lastPage = doc.Pages[pageCount - 1];//Identifica última Hoja
            var lastpagGraphics = lastPage.Graphics;
            var altoFinal = pdfGridLayoutResult.Bounds.Bottom + 10;
            if (altoFinal > 795f)
            {
                altoFinal = 7f;
                doc.Pages.Add();
                pageCount = doc.Pages.Count;
                lastPage = doc.Pages[pageCount - 1];
                lastpagGraphics = lastPage.Graphics;
            }

            if (File.Exists($@"firma_{GlobalClass.id_proyecto}.png"))
            {
                PdfBitmap image = new PdfBitmap($@"firma_{GlobalClass.id_proyecto}.png");
                RectangleF imageBounds = new RectangleF(17, altoFinal, 100, 36);//Setting image bounds
                lastpagGraphics.DrawImage(image, imageBounds);//Draw the image
            }

            doc.Save($@"{Ruta}/00_ÍNDICE DEL EXPEDIENTE JUDICIAL ELECTRÓNICO.pdf");
            //Close the document.
            doc.Close(true);
            //GENERA EXCEL
            //ciudad,despacho,subserie,nroRadicacion,parteA,parteB,cuaderno,totalCuardernos
            ws.Cell("B2").Value = ciudad;
            ws.Cell("B3").Value = despacho;
            ws.Cell("B4").Value = subserie;
            ws.Cell("B5").Value = "'"+nroRadicacion.ToString().Trim();
            ws.Cell("B6").Value = parteA;
            ws.Cell("B7").Value = parteB;
            ws.Cell("B8").Value = cuaderno;
            ws.Cell("J5").Value = totalCuardernos;
            for (int c = 0; c < ListDetalle.Count; c++)
            {
                float tamanio = ListDetalle[c].tamanio / 1024;
                ws.Cell(c + 11, "A").Value = ListDetalle[c].NombreDocumento;
                ws.Cell(c + 11, "B").Value = ListDetalle[c].fechaCreacion.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
                ws.Cell(c + 11, "C").Value = ListDetalle[c].fechaIncorporacion.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
                ws.Cell(c + 11, "D").Value = ListDetalle[c].orden.ToString();
                ws.Cell(c + 11, "E").Value = ListDetalle[c].TotalPaginas.ToString();
                ws.Cell(c + 11, "F").Value = ListDetalle[c].PaginaInicio.ToString();
                ws.Cell(c + 11, "G").Value = ListDetalle[c].PaginaFin.ToString();
                ws.Cell(c + 11, "H").Value = ListDetalle[c].Formato;
                ws.Cell(c + 11, "I").Value = tamanio.ToString();
                ws.Cell(c + 11, "J").Value = ListDetalle[c].Origen;
                ws.Cell(c + 11, "K").Value = ListDetalle[c].Observaciones;
            }
            wb.SaveAs($@"{Ruta}/00_ÍNDICE DEL EXPEDIENTE JUDICIAL ELECTRÓNICO.xlsx");
        }

    }
}