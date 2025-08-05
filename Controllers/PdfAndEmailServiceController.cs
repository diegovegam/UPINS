using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using System.Net;
using System.Net.Mail;
using UPINS.Models.ViewModels;


namespace UPINS.Controllers
{
    public class PdfAndEmailServiceController
    {

        public void SendEmailWithPdfAsync(Byte[] pdfFile)
        {
            var pdfBytes = pdfFile;

            var message = new MailMessage();
            message.To.Add("diegovega_2501@hotmail.com");
            message.Subject = "Factura Cafeteria UPINS";
            message.Body = "Por favor revise la factura adjunta.";
            message.Attachments.Add(new Attachment(new MemoryStream(pdfBytes), "Invoice.pdf", "application/pdf"));
            message.From = new MailAddress("MS_wu1vXn@test-65qngkdo7p3lwr12.mlsender.net", "Cafetería UPINS");

            using var smtp = new SmtpClient("smtp.mailersend.net");
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("MS_wu1vXn@test-65qngkdo7p3lwr12.mlsender.net", "mssp.S6gvCfD.x2p03473dyygzdrn.w0YYid3");
            smtp.Send(message);
        }
        public async Task GeneratePdfAndSendEmail (BillViewModel billViewModel, ControllerContext controllerContext)
        {
            var pdfResult = new ViewAsPdf("ViewBillToSendViaEmail", billViewModel)
            {
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };

            var pdfBytes = await pdfResult.BuildFile(controllerContext);
            SendEmailWithPdfAsync(pdfBytes);
        }
    }
}
