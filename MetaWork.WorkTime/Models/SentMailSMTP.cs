using MetaWork.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace MetaWork.WorkTime.Models
{
    public class SentMailSMTP
    {
        #region Member
        public string HostName { get; set; }
        public int Port { get; set; } = 25;
        public bool UseSSL { get; set; } = false;
        public string UserName { get; set; }
        public string Password { get; set; }
        #endregion

        #region Constructor
        public SentMailSMTP()
        {

        }
        #endregion

        #region PrivateMethod

        private SmtpClient getsmtpClient()
        {
            try
            {                
                return new SmtpClient()
                {
                    Host = "mail.tecotec.com.vn",
                    Port = 25,
                    EnableSsl = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential("sourcing@tecotec.com.vn", "Tecotec@19962021"),
                };
            }
            catch
            {
                return new SmtpClient()
                {
                    Host = "mail.tecotec.com.vn",
                    Port = 25,
                    EnableSsl = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential("sourcing@tecotec.com.vn", "Tecotec@19962021"),
                };
            }
        }
        private void addDefaultForm(ref MailMessage value)
        {
            if (value.From == null || string.IsNullOrEmpty(value.From.Address))
            {
                value.From = new MailAddress(UserName);
            }
        }
        #endregion

        #region ISentMailModel
        public SendEmailResult SendMail(MailMessage value)
        {
            bool bStatus = false;
            string mesage = string.Empty;
            try
            {
                using (var client = getsmtpClient())
                {
                    addDefaultForm(ref value);
                    client.Send(value);
                }
                bStatus = true;
            }
            catch (Exception ex)
            {
                bStatus = false;
                mesage = "Error:" + ex.Message;
            }
            return new SendEmailResult { Status = bStatus, Message = mesage };
        }
        public SendEmailResult SendMail(MailMessage value, string mailBoxFolder)
        {
            bool bStatus = false;
            string mesage = string.Empty;
            try
            {
                using (var client = getsmtpClient())
                {
                    //client.DeliveryFormat = SmtpDeliveryFormat.
                    //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    //client.PickupDirectoryLocation = mailBoxFolder;
                    //client.EnableSsl = true;

                    addDefaultForm(ref value);
                    client.Send(value);

                }
                bStatus = true;
            }
            catch (Exception ex)
            {
                bStatus = false;
                mesage = "Error:" + ex.Message;
            }
            return new SendEmailResult { Status = bStatus, Message = mesage };
        }
        public async Task SendMailAsync(MailMessage value)
        {
            using (var client = getsmtpClient())
            {
                addDefaultForm(ref value);
                await client.SendMailAsync(value);
            }
        }
        #endregion
    }
}